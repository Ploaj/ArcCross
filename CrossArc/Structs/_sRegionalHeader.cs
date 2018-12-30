using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sRegionalHeader
    {
        public long SectionSize;
        public uint Count1;
        public uint Count2;
        public uint Count2_2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sRegionalHash
    {
        public uint Flag;
        public uint Hash;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sRegionalGroup
    {
        public uint Hash1;
        public uint Unk1;
        public uint Hash2;
        public uint Unk2;
        public uint Hash3;
        public uint Unk3;
        public uint Offset;
        public uint Count;
    }
}
