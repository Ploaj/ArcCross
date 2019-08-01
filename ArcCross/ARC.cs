using ArcCross.StructsV1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Zstandard.Net;

namespace ArcCross
{
    public class ARC
    {
        private string filePath;

        // Optimize repeated calls to GetFileList
        private List<string> filePaths = new List<string>();

        private const ulong Magic = 0xABCDEF9876543210;

        private _sArcHeader header;

        public int Version { get; set; }

        // stream

        private _sStreamUnk[] streamUnk;
        private _sStreamHashToName[] streamHashToName;
        private _sStreamNameToHash[] streamNameToHash;
        private _sStreamIndexToOffset[] streamIndexToFile;
        private _sStreamOffset[] streamOffsets;

        // file system

        private _sFileSystemHeader FSHeader;
        private byte[] regionalbytes;
        private _sStreamHeader StreamHeader;

        private _sFileInformationPath[] fileInfoPath;
        private _sFileInformationIndex[] fileInfoIndex;
        private _sFileInformationSubIndex[] fileInfoSubIndex;
        public _sFileInformationV2[] fileInfoV2;

        private _sSubFileInfo[] subFiles;

        private _sFileInformationUnknownTable[] fileInfoUnknownTable;
        private _sHashIndexGroup[] filePathToIndexHashGroup;

        // Directory information

        private _sHashIndexGroup[] directoryHashGroup;
        private _sDirectoryList[] directoryList;
        private _sDirectoryOffset[] directoryOffsets;
        private _sHashIndexGroup[] directoryChildHashGroup;


        // V1 arc only
        public _sFileInformationV1[] fileInfoV1;


        // handling
        public bool Initialized { get; internal set; } = false;

        public Dictionary<uint, _sFileInformationV2> pathToFileInfo;
        public Dictionary<uint, _sFileInformationV1> pathToFileInfoV1;

        /// <summary>
        /// Initializes file system from file
        /// </summary>
        public void InitFileSystem(string arcFilePath, bool readOnly = true)
        {
            using (ExtBinaryReader reader = new ExtBinaryReader(new FileStream(arcFilePath, FileMode.Open)))
            {
                Initialized = Init(reader);
                filePath = arcFilePath;
            }
            pathToFileInfo = new Dictionary<uint, _sFileInformationV2>();

            var paths = GetFileList();
            if (Version == 0x00010000)
            {
                pathToFileInfoV1 = new Dictionary<uint, _sFileInformationV1>();
                for (int i = 0; i < paths.Count; i++)
                {
                    uint crc = CRC32.Crc32C(paths[i]);
                    if (!pathToFileInfoV1.ContainsKey(crc))
                        pathToFileInfoV1.Add(crc, fileInfoV1[i]);
                }
            }
            else
            {
                pathToFileInfo = new Dictionary<uint, _sFileInformationV2>();
                for (int i = 0; i < paths.Count; i++)
                {
                    uint crc = CRC32.Crc32C(paths[i]);
                    if (!pathToFileInfo.ContainsKey(crc))
                        pathToFileInfo.Add(crc, fileInfoV2[i]);
                }
            }
        }

        /// <summary>
        /// Initializes arc from binary reader
        /// </summary>
        /// <param name="reader"></param>
        private bool Init(ExtBinaryReader reader)
        {
            if (!HashDict.Initialized)
                HashDict.Init();

            if (reader.BaseStream.Length < Marshal.SizeOf<_sArcHeader>())
                return false;

            header = reader.ReadType<_sArcHeader>();

            if (header.Magic != Magic)
                throw new InvalidDataException("ARC magic does not match");

            reader.BaseStream.Seek(header.FileSystemOffset, SeekOrigin.Begin);
            if (header.FileDataOffset < 0x8824AF68)
            {
                Version = 0x00010000;
                ReadFileSystemV1(ReadCompressedTable(reader));
            }
            else
            {
                ReadFileSystem(ReadCompressedTable(reader));
            }

            //reader.BaseStream.Seek(header.FileSystemSearchOffset, SeekOrigin.Begin);
            //ReadSearchTable(ReadCompressedTable(reader));

            return true;
        }

        /// <summary>
        /// Reads file system table from Arc
        /// </summary>
        /// <param name="fileSystemTable"></param>
        private void ReadFileSystem(byte[] fileSystemTable)
        {
            using (ExtBinaryReader reader = new ExtBinaryReader(new MemoryStream(fileSystemTable)))
            {
                FSHeader = reader.ReadType<_sFileSystemHeader>();

                uint ExtraFolder = 0;
                uint ExtraCount = 0;

                if(fileSystemTable.Length >= 0x2992DD4)
                {
                    // Version 3+
                    Version = reader.ReadInt32();

                    ExtraFolder = reader.ReadUInt32(); 
                    ExtraCount = reader.ReadUInt32(); 

                    reader.ReadBytes(0x10);  // some extra thing :thinking
                }
                else
                {
                    Version = 0x00020000;
                    reader.BaseStream.Seek(0x3C, SeekOrigin.Begin);
                }

                // skip this for now
                regionalbytes = reader.ReadBytes(0xE * 12);

                // Streams
                StreamHeader = reader.ReadType<_sStreamHeader>();

                streamUnk = reader.ReadType<_sStreamUnk>(StreamHeader.UnkCount);
                
                streamHashToName = reader.ReadType<_sStreamHashToName>(StreamHeader.StreamHashCount);
                
                streamNameToHash = reader.ReadType<_sStreamNameToHash>(StreamHeader.StreamHashCount);

                streamIndexToFile = reader.ReadType<_sStreamIndexToOffset>(StreamHeader.StreamIndexToOffsetCount);

                streamOffsets = reader.ReadType<_sStreamOffset>(StreamHeader.StreamOffsetCount);

                Console.WriteLine(reader.BaseStream.Position.ToString("X")); 

                // Unknown
                uint UnkCount1 = reader.ReadUInt32();
                uint UnkCount2 = reader.ReadUInt32();
                fileInfoUnknownTable = reader.ReadType<_sFileInformationUnknownTable>(UnkCount2);
                filePathToIndexHashGroup = reader.ReadType<_sHashIndexGroup>(UnkCount1);

                // FileTables
                
                fileInfoPath = reader.ReadType<_sFileInformationPath>(FSHeader.FileInformationPathCount);

                fileInfoIndex = reader.ReadType<_sFileInformationIndex>(FSHeader.FileInformationIndexCount);

                // directory tables

                // directory hashes by length and index to directory probably 0x6000 something
                Console.WriteLine(reader.BaseStream.Position.ToString("X"));
                directoryHashGroup = reader.ReadType<_sHashIndexGroup>(FSHeader.DirectoryCount);
                
                directoryList = reader.ReadType<_sDirectoryList>(FSHeader.DirectoryCount);
                
                directoryOffsets = reader.ReadType<_sDirectoryOffset>(FSHeader.DirectoryOffsetCount1 + FSHeader.DirectoryOffsetCount2 + ExtraFolder);
                
                directoryChildHashGroup = reader.ReadType<_sHashIndexGroup>(FSHeader.DirectoryHashSearchCount);

                // file information tables
                Console.WriteLine(reader.BaseStream.Position.ToString("X"));
                fileInfoV2 = reader.ReadType<_sFileInformationV2>(FSHeader.FileInformationCount + FSHeader.SubFileCount2 + ExtraCount);
                
                fileInfoSubIndex = reader.ReadType<_sFileInformationSubIndex>(FSHeader.FileInformationSubIndexCount + FSHeader.SubFileCount2 + ExtraCount);
                
                subFiles = reader.ReadType<_sSubFileInfo>(FSHeader.SubFileCount + FSHeader.SubFileCount2);

                Console.WriteLine(reader.BaseStream.Position.ToString("X"));
                //uint max = 0;
                /*using (StreamWriter writer = new StreamWriter(new FileStream("FS1.txt", FileMode.Create)))
                    for (int i = 0; i < (int)FSHeader.FileInformationCount; i++)
                    {
                        var fileinfo = fileInfoV2[i];
                        var path = fileInfoPath[fileinfo.PathIndex];
                        var subindex = fileInfoSubIndex[fileinfo.SubIndexIndex];
                        writer.WriteLine(fileinfo.Flags.ToString("X") + " " + fileinfo.PathIndex.ToString("X") + " " + subindex.SubFileIndex.ToString("X") + " " + HashDict.GetString(path.Path) + " " + HashDict.GetString(path.FileName));
                        //max = Math.Max(max, fp.SomeIndex2);
                    }
                using (StreamWriter writer = new StreamWriter(new FileStream("FS2.txt", FileMode.Create)))
                for (int i = (int)FSHeader.FileInformationCount ;i < fileInfoV2.Length; i++)
                    {
                        var fileinfo = fileInfoV2[i];
                        var path = fileInfoPath[fileinfo.PathIndex];
                        var subindex = fileInfoSubIndex[fileinfo.SubIndexIndex];
                        writer.WriteLine(fileinfo.Flags.ToString("X") + " " + fileinfo.PathIndex.ToString("X") + " " + subindex.SubFileIndex.ToString("X") + " " + HashDict.GetString(path.Path) + " " + HashDict.GetString(path.FileName));
                        //max = Math.Max(max, fp.SomeIndex2);
                    }*/
                //Console.WriteLine("Max: " + max.ToString("X"));

                /*int MaxIntTableValue = 0;
                foreach (var g in fileInfoIndex)
                {
                    MaxIntTableValue = Math.Max((int)g.FileInformationIndex, MaxIntTableValue);
                    //Console.WriteLine(HashDict.GetString(g.Hash));
                }
                Console.WriteLine("Max table value: " + MaxIntTableValue.ToString("X"));*/

                /*var flags = new System.Collections.Generic.List<uint>();
                foreach (var g in fileInfoV2)
                {
                    if (!flags.Contains(g.Flags))
                        flags.Add(g.Flags);
                }*/
                /*using (StreamWriter writer = new StreamWriter(new FileStream("print.txt", FileMode.Create)))
                    foreach (var g in PathNameHashLengthLookup)
                    {
                        writer.WriteLine(HashDict.GetString(g.FilePathHash) + " " + HashDict.GetString(PathNameHashGroupLookup[IndexTable[g.ExtensionHash]].FileNameHash));
                    }*/
            }
        }

        public void WriteFileSystem(string filename)
        {
            MemoryStream stream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(GetBytes(FSHeader));

                writer.Write(regionalbytes);

                writer.Write(GetBytes(StreamHeader));

                writer.Write(GetBytes(streamUnk));

                writer.Write(GetBytes(streamHashToName));

                writer.Write(GetBytes(streamNameToHash));

                writer.Write(GetBytes(streamIndexToFile));

                writer.Write(GetBytes(streamOffsets));

                writer.Write(filePathToIndexHashGroup.Length);
                writer.Write(fileInfoUnknownTable.Length);

                writer.Write(GetBytes(fileInfoUnknownTable));

                writer.Write(GetBytes(filePathToIndexHashGroup));

                writer.Write(GetBytes(fileInfoPath));

                writer.Write(GetBytes(fileInfoIndex));

                writer.Write(GetBytes(directoryHashGroup));

                writer.Write(GetBytes(directoryList));
                writer.Write(GetBytes(directoryOffsets));
                writer.Write(GetBytes(directoryChildHashGroup));

                // file information tables

                writer.Write(GetBytes(fileInfoV2));
                writer.Write(GetBytes(fileInfoSubIndex));
                writer.Write(GetBytes(subFiles));

            }

            byte[] data = stream.ToArray();
            stream.Dispose();
            using (var memoryStream = new MemoryStream())
            {
                using (var zstream = new ZstandardStream(memoryStream, 20, true))
                {
                    zstream.Write(data, 0, data.Length);
                }
                File.WriteAllBytes("tablecompressed.bin", memoryStream.ToArray());
            }
        }

        private static byte[] GetBytes<T>(T[] str)
        {
            int size = Marshal.SizeOf<T>();
            byte[] arr = new byte[size * str.Length];

            for(int i = 0; i < str.Length; i++)
            {
                byte[] bytes = GetBytes(str[i]);
                Array.Copy(bytes, 0, arr, i * size, bytes.Length);
            }
            return arr;
        }

        private static byte[] GetBytes<T>(T str)
        {
            int size = Marshal.SizeOf<T>();
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// Reads the search table information for V2 Arc
        /// </summary>
        /// <param name="searchTable"></param>
        private void ReadSearchTable(byte[] searchTable)
        {
            using (ExtBinaryReader r = new ExtBinaryReader(new MemoryStream(searchTable)))
            {
                var header = r.ReadType<_sSearchHashHeader>();

                // paths
                var ActualPathHashSection = r.ReadType<_sHashIndexGroup>(header.FolderLengthHashCount);

                int MaxFolderIndex = 0;
                foreach(var section in ActualPathHashSection)
                    MaxFolderIndex = Math.Max(MaxFolderIndex, section.index >> 8);
                Console.WriteLine("Max FolderIndex " + MaxFolderIndex.ToString("X"));

                // path lookup group
                var UnkHashGroup = r.ReadType<_sHashGroup>(header.FolderLengthHashCount);
                Console.WriteLine("End at: " + r.BaseStream.Position.ToString("X"));

                // file paths
                var ActualFullPath = r.ReadType<_sHashIndexGroup>(header.SomeCount3);
                Console.WriteLine("End at: " + r.BaseStream.Position.ToString("X"));

                // index to look up pathgroup? why they needed a separate index who knows...
                var IndexTable = r.ReadType<int>(header.SomeCount3);
                Console.WriteLine("End at: " + r.BaseStream.Position.ToString("X"));

                // file path lookup group
                var PathNameHashGroupLookup = r.ReadType<_sHashGroup>(header.SomeCount4);
                Console.WriteLine("End at: " + r.BaseStream.Position.ToString("X"));

                int MaxIntTableValue = 0;
                foreach (var g in UnkHashGroup)
                {
                    MaxIntTableValue = Math.Max((int)g.ExtensionHash, MaxIntTableValue);
                    //Console.WriteLine(HashDict.GetString(g.Hash));
                }
                Console.WriteLine("Max table value: " + MaxIntTableValue.ToString("X"));

                
                /*using (StreamWriter writer = new StreamWriter(new FileStream("print.txt", FileMode.Create)))
                    foreach (var g in ActualFullPath)
                    {
                        writer.WriteLine(HashDict.GetString(g.Hash));
                    }*/
            }
        }

        /// <summary>
        /// Arc versions other than 1.0 have compressed tables
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private byte[] ReadCompressedTable(ExtBinaryReader reader)
        {
            var compHeader = reader.ReadType<_sCompressedTableHeader>();

            if(compHeader.DataOffset > 0x10)
            {
                var tableStart = reader.BaseStream.Position - 0x10;
                reader.BaseStream.Seek(tableStart, SeekOrigin.Begin);
                var size = reader.ReadInt32();
                reader.BaseStream.Seek(tableStart, SeekOrigin.Begin);
                return reader.ReadBytes(size);
            }
            else
            if(compHeader.DataOffset == 0x10)
            {
                return reader.ReadZstdCompressed(compHeader.CompressedSize);
            }
            else
            {
                return reader.ReadBytes(compHeader.CompressedSize);
            }
        }

        #region functions

        public long MaxOffset()
        {
            long max = 0;
            foreach(var fi in fileInfoV2)
            {
                var path = fileInfoPath[fi.PathIndex];
                var subindex = fileInfoSubIndex[fi.SubIndexIndex];

                var subfile = subFiles[subindex.SubFileIndex];

                var directoryOffset = directoryOffsets[subindex.DirectoryOffsetIndex];
                max = Math.Max(max, header.FileDataOffset + directoryOffset.Offset + (subfile.Offset<<2));
                
                if (header.FileDataOffset + directoryOffset.Offset + (subfile.Offset << 2) > header.FileDataOffset2)
                    Console.WriteLine(HashDict.GetString(path.FileName));
            }
            return max;
        }

        public void PrintDirectoryInfo(string directory)
        {
            uint hash = CRC32.Crc32C(directory);
            int count = -1;
            foreach(var dir in directoryHashGroup)
            {
                //Console.WriteLine(HashDict.GetString(dir.Hash) + " " + dir.index.ToString("X"));
                count++;
                if(dir.Hash == hash)
                {
                    var direct = directoryList[dir.index >> 8];
                    Console.WriteLine(directory);

                    Console.WriteLine("Sub Folders:");
                    for(int i = direct.ChildDirectoryStartIndex; i < direct.ChildDirectoryStartIndex + direct.ChildDirectoryCount; i++)
                    {
                        var hashGroup = directoryChildHashGroup[i];
                        Console.WriteLine("\t" + HashDict.GetString(hashGroup.Hash));
                    }

                    Console.WriteLine("Sub Files:");
                    for (int i = direct.FileInformationStartIndex; i < direct.FileInformationStartIndex + direct.FileInformationCount; i++)
                    {
                        var fileinfo = fileInfoV2[i];

                        var fileindex = fileInfoIndex[fileinfo.IndexIndex];

                        //redirect
                        if ((fileinfo.Flags & 0x00000010) == 0x10)
                        {
                            fileinfo = fileInfoV2[fileindex.FileInformationIndex];
                        }

                        var path = fileInfoPath[fileinfo.PathIndex];
                        var subindex = fileInfoSubIndex[fileinfo.SubIndexIndex];

                        var subfile = subFiles[subindex.SubFileIndex];
                        var directoryOffset = directoryOffsets[subindex.DirectoryOffsetIndex];
                        
                        //regional
                        if ((fileinfo.Flags & 0x00008000) == 0x8000)
                        {
                            subindex = fileInfoSubIndex[fileinfo.SubIndexIndex+2];
                            subfile = subFiles[subindex.SubFileIndex];
                            directoryOffset = directoryOffsets[subindex.DirectoryOffsetIndex];
                        }

                        Console.WriteLine("\t" + HashDict.GetString(path.Parent) + HashDict.GetString(path.FileName));
                        Console.WriteLine("\t\t" + i + " " + fileinfo.SubIndexIndex + " " + path.DirectoryIndex + " " + fileinfo.IndexIndex + " " + (header.FileDataOffset + directoryOffset.Offset + (subfile.Offset<<2)).ToString("X") + " " + subfile.CompSize.ToString("X") + " " + subfile.DecompSize.ToString("X") + " " + subfile.Flags.ToString("X") + " " + fileinfo.Flags.ToString("X") + " " + subindex.SubFileIndex + " " + directoryOffset.SubDataStartIndex + " " + subindex.FileInformationIndexAndFlag.ToString("X"));
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// returns an unorganized list of the files in the arc excluding stream files
        /// </summary>
        /// <returns></returns>
        public List<string> GetFileList()
        {
            // Optimize repeated calls.
            if (filePaths.Count > 0)
                return filePaths;

            if (Version == 0x00010000)
            {
                filePaths = GetFileListV1();
            }
            else
            {
                filePaths = new List<string>(fileInfoV2.Length);

                foreach (var fileinfo in fileInfoV2)
                {
                    var path = fileInfoPath[fileinfo.PathIndex];

                    string pathString = HashDict.GetString(path.Parent, (int)(path.Unk5 & 0xFF));
                    if (pathString.StartsWith("0x"))
                        pathString += "/";

                    filePaths.Add(pathString + HashDict.GetString(path.FileName, (int)(path.Unk6 & 0xFF)));
                }
            }

            return filePaths;
        }

        /// <summary>
        /// returns the decompressed file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public byte[] GetFile(string filepath, int region = 0)
        {
            long Offset;
            uint CompSize, DecompSize;
            bool regional;
            GetFileInformation(filepath, out Offset, out CompSize, out DecompSize, out regional, region);

            if(DecompSize > 0 && DecompSize != CompSize)
            {
                var decompressed = ExtBinaryReader.DecompressZstd(GetSection(Offset, CompSize));
                if (decompressed.Length != DecompSize)
                    throw new InvalidDataException("Error decompressing file");
                return decompressed;
            }
            return GetSection(Offset, CompSize);
        }

        /// <summary>
        /// returns the compressed version of the file (if it is compressed)
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public byte[] GetFileCompressed(string filepath, int region = 0)
        {
            long Offset;
            uint CompSize, DecompSize;
            bool regional;
            GetFileInformation(filepath, out Offset, out CompSize, out DecompSize, out regional, region);
            return GetSection(Offset, CompSize);
        }

        /// <summary>
        /// returns a section of the arc
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        private byte[] GetSection(long offset, uint Size)
        {
            if (!Initialized)
                return new byte[0];
            byte[] data;
            using (BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                reader.BaseStream.Position = offset;
                data = reader.ReadBytes((int)Size);
            }
            return data;
        }


        /// <summary>
        /// gets file information from the file's path
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="offset"></param>
        /// <param name="compSize"></param>
        /// <param name="decompSize"></param>
        /// <param name="regionIndex"></param>
        public void GetFileInformation(string filepath, out long offset, out uint compSize, out uint decompSize, out bool regional, int regionIndex = 0)
        {
            offset = 0;
            compSize = 0;
            decompSize = 0;
            regional = false;
            uint crc = CRC32.Crc32C(filepath);
            if ((Version != 0x00010000 && !pathToFileInfo.ContainsKey(crc)) ||
                (Version == 0x00010000 && !pathToFileInfoV1.ContainsKey(crc)))
            {   //
                // check for stream file

                foreach (var fileinfo in streamNameToHash)
                {
                    if (fileinfo.Hash == crc)
                    {
                        if (fileinfo.Flags == 1 || fileinfo.Flags == 2)
                        {
                            if (fileinfo.Flags == 2 && regionIndex > 5)
                                regionIndex = 0;

                            var streamindex = streamIndexToFile[(fileinfo.NameIndex >> 8) + regionIndex].FileIndex;
                            var offsetinfo = streamOffsets[streamindex];
                            offset = offsetinfo.Offset;
                            compSize = (uint)offsetinfo.Size;
                            decompSize = (uint)offsetinfo.Size;
                            regional = true;
                        }
                        else
                        {
                            var streamindex = streamIndexToFile[fileinfo.NameIndex >> 8].FileIndex;
                            var offsetinfo = streamOffsets[streamindex];
                            offset = offsetinfo.Offset;
                            compSize = (uint)offsetinfo.Size;
                            decompSize = (uint)offsetinfo.Size;
                        }
                        return;
                    }
                }

                return;
            }
            if (IsRegional(filepath))
                regional = true;

            if(Version == 0x00010000)
                GetFileInformation(pathToFileInfoV1[crc], out offset, out compSize, out decompSize, regionIndex);
            else
                GetFileInformation(pathToFileInfo[crc], out offset, out compSize, out decompSize, regionIndex);
        }

        /// <summary>
        /// Gets the information for a given file info
        /// </summary>
        /// <param name="fileinfo"></param>
        /// <param name="offset"></param>
        /// <param name="compSize"></param>
        /// <param name="decompSize"></param>
        /// <param name="regionIndex"></param>
        private void GetFileInformation(_sFileInformationV2 fileinfo, out long offset, out uint compSize, out uint decompSize, int regionIndex = 0)
        {
            var fileindex = fileInfoIndex[fileinfo.IndexIndex];

            //redirect
            if ((fileinfo.Flags & 0x00000010) == 0x10)
            {
                fileinfo = fileInfoV2[fileindex.FileInformationIndex];
            }

            var path = fileInfoPath[fileinfo.PathIndex];
            var subindex = fileInfoSubIndex[fileinfo.SubIndexIndex];

            var subfile = subFiles[subindex.SubFileIndex];
            var directoryOffset = directoryOffsets[subindex.DirectoryOffsetIndex];

            //regional
            if ((fileinfo.Flags & 0x00008000) == 0x8000)
            {
                subindex = fileInfoSubIndex[fileinfo.SubIndexIndex + 1 + regionIndex];
                subfile = subFiles[subindex.SubFileIndex];
                directoryOffset = directoryOffsets[subindex.DirectoryOffsetIndex];
            }

            offset = (header.FileDataOffset + directoryOffset.Offset + (subfile.Offset << 2));
            compSize = subfile.CompSize;
            decompSize = subfile.DecompSize;
        }

        public bool IsRedirected(string path)
        {
            uint crc = CRC32.Crc32C(path);
            if (pathToFileInfoV1 != null && pathToFileInfoV1.ContainsKey(crc))
                return ((pathToFileInfoV1[crc].Flags & 0x00300000) == 0x00300000);
            if (pathToFileInfo != null && pathToFileInfo.ContainsKey(crc))
                return (pathToFileInfo[crc].Flags & 0x00000010) == 0x10;
            return false;
        }

        public bool IsRegional(string path)
        {
            uint crc = CRC32.Crc32C(path);
            if (pathToFileInfoV1 != null && pathToFileInfoV1.ContainsKey(crc))
                return ((pathToFileInfoV1[crc].FileTableFlag >> 8) > 0);
            if (pathToFileInfo != null && pathToFileInfo.ContainsKey(crc))
                return ((pathToFileInfo[crc].Flags & 0x00008000) == 0x8000);
            return false;
        }


        /// <summary>
        /// returns an unorganized list of the stream files in the arc
        /// </summary>
        /// <returns></returns>
        public List<string> GetStreamFileList()
        {
            var files = new List<string>(streamNameToHash.Length);

            foreach (var fileinfo in streamNameToHash)
            {
                files.Add(HashDict.GetString(fileinfo.Hash, (int)(fileinfo.NameIndex&0xFF)));
            }

            return files;
        }

#endregion

        #region ARC v1

        /// <summary>
        /// Parses filesystem from the 1.0.0 arc
        /// </summary>
        /// <param name="fileSystemTable"></param>
        private void ReadFileSystemV1(byte[] fileSystemTable)
        {
            using (ExtBinaryReader reader = new ExtBinaryReader(new MemoryStream(fileSystemTable)))
            {
                reader.BaseStream.Position = 0;
                var NodeHeader = reader.ReadType<_sFileSystemHeaderV1>();

                reader.BaseStream.Seek(0x68, SeekOrigin.Begin);

                // Hash Table
                reader.BaseStream.Seek(0x8 * NodeHeader.Part1Count, SeekOrigin.Current);

                // Hash Table 2
                System.Diagnostics.Debug.WriteLine("stream " + reader.BaseStream.Position.ToString("X"));
                streamNameToHash = reader.ReadType<_sStreamNameToHash>(NodeHeader.Part1Count);

                // Hash Table 3
                System.Diagnostics.Debug.WriteLine("stream " + reader.BaseStream.Position.ToString("X"));
                streamIndexToFile = reader.ReadType<_sStreamIndexToOffset>(NodeHeader.Part2Count);

                // stream offsets
                System.Diagnostics.Debug.WriteLine("stream " + reader.BaseStream.Position.ToString("X"));
                streamOffsets = reader.ReadType<_sStreamOffset>(NodeHeader.MusicFileCount);

                // Another Hash Table
                System.Diagnostics.Debug.WriteLine("RegionalHashList " + reader.BaseStream.Position.ToString("X"));
                reader.BaseStream.Seek(0xC * 0xE, SeekOrigin.Current);

                //folders

                System.Diagnostics.Debug.WriteLine("FolderHashes " + reader.BaseStream.Position.ToString("X"));
                directoryList = reader.ReadType<_sDirectoryList>(NodeHeader.FolderCount);

                //file offsets

                System.Diagnostics.Debug.WriteLine("fileoffsets " + reader.BaseStream.Position.ToString("X"));
                directoryOffsets = reader.ReadType<_sDirectoryOffset>(NodeHeader.FileCount1 + NodeHeader.FileCount2);
                //DirectoryOffsets_2 = reader.ReadType<_sDirectoryOffsets>(R, NodeHeader.FileCount2);

                System.Diagnostics.Debug.WriteLine("subfileoffset " + reader.BaseStream.Position.ToString("X"));
                directoryChildHashGroup = reader.ReadType<_sHashIndexGroup>(NodeHeader.HashFolderCount);

                System.Diagnostics.Debug.WriteLine("subfileoffset " + reader.BaseStream.Position.ToString("X"));
                fileInfoV1 = reader.ReadType<_sFileInformationV1>(NodeHeader.FileInformationCount);

                System.Diagnostics.Debug.WriteLine("subfileoffset " + reader.BaseStream.Position.ToString("X"));
                subFiles = reader.ReadType<_sSubFileInfo>(NodeHeader.SubFileCount + NodeHeader.SubFileCount2);
                /*SubFileInformationStart = R.BaseStream.Position;
                SubFileInformation_1 = reader.ReadType<_SubFileInfo>(R, NodeHeader.SubFileCount);
                SubFileInformationStart2 = R.BaseStream.Position;
                SubFileInformation_2 = reader.ReadType<_SubFileInfo>(R, NodeHeader.SubFileCount2);*/

                //_sHashInt[] HashInts = reader.ReadType<_sHashInt>(R, NodeHeader.FolderCount);

                // okay some more file information
                //uint FileHashCount = reader.ReadUInt32();
                //uint UnknownTableCount = reader.ReadUInt32();

                //_sExtraFITable[] Extra1 = reader.ReadType<_sExtraFITable>(R, UnknownTableCount);
                //_sExtraFITable2[] Extra2 = reader.ReadType<_sExtraFITable2>(R, FileHashCount);

                /*reader.BaseStream.Position += 8 * NodeHeader.FileInformationCount;

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
                }*/
            }
        }

        /// <summary>
        /// returns an unorganized list of the files in the arc excluding stream files
        /// </summary>
        /// <returns></returns>
        public List<string> GetFileListV1()
        {
            var files = new List<string>(fileInfoV1.Length);

            foreach (var fileinfo in fileInfoV1)
            {
                string pathString = HashDict.GetString(fileinfo.Parent, (int)(fileinfo.Unk5 & 0xFF));
                if (pathString.StartsWith("0x"))
                    pathString += "/";

                files.Add(pathString + HashDict.GetString(fileinfo.Hash2, (int)(fileinfo.Unk6 & 0xFF)));
            }

            return files;
        }

        private void GetFileInformation(_sFileInformationV1 fileinfo, out long offset, out uint compSize, out uint decompSize, int regionIndex = 0)
        {
            var subfile = subFiles[fileinfo.SubFile_Index];
            var dirIndex = directoryList[fileinfo.DirectoryIndex >> 8].FullPathHashLengthAndIndex >> 8;
            var directoryOffset = directoryOffsets[dirIndex];
            
            //redirect
            if ((fileinfo.Flags & 0x00300000) == 0x00300000)
            {
                GetFileInformation(fileInfoV1[subfile.Flags], out offset, out compSize, out decompSize, regionIndex);
                return;
            }

            //regional
            if (IsRegional(fileinfo))
            {
                subfile = subFiles[(fileinfo.FileTableFlag >> 8) + regionIndex];
                directoryOffset = directoryOffsets[dirIndex + 1 + regionIndex];
            }

            offset = (header.FileDataOffset + directoryOffset.Offset + (subfile.Offset << 2));
            compSize = subfile.CompSize;
            decompSize = subfile.DecompSize;
        }

        private bool IsRegional(_sFileInformationV1 info)
        {
            return (info.FileTableFlag >> 8) > 0;
        }

        #endregion
    }
}
