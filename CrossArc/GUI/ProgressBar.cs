using CrossArc.GUI.Nodes;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CrossArc.GUI
{
    public partial class ProgressBar : Form
    {
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

        public void UpdateProgress(int i, string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int, string>(UpdateProgress), i, message);
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
                    FileExtraction.ExtractAllRegions(path, file.ArcPath, DecompressFiles, UseOffsetName);
                else
                {
                    if (regional)
                        path = path.Replace(Path.GetExtension(path), FileExtraction.RegionTags[MainForm.SelectedRegion] + Path.GetExtension(path));

                    FileExtraction.SaveFile(path, file.ArcPath, MainForm.SelectedRegion, DecompressFiles, UseOffsetName);
                }

                UpdateProgress(GetPercentage(index, toExtract.Length), path);
                index++;
            }
            UpdateProgress(100, "Done");

            // Make sure the completion message stays on screen long enough to be read.
            Thread.Sleep(500);
            UpdateProgress(101, "Done");
        }

        private int GetPercentage(int current, int total)
        {
            return (int)Math.Floor((current / (float)total) * 100); ;
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
