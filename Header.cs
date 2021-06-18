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
    public class HeaderReader
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
        public static CUEChunk CUE;
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

        public static double[] Phase(Complex[] samples)
        {
            int N = samples.Length;
            double[] result = new double[N];
            for (int i = 0; i < N; i++)
            {
                result[i] = Math.Atan(samples[i].Imaginary / samples[i].Real);
            }
            return result;
        }

        

        public static void FIND_INFO(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            
            byte[] dta;
            uint len;
            char[] type;
            uint loop;
            
            while (fs.Position < fs.Length)
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
                    infochunk.ShowInfoChunk();
                   
                }
                if (System.Text.Encoding.UTF8.GetString(id) == "cue ")
                {
                    Console.WriteLine("znaleziono cue");
                    type = System.Text.Encoding.UTF8.GetString(id).ToCharArray();
                    len = br.ReadUInt32();
                    int numPoints = br.ReadInt32();
                    dta = br.ReadBytes((int)len-4);
                    CUE = new CUEChunk(type, len, dta, numPoints);
                    CUE.ShowData();
                    
                }
                
            }
            
            fs.Close();
            br.Close();
            
        }

       

        public static void GetData(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            br.ReadBytes(44);
            HeaderReader.data = br.ReadBytes(Convert.ToInt32(fs.Length)-44);
            br.Close();
            fs.Close();

        }

       public static void SaveWithoutMetaData(string path, string Filename)
        {
            FileStream f = new FileStream(Filename, FileMode.Create);
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            BinaryWriter bw = new BinaryWriter(f);
            br.ReadBytes(44);
            //wpisanie nagłówka
            bw.Write(new char[4] { 'R', 'I', 'F', 'F' });
            bw.Write((int)HeaderReader.wav.ChunkSize);
            bw.Write(new char[4] { 'W', 'A', 'V', 'E' });
            bw.Write(new char[4] { 'f', 'm', 't', ' ' });
            bw.Write((int)HeaderReader.wav.Subchunk1_size);
            bw.Write((short)HeaderReader.wav.AudioFormat);
            bw.Write((short)HeaderReader.wav.NumberOfChannels);
            bw.Write((int)HeaderReader.wav.SampleRate);
            bw.Write((int)HeaderReader.wav.ByteRate);
            bw.Write((short)HeaderReader.wav.BlockAlign);
            bw.Write((short)HeaderReader.wav.BitsPerSample);
            bw.Write(new char[4] { 'd', 'a', 't', 'a' });
            bw.Write((int)HeaderReader.wav.Subchunk2_size);
            byte[] dta;
            uint len;
            char[] type;
            uint loop;

            while (fs.Position < fs.Length)
            {
                byte[] id = br.ReadBytes(4);


                if (System.Text.Encoding.UTF8.GetString(id) == "LIST")
                {

                    type = System.Text.Encoding.UTF8.GetString(id).ToCharArray();
                    len = br.ReadUInt32();
                    dta = br.ReadBytes(4);
                    infochunk = new INFOChunk(type, len, dta);
                    loop = len;
                    for (int i = 0; i <= (int)loop; i++)
                    {
                        type = br.ReadChars(4);
                        len = br.ReadUInt32();
                        dta = br.ReadBytes((int)len);
                        infochunk.RecognizeChunk(type, len, dta);
                        i += 3 + (int)len + 4;
                    }


                }
                if (System.Text.Encoding.UTF8.GetString(id) == "cue ")
                {
                    
                    type = System.Text.Encoding.UTF8.GetString(id).ToCharArray();
                    len = br.ReadUInt32();
                    int numPoints = br.ReadInt32();
                    dta = br.ReadBytes((int)len - 4);
                    

                }
                else
                    bw.Write(id);

            }
            f.Close();
            bw.Close();
            br.Close();
            fs.Close();
        }

        public static void ReadWAV(string path)//odczytywanie pliku
        {
            Complex[] samples = new Complex[2000];//sample
            double[] magnitude = new double[2000];
            double[] phase = new double[2000];
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
            var nSamples = ((long)wav.Subchunk2_size) / (long)(wav.NumberOfChannels * 8) / (long)(wav.BitsPerSample);//ilość sampli na kanał
            
            
            if (wav.NumberOfChannels == 2 && wav.BitsPerSample == 16)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    samples[i] = br.ReadInt16();
                    br.ReadInt16(); // pominięcie drugiego kanału
                }
            }
           
            br.Close();
            fs.Close();

            

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("samples.dat")))
            {
                for (int i=0; i< samples.Length/2;i++)
                {
                    outputFile.WriteLine(samples[i].Real + ";" + samples[i].Imaginary);
                }
            }

            samples = DFT_transform(samples);
            magnitude = Magnitude(samples);
            phase = Phase(samples);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("DFT.dat")))
            {
                for (int i = 0; i < samples.Length/2; i++)
                {
                    outputFile.WriteLine(i+ "\t" + samples[i].Real + "\t" + samples[i].Imaginary);
                }
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("Magnitude.dat")))
            {
                for (int i = 0; i < magnitude.Length/2; i++)
                {
                    outputFile.WriteLine(i + "\t" + magnitude[i]);

                }
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("Phase.dat")))
            {
                for (int i = 0; i < phase.Length/2; i++)
                {
                    outputFile.WriteLine(i + "\t" + phase[i]);

                }
            }

        }
    }
}
