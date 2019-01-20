
namespace CrossArc.GUI
{
    public class ArcExtractInformation
    {
        public string FilePath;
        public long ArcOffset;
        public uint CompSize;
        public uint DecompSize;

        public ArcExtractInformation(string filePath, long arcOffset, uint compSize, uint decompSize)
        {
            FilePath = filePath;
            ArcOffset = arcOffset;
            CompSize = compSize;
            DecompSize = decompSize;
        }
    }
}
