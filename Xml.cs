using System.Xml;

namespace astronomy
{
    internal class Xml
    {
        public static void DoStuff()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Users\\benko\\Downloads\\maestro_settings.txt");
            List<string> openSequence = new List<string>();
            List<string> closeSequence = new List<string>();

            var sequencesNode = doc.GetElementsByTagName("Sequences")[0];

            for (int i = 0; i < sequencesNode?.ChildNodes.Count; i++)
            {
                XmlNode child = sequencesNode.ChildNodes[i];
                string attrName = child.Attributes["name"].Value;
                Console.WriteLine(child.ChildNodes.Count);
            }
        }


    }
}
