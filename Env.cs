using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astronomy
{
    internal class Env
    {
        private static Dictionary<string, string> settings = new();

        public static Dictionary<string, string> Settings {
            get { return settings; }
        }

        public static string GetValue(string key)
        {
            try
            {
                return settings[key];
            }
            catch (KeyNotFoundException)
            {
                return "";
            }
        }

        public static void SetValue(string key, string value) {
            if (GetValue(key) == "") {
                using StreamWriter streamWriter = File.AppendText(@"C:\global.txt");
                streamWriter.WriteLine(Environment.NewLine + $"{key}: {value}");
            } else
            {

            }
        }

        static Env() {
            try {
                using (StreamReader streamReader = new(@"C:\global.txt")) { 
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line == "") continue;

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
