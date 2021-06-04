using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emedia_zad1.Chunks
{
    public abstract class Chunk
    {
        public char[] type;
        public uint length;
        public byte[] data;

        public Chunk(char[] type, uint length, byte[] data)
        {
            this.type = type;
            this.length = length;
            this.data = data;
        }
    }
}
