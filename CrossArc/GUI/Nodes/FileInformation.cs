using System;
using System.ComponentModel;

namespace CrossArc.GUI.Nodes
{
    public class FileInformation
    {
        [ReadOnly(true), DisplayName("Arc Offset")]
        public string ArcOffset { get { return Offset.ToString("X"); } }

        [ReadOnly(true), DisplayName("Compressed Size")]
        public string comp { get { return CompressedSize.ToString("X"); } }

        [ReadOnly(true), DisplayName("Decompressed Size")]
        public string decomp { get { return DecompressedSize.ToString("X"); } }

        [ReadOnly(true), DisplayName("Regional File")]
        public string region { get { return regional.ToString(); } }

        [ReadOnly(true), DisplayName("Arc Path")]
        public string Path { get; set; }

        public long Offset;

        public uint CompressedSize;

        public uint DecompressedSize;

        private bool regional = false;

        public FileInformation(string ArcPath)
        {
            if (MainForm.ArcFile.Initialized)
            {
                Path = ArcPath;
                long off;
                uint comp, decomp;
                MainForm.ArcFile.GetFileInformation(ArcPath, out off, out comp, out decomp, out regional, MainForm.SelectedRegion);
                Offset = off;
                CompressedSize = comp;
                DecompressedSize = decomp;
            }
        }
    }
}
