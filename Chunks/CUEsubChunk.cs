using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emedia_zad1.Chunks
{
    public class CUEsubChunk 
    {
        public string ID;
        public int position;
        public string data_chunk_ID;
        public int chunk_start;
        public int block_start;
        public int sample_offset;
        public CUEsubChunk(byte[] data) 
        {
            this.ID = Encoding.UTF8.GetString(data, 0, 4);           
            this.position = BitConverter.ToInt32(data, 4);      
            this.data_chunk_ID= Encoding.UTF8.GetString(data, 8, 4);
            this.chunk_start= BitConverter.ToInt32(data, 12);
            this.block_start= BitConverter.ToInt32(data, 16);
            this.sample_offset = BitConverter.ToInt32(data, 20);
        }
        
    }
}
