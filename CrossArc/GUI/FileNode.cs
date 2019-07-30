using System.Collections.Generic;
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

        public bool IsRegional
        {
            get => _isRegional;
            set
            {
                if (value)
                    ForeColor = System.Drawing.Color.DarkOliveGreen;
                _isRegional = value;
            }
        }
        private bool _isRegional = false;

        public string FullFilePath { get; set; }
        public string DirectoryPath
        {
            get
            {
                TreeNode me = this.Parent;
                string Path = "";
                while (me != null)
                {
                    Path = me.Text + "/" + Path;
                    me = me.Parent;
                }
                Path = Path.Replace(":", "");
                return Path;
            }
        }

        public long[] _rArcOffset;
        public long[] _rCompSize;
        public long[] _rDecompSize;
        public uint[] _rFlags;

        public FileNode(string text, ContextMenu menu)
        {
            this.Text = text;
            ContextMenu = menu;

            ImageKey = "file";
            SelectedImageKey = "file";
        }

        public void Extract(ARC arc, bool preserveCompression)
        {
            string DirectoryPath = this.DirectoryPath;

            Directory.CreateDirectory(DirectoryPath);

            SaveFile(DirectoryPath + "/" + Text.Replace(":", ""), arc, preserveCompression);
        }

        public void ExtractFolder(ARC arc, bool preserveCompression)
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.FileName = Text;

                if (d.ShowDialog() == DialogResult.OK)
                {
                    SaveFile(d.FileName, arc, preserveCompression);
                }
            }
        }

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

        public ArcExtractInformation[] GetExtractInformation(string fileName, bool preserveCompressed = false)
        {
            var extractInfo = new ArcExtractInformation(fileName, ArcOffset, CompSize, preserveCompressed ? CompSize : DecompSize);

            if (IsRegional)
            {
                if (Form1.SelectedRegion == 14)
                {
                    List<ArcExtractInformation> info = new List<ArcExtractInformation>(_rArcOffset.Length);
                    for (int i = 0; i < 13; i++)
                    {
                        string newFileName = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + regions[i] + Path.GetExtension(fileName); ;
                        //Console.WriteLine(newFileName);
                        extractInfo = new ArcExtractInformation(newFileName, _rArcOffset[i], (uint)_rCompSize[i], preserveCompressed ? (uint)_rCompSize[i] : (uint)_rDecompSize[i]);
                        info.Add(extractInfo);
                    }
                    return info.ToArray();
                }
                string neFileName = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + regions[Form1.SelectedRegion] + Path.GetExtension(fileName); ;
                //Console.WriteLine(neFileName);

                extractInfo = new ArcExtractInformation(neFileName, _rArcOffset[Form1.SelectedRegion], (uint)_rCompSize[Form1.SelectedRegion], preserveCompressed ? (uint)_rCompSize[Form1.SelectedRegion] : (uint)_rDecompSize[Form1.SelectedRegion]);

                return new ArcExtractInformation[] { extractInfo };
            }

            return new ArcExtractInformation[] { extractInfo };
        }

        public void SaveFile(string fileName, ARC arc, bool preserveCompressed)
        {
            if (IsRegional)
            {
                if (Form1.SelectedRegion != -1)
                {
                    //Extract all
                }
                if (preserveCompressed)
                {
                    // Make DecompSize = CompSize so it doesn't get decompressed
                    File.WriteAllBytes(fileName, arc.GetFileData(_rArcOffset[Form1.SelectedRegion], _rCompSize[Form1.SelectedRegion], _rCompSize[Form1.SelectedRegion]));
                }
                else
                {
                    File.WriteAllBytes(fileName, arc.GetFileData(_rArcOffset[Form1.SelectedRegion], _rCompSize[Form1.SelectedRegion], _rDecompSize[Form1.SelectedRegion]));
                }
            }
            else
            {
                if (preserveCompressed)
                {
                    // Make DecompSize = CompSize so it doesn't get decompressed
                    File.WriteAllBytes(fileName, arc.GetFileData(ArcOffset, CompSize, CompSize));
                }
                else
                {
                    File.WriteAllBytes(fileName, arc.GetFileData(ArcOffset, CompSize, DecompSize));
                }
            }
        }
    }
}
