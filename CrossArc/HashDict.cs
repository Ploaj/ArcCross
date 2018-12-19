using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossArc
{
    public class HashDict
    {
        public static Dictionary<uint, string> Hashes = new Dictionary<uint, string>();

        public static bool TryGetValue(uint key, out string name)
        {
            return Hashes.TryGetValue(key, out name);
        }

        public static void Init()
        {
            string[] strings = File.ReadAllLines("Hashes.txt");

            //StreamWriter w = new StreamWriter(new FileStream("newstrings.txt", FileMode.Create));
            foreach (string s in strings)
            {
                //if (s.Contains("/")) continue;
                //w.WriteLine(s);
                uint crc = CRC32.Crc32C(s);
                if (!Hashes.ContainsKey(crc))
                    Hashes.Add(crc, s);
            }
            //w.Close();
        }
    }
}
