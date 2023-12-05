using System.Xml.Linq;

namespace astronomy
{
    internal class CreateXML : IPerformable
    {
        public static void Perform()
        {
            XDocument doc = new(
                new XElement("outputs",
                    new XElement("output",
                        new XAttribute("name", "Taylor Swift"),
                        new XAttribute("born", "1989"),
                        new XElement("albums",
                            new XElement("album",
                                new XAttribute("name", "Reputation"),
                                new XAttribute("year", "2017")
                            ),
                            new XElement("album",
                                new XAttribute("name", "folklore"),
                                new XAttribute("year", "2020")
                            )
                        )
                    )
                )
            );
            Console.WriteLine(doc.ToString());
        }
    }
}
