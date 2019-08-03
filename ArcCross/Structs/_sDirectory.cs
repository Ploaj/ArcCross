using System;
using System.Runtime.InteropServices;

namespace ArcCross
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sDirectoryList
    {
        public uint FullPathHash;
        public uint FullPathHashLengthAndIndex; // index to 0x922D

        public uint NameHash;
        public uint NameHashLength;

        public uint ParentFolderHash;
        public uint ParentFolderHashLength;

        public uint ExtraDisRe; // disposible and resident only 4 entries have this
        public uint ExtraDisReLength;

        public int FileInformationStartIndex;
        public int FileInformationCount;

        public int ChildDirectoryStartIndex;
        public int ChildDirectoryCount;

        public uint Flags; // TODO
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sDirectoryOffset
    {
        public long Offset;
        public uint UnknownSomeSize; // TODO: decompsize?
        public uint Size;
        public uint SubDataStartIndex;
        public uint SubDataCount;
        public uint RedirectIndex;
    }
}
