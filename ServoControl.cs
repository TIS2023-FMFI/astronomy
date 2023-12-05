using Pololu.UsbWrapper;
using Pololu.Usc;
using System.Text;

namespace astronomy
{
    internal class ServoControl: IPerformable
    {
        private static Usc Connect()
        {
            List<DeviceListItem> devices = Usc.getConnectedDevices();
            if (devices.Count < 2)
            {
                return new Usc(devices.First());
            }

            Console.Write("Select device: ");
            Console.ReadLine();
            return new Usc(devices.First());
        }

        private static byte FindServo()
        {
            char[] selection;

            do
            {
                Console.Write("Select servo channel (0-7): ");
                string str;
                str = Console.ReadLine() ?? "";
                selection = str.ToCharArray();
                if (str.Equals(""))
                {
                    selection = new char[1];
                    selection[0] = (char)0;
                    continue;
                }

            }
            while (selection[0] < '0' || selection[0] > '9');


            return (byte)(Encoding.ASCII.GetBytes(selection)[0] - 48);
        }

        private static ushort FindTarget(ushort min, ushort max)
        {
            ushort targetNumber;
            string str;
            do
            {
                Console.Write($"Target ({min}-{max}): ");
                str = Console.ReadLine() ?? "";

                bool check = ushort.TryParse(str, out _);
                if (!check || str.Equals(""))
                {
                    targetNumber = 0;
                    continue;
                }
                targetNumber = ushort.Parse(str);
            } while (targetNumber < min || targetNumber > max);

            return targetNumber;
        }

        public static void Perform()
        {
            Usc device = Connect();
            byte servoNumber = FindServo();
            ushort targetNumber = FindTarget(3968, 8000);

            device.setAcceleration(servoNumber, 100);
            device.setSpeed(servoNumber, 0);
            device.setTarget(servoNumber, targetNumber);

            device.Dispose();
        }
    }
}
