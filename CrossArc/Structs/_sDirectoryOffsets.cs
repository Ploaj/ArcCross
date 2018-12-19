using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sDirectoryOffsets
    {
        public long Offset;
        public uint DecompSize;
        public uint Size;
        public uint SubDataStartIndex;
        public uint SubDataCount;
        public uint ResourceIndex;
    }
}
