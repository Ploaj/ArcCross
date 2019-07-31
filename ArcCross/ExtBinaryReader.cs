using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Zstandard.Net;

namespace ArcCross
{
    public class ExtBinaryReader : BinaryReader
    {
        public ExtBinaryReader(Stream stream) : base(stream)
        {

        }
        
        /// <summary>
        /// Reads and decompresses ZSTD compressed data
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] ReadZstdCompressed(int size)
        {
            byte[] compressed = ReadBytes(size);
            using (var memoryStream = new MemoryStream(compressed))
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
            using (var temp = new MemoryStream())
            {
                compressionStream.CopyTo(temp);
                return temp.ToArray();
            }
        }

        /// <summary>
        /// decompresses given zstd compressed data
        /// </summary>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static byte[] DecompressZstd(byte[] compressed)
        {
            using (var memoryStream = new MemoryStream(compressed))
            using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
            using (var temp = new MemoryStream())
            {
                compressionStream.CopyTo(temp);
                return temp.ToArray();
            }
        }

        /// <summary>
        /// Reads an array of structs from the reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public T[] ReadType<T>(uint Size) where T : struct
        {
            int sizeOfT = Marshal.SizeOf(typeof(T));

            var buffer = ReadBytes((int)(sizeOfT * Size));

            T[] result = new T[Size];

            var pinnedHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
            Marshal.Copy(buffer, 0, pinnedHandle.AddrOfPinnedObject(), buffer.Length);
            pinnedHandle.Free();

            return result;
        }

        /// <summary>
        /// Reads binary reader into struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public T ReadType<T>()
        {
            byte[] bytes = ReadBytes(Marshal.SizeOf<T>());

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();

            return theStructure;
        }
    }
}
