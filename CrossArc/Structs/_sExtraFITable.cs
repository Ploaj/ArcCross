using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sExtraFITable
    {
        public uint Unk1;
        public uint Unk2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sExtraFITable2
    {
        public uint Hash1;
        public uint Hash2;
    }
}
