using astronomy;

namespace Astronomy
{
    internal class Index
    {
        static void Main()
        {
            char userOption;

            while (true) {
                Console.WriteLine("1) Control servo motors");
                Console.WriteLine("2) Execute XML sequence");
                Console.WriteLine("3) Create XML sequence");
                Console.WriteLine("4) Edit global settings");
                Console.WriteLine("x) Exit");
                Console.Write("Enter option: ");
                string? raw = Console.ReadLine();

                if (raw == "" || raw == null) continue;
                userOption = raw.ToCharArray()[0];

                if (userOption == '1') ServoControl.Perform();
                if (userOption == '2') ParseXML.Perform();
                if (userOption == '3') CreateXML.Perform();
                if (userOption == '4') GlobalSettings.Perform();
                if (Char.ToLower(userOption) == 'x')
                {
                    Exit.Perform();
                    break;
                }
            }
        }
    }
}