using System;
using System.Text;
namespace ThoughtWorks.QRCode.Codec.Util
{
	public class QRCodeUtility
	{
		public static int sqrt(int val)
		{
			int g = 0;
			int b = 32768;
			int bshft = 15;
			do
			{
				int temp;
				if (val >= (temp = (g << 1) + b << (bshft-- & 31)))
				{
					g += b;
					val -= temp;
				}
			}
			while ((b >>= 1) > 0);
			return g;
		}
		public static bool IsUniCode(string value)
		{
			byte[] ascii = QRCodeUtility.AsciiStringToByteArray(value);
			byte[] unicode = QRCodeUtility.UnicodeStringToByteArray(value);
			string value2 = QRCodeUtility.FromASCIIByteArray(ascii);
			string value3 = QRCodeUtility.FromUnicodeByteArray(unicode);
			return value2 != value3;
		}
		public static bool IsUnicode(byte[] byteData)
		{
			string value = QRCodeUtility.FromASCIIByteArray(byteData);
			string value2 = QRCodeUtility.FromUnicodeByteArray(byteData);
			byte[] ascii = QRCodeUtility.AsciiStringToByteArray(value);
			byte[] unicode = QRCodeUtility.UnicodeStringToByteArray(value2);
			return ascii[0] != unicode[0];
		}
		public static string FromASCIIByteArray(byte[] characters)
		{
			ASCIIEncoding encoding = new ASCIIEncoding();
			return encoding.GetString(characters);
		}
		public static string FromUnicodeByteArray(byte[] characters)
		{
			Encoding encoding = Encoding.GetEncoding("gb2312");
			return encoding.GetString(characters);
		}
		public static byte[] AsciiStringToByteArray(string str)
		{
			ASCIIEncoding encoding = new ASCIIEncoding();
			return encoding.GetBytes(str);
		}
		public static byte[] UnicodeStringToByteArray(string str)
		{
			Encoding encoding = Encoding.GetEncoding("gb2312");
			return encoding.GetBytes(str);
		}
	}
}
