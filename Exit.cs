using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astronomy
{
    internal class Exit : IPerformable
    {
        public static void Perform()
        {
            Console.WriteLine("Goodbye!");
        }
    }
}
