using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WindowsGame;

namespace WindowsGame
{
    class CatcherHolder
    {
        private Random random; // Генератор случайных чисел

        public List<Catcher> particles; // Массив частичек (Catcher)
        private List<Texture2D> textures; // Массив текстур

        public List<float> accomulator { get; set; } // Массив float-значений, что такое и зачем нужен accomulator — объясню чуть позже.

        public CatcherHolder(List<Texture2D> textures)
        {
            this.textures = textures;
            this.particles = new List<Catcher>();

            random = new Random();

            accomulator = new List<float>(); // Инициализируем массив и записываем во все 128 ячеек — 1.0f
            for (int a = 0; a < 128; a++)
            {
                accomulator.Add(1.0f);
            }
        }
    // Генерация одной частички
    // Wave - волна, число от 0f до ширины экрана.
     private Catcher GenerateNewParticle(float Wave) 
        {
            Texture2D texture = textures[random.Next(textures.Count)]; // Берем случайную текстуру из массива
            Vector2 position = new Vector2(Wave, 0); // Задаем позицию
            Vector2 velocity = new Vector2((float)(random.NextDouble()  - 0.5), (float)(random.NextDouble() * 10)); // Случайное ускорение, 0.5f для X и 10f для Y
            float angle = 0; // Угол поворота = 0
            float angularVelocity = 0.05f * (float)(random.NextDouble()*2 - 1 ); // Случайная скорость вращения
            Color color = new Color(0f, 1f, 0f); // Зеленый цвет (изменится цвет уже в самом Catcher)
            float size = (float)random.NextDouble()*.8f + .2f; // Случайный размер
            int ttl = 400; // Время жизни в 400 (400 актов рисования живет частица, т.е. 400 / 60 — 6 с лишним секунд.

            int type = 0; // изначальный тип 0

        // Вероятность появления
         if (random.Next(10000) > 9900) // враг
                type = 1;
            else if (random.Next(10000) > 9950) // желтый
                type = 3;
            else if (random.Next(10000) > 9978) // пурпурный
                type = 2;
            else if (random.Next(10000) > 9980) // мигающий
                type = 4;
    
            return new Catcher(texture, position, velocity, angle, angularVelocity, type, size, ttl); // Создаем частичку и возвращаем её
        }

    // Генерация желтых частичек при касании с красной частичкой
     public void GenerateYellowExplossion(int x, int y, int radius)
        {
            Texture2D texture = textures[random.Next(textures.Count)];

            Vector2 direction = Vector2.Zero;
            float angle = (float)Math.PI * 2.0f * (float)random.NextDouble();
            float length = radius * 4f;

            direction.X = (float)Math.Cos(angle);
            direction.Y = -(float)Math.Sin(angle);

            Vector2 position = new Vector2(x, y) + direction * length;

            Vector2 velocity = direction * 4f;
            
            float angularVelocity = 0.05f * (float)(random.NextDouble() * 2 - 1);

            float size = (float)random.NextDouble() * .8f + .2f;
            int ttl = 400;

            int type = 3;
            particles.Add(new Catcher(texture, position, velocity, 0, angularVelocity, type, size, ttl));
        }

    // "Музыкальный" импульс, создание частички
        public void Beat(float Wave)
        {
            particles.Add(GenerateNewParticle(Wave));
        }

        public void Update() // Обновление всех частиц
        {
           
           for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].Size <= 0 || particles[particle].TTL <= 0)
                {
            // Если частичка дохлая или размер нуль или меньше, удаляем её
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
    
    // Обновляем аккумулятор, если значения ячейки меньше 1f, то добавляем значение, указанное в статическом классе Constants — ACCUMULATE_SPEED, листинг Constanst - ниже.
            for (int a = 0; a < 128; a++)
                if (accomulator[a] < 1.0f)
                    accomulator[a] += Constant.ACCUMULATE_SPEED;
            
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        // Прорисовываем все частички, важно указать BlendState.Additive, чтобы частички были более "мягкие".
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
