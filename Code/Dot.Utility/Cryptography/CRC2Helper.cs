using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Cryptography
{
    public class CRC2Helper
    {
        public static int GetKey(byte[] data)
        {
            int count = data.Length;
            byte[] buf = new byte[data.Length + 2];
            data.CopyTo(buf, 0);
            int ptr = 0;
            int i = 0;
            int crc = 0;
            byte crc1, crc2, crc3;
            crc1 = buf[ptr++];
            crc2 = buf[ptr++];
            buf[count] = 0;
            buf[count + 1] = 0;
            while (--count >= 0)
            {
                crc3 = buf[ptr++];
                for (i = 0; i < 8; i++)
                {
                    if (((crc1 & 0x80) >> 7) == 1)//判断crc1高位是否为1
                    {
                        crc1 = (byte)(crc1 << 1); //移出高位
                        if (((crc2 & 0x80) >> 7) == 1)//判断crc2高位是否为1
                        {
                            crc1 = (byte)(crc1 | 0x01);//crc1低位由0变1
                        }
                        crc2 = (byte)(crc2 << 1);//crc2移出高位
                        if (((crc3 & 0x80) >> 7) == 1) //判断crc3高位是否为1
                        {
                            crc2 = (byte)(crc2 | 0x01); //crc2低位由0变1
                        }
                        crc3 = (byte)(crc3 << 1);//crc3移出高位
                        crc1 = (byte)(crc1 ^ 0x10);
                        crc2 = (byte)(crc2 ^ 0x21);
                    }
                    else
                    {
                        crc1 = (byte)(crc1 << 1); //移出高位
                        if (((crc2 & 0x80) >> 7) == 1)//判断crc2高位是否为1
                        {
                            crc1 = (byte)(crc1 | 0x01);//crc1低位由0变1
                        }
                        crc2 = (byte)(crc2 << 1);//crc2移出高位
                        if (((crc3 & 0x80) >> 7) == 1) //判断crc3高位是否为1
                        {
                            crc2 = (byte)(crc2 | 0x01); //crc2低位由0变1
                        }
                        crc3 = (byte)(crc3 << 1);//crc3移出高位
                    }
                }
            }
            crc = (int)((crc1 << 8) + crc2);
            return crc;
        }

    }
}
