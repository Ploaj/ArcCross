using System.Collections.Generic;
using System.IO;

namespace ArcCross
{
    public class HashDict
    {
        public static bool Initialized { get; internal set; } = false;

        private static Dictionary<uint, List<string>> HashLookup = new Dictionary<uint, List<string>>();

        public static void Init()
        {
            if (!File.Exists("Hashes.txt"))
                return;

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

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

            ReadHashesFromTXT();
            //SaveHashCache();

            Initialized = true;

            System.Diagnostics.Debug.WriteLine(sw.Elapsed.Milliseconds);
        }

        private static void ReadHashesFromTXT()
        {
            using (StreamReader sr = new StreamReader("Hashes.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    uint Hash = CRC32.Crc32C(line);
                    if (!HashLookup.ContainsKey(Hash))
                        HashLookup.Add(Hash, new List<string>());
                    HashLookup[Hash].Add(line);
                }
            }
        }

        private static void SaveHashCache()
        {
            using (BinaryWriter w = new BinaryWriter(new FileStream("Hashes.cache", FileMode.Create)))
            {
                w.Write(CRC32.Crc32C(File.GetCreationTime("Hashes.txt").ToLongDateString()));
                w.Write((int)HashLookup.Count);
                foreach(var v in HashLookup)
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
                HashLookup.Clear();
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
                    HashLookup.Add(hash, strings);
                }
            }
            return true;
        }

        public static void Unload()
        {
            HashLookup.Clear();
            Initialized = false;
        }

        /// <summary>
        /// Gets the strings with the associated hash and optional length
        /// </summary>
        /// <param name="Hash"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string GetString(uint Hash, int Length = -1)
        {
            if (HashLookup.ContainsKey(Hash))
            {
                if (Length == -1)
                    return HashLookup[Hash][0];
                else
                    foreach(var str in HashLookup[Hash])
                    {
                        if (str.Length == Length)
                            return str;
                    }
                return HashLookup[Hash][0];
            }
            return "0x" + Hash.ToString("X");
        }

    }
}
