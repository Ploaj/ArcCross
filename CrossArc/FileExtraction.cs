using CrossArc.GUI;
using System.IO;

namespace CrossArc
{
    public static class FileExtraction
    {
        public static readonly string[] RegionTags =
{
            "+jp_ja",
            "+us_en",
            "+us_fr",
            "+us_es",
            "+eu_en",
            "+eu_fr",
            "+eu_es",
            "+eu_de",
            "+eu_nl",
            "+eu_it",
            "+eu_ru",
            "+kr_ko",
            "+zh_cn",
            "+zh_tw"
        };

        public static string GetRegionalPath(string path)
        {
            return path.Replace(Path.GetExtension(path), RegionTags[MainForm.SelectedRegion] + Path.GetExtension(path));
        }

        public static void ExtractAllRegions(string path, string arcPath, bool decompressFiles, bool useOffsetName)
        {
            for (int regionIndex = 0; regionIndex < RegionTags.Length; regionIndex++)
            {
                var newPath = path.Replace(Path.GetExtension(path), RegionTags[regionIndex] + Path.GetExtension(path));

                SaveFile(newPath, arcPath, regionIndex, decompressFiles, useOffsetName);
            }
        }

        public static void SaveFile(string filepath, string arcpath, int regionIndex, bool decompressFiles, bool useOffsetName)
        {
            MainForm.ArcFile.GetFileInformation(arcpath, out long offset, out _, out _, out _, regionIndex);

            byte[] data;

            if (decompressFiles)
                data = MainForm.ArcFile.GetFile(arcpath, regionIndex);
            else
                data = MainForm.ArcFile.GetFileCompressed(arcpath, regionIndex);

            if (useOffsetName)
            {
                var extension = Path.GetExtension(filepath);
                filepath = filepath.Replace(extension, "_0x" + offset.ToString("X8") + extension);
            }

            File.WriteAllBytes(filepath, data);
        }
    }
}
