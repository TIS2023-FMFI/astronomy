using Microsoft.VisualBasic;
using Pololu.Usc;
using System.Xml;

namespace astronomy
{

    class Configuration
    {
        public string Name { get; set; }
        public uint Duration { get; set; }
        public ushort[] Positions { get; set; }

        public Configuration(string name, uint duration, ushort[] positions)
        {
            this.Name = name;
            this.Duration = duration;
            this.Positions = positions;
        }

        public override string ToString()
        {
            return $"{Name}: ({string.Join(", ", Positions)}) ({Duration}ms)";
        }

        public void Deconstruct(out string name, out uint duration, out ushort[] positions) {
            name = Name;
            duration = Duration;
            positions = Positions;
        }
    }

    internal class Xml
    {
        private string? path {get; set;}
        private List<Configuration>? openSequence;
        private List<Configuration>? closeSequence;
        private char openOption;
        private char closeOption;
        public Xml()
        {
        }

        public string SetPathInteractive(string defaultPath = "C:\\Users\\benko\\Downloads\\maestro_settings.txt")
        {
            Console.Write("XML configuration file path: ");
            string path = Console.ReadLine();
            if (path == "" || path == null)
                path = defaultPath;

            string parsed = path.Replace("\"", "");
            Console.WriteLine("Path: {0}", parsed);

            return parsed;
        }

        private string GetAttributeValue(XmlNode node, string name) {
            if (node.Attributes == null) return "";

            foreach (XmlAttribute attr in node.Attributes)
                if (attr.Name == name)
                    return attr.Value;

            return "";
        }

        public List<Configuration>? GetSequence(SequenceType? type = null)
        {
            if (path == null) return null;

            List<Configuration> seq = new();
            XmlDocument doc = new();
            doc.Load(path);

            uint duration = 0;
            ushort[] positions = Array.Empty<ushort>();
            string name = "";

            XmlNode? sequences = doc.GetElementsByTagName("Sequences")[0];
            if (sequences == null) return null;

            foreach (XmlNode sequence in sequences)
            {
                if (sequence == null || sequence.Attributes == null) break;

                string sequenceName = GetAttributeValue(sequence, "name");

                if (type == SequenceType.OPEN && sequenceName.ToLower() != "open") continue;
                if (type == SequenceType.CLOSE && sequenceName.ToLower() != "close") continue;

                foreach (XmlNode frame in sequence)
                {
                    if (frame.Attributes  == null) continue;

                    name = GetAttributeValue(frame, "name");
                    duration = Convert.ToUInt32(GetAttributeValue(frame, "duration"));
                    positions = frame.InnerText.Split(" ").Select(ushort.Parse).ToArray();

                    seq.Add(new Configuration(name, duration, positions));
                }
            }

            return seq;
        }

        public char menu(char openOption = 'a', char closeOption = 'b')
        {
            this.openOption = openOption;
            this.closeOption = closeOption;

            char userInput;
            List<char> options = new();
            do {
                if (openSequence != null && openSequence.Count > 0)
                {
                    Console.WriteLine("{0}) Run open sequence", openOption);
                    options.Add(openOption);
                }

                if (closeSequence != null && closeSequence.Count > 0)
                {
                    Console.WriteLine("{0}) Close sequence", closeOption);
                    options.Add(closeOption);
                }

                Console.WriteLine("x) Go to Main Menu");

                Console.Write("Select: ");

                string rawInput = Console.ReadLine();
                if (rawInput == null) return (char)0;

                userInput = rawInput.ToLower().ToCharArray()[0];
                if (userInput == 'x') break;
            } while (options.Count > 0 && !options.Contains(userInput));

            return userInput;
            
        }

        public void RunFrames(List<Configuration> configurations, Usc device)
        {
            foreach (Configuration configuration in configurations)
            {
                var (_, duration, positions) = configuration;
                Console.WriteLine("Running frame:");
                Console.WriteLine(configuration.ToString());

                for (byte i = 0; i < positions.Length; i++)
                {
                    device.setAcceleration(i, 100);
                    device.setSpeed(i, 0);
                    device.setTarget(i, positions[i]);
                }

                Thread.Sleep(checked((int)duration));
            }
        }

        public void DoStuff()
        {
            path = SetPathInteractive();
            openSequence = GetSequence(SequenceType.OPEN);
            closeSequence = GetSequence(SequenceType.CLOSE);

            char userSelection;
            while ((userSelection = menu()) != 'x') {
                if (userSelection == openOption && openSequence != null)
                {
                    Servo servo = new();
                    servo.Execute(device => RunFrames(openSequence, device));
                }

                if (userSelection == closeOption && closeSequence != null)
                {
                    Servo servo = new();
                    servo.Execute(device => RunFrames(openSequence, device));
                }
            }

            Console.WriteLine();
        }
    }
}
