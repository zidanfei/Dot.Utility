using System;
using System.Drawing;
using System.IO;
using System.Text;
using ThoughtWorks.QRCode.Codec.Util;
using ThoughtWorks.QRCode.Properties;
namespace ThoughtWorks.QRCode.Codec
{
	public class QRCodeEncoder
	{
		public enum ENCODE_MODE
		{
			ALPHA_NUMERIC,
			NUMERIC,
			BYTE
		}
		public enum ERROR_CORRECTION
		{
			L,
			M,
			Q,
			H
		}
		internal QRCodeEncoder.ERROR_CORRECTION qrcodeErrorCorrect;
		internal QRCodeEncoder.ENCODE_MODE qrcodeEncodeMode;
		internal int qrcodeVersion;
		internal int qrcodeStructureappendN;
		internal int qrcodeStructureappendM;
		internal int qrcodeStructureappendParity;
		internal System.Drawing.Color qrCodeBackgroundColor;
		internal System.Drawing.Color qrCodeForegroundColor;
		internal int qrCodeScale;
		internal string qrcodeStructureappendOriginaldata;
		public virtual QRCodeEncoder.ERROR_CORRECTION QRCodeErrorCorrect
		{
			get
			{
				return this.qrcodeErrorCorrect;
			}
			set
			{
				this.qrcodeErrorCorrect = value;
			}
		}
		public virtual int QRCodeVersion
		{
			get
			{
				return this.qrcodeVersion;
			}
			set
			{
				if (value >= 0 && value <= 40)
				{
					this.qrcodeVersion = value;
				}
			}
		}
		public virtual QRCodeEncoder.ENCODE_MODE QRCodeEncodeMode
		{
			get
			{
				return this.qrcodeEncodeMode;
			}
			set
			{
				this.qrcodeEncodeMode = value;
			}
		}
		public virtual int QRCodeScale
		{
			get
			{
				return this.qrCodeScale;
			}
			set
			{
				this.qrCodeScale = value;
			}
		}
		public virtual System.Drawing.Color QRCodeBackgroundColor
		{
			get
			{
				return this.qrCodeBackgroundColor;
			}
			set
			{
				this.qrCodeBackgroundColor = value;
			}
		}
		public virtual System.Drawing.Color QRCodeForegroundColor
		{
			get
			{
				return this.qrCodeForegroundColor;
			}
			set
			{
				this.qrCodeForegroundColor = value;
			}
		}
		public QRCodeEncoder()
		{
			this.qrcodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
			this.qrcodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
			this.qrcodeVersion = 7;
			this.qrcodeStructureappendN = 0;
			this.qrcodeStructureappendM = 0;
			this.qrcodeStructureappendParity = 0;
			this.qrcodeStructureappendOriginaldata = "";
			this.qrCodeScale = 4;
			this.qrCodeBackgroundColor = System.Drawing.Color.White;
			this.qrCodeForegroundColor = System.Drawing.Color.Black;
		}
		public virtual void setStructureappend(int m, int n, int p)
		{
			if (n > 1 && n <= 16 && m > 0 && m <= 16 && p >= 0 && p <= 255)
			{
				this.qrcodeStructureappendM = m;
				this.qrcodeStructureappendN = n;
				this.qrcodeStructureappendParity = p;
			}
		}
		public virtual int calStructureappendParity(sbyte[] originaldata)
		{
			int i = 0;
			int originaldataLength = originaldata.Length;
			int structureappendParity;
			if (originaldataLength > 1)
			{
				structureappendParity = 0;
				while (i < originaldataLength)
				{
					structureappendParity ^= ((int)originaldata[i] & 255);
					i++;
				}
			}
			else
			{
				structureappendParity = -1;
			}
			return structureappendParity;
		}
		public virtual bool[][] calQrcode(byte[] qrcodeData)
		{
			int dataCounter = 0;
			int dataLength = qrcodeData.Length;
			int[] dataValue = new int[dataLength + 32];
			sbyte[] dataBits = new sbyte[dataLength + 32];
			bool[][] result;
			if (dataLength <= 0)
			{
				bool[][] array = new bool[1][];
				bool[][] arg_3B_0 = array;
				int arg_3B_1 = 0;
				bool[] array2 = new bool[1];
				arg_3B_0[arg_3B_1] = array2;
				bool[][] ret = array;
				result = ret;
			}
			else
			{
				if (this.qrcodeStructureappendN > 1)
				{
					dataValue[0] = 3;
					dataBits[0] = 4;
					dataValue[1] = this.qrcodeStructureappendM - 1;
					dataBits[1] = 4;
					dataValue[2] = this.qrcodeStructureappendN - 1;
					dataBits[2] = 4;
					dataValue[3] = this.qrcodeStructureappendParity;
					dataBits[3] = 8;
					dataCounter = 4;
				}
				dataBits[dataCounter] = 4;
				int[] codewordNumPlus;
				int codewordNumCounterValue;
				switch (this.qrcodeEncodeMode)
				{
				case QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC:
					codewordNumPlus = new int[]
					{
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4
					};
					dataValue[dataCounter] = 2;
					dataCounter++;
					dataValue[dataCounter] = dataLength;
					dataBits[dataCounter] = 9;
					codewordNumCounterValue = dataCounter;
					dataCounter++;
					for (int i = 0; i < dataLength; i++)
					{
						char chr = (char)qrcodeData[i];
						sbyte chrValue = 0;
						if (chr >= '0' && chr < ':')
						{
							chrValue = (sbyte)(chr - '0');
						}
						else
						{
							if (chr >= 'A' && chr < '[')
							{
								chrValue = (sbyte)(chr - '7');
							}
							else
							{
								if (chr == ' ')
								{
									chrValue = 36;
								}
								if (chr == '$')
								{
									chrValue = 37;
								}
								if (chr == '%')
								{
									chrValue = 38;
								}
								if (chr == '*')
								{
									chrValue = 39;
								}
								if (chr == '+')
								{
									chrValue = 40;
								}
								if (chr == '-')
								{
									chrValue = 41;
								}
								if (chr == '.')
								{
									chrValue = 42;
								}
								if (chr == '/')
								{
									chrValue = 43;
								}
								if (chr == ':')
								{
									chrValue = 44;
								}
							}
						}
						if (i % 2 == 0)
						{
							dataValue[dataCounter] = (int)chrValue;
							dataBits[dataCounter] = 6;
						}
						else
						{
							dataValue[dataCounter] = dataValue[dataCounter] * 45 + (int)chrValue;
							dataBits[dataCounter] = 11;
							if (i < dataLength - 1)
							{
								dataCounter++;
							}
						}
					}
					dataCounter++;
					break;
				case QRCodeEncoder.ENCODE_MODE.NUMERIC:
					codewordNumPlus = new int[]
					{
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						2,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4,
						4
					};
					dataValue[dataCounter] = 1;
					dataCounter++;
					dataValue[dataCounter] = dataLength;
					dataBits[dataCounter] = 10;
					codewordNumCounterValue = dataCounter;
					dataCounter++;
					for (int i = 0; i < dataLength; i++)
					{
						if (i % 3 == 0)
						{
							dataValue[dataCounter] = (int)(qrcodeData[i] - 48);
							dataBits[dataCounter] = 4;
						}
						else
						{
							dataValue[dataCounter] = dataValue[dataCounter] * 10 + (int)(qrcodeData[i] - 48);
							if (i % 3 == 1)
							{
								dataBits[dataCounter] = 7;
							}
							else
							{
								dataBits[dataCounter] = 10;
								if (i < dataLength - 1)
								{
									dataCounter++;
								}
							}
						}
					}
					dataCounter++;
					break;
				default:
					codewordNumPlus = new int[]
					{
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8,
						8
					};
					dataValue[dataCounter] = 4;
					dataCounter++;
					dataValue[dataCounter] = dataLength;
					dataBits[dataCounter] = 8;
					codewordNumCounterValue = dataCounter;
					dataCounter++;
					for (int i = 0; i < dataLength; i++)
					{
						dataValue[i + dataCounter] = (int)(qrcodeData[i] & 255);
						dataBits[i + dataCounter] = 8;
					}
					dataCounter += dataLength;
					break;
				}
				int totalDataBits = 0;
				for (int i = 0; i < dataCounter; i++)
				{
					totalDataBits += (int)dataBits[i];
				}
				int ec;
				switch (this.qrcodeErrorCorrect)
				{
				case QRCodeEncoder.ERROR_CORRECTION.L:
					ec = 1;
					goto IL_3D0;
				case QRCodeEncoder.ERROR_CORRECTION.Q:
					ec = 3;
					goto IL_3D0;
				case QRCodeEncoder.ERROR_CORRECTION.H:
					ec = 2;
					goto IL_3D0;
				}
				ec = 0;
				IL_3D0:
				int[][] maxDataBitsArray = new int[][]
				{
					new int[]
					{
						0,
						128,
						224,
						352,
						512,
						688,
						864,
						992,
						1232,
						1456,
						1728,
						2032,
						2320,
						2672,
						2920,
						3320,
						3624,
						4056,
						4504,
						5016,
						5352,
						5712,
						6256,
						6880,
						7312,
						8000,
						8496,
						9024,
						9544,
						10136,
						10984,
						11640,
						12328,
						13048,
						13800,
						14496,
						15312,
						15936,
						16816,
						17728,
						18672
					},
					new int[]
					{
						0,
						152,
						272,
						440,
						640,
						864,
						1088,
						1248,
						1552,
						1856,
						2192,
						2592,
						2960,
						3424,
						3688,
						4184,
						4712,
						5176,
						5768,
						6360,
						6888,
						7456,
						8048,
						8752,
						9392,
						10208,
						10960,
						11744,
						12248,
						13048,
						13880,
						14744,
						15640,
						16568,
						17528,
						18448,
						19472,
						20528,
						21616,
						22496,
						23648
					},
					new int[]
					{
						0,
						72,
						128,
						208,
						288,
						368,
						480,
						528,
						688,
						800,
						976,
						1120,
						1264,
						1440,
						1576,
						1784,
						2024,
						2264,
						2504,
						2728,
						3080,
						3248,
						3536,
						3712,
						4112,
						4304,
						4768,
						5024,
						5288,
						5608,
						5960,
						6344,
						6760,
						7208,
						7688,
						7888,
						8432,
						8768,
						9136,
						9776,
						10208
					},
					new int[]
					{
						0,
						104,
						176,
						272,
						384,
						496,
						608,
						704,
						880,
						1056,
						1232,
						1440,
						1648,
						1952,
						2088,
						2360,
						2600,
						2936,
						3176,
						3560,
						3880,
						4096,
						4544,
						4912,
						5312,
						5744,
						6032,
						6464,
						6968,
						7288,
						7880,
						8264,
						8920,
						9368,
						9848,
						10288,
						10832,
						11408,
						12016,
						12656,
						13328
					}
				};
				int maxDataBits = 0;
				if (this.qrcodeVersion == 0)
				{
					this.qrcodeVersion = 1;
					for (int i = 1; i <= 40; i++)
					{
						if (maxDataBitsArray[ec][i] >= totalDataBits + codewordNumPlus[this.qrcodeVersion])
						{
							maxDataBits = maxDataBitsArray[ec][i];
							break;
						}
						this.qrcodeVersion++;
					}
				}
				else
				{
					maxDataBits = maxDataBitsArray[ec][this.qrcodeVersion];
				}
				totalDataBits += codewordNumPlus[this.qrcodeVersion];
				dataBits[codewordNumCounterValue] = (sbyte)((int)dataBits[codewordNumCounterValue] + codewordNumPlus[this.qrcodeVersion]);
				int[] maxCodewordsArray = new int[]
				{
					0,
					26,
					44,
					70,
					100,
					134,
					172,
					196,
					242,
					292,
					346,
					404,
					466,
					532,
					581,
					655,
					733,
					815,
					901,
					991,
					1085,
					1156,
					1258,
					1364,
					1474,
					1588,
					1706,
					1828,
					1921,
					2051,
					2185,
					2323,
					2465,
					2611,
					2761,
					2876,
					3034,
					3196,
					3362,
					3532,
					3706
				};
				int maxCodewords = maxCodewordsArray[this.qrcodeVersion];
				int maxModules1side = 17 + (this.qrcodeVersion << 2);
				int[] matrixRemainBit = new int[]
				{
					0,
					0,
					7,
					7,
					7,
					7,
					7,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					3,
					3,
					3,
					3,
					3,
					3,
					3,
					4,
					4,
					4,
					4,
					4,
					4,
					4,
					3,
					3,
					3,
					3,
					3,
					3,
					3,
					0,
					0,
					0,
					0,
					0,
					0
				};
				int byte_num = matrixRemainBit[this.qrcodeVersion] + (maxCodewords << 3);
				sbyte[] matrixX = new sbyte[byte_num];
				sbyte[] matrixY = new sbyte[byte_num];
				sbyte[] maskArray = new sbyte[byte_num];
				sbyte[] formatInformationX2 = new sbyte[15];
				sbyte[] formatInformationY2 = new sbyte[15];
				sbyte[] rsEccCodewords = new sbyte[1];
				sbyte[] rsBlockOrderTemp = new sbyte[128];
				try
				{
					string fileName = "qrv" + Convert.ToString(this.qrcodeVersion) + "_" + Convert.ToString(ec);
					MemoryStream memoryStream = new MemoryStream((byte[])Resources.ResourceManager.GetObject(fileName));
					BufferedStream bis = new BufferedStream(memoryStream);
					SystemUtils.ReadInput(bis, matrixX, 0, matrixX.Length);
					SystemUtils.ReadInput(bis, matrixY, 0, matrixY.Length);
					SystemUtils.ReadInput(bis, maskArray, 0, maskArray.Length);
					SystemUtils.ReadInput(bis, formatInformationX2, 0, formatInformationX2.Length);
					SystemUtils.ReadInput(bis, formatInformationY2, 0, formatInformationY2.Length);
					SystemUtils.ReadInput(bis, rsEccCodewords, 0, rsEccCodewords.Length);
					SystemUtils.ReadInput(bis, rsBlockOrderTemp, 0, rsBlockOrderTemp.Length);
					bis.Close();
					memoryStream.Close();
				}
				catch (Exception e)
				{
					SystemUtils.WriteStackTrace(e, Console.Error);
				}
				sbyte rsBlockOrderLength = 1;
				sbyte j = 1;
				while ((int)j < 128)
				{
					if (rsBlockOrderTemp[(int)j] == 0)
					{
						rsBlockOrderLength = j;
						break;
					}
					j += 1;
				}
				sbyte[] rsBlockOrder = new sbyte[(int)rsBlockOrderLength];
				Array.Copy(rsBlockOrderTemp, 0, rsBlockOrder, 0, (int)((byte)rsBlockOrderLength));
				sbyte[] formatInformationX3 = new sbyte[]
				{
					0,
					1,
					2,
					3,
					4,
					5,
					7,
					8,
					8,
					8,
					8,
					8,
					8,
					8,
					8
				};
				sbyte[] formatInformationY3 = new sbyte[]
				{
					8,
					8,
					8,
					8,
					8,
					8,
					8,
					8,
					7,
					5,
					4,
					3,
					2,
					1,
					0
				};
				int maxDataCodewords = maxDataBits >> 3;
				int modules1Side = 4 * this.qrcodeVersion + 17;
				int matrixTotalBits = modules1Side * modules1Side;
				sbyte[] frameData = new sbyte[matrixTotalBits + modules1Side];
				try
				{
					string fileName = "qrvfr" + Convert.ToString(this.qrcodeVersion);
					MemoryStream memoryStream = new MemoryStream((byte[])Resources.ResourceManager.GetObject(fileName));
					BufferedStream bis = new BufferedStream(memoryStream);
					SystemUtils.ReadInput(bis, frameData, 0, frameData.Length);
					bis.Close();
					memoryStream.Close();
				}
				catch (Exception e)
				{
					SystemUtils.WriteStackTrace(e, Console.Error);
				}
				if (totalDataBits <= maxDataBits - 4)
				{
					dataValue[dataCounter] = 0;
					dataBits[dataCounter] = 4;
				}
				else
				{
					if (totalDataBits < maxDataBits)
					{
						dataValue[dataCounter] = 0;
						dataBits[dataCounter] = (sbyte)(maxDataBits - totalDataBits);
					}
					else
					{
						if (totalDataBits > maxDataBits)
						{
							Console.Out.WriteLine("overflow");
						}
					}
				}
				sbyte[] dataCodewords = QRCodeEncoder.divideDataBy8Bits(dataValue, dataBits, maxDataCodewords);
				sbyte[] codewords = QRCodeEncoder.calculateRSECC(dataCodewords, rsEccCodewords[0], rsBlockOrder, maxDataCodewords, maxCodewords);
				sbyte[][] matrixContent = new sbyte[modules1Side][];
				for (int i2 = 0; i2 < modules1Side; i2++)
				{
					matrixContent[i2] = new sbyte[modules1Side];
				}
				for (int i = 0; i < modules1Side; i++)
				{
					for (int k = 0; k < modules1Side; k++)
					{
						matrixContent[k][i] = 0;
					}
				}
				for (int i = 0; i < maxCodewords; i++)
				{
					sbyte codeword_i = codewords[i];
					for (int k = 7; k >= 0; k--)
					{
						int codewordBitsNumber = i * 8 + k;
						matrixContent[(int)matrixX[codewordBitsNumber] & 255][(int)matrixY[codewordBitsNumber] & 255] = (sbyte)(255 * (int)(codeword_i & 1) ^ (int)maskArray[codewordBitsNumber]);
						codeword_i = (sbyte)SystemUtils.URShift((int)codeword_i & 255, 1);
					}
				}
				for (int matrixRemain = matrixRemainBit[this.qrcodeVersion]; matrixRemain > 0; matrixRemain--)
				{
					int remainBitTemp = matrixRemain + maxCodewords * 8 - 1;
					matrixContent[(int)matrixX[remainBitTemp] & 255][(int)matrixY[remainBitTemp] & 255] = (sbyte)(255 ^ (int)maskArray[remainBitTemp]);
				}
				sbyte maskNumber = QRCodeEncoder.selectMask(matrixContent, matrixRemainBit[this.qrcodeVersion] + maxCodewords * 8);
				sbyte maskContent = (sbyte)(1 << (int)maskNumber);
				sbyte formatInformationValue = (sbyte)(ec << 3 | (int)maskNumber);
				string[] formatInformationArray = new string[]
				{
					"101010000010010",
					"101000100100101",
					"101111001111100",
					"101101101001011",
					"100010111111001",
					"100000011001110",
					"100111110010111",
					"100101010100000",
					"111011111000100",
					"111001011110011",
					"111110110101010",
					"111100010011101",
					"110011000101111",
					"110001100011000",
					"110110001000001",
					"110100101110110",
					"001011010001001",
					"001001110111110",
					"001110011100111",
					"001100111010000",
					"000011101100010",
					"000001001010101",
					"000110100001100",
					"000100000111011",
					"011010101011111",
					"011000001101000",
					"011111100110001",
					"011101000000110",
					"010010010110100",
					"010000110000011",
					"010111011011010",
					"010101111101101"
				};
				for (int i = 0; i < 15; i++)
				{
					sbyte content = sbyte.Parse(formatInformationArray[(int)formatInformationValue].Substring(i, i + 1 - i));
					matrixContent[(int)formatInformationX3[i] & 255][(int)formatInformationY3[i] & 255] = (sbyte)((int)content * 255);
					matrixContent[(int)formatInformationX2[i] & 255][(int)formatInformationY2[i] & 255] = (sbyte)((int)content * 255);
				}
				bool[][] out_Renamed = new bool[modules1Side][];
				for (int i3 = 0; i3 < modules1Side; i3++)
				{
					out_Renamed[i3] = new bool[modules1Side];
				}
				int c = 0;
				for (int i = 0; i < modules1Side; i++)
				{
					for (int k = 0; k < modules1Side; k++)
					{
						if ((matrixContent[k][i] & maskContent) != 0 || frameData[c] == 49)
						{
							out_Renamed[k][i] = true;
						}
						else
						{
							out_Renamed[k][i] = false;
						}
						c++;
					}
					c++;
				}
				result = out_Renamed;
			}
			return result;
		}
		private static sbyte[] divideDataBy8Bits(int[] data, sbyte[] bits, int maxDataCodewords)
		{
			int l = bits.Length;
			int codewordsCounter = 0;
			int remainingBits = 8;
			int max = 0;
			if (l != data.Length)
			{
			}
			for (int i = 0; i < l; i++)
			{
				max += (int)bits[i];
			}
			int l2 = (max - 1) / 8 + 1;
			sbyte[] codewords = new sbyte[maxDataCodewords];
			for (int i = 0; i < l2; i++)
			{
				codewords[i] = 0;
			}
			for (int i = 0; i < l; i++)
			{
				int buffer = data[i];
				int bufferBits = (int)bits[i];
				bool flag = true;
				if (bufferBits == 0)
				{
					break;
				}
				while (flag)
				{
					if (remainingBits > bufferBits)
					{
						codewords[codewordsCounter] = (sbyte)((int)codewords[codewordsCounter] << bufferBits | buffer);
						remainingBits -= bufferBits;
						flag = false;
					}
					else
					{
						bufferBits -= remainingBits;
						codewords[codewordsCounter] = (sbyte)((int)codewords[codewordsCounter] << remainingBits | buffer >> bufferBits);
						if (bufferBits == 0)
						{
							flag = false;
						}
						else
						{
							buffer &= (1 << bufferBits) - 1;
							flag = true;
						}
						codewordsCounter++;
						remainingBits = 8;
					}
				}
			}
			if (remainingBits != 8)
			{
				codewords[codewordsCounter] = (sbyte)(codewords[codewordsCounter] << remainingBits);
			}
			else
			{
				codewordsCounter--;
			}
			if (codewordsCounter < maxDataCodewords - 1)
			{
				bool flag = true;
				while (codewordsCounter < maxDataCodewords - 1)
				{
					codewordsCounter++;
					if (flag)
					{
						codewords[codewordsCounter] = -20;
					}
					else
					{
						codewords[codewordsCounter] = 17;
					}
					flag = !flag;
				}
			}
			return codewords;
		}
		private static sbyte[] calculateRSECC(sbyte[] codewords, sbyte rsEccCodewords, sbyte[] rsBlockOrder, int maxDataCodewords, int maxCodewords)
		{
			sbyte[][] rsCalTableArray = new sbyte[256][];
			for (int i = 0; i < 256; i++)
			{
				rsCalTableArray[i] = new sbyte[(int)rsEccCodewords];
			}
			try
			{
				string fileName = "rsc" + rsEccCodewords.ToString();
				MemoryStream memoryStream = new MemoryStream((byte[])Resources.ResourceManager.GetObject(fileName));
				BufferedStream bis = new BufferedStream(memoryStream);
				for (int i = 0; i < 256; i++)
				{
					SystemUtils.ReadInput(bis, rsCalTableArray[i], 0, rsCalTableArray[i].Length);
				}
				bis.Close();
				memoryStream.Close();
			}
			catch (Exception e)
			{
				SystemUtils.WriteStackTrace(e, Console.Error);
			}
			int j = 0;
			int rsBlockNumber = 0;
			sbyte[][] rsTemp = new sbyte[rsBlockOrder.Length][];
			sbyte[] res = new sbyte[maxCodewords];
			Array.Copy(codewords, 0, res, 0, codewords.Length);
			for (int i2 = 0; i2 < rsBlockOrder.Length; i2++)
			{
				rsTemp[i2] = new sbyte[((int)rsBlockOrder[i2] & 255) - (int)rsEccCodewords];
			}
			for (int i2 = 0; i2 < maxDataCodewords; i2++)
			{
				rsTemp[rsBlockNumber][j] = codewords[i2];
				j++;
				if (j >= ((int)rsBlockOrder[rsBlockNumber] & 255) - (int)rsEccCodewords)
				{
					j = 0;
					rsBlockNumber++;
				}
			}
			for (rsBlockNumber = 0; rsBlockNumber < rsBlockOrder.Length; rsBlockNumber++)
			{
				sbyte[] rsTempData = new sbyte[rsTemp[rsBlockNumber].Length];
				rsTemp[rsBlockNumber].CopyTo(rsTempData, 0);
				int rsCodewords = (int)rsBlockOrder[rsBlockNumber] & 255;
				int rsDataCodewords = rsCodewords - (int)rsEccCodewords;
				for (j = rsDataCodewords; j > 0; j--)
				{
					sbyte first = rsTempData[0];
					if (first != 0)
					{
						sbyte[] leftChr = new sbyte[rsTempData.Length - 1];
						Array.Copy(rsTempData, 1, leftChr, 0, rsTempData.Length - 1);
						sbyte[] cal = rsCalTableArray[(int)first & 255];
						rsTempData = QRCodeEncoder.calculateByteArrayBits(leftChr, cal, "xor");
					}
					else
					{
						if ((int)rsEccCodewords < rsTempData.Length)
						{
							sbyte[] rsTempNew = new sbyte[rsTempData.Length - 1];
							Array.Copy(rsTempData, 1, rsTempNew, 0, rsTempData.Length - 1);
							rsTempData = new sbyte[rsTempNew.Length];
							rsTempNew.CopyTo(rsTempData, 0);
						}
						else
						{
							sbyte[] rsTempNew = new sbyte[(int)rsEccCodewords];
							Array.Copy(rsTempData, 1, rsTempNew, 0, rsTempData.Length - 1);
							rsTempNew[(int)(rsEccCodewords - 1)] = 0;
							rsTempData = new sbyte[rsTempNew.Length];
							rsTempNew.CopyTo(rsTempData, 0);
						}
					}
				}
				Array.Copy(rsTempData, 0, res, codewords.Length + rsBlockNumber * (int)rsEccCodewords, (int)((byte)rsEccCodewords));
			}
			return res;
		}
		private static sbyte[] calculateByteArrayBits(sbyte[] xa, sbyte[] xb, string ind)
		{
			sbyte[] xl;
			sbyte[] xs;
			if (xa.Length > xb.Length)
			{
				xl = new sbyte[xa.Length];
				xa.CopyTo(xl, 0);
				xs = new sbyte[xb.Length];
				xb.CopyTo(xs, 0);
			}
			else
			{
				xl = new sbyte[xb.Length];
				xb.CopyTo(xl, 0);
				xs = new sbyte[xa.Length];
				xa.CopyTo(xs, 0);
			}
			int ll = xl.Length;
			int ls = xs.Length;
			sbyte[] res = new sbyte[ll];
			for (int i = 0; i < ll; i++)
			{
				if (i < ls)
				{
					if (ind == "xor")
					{
						res[i] = (sbyte)(xl[i] ^ xs[i]);
					}
					else
					{
						res[i] = (sbyte)(xl[i] | xs[i]);
					}
				}
				else
				{
					res[i] = xl[i];
				}
			}
			return res;
		}
		private static sbyte selectMask(sbyte[][] matrixContent, int maxCodewordsBitWithRemain)
		{
			int i = matrixContent.Length;
			int[] array = new int[8];
			int[] d = array;
			array = new int[8];
			int[] d2 = array;
			array = new int[8];
			int[] d3 = array;
			array = new int[8];
			int[] d4 = array;
			int d2And = 0;
			int d2Or = 0;
			array = new int[8];
			int[] d4Counter = array;
			for (int y = 0; y < i; y++)
			{
				array = new int[8];
				int[] xData = array;
				array = new int[8];
				int[] yData = array;
				bool[] array2 = new bool[8];
				bool[] xD1Flag = array2;
				array2 = new bool[8];
				bool[] yD1Flag = array2;
				for (int x = 0; x < i; x++)
				{
					if (x > 0 && y > 0)
					{
						d2And = ((int)(matrixContent[x][y] & matrixContent[x - 1][y] & matrixContent[x][y - 1] & matrixContent[x - 1][y - 1]) & 255);
						d2Or = (((int)matrixContent[x][y] & 255) | ((int)matrixContent[x - 1][y] & 255) | ((int)matrixContent[x][y - 1] & 255) | ((int)matrixContent[x - 1][y - 1] & 255));
					}
					for (int maskNumber = 0; maskNumber < 8; maskNumber++)
					{
						xData[maskNumber] = ((xData[maskNumber] & 63) << 1 | (SystemUtils.URShift((int)matrixContent[x][y] & 255, maskNumber) & 1));
						yData[maskNumber] = ((yData[maskNumber] & 63) << 1 | (SystemUtils.URShift((int)matrixContent[y][x] & 255, maskNumber) & 1));
						if (((int)matrixContent[x][y] & 1 << maskNumber) != 0)
						{
							d4Counter[maskNumber]++;
						}
						if (xData[maskNumber] == 93)
						{
							d3[maskNumber] += 40;
						}
						if (yData[maskNumber] == 93)
						{
							d3[maskNumber] += 40;
						}
						if (x > 0 && y > 0)
						{
							if ((d2And & 1) != 0 || (d2Or & 1) == 0)
							{
								d2[maskNumber] += 3;
							}
							d2And >>= 1;
							d2Or >>= 1;
						}
						if ((xData[maskNumber] & 31) == 0 || (xData[maskNumber] & 31) == 31)
						{
							if (x > 3)
							{
								if (xD1Flag[maskNumber])
								{
									d[maskNumber]++;
								}
								else
								{
									d[maskNumber] += 3;
									xD1Flag[maskNumber] = true;
								}
							}
						}
						else
						{
							xD1Flag[maskNumber] = false;
						}
						if ((yData[maskNumber] & 31) == 0 || (yData[maskNumber] & 31) == 31)
						{
							if (x > 3)
							{
								if (yD1Flag[maskNumber])
								{
									d[maskNumber]++;
								}
								else
								{
									d[maskNumber] += 3;
									yD1Flag[maskNumber] = true;
								}
							}
						}
						else
						{
							yD1Flag[maskNumber] = false;
						}
					}
				}
			}
			int minValue = 0;
			sbyte res = 0;
			int[] d4Value = new int[]
			{
				90,
				80,
				70,
				60,
				50,
				40,
				30,
				20,
				10,
				0,
				0,
				10,
				20,
				30,
				40,
				50,
				60,
				70,
				80,
				90,
				90
			};
			for (int maskNumber = 0; maskNumber < 8; maskNumber++)
			{
				d4[maskNumber] = d4Value[20 * d4Counter[maskNumber] / maxCodewordsBitWithRemain];
				int demerit = d[maskNumber] + d2[maskNumber] + d3[maskNumber] + d4[maskNumber];
				if (demerit < minValue || maskNumber == 0)
				{
					res = (sbyte)maskNumber;
					minValue = demerit;
				}
			}
			return res;
		}
		public virtual Bitmap Encode(string content, Encoding encoding)
		{
			bool[][] matrix = this.calQrcode(encoding.GetBytes(content));
			SolidBrush brush = new SolidBrush(this.qrCodeBackgroundColor);
			Bitmap image = new Bitmap(matrix.Length * this.qrCodeScale + 1, matrix.Length * this.qrCodeScale + 1);
			Graphics g = Graphics.FromImage(image);
			g.FillRectangle(brush, new Rectangle(0, 0, image.Width, image.Height));
			brush.Color = this.qrCodeForegroundColor;
			for (int i = 0; i < matrix.Length; i++)
			{
				for (int j = 0; j < matrix.Length; j++)
				{
					if (matrix[j][i])
					{
						g.FillRectangle(brush, j * this.qrCodeScale, i * this.qrCodeScale, this.qrCodeScale, this.qrCodeScale);
					}
				}
			}
			return image;
		}
		public virtual Bitmap Encode(string content)
		{
			Bitmap result;
			if (QRCodeUtility.IsUniCode(content))
			{
				result = this.Encode(content, Encoding.GetEncoding("gb2312"));
			}
			else
			{
				result = this.Encode(content, Encoding.ASCII);
			}
			return result;
		}
	}
}
