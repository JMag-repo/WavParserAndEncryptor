using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Header
{
    public static class RSA
    {
        private static BigInteger p;
        private static BigInteger q;

        public static Key publicKey;
        public static Key privateKey;

        public static void KeyGenerator(int p, int q)
        {
            RSA.p = p;
            RSA.q = q;

            BigInteger euler = (p - 1) * (q - 1);
            BigInteger n = p * q;
            BigInteger e = Create_E(n, euler);
            BigInteger d = ModInverse(e, euler);
            publicKey = new Key(e, n);
            privateKey = new Key(d, n);
            Console.WriteLine("p= " + p);
            Console.WriteLine("q= " + q);
            Console.WriteLine("n= " + n);
            Console.WriteLine("e= " + e);
            Console.WriteLine("d= " + d);


        }

        private static Boolean isSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }

        public static BigInteger Sqrt(this BigInteger n)
        {
            if (n == 0) return 0;
            if (n > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!isSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }

        private static bool IsPrime(BigInteger a)
        {
            if (a % 2 == 0)
                return (a == 2);
            for (int i = 3; i <= Sqrt(a); i += 2)
            {
                if (a % i == 0)
                {
                    return false;
                }
            }
            return true;
        }



        public static BigInteger EuklidesNWD(BigInteger a, BigInteger b)
        {
            BigInteger tmp;
            while (b != 0)
            {
                tmp = b;
                b = a % b;
                a = tmp;
            }
            return a;
        }

        private static BigInteger Create_E(BigInteger mod, BigInteger euler)
        {
            BigInteger e, tmp;
            int eul = (int)euler;
            do
            {
                tmp = new Random().Next(3, eul);
            } while (!IsPrime(tmp));

            for (e = tmp; EuklidesNWD(e, mod) != 1; e -= 2) ;
            return e;
        }

        public static BigInteger ModInverse(BigInteger a, BigInteger b)
        {
            BigInteger i = b, v = 0, d = 1;
            while (a != 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= b;
            if (v < 0)
            {
                v = (v + b) % b;
            }
            return v;

        }

        public static BigInteger PowModulo(BigInteger value, BigInteger b, BigInteger n)
        {
            BigInteger i;
            BigInteger result = 1;
            BigInteger x = value % n;

            for (i = 1; i <= b; i <<= 1)
            {
                x %= n;
                if ((b & i) != 0)
                {
                    result *= x;
                    result %= n;
                }
                x *= x;
            }
            return result;

        }

        public static byte[] EncryptData(byte[] data)
        {
            List<byte> result = new List<byte>();
            List<byte> DataSample = new List<byte>();
            BigInteger DataToEncrypt;
            int DataLen = publicKey.n.ToByteArray().Length - 1;
            int KeyLen = publicKey.n.ToByteArray().Length;


            foreach (byte DataByte in data)
            {
                DataSample.Add(DataByte);
                if (DataSample.Count == DataLen)
                {
                    DataSample.Add(0);//czytam jako dodatnie
                    DataToEncrypt = new BigInteger(DataSample.ToArray());

                    byte[] encrypted = BigInteger.ModPow(DataToEncrypt, publicKey.e, publicKey.n).ToByteArray();
                    Array.Resize<byte>(ref encrypted, KeyLen);
                    result.AddRange(encrypted);
                    DataSample.Clear();
                }
            }

            if (DataSample.Count > 0)
            {
                DataToEncrypt = new BigInteger(DataSample.ToArray());
                byte[] encrypted = BigInteger.ModPow(DataToEncrypt, publicKey.e, publicKey.n).ToByteArray();
                Array.Resize<byte>(ref encrypted, KeyLen);
                result.AddRange(encrypted);
                DataSample.Clear();
            }



            Console.WriteLine("Po zaszyfrowaniu bajty: " + result.ToArray().Length);
            return result.ToArray();
        }



        public static byte[] DecryptData(byte[] data)
        {
            List<byte> result = new List<byte>();
            List<byte> DataSample = new List<byte>();
            BigInteger DataToEncrypt;
            int DataLen = privateKey.n.ToByteArray().Length - 1;
            int KeyLen = privateKey.n.ToByteArray().Length;

            foreach (byte DataByte in data)
            {

                DataSample.Add(DataByte);
                if (DataSample.Count == KeyLen)
                {
                    DataToEncrypt = new BigInteger(DataSample.ToArray());

                    byte[] decrypted = BigInteger.ModPow(DataToEncrypt, privateKey.e, privateKey.n).ToByteArray();

                    Array.Resize<byte>(ref decrypted, DataLen);
                    result.AddRange(decrypted);
                    DataSample.Clear();


                }
            }

            Console.WriteLine("Po odszyfrowaniu bajty: " + result.ToArray().Length);
            return result.ToArray();
        }


        public static void EncryptTest(byte[] data)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var enc1=EncryptData(data);
            sw.Stop();
            Console.WriteLine("Czas naszego algorytmu przy enkrypcji:" + sw.Elapsed);
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            sw.Reset();
            sw.Start();
            var enc2 =RSA.Encrypt(data, false);
            sw.Stop();
            Console.WriteLine("Czas wbudowanego algorytmu przy enkrypcji:" + sw.Elapsed);
            sw.Reset();
            sw.Start();
            EncryptData(enc1);
            sw.Stop();
            Console.WriteLine("Czas naszego algorytmu przy enkrypcji:" + sw.Elapsed);
            sw.Reset();
            sw.Start();
             RSA.Decrypt(enc2, false);
            sw.Stop();
            Console.WriteLine("Czas wbudowanego algorytmu przy dekrypcji:" + sw.Elapsed);

        }   



    }


    public struct Key
    {
        public BigInteger e { get; private set; }
        public BigInteger n { get; private set; }
        public Key(BigInteger e, BigInteger n)
        {
            this.e = e;
            this.n = n;
        }
    }
}
