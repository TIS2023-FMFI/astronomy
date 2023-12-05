using System.Xml.Linq;

namespace astronomy
{
    internal class CreateXML : IPerformable
    {
        public static void Perform()
        {
            var albums = new List<List<string>>() {
                new List<string> { "Reputation", "2017" },
                new List<string> { "folklore", "2020"},
                new List<string> { "Lover", "2019"},
                new List<string> { "Midnights", "2022"},

            };
            XDocument doc = new(
                new XElement("outputs",
                    new XElement("output",
                        new XAttribute("name", "Taylor Swift"),
                        new XAttribute("born", "1989"),
                        new XElement("albums",
                            from album in albums
                            select new XElement("album", new XAttribute("name", album[0]), new XAttribute("year", album[1]))
                        )
                    )
                )
            );
            Console.WriteLine(doc.ToString());
        }
    }
}
