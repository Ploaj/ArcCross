using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArcCross
{
    [DebuggerDisplay("{PathString}")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformationPath
    {
        public uint Path;
        public uint DirectoryIndex;
        public uint Extension;
        public uint FileTableFlag;

        public uint Parent;
        public uint Unk5;
        public uint FileName;
        public uint Unk6;

        public string PathString { get
            {
                if(HashDict.Initialized)
                {
                    string pathString = HashDict.GetString(Parent, (int)(Unk5 & 0xFF));
                    string filename = HashDict.GetString(FileName, (int)(Unk6 & 0xFF));
                    if (filename.StartsWith("0x"))
                        filename += HashDict.GetString(Extension);

                    return pathString + filename;
                }
                return Path.ToString("X8");
            } 
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformationUnknownTable
    {
        public uint SomeIndex;
        public uint SomeIndex2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformationIndex
    {
        public uint DirectoryOffsetIndex;
        public uint FileInformationIndex;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformationSubIndex
    {
        public uint DirectoryOffsetIndex;
        public uint SubFileIndex;
        public uint FileInformationIndexAndFlag; // TODO figure out flag
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sFileInformationV2
    {
        public uint PathIndex; 
        public uint IndexIndex; 

        public uint SubIndexIndex;
        public uint Flags;
    }
}
