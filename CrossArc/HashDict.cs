using System.Collections.Generic;
using System.IO;

namespace CrossArc
{
    public class HashDict
    {
        // Predict the size to avoid resizing the dictionary.
        public static Dictionary<uint, string> Hashes = new Dictionary<uint, string>(630000);

        public static bool TryGetValue(uint key, out string name)
        {
            return Hashes.TryGetValue(key, out name);
        }

        public static void Init()
        {
            foreach (string s in File.ReadLines("Hashes.txt"))
            {
                uint crc = CRC32.Crc32C(s);
                Hashes[crc] = s;
            }
        }
    }
}
