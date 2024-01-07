using System.Text;

namespace astronomy
{
    internal class GlobalSettings : IPerformable
    {
        public static void Perform()
        {
            var header = new StringBuilder("\nGLOBAL SETTINGS FILE\n");
            Enumerable.Range(0, Console.WindowWidth).ToList().ForEach(_ => header.Append('-'));
            header.Append('\n');
            Console.WriteLine(header.ToString());
            var keys = Env.Settings.Keys;
            foreach ( var key in keys )
            {
                Console.WriteLine($"{key}: {Env.GetValue(key)}");
            }
            Console.WriteLine();
        }
    }
}
