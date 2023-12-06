using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astronomy
{
    internal class Env
    {
        private Dictionary<string, string> settings = new();

        public Dictionary<string, string> Settings {
            get { return settings; }
        }

        public string GetValue(string key)
        {
            return settings[key] ?? "";
        }

        public Env() {
            try {
                using (var sr = new StreamReader(@"C:\global.txt")) {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var parts = line.Split(": ");
                        settings.Add(parts[0], parts[1]);
                    }
                }
            } catch (IOException e) {
                Console.WriteLine("File could not be read: ");
                Console.WriteLine(e.Message);
            }
        }
    }
}
