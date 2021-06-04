using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using  Emedia_zad1.Chunks;


namespace Header
{
    class HeaderReader
    {   
        
        
        public struct WAV //struktura zawierająca interpretacje pierwszych 44 bajtów z nagłówka reprezentujących informacje o pliku WAV
        {
            public string RIFF;          // 4 bajty
            public int ChunkSize;        // 4 bajty
            public string WAVE;          // 4 bajty
            public string fmt;           // 4 bajty
            public int Subchunk1_size;   // 4 bajty
            public int AudioFormat;      // 2 bajty
            public int NumberOfChannels; // 2 bajty
            public int SampleRate;       // 4 bajty
            public int ByteRate;         // 4 bajty
            public int BlockAlign;       // 2 bajty
            public int BitsPerSample;    // 2 bajty
            public string data;          // 4 bajty
            public int Subchunk2_size;   // 4 bajty
            
        }

        public static INFOChunk infochunk;
        public static WAV wav;
        public static byte[] data { get; set; }

        public static void PrintHeader() //wypisanie w konsoli informacji o nagłówku
        {
            Console.WriteLine("Naglowek pliku WAV:");
            Console.WriteLine("ChunkID: " + wav.RIFF);
            Console.WriteLine("ChunkSize: " + wav.ChunkSize);
            Console.WriteLine("Format: " + wav.WAVE);
            Console.WriteLine("SubChunk1ID: " + wav.fmt);
            Console.WriteLine("Subchunk1Size: " + wav.Subchunk1_size);
            Console.WriteLine("AudioFormat : " + wav.AudioFormat);
            Console.WriteLine("Number of channels: " + wav.NumberOfChannels);
            Console.WriteLine("SampleRate: " + wav.SampleRate);
            Console.WriteLine("ByteRate: " + wav.ByteRate);
            Console.WriteLine("BlockAlign: " + wav.BlockAlign);
            Console.WriteLine("BitsPerSample: " + wav.BitsPerSample);
            Console.WriteLine("Subchunk2ID: " + wav.data);
            Console.WriteLine("Subchunk2Size: " + wav.Subchunk2_size);
        }


        public static Complex[] DFT_transform(Complex[] samples)//transformacja FFT
        {
            int N = samples.Length;
            Complex[] result = new Complex[N];
            for (int k=0; k < N; k++)
            {
                Complex sum = 0;
                for (int n=0; n < N; n++)
                {
                    double omega = 2 * Math.PI * n * k / N;
                    sum += samples[n] * Complex.Exp(new Complex(0, -omega));
                }
                result[k] = sum;
            }
            return result;
        }

        public static double[] Magnitude(Complex[] samples)
        {
            int N = samples.Length;
            double[] result = new double[N];
            for (int i = 0; i < N; i++)
            {
                result[i] = (Math.Sqrt(samples[i].Real * samples[i].Real + samples[i].Imaginary * samples[i].Imaginary));
            }
            return result;
        }

        public static double[] MagnitudeDB(Complex[] samples)
        {
            int N = samples.Length;
            double[] result = new double[N];
            for (int i = 0; i < N; i++)
            {
                result[i] = (((samples[i].Real == 0) && (samples[i].Imaginary == 0)) ? (0.0) : (10.0 * Math.Log10((double)(samples[i].Real * samples[i].Real + samples[i].Imaginary * samples[i].Imaginary))));
            }
            return result;
        }

        public static bool FIND_INFO1(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            //br.ReadBytes(44); //pominięcie nagłówka
            byte tmp;   //zmienna pomocnicza
            byte[] chunkName = new byte[4];
            byte[] chunk;
            string output, info;
            for (int i = 45; i < fs.Length; i++)
            {
                tmp = br.ReadByte();
                output = tmp.ToString("X2");
                if (output == "4C")
                {
                    chunkName[0] = tmp;
                    info = output;
                    for (int j = 0; j < 3; j++) //wczytywanie danych po 4 (po 4 liczby na próbkę)
                    {
                        tmp = br.ReadByte();
                        chunkName[j + 1] = tmp;
                        output = tmp.ToString("X2");
                        info += output;
                    }
                    if (info == "4C495354") //napis LIST zakodowany w ASCII
                    {
                        Console.WriteLine("jest inf0");
                        Console.WriteLine(ASCIIEncoding.ASCII.GetString(chunkName));
                        Int32 chunkSize = br.ReadInt32();   //rozmiar chunku LIST
                        chunk = new byte[chunkSize];
                        Console.WriteLine("Size: " + chunkSize);
                        for (int j = 0; j < chunkSize; j++) //wczytywanie danych po 4 (po 4 liczby na próbkę)
                        {
                            tmp = br.ReadByte();
                            chunk[j] = tmp;
                        }
                        i += chunkSize + 4;
                        Console.WriteLine(ASCIIEncoding.ASCII.GetString(chunk));
                    }
                    i += 3;
                }
                if (output == "69")
                {
                    chunkName[0] = tmp;
                    info = output;
                    for (int j = 0; j < 3; j++)
                    {
                        tmp = br.ReadByte();
                        chunkName[j + 1] = tmp;
                        output = tmp.ToString("X2");
                        info += output;
                    }
                    i += 3;

                    if (info == "69584D4C") //napis aXML zakodowany w ASCII
                    {
                        Console.WriteLine("jest xml");
                        Console.WriteLine(ASCIIEncoding.ASCII.GetString(chunkName));
                        Int32 chunkSize = br.ReadInt32();   //rozmiar chunku LIST
                        chunk = new byte[chunkSize];
                        Console.WriteLine("Size: " + chunkSize);
                        for (int j = 0; j < chunkSize; j++) //wczytywanie danych po 4 (po 4 liczby na próbkę)
                        {
                            tmp = br.ReadByte();
                            chunk[j] = tmp;
                        }
                        i += chunkSize + 4;
                        Console.WriteLine(ASCIIEncoding.ASCII.GetString(chunk));
                        Console.WriteLine("iXML");
                    }
                }


                if (output == "61")
                {
                    chunkName[0] = tmp;
                    info = output;
                    for (int j = 0; j < 3; j++)
                    {
                        tmp = br.ReadByte();
                        chunkName[j + 1] = tmp;
                        output = tmp.ToString("X2");
                        info += output;
                    }
                    i += 3;

                    if (info == "61584D4C") //napis iXML zakodowany w ASCII
                    {
                        Console.WriteLine("jest xml");
                        Console.WriteLine(ASCIIEncoding.ASCII.GetString(chunkName));
                        Int32 chunkSize = br.ReadInt32();   //rozmiar chunku LIST
                        chunk = new byte[chunkSize];
                        Console.WriteLine("Size: " + chunkSize);
                        for (int j = 0; j < chunkSize; j++) //wczytywanie danych po 4 (po 4 liczby na próbkę)
                        {
                            tmp = br.ReadByte();
                            chunk[j] = tmp;
                        }
                        i += chunkSize + 4;
                        Console.WriteLine(ASCIIEncoding.ASCII.GetString(chunk));
                    }
                }


                if (output == "5F")
                {
                    chunkName[0] = tmp;
                    info = output;
                    for (int j = 0; j < 3; j++)
                    {
                        tmp = br.ReadByte();
                        chunkName[j + 1] = tmp;
                        output = tmp.ToString("X2");
                        info += output;
                    }
                    i += 3;

                    if (info == "5F504D58") //napis _PMX zakodowany w ASCII
                    {
                        Console.WriteLine("jest xml");
                        Console.WriteLine(ASCIIEncoding.ASCII.GetString(chunkName));
                        Int32 chunkSize = br.ReadInt32();   //rozmiar chunku LIST
                        chunk = new byte[chunkSize];
                        Console.WriteLine("Size: " + chunkSize);
                        for (int j = 0; j < chunkSize; j++) //wczytywanie danych po 4 (po 4 liczby na próbkę)
                        {
                            tmp = br.ReadByte();
                            chunk[j] = tmp;
                        }
                        i += chunkSize + 4;
                        Console.WriteLine(ASCIIEncoding.ASCII.GetString(chunk));
                    }
                }


            }
            Console.WriteLine("kuniec");
            fs.Close();
            br.Close();
            return true;
        }
    

        public static void FIND_INFO(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            
            byte[] dta;
            uint len;
            char[] type;
            uint loop;
            //fs.Position=44;
            // StreamWriter outputFile = new StreamWriter(Path.Combine("chunki.txt"));
            while (true)
            {
                byte[] id = br.ReadBytes(4);

                


                if (System.Text.Encoding.UTF8.GetString(id) == "LIST")
                {
                    
                    type = System.Text.Encoding.UTF8.GetString(id).ToCharArray();
                    len = br.ReadUInt32();
                    dta = br.ReadBytes(4);
                    infochunk = new INFOChunk(type, len, dta);
                    loop = len;
                    for (int i = 0; i <= (int)loop;i++)
                    {
                        type = br.ReadChars(4);
                        len = br.ReadUInt32();
                        dta = br.ReadBytes((int)len);
                        infochunk.RecognizeChunk(type, len, dta);
                        i += 3 + (int)len + 4;
                    }
                    break;
                    
                    
                    
                    
                }
            }
            
            fs.Close();
            br.Close();
            
        }

        public static void GetChunks(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            br.ReadBytes(44);
            byte[] tmp;
            string info;
            for (int i = 45; i < fs.Length; i++)
            {
                tmp=br.ReadBytes(4);
            }
        }

        public static void GetData(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            HeaderReader.data = br.ReadBytes(Convert.ToInt32(fs.Length) - 44);
            br.Close();
            fs.Close();

        }

        public static void ReadWAV(string path)//odczytywanie pliku
        {
            Complex[] samples = new Complex[2000];//sample
            double[] magnitude = new double[2000];
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            wav.RIFF = new string(br.ReadChars(4));
            wav.ChunkSize = br.ReadInt32();
            wav.WAVE = new string(br.ReadChars(4));
            wav.fmt = new string(br.ReadChars(4));
            wav.Subchunk1_size = br.ReadInt32();
            wav.AudioFormat = br.ReadInt16();
            wav.NumberOfChannels = br.ReadInt16();
            wav.SampleRate = br.ReadInt32();
            wav.ByteRate = br.ReadInt32();
            wav.BlockAlign = br.ReadInt16();
            wav.BitsPerSample = br.ReadInt16();
            wav.data = new string(br.ReadChars(4));
            wav.Subchunk2_size = br.ReadInt32();


            if(wav.NumberOfChannels == 2)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    samples[i] = br.ReadInt16();
                }
            }
           
            br.Close();
            fs.Close();

            

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("samples.dat")))
            {
                for (int i=0; i< samples.Length;i++)
                {
                    outputFile.WriteLine(samples[i].Real + ";" + samples[i].Imaginary);
                }
            }

            samples = DFT_transform(samples);
            magnitude = Magnitude(samples);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("DFT.dat")))
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    outputFile.WriteLine(i+ "\t" + samples[i].Real + "\t" + samples[i].Imaginary);
                }
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("Magnitude.dat")))
            {
                for (int i = 0; i < magnitude.Length; i++)
                {
                    outputFile.WriteLine(i + "\t" + magnitude[i]);

                }
            }

        }
    }
}
