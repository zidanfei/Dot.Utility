namespace Dot.Utility.Cryptography
{
    public class Base64Decoder
    {
        // Fields
        private int blockCount;
        private int length;
        private int length2;
        private int length3;
        private int paddingCount;
        private char[] source;

        /// <summary>
        /// 设置源数据
        /// </summary>
        public char[] Source
        {
            set
            {
                this.source = value;
                int num = 0;
                this.length = source.Length;

                for (int i = 0; i < 2; i++)
                    if (source[(this.length - i) - 1] == '=')
                        num++;

                this.paddingCount = num;
                this.blockCount = this.length / 4;
                this.length2 = this.blockCount * 3;
            }
        }

        public Base64Decoder()
        { 
            
        }

        /// <summary>
        /// 初始化Base64解码器
        /// </summary>
        /// <param name="input">Base64文本</param>
        public Base64Decoder(char[] input)
        {
            this.Source = input;
        }

        private byte char2sixbit(char c)
        {
            char[] chArray = new char[] { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
            'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
         };
            if (c != '=')
            {
                for (int i = 0; i < 0x40; i++)
                {
                    if (chArray[i] == c)
                    {
                        return (byte)i;
                    }
                }
            }
            return 0;
        }


        /// <summary>
        /// 对已输入的数据进行解码
        /// </summary>
        /// <returns>返回的数据流</returns>
        public byte[] GetDecoded()
        {
            byte[] buffer = new byte[this.length];
            byte[] buffer2 = new byte[this.length2];
            for (int i = 0; i < this.length; i++)
            {
                buffer[i] = this.char2sixbit(this.source[i]);
            }
            for (int j = 0; j < this.blockCount; j++)
            {
                byte n6 = buffer[j * 4];
                byte n8 = buffer[(j * 4) + 2];
                byte n7 = buffer[(j * 4) + 1];
                byte n9 = buffer[(j * 4) + 3];
                byte n3 = (byte)((n7 & 0x30) >> 4);
                byte n2 = (byte)(n6 << 2);
                n3 = (byte)(n3 + n2);
                n2 = (byte)((n7 & 15) << 4);
                byte n4 = (byte)((n8 & 60) >> 2);
                n4 = (byte)(n4 + n2);
                n2 = (byte)((n8 & 3) << 6);
                byte n5 = n9;
                n5 = (byte)(n5 + n2);
                buffer2[j * 3] = n3;
                buffer2[(j * 3) + 1] = n4;
                buffer2[(j * 3) + 2] = n5;
            }
            this.length3 = this.length2 - this.paddingCount;
            byte[] buffer3 = new byte[this.length3];
            for (int k = 0; k < this.length3; k++)
            {
                buffer3[k] = buffer2[k];
            }
            return buffer3;
        }
    }
}
 
