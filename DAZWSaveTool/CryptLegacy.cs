using System.Text;
using ICSharpCode.SharpZipLib.GZip;

namespace DAZWSaveTool
{
    // Pulled from 2.6.2 .dll, legacy encryption method
    public enum CryptMode
    {
        // Token: 0x0400261E RID: 9758
        Decrypt,
        // Token: 0x0400261F RID: 9759
        Crypt
    }

    public class CryptStream : Stream
    {
        // Token: 0x06003B9D RID: 15261 RVA: 0x00101C14 File Offset: 0x00100014
        public CryptStream(Stream stream, CryptMode mode, ulong xorKey)
        {
            this.baseStream = stream;
            this.mode = mode;
            this.xorKey[0] = (byte)(xorKey & 255UL);
            this.xorKey[1] = (byte)((xorKey & 65280UL) >> 8);
            this.xorKey[2] = (byte)((xorKey & 16711680UL) >> 16);
            this.xorKey[3] = (byte)((xorKey & unchecked((ulong)(-16777216))) >> 24);
            this.baseStreamLength = this.baseStream.Length;
        }

        // Token: 0x17000B08 RID: 2824
        // (get) Token: 0x06003B9E RID: 15262 RVA: 0x00101C9E File Offset: 0x0010009E
        public override bool CanRead
        {
            get
            {
                return this.mode == CryptMode.Decrypt && this.baseStream.CanRead;
            }
        }

        // Token: 0x17000B09 RID: 2825
        // (get) Token: 0x06003B9F RID: 15263 RVA: 0x00101CB9 File Offset: 0x001000B9
        public override bool CanSeek
        {
            get
            {
                return this.baseStream.CanSeek;
            }
        }

        // Token: 0x17000B0A RID: 2826
        // (get) Token: 0x06003BA0 RID: 15264 RVA: 0x00101CC6 File Offset: 0x001000C6
        public override bool CanWrite
        {
            get
            {
                return this.mode == CryptMode.Crypt && this.baseStream.CanWrite;
            }
        }

        // Token: 0x17000B0B RID: 2827
        // (get) Token: 0x06003BA1 RID: 15265 RVA: 0x00101CE2 File Offset: 0x001000E2
        public override long Length
        {
            get
            {
                return this.baseStream.Length;
            }
        }

        // Token: 0x17000B0C RID: 2828
        // (get) Token: 0x06003BA2 RID: 15266 RVA: 0x00101CEF File Offset: 0x001000EF
        // (set) Token: 0x06003BA3 RID: 15267 RVA: 0x00101CFC File Offset: 0x001000FC
        public override long Position
        {
            get
            {
                return this.baseStream.Position;
            }
            set
            {
                this.baseStream.Position = value;
            }
        }

        // Token: 0x06003BA4 RID: 15268 RVA: 0x00101D0A File Offset: 0x0010010A
        public override void Flush()
        {
            this.baseStream.Flush();
        }

        // Token: 0x06003BA5 RID: 15269 RVA: 0x00101D17 File Offset: 0x00100117
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.baseStream.Seek(offset, origin);
        }

        // Token: 0x06003BA6 RID: 15270 RVA: 0x00101D26 File Offset: 0x00100126
        public override void SetLength(long value)
        {
            this.baseStream.SetLength(value);
        }

        // Token: 0x06003BA7 RID: 15271 RVA: 0x00101D34 File Offset: 0x00100134
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.Decrypt(buffer, offset, count);
        }

        // Token: 0x06003BA8 RID: 15272 RVA: 0x00101D4C File Offset: 0x0010014C
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.Crypt(buffer, offset, count);
        }

        // Token: 0x06003BA9 RID: 15273 RVA: 0x00101D58 File Offset: 0x00100158
        private int Crypt(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                long position = this.Position;
                byte b = buffer[offset + i];
                byte b2 = (byte)((long)(b + 200) + position);
                b2 = this.XorByte(b2, position);
                this.baseStream.WriteByte(b2);
            }
            return count;
        }

        // Token: 0x06003BAA RID: 15274 RVA: 0x00101DA8 File Offset: 0x001001A8
        private int Decrypt(byte[] buffer, int offset, int count)
        {
            if (this.baseStreamLength - this.Position < (long)count)
            {
                count = (int)(this.baseStreamLength - this.Position);
            }
            for (int i = 0; i < count; i++)
            {
                long position = this.Position;
                int num = this.baseStream.ReadByte();
                if (num == -1)
                {
                    return i;
                }
                byte b = (byte)num;
                byte b2 = this.XorByte(b, position);
                b2 = (byte)((long)(b2 - 200) - position);
                buffer[offset + i] = b2;
            }
            return count;
        }

        // Token: 0x06003BAB RID: 15275 RVA: 0x00101E2C File Offset: 0x0010022C
        private byte XorByte(byte sourceByte, long position)
        {
            byte b = this.xorKey[(int)(checked((IntPtr)(position % 4L)))];
            return (byte)(sourceByte ^ b);
        }

        // Token: 0x04002620 RID: 9760
        private Stream baseStream;

        // Token: 0x04002621 RID: 9761
        private CryptMode mode;

        // Token: 0x04002622 RID: 9762
        private byte[] xorKey = new byte[4];

        // Token: 0x04002623 RID: 9763
        private long baseStreamLength;
    }

    public static class CryptUtils
    {
        // Token: 0x06003BAC RID: 15276 RVA: 0x00101E4C File Offset: 0x0010024C
        public static string EncryptString(string stringToEncrypt, ulong xorKey)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(stringToEncrypt);
            string text2;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Stream stream = new CryptStream(memoryStream, CryptMode.Crypt, xorKey))
                {
                    using (GZipOutputStream gzipOutputStream = new GZipOutputStream(stream))
                    {
                        using (MemoryStream memoryStream2 = new MemoryStream(bytes))
                        {
                            gzipOutputStream.IsStreamOwner = false;
                            gzipOutputStream.SetLevel(9);
                            memoryStream2.WriteTo(gzipOutputStream);
                        }
                    }
                }
                string text = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                text2 = text;
            }
            return text2;
        }

        // Token: 0x06003BAD RID: 15277 RVA: 0x00101F34 File Offset: 0x00100334
        public static string DecryptString(string stringToDecrypt, ulong xorKey)
        {
            byte[] array = Convert.FromBase64String(stringToDecrypt);
            string text2;
            using (MemoryStream memoryStream = new MemoryStream(array))
            {
                using (Stream stream = new CryptStream(memoryStream, CryptMode.Decrypt, xorKey))
                {
                    using (GZipInputStream gzipInputStream = new GZipInputStream(stream))
                    {
                        using (StreamReader streamReader = new StreamReader(gzipInputStream, Encoding.UTF8))
                        {
                            string text = streamReader.ReadToEnd();
                            text2 = text;
                        }
                    }
                }
            }
            return text2;
        }
    }
}
