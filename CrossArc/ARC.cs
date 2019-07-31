using CrossArc.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Zstandard.Net;

namespace CrossArc
{
    public class FileOffsetGroup
    {
        public long[] ArcOffset = new long[0xE];
        public long[] Offset = new long[0xE];
        public long[] CompSize = new long[0xE];
        public long[] DecompSize = new long[0xE];
        public uint[] Flags = new uint[0xE];
        public string Path;
        public string FileName;
        public uint FileNameHash;
        public uint PathHash;
        public long SubFileInformationOffset;
        public bool IsRegional;
    }

    public class ARC
    {
        public static readonly Dictionary<uint, string> MissingHashes = new Dictionary<uint, string>();
        public static readonly Dictionary<uint, string> UsedHashes = new Dictionary<uint, string>();

        public string FilePath { get; }

        private int arcVersion = 1;

        private _sArcHeader header;
        private _sDirectoryOffsets[] directoryOffsets1;
        private _sDirectoryOffsets[] directoryOffsets2;
        private _sDirectoryList[] directoryLists;
        private _sFileInformation[] fileInformation;
        private _sFolderHashIndex[] hashFolderCounts;
        private _SubFileInfo[] subFileInformation1;
        private _SubFileInfo[] subFileInformation2;

        // Stream Stuff
        private _sStreamHashToName[] streamHashToName;
        private _sStreamNameToHash[] streamNameToHash;
        private _sStreamIndexToFile[] streamIndexToFile;
        private _sStreamOffset[] streamOffsets;

        // Arc v2 exclusive
        private _sFileInformationV2[] fileInformationV2;
        private _sFileInformationPath[] fileInformationPath;
        private _sFileInformationIndex[] fileInformationIndex;
        private _sFileInformationSubIndex[] fileInformationSubIndex;

        // for speed
        private Dictionary<uint, _sDirectoryList> folderHashDict = new Dictionary<uint, _sDirectoryList>();
        private Dictionary<int, _sDirectoryOffsets> chunkHash1 = new Dictionary<int, _sDirectoryOffsets>();
        private Dictionary<int, _sDirectoryOffsets> chunkHash2 = new Dictionary<int, _sDirectoryOffsets>();

        private long subFileInformationStart;
        private long subFileInformationStart2;

        public ARC(string fileName)
        {
            FilePath = fileName;
            Open();
        }

        private void Open()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            MemoryStream decompTables = new MemoryStream();
            byte[] decompTableData;
            using (BinaryReader r = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                header = ByteToType<_sArcHeader>(r);

                // Reading First Section
                Debug.WriteLine(r.BaseStream.Position.ToString("X"));
                //compressed table
                //FileStream DecompTables = new FileStream("Tables", FileMode.Create);
                r.BaseStream.Seek(header.NodeSectionOffset, SeekOrigin.Begin);
                using (BinaryWriter writer = new BinaryWriter(decompTables))
                {
                    if (r.ReadUInt32() == 0x10)
                    {
                        // grab tables
                        r.ReadInt32();
                        int compSize = r.ReadInt32();
                        long nextTable = r.ReadInt32() + r.BaseStream.Position;
                        writer.Write(Decompress(r.ReadBytes(compSize)));
                        r.BaseStream.Position = nextTable;
                    }
                }
                decompTableData = decompTables.ToArray();
                if (decompTableData.Length == 0)
                {
                    r.BaseStream.Seek(header.NodeSectionOffset, SeekOrigin.Begin);
                    decompTableData = (r.ReadBytes((int)(r.BaseStream.Length - r.BaseStream.Position)));
                }
            }


            // hacky hacky paddy wacky
            if (header.NodeSectionOffset == 0x0375D6E118)
            {
                arcVersion = 3;
                ReadV2Arc(decompTableData);
            }
            else
            if (header.FileSectionOffset >= 0x8824AF68)
            {
                arcVersion = 2;
                ReadV2Arc(decompTableData);
            }
            else
                ReadV1Arc(decompTableData);

            stopwatch.Stop();
            Debug.WriteLine($"Initiating Arc: {stopwatch.ElapsedMilliseconds}");
        }

        private void ReadV1Arc(byte[] tableData)
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(tableData)))
            {
                reader.BaseStream.Position = 0;
                _sNodeHeader nodeHeader = ByteToType<_sNodeHeader>(reader);

                reader.BaseStream.Seek(0x68, SeekOrigin.Begin);

                // Hash Table
                reader.BaseStream.Seek(0x8 * nodeHeader.Part1Count, SeekOrigin.Current);

                // Hash Table 2
                streamNameToHash = ArcArrayReading.ReadSStreamNamesToHashes(reader, nodeHeader.Part1Count);

                // Hash Table 3
                streamIndexToFile = ArcArrayReading.ReadSStreamIndicesToFiles(reader, nodeHeader.Part2Count);

                // stream offsets
                streamOffsets = ArcArrayReading.ReadSStreamOffsets(reader, nodeHeader.MusicFileCount);

                // Another Hash Table
                Debug.WriteLine("RegionalHashList " + reader.BaseStream.Position.ToString("X"));
                reader.BaseStream.Seek(0xC * 0xE, SeekOrigin.Current);

                //folders

                Debug.WriteLine("FodlerHashes " + reader.BaseStream.Position.ToString("X"));
                directoryLists = ArcArrayReading.ReadSDirectoryLists(reader, nodeHeader.FolderCount);

                //file offsets

                Debug.WriteLine("fileoffsets " + reader.BaseStream.Position.ToString("X"));
                directoryOffsets1 = ArcArrayReading.ReadDirectoryOffsets(reader, nodeHeader.FileCount1);
                directoryOffsets2 = ArcArrayReading.ReadDirectoryOffsets(reader, nodeHeader.FileCount2);

                Debug.WriteLine("subfileoffset " + reader.BaseStream.Position.ToString("X"));
                hashFolderCounts = ArcArrayReading.ReadSFolderHashIndices(reader, nodeHeader.HashFolderCount);

                Debug.WriteLine("subfileoffset " + reader.BaseStream.Position.ToString("X"));
                fileInformation = ReadArray<_sFileInformation>(reader, nodeHeader.FileInformationCount);

                Debug.WriteLine("subfileoffset " + reader.BaseStream.Position.ToString("X"));
                subFileInformationStart = reader.BaseStream.Position;
                subFileInformation1 = ArcArrayReading.ReadSubFileInfos(reader, nodeHeader.SubFileCount);
                subFileInformationStart2 = reader.BaseStream.Position;
                subFileInformation2 = ArcArrayReading.ReadSubFileInfos(reader, nodeHeader.SubFileCount2);

                _sHashInt[] hashInts = ArcArrayReading.ReadHashInts(reader, nodeHeader.FolderCount);

                // okay some more file information
                uint fileHashCount = reader.ReadUInt32();
                uint unknownTableCount = reader.ReadUInt32();

                // TODO: Add methods to ArcArrayReading.cs
                _sExtraFITable[] extra1 = ReadArray<_sExtraFITable>(reader, unknownTableCount);
                _sExtraFITable2[] extra2 = ReadArray<_sExtraFITable2>(reader, fileHashCount);

                reader.BaseStream.Position += 8 * nodeHeader.FileInformationCount;

                // data.arc files will all have similar file counts, so the capacities can be estimated.
                folderHashDict = new Dictionary<uint, _sDirectoryList>(25838);
                foreach (_sDirectoryList fh in directoryLists)
                {
                    folderHashDict.Add(fh.HashID, fh);
                }

                chunkHash1 = new Dictionary<int, _sDirectoryOffsets>(479864);
                foreach (_sDirectoryOffsets chunk in directoryOffsets1)
                {
                    for (int i = 0; i < chunk.SubDataCount; i++)
                        chunkHash1[(int)chunk.SubDataStartIndex + i] = chunk;
                }

                chunkHash2 = new Dictionary<int, _sDirectoryOffsets>(14912);
                foreach (_sDirectoryOffsets chunk in directoryOffsets2)
                {
                    for (int i = 0; i < chunk.SubDataCount; i++)
                        chunkHash2[(int)chunk.SubDataStartIndex + i] = chunk;
                }
            }
        }

        private void ReadV2Arc(byte[] tableData)
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(tableData)))
            {
                reader.BaseStream.Position = 0;
                _sNodeHeaderv2 nodeHeader = ByteToType<_sNodeHeaderv2>(reader);
                //PrintStruct<_sNodeHeader>(NodeHeader);

                if (arcVersion == 3)
                    reader.BaseStream.Seek(0x58, SeekOrigin.Begin);
                else
                    reader.BaseStream.Seek(0x3C, SeekOrigin.Begin);

                // Unknown?? maybe something with languages
                reader.BaseStream.Seek(0xE * 12, SeekOrigin.Current);

                // Another structure
                _sNodeHeaderv2_2 nodeHeader2 = ByteToType<_sNodeHeaderv2_2>(reader);

                // Hash Table
                Console.WriteLine("Hash table 1 " + reader.BaseStream.Position.ToString("X"));
                reader.BaseStream.Seek(0x8 * nodeHeader2.Part1Count, SeekOrigin.Current);

                // Hash Table 2
                Console.WriteLine("StreamToHash table 2 " + reader.BaseStream.Position.ToString("X"));
                streamNameToHash = ArcArrayReading.ReadSStreamNamesToHashes(reader, nodeHeader2.Part1Count);

                // Hash Table 3
                Console.WriteLine("Hash table 3 " + reader.BaseStream.Position.ToString("X"));
                streamIndexToFile = ArcArrayReading.ReadSStreamIndicesToFiles(reader, nodeHeader2.Part2Count);

                // stream offsets
                Console.WriteLine("Hash table 4 " + reader.BaseStream.Position.ToString("X"));
                streamOffsets = ArcArrayReading.ReadSStreamOffsets(reader, nodeHeader2.Part3Size);

                Console.WriteLine("Hash table 5 " + reader.BaseStream.Position.ToString("X"));
                int unkCount1 = reader.ReadInt32();
                int unkCount2 = reader.ReadInt32();
                reader.BaseStream.Seek(0x8 * unkCount2, SeekOrigin.Current);
                reader.BaseStream.Seek(0x8 * unkCount1, SeekOrigin.Current);

                Debug.WriteLine("FilePathInfo " + reader.BaseStream.Position.ToString("X"));
                fileInformationPath = ArcArrayReading.ReadSFileInformationPaths(reader, nodeHeader.UnkCount);

                // Start of first node header section
                Debug.WriteLine("SomeIndicesForFileInformation " + reader.BaseStream.Position.ToString("X"));
                fileInformationIndex = ArcArrayReading.ReadSFileInformationIndices(reader, nodeHeader.UnkOffsetSizeCount);

                // Start of first node header section
                Debug.WriteLine("FolderHashes " + reader.BaseStream.Position.ToString("X"));
                reader.BaseStream.Seek(0x8 * nodeHeader.FolderCount, SeekOrigin.Current);

                //folders

                Debug.WriteLine("FolderHashes " + reader.BaseStream.Position.ToString("X"));
                directoryLists = ArcArrayReading.ReadSDirectoryLists(reader, nodeHeader.FolderCount);

                //file offsets

                Debug.WriteLine("fileoffsets " + reader.BaseStream.Position.ToString("X"));
                directoryOffsets1 = ArcArrayReading.ReadDirectoryOffsets(reader, nodeHeader.FileCount1);
                directoryOffsets2 = ArcArrayReading.ReadDirectoryOffsets(reader, nodeHeader.FileCount2);

                Debug.WriteLine("hashfolderoffset " + reader.BaseStream.Position.ToString("X"));
                hashFolderCounts = ArcArrayReading.ReadSFolderHashIndices(reader, nodeHeader.HashFolderCount);

                Debug.WriteLine("fileinformationoffset " + reader.BaseStream.Position.ToString("X"));
                fileInformationV2 = ArcArrayReading.ReadSFileInformationV2(reader, nodeHeader.FileInformationCount + nodeHeader.SubFileCount2);

                Debug.WriteLine("sub index table " + reader.BaseStream.Position.ToString("X"));
                fileInformationSubIndex = ArcArrayReading.ReadSFileInformationSubIndices(reader, nodeHeader.LastTableCount + nodeHeader.SubFileCount2);

                Debug.WriteLine("subfileoffset " + reader.BaseStream.Position.ToString("X"));
                subFileInformationStart = reader.BaseStream.Position;
                subFileInformation1 = ArcArrayReading.ReadSubFileInfos(reader, nodeHeader.SubFileCount);

                subFileInformationStart2 = reader.BaseStream.Position;
                subFileInformation2 = ArcArrayReading.ReadSubFileInfos(reader, nodeHeader.SubFileCount2);

                int max = 0;
                foreach (var dir in fileInformationSubIndex)
                {
                    max = Math.Max(max, (int)dir.SomeIndex1);
                }
                Console.WriteLine($"Directory index {max.ToString("X")} {directoryLists.Length.ToString("X")} {(directoryOffsets1.Length + directoryOffsets2.Length).ToString("X")}");

                // data.arc files will all have similar file counts, so the capacities can be estimated.
                folderHashDict = new Dictionary<uint, _sDirectoryList>(28664);
                foreach (_sDirectoryList fh in directoryLists)
                {
                    folderHashDict.Add(fh.HashID, fh);
                }

                chunkHash1 = new Dictionary<int, _sDirectoryOffsets>(226756);
                foreach (_sDirectoryOffsets chunk in directoryOffsets1)
                {
                    for (int i = 0; i < chunk.SubDataCount; i++)
                    {
                        chunkHash1[(int)chunk.SubDataStartIndex + i] = chunk;
                    }
                }

                chunkHash2 = new Dictionary<int, _sDirectoryOffsets>(15614);
                foreach (_sDirectoryOffsets chunk in directoryOffsets2)
                {
                    for (int i = 0; i < chunk.SubDataCount; i++)
                    {
                        chunkHash2[(int)chunk.SubDataStartIndex + i] = chunk;
                    }
                }
            }
        }

        public List<FileOffsetGroup> GetStreamFiles()
        {
            var streamFiles = new List<FileOffsetGroup>(streamNameToHash.Length);

            foreach (var streamFile in streamNameToHash)
            {
                FileOffsetGroup group = new FileOffsetGroup();
                group.FileName = GetName(streamFile.Hash);

                if (streamFile.Flags == 1 || streamFile.Flags == 2)
                {
                    int size = 0xE;
                    if (streamFile.Flags == 2)
                        size = 0x5;
                    for (int i = 0; i < size; i++)
                    {
                        var streamIndex = streamIndexToFile[(streamFile.NameIndex >> 8) + i].FileIndex;
                        var offset = streamOffsets[streamIndex];
                        group.ArcOffset[i] = offset.Offset;
                        group.CompSize[i] = offset.Size;
                        group.DecompSize[i] = offset.Size;
                        group.Flags[i] = streamFile.Flags;
                    }
                }
                else
                {
                    var streamIndex = streamIndexToFile[streamFile.NameIndex >> 8].FileIndex;
                    var offset = streamOffsets[streamIndex];
                    group.ArcOffset[0] = offset.Offset;
                    group.CompSize[0] = offset.Size;
                    group.DecompSize[0] = offset.Size;
                    group.Flags[0] = streamFile.Flags;
                }

                streamFiles.Add(group);
            }
            return streamFiles;
        }


        public List<FileOffsetGroup> GetFiles()
        {
            Console.WriteLine($"Version: {arcVersion}, V2: {(arcVersion == 2)}");
            if (arcVersion == 3)
                return GetFilesV2();
            else if (arcVersion == 2)
                return GetFilesV2();
            else
                return GetFilesV1();
        }

        private List<FileOffsetGroup> GetFilesV1()
        {
            List<FileOffsetGroup> files = new List<FileOffsetGroup>(fileInformation.Length);
            foreach (var item in fileInformation)
            {
                var directory = directoryLists[item.DirectoryIndex >> 8];
                var offsetGroup = GetFileInformation(item, 0);
                if ((item.FileTableFlag >> 8) > 0)
                {
                    offsetGroup.IsRegional = true;

                    for (int i = 1; i < 0xE; i++)
                    {
                        var regionalOffsetGroup = GetFileInformation(item, i);
                        UpdateOffsetGroupArrays(offsetGroup, regionalOffsetGroup, i);
                    }
                }

                offsetGroup.FileNameHash = item.Hash2;
                offsetGroup.PathHash = item.Parent;

                string extension = GetExtensionV1(item);

                offsetGroup.FileName = GetName(item.Hash2, extension);
                offsetGroup.Path = GetName(item.Parent);
                files.Add(offsetGroup);
            }
            return files;
        }

        private static string GetExtensionV1(_sFileInformation item)
        {
            HashDict.TryGetValue(item.Extension, out string extension);
            if (extension != "")
                extension = "." + extension;
            return extension;
        }

        private List<FileOffsetGroup> GetFilesV2()
        {
            List<FileOffsetGroup> files = new List<FileOffsetGroup>(fileInformationV2.Length);
            Console.WriteLine("Files " + fileInformationV2.Length);

            foreach (var item in fileInformationV2)
            {
                var offsetGroup = CreateFileOffsetGroup(item);
                files.Add(offsetGroup);
            }
            return files;
        }

        private FileOffsetGroup CreateFileOffsetGroup(_sFileInformationV2 item)
        {
            var offsetGroup = GetFileInformation(item, 0);
            if ((item.Flags & 0xF000) == 0x8000)
            {
                offsetGroup.IsRegional = true;

                for (int i = 1; i < 0xE; i++)
                {
                    var regionalOffsetGroup = GetFileInformation(item, i);
                    UpdateOffsetGroupArrays(offsetGroup, regionalOffsetGroup, i);
                }
            }

            var pathInfo = fileInformationPath[item.HashIndex];
            offsetGroup.FileNameHash = pathInfo.Hash2;
            offsetGroup.PathHash = pathInfo.Parent;

            string extension = GetExtensionV2(pathInfo);

            offsetGroup.FileName = GetName(pathInfo.Hash2, extension);
            offsetGroup.Path = GetName(pathInfo.Parent);
            return offsetGroup;
        }

        private static string GetExtensionV2(_sFileInformationPath pathInfo)
        {
            HashDict.TryGetValue(pathInfo.Extension, out string extension);
            if (extension != "")
                extension = "." + extension;
            return extension;
        }

        private static void UpdateOffsetGroupArrays(FileOffsetGroup target, FileOffsetGroup source, int index)
        {
            target.ArcOffset[index] = source.ArcOffset[0];
            target.Offset[index] = source.Offset[0];
            target.CompSize[index] = source.CompSize[0];
            target.DecompSize[index] = source.DecompSize[0];
            target.Flags[index] = source.Flags[0];
        }

        private FileOffsetGroup GetFileInformation(_sFileInformation fileInfo, int regionalIndex)
        {
            return GetFileInformation(fileInfo.SubFile_Index, (fileInfo.FileTableFlag >> 8), directoryLists[fileInfo.DirectoryIndex >> 8].DirOffsetIndex >> 8, regionalIndex);
        }

        private FileOffsetGroup GetFileInformation(_sFileInformationV2 fileInfo, int regionalIndex)
        {
            bool regional = fileInfo.Flags == 0x8010;
            var path = fileInformationPath[fileInfo.HashIndex];
            var dirinfo = fileInformationIndex[fileInfo.HashIndex2 + (regional ? regionalIndex + 1 : 0)];
            var subinfo = fileInformationSubIndex[fileInfo.SubFile_Index + (regional ? regionalIndex + 1 : 0)];
            return GetFileInformation(subinfo.SomeIndex2, 0, subinfo.SomeIndex1, regionalIndex);
        }

        private FileOffsetGroup GetFileInformation(uint subFileIndex, uint regionalOffset, uint directoryIndex, int regionalIndex = 0)
        {
            FileOffsetGroup g = new FileOffsetGroup();

            // Get File Data

            _SubFileInfo fileOffset;
            if (subFileIndex < subFileInformation1.Length)
                fileOffset = subFileInformation1[subFileIndex];
            else
                fileOffset = subFileInformation2[subFileIndex - subFileInformation1.Length];

            _sDirectoryOffsets directoryOffset;
            if (directoryIndex < directoryOffsets1.Length)
                directoryOffset = directoryOffsets1[directoryIndex];
            else
                directoryOffset = directoryOffsets2[directoryIndex - directoryOffsets1.Length];
            var directoryOffset2 = directoryOffset;

            if (regionalOffset > 0)
            {
                fileOffset = subFileInformation1[regionalOffset + regionalIndex];
                directoryIndex = (uint)(directoryIndex + 1 + regionalIndex);
                if (directoryIndex < directoryOffsets1.Length)
                    directoryOffset = directoryOffsets1[directoryIndex];
                else
                    directoryOffset = directoryOffsets2[directoryIndex - directoryOffsets1.Length];
                directoryOffset2 = directoryOffset;
            }
            else
            if ((directoryOffset.ResourceIndex & 0xFFFFFF) != 0xFFFFFF)
            {
                if (directoryOffset.ResourceIndex < directoryOffsets1.Length)
                    directoryOffset2 = directoryOffsets1[((directoryOffset.ResourceIndex) & 0xFFFFFF)];
                else
                    directoryOffset2 = directoryOffsets2[((directoryOffset.ResourceIndex - directoryOffsets1.Length) & 0xFFFFFF)];

            }

            // Parse Flags
            int flag = ((int)fileOffset.Flags >> 24);
            bool compressed = (flag & 0x03) == 0x03;
            bool external = (flag & 0x08) == 0x08;
            int externalOffset = (int)fileOffset.Flags & 0xFFFFFF;

            if ((arcVersion >= 2))
            {
                // why did they do this
                flag = ((int)fileOffset.Flags & 0xFF);
                compressed = (flag & 0x03) == 0x03;
                external = (flag & 0x08) == 0x08;
                externalOffset = (int)fileOffset.Flags >> 8;
            }

            if (externalOffset > 0 && !external)
            {
                if (arcVersion >= 2)
                {
                    return GetFileInformation(fileInformationV2[externalOffset], 0);
                }
                else
                {
                    return GetFileInformation(fileInformation[externalOffset], 0);
                }
            }

            if (external)
            {
                //hack
                directoryOffset2 = chunkHash2[externalOffset];
                fileOffset = subFileInformation2[externalOffset];

                g.ArcOffset[0] = header.FileSectionOffset + directoryOffset2.Offset + (fileOffset.Offset << 2);
                g.Offset[0] = fileOffset.Offset;
                g.CompSize[0] = fileOffset.CompSize;
                g.DecompSize[0] = fileOffset.DecompSize;
                g.Flags[0] = fileOffset.Flags;
                g.SubFileInformationOffset = subFileInformationStart2 + externalOffset * 16;
            }
            else
            {
                g.ArcOffset[0] = header.FileSectionOffset + directoryOffset.Offset + (fileOffset.Offset << 2);
                g.Offset[0] = fileOffset.Offset;
                g.CompSize[0] = fileOffset.CompSize;
                g.DecompSize[0] = fileOffset.DecompSize;
                g.Flags[0] = fileOffset.Flags;
                g.SubFileInformationOffset = subFileInformationStart + subFileIndex * 16;
            }

            return g;
        }

        public string[] GetPaths()
        {
            List<string> paths = new List<string>();

            foreach (_sFolderHashIndex fhi in hashFolderCounts)
            {
                paths.Add(GetPathString(folderHashDict, folderHashDict[fhi.Hash]));
            }

            return paths.ToArray();
        }

        public void CommandFunctions(string folderToExtract)
        {
            using (BinaryReader r = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                if (!folderToExtract.Equals(""))
                {
                    _sDirectoryList toExtract;
                    if (folderHashDict.TryGetValue(CRC32.Crc32C(folderToExtract), out toExtract))
                    {
                        Console.WriteLine("Extracting " + GetPathString(folderHashDict, toExtract));
                        ExtractFolder(r, toExtract);
                    }
                    else
                    {
                        foreach (_sDirectoryList h in directoryLists)
                        {
                            if (GetPathString(folderHashDict, h).Equals(folderToExtract))
                            {
                                ExtractFolder(r, h);
                                break;
                            }
                        }
                        Debug.WriteLine("Could no extract " + folderToExtract);
                    }
                }
                else
                {
                    foreach (_sDirectoryList h in directoryLists)
                    {
                        Console.WriteLine("Extracting " + GetPathString(folderHashDict, h));
                        ExtractFolder(r, h);
                    }
                }
            }

        }


        public void HashCheck()
        {
            foreach (_sDirectoryList fh in directoryLists)
            {
                GetPathString(folderHashDict, fh);
            }
            foreach (_sFileInformation fi in fileInformation)
            {
                GetName(fi.Hash2);
                GetName(fi.Extension);
                GetName(fi.Parent);
            }

            StreamWriter w = new StreamWriter(new FileStream("UsedString.txt", FileMode.Create));
            foreach (uint i in UsedHashes.Keys)
            {
                w.WriteLine(UsedHashes[i]);
            }
            w.Close();
            w = new StreamWriter(new FileStream("UnusedString.txt", FileMode.Create));
            foreach (uint i in MissingHashes.Keys)
            {
                w.WriteLine(i.ToString("X"));
            }
            w.Close();

            Console.WriteLine("Total Hashes: " + (MissingHashes.Count + UsedHashes.Count) + " Found: " + UsedHashes.Count + " Missing: " + MissingHashes.Count + " Percent Found: " + ((UsedHashes.Count) / (float)(UsedHashes.Count + MissingHashes.Count)) * 100);

        }

        private void ExtractFolder(BinaryReader stream, _sDirectoryList folderHash)
        {
            for (int i = 0; i < folderHash.FileInformationCount; i++)
            {
                _sFileInformation info = fileInformation[folderHash.FileInformationStartIndex + i];

                string finalPath = GetPathString(folderHashDict, folderHash) + "/" + info.Parent.ToString("X");
                //Directory.CreateDirectory(FinalPath);

                _SubFileInfo sub = subFileInformation1[info.SubFile_Index];
                //Debug.WriteLine("\tFolder - " + info.Unk4.ToString("X"));
                //Debug.WriteLine("\t\t" + GetName(info.Hash2) + " " + sub.Flags.ToString("X"));

                GetName(info.Hash2);
                FileOffsetGroup group = GetOffsetFromSubFile(info);
                //Debug.WriteLine("\t\t\tArcOffset: 0x" + group.ArcOffset.ToString("X") + " Offset: " + group.Offset.ToString("X") + " CompSize: 0x" + group.CompSize.ToString("X") + " DecompSize:" + group.DecompSize.ToString("X") + " Flags: 0x" + group.Flags.ToString("X"));

                ExportFile(finalPath + "/" + GetName(info.Hash2), stream, group.ArcOffset[0], group.CompSize[0], group.DecompSize[0]);
            }
        }

        public static void ExportFile(string filepath, BinaryReader r, long offset, long compSize, long decompSize)
        {
            File.WriteAllBytes(filepath, GetFileData(r, offset, compSize, decompSize));
        }

        private static byte[] GetFileData(BinaryReader r, long offset, long compSize, long decompSize)
        {
            r.BaseStream.Seek(offset, SeekOrigin.Begin);

            byte[] data = r.ReadBytes((int)compSize);

            if (compSize != decompSize && decompSize > 0 && compSize > 0)
            {
                try
                {
                    data = Decompress(data);
                }
                catch (IOException)
                {
                    Console.WriteLine("Error: Could Not Decompress File ");
                }
            }

            if (decompSize != data.Length)
                Console.WriteLine("Error: Filesize mismatch ");

            return data;
        }


        public byte[] GetFileData(long offset, long compSize, long decompSize)
        {
            using (BinaryReader r = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                return GetFileData(r, offset, compSize, decompSize);
            }
        }

        // TODO: Move this method
        public static byte[] Decompress(byte[] compressed)
        {
            using (var memoryStream = new MemoryStream(compressed))
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
            using (var temp = new MemoryStream())
            {
                compressionStream.CopyTo(temp);
                return temp.ToArray();
            }
        }

        private FileOffsetGroup GetOffsetFromSubFile(_sFileInformation fileInfo)
        {
            FileOffsetGroup g = new FileOffsetGroup();
            int subIndex = (int)fileInfo.SubFile_Index;
            _SubFileInfo subfile = subFileInformation1[subIndex];

            int flag = ((int)subfile.Flags >> 24);
            bool useTable2 = (flag & 0xFF) == 0x08 || (flag & 0xFF) == 0x0B;
            int externalOffset = (int)subfile.Flags & 0xFFFFFF;

            if (flag == 0x03 && externalOffset > 0)
            {
                //Debug.WriteLine("\tEmpty?------------");
                //Debug.WriteLine("\t\t" + subfile.Offset.ToString("X") + " " + subfile.CompSize.ToString("X") + " " + subfile.DecompSize.ToString("X"));
            }

            if (externalOffset > 0 && !useTable2)
            {
                //Debug.WriteLine("\t\t External ->" + GetName(FileInformation[ExternalOffset].Hash2));
                return GetOffsetFromSubFile(fileInformation[externalOffset]);
            }
            Dictionary<int, _sDirectoryOffsets> offsets = chunkHash1;
            if (useTable2)
            {
                offsets = chunkHash2;
                subIndex = (int)(subfile.Flags & 0xFFFFFF);
                subfile = subFileInformation2[subIndex];
            }

            //foreach (_sFileChunkOffsets chunk in Offsets)
            _sDirectoryOffsets chunk = offsets[subIndex];
            {
                if (subIndex >= chunk.SubDataStartIndex && subIndex < chunk.SubDataStartIndex + chunk.SubDataCount)
                {
                    g.ArcOffset = new long[] { (header.FileSectionOffset + chunk.Offset + (subfile.Offset << 2)) };
                    g.Offset = new long[] { subfile.Offset };// (Header.FileSectionOffset + chunk.Offset + (subfile.Offset << 2));
                    g.CompSize = new long[] { subfile.CompSize };
                    g.DecompSize = new long[] { subfile.DecompSize };
                    g.Flags = new uint[] { subfile.Flags };
                    //break;
                }
            }

            return g;
        }

        public static string GetPathString(Dictionary<uint, _sDirectoryList> hashBank, _sDirectoryList folder)
        {
            string folder1 = "";
            string folder2 = "";
            string folder3 = "";

            if (folder.HashID != folder.NameHash && folder.NameHash != 0 && hashBank.ContainsKey(folder.NameHash))
                folder3 = GetPathString(hashBank, hashBank[folder.NameHash]) + "/";
            else if (folder.NameHash != 0)
                folder3 = GetName(folder.NameHash);

            if (hashBank.ContainsKey(folder.Hash4))
                folder2 = GetPathString(hashBank, hashBank[folder.Hash4]) + "/";
            else if (folder.Hash4 != 0)
                folder3 = GetName(folder.Hash4);

            /*if (Folder.Hash4 != 0 && Folder.Hash4 != 0 && HashBank.ContainsKey(Folder.Hash4))
                Folder1 = GetPathString(HashBank, HashBank[Folder.Hash4]) + "/";
            else if (Folder.Hash4 != 0)
                Folder3 = GetName(Folder.Hash4);*/

            return folder1 + folder2 + folder3;
        }

        public static string GetName(uint hash, string extension = "")
        {
            string name = "";
            if (HashDict.TryGetValue(hash, out name))
            {
                //if (!UsedHashes.ContainsKey(Hash))
                //    UsedHashes.Add(Hash, name);
                return name;
            }

            //if (!MissingHashes.ContainsKey(Hash))
            //    MissingHashes.Add(Hash, name);
            return "0x" + hash.ToString("X") + extension;
        }

        // TODO: Move this method
        public static T[] ReadArray<T>(BinaryReader reader, uint size)
        {
            // slow af but whatevs
            T[] arr = new T[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = ByteToType<T>(reader);
            }
            return arr;
        }

        // TODO: Move this method
        public static T ByteToType<T>(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        public struct HashCompareCheck
        {
            public uint Name;
            public uint Path;
            public uint Hash;
        }

        public static void CompareHashes(string previousHashFileName, string currentHashFileName)
        {
            Dictionary<long, HashCompareCheck> previousHashes = new Dictionary<long, HashCompareCheck>();
            List<HashCompareCheck> hash2 = new List<HashCompareCheck>();

            using (BinaryReader reader = new BinaryReader(new FileStream(previousHashFileName, FileMode.Open)))
            {
                reader.ReadUInt32();
                uint fileCount = reader.ReadUInt32();
                fileCount = (uint)(reader.BaseStream.Length - 8) / 12;
                var hashes = ReadArray<HashCompareCheck>(reader, fileCount);
                foreach (var f in hashes)
                {
                    long key = ((long)f.Name << 32) | f.Path;
                    if (!previousHashes.ContainsKey(key))
                        previousHashes.Add(key, f);
                    else
                    {
                        string path = "";
                        if (!HashDict.TryGetValue(f.Path, out path))
                            path = "0x" + f.Path.ToString("X");
                        string name = "";
                        if (!HashDict.TryGetValue(f.Name, out name))
                            name = "0x" + f.Name.ToString("X");
                        Console.WriteLine($"Duplicate: {path}{name}");
                        //PreviousHashes[key] = f;
                    }
                }
            }

            using (BinaryReader reader = new BinaryReader(new FileStream(currentHashFileName, FileMode.Open)))
            {
                reader.ReadUInt32();
                uint fileCount = reader.ReadUInt32();
                fileCount = (uint)(reader.BaseStream.Length - 8) / 12;
                var hashes = ReadArray<HashCompareCheck>(reader, fileCount);
                Dictionary<long, object> keys = new Dictionary<long, object>();
                foreach (var f in hashes)
                {
                    long key = ((long)f.Name << 32) | f.Path;
                    if (!keys.ContainsKey(key))
                    {
                        hash2.Add(f);
                        keys.Add(key, f);
                    }
                }
            }

            List<string> changed = new List<string>();
            List<string> New = new List<string>();


            int count = 0;
            int percent = 1;

            foreach (HashCompareCheck c in hash2)
            {
                count++;
                if (count == hash2.Count / 100)
                {
                    Console.WriteLine($"{percent}% done");
                    percent++;
                    count = 0;
                }
                string path = "";
                if (!HashDict.TryGetValue(c.Path, out path))
                    path = "0x" + c.Path.ToString("X");
                string name = "";
                if (!HashDict.TryGetValue(c.Name, out name))
                    name = "0x" + c.Name.ToString("X");
                long key = ((long)c.Name << 32) | c.Path;
                if (previousHashes.ContainsKey(key))
                {
                    if (previousHashes[key].Hash != c.Hash)
                        changed.Add(path + name);
                    previousHashes.Remove(key);
                }
                else
                {
                    New.Add(path + name);
                }
            }

            using (StreamWriter writer = new StreamWriter(new FileStream("ChangeLog.txt", FileMode.Create)))
            {
                foreach (var f in New)
                {
                    writer.WriteLine($"Added: {f}");
                }
                foreach (var f in changed)
                {
                    writer.WriteLine($"Changed: {f}");
                }
            }
        }

        public void CreateHashCompare(string outFileName)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                using (BinaryWriter writer = new BinaryWriter(new FileStream(outFileName, FileMode.Create)))
                {
                    var files = GetFiles();
                    writer.Write("CRCH".ToCharArray());
                    writer.Write(files.Count);
                    int percentSlice = files.Count / 100;
                    int fileCount = 0;
                    int percent = 0;
                    foreach (var file in files)
                    {
                        if (fileCount >= percentSlice)
                        {
                            fileCount = 0;
                            Console.WriteLine($"{percent}% Done");
                            percent++;
                        }
                        if (file.CompSize[0] == 0 && file.DecompSize[0] == 0)
                            continue;
                        fileCount++;
                        writer.Write(file.FileNameHash);
                        writer.Write(file.PathHash);
                        reader.BaseStream.Position = file.ArcOffset[0];
                        byte[] data = reader.ReadBytes((int)file.CompSize[0]);
                        writer.Write(CRC32.Crc32C(data));
                    }
                    writer.BaseStream.Position = 4;
                    writer.Write(fileCount);
                }
            }

        }
    }
}
