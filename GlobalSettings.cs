using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astronomy
{
    internal class GlobalSettings : IPerformable
    {
        public static void Perform()
        {
            Console.WriteLine("\nGLOBAL SETTINGS FILE\n-------------------------------");
            var env = new Env();
            var keys = env.Settings.Keys;
            foreach ( var key in keys )
            {
                Console.WriteLine($"{key}: {env.GetValue(key)}");
            }
            Console.WriteLine();
        }
    }
}
