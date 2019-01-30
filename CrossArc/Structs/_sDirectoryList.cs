using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sDirectoryList
    {
        public uint HashID;
        public uint DirOffsetIndex;
        public uint NameHash;
        public uint ParentHash;

        public uint Hash4;
        public uint FirstFilIndex;
        public uint unk;
        public uint FirstFileIndex;

        public uint FileInformationStartIndex;
        public uint FileInformationCount;
        public uint FileNameStartIndex;
        public short FileNameCount;
        public ushort Unk4;

        public uint Unk5;
    }
}
