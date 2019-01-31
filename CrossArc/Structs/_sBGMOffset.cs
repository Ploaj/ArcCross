using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrossArc.Structs
{

    [StructLayout(LayoutKind.Sequential)]
    public struct _sStreamHashToName
    {
        public uint Hash;
        public uint NameIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _sStreamNameToHash
    {
        public uint Hash;
        public uint NameIndex;
        public uint Unk;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _sStreamIndexToFile
    {
        public int FileIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _sStreamOffset
    {
        public long Size;
        public long Offset;
    }
}
