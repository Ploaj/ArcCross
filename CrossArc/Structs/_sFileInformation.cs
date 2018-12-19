using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
}
