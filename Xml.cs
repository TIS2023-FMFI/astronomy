using Microsoft.VisualBasic;
using System.Xml;

namespace astronomy
{

    class Configuration
    {
        public string name;
        public uint duration;
        public ushort[] positions;

        public Configuration(string name, uint duration, ushort[] positions)
        {
            this.name = name;
            this.duration = duration;
            this.positions = positions;
        }

        public override string ToString()
        {
            return $"{name}: ({string.Join(", ", positions)}) ({duration}ms)";
        }
    }

    internal class Xml
    {
        private string? path {get; set;}
        private List<Configuration>? openSequence;
        private List<Configuration>? closeSequence;
        public Xml()
        {
        }

        public string SetPathInteractive(string defaultPath = "C:\\Users\\benko\\Downloads\\maestro_settings.txt")
        {
            Console.Write("XML configuration file path: ");
            string path = Console.ReadLine();
            if (path == "" || path == null)
                path = defaultPath;

            return path;
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

        public void DoStuff()
        {
            path = SetPathInteractive();
            openSequence = GetSequence(SequenceType.OPEN);
            closeSequence = GetSequence(SequenceType.CLOSE);
            
            if (openSequence == null) return;

            openSequence.ForEach(Console.WriteLine);
            closeSequence.ForEach(Console.WriteLine);
        }
    }
}
