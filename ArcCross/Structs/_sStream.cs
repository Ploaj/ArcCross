using System;
using System.Runtime.InteropServices;

namespace ArcCross
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct _sStreamHashToName
    {
        public uint Hash;
        public uint NameIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct _sStreamNameToHash
    {
        public uint Hash;
        public uint NameIndex;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct _sStreamIndexToOffset
    {
        public int FileIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct _sStreamOffset
    {
        public long Size;
        public long Offset;
    }
}
