using System.Runtime.InteropServices;

namespace ArcCross
{
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _sArcHeader
    {
        public ulong Magic;
        public long MusicDataOffset;
        public long FileDataOffset;
        public long FileDataOffset2;
        public long FileSystemOffset;
        public long FileSystemSearchOffset;
        public long Padding;
    }
}
