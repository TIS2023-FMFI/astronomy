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
            DailyScheduler.TestSuite();

            //StringBuilder sb = new();
            //DailyScheduleCalculator dailyScheduleCalculator = new(4, 1, 2024, 1, 17.39519, 48.22027, 2);

            //sb.AppendLine("     Slnko     občian.   nautic.   METEOR     Mesiac");
            //sb.AppendLine("dátum     v    z    v    z    v    z    vyp. zap.  v    z");
            //sb.AppendLine("------------------------------------------------------------");

            //var mj = dailyScheduleCalculator.Mjd();
            //for (int i = 0; i < dailyScheduleCalculator.Duration; i++)
            //{
            //    sb.Append(dailyScheduleCalculator.CalDat(mj + i));
            //    sb.Append(' ');
            //    sb.Append(dailyScheduleCalculator.FindSunAndTwiEventsForDate(mj + i));
            //    sb.Append(' ');
            //    sb.Append(dailyScheduleCalculator.FindMoonriseSet(mj + i));
            //}

            //sb.AppendLine();
            //sb.AppendLine();
            //sb.AppendLine("""
            //    Legenda:

            //    Slnko - východ/západ Slnka
            //    občian. - v = brieždenie, z = občiansky súmrak
            //    nautic. - nautický súmrak
            //    METEOR - čas vypnutia/zapnutia kamery na záznam meteorov v Senci
            //    Mesiac - východ/západ Mesiaca

            //    v - východ
            //    z - západ

            //    .... Slnko/Mesiac ostáva celý deň pod horizontom alebo súmrak sa nezačína
            //    **** Slnko/Mesiac ostáva celý deň nad horizontom alebo súmrak sa neskončí
            //    ---- Slnko/Mesiac nevychádza, alebo nezapadá
            //    """);

            
            //Console.WriteLine(sb.ToString());
        }
    }
}
