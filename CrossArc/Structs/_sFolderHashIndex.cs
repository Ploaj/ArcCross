using System.Runtime.InteropServices;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFolderHashIndex
    {
        public uint Hash;
        public uint Count;
    }
}
