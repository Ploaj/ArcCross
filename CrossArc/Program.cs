using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using CrossArc.Structs;
using System.Diagnostics;
using System.Reflection;
using Zstandard.Net;
using System.IO.Compression;
using System.Windows.Forms;

namespace CrossArc
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            bool Extract = false;
            string ArcOffset = "data.arc";
            string FolderToExtract = "";

            //args = new string[] { "-x", "data.arc" };//, "stage/BossStage_Dracula/normal/model/bs_dc_floor_shadow_set" };

            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Notes" +
                    "\nA lot of files/folders will not have proper filenames" +
                    "\nCommands:" +
                    "\nExtract entire arc (Untested! EXPERIMENTAL!!!)" +
                    "\n-x (data.arc)" +
                    "\nExtract folder from arc" +
                    "\n-x (data.arc) (folderinarc)" +
                    "\nEx: -x data.arc fighter/mario/c00");
            }

            //process args
            for(int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-x"))
                {
                    Extract = true;
                    Console.WriteLine("Extract");
                    if (args.Length >= 2)
                    {
                        i++;
                        ArcOffset = args[i];
                        Console.WriteLine(ArcOffset);
                        if (args.Length >= 3)
                        {
                            i++;
                            FolderToExtract = args[i];
                            Console.WriteLine(FolderToExtract);
                        }
                    }
                }
            }
            Debug.WriteLine(FolderToExtract);

            /*var list1 = new List<string>();
            list1.AddRange(File.ReadAllLines("moosestrings.txt"));
            list1.Union(File.ReadAllLines("Hashes.txt"));
            list1.Sort();
            using (StreamWriter w = new StreamWriter(new FileStream("out.txt", FileMode.Create)))
            {
                foreach(string s in list1)
                w.WriteLine(s);
            }*/

            //ARC.HashCheck();

            //HashDict.Init();
            //ARC.CompareHashes("ARCV1_1Hashes.bin", "ARCV2_0Hashes.bin");
            //return;
            if (!Extract)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var timer = new Stopwatch();
                timer.Start();
                HashDict.Init();
                timer.Stop();
                Debug.WriteLine("Initiating Hash Dict: " + timer.ElapsedMilliseconds);
                timer.Reset();
                timer.Start();
                ARC.Open();
                timer.Stop();
                Debug.WriteLine("Initiating Arc: " + timer.ElapsedMilliseconds);
                //ARC.CreateHashCompare("ARCV2_0Hashes.bin");
                Application.Run(new Form1());
            }
            else
            {
                //ARC.CommandFunctions(FolderToExtract);
            }
            
            //Console.WriteLine("Done");
            //Console.WriteLine("Press enter to close");
            //Console.ReadLine();
        }


        

        public static void PrintStruct<T>(T Struct) where T : struct
        {
            FieldInfo[] Fields = Struct.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach(FieldInfo f in Fields)
            {
                Debug.WriteLine(f.Name + " " + f.GetValue(Struct));
            }
        }
    }
}
