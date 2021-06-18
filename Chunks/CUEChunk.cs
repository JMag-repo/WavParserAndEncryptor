using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emedia_zad1.Chunks 
{
    public class CUEChunk : Chunk
    {
        int numPoints;
        List<CUEsubChunk> Points = new List<CUEsubChunk>();
        public CUEChunk(char[] type, uint length, byte[] data,int numPoints) : base(type, length, data)
        {
            this.numPoints = numPoints;
            for (int i = 0; i < this.numPoints*24; i += 24)
            {
                byte[] temp = new byte[24];
                Buffer.BlockCopy(this.data,i,temp,0,24);
                CUEsubChunk tempCue = new CUEsubChunk(temp);
                Points.Add(tempCue);
            }

        }
        public void ShowData()
        {
            Console.WriteLine("Cue Chunk ");
            Console.WriteLine("Typ: " + new string(type));
            Console.WriteLine("Rozmiar: " + this.length);
            Console.WriteLine("Liczba punktow cue: " + this.numPoints);
            foreach(CUEsubChunk temp in Points)
            {
                Console.WriteLine("Cue ID " + temp.ID);
                Console.WriteLine("position " + temp.position);
                Console.WriteLine("data chunk ID " + temp.data_chunk_ID);
                Console.WriteLine("chunk start " + temp.chunk_start);
                Console.WriteLine("block start " + temp.block_start);
                Console.WriteLine("sample offset " + temp.sample_offset);
            }

        }
    }
}
