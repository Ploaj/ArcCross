using System.Runtime.InteropServices;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sDirectoryOffsets
    {
        public long Offset;
        public uint DecompSize;
        public uint Size;
        public uint SubDataStartIndex;
        public uint SubDataCount;
        public uint ResourceIndex;
    }
}
