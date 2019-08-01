using System.Runtime.InteropServices;

namespace ArcCross.StructsV1
{
    [StructLayout(LayoutKind.Sequential)]
    public struct _sFileSystemHeaderV1
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
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformationV1
    {
        public uint Path;
        public uint DirectoryIndex;
        public uint Extension;
        public uint FileTableFlag;

        public uint Parent;
        public uint Unk5;
        public uint Hash2;
        public uint Unk6;

        public uint SubFile_Index;
        public uint Flags;
    }
}
