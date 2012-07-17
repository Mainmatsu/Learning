using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame
{
    class Catcher
    {
        public Texture2D Texture { get; set; }        // Текстура частицы
        public Vector2 Position { get; set; }        // Позиция частицы
        public Vector2 Velocity { get; set; }        // Скорость частицы
        public float Angle { get; set; }            // Угол поворота частицы
        public float AngularVelocity { get; set; }    // Угловая скорость частицы
        public Color Color { get; set; }            // Цвет частицы
        public float Size { get; set; }                // Размер частицы
        public int TTL { get; set; }                // Время жизни частицы

        private float RComponent; // Красный компонент RGB
        private float GComponent; // Зеленый компонент RGB
        private float BComponent; // Синий компонент RGB
        public int type; // Тип частицы
        private Random random; // Генератор случайных чисел

        public Catcher(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, int type, float size, int ttl)
        {
            // Установка переменных из конструктора
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            this.type = type;
            Size = size;
            TTL = ttl;

            SetType(type); // Установка цвета под определенный тип

        }

        public void ApplyImpulse(Vector2 vector) // Добавление импульса (используется бонусом)
        {
            Velocity += vector;
        }

        public void Update() // Обновление единичной частички
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;

            if (type != -1)
            {
                Velocity = new Vector2(Velocity.X, Velocity.Y - .1f);
                Size = (10 + Velocity.Y) / 20;
                if (Size > 0.8f) Size = 0.8f;
            }

            if (type == 0)
            {
                GComponent -= 0.005f;
                BComponent += 0.005f;

                Color = new Color(RComponent, GComponent, BComponent);
            }
            else if (type == 4)
            {
                Color = new Color((float)(1f * random.NextDouble()), (float)(1f * random.NextDouble()), (float)(1f * random.NextDouble()));
            }
        }

        public void Draw(SpriteBatch spriteBatch) // Прорисовка частички
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }

        public void SetType(int type) // Установка цвета частички
        {
            this.type = type;
            Color StartColor = new Color(1f, 1f, 1f);

            switch (type)
            {
                case 0: StartColor = new Color(0f, 1f, 0f); break; // Обычная
                case 1: StartColor = new Color(1f, 0f, 0f); break; // Красная
                case 2: StartColor = new Color(1f, 0f, 1f); break; // Пурпурная
                case 3: StartColor = new Color(1f, 1f, 0f); break; // Желтая
                case 4: random = new Random(); break; // Мигающая
            }

            RComponent = ((int)StartColor.R) / 255f;
            GComponent = ((int)StartColor.G) / 255f;
            BComponent = ((int)StartColor.B) / 255f;

            Color = new Color(RComponent, GComponent, BComponent);

            if (type == -1)
            {
                Color = new Color(1f, 1f, 1f, 0.1f);
            }

        }
    }
}
