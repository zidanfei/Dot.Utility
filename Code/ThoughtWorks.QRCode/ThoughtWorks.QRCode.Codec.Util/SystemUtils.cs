using System;
using System.IO;
using System.Text;
namespace ThoughtWorks.QRCode.Codec.Util
{
	public class SystemUtils
	{
		public static int ReadInput(Stream sourceStream, sbyte[] target, int start, int count)
		{
			int result;
			if (target.Length == 0)
			{
				result = 0;
			}
			else
			{
				byte[] receiver = new byte[target.Length];
				int bytesRead = sourceStream.Read(receiver, start, count);
				if (bytesRead == 0)
				{
					result = -1;
				}
				else
				{
					for (int i = start; i < start + bytesRead; i++)
					{
						target[i] = (sbyte)receiver[i];
					}
					result = bytesRead;
				}
			}
			return result;
		}
		public static int ReadInput(TextReader sourceTextReader, short[] target, int start, int count)
		{
			int result;
			if (target.Length == 0)
			{
				result = 0;
			}
			else
			{
				char[] charArray = new char[target.Length];
				int bytesRead = sourceTextReader.Read(charArray, start, count);
				if (bytesRead == 0)
				{
					result = -1;
				}
				else
				{
					for (int index = start; index < start + bytesRead; index++)
					{
						target[index] = (short)charArray[index];
					}
					result = bytesRead;
				}
			}
			return result;
		}
		public static void WriteStackTrace(Exception throwable, TextWriter stream)
		{
			stream.Write(throwable.StackTrace);
			stream.Flush();
		}
		public static int URShift(int number, int bits)
		{
			int result;
			if (number >= 0)
			{
				result = number >> bits;
			}
			else
			{
				result = (number >> bits) + (2 << ~bits);
			}
			return result;
		}
		public static int URShift(int number, long bits)
		{
			return SystemUtils.URShift(number, (int)bits);
		}
		public static long URShift(long number, int bits)
		{
			long result;
			if (number >= 0L)
			{
				result = number >> bits;
			}
			else
			{
				result = (number >> bits) + (2L << ~bits);
			}
			return result;
		}
		public static long URShift(long number, long bits)
		{
			return SystemUtils.URShift(number, (int)bits);
		}
		public static byte[] ToByteArray(sbyte[] sbyteArray)
		{
			byte[] byteArray = null;
			if (sbyteArray != null)
			{
				byteArray = new byte[sbyteArray.Length];
				for (int index = 0; index < sbyteArray.Length; index++)
				{
					byteArray[index] = (byte)sbyteArray[index];
				}
			}
			return byteArray;
		}
		public static byte[] ToByteArray(string sourceString)
		{
			return Encoding.UTF8.GetBytes(sourceString);
		}
		public static byte[] ToByteArray(object[] tempObjectArray)
		{
			byte[] byteArray = null;
			if (tempObjectArray != null)
			{
				byteArray = new byte[tempObjectArray.Length];
				for (int index = 0; index < tempObjectArray.Length; index++)
				{
					byteArray[index] = (byte)tempObjectArray[index];
				}
			}
			return byteArray;
		}
		public static sbyte[] ToSByteArray(byte[] byteArray)
		{
			sbyte[] sbyteArray = null;
			if (byteArray != null)
			{
				sbyteArray = new sbyte[byteArray.Length];
				for (int index = 0; index < byteArray.Length; index++)
				{
					sbyteArray[index] = (sbyte)byteArray[index];
				}
			}
			return sbyteArray;
		}
		public static char[] ToCharArray(sbyte[] sByteArray)
		{
			return Encoding.UTF8.GetChars(SystemUtils.ToByteArray(sByteArray));
		}
		public static char[] ToCharArray(byte[] byteArray)
		{
			return Encoding.UTF8.GetChars(byteArray);
		}
	}
}
