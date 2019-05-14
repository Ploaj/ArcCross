using System.Runtime.InteropServices;

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

    [StructLayout(LayoutKind.Sequential)]
    public struct _sNodeHeaderv2
    {
        public uint FileSize;
        public uint UnkCount;
        public uint UnkOffsetSizeCount;
        public uint FolderCount;
        
        public uint FileCount1;

        public uint HashFolderCount;
        public uint FileInformationCount;
        public uint LastTableCount;
        public uint SubFileCount;

        public uint FileCount2;
        public uint SubFileCount2;
        public uint unk11;

        public uint unk1_10;
        public uint unk2_10;
        public uint unk13;
        public uint unk14;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _sNodeHeaderv2_2
    {
        public uint unk3;
        public uint Part1Count;
        public uint Part2Count;
        public uint Part3Size;

        public uint unk5;
        public uint unk6;
        public uint unk7;
        public uint unk8;

        public uint unk9;
        public uint unk10;
        public uint unk11;

        public uint unk12;
        public uint unk13;
    }
}
