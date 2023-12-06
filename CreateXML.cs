using System.Xml.Linq;

namespace astronomy
{
    class Frame
    {
        public Frame() { }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private uint _duration;
        public uint Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        private ushort[] _positions;
        public ushort[] Positions
        {
            get { return _positions; }
            set { _positions = value; }
        }

        private SequenceType _sequenceType;
        public SequenceType SequenceType
        {
            get { return _sequenceType; }
            set { _sequenceType = value; }
        }

        public override string ToString()
        {
            return $"{Name} ({Duration}): {string.Join(", ", Positions)}";
        }
    }

    internal class CreateXML : IPerformable
    {
        private static XElement AddFrame(Frame frame)
        {
            XElement element = new("Frame");
            element.Add(new XAttribute("name", (string) frame.Name));
            element.Add(new XAttribute("duration", (string) frame.Duration.ToString()));
            element.Value = string.Join(" ", frame.Positions);
            return element;
        }

        private static List<Frame> AskForFrames()
        {
            List<Frame> frames = new();

            string frameNameInput, durationInput, framesInput, typeInput;
            while (true)
            {
                Frame frame = new();
                Console.WriteLine("(press Enter to finish adding frames)");
                Console.Write("\nEnter frame name: ");
                frameNameInput = Console.ReadLine();
                if (frameNameInput == "") break;

                frame.Name = frameNameInput;

                Console.Write("Enter frame duration (ms): ");
                durationInput = Console.ReadLine();
                frame.Duration = Convert.ToUInt32(durationInput);

                Console.Write("Enter frame states (space-separated integers): ");
                framesInput = Console.ReadLine();
                frame.Positions = framesInput.Split(" ").Select(ushort.Parse).ToArray();

                Console.Write("(O)pen or (C)lose sequence: ");
                typeInput = Console.ReadLine();
                string sequenceType = typeInput.ToCharArray()[0].ToString().ToLower();
                if (sequenceType == "c") frame.SequenceType = SequenceType.CLOSE;
                else if (sequenceType == "o") frame.SequenceType = SequenceType.OPEN;

                frames.Add(frame);
            };

            return frames;
        }

        public static string EnterFileName()
        {
            Console.Write("Enter file name (without suffix): ");
            string fileInput = Console.ReadLine();
            string fileName = fileInput;
            if (fileInput == null || fileInput == "")
            {
                fileName = "file";
            }

            fileName.Replace(' ', '-');
            return String.Format(@"{0}\{1}-{2}.txt", Environment.GetEnvironmentVariable("USERPROFILE"), fileName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff"));
        }

        public static void Perform()
        {
            List<Frame> closeFrames = new();
            List<Frame> openFrames = new();

            List<Frame> frames = AskForFrames();
            Console.WriteLine("Frame count: " + frames.Count);
            frames.ForEach(frame => {
                Console.WriteLine(frame);

                if (frame.SequenceType == SequenceType.CLOSE)
                    closeFrames.Add(frame);
                
                if (frame.SequenceType == SequenceType.OPEN)
                    openFrames.Add(frame);
            });


            XElement closeSequence = new("Sequence",
                new XAttribute("name", "Close"),
                closeFrames.Select(frame => AddFrame(frame))
            );

            XElement openSequence = new("Sequence",
                new XAttribute("name", "Open"),
                openFrames.Select(frame => AddFrame(frame))
            );

            XElement sequences = new("Sequences", closeSequence, openSequence);
            XDocument main = new(
                new XElement("UscSettings", new XAttribute("version", (new Env()).GetValue("Usc_Settings")),
                new XElement("NeverSuspend", new Env().GetValue("Never_Suspend")),
                new XElement("SerialMode", new Env().GetValue("Serial_Mode")),
                new XElement("FixedBaudRate", new Env().GetValue("Fixed_Baud_Rate")),
                new XElement("SerialTimeout", new Env().GetValue("Serial_Timeout")),
                new XElement("EnableCrc", new Env().GetValue("Enable_Crc")),
                new XElement("SerialDeviceNumber", new Env().GetValue("Serial_Device_Number")),
                new XElement("SerialMiniSscOffset", new Env().GetValue("Serial_Mini_Ssc_Offset")),
                new XElement("Script", new XAttribute("ScriptDone", false)),
            sequences));

            string path = EnterFileName();
            main.Save(path);

            Console.WriteLine($"\nSaved to {path}\n");
        }
    }
}
