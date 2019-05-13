using System.Runtime.InteropServices;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformation
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

    // V2 Below here

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformationPath
    {
        public uint Path;
        public uint DirectoryIndex;
        public uint Extension;
        public uint FileTableFlag;

        public uint Parent;
        public uint Unk5;
        public uint Hash2;
        public uint Unk6;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformationIndex
    {
        public uint SomeIndex1;
        public uint SomeIndex2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformationSubIndex
    {
        public uint SomeIndex1;
        public uint SomeIndex2;
        public uint SomeIndex3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformationV2
    {
        public uint HashIndex; // index to unknown table in node header
        public uint HashIndex2; // index to UnkOffsetSizeCount in node header

        public uint SubFile_Index;
        public uint Flags;
    }
}
