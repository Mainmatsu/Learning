using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private List<Texture2D> MelList;
        private Texture2D mouse;

        private CatcherHolder m_cHolder;

        MediaLibrary mediaLibrary; // Грубо говоря "проигрыватель"
        Song song; // Сама музыка
        VisualizationData visualizationData;

        SpriteFont font;

        private int scores = 0; // очки
        private float self_size = 1f; // размер "игрока"
        private int xsize = 1; // множитель очков

        private float power = 0f; // переменная для пурпурного бонуса
        private float activity = 0f; // переменная для активности игрока

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            // Создаем переменные
            mediaLibrary = new MediaLibrary();
            visualizationData = new VisualizationData();

            scores = 0;

        }

        protected override void Initialize()
        {
            MelList = new List<Texture2D>();

            for (int a = 2; a <= 6; a++)
                MelList.Add(Content.Load<Texture2D>(a.ToString()));

            m_cHolder = new CatcherHolder(MelList);

            MediaPlayer.Play(Content.Load<Song>("1")); // начинаем играть музыку
            MediaPlayer.IsVisualizationEnabled = true; // включаем визуализатор

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            MelList = new List<Texture2D>();

            for (int a = 2; a <= 6; a++)
                MelList.Add(Content.Load<Texture2D>(a.ToString()));

            mouse = Content.Load<Texture2D>("Mouse");

            song = Content.Load<Song>("1");

            font = Content.Load<SpriteFont>("SpriteFont1");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            m_cHolder.Update();
            MediaPlayer.GetVisualizationData(visualizationData); // получаем данные 
            // "Прогоняем" массив с частотами, выполняем условия
            for (int a = 0; a < 128; a++)
            {
                if (visualizationData.Frequencies[a] > Constant.BEAT_REACTION && m_cHolder.accomulator[a] > Constant.ACCOMULATOR_REACTION)
                {
                    m_cHolder.Beat(a * 3.125f * 2); // вызываем "бит", которые создает частичку.
                    m_cHolder.accomulator[a] -= Constant.BEAT_COST; // убавляем аккумулятор
                }
            }
            // проверяем, есть ли бонус, который тянет к игроку все ноты
            if (power > 0f)
            {
                for (int particle = 0; particle < m_cHolder.particles.Count; particle++)
                {
                    if (m_cHolder.particles[particle].type != 1) // если не враг, то тянем
                    {
                        float body1X = m_cHolder.particles[particle].Position.X;
                        float body1Y = m_cHolder.particles[particle].Position.Y;
                        float body2X = (float)Mouse.GetState().X;
                        float body2Y = (float)Mouse.GetState().Y;

                        float Angle = (float)Math.Atan2(body2X - body1X, body2Y - body1Y) - ((float)Math.PI / 2.0f); // находим угол к игроку

                        float Lenght = (float)(5000f * power) / (float)Math.Pow((float)Distance(body1X, body1Y, body2X, body2Y), 2.0f); // находим силу

                        m_cHolder.particles[particle].ApplyImpulse(AngleToV2(Angle, Lenght)); // даем пинка ноте
                    }
                }
                power -= 0.001f; // убавляем бонус
            }

            activity -= 0.001f; // убавляем активность игрока

            if (activity < 0.0f)
                activity = 0.0f;
            else if (activity > 0.5f) activity = 0.5f;
            // Держим активность игрока от 0f до .5f

            // Проверяем столкновения двух кругов: игрока и нот
            for (int particle = 0; particle < m_cHolder.particles.Count; particle++)
            {
                int x = (int)m_cHolder.particles[particle].Position.X;
                int y = (int)m_cHolder.particles[particle].Position.Y;
                int radius = (int)(16f * m_cHolder.particles[particle].Size);

                if (circlesColliding(Mouse.GetState().X, Mouse.GetState().Y, (int)(16f * self_size), x, y, radius))
                {

                    scores += (int)(10f * m_cHolder.particles[particle].Size * xsize); // добавляем очки, которые зависят от размера ноты и множителя
                    activity += 0.005f; // добавляем активность
                    int type = m_cHolder.particles[particle].type;

                    // выполняем всякие условия, которые возникают при коллизии
                    switch (type)
                    {
                        case 3: // желтый
                            self_size += 0.1f;
                            xsize += 1;

                            // увеличиваем множитель и размер игрока
                            if (self_size > 4.0f)
                                self_size = 4.0f;
                            break;

                        case 2: // пурпурный
                            power = 1f; // даем бонус игроку, который все ноты притягивает к себе

                            break;

                        case 4: // мигающий
                            for (int b = 0; b < m_cHolder.particles.Count; b++)
                                m_cHolder.particles[b].SetType(3); // устанавливает всем нотам тип — желтый


                            break;

                        case 1: // красный (враг)
                            for (int a = 1; a < xsize; a++)
                                m_cHolder.GenerateYellowExplossion(Mouse.GetState().X, Mouse.GetState().Y, (int)(16f * self_size));

                            xsize = 1;
                            self_size = 1f;
                            scores -= (int)(scores / 4);
                            break;

                    }


                    // удаляем частичку
                    m_cHolder.particles[particle].TTL = 0;
                    m_cHolder.particles.RemoveAt(particle);
                    particle--;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            m_cHolder.Draw(spriteBatch); // рисуем CatcherHolder

            spriteBatch.Begin();

            Rectangle sourceRectangle = new Rectangle(0, 0, mouse.Width, mouse.Height); // размеры текстуры
            Vector2 origin = new Vector2(mouse.Width / 2, mouse.Height / 2); // offset текстуры
            Vector2 mouse_vector = new Vector2(Mouse.GetState().X, Mouse.GetState().Y); // вектор(позиция) мышки

            string xtext = "x" + xsize.ToString(); // текст
            Vector2 text_vector = font.MeasureString(xtext) / 2.0f; // вычисления offset'a текста

            spriteBatch.Draw(mouse, mouse_vector, sourceRectangle, new Color(0.5f - power / 2.0f + activity, 0.5f, 0.5f - power / 2.0f), 0.0f, origin, self_size, SpriteEffects.None, 0f); // рисуем игрока
            spriteBatch.DrawString(font, xtext, mouse_vector - text_vector, Color.White); // рисуем множитель

            spriteBatch.DrawString(font, "Score: " + scores.ToString(), new Vector2(5, graphics.PreferredBackBufferHeight - 34), Color.White); // рисуем очки
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // возвращает, столкнулись ли два круга или нет
        bool circlesColliding(int x1, int y1, int radius1, int x2, int y2, int radius2)
        {

            int dx = x2 - x1;
            int dy = y2 - y1;
            int radii = radius1 + radius2;
            if ((dx * dx) + (dy * dy) < radii * radii)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // функция перевода угла в вектор
        public Vector2 AngleToV2(float angle, float length)
        {
            Vector2 direction = Vector2.Zero;
            direction.X = (float)Math.Cos(angle) * length;
            direction.Y = -(float)Math.Sin(angle) * length;
            return direction;
        }

        // дистанция
        public float Distance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt((float)Math.Pow(x2 - x1, 2) + (float)Math.Pow(y2 - y1, 2));
        }
    }
}