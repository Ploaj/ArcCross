using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

namespace CrossArc.GUI.Nodes
{
    public class FileInformation
    {
        [ReadOnly(true), DisplayName("Arc Offset")]
        public string ArcOffset { get { return "0x" + Offset.ToString("X"); } }

        [ReadOnly(true), DisplayName("Compressed Size")]
        public string comp { get { return "0x" + CompressedSize.ToString("X"); } }

        [ReadOnly(true), DisplayName("Decompressed Size")]
        public string decomp { get { return "0x" + DecompressedSize.ToString("X"); } }

        [ReadOnly(true), DisplayName("Regional File")]
        public bool region { get { return regional; } }

        [ReadOnly(true), DisplayName("Arc Path")]
        public string Path { get; set; }

        [ReadOnly(true), DisplayName("Shared files")]
        public string[] SharedResources { get; }

        public List<string> _sharedResources = new List<string>();

        public long Offset;

        public uint CompressedSize;

        public uint DecompressedSize;

        private bool regional = false;

        public FileInformation(string ArcPath) : this(ArcPath, MainForm.SelectedRegion)
        {
            
        }

        public FileInformation(string ArcPath, int region)
        {
            if (MainForm.ArcFile.Initialized)
            {
                Path = ArcPath;
                long off;
                uint comp, decomp;
                MainForm.ArcFile.GetFileInformation(ArcPath, out off, out comp, out decomp, out regional, region);
                Offset = off;
                CompressedSize = comp;
                DecompressedSize = decomp;

                _sharedResources = MainForm.ArcFile.GetSharedFiles(Path, region);

                _sharedResources.Remove(Path); // don't display our own path

                SharedResources = _sharedResources.ToArray();
            }
        }
    }
}
