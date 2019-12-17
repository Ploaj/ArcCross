using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
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
        /// <param name="sizeInBytes"></param>
        /// <returns></returns>
        public byte[] ReadZstdCompressed(int sizeInBytes)
        {
            byte[] compressed = ReadBytes(sizeInBytes);
            return DecompressZstd(compressed);
        }

        /// <summary>
        /// decompresses given zstd compressed data
        /// </summary>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static byte[] DecompressZstd(byte[] compressed)
        {
            using (var memoryStream = new MemoryStream(compressed))
            {
                using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
                {
                    // TODO: Try and avoid the additional copy.
                    using (var temp = new MemoryStream())
                    {
                        compressionStream.CopyTo(temp);
                        return temp.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Reads an array of structs from the reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <returns></returns>
        public T[] ReadType<T>(uint count) where T : struct
        {
            int sizeOfT = Marshal.SizeOf(typeof(T));

            var buffer = ReadBytes((int)(sizeOfT * count));

            T[] result = new T[count];

            var pinnedHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
            Marshal.Copy(buffer, 0, pinnedHandle.AddrOfPinnedObject(), buffer.Length);
            pinnedHandle.Free();

            return result;
        }

        /// <summary>
        /// Reads binary reader into struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
