using System;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace CrossArc.GUI
{
    public partial class ExtractProgressBar : Form
    {
        private ArcExtractInformation[] toExtract;

        public ExtractProgressBar()
        {
            InitializeComponent();
            TopMost = true;
        }

        public void Extract(ArcExtractInformation[] toExtract)
        {
            this.toExtract = toExtract;
            Thread thread = new Thread(ExtractFileInformation);
            thread.IsBackground = true;
            thread.Start();
        }
        
        public void Update(int i, string message)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<int, string>(Update), new object[] { i, message });
                return;
            }

            progressBar1.Value = i;
            label1.Text = message;
            if(i == 100)
            {
                Close();
            }
        }

        private void ExtractFileInformation()
        {
            int index = 0;
            using (BinaryReader r = new BinaryReader(new FileStream(ARC.FilePath, FileMode.Open)))
            {
                foreach (var file in toExtract)
                {
                    try
                    {
                        r.BaseStream.Position = (file.ArcOffset);
                        byte[] data = r.ReadBytes((int)file.CompSize);
                        if (file.CompSize != file.DecompSize)
                        {
                            data = ARC.Decompress(data);
                        }
                        string path = "root\\" + file.FilePath.Replace(":", "");
                        Directory.CreateDirectory(Path.GetDirectoryName(path) + "\\");
                        File.WriteAllBytes(path, data);
                        index++;
                        Update((int)Math.Floor((index / (float)toExtract.Length) * 100), file.FilePath);
                    }
                    catch(Exception e)
                    {
                        index++;
                        Update(0, e.ToString());
                        break;
                    }
                }
            }
        }
    }
}
