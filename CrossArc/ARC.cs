using System;
using System.Collections.Generic;
using CrossArc.Structs;
using System.IO;
using System.Diagnostics;
using Zstandard.Net;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Windows.Forms;
using System.Linq;

namespace CrossArc
{
    public struct FileOffsetGroup
    {
        public long[] ArcOffset;
        public long[] Offset;
        public long[] CompSize;
        public long[] DecompSize;
        public uint[] Flags;
        public string Path;
        public string FileName;
        public uint FileNameHash;
        public uint PathHash;
        public long SubFileInformationOffset;
    }


    public class ARC
    {
        public static string FilePath = "data.arc";

        private static int ArcVersion = 1;

        public static _sArcHeader Header;
        public static _sDirectoryOffsets[] DirectoryOffsets_1;
        public static _sDirectoryOffsets[] DirectoryOffsets_2;
        public static _sDirectoryList[] DirectoryLists;
        public static _sFileInformation[] FileInformation;
        public static _sFolderHashIndex[] HashFolderCounts;
        public static _SubFileInfo[] SubFileInformation_1;
        public static _SubFileInfo[] SubFileInformation_2;

        // Stream Stuff
        public static _sStreamHashToName[] StreamHashToName;
        public static _sStreamNameToHash[] StreamNameToHash;
        public static _sStreamIndexToFile[] StreamIndexToFile;
        public static _sStreamOffset[] StreamOffsets;

        // Arc v2 exclusive
        public static _sFileInformationV2[] FileInformationV2;
        public static _sFileInformationPath[] FileInformationPath;
        public static _sFileInformationIndex[] FileInformationIndex;
        public static _sFileInformationSubIndex[] FileInformationSubIndex;

        // for speed
        public static Dictionary<uint, _sDirectoryList> FolderHashDict;
        public static Dictionary<int, _sDirectoryOffsets> ChunkHash1 = new Dictionary<int, _sDirectoryOffsets>();
        public static Dictionary<int, _sDirectoryOffsets> ChunkHash2 = new Dictionary<int, _sDirectoryOffsets>();

        private static long SubFileInformationStart;
        private static long SubFileInformationStart2;

        public static void Open()
        {
            if (!File.Exists(FilePath))
            {
                using (OpenFileDialog d = new OpenFileDialog())
                {
                    d.Title = "Select your data.arc file";
                    d.FileName = "data.arc";
                    d.Filter = "Smash Ultimate ARC (*.arc)|*.arc";

                    if(d.ShowDialog() == DialogResult.OK)
                    {
                        FilePath = d.FileName;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            //MemoryStream DecompRegionalTables = new MemoryStream();
            //byte[] DecompRegionalTableData;
            MemoryStream DecompTables = new MemoryStream();
            byte[] DecompTableData;
            using (BinaryReader R = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                Header = ByteToType<_sArcHeader>(R);


                // decompress unknown table
                /*R.BaseStream.Position = Header.MusicSectionOffset;

                using (BinaryWriter writer = new BinaryWriter(new FileStream("UnknownTable.bin", FileMode.Create)))
                {
                    writer.Write(R.ReadBytes((int)(Header.NodeSectionOffset - Header.MusicSectionOffset)));
                }*/

                // Reading First Section
                Debug.WriteLine(R.BaseStream.Position.ToString("X"));
                //compressed table
                //FileStream DecompTables = new FileStream("Tables", FileMode.Create);
                R.BaseStream.Seek(Header.NodeSectionOffset, SeekOrigin.Begin);
                using (BinaryWriter writer = new BinaryWriter(DecompTables))
                {
                    if (R.ReadUInt32() == 0x10)
                    {
                        // grab tables
                        R.ReadInt32();
                        int CompSize = R.ReadInt32();
                        long NextTable = R.ReadInt32() + R.BaseStream.Position;
                        writer.Write(Decompress(R.ReadBytes(CompSize)));
                        R.BaseStream.Position = NextTable;
                    }
                }
                DecompTableData = DecompTables.ToArray();
                if(DecompTableData.Length == 0)
                {
                    R.BaseStream.Seek(Header.NodeSectionOffset, SeekOrigin.Begin);
                    DecompTableData = (R.ReadBytes((int)(R.BaseStream.Length - R.BaseStream.Position)));
                }

                /*R.BaseStream.Seek(Header.UnkSectionOffset, SeekOrigin.Begin);
                using (BinaryWriter writer = new BinaryWriter(DecompRegionalTables))
                {
                    if (R.ReadUInt32() == 0x10)
                    {
                        // grab tables
                        R.ReadInt32();
                        int CompSize = R.ReadInt32();
                        long NextTable = R.ReadInt32() + R.BaseStream.Position;
                        writer.Write(Decompress(R.ReadBytes(CompSize)));
                        R.BaseStream.Position = NextTable;
                    }
                }
                DecompRegionalTableData = DecompRegionalTables.ToArray();
                if (DecompRegionalTableData.Length == 0)
                {
                    R.BaseStream.Seek(Header.UnkSectionOffset, SeekOrigin.Begin);
                    DecompRegionalTableData = (R.ReadBytes((int)(R.BaseStream.Length - R.BaseStream.Position)));
                }*/



                /*StreamWriter w = new StreamWriter(new FileStream("TEX.txt", FileMode.Create));
                foreach(_sFileInformation fi in FileInformation)
                {
                    //Console.WriteLine(GetName(fi.Hash2) + " " + GetName(fi.Unk2));
                    if (GetName(fi.Unk2).Equals("nutexb"))
                    {
                        // gottem
                        if (GetName(fi.Hash2).Contains("0x"))
                        {
                            FileOffsetGroup group = GetOffsetFromSubFile(fi);
                            byte[] data = GetFileData(R, group.ArcOffset, group.CompSize, group.DecompSize);
                            string s = "";
                            if (data.Length < 0x6C) continue;
                            for(int i = data.Length - 0x6C; i < data.Length; i++)
                            {
                                if (data[i] == 0) break;
                                s += (char)data[i];
                            }
                            w.WriteLine(s);
                            //Console.WriteLine(s + " " + GetName(fi.Unk2));
                        }
                    }
                }
                w.Close();;*/

                /*HashCheck();
                Console.WriteLine("Done");
                Console.WriteLine("Press enter to close");
                Console.ReadLine();
                return;*/
            }


            if (Header.FileSectionOffset >= 0x8824AF68)
            {
                ArcVersion = 2;
                ReadV2Arc(DecompTableData);
            }
            else
                ReadV1Arc(DecompTableData);
        }


        private static void ReadV1Arc(byte[] TableData)
        {
            using (BinaryReader R = new BinaryReader(new MemoryStream(TableData)))
            {
                R.BaseStream.Position = 0;
                _sNodeHeader NodeHeader = ByteToType<_sNodeHeader>(R);
                //PrintStruct<_sNodeHeader>(NodeHeader);

                R.BaseStream.Seek(0x68, SeekOrigin.Begin);

                //Hash Table
                //BGMHashToName = ReadArray<_sBGMHashToName>(R, NodeHeader.Part1Count);
                R.BaseStream.Seek(0x8 * NodeHeader.Part1Count, SeekOrigin.Current);

                // Hash Table 2
                //BGMNameToHash = ReadArray<_sBGMNameToHash>(R, NodeHeader.Part1Count);
                R.BaseStream.Seek(0xC * NodeHeader.Part1Count, SeekOrigin.Current);

                // Hash Table 3
                //BGMIndexToFile = ReadArray<_sBGMIndexToFile>(R, NodeHeader.Part1Count);
                R.BaseStream.Seek(0x4 * NodeHeader.Part2Count, SeekOrigin.Current);

                //Console.WriteLine(R.BaseStream.Position.ToString("X") + " " + NodeHeader.MusicFileCount.ToString("X"));
                StreamOffsets = ReadArray<_sStreamOffset>(R, NodeHeader.MusicFileCount);

                // Another Hash Table
                Debug.WriteLine("RegionalHashList " + R.BaseStream.Position.ToString("X"));
                R.BaseStream.Seek(0xC * 0xE, SeekOrigin.Current);
                /*for(int i = 0; i < 0xE; i++)
                {
                    //string Name = "";
                    //HashDict.TryGetValue(, out Name);
                    Console.WriteLine(CRC32.Crc32C("+us_en").ToString("X") + " " + R.ReadUInt32().ToString("X") + " " + R.ReadUInt64().ToString("X"));
                }*/

                //folders

                Debug.WriteLine("FodlerHashes " + R.BaseStream.Position.ToString("X"));
                DirectoryLists = ReadArray<_sDirectoryList>(R, NodeHeader.FolderCount);

                //file offsets

                Debug.WriteLine("fileoffsets " + R.BaseStream.Position.ToString("X"));
                DirectoryOffsets_1 = ReadArray<_sDirectoryOffsets>(R, NodeHeader.FileCount1);
                DirectoryOffsets_2 = ReadArray<_sDirectoryOffsets>(R, NodeHeader.FileCount2);

                Debug.WriteLine("subfileoffset " + R.BaseStream.Position.ToString("X"));
                HashFolderCounts = ReadArray<_sFolderHashIndex>(R, NodeHeader.HashFolderCount);

                Debug.WriteLine("subfileoffset " + R.BaseStream.Position.ToString("X"));
                FileInformation = ReadArray<_sFileInformation>(R, NodeHeader.FileInformationCount);

                Debug.WriteLine("subfileoffset " + R.BaseStream.Position.ToString("X"));
                SubFileInformationStart = R.BaseStream.Position;
                SubFileInformation_1 = ReadArray<_SubFileInfo>(R, NodeHeader.SubFileCount);
                SubFileInformationStart2 = R.BaseStream.Position;
                SubFileInformation_2 = ReadArray<_SubFileInfo>(R, NodeHeader.SubFileCount2);

                _sHashInt[] HashInts = ReadArray<_sHashInt>(R, NodeHeader.FolderCount);

                // okay some more file information
                uint FileHashCount = R.ReadUInt32();
                uint UnknownTableCount = R.ReadUInt32();

                _sExtraFITable[] Extra1 = ReadArray<_sExtraFITable>(R, UnknownTableCount);
                _sExtraFITable2[] Extra2 = ReadArray<_sExtraFITable2>(R, FileHashCount);

                R.BaseStream.Position += 8 * NodeHeader.FileInformationCount;


                // this is not actually regional stuff, just more lookups for paths....
                /*using (BinaryReader Reg = new BinaryReader(new MemoryStream(DecompRegionalTableData)))
                {
                    Reg.BaseStream.Seek(0, SeekOrigin.Begin);
                    // these are folders
                    _sRegionalHeader RegionalHeader = ByteToType<_sRegionalHeader>(Reg);
                    _sRegionalHash[] RegionalHashes = ReadArray<_sRegionalHash>(Reg, RegionalHeader.Count1);
                    _sRegionalGroup[] RegionalGroups = ReadArray<_sRegionalGroup>(Reg, RegionalHeader.Count1);
                    Debug.WriteLine(Reg.BaseStream.Position.ToString("X"));
                    using (StreamWriter w = new StreamWriter(new FileStream("extra.txt", FileMode.Create)))
                    {
                        int max = 0;
                        foreach (var n in FileInformation)
                        {
                            if ((n.FileTableFlag >> 8) == 0) continue;
                                string Name = "";
                            HashDict.TryGetValue(n.Hash2, out Name);
                            w.WriteLine(n.DirectoryIndex.ToString("X") + " " + (n.FileTableFlag).ToString("X") + " " + Name);
                            var FileOffset = SubFileInformation_1[(n.FileTableFlag >> 8) + 2];
                            w.WriteLine("\t" + FileOffset.Offset.ToString("X") + " " + FileOffset.DecompSize.ToString("X") + " " + FileOffset.Flags.ToString("X"));
                        }
                        foreach (var n in DirectoryOffsets_1)
                        {
                            w.WriteLine((Header.FileSectionOffset + n.Offset).ToString("X"));
                        }
                        w.WriteLine(max.ToString("X"));
                    }
                }*/






                //Debug.WriteLine((Header.FileSectionOffset + DirectoryOffsets_1[510].Offset).ToString("X"));

                /*Debug.WriteLine(HashInts[HashInts.Length - 1].Index.ToString("X") + " " + HashInts[HashInts.Length - 1].Hash.ToString("X"));
                Debug.WriteLine("FolderHashes 0x" + DirectoryLists.Length.ToString("X"));
                Debug.WriteLine("Music Table 0x" + BGMOffsets.Length.ToString("X"));
                Debug.WriteLine("Chunks 1 0x" + DirectoryOffsets_1.Length.ToString("X"));
                Debug.WriteLine("Chunks 2 0x" + DirectoryOffsets_2.Length.ToString("X"));
                Debug.WriteLine("HashFolderCounts 0x" + HashFolderCounts.Length.ToString("X"));
                Debug.WriteLine("FileInformation 0x" + FileInformation.Length.ToString("X"));
                Debug.WriteLine("SubFile1 0x" + SubFileInformation_1.Length.ToString("X"));
                Debug.WriteLine("SubFile2 0x" + SubFileInformation_2.Length.ToString("X"));*/

                FolderHashDict = new Dictionary<uint, _sDirectoryList>();
                foreach (_sDirectoryList fh in DirectoryLists)
                {
                    FolderHashDict.Add(fh.HashID, fh);
                }

                foreach (_sDirectoryOffsets chunk in DirectoryOffsets_1)
                {
                    for (int i = 0; i < chunk.SubDataCount; i++)
                        if (!ChunkHash1.ContainsKey((int)chunk.SubDataStartIndex + i))
                            ChunkHash1.Add((int)chunk.SubDataStartIndex + i, chunk);
                }
                foreach (_sDirectoryOffsets chunk in DirectoryOffsets_2)
                {
                    for (int i = 0; i < chunk.SubDataCount; i++)
                        if (!ChunkHash2.ContainsKey((int)chunk.SubDataStartIndex + i))
                            ChunkHash2.Add((int)chunk.SubDataStartIndex + i, chunk);
                }


                /*using (StreamWriter o = new StreamWriter(new FileStream("Output.txt", FileMode.Create)))
                {
                    foreach (var item in FileInformation)
                    {
                        /*var Directory = DirectoryLists[item.DirectoryIndex >> 8];
                        var DirectoryOffset = DirectoryOffsets_1[(Directory.DirOffsetIndex>>8)];
                        _sDirectoryOffsets DirectoryOffset2 = DirectoryOffset;
                        var FileOffset = SubFileInformation_1[item.SubFile_Index];
                        if((DirectoryOffset.ResourceIndex & 0xFFFFFF) != 0xFFFFFF)
                        {
                            DirectoryOffset2 = DirectoryOffsets_2[(DirectoryOffset.ResourceIndex>>8) & 0xFFFFFF];
                        }
                        var Directory = DirectoryLists[item.DirectoryIndex >> 8];
                        var OffsetGroup = GetFileInformation(item);
                        o.WriteLine(GetName(item.Parent) + "/" + GetName(item.Hash2) +
                            " Offset: 0x" + OffsetGroup.ArcOffset.ToString("X") + 
                            " CompSize: 0x" + OffsetGroup.CompSize.ToString("X") + 
                            " DecompSize: 0x" + OffsetGroup.DecompSize.ToString("X") +
                            " Flag: 0x" + OffsetGroup.Flags.ToString("X"));
                        /*o.WriteLine(GetName(item.Hash2) + " " + GetName(Directory.NameHash) +
                            " Directory Offset: 0x" + (Header.FileSectionOffset + DirectoryOffset.Offset).ToString("X") +
                            (DirectoryOffset.Offset == DirectoryOffset2.Offset ? "" : " 0x" +
                            (Header.FileSectionOffset + DirectoryOffset2.Offset).ToString("X")) +
                            " Offset: 0x" + (FileOffset.Offset << 2).ToString("X") +
                            " Compsize: 0x" + FileOffset.CompSize.ToString("X") +
                            " Flags: 0x" + FileOffset.Flags.ToString("X") +
                            " " + item.SubFile_Index + " " + (FileOffset.Flags & 0xFFFFFF) + 
                            " " + DirectoryOffset.SubDataStartIndex + " - " + (DirectoryOffset.SubDataStartIndex + DirectoryOffset.SubDataCount));
                            
                    }
        }*/
            }
        }


        private static void ReadV2Arc(byte[] TableData)
        {
            using (BinaryReader R = new BinaryReader(new MemoryStream(TableData)))
            {
                R.BaseStream.Position = 0;
                _sNodeHeaderv2 NodeHeader = ByteToType<_sNodeHeaderv2>(R);
                //PrintStruct<_sNodeHeader>(NodeHeader);

                R.BaseStream.Seek(0x3C, SeekOrigin.Begin);

                // Unknown?? maybe something with languages
                R.BaseStream.Seek(0xE * 12, SeekOrigin.Current);

                // Another structure
                _sNodeHeaderv2_2 NodeHeader2 = ByteToType<_sNodeHeaderv2_2>(R);

                // Hash Table
                Console.WriteLine("Hash table 1 " + R.BaseStream.Position.ToString("X"));
                R.BaseStream.Seek(0x8 * NodeHeader2.Part1Count, SeekOrigin.Current);

                // Hash Table 2
                Console.WriteLine("StreamToHash table 2 " + R.BaseStream.Position.ToString("X"));
                StreamNameToHash = ReadArray<_sStreamNameToHash>(R, NodeHeader2.Part1Count);

                // Hash Table 3
                Console.WriteLine("Hash table 3 " + R.BaseStream.Position.ToString("X"));
                StreamIndexToFile = ReadArray<_sStreamIndexToFile>(R, NodeHeader2.Part2Count);

                // stream offsets
                Console.WriteLine("Hash table 4 " + R.BaseStream.Position.ToString("X"));
                StreamOffsets = ReadArray<_sStreamOffset>(R, NodeHeader2.Part3Size);

                Console.WriteLine("Hash table 5 " + R.BaseStream.Position.ToString("X"));
                int UnkCount1 = R.ReadInt32();
                int UnkCount2 = R.ReadInt32();
                R.BaseStream.Seek(0x8 * UnkCount2, SeekOrigin.Current);
                R.BaseStream.Seek(0x8 * UnkCount1, SeekOrigin.Current);

                Debug.WriteLine("FilePathInfo " + R.BaseStream.Position.ToString("X"));
                FileInformationPath = ReadArray<_sFileInformationPath>(R, NodeHeader.UnkCount);

                // Start of first node header section
                Debug.WriteLine("SomeIndicesForFileInformation " + R.BaseStream.Position.ToString("X"));
                FileInformationIndex = ReadArray<_sFileInformationIndex>(R, NodeHeader.UnkOffsetSizeCount);

                // Start of first node header section
                Debug.WriteLine("FolderHashes " + R.BaseStream.Position.ToString("X"));
                R.BaseStream.Seek(0x8 * NodeHeader.FolderCount, SeekOrigin.Current);

                //folders

                Debug.WriteLine("FolderHashes " + R.BaseStream.Position.ToString("X"));
                DirectoryLists = ReadArray<_sDirectoryList>(R, NodeHeader.FolderCount);

                //file offsets

                Debug.WriteLine("fileoffsets " + R.BaseStream.Position.ToString("X"));
                DirectoryOffsets_1 = ReadArray<_sDirectoryOffsets>(R, NodeHeader.FileCount1);
                DirectoryOffsets_2 = ReadArray<_sDirectoryOffsets>(R, NodeHeader.FileCount2);

                Debug.WriteLine("hashfolderoffset " + R.BaseStream.Position.ToString("X"));
                HashFolderCounts = ReadArray<_sFolderHashIndex>(R, NodeHeader.HashFolderCount);
                
                Debug.WriteLine("fileinformationoffset " + R.BaseStream.Position.ToString("X"));
                FileInformationV2 = ReadArray<_sFileInformationV2>(R, NodeHeader.FileInformationCount + NodeHeader.SubFileCount2);

                Debug.WriteLine("sub index table " + R.BaseStream.Position.ToString("X"));
                FileInformationSubIndex = ReadArray<_sFileInformationSubIndex>(R, (NodeHeader.LastTableCount + NodeHeader.SubFileCount2));

                Debug.WriteLine("subfileoffset " + R.BaseStream.Position.ToString("X"));
                SubFileInformationStart = R.BaseStream.Position;
                SubFileInformation_1 = ReadArray<_SubFileInfo>(R, NodeHeader.SubFileCount);
                SubFileInformationStart2 = R.BaseStream.Position;
                SubFileInformation_2 = ReadArray<_SubFileInfo>(R, NodeHeader.SubFileCount2);

                int max = 0;
                foreach(var dir in FileInformationSubIndex)
                {
                    max = Math.Max(max, (int)dir.SomeIndex1);
                }
                Console.WriteLine("Directory index " + max.ToString("X") + " " + (DirectoryLists.Length).ToString("X") + " " + (DirectoryOffsets_1.Length + DirectoryOffsets_2.Length).ToString("X"));
                

                FolderHashDict = new Dictionary<uint, _sDirectoryList>();
                foreach (_sDirectoryList fh in DirectoryLists)
                {
                    FolderHashDict.Add(fh.HashID, fh);
                }

                foreach (_sDirectoryOffsets chunk in DirectoryOffsets_1)
                {
                    for (int i = 0; i < chunk.SubDataCount; i++)
                        if (!ChunkHash1.ContainsKey((int)chunk.SubDataStartIndex + i))
                            ChunkHash1.Add((int)chunk.SubDataStartIndex + i, chunk);
                }
                foreach (_sDirectoryOffsets chunk in DirectoryOffsets_2)
                {
                    for (int i = 0; i < chunk.SubDataCount; i++)
                        if (!ChunkHash2.ContainsKey((int)chunk.SubDataStartIndex + i))
                            ChunkHash2.Add((int)chunk.SubDataStartIndex + i, chunk);
                }

                
            }
        }


        public static List<FileOffsetGroup> GetStreamFiles()
        {
            if (ArcVersion == 2)
                return GetStreamFilesV2();
            else
                return new List<FileOffsetGroup>();
        }

        private static List<FileOffsetGroup> GetStreamFilesV2()
        {
            var streamfiles = new List<FileOffsetGroup>(StreamNameToHash.Length);

            foreach(var streamfile in StreamNameToHash)
            {
                var streamindex = StreamIndexToFile[streamfile.NameIndex >> 8].FileIndex;
                var offset = StreamOffsets[streamindex];

                FileOffsetGroup group = new FileOffsetGroup();
                group.FileName = GetName(streamfile.Hash);
                group.ArcOffset = new long[] { offset.Offset };
                group.CompSize = new long[] { offset.Size };
                group.DecompSize = new long[] { offset.Size };
                streamfiles.Add(group);
            }
            return streamfiles;
        }


        public static List<FileOffsetGroup> GetFiles()
        {
            Console.WriteLine("Version " + ArcVersion + " " + (ArcVersion == 2));
            if (ArcVersion == 2)
                return GetFilesV2();
            else
                return GetFilesV1();
        }


        public static List<FileOffsetGroup> GetFilesV1()
        {
            List<FileOffsetGroup> Files = new List<FileOffsetGroup>(FileInformation.Length);
            int index = 0;
            foreach (var item in FileInformation)
            {
                var Directory = DirectoryLists[item.DirectoryIndex >> 8];
                var OffsetGroup = GetFileInformation(item);
                if ((item.FileTableFlag >> 8) > 0)
                {
                    for (int i = 1; i < 0xE; i++)
                    {
                        var OffsetGroup2 = GetFileInformation(item, i);
                        Array.Resize(ref OffsetGroup.ArcOffset, OffsetGroup.ArcOffset.Length + 1);
                        OffsetGroup.ArcOffset[OffsetGroup.ArcOffset.Length - 1] = OffsetGroup2.ArcOffset[0];
                        Array.Resize(ref OffsetGroup.Offset, OffsetGroup.Offset.Length + 1);
                        OffsetGroup.Offset[OffsetGroup.Offset.Length - 1] = OffsetGroup2.Offset[0];
                        Array.Resize(ref OffsetGroup.CompSize, OffsetGroup.CompSize.Length + 1);
                        OffsetGroup.CompSize[OffsetGroup.CompSize.Length - 1] = OffsetGroup2.CompSize[0];
                        Array.Resize(ref OffsetGroup.DecompSize, OffsetGroup.DecompSize.Length + 1);
                        OffsetGroup.DecompSize[OffsetGroup.DecompSize.Length - 1] = OffsetGroup2.DecompSize[0];
                        Array.Resize(ref OffsetGroup.Flags, OffsetGroup.Flags.Length + 1);
                        OffsetGroup.Flags[OffsetGroup.Flags.Length - 1] = OffsetGroup2.Flags[0];
                    }
                }

                OffsetGroup.FileNameHash = item.Hash2;
                OffsetGroup.PathHash = item.Parent;
                string Extension = "";
                HashDict.TryGetValue(item.Extension, out Extension);
                if (Extension != "") Extension = "." + Extension;
                OffsetGroup.FileName = GetName(item.Hash2, Extension);
                OffsetGroup.Path = GetName(item.Parent);
                Files.Add(OffsetGroup);
                index++;
            }
            return Files;
        }

        public static List<FileOffsetGroup> GetFilesV2()
        {
            List<FileOffsetGroup> Files = new List<FileOffsetGroup>(FileInformationV2.Length);
            int index = 0;
            Console.WriteLine("Files " + FileInformationV2.Length);
            foreach (var item in FileInformationV2)
            {
                var PathInfo = FileInformationPath[item.HashIndex];
                //var Directory = DirectoryLists[PathInfo.DirectoryIndex >> 8];
                var OffsetGroup = GetFileInformation(item);
                if ((item.Flags & 0xF000) == 0x8000)
                {
                    for (int i = 1; i < 0xE; i++)
                    {
                        var OffsetGroup2 = GetFileInformation(item, i);
                        Array.Resize(ref OffsetGroup.ArcOffset, OffsetGroup.ArcOffset.Length + 1);
                        OffsetGroup.ArcOffset[OffsetGroup.ArcOffset.Length - 1] = OffsetGroup2.ArcOffset[0];
                        Array.Resize(ref OffsetGroup.Offset, OffsetGroup.Offset.Length + 1);
                        OffsetGroup.Offset[OffsetGroup.Offset.Length - 1] = OffsetGroup2.Offset[0];
                        Array.Resize(ref OffsetGroup.CompSize, OffsetGroup.CompSize.Length + 1);
                        OffsetGroup.CompSize[OffsetGroup.CompSize.Length - 1] = OffsetGroup2.CompSize[0];
                        Array.Resize(ref OffsetGroup.DecompSize, OffsetGroup.DecompSize.Length + 1);
                        OffsetGroup.DecompSize[OffsetGroup.DecompSize.Length - 1] = OffsetGroup2.DecompSize[0];
                        Array.Resize(ref OffsetGroup.Flags, OffsetGroup.Flags.Length + 1);
                        OffsetGroup.Flags[OffsetGroup.Flags.Length - 1] = OffsetGroup2.Flags[0];
                    }
                }

                OffsetGroup.FileNameHash = PathInfo.Hash2;
                OffsetGroup.PathHash = PathInfo.Parent;
                string Extension = "";
                HashDict.TryGetValue(PathInfo.Extension, out Extension);
                if (Extension != "") Extension = "." + Extension;
                OffsetGroup.FileName = GetName(PathInfo.Hash2, Extension);
                /*if ( OffsetGroup.FileName.Equals("vc_bayonetta.nus3audio"))////
                {
                    int sindex = 0;
                    foreach(var sb in SubFileInformation_1)
                    {
                        if(sb.DecompSize == 0x107810)
                        {
                            Console.WriteLine(OffsetGroup.FileName + " " + sindex.ToString("X") + " " + sb.CompSize.ToString("X"));
                        }
                        sindex++;
                    }
                    sindex = 0;
                    foreach (var sb in FileInformationSubIndex)
                    {
                        if(sb.SomeIndex2 == 0x31DF)
                        {
                            Console.WriteLine(OffsetGroup.FileName + " " + sindex.ToString("X") + " " + sb.SomeIndex3.ToString("X"));

                        }
                        sindex++;
                    }
                    var subinfo = FileInformationSubIndex[item.SubFile_Index + 1];
                    var regionalsubfile = SubFileInformation_1[subinfo.SomeIndex2];
                    Console.WriteLine(OffsetGroup.FileName + " " + regionalsubfile.DecompSize.ToString("X") + " " + item.SubFile_Index.ToString("X") + " " + PathInfo.FileTableFlag.ToString("X") + " " + subinfo.SomeIndex1.ToString("X") + " " + subinfo.SomeIndex2.ToString("X") + " " + subinfo.SomeIndex3.ToString("X") + " " + item.Flags.ToString("X"));
                }
                if(item.Flags == 0x8010)
                {
                    //Console.WriteLine(OffsetGroup.FileName);
                    var subinfo = FileInformationSubIndex[item.SubFile_Index];
                    var regionalsubfile = SubFileInformation_1[item.HashIndex2];
                    //Console.WriteLine(OffsetGroup.FileName + " " + item.HashIndex2.ToString("X") + " " + regionalsubfile.DecompSize.ToString("X"));

                }*/
                OffsetGroup.Path = GetName(PathInfo.Parent);
                Files.Add(OffsetGroup);
                index++;
            }
            return Files;
        }

        private static FileOffsetGroup GetFileInformation(_sFileInformation FileInfo, int RegionalIndex = 0)
        {
            return GetFileInformation(FileInfo.SubFile_Index, (FileInfo.FileTableFlag >> 8), DirectoryLists[FileInfo.DirectoryIndex >> 8].DirOffsetIndex >> 8, RegionalIndex);
        }

        private static FileOffsetGroup GetFileInformation(_sFileInformationV2 FileInfo, int RegionalIndex = 0)
        {
            bool Regional = FileInfo.Flags == 0x8010;
            var path = FileInformationPath[FileInfo.HashIndex];
            var dirinfo = FileInformationIndex[FileInfo.HashIndex2 + (Regional ? RegionalIndex + 1 : 0)];
            var subinfo = FileInformationSubIndex[FileInfo.SubFile_Index + (Regional ? RegionalIndex + 1 : 0)];
            return GetFileInformation(subinfo.SomeIndex2, 0, subinfo.SomeIndex1, RegionalIndex);
        }

        private static FileOffsetGroup GetFileInformation(uint SubFile_Index, uint RegionalOffset, uint DirectoryIndex, int RegionalIndex = 0)
        {
            FileOffsetGroup g = new FileOffsetGroup();

            // Get File Data

            _SubFileInfo FileOffset;
            if(SubFile_Index < SubFileInformation_1.Length)
                FileOffset = SubFileInformation_1[SubFile_Index];
            else
                FileOffset = SubFileInformation_2[SubFile_Index - SubFileInformation_1.Length];

            _sDirectoryOffsets DirectoryOffset;
            if(DirectoryIndex < DirectoryOffsets_1.Length)
                DirectoryOffset = DirectoryOffsets_1[DirectoryIndex];
            else
                DirectoryOffset = DirectoryOffsets_2[DirectoryIndex - DirectoryOffsets_1.Length];
            var DirectoryOffset2 = DirectoryOffset;

            if (RegionalOffset > 0)
            {
                FileOffset = SubFileInformation_1[RegionalOffset + RegionalIndex];
                DirectoryIndex = (uint)(DirectoryIndex + 1 + RegionalIndex);
                if (DirectoryIndex < DirectoryOffsets_1.Length)
                    DirectoryOffset = DirectoryOffsets_1[DirectoryIndex];
                else
                    DirectoryOffset = DirectoryOffsets_2[DirectoryIndex - DirectoryOffsets_1.Length];
                DirectoryOffset2 = DirectoryOffset;
            }
            else
            if ((DirectoryOffset.ResourceIndex & 0xFFFFFF) != 0xFFFFFF)
            {
                if(DirectoryOffset.ResourceIndex < DirectoryOffsets_1.Length)
                    DirectoryOffset2 = DirectoryOffsets_1[((DirectoryOffset.ResourceIndex) & 0xFFFFFF)];
                else
                    DirectoryOffset2 = DirectoryOffsets_2[((DirectoryOffset.ResourceIndex - DirectoryOffsets_1.Length) & 0xFFFFFF)];
                
            }

            // Parse Flags
            int flag = ((int)FileOffset.Flags >> 24);
            bool Compressed = (flag & 0x03) == 0x03;
            bool External = (flag & 0x08) == 0x08;
            int ExternalOffset = (int)FileOffset.Flags & 0xFFFFFF;

            if((ArcVersion == 2))
            {
                // why did they do this
                flag = ((int)FileOffset.Flags & 0xFF);
                Compressed = (flag & 0x03) == 0x03;
                External = (flag & 0x08) == 0x08;
                ExternalOffset = (int)FileOffset.Flags >> 8;
            }

            if (ExternalOffset > 0 && !External)
            {
                if (ArcVersion == 2)
                {
                    return GetFileInformation(FileInformationV2[ExternalOffset]);
                }
                else
                {
                    return GetFileInformation(FileInformation[ExternalOffset]);
                }
            }

            if (External)
            {
                //hack
                DirectoryOffset2 = ChunkHash2[ExternalOffset];
                FileOffset = SubFileInformation_2[ExternalOffset];

                g.ArcOffset = new long[] { (Header.FileSectionOffset + DirectoryOffset2.Offset + (FileOffset.Offset << 2)) };
                g.Offset = new long[] { FileOffset.Offset };
                g.CompSize = new long[] { FileOffset.CompSize };
                g.DecompSize = new long[] { FileOffset.DecompSize };
                g.Flags = new uint[] { FileOffset.Flags };
                g.SubFileInformationOffset = SubFileInformationStart2 + ExternalOffset * 16;
            }
            else
            {
                g.ArcOffset = new long[] { (Header.FileSectionOffset + DirectoryOffset.Offset + (FileOffset.Offset << 2)) };
                g.Offset = new long[] { FileOffset.Offset };
                g.CompSize = new long[] { FileOffset.CompSize };
                g.DecompSize = new long[] { FileOffset.DecompSize };
                g.Flags = new uint[] { FileOffset.Flags };
                g.SubFileInformationOffset = SubFileInformationStart + SubFile_Index * 16;
            }

            return g;
        }

        public static string[] GetPaths()
        {
            List<string> paths = new List<string>();

            foreach(_sFolderHashIndex fhi in HashFolderCounts)
            {
                paths.Add(GetPathString(FolderHashDict, FolderHashDict[fhi.Hash]));
            }

            return paths.ToArray();
        }

        public static void CommandFunctions(string FolderToExtract)
        {
            using (BinaryReader R = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                if (!FolderToExtract.Equals(""))
                {
                    _sDirectoryList ToExtract;
                    if (FolderHashDict.TryGetValue(CRC32.Crc32C(FolderToExtract), out ToExtract))
                    {
                        Console.WriteLine("Extracting " + GetPathString(FolderHashDict, ToExtract));
                        ExtractFolder(R, ToExtract);
                    }
                    else
                    {
                        foreach (_sDirectoryList h in DirectoryLists)
                        {
                            if (GetPathString(FolderHashDict, h).Equals(FolderToExtract))
                            {
                                ExtractFolder(R, h);
                                break;
                            }
                        }
                        Debug.WriteLine("Could no extract " + FolderToExtract);
                    }
                }
                else
                {
                    foreach (_sDirectoryList h in DirectoryLists)
                    {
                        Console.WriteLine("Extracting " + GetPathString(FolderHashDict, h));
                        ExtractFolder(R, h);
                    }
                }
            }

        }


        public static void HashCheck()
        {
            foreach (_sDirectoryList fh in DirectoryLists)
            {
                GetPathString(FolderHashDict, fh);
            }
            foreach (_sFileInformation fi in FileInformation)
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

        public static _sFileInformation[] GetFileInformationFromFolder(_sDirectoryList FolderHash)
        {
            _sFileInformation[] info = new _sFileInformation[(int)FolderHash.FileInformationCount];
            for (int i = 0; i < FolderHash.FileInformationCount; i++)
            {
                info[i] = (FileInformation[FolderHash.FileInformationStartIndex + i]);
            }
            return info;
        }

        public static void ExtractFolder(BinaryReader Stream, _sDirectoryList FolderHash)
        {
            for (int i = 0; i < FolderHash.FileInformationCount; i++)
            {
                _sFileInformation info = FileInformation[FolderHash.FileInformationStartIndex + i];

                string FinalPath = GetPathString(FolderHashDict, FolderHash) + "/" + info.Parent.ToString("X");
                //Directory.CreateDirectory(FinalPath);

                _SubFileInfo sub = SubFileInformation_1[info.SubFile_Index];
                //Debug.WriteLine("\tFolder - " + info.Unk4.ToString("X"));
                //Debug.WriteLine("\t\t" + GetName(info.Hash2) + " " + sub.Flags.ToString("X"));

                GetName(info.Hash2);
                FileOffsetGroup group = GetOffsetFromSubFile(info);
                //Debug.WriteLine("\t\t\tArcOffset: 0x" + group.ArcOffset.ToString("X") + " Offset: " + group.Offset.ToString("X") + " CompSize: 0x" + group.CompSize.ToString("X") + " DecompSize:" + group.DecompSize.ToString("X") + " Flags: 0x" + group.Flags.ToString("X"));

                ExportFile(FinalPath + "/" + GetName(info.Hash2), Stream, group.ArcOffset[0], group.CompSize[0], group.DecompSize[0]);
            }
        }

        public static void ExportFile(string filepath, BinaryReader R, long Offset, long CompSize, long DecompSize)
        {
            File.WriteAllBytes(filepath, GetFileData(R, Offset, CompSize, DecompSize));
        }

        private static byte[] GetFileData(BinaryReader R, long Offset, long CompSize, long DecompSize)
        {
            R.BaseStream.Seek(Offset, SeekOrigin.Begin);

            byte[] data = R.ReadBytes((int)CompSize);

            if (CompSize != DecompSize && DecompSize > 0 && CompSize > 0)
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

            if (DecompSize != data.Length)
                Console.WriteLine("Error: Filesize mismatch ");

            return data;
        }


        public static byte[] GetFileData(long Offset, long CompSize, long DecompSize)
        {
            using (BinaryReader R = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                return GetFileData(R, Offset, CompSize, DecompSize);
            }
        }

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

        public static FileOffsetGroup GetOffsetFromSubFile(_sFileInformation FileInfo)
        {
            FileOffsetGroup g = new FileOffsetGroup();
            int SubIndex = (int)FileInfo.SubFile_Index;
            _SubFileInfo subfile = SubFileInformation_1[SubIndex];

            int flag = ((int)subfile.Flags >> 24);
            bool UseTable2 = (flag & 0xFF) == 0x08 || (flag & 0xFF) == 0x0B;
            int ExternalOffset = (int)subfile.Flags & 0xFFFFFF;

            if (flag == 0x03 && ExternalOffset > 0)
            {
                //Debug.WriteLine("\tEmpty?------------");
                //Debug.WriteLine("\t\t" + subfile.Offset.ToString("X") + " " + subfile.CompSize.ToString("X") + " " + subfile.DecompSize.ToString("X"));
            }

            if (ExternalOffset > 0 && !UseTable2)
            {
                //Debug.WriteLine("\t\t External ->" + GetName(FileInformation[ExternalOffset].Hash2));
                return GetOffsetFromSubFile(FileInformation[ExternalOffset]);
            }
            Dictionary<int, _sDirectoryOffsets> Offsets = ChunkHash1;
            if (UseTable2)
            {
                Offsets = ChunkHash2;
                SubIndex = (int)(subfile.Flags & 0xFFFFFF);
                subfile = SubFileInformation_2[SubIndex];
            }

            //foreach (_sFileChunkOffsets chunk in Offsets)
            _sDirectoryOffsets chunk = Offsets[SubIndex];
            {
                if (SubIndex >= chunk.SubDataStartIndex && SubIndex < chunk.SubDataStartIndex + chunk.SubDataCount)
                {
                    g.ArcOffset = new long[] { (Header.FileSectionOffset + chunk.Offset + (subfile.Offset << 2)) };
                    g.Offset = new long[] { subfile.Offset };// (Header.FileSectionOffset + chunk.Offset + (subfile.Offset << 2));
                    g.CompSize = new long[] { subfile.CompSize };
                    g.DecompSize = new long[] { subfile.DecompSize };
                    g.Flags = new uint[] { subfile.Flags };
                    //break;
                }
            }

            return g;
        }

        public static string GetPathString(Dictionary<uint, _sDirectoryList> HashBank, _sDirectoryList Folder)
        {
            string Folder1 = "";
            string Folder2 = "";
            string Folder3 = "";

            if (Folder.HashID != Folder.NameHash && Folder.NameHash != 0 && HashBank.ContainsKey(Folder.NameHash))
                Folder3 = GetPathString(HashBank, HashBank[Folder.NameHash]) + "/";
            else if (Folder.NameHash != 0)
                Folder3 = GetName(Folder.NameHash);

            if (HashBank.ContainsKey(Folder.Hash4))
                Folder2 = GetPathString(HashBank, HashBank[Folder.Hash4]) + "/";
            else if (Folder.Hash4 != 0)
                Folder3 = GetName(Folder.Hash4);

            /*if (Folder.Hash4 != 0 && Folder.Hash4 != 0 && HashBank.ContainsKey(Folder.Hash4))
                Folder1 = GetPathString(HashBank, HashBank[Folder.Hash4]) + "/";
            else if (Folder.Hash4 != 0)
                Folder3 = GetName(Folder.Hash4);*/

            return Folder1 + Folder2 + Folder3;
        }


        public static Dictionary<uint, string> MissingHashes = new Dictionary<uint, string>();
        public static Dictionary<uint, string> UsedHashes = new Dictionary<uint, string>();
        public static string GetName(uint Hash, string Extension = "")
        {
            string name = "";
            if (HashDict.TryGetValue(Hash, out name))
            {
                //if (!UsedHashes.ContainsKey(Hash))
                //    UsedHashes.Add(Hash, name);
                return name;
            }

            //if (!MissingHashes.ContainsKey(Hash))
            //    MissingHashes.Add(Hash, name);
            return "0x" + Hash.ToString("X") + Extension;
        }



        public static T[] ReadArray<T>(BinaryReader reader, uint Size)
        {
            // slow af but whatevs
            T[] arr = new T[Size];
            for (int i = 0; i < Size; i++)
            {
                arr[i] = ByteToType<T>(reader);
            }
            return arr;
        }

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

        public static void CompareHashes(string PreviousHashFileName, string CurrentHashFileName)
        {
            Dictionary<long, HashCompareCheck> PreviousHashes = new Dictionary<long, HashCompareCheck>();
            List<HashCompareCheck> Hash2 = new List<HashCompareCheck>();

            using (BinaryReader reader = new BinaryReader(new FileStream(PreviousHashFileName, FileMode.Open)))
            {
                reader.ReadUInt32();
                uint FileCount = reader.ReadUInt32();
                FileCount = (uint)(reader.BaseStream.Length - 8) / 12;
                var Hashes = ReadArray<HashCompareCheck>(reader, FileCount);
                foreach(var f in Hashes)
                {
                    long key = ((long)f.Name << 32) | f.Path;
                    if(!PreviousHashes.ContainsKey(key))
                        PreviousHashes.Add(key, f);
                    else
                    {
                        string Path = "";
                        if (!HashDict.TryGetValue(f.Path, out Path))
                            Path = "0x" + f.Path.ToString("X");
                        string Name = "";
                        if (!HashDict.TryGetValue(f.Name, out Name))
                            Name = "0x" + f.Name.ToString("X");
                        Console.WriteLine($"Duplicate: {Path}{Name}");
                        //PreviousHashes[key] = f;
                    }
                }
            }

            using (BinaryReader reader = new BinaryReader(new FileStream(CurrentHashFileName, FileMode.Open)))
            {
                reader.ReadUInt32();
                uint FileCount = reader.ReadUInt32();
                FileCount = (uint)(reader.BaseStream.Length - 8) / 12;
                var Hashes = ReadArray<HashCompareCheck>(reader, FileCount);
                Dictionary<long, object> Keys = new Dictionary<long, object>();
                foreach (var f in Hashes)
                {
                    long key = ((long)f.Name << 32) | f.Path;
                    if (!Keys.ContainsKey(key))
                    {
                        Hash2.Add(f);
                        Keys.Add(key, f);
                    }
                }
            }

            List<string> Changed = new List<string>();
            List<string> New = new List<string>();


            int count = 0;
            int Percent = 1;

            foreach (HashCompareCheck c in Hash2)
            {
                count++;
                if (count == Hash2.Count / 100)
                {
                    Console.WriteLine($"{Percent}% done");
                    Percent++;
                    count = 0;
                }
                string Path = "";
                if (!HashDict.TryGetValue(c.Path, out Path))
                    Path = "0x" + c.Path.ToString("X");
                string Name = "";
                if (!HashDict.TryGetValue(c.Name, out Name))
                    Name = "0x" + c.Name.ToString("X");
                long key = ((long)c.Name << 32) | c.Path;
                if (PreviousHashes.ContainsKey(key))
                {
                    if(PreviousHashes[key].Hash != c.Hash)
                        Changed.Add(Path + Name);
                    PreviousHashes.Remove(key);
                }
                else
                {
                    New.Add(Path + Name);
                }
            }

            using (StreamWriter writer = new StreamWriter(new FileStream("ChangeLog.txt", FileMode.Create)))
            {
                foreach (var f in New)
                {
                    writer.WriteLine($"Added: {f}");
                }
                foreach(var f in Changed)
                {
                    writer.WriteLine($"Changed: {f}");
                }
            }
        }

        public static void CreateHashCompare(string outFileName)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                using (BinaryWriter writer = new BinaryWriter(new FileStream(outFileName, FileMode.Create)))
                {
                    var Files = GetFiles();
                    writer.Write("CRCH".ToCharArray());
                    writer.Write(Files.Count);
                    int PercentSlice = Files.Count / 100;
                    int FileCount = 0;
                    int Percent = 0;
                    foreach (var file in Files)
                    {
                        if(FileCount >= PercentSlice)
                        {
                            FileCount = 0;
                            Console.WriteLine($"{Percent}% Done");
                            Percent++;
                        }
                        if (file.CompSize[0] == 0 && file.DecompSize[0] == 0)
                            continue;
                        FileCount++;
                        writer.Write(file.FileNameHash);
                        writer.Write(file.PathHash);
                        reader.BaseStream.Position = file.ArcOffset[0];
                        byte[] data = reader.ReadBytes((int)file.CompSize[0]);
                        writer.Write(CRC32.Crc32C(data));
                    }
                    writer.BaseStream.Position = 4;
                    writer.Write(FileCount);
                }
            }
            
        }
    }
}
