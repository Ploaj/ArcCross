using System.Runtime.InteropServices;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sExtraFITable
    {
        public uint Unk1;
        public uint Unk2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sExtraFITable2
    {
        public uint Hash1;
        public uint Hash2;
    }
}
