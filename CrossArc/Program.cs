using System;
using System.Diagnostics;
using System.Reflection;
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

                Application.Run(new Form1());
            }
            else
            {
                //ARC.CommandFunctions(FolderToExtract);
            }
            
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
