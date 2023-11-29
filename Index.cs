using astronomy;
using Pololu.UsbWrapper;
using Pololu.Usc;
using System.Text;

namespace Astronomy
{
    internal class Index
    {
        static void Main()
        {
            char userOption;

            while (true) {
                Console.WriteLine("1) Control servo motors");
                Console.WriteLine("2) Parse XML");
                Console.WriteLine("x) Exit");
                Console.Write("Enter option: ");
                userOption = Console.ReadLine().ToCharArray()[0];

                if (userOption == '1')
                {
                    ServoControl.Perform();
                }
                if (userOption == '2') ParseXML.Perform();
                if (Char.ToLower(userOption) == 'x')
                {
                    Exit.Perform();
                    break;
                }
            }
        }
    }
}