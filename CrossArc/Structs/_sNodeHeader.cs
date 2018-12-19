using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct _sNodeHeader
    {
        public uint FileSize;
        public uint FolderCount;
        public uint FileCount1;
        public uint FileNameCount;

        public uint SubFileCount;
        public uint LastTableCount;
        public uint HashFolderCount;
        public uint FileInformationCount;

        public uint FileCount2;
        public uint SubFileCount2;
        public uint unk1_10;
        public uint unk2_10;
        public byte AnotherHashTableSize;
        public byte unk11;
        public ushort unk12;

        public uint MovieCount;
        public uint Part1Count;
        public uint Part2Count;
        public uint MusicFileCount;
    }
}
