using CrossArc.Structs;
using System.IO;

namespace CrossArc
{
    /// <summary>
    /// Contains methods for reading arrays of specific structs.
    /// Parsing structs manually using a binary reader is verbose but has a noticeable performance improvement.
    /// </summary>
    internal static class ArcArrayReading
    {
        public static _sFileInformationPath[] ReadSFileInformationPaths(BinaryReader reader, uint count)
        {
            _sFileInformationPath[] array = new _sFileInformationPath[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sFileInformationPath
                {
                    Path = reader.ReadUInt32(),
                    DirectoryIndex = reader.ReadUInt32(),
                    Extension = reader.ReadUInt32(),
                    FileTableFlag = reader.ReadUInt32(),
                    Parent = reader.ReadUInt32(),
                    Unk5 = reader.ReadUInt32(),
                    Hash2 = reader.ReadUInt32(),
                    Unk6 = reader.ReadUInt32()
                };
            }

            return array;
        }

        public static _sStreamIndexToFile[] ReadSStreamIndicesToFiles(BinaryReader reader, uint count)
        {
            _sStreamIndexToFile[] array = new _sStreamIndexToFile[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sStreamIndexToFile
                {
                    FileIndex = reader.ReadInt32()
                };
            }

            return array;
        }

        public static _sStreamNameToHash[] ReadSStreamNamesToHashes(BinaryReader reader, uint count)
        {
            _sStreamNameToHash[] array = new _sStreamNameToHash[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sStreamNameToHash
                {
                    Hash = reader.ReadUInt32(),
                    NameIndex = reader.ReadUInt32(),
                    Flags = reader.ReadUInt32()
                };
            }

            return array;
        }

        public static _sStreamOffset[] ReadSStreamOffsets(BinaryReader reader, uint count)
        {
            _sStreamOffset[] array = new _sStreamOffset[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sStreamOffset
                {
                    Size = reader.ReadInt64(),
                    Offset = reader.ReadInt64()
                };
            }

            return array;
        }

        public static _sDirectoryList[] ReadSDirectoryLists(BinaryReader reader, uint count)
        {
            _sDirectoryList[] array = new _sDirectoryList[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sDirectoryList
                {
                    HashID = reader.ReadUInt32(),
                    DirOffsetIndex = reader.ReadUInt32(),
                    NameHash = reader.ReadUInt32(),
                    ParentHash = reader.ReadUInt32(),

                    Hash4 = reader.ReadUInt32(),
                    FirstFilIndex = reader.ReadUInt32(),
                    unk = reader.ReadUInt32(),
                    FirstFileIndex = reader.ReadUInt32(),

                    FileInformationStartIndex = reader.ReadUInt32(),
                    FileInformationCount = reader.ReadUInt32(),
                    FileNameStartIndex = reader.ReadUInt32(),
                    FileNameCount = reader.ReadInt16(),
                    Unk4 = reader.ReadUInt16(),

                    Unk5 = reader.ReadUInt32()
                };
            }

            return array;
        }

        public static _sDirectoryOffsets[] ReadDirectoryOffsets(BinaryReader reader, uint count)
        {
            _sDirectoryOffsets[] array = new _sDirectoryOffsets[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sDirectoryOffsets
                {
                    Offset = reader.ReadInt64(),
                    DecompSize = reader.ReadUInt32(),
                    Size = reader.ReadUInt32(),
                    SubDataStartIndex = reader.ReadUInt32(),
                    SubDataCount = reader.ReadUInt32(),
                    ResourceIndex = reader.ReadUInt32(),
                };
            }

            return array;
        }

        public static _sFolderHashIndex[] ReadSFolderHashIndices(BinaryReader reader, uint count)
        {
            _sFolderHashIndex[] array = new _sFolderHashIndex[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sFolderHashIndex
                {
                    Hash = reader.ReadUInt32(),
                    Count = reader.ReadUInt32()
                };
            }

            return array;
        }

        public static _sFileInformationIndex[] ReadSFileInformationIndices(BinaryReader reader, uint count)
        {
            _sFileInformationIndex[] array = new _sFileInformationIndex[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sFileInformationIndex
                {
                    SomeIndex1 = reader.ReadUInt32(),
                    SomeIndex2 = reader.ReadUInt32()
                };
            }

            return array;
        }

        public static _sFileInformationV2[] ReadSFileInformationV2(BinaryReader reader, uint count)
        {
            _sFileInformationV2[] array = new _sFileInformationV2[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sFileInformationV2
                {
                    HashIndex = reader.ReadUInt32(),
                    HashIndex2 = reader.ReadUInt32(),
                    SubFile_Index = reader.ReadUInt32(),
                    Flags = reader.ReadUInt32()
                };
            }

            return array;
        }

        public static _sFileInformationSubIndex[] ReadSFileInformationSubIndices(BinaryReader reader, uint count)
        {
            _sFileInformationSubIndex[] array = new _sFileInformationSubIndex[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sFileInformationSubIndex
                {
                    SomeIndex1 = reader.ReadUInt32(),
                    SomeIndex2 = reader.ReadUInt32(),
                    SomeIndex3 = reader.ReadUInt32()
                };
            }

            return array;
        }

        public static _SubFileInfo[] ReadSubFileInfos(BinaryReader reader, uint count)
        {
            _SubFileInfo[] array = new _SubFileInfo[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _SubFileInfo
                {
                    Offset = reader.ReadUInt32(),
                    CompSize = reader.ReadUInt32(),
                    DecompSize = reader.ReadUInt32(),
                    Flags = reader.ReadUInt32()
                };
            }

            return array;
        }


        public static _sHashInt[] ReadHashInts(BinaryReader reader, uint count)
        {
            _sHashInt[] array = new _sHashInt[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new _sHashInt
                {
                    Hash = reader.ReadUInt32(),
                    Index = reader.ReadUInt32()
                };
            }

            return array;
        }
    }
}
