using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _SubFileInfo
    {
        public uint Offset;
        public uint CompSize;
        public uint DecompSize;
        public uint Flags;
    }
}
