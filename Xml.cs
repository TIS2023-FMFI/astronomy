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
        public static void DoStuff()
        {
            Console.Write("XML configuration file path: ");
            string? path = Console.ReadLine();
            if (path == "" || path == null)
            {
                path = "C:\\Users\\benko\\Downloads\\maestro_settings.txt";
            }


            XmlDocument doc = new();
            doc.Load(path);

            List<Configuration> configs = new List<Configuration>();

            XmlNode? sequencesNode = doc.GetElementsByTagName("Sequences")[0];

            for (int i = 0; i < sequencesNode?.ChildNodes.Count; i++)
            {
                XmlNode? sequence = sequencesNode.ChildNodes[i];
                if (sequence == null) break;

                foreach (XmlNode frame in sequence.ChildNodes)
                {
                    if (frame.Attributes == null) break;

                    ushort[] positions = frame.InnerText.Split(" ").Select(item => ushort.Parse(item)).ToArray();
                    string name = "";
                    uint duration = 0;
                    foreach (XmlAttribute attr in frame.Attributes)
                    {
                        if (attr.Name == "name") name = attr.Value;
                        if (attr.Name == "duration") duration = Convert.ToUInt32(attr.Value);
                    }

                    configs.Add(new Configuration(name, duration, positions));
                }
            }

            foreach (Configuration config in configs)
            {
                Console.WriteLine(config);
            }
        }
    }
}
