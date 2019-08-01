using System.Collections.Generic;
using System.IO;

namespace ArcCross
{
    public class HashDict
    {
        public static bool Initialized { get; internal set; } = false;

        private static readonly Dictionary<uint, List<string>> hashLookup = new Dictionary<uint, List<string>>();

        public static void Init()
        {
            if (!File.Exists("Hashes.txt"))
                return;

            var sw = System.Diagnostics.Stopwatch.StartNew();

            /*var hash = CRC32.Crc32C(File.GetCreationTime("Hashes.txt").ToLongDateString());

            if (File.Exists("Hashes.cache"))
            {
                if (LoadHashCache(hash))
                {
                    Initialized = true;

                    System.Diagnostics.Debug.WriteLine(sw.Elapsed.Milliseconds);
                    return;
                }
            }*/

            ReadHashesFromTxt();
            //SaveHashCache();

            Initialized = true;

            System.Diagnostics.Debug.WriteLine($"Init Hashes: {sw.Elapsed.Milliseconds} ms");
        }

        private static void ReadHashesFromTxt()
        {
            foreach (var line in File.ReadLines("Hashes.txt"))
            {
                uint hash = CRC32.Crc32C(line);
                if (!hashLookup.ContainsKey(hash))
                    hashLookup.Add(hash, new List<string>());
                hashLookup[hash].Add(line);
            }
        }

        private static void SaveHashCache()
        {
            using (BinaryWriter w = new BinaryWriter(new FileStream("Hashes.cache", FileMode.Create)))
            {
                w.Write(CRC32.Crc32C(File.GetCreationTime("Hashes.txt").ToLongDateString()));
                w.Write((int)hashLookup.Count);
                foreach(var v in hashLookup)
                {
                    w.Write(v.Key);
                    w.Write(v.Value.Count);
                    foreach (var s in v.Value)
                    {
                        w.Write((short)s.Length);
                        w.Write(s.ToCharArray());
                    }
                }
            }
        }

        private static bool LoadHashCache(uint createHash)
        {
            using (BinaryReader r = new BinaryReader(new FileStream("Hashes.cache", FileMode.Open)))
            {
                hashLookup.Clear();
                if (createHash != r.ReadUInt32())
                    return false;
                var count = r.ReadInt32();
                for(int i = 0; i < count; i++)
                {
                    var hash = r.ReadUInt32();
                    var count2 = r.ReadInt32();
                    List<string> strings = new List<string>();
                    for (int j = 0; j < count2; j++)
                        strings.Add(new string(r.ReadChars(r.ReadInt16())));
                    hashLookup.Add(hash, strings);
                }
            }
            return true;
        }

        public static void Unload()
        {
            hashLookup.Clear();
            Initialized = false;
        }

        /// <summary>
        /// Gets the strings with the associated hash and optional length
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetString(uint hash, int length = -1)
        {
            if (hashLookup.ContainsKey(hash))
            {
                if (length == -1)
                    return hashLookup[hash][0];

                foreach(var str in hashLookup[hash])
                {
                    if (str.Length == length)
                        return str;
                }
                return hashLookup[hash][0];
            }
            return "0x" + hash.ToString("X");
        }

    }
}
