using System;
using System.Runtime.InteropServices;

namespace ArcCross
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    public struct _sSubFileInfo
    {
        public uint Offset;
        public uint CompSize;
        public uint DecompSize;
        public uint Flags; // 0x03 if compressed
    }
}
