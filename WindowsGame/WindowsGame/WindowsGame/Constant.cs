using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame
{
    public class Constant
    {
        public const float BEAT_COST = .4f; // "Стоимость" частички, отнимается у аккумулятора при генерации новой частички
        public const float ACCUMULATE_SPEED = .01f; // Скорость аккумуляции
        public const float BEAT_REACTION = .5f; // Значение реакции на "бит" в музыке
        public const float ACCOMULATOR_REACTION = .5f; // Разрешает создавать новую частичку только тогда, когда значения больше реакции аккумулятора
    }
}
