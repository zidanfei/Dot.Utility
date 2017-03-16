using System;
using System.IO;
using ThoughtWorks.QRCode.Codec.Util;
using ThoughtWorks.QRCode.ExceptionHandler;
namespace ThoughtWorks.QRCode.Codec.Reader
{
	public class QRCodeDataBlockReader
	{
		private const int MODE_NUMBER = 1;
		private const int MODE_ROMAN_AND_NUMBER = 2;
		private const int MODE_8BIT_BYTE = 4;
		private const int MODE_KANJI = 8;
		internal int[] blocks;
		internal int dataLengthMode;
		internal int blockPointer;
		internal int bitPointer;
		internal int dataLength;
		internal int numErrorCorrectionCode;
		internal DebugCanvas canvas;
		private int[][] sizeOfDataLengthInfo = new int[][]
		{
			new int[]
			{
				10,
				9,
				8,
				8
			},
			new int[]
			{
				12,
				11,
				16,
				10
			},
			new int[]
			{
				14,
				13,
				16,
				12
			}
		};
		internal virtual int NextMode
		{
			get
			{
				int result;
				if (this.blockPointer > this.blocks.Length - this.numErrorCorrectionCode - 2)
				{
					result = 0;
				}
				else
				{
					result = this.getNextBits(4);
				}
				return result;
			}
		}
		public virtual sbyte[] DataByte
		{
			get
			{
				this.canvas.println("Reading data blocks.");
				MemoryStream output = new MemoryStream();
				try
				{
					int mode;
					while (true)
					{
						mode = this.NextMode;
						if (mode == 0)
						{
							break;
						}
						if (mode != 1 && mode != 2 && mode != 4 && mode != 8)
						{
							goto Block_7;
						}
						this.dataLength = this.getDataLength(mode);
						if (this.dataLength < 1)
						{
							goto Block_8;
						}
						int num = mode;
						switch (num)
						{
						case 1:
						{
							sbyte[] temp_sbyteArray = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(this.getFigureString(this.dataLength)));
							output.Write(SystemUtils.ToByteArray(temp_sbyteArray), 0, temp_sbyteArray.Length);
							break;
						}
						case 2:
						{
							sbyte[] temp_sbyteArray2 = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(this.getRomanAndFigureString(this.dataLength)));
							output.Write(SystemUtils.ToByteArray(temp_sbyteArray2), 0, temp_sbyteArray2.Length);
							break;
						}
						case 3:
							break;
						case 4:
						{
							sbyte[] temp_sbyteArray3 = this.get8bitByteArray(this.dataLength);
							output.Write(SystemUtils.ToByteArray(temp_sbyteArray3), 0, temp_sbyteArray3.Length);
							break;
						}
						default:
							if (num == 8)
							{
								sbyte[] temp_sbyteArray4 = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(this.getKanjiString(this.dataLength)));
								output.Write(SystemUtils.ToByteArray(temp_sbyteArray4), 0, temp_sbyteArray4.Length);
							}
							break;
						}
					}
					if (output.Length > 0L)
					{
						goto IL_24B;
					}
					throw new InvalidDataBlockException("Empty data block");
					Block_7:
					throw new InvalidDataBlockException(string.Concat(new object[]
					{
						"Invalid mode: ",
						mode,
						" in (block:",
						this.blockPointer,
						" bit:",
						this.bitPointer,
						")"
					}));
					Block_8:
					throw new InvalidDataBlockException("Invalid data length: " + this.dataLength);
				}
				catch (IndexOutOfRangeException e)
				{
					SystemUtils.WriteStackTrace(e, Console.Error);
					throw new InvalidDataBlockException(string.Concat(new object[]
					{
						"Data Block Error in (block:",
						this.blockPointer,
						" bit:",
						this.bitPointer,
						")"
					}));
				}
				catch (IOException e2)
				{
					throw new InvalidDataBlockException(e2.Message);
				}
				IL_24B:
				return SystemUtils.ToSByteArray(output.ToArray());
			}
		}
		public virtual string DataString
		{
			get
			{
				this.canvas.println("Reading data blocks...");
				string dataString = "";
				while (true)
				{
					int mode = this.NextMode;
					this.canvas.println("mode: " + mode);
					if (mode == 0)
					{
						break;
					}
					if (mode != 1 && mode != 2 && mode != 4 && mode != 8)
					{
					}
					this.dataLength = this.getDataLength(mode);
					this.canvas.println(Convert.ToString(this.blocks[this.blockPointer]));
					Console.Out.WriteLine("length: " + this.dataLength);
					int num = mode;
					switch (num)
					{
					case 1:
						dataString += this.getFigureString(this.dataLength);
						break;
					case 2:
						dataString += this.getRomanAndFigureString(this.dataLength);
						break;
					case 3:
						break;
					case 4:
						dataString += this.get8bitByteString(this.dataLength);
						break;
					default:
						if (num == 8)
						{
							dataString += this.getKanjiString(this.dataLength);
						}
						break;
					}
				}
				Console.Out.WriteLine("");
				return dataString;
			}
		}
		public QRCodeDataBlockReader(int[] blocks, int version, int numErrorCorrectionCode)
		{
			this.blockPointer = 0;
			this.bitPointer = 7;
			this.dataLength = 0;
			this.blocks = blocks;
			this.numErrorCorrectionCode = numErrorCorrectionCode;
			if (version <= 9)
			{
				this.dataLengthMode = 0;
			}
			else
			{
				if (version >= 10 && version <= 26)
				{
					this.dataLengthMode = 1;
				}
				else
				{
					if (version >= 27 && version <= 40)
					{
						this.dataLengthMode = 2;
					}
				}
			}
			this.canvas = QRCodeDecoder.Canvas;
		}
		internal virtual int getNextBits(int numBits)
		{
			int result;
			if (numBits < this.bitPointer + 1)
			{
				int mask = 0;
				for (int i = 0; i < numBits; i++)
				{
					mask += 1 << i;
				}
				mask <<= this.bitPointer - numBits + 1;
				int bits = (this.blocks[this.blockPointer] & mask) >> this.bitPointer - numBits + 1;
				this.bitPointer -= numBits;
				result = bits;
			}
			else
			{
				if (numBits < this.bitPointer + 1 + 8)
				{
					int mask2 = 0;
					for (int i = 0; i < this.bitPointer + 1; i++)
					{
						mask2 += 1 << i;
					}
					int bits = (this.blocks[this.blockPointer] & mask2) << numBits - (this.bitPointer + 1);
					this.blockPointer++;
					bits += this.blocks[this.blockPointer] >> 8 - (numBits - (this.bitPointer + 1));
					this.bitPointer -= numBits % 8;
					if (this.bitPointer < 0)
					{
						this.bitPointer = 8 + this.bitPointer;
					}
					result = bits;
				}
				else
				{
					if (numBits < this.bitPointer + 1 + 16)
					{
						int mask2 = 0;
						int mask3 = 0;
						for (int i = 0; i < this.bitPointer + 1; i++)
						{
							mask2 += 1 << i;
						}
						int bitsFirstBlock = (this.blocks[this.blockPointer] & mask2) << numBits - (this.bitPointer + 1);
						this.blockPointer++;
						int bitsSecondBlock = this.blocks[this.blockPointer] << numBits - (this.bitPointer + 1 + 8);
						this.blockPointer++;
						for (int i = 0; i < numBits - (this.bitPointer + 1 + 8); i++)
						{
							mask3 += 1 << i;
						}
						mask3 <<= 8 - (numBits - (this.bitPointer + 1 + 8));
						int bitsThirdBlock = (this.blocks[this.blockPointer] & mask3) >> 8 - (numBits - (this.bitPointer + 1 + 8));
						int bits = bitsFirstBlock + bitsSecondBlock + bitsThirdBlock;
						this.bitPointer -= (numBits - 8) % 8;
						if (this.bitPointer < 0)
						{
							this.bitPointer = 8 + this.bitPointer;
						}
						result = bits;
					}
					else
					{
						Console.Out.WriteLine("ERROR!");
						result = 0;
					}
				}
			}
			return result;
		}
		internal virtual int guessMode(int mode)
		{
			int result;
			switch (mode)
			{
			case 3:
				result = 1;
				return result;
			case 5:
				result = 4;
				return result;
			case 6:
				result = 4;
				return result;
			case 7:
				result = 4;
				return result;
			case 9:
				result = 8;
				return result;
			case 10:
				result = 8;
				return result;
			case 11:
				result = 8;
				return result;
			case 12:
				result = 4;
				return result;
			case 13:
				result = 4;
				return result;
			case 14:
				result = 4;
				return result;
			case 15:
				result = 4;
				return result;
			}
			result = 8;
			return result;
		}
		internal virtual int getDataLength(int modeIndicator)
		{
			int index = 0;
			while (modeIndicator >> index != 1)
			{
				index++;
			}
			return this.getNextBits(this.sizeOfDataLengthInfo[this.dataLengthMode][index]);
		}
		internal virtual string getFigureString(int dataLength)
		{
			int length = dataLength;
			int intData = 0;
			string strData = "";
			do
			{
				if (length >= 3)
				{
					intData = this.getNextBits(10);
					if (intData < 100)
					{
						strData += "0";
					}
					if (intData < 10)
					{
						strData += "0";
					}
					length -= 3;
				}
				else
				{
					if (length == 2)
					{
						intData = this.getNextBits(7);
						if (intData < 10)
						{
							strData += "0";
						}
						length -= 2;
					}
					else
					{
						if (length == 1)
						{
							intData = this.getNextBits(4);
							length--;
						}
					}
				}
				strData += Convert.ToString(intData);
			}
			while (length > 0);
			return strData;
		}
		internal virtual string getRomanAndFigureString(int dataLength)
		{
			int length = dataLength;
			string strData = "";
			char[] tableRomanAndFigure = new char[]
			{
				'0',
				'1',
				'2',
				'3',
				'4',
				'5',
				'6',
				'7',
				'8',
				'9',
				'A',
				'B',
				'C',
				'D',
				'E',
				'F',
				'G',
				'H',
				'I',
				'J',
				'K',
				'L',
				'M',
				'N',
				'O',
				'P',
				'Q',
				'R',
				'S',
				'T',
				'U',
				'V',
				'W',
				'X',
				'Y',
				'Z',
				' ',
				'$',
				'%',
				'*',
				'+',
				'-',
				'.',
				'/',
				':'
			};
			do
			{
				if (length > 1)
				{
					int intData = this.getNextBits(11);
					int firstLetter = intData / 45;
					int secondLetter = intData % 45;
					strData += Convert.ToString(tableRomanAndFigure[firstLetter]);
					strData += Convert.ToString(tableRomanAndFigure[secondLetter]);
					length -= 2;
				}
				else
				{
					if (length == 1)
					{
						int intData = this.getNextBits(6);
						strData += Convert.ToString(tableRomanAndFigure[intData]);
						length--;
					}
				}
			}
			while (length > 0);
			return strData;
		}
		public virtual sbyte[] get8bitByteArray(int dataLength)
		{
			int length = dataLength;
			MemoryStream output = new MemoryStream();
			do
			{
				this.canvas.println("Length: " + length);
				int intData = this.getNextBits(8);
				output.WriteByte((byte)intData);
				length--;
			}
			while (length > 0);
			return SystemUtils.ToSByteArray(output.ToArray());
		}
		internal virtual string get8bitByteString(int dataLength)
		{
			int length = dataLength;
			string strData = "";
			do
			{
				int intData = this.getNextBits(8);
				strData += (char)intData;
				length--;
			}
			while (length > 0);
			return strData;
		}
		internal virtual string getKanjiString(int dataLength)
		{
			int length = dataLength;
			string unicodeString = "";
			do
			{
				int intData = this.getNextBits(13);
				int lowerByte = intData % 192;
				int higherByte = intData / 192;
				int tempWord = (higherByte << 8) + lowerByte;
				int shiftjisWord;
				if (tempWord + 33088 <= 40956)
				{
					shiftjisWord = tempWord + 33088;
				}
				else
				{
					shiftjisWord = tempWord + 49472;
				}
				unicodeString += new string(SystemUtils.ToCharArray(SystemUtils.ToByteArray(new sbyte[]
				{
					(sbyte)(shiftjisWord >> 8),
					(sbyte)(shiftjisWord & 255)
				})));
				length--;
			}
			while (length > 0);
			return unicodeString;
		}
	}
}
