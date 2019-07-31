using System.Runtime.InteropServices;

namespace ArcCross
{
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sCompressedTableHeader
    {
        public uint DataOffset;
        public int DecompressedSize;
        public int CompressedSize;
        public int SectionSize;
    }
}
