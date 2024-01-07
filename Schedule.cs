using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astronomy
{
    internal class Schedule : IPerformable
    {
        public static void Perform()
        {
            var dailyScheduler = new DailyScheduler(4, 1, 2024, 5, 17.39519, 48.22027, 1);
            dailyScheduler.GetSchedule();
        }
    }
}
