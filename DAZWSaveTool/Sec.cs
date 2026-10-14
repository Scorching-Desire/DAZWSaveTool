using System.Collections;

namespace DAZWSaveTool
{
    public class Sec
    {
        public static int EncryptSecureInt(int rawValue)
        {
            int mask = -1514914902;
            int[] steps = new int[]
            {
            42, 1, 1, 3, 1, 2, 1, 0, 7, 1, 1, 4, 9, 2, 12, 1,
            0, 12, 15, 2, 8, 4, 16, 13, 3, 4, 10, 25, 24, 29, 22, 18
            };
            // 1. Force Little-Endian Byte Array
            byte[] rawBytes = new byte[4];
            rawBytes[0] = (byte)(rawValue & 0xFF);
            rawBytes[1] = (byte)((rawValue >> 8) & 0xFF);
            rawBytes[2] = (byte)((rawValue >> 16) & 0xFF);
            rawBytes[3] = (byte)((rawValue >> 24) & 0xFF);

            byte[] maskBytes = new byte[4];
            maskBytes[0] = (byte)(mask & 0xFF);
            maskBytes[1] = (byte)((mask >> 8) & 0xFF);
            maskBytes[2] = (byte)((mask >> 16) & 0xFF);
            maskBytes[3] = (byte)((mask >> 24) & 0xFF);

            // 2. Create BitArrays directly from the raw memory bytes
            BitArray bits = new BitArray(rawBytes);
            BitArray maskBits = new BitArray(maskBytes);

            // 3. XOR (System.Collections.BitArray$$Xor)
            bits.Xor(maskBits);

            // 4. Shuffle (SecureInt$$Sh)
            // The ARM64 loop runs backwards. i goes from 31 down to 1.
            for (int i = 31; i >= 1; i--)
            {
                int targetIdx = steps[i];

                if (targetIdx >= 32 || targetIdx < 0) continue;

                if (targetIdx != i)
                {
                    bool temp = bits[targetIdx];
                    bits[targetIdx] = bits[i];
                    bits[i] = temp;
                }
            }

            // 5. Export back to Signed Integer
            byte[] outBytes = new byte[4];
            bits.CopyTo(outBytes, 0);

            // Rebuild the int32
            return outBytes[0] | (outBytes[1] << 8) | (outBytes[2] << 16) | (outBytes[3] << 24);
        }
    }
}
