using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Emedia_zad1.Chunks;

namespace Emedia_zad1
{
    public static class BinaryWriterMethods
    {
        public static byte[] UlongToByteArray(ulong number)
        {
            return BitConverter.GetBytes(number).Reverse().ToArray();
        }
        public static void WriteUlong(this BinaryWriter writer, ulong number)
        {
            writer.Write(UlongToByteArray(number));
        }

        public static void WriteChunk(this BinaryWriter writer, Chunk chunk)
        {
            writer.Write(chunk.type);
            writer.WriteUlong(chunk.length);
            writer.Write(chunk.data);
        }
    }
}
