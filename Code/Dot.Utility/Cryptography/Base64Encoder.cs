namespace Dot.Utility.Cryptography
{

    public class Base64Encoder
    {
        // Fields
        private int blockCount;
        private int length;
        private int length2;
        private int paddingCount;
        private byte[] source;

        /// <summary>
        /// 源数据
        /// </summary>
        public byte[] Source
        {
            set
            {
                this.source = value;
                this.length = source.Length;
                if ((this.length % 3) == 0)
                {
                    this.paddingCount = 0;
                    this.blockCount = this.length / 3;
                }
                else
                {
                    this.paddingCount = 3 - (this.length % 3);
                    this.blockCount = (this.length + this.paddingCount) / 3;
                }
                this.length2 = this.length + this.paddingCount;
            }
        }

        public Base64Encoder()
        { 
        
        }


        /// <summary>
        /// 初始化Base64编码器
        /// </summary>
        /// <param name="input">要进行编码的数据流</param>
        public Base64Encoder(byte[] input)
        {
            this.Source = input;            
        }


        /// <summary>
        /// 获取编码后的Base64字符串
        /// </summary>
        /// <returns>编码后的Base64字符</returns>
        public char[] GetEncoded()
        {
            byte[] buffer = new byte[this.length2];
            for (int i = 0; i < this.length2; i++)
            {
                if (i < this.length)
                {
                    buffer[i] = this.source[i];
                }
                else
                {
                    buffer[i] = 0;
                }
            }
            byte[] buffer2 = new byte[this.blockCount * 4];
            char[] chArray = new char[this.blockCount * 4];
            for (int j = 0; j < this.blockCount; j++)
            {
                byte num2 = buffer[j * 3];
                byte num3 = buffer[(j * 3) + 1];
                byte num4 = buffer[(j * 3) + 2];
                byte num6 = (byte)((num2 & 0xfc) >> 2);
                byte num5 = (byte)((num2 & 3) << 4);
                byte num7 = (byte)((num3 & 240) >> 4);
                num7 = (byte)(num7 + num5);
                num5 = (byte)((num3 & 15) << 2);
                byte num8 = (byte)((num4 & 0xc0) >> 6);
                num8 = (byte)(num8 + num5);
                byte num9 = (byte)(num4 & 0x3f);
                buffer2[j * 4] = num6;
                buffer2[(j * 4) + 1] = num7;
                buffer2[(j * 4) + 2] = num8;
                buffer2[(j * 4) + 3] = num9;
            }
            for (int k = 0; k < (this.blockCount * 4); k++)
            {
                chArray[k] = this.sixbit2char(buffer2[k]);
            }
            switch (this.paddingCount)
            {
                case 0:
                    return chArray;

                case 1:
                    chArray[(this.blockCount * 4) - 1] = '=';
                    return chArray;

                case 2:
                    chArray[(this.blockCount * 4) - 1] = '=';
                    chArray[(this.blockCount * 4) - 2] = '=';
                    return chArray;
            }
            return chArray;
        }

        private char sixbit2char(byte b)
        {
            char[] chArray = new char[] { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
            'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
         };
            if ((b >= 0) && (b <= 0x3f))
            {
                return chArray[b];
            }
            return ' ';
        }
    }

}