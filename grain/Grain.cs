using System;
using System.Diagnostics;

namespace grain
{
    class Grain
    {
        //TODO: byte or  bool?
        //LFSR
        protected byte[] s = new byte[80];
        //NFSR
        protected byte[] b = new byte[80];

        public void Init(byte[] key, byte[] iv)
        {
            if (key.Length != 10)
            {
                throw new OverflowException("key must be 10 bytes");
            }
            if (iv.Length != 8)
            {
                throw new OverflowException("iv must be 8 bytes");
            }

            for (byte i = 0; i < 10; ++i)
            {
                for (byte j = 0; j < 8; ++j)
                {
                    b[i * 8 + j] = (byte)((key[i] >> j) & 1);
                }
            }

            for (byte i = 0; i < 8; ++i)
            {
                for (byte j = 0; j < 8; ++j)
                {
                    s[i * 8 + j] = (byte)((iv[i] >> j) & 1);
                }
            }

            for (byte i = 8; i < 10; ++i)
            {
                for (byte j = 0; j < 8; ++j)
                {
                    s[i * 8 + j] = 1;
                }
            }

            for (uint i = 0; i < 160; ++i)
            {
                byte outbit = Update();
                //init sp
                s[79] ^= outbit;
                b[79] ^= outbit;
            }
        }

        public byte Update()
        {
            byte sbit = (byte)(s[62] ^ s[51] ^ s[38] ^ s[23] ^ s[13] ^ s[0]);
            Debug.Assert((sbit & 0xFE) == 0, "bit error");

            byte bbit = (byte)(
                s[0] ^ b[62] ^ b[60] ^ b[52] ^ b[45] ^ b[37] ^ b[33] ^ b[28] ^ b[21] ^
                b[14] ^ b[9] ^ b[0] ^ b[63] & b[60] ^ b[37] & b[33] ^ b[15] & b[9] ^
                b[60] & b[52] & b[45] ^ b[33] & b[28] & b[21] ^ b[63] & b[45] & b[28] & b[9] ^
                b[60] & b[52] & b[37] & b[33] ^ b[63] & b[60] & b[21] & b[15] ^
                b[63] & b[60] & b[52] & b[45] & b[37] ^ b[33] & b[28] & b[21] & b[15] & b[9] ^
                b[52] & b[45] & b[37] & b[33] & b[28] & b[21]
                );
            Debug.Assert((bbit & 0xFE) == 0, "bit error");

            byte[] x = new byte[] { s[3], s[25], s[46], s[64], b[63] };
            byte h = (byte)(
                x[1] ^ x[4] ^ x[0] & x[3] ^ x[2] & x[3] ^ x[3] & x[4] ^ x[0] & x[1] & x[2] ^
                x[0] & x[2] & x[3] ^ x[0] & x[2] & x[4] ^ x[1] & x[2] & x[4] ^ x[2] & x[3] & x[4]
                );
            Debug.Assert((h & 0xFE) == 0, "bit error");

            byte output = (byte)(b[1] ^ b[2] ^ b[4] ^ b[10] ^ b[31] ^ b[43] ^ b[56] ^ h);
            Debug.Assert((output & 0xFE) == 0, "bit error");

            //shift
            for (uint i = 1; i < 80; ++i)
            {
                s[i - 1] = s[i];
                b[i - 1] = b[i];
            }
            s[79] = sbit;
            b[79] = bbit;

            return output;
        }

        public byte GetByte()
        {
            byte data = 0;
            for (byte i = 0; i < 8; ++i)
            {
                data |= (byte)(Update() << i);
            }
            return data;
        }

        public byte[] GetBytes(uint byte_length)
        {
            byte[] array = new byte[byte_length];
            for (uint i = 0; i < byte_length; ++i)
            {
                array[i] = GetByte();
            }
            return array;
        }
    }
}
