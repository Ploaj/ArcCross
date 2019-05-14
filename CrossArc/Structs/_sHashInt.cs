using System.Runtime.InteropServices;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sHashInt
    {
        public uint Hash;
        public uint Index;
    }
}
