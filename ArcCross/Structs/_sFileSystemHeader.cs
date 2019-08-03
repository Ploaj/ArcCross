using System;
using System.Runtime.InteropServices;

namespace ArcCross
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileSystemHeader
    {
        public uint TableFileSize;
        public uint FileInformationPathCount;
        public uint FileInformationIndexCount;
        public uint DirectoryCount;

        public uint DirectoryOffsetCount1;

        public uint DirectoryHashSearchCount;
        public uint FileInformationCount;
        public uint FileInformationSubIndexCount;
        public uint SubFileCount;

        public uint DirectoryOffsetCount2;
        public uint SubFileCount2;
        public uint padding; // padding

        public uint unk1_10; // both always 0x10
        public uint unk2_10;
        public byte RegionalCount1; // 0xE
        public byte RegionalCount2; // 0x5
        public ushort padding2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sStreamHeader
    {
        public uint UnkCount;
        public uint StreamHashCount;
        public uint StreamIndexToOffsetCount;
        public uint StreamOffsetCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sStreamUnk
    {
        public uint Hash;
        public uint LengthAndSize;
        public uint Index;
    }
}
