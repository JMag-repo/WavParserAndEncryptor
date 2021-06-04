using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emedia_zad1.Chunks
{
    public class INFOChunk : Chunk
    {
        public INFOChunk(char[] type, uint length, byte[] data) : base(type, length, data)
        {
        }
        INFOsubChunk IART;  //wykonawca
        INFOsubChunk INAM;  //tytuł 
        INFOsubChunk IPRD;  //tytuł albumu
        INFOsubChunk ICRD;  //data wydania
        INFOsubChunk IGNR;  //gatunek   
        INFOsubChunk ICMT;  //komentarze
        INFOsubChunk ITRK;  //komentarze
        INFOsubChunk ISFT;  //oprogramowanie
        byte[] nierozpoznane;

        public void RecognizeChunk(char[] type,uint len,byte[] data)
        {
            string CatchedType = new string(type);
            switch (CatchedType)
            {
                case "IART":
                    IART = new INFOsubChunk(type, len, data);
                    break;
                case "INAM":
                    INAM = new INFOsubChunk(type, len, data);
                    break;
                case "IPRD":
                    IPRD = new INFOsubChunk(type, len, data);
                    break;
                case "ICRD":
                    ICRD = new INFOsubChunk(type, len, data);
                    break;
                case "IGNR":
                    IGNR = new INFOsubChunk(type, len, data);
                    break;
                case "ICMT":
                    ICMT = new INFOsubChunk(type, len, data);
                    break;
                case "ITRK":
                    ITRK = new INFOsubChunk(type, len, data);
                    break;
                case "ISFT":
                    ISFT = new INFOsubChunk(type, len, data);
                    break;
                

            }

        }
        public void ShowInfoChunk()
        {
            if (IPRD != null)
                Console.WriteLine("IPRD " + IPRD.ShowINFOsubChunk());
            if (IART != null)
                Console.WriteLine("IART " + IART.ShowINFOsubChunk());
            if (INAM != null)
                Console.WriteLine("INAM " + INAM.ShowINFOsubChunk());
            if (ICRD != null)
                Console.WriteLine("ICRD " + ICRD.ShowINFOsubChunk());
            if (IGNR != null)
                Console.WriteLine("IGNR " + IGNR.ShowINFOsubChunk());
            if (ITRK != null)
                Console.WriteLine("ITRK " + ITRK.ShowINFOsubChunk());
            if (ICMT != null)
                Console.WriteLine("ICMT " + ICMT.ShowINFOsubChunk());
            if (ISFT != null)
                Console.WriteLine("ISFT " + ISFT.ShowINFOsubChunk());
        }
    }
}
