using System;
using System.Collections;
using System.Text;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode.Codec.Ecc;
using ThoughtWorks.QRCode.Codec.Reader;
using ThoughtWorks.QRCode.Codec.Util;
using ThoughtWorks.QRCode.ExceptionHandler;
using ThoughtWorks.QRCode.Geom;
namespace ThoughtWorks.QRCode.Codec
{
	public class QRCodeDecoder
	{
		internal class DecodeResult
		{
			internal int numCorrections;
			internal bool correctionSucceeded;
			internal sbyte[] decodedBytes;
			private QRCodeDecoder enclosingInstance;
			public virtual sbyte[] DecodedBytes
			{
				get
				{
					return this.decodedBytes;
				}
			}
			public virtual int NumErrors
			{
				get
				{
					return this.numCorrections;
				}
			}
			public virtual bool CorrectionSucceeded
			{
				get
				{
					return this.correctionSucceeded;
				}
			}
			public QRCodeDecoder Enclosing_Instance
			{
				get
				{
					return this.enclosingInstance;
				}
			}
			public DecodeResult(QRCodeDecoder enclosingInstance, sbyte[] decodedBytes, int numErrors, bool correctionSucceeded)
			{
				this.InitBlock(enclosingInstance);
				this.decodedBytes = decodedBytes;
				this.numCorrections = numErrors;
				this.correctionSucceeded = correctionSucceeded;
			}
			private void InitBlock(QRCodeDecoder enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
		}
		internal QRCodeSymbol qrCodeSymbol;
		internal int numTryDecode;
		internal ArrayList results;
		internal ArrayList lastResults = ArrayList.Synchronized(new ArrayList(10));
		internal static DebugCanvas canvas;
		internal QRCodeImageReader imageReader;
		internal int numLastCorrections;
		internal bool correctionSucceeded;
		public static DebugCanvas Canvas
		{
			get
			{
				return QRCodeDecoder.canvas;
			}
			set
			{
				QRCodeDecoder.canvas = value;
			}
		}
		internal virtual Point[] AdjustPoints
		{
			get
			{
				ArrayList adjustPoints = ArrayList.Synchronized(new ArrayList(10));
				for (int d = 0; d < 4; d++)
				{
					adjustPoints.Add(new Point(1, 1));
				}
				int lastX = 0;
				int lastY = 0;
				for (int y = 0; y > -4; y--)
				{
					for (int x = 0; x > -4; x--)
					{
						if (x != y && (x + y) % 2 == 0)
						{
							adjustPoints.Add(new Point(x - lastX, y - lastY));
							lastX = x;
							lastY = y;
						}
					}
				}
				Point[] adjusts = new Point[adjustPoints.Count];
				for (int i = 0; i < adjusts.Length; i++)
				{
					adjusts[i] = (Point)adjustPoints[i];
				}
				return adjusts;
			}
		}
		public QRCodeDecoder()
		{
			this.numTryDecode = 0;
			this.results = ArrayList.Synchronized(new ArrayList(10));
			QRCodeDecoder.canvas = new DebugCanvasAdapter();
		}
		public virtual sbyte[] decodeBytes(QRCodeImage qrCodeImage)
		{
			Point[] adjusts = this.AdjustPoints;
			ArrayList results = ArrayList.Synchronized(new ArrayList(10));
			sbyte[] decodedBytes;
			while (this.numTryDecode < adjusts.Length)
			{
				try
				{
					QRCodeDecoder.DecodeResult result = this.decode(qrCodeImage, adjusts[this.numTryDecode]);
					if (result.CorrectionSucceeded)
					{
						decodedBytes = result.DecodedBytes;
						return decodedBytes;
					}
					results.Add(result);
					QRCodeDecoder.canvas.println("Decoding succeeded but could not correct");
					QRCodeDecoder.canvas.println("all errors. Retrying..");
				}
				catch (DecodingFailedException dfe)
				{
					if (dfe.Message.IndexOf("Finder Pattern") >= 0)
					{
						throw dfe;
					}
				}
				finally
				{
					this.numTryDecode++;
				}
			}
			if (results.Count == 0)
			{
				throw new DecodingFailedException("Give up decoding");
			}
			int lowestErrorIndex = -1;
			int lowestError = 2147483647;
			for (int i = 0; i < results.Count; i++)
			{
				QRCodeDecoder.DecodeResult result = (QRCodeDecoder.DecodeResult)results[i];
				if (result.NumErrors < lowestError)
				{
					lowestError = result.NumErrors;
					lowestErrorIndex = i;
				}
			}
			QRCodeDecoder.canvas.println("All trials need for correct error");
			QRCodeDecoder.canvas.println("Reporting #" + lowestErrorIndex + " that,");
			QRCodeDecoder.canvas.println("corrected minimum errors (" + lowestError + ")");
			QRCodeDecoder.canvas.println("Decoding finished.");
			decodedBytes = ((QRCodeDecoder.DecodeResult)results[lowestErrorIndex]).DecodedBytes;
			return decodedBytes;
		}
		public virtual string decode(QRCodeImage qrCodeImage, Encoding encoding)
		{
			sbyte[] data = this.decodeBytes(qrCodeImage);
			byte[] byteData = new byte[data.Length];
			Buffer.BlockCopy(data, 0, byteData, 0, byteData.Length);
			return encoding.GetString(byteData);
		}
		public virtual string decode(QRCodeImage qrCodeImage)
		{
			sbyte[] data = this.decodeBytes(qrCodeImage);
			byte[] byteData = new byte[data.Length];
			Buffer.BlockCopy(data, 0, byteData, 0, byteData.Length);
			Encoding encoding = Encoding.GetEncoding("gb2312");
			return encoding.GetString(byteData);
		}
		internal virtual QRCodeDecoder.DecodeResult decode(QRCodeImage qrCodeImage, Point adjust)
		{
			try
			{
				if (this.numTryDecode == 0)
				{
					QRCodeDecoder.canvas.println("Decoding started");
					int[][] intImage = this.imageToIntArray(qrCodeImage);
					this.imageReader = new QRCodeImageReader();
					this.qrCodeSymbol = this.imageReader.getQRCodeSymbol(intImage);
				}
				else
				{
					QRCodeDecoder.canvas.println("--");
					QRCodeDecoder.canvas.println("Decoding restarted #" + this.numTryDecode);
					this.qrCodeSymbol = this.imageReader.getQRCodeSymbolWithAdjustedGrid(adjust);
				}
			}
			catch (SymbolNotFoundException e)
			{
				throw new DecodingFailedException(e.Message);
			}
			QRCodeDecoder.canvas.println("Created QRCode symbol.");
			QRCodeDecoder.canvas.println("Reading symbol.");
			QRCodeDecoder.canvas.println("Version: " + this.qrCodeSymbol.VersionReference);
			QRCodeDecoder.canvas.println("Mask pattern: " + this.qrCodeSymbol.MaskPatternRefererAsString);
			int[] blocks = this.qrCodeSymbol.Blocks;
			QRCodeDecoder.canvas.println("Correcting data errors.");
			blocks = this.correctDataBlocks(blocks);
			QRCodeDecoder.DecodeResult result;
			try
			{
				sbyte[] decodedByteArray = this.getDecodedByteArray(blocks, this.qrCodeSymbol.Version, this.qrCodeSymbol.NumErrorCollectionCode);
				result = new QRCodeDecoder.DecodeResult(this, decodedByteArray, this.numLastCorrections, this.correctionSucceeded);
			}
			catch (InvalidDataBlockException e2)
			{
				QRCodeDecoder.canvas.println(e2.Message);
				throw new DecodingFailedException(e2.Message);
			}
			return result;
		}
		internal virtual int[][] imageToIntArray(QRCodeImage image)
		{
			int width = image.Width;
			int height = image.Height;
			int[][] intImage = new int[width][];
			for (int i = 0; i < width; i++)
			{
				intImage[i] = new int[height];
			}
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					intImage[x][y] = image.getPixel(x, y);
				}
			}
			return intImage;
		}
		internal virtual int[] correctDataBlocks(int[] blocks)
		{
			int numCorrections = 0;
			int dataCapacity = this.qrCodeSymbol.DataCapacity;
			int[] dataBlocks = new int[dataCapacity];
			int numErrorCollectionCode = this.qrCodeSymbol.NumErrorCollectionCode;
			int numRSBlocks = this.qrCodeSymbol.NumRSBlocks;
			int eccPerRSBlock = numErrorCollectionCode / numRSBlocks;
			int[] result;
			if (numRSBlocks == 1)
			{
				ReedSolomon corrector = new ReedSolomon(blocks, eccPerRSBlock);
				corrector.correct();
				numCorrections += corrector.NumCorrectedErrors;
				if (numCorrections > 0)
				{
					QRCodeDecoder.canvas.println(Convert.ToString(numCorrections) + " data errors corrected.");
				}
				else
				{
					QRCodeDecoder.canvas.println("No errors found.");
				}
				this.numLastCorrections = numCorrections;
				this.correctionSucceeded = corrector.CorrectionSucceeded;
				result = blocks;
			}
			else
			{
				int numLongerRSBlocks = dataCapacity % numRSBlocks;
				if (numLongerRSBlocks == 0)
				{
					int lengthRSBlock = dataCapacity / numRSBlocks;
					int[][] tmpArray = new int[numRSBlocks][];
					for (int i = 0; i < numRSBlocks; i++)
					{
						tmpArray[i] = new int[lengthRSBlock];
					}
					int[][] RSBlocks = tmpArray;
					for (int i = 0; i < numRSBlocks; i++)
					{
						for (int j = 0; j < lengthRSBlock; j++)
						{
							RSBlocks[i][j] = blocks[j * numRSBlocks + i];
						}
						ReedSolomon corrector = new ReedSolomon(RSBlocks[i], eccPerRSBlock);
						corrector.correct();
						numCorrections += corrector.NumCorrectedErrors;
						this.correctionSucceeded = corrector.CorrectionSucceeded;
					}
					int p = 0;
					for (int i = 0; i < numRSBlocks; i++)
					{
						for (int j = 0; j < lengthRSBlock - eccPerRSBlock; j++)
						{
							dataBlocks[p++] = RSBlocks[i][j];
						}
					}
				}
				else
				{
					int lengthShorterRSBlock = dataCapacity / numRSBlocks;
					int lengthLongerRSBlock = dataCapacity / numRSBlocks + 1;
					int numShorterRSBlocks = numRSBlocks - numLongerRSBlocks;
					int[][] tmpArray2 = new int[numShorterRSBlocks][];
					for (int i2 = 0; i2 < numShorterRSBlocks; i2++)
					{
						tmpArray2[i2] = new int[lengthShorterRSBlock];
					}
					int[][] shorterRSBlocks = tmpArray2;
					int[][] tmpArray3 = new int[numLongerRSBlocks][];
					for (int i3 = 0; i3 < numLongerRSBlocks; i3++)
					{
						tmpArray3[i3] = new int[lengthLongerRSBlock];
					}
					int[][] longerRSBlocks = tmpArray3;
					for (int i = 0; i < numRSBlocks; i++)
					{
						if (i < numShorterRSBlocks)
						{
							int mod = 0;
							for (int j = 0; j < lengthShorterRSBlock; j++)
							{
								if (j == lengthShorterRSBlock - eccPerRSBlock)
								{
									mod = numLongerRSBlocks;
								}
								shorterRSBlocks[i][j] = blocks[j * numRSBlocks + i + mod];
							}
							ReedSolomon corrector = new ReedSolomon(shorterRSBlocks[i], eccPerRSBlock);
							corrector.correct();
							numCorrections += corrector.NumCorrectedErrors;
							this.correctionSucceeded = corrector.CorrectionSucceeded;
						}
						else
						{
							int mod = 0;
							for (int j = 0; j < lengthLongerRSBlock; j++)
							{
								if (j == lengthShorterRSBlock - eccPerRSBlock)
								{
									mod = numShorterRSBlocks;
								}
								longerRSBlocks[i - numShorterRSBlocks][j] = blocks[j * numRSBlocks + i - mod];
							}
							ReedSolomon corrector = new ReedSolomon(longerRSBlocks[i - numShorterRSBlocks], eccPerRSBlock);
							corrector.correct();
							numCorrections += corrector.NumCorrectedErrors;
							this.correctionSucceeded = corrector.CorrectionSucceeded;
						}
					}
					int p = 0;
					for (int i = 0; i < numRSBlocks; i++)
					{
						if (i < numShorterRSBlocks)
						{
							for (int j = 0; j < lengthShorterRSBlock - eccPerRSBlock; j++)
							{
								dataBlocks[p++] = shorterRSBlocks[i][j];
							}
						}
						else
						{
							for (int j = 0; j < lengthLongerRSBlock - eccPerRSBlock; j++)
							{
								dataBlocks[p++] = longerRSBlocks[i - numShorterRSBlocks][j];
							}
						}
					}
				}
				if (numCorrections > 0)
				{
					QRCodeDecoder.canvas.println(Convert.ToString(numCorrections) + " data errors corrected.");
				}
				else
				{
					QRCodeDecoder.canvas.println("No errors found.");
				}
				this.numLastCorrections = numCorrections;
				result = dataBlocks;
			}
			return result;
		}
		internal virtual sbyte[] getDecodedByteArray(int[] blocks, int version, int numErrorCorrectionCode)
		{
			QRCodeDataBlockReader reader = new QRCodeDataBlockReader(blocks, version, numErrorCorrectionCode);
			sbyte[] byteArray;
			try
			{
				byteArray = reader.DataByte;
			}
			catch (InvalidDataBlockException e)
			{
				throw e;
			}
			return byteArray;
		}
		internal virtual string getDecodedString(int[] blocks, int version, int numErrorCorrectionCode)
		{
			string dataString = null;
			QRCodeDataBlockReader reader = new QRCodeDataBlockReader(blocks, version, numErrorCorrectionCode);
			try
			{
				dataString = reader.DataString;
			}
			catch (IndexOutOfRangeException e)
			{
				throw new InvalidDataBlockException(e.Message);
			}
			return dataString;
		}
	}
}
