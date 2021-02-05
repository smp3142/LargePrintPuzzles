using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace GamesData
{
    public static class Utils
    {
        public static List<string> DecompressAndDeserialize(this string base64Data)
        {
            byte[] data = Convert.FromBase64String(base64Data);
            List<string> outData = new List<string>();
            using MemoryStream memoryStream = new MemoryStream(data);
            using DeflateStream compressedStream = new DeflateStream(memoryStream, CompressionMode.Decompress, true);
            using BinaryReader reader = new BinaryReader(compressedStream);
            while (true)
            {
                try
                {
                    outData.Add(reader.ReadString());
                }
                catch (EndOfStreamException)
                {
                    return outData;
                }
            }
        }
    }
}
