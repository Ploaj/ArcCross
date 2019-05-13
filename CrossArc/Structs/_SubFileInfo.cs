using System.Runtime.InteropServices;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _SubFileInfo
    {
        public uint Offset;
        public uint CompSize;
        public uint DecompSize;
        public uint Flags;
    }
}
