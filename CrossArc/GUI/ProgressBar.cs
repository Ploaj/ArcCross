using CrossArc.GUI.Nodes;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CrossArc.GUI
{
    public partial class ProgressBar : Form
    {
        private static readonly string[] regions =
        {
            "+jp_ja",
            "+us_en",
            "+us_fr",
            "+us_es",
            "+eu_en",
            "+eu_fr",
            "+eu_es",
            "+eu_de",
            "+eu_nl",
            "+eu_it",
            "+eu_ru",
            "+kr_ko",
            "+zh_cn",
            "+zh_tw"
        };

        public ProgressBar()
        {
            InitializeComponent();
            TopMost = true;

            FormClosing += (sender, args) => { if (thread != null) thread.Abort(); };
        }

        private FileNode[] toExtract;
        private Thread thread;

        public bool UseOffsetName { get; set; } = false;
        public bool DecompressFiles { get; set; } = true;

        public void Extract(FileNode[] toExtract)
        {
            this.toExtract = toExtract;
            thread = new Thread(ExtractFileInformation) { IsBackground = true };
            thread.Start();
        }

        public void Update(int i, string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int, string>(Update), i, message);
                return;
            }

            if (i == 101)
            {
                Close();
            }
            else
            {
                SetPercent(i);
                SetMessage(message);
            }
        }

        private void ExtractFileInformation()
        {
            int index = 1;
            foreach (var file in toExtract)
            {
                string path = file.FullPath.Replace(":", "");

                Directory.CreateDirectory(Path.GetDirectoryName(path) + "\\");

                bool regional = MainForm.ArcFile.IsRegional(file.ArcPath);

                if (regional && MainForm.SelectedRegion == 14)
                    ExtractAllRegions(path, file.ArcPath);
                else
                {
                    if (regional)
                        path = path.Replace(Path.GetExtension(path), regions[MainForm.SelectedRegion] + Path.GetExtension(path));

                    SaveFile(path, file.ArcPath, MainForm.SelectedRegion);
                }

                Update((int)Math.Floor((index / (float)toExtract.Length) * 100), path);
                index++;
            }
            Update(100, "Done");
            // TODO: Just don't use a progress bar for a small number of files to extract.
            Thread.Sleep(1000);
            Update(101, "Done");
        }

        private void ExtractAllRegions(string path, string arcPath)
        {
            for (int i = 0; i < 14; i++)
            {
                var newPath = path.Replace(Path.GetExtension(path), regions[i] + Path.GetExtension(path));

                SaveFile(newPath, arcPath, i);
            }
        }

        private void SaveFile(string filepath, string arcpath, int i)
        {
            MainForm.ArcFile.GetFileInformation(arcpath, out long offset, out uint compSize, out uint decompSize, out bool regional, i);

            byte[] data;

            if (DecompressFiles)
                data = MainForm.ArcFile.GetFile(arcpath, i);
            else
                data = MainForm.ArcFile.GetFileCompressed(arcpath, i);

            if (UseOffsetName)
                filepath = Path.GetDirectoryName(filepath) + "/0x" + offset.ToString("X");

            File.WriteAllBytes(filepath, data);
        }

        public void SetMessage(string message)
        {
            label1.Text = message;
        }

        public void SetPercent(int percent)
        {
            progressBar1.Value = percent;
        }
    }
}
