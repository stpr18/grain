using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grain
{
    class Program
    {
        static void PrintData(byte[] key, byte[] iv, byte[] stream)
        {
            Console.WriteLine("Key : ");
            for (byte i = 0; i < 10; ++i)
                Console.Write("{0:X}", key[i]);
            Console.WriteLine("");

            Console.WriteLine("IV : ");
            for (byte i = 0; i < 8; ++i)
                Console.Write("{0:X}", iv[i]);
            Console.WriteLine("");

            Console.WriteLine("Stream : ");
            for (byte i = 0; i < stream.Length; ++i)
                Console.Write("{0:X}", stream[i]);
            Console.WriteLine("");
        }

        static void Main(string[] args)
        {
            Grain grain = new Grain();
            byte[] key = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef, 0x12, 0x34 };
            byte[] iv = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef };
            grain.Init(key, iv);
            byte[] array = grain.GetBytes(10);
            PrintData(key, iv, array);
            Console.ReadKey();
        }
    }
}
