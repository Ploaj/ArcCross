using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CrossArc.GUI
{
    public class FileNode : TreeNode
    {
        public long ArcOffset { get; set; }
        public uint CompSize { get; set; }
        public uint DecompSize { get; set; }
        public uint Flags { get; set; }

        public bool IsRegional { get; set; } = false;

        public long[] _rArcOffset;
        public long[] _rCompSize;
        public long[] _rDecompSize;
        public uint[] _rFlags;

        public FileNode(string text)
        {
            this.Text = text;
            ContextMenu = Form1.NodeContextMenu;

            ImageKey = "file";
            SelectedImageKey = "file";
        }

        public void Extract()
        {
            TreeNode me = this.Parent;
            string Path = "";
            while (me != null)
            {
                Path = me.Text + "/" + Path;
                me = me.Parent;
            }
            Path = Path.Replace(":", "");
            Directory.CreateDirectory(Path);

            SaveFile(Path + "/" + Text.Replace(":", ""));
        }

        public void ExtractFolder()
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.FileName = Text;

                if(d.ShowDialog() == DialogResult.OK)
                {
                    SaveFile(d.FileName);
                }
            }
        }

        public void SaveFile(string FileName)
        {
            if (IsRegional)
            {
                File.WriteAllBytes(FileName, ARC.GetFileData(_rArcOffset[Form1.SelectedRegion], _rCompSize[Form1.SelectedRegion], _rDecompSize[Form1.SelectedRegion]));
            }
            else
            File.WriteAllBytes(FileName, ARC.GetFileData(ArcOffset, CompSize, DecompSize));
        }
    }
}
