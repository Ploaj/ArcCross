using System.Runtime.InteropServices;

namespace CrossArc.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct _sArcHeader
    {
        public long Hash;
        public long MusicFileSectionOffset;
        public long FileSectionOffset;
        public long FileSection2Offset;
        public long NodeSectionOffset;
        public long UnkSectionOffset;
    }
}
