using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct _sArcHeader
    {
        public long Hash;
        public long MusicFileSectionOffset;
        public long FileSectionOffset;
        public long MusicSectionOffset;
        public long NodeSectionOffset;
        public long UnkSectionOffset;
    }
}
