using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Planet Earth = new Planet("Земля", 21323123123.231f);

            Object a = new Int32();

            Console.WriteLine(a.GetType());
            a.GetType();

            Earth.WriteInfo();

            
        }
    }

    class Planet
    {
        string name;
        float distans;

        public Planet(string name, float distans)
        {
            this.name = name;
            this.distans = distans;
        }

        public void WriteInfo()
        {
            Console.WriteLine("Название планеты {0} Растояние {1}",name,distans);
            Console.Read();
        }
        
    }
}
