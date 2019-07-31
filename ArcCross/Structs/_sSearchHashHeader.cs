using System.Runtime.InteropServices;

namespace ArcCross
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public struct _sSearchHashHeader
    {
        public ulong SectionSize;
        public uint FolderLengthHashCount;
        public uint SomeCount3;
        public uint SomeCount4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public struct _sHashIndexGroup
    {
        public uint Hash;
        public int index;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public struct _sHashGroup
    {
        public uint FilePathHash;
        public int FilePathLengthAndIndex;
        public uint FolderHash;
        public int FolderHashLengthAndIndex;
        public uint FileNameHash;
        public int FileNameLength;
        public uint ExtensionHash;
        public uint ExtensionLength;
    }
}
