using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emedia_zad1.Chunks
{
    public class INFOsubChunk : Chunk
    {
        string info;
        public INFOsubChunk(char[] type, uint length, byte[] data) : base(type, length, data)
        {
            info = System.Text.Encoding.UTF8.GetString(this.data);
        }
        public string ShowINFOsubChunk()
        {
            
            return this.info;
        }
    }
}
