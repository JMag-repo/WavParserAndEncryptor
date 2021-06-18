using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace Header
{
    class Program
    {
        static void Main(string[] args)
        {
            RSA.KeyGenerator(18583, 20113);
            string path = @"C:\Users\magdz\source\repos\Emedia-zad1\plik1.wav";
            HeaderReader.ReadWAV(path);
            HeaderReader.PrintHeader();
            HeaderReader.GetData(path);

            byte[] enc = RSA.EncryptData(HeaderReader.data);
            byte[] dec = RSA.DecryptData(enc);
            FileStream f = new FileStream("moj.wav", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(f);
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


            bw.Write(enc);
            f.Close();
            bw.Close();




            //GNUPlot(1);
            //GNUPlot(2);
            //GNUPlot(4);

            Console.Read();

        }

       

        private static void SaveToFile()
        {
            byte[] enc = RSA.EncryptData(HeaderReader.data);
            byte[] dec = RSA.DecryptData(enc);
            FileStream f = new FileStream("moj.wav", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(f);
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


            bw.Write(dec);
            f.Close();
            bw.Close(); 
        }

        private void testTime()
        {
            byte[] test = new byte[100];
            Buffer.BlockCopy(HeaderReader.data, 0, test, 0, 100);
            RSA.EncryptTest(test);
        }
        private static void GNUPlot(int option)
        {
            string Pgm = @"C:\Users\magdz\source\repos\Emedia-zad1\gnuplot\bin\gnuplot.exe";
            if (!File.Exists(Pgm))
            {
                Console.WriteLine("Brak zainstalowanego gnuplota");
                return;
            }
            Process extPro = new Process();
            extPro.StartInfo.FileName = Pgm;
            extPro.StartInfo.UseShellExecute = false;
            extPro.StartInfo.RedirectStandardInput = true;
            extPro.Start();

            StreamWriter gnupStWr = extPro.StandardInput;
            string path = @Directory.GetCurrentDirectory();
            path = path.Replace('\\', '/');
            string dft = path + "/DFT.dat";
            string magnitude = path + "/Magnitude.dat";
            string phase = path + "/Phase.dat";
            switch (option)
            {
                case 1:
                    {
                        gnupStWr.WriteLine("plot \"" + dft + "\" using 1:2 title 'REAL' with lines");
                        break;
                    }
                case 2:
                    {
                        gnupStWr.WriteLine("plot \"" + phase + "\" using 1:2 title 'Imag' with lines");
                        break;
                    }
                case 3:
                    {
                        gnupStWr.WriteLine("plot sin(x)");
                        break;
                    }
                case 4:
                    {
                        gnupStWr.WriteLine("plot \"" + magnitude + "\" using 1:2 title 'Magnitude' with lines");
                        break;
                    }




            }
            gnupStWr.Flush();
        }

    }
}
