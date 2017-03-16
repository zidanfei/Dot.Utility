using System;
using System.Collections;
using ThoughtWorks.QRCode.Codec.Util;
using ThoughtWorks.QRCode.ExceptionHandler;
using ThoughtWorks.QRCode.Geom;
namespace ThoughtWorks.QRCode.Codec.Reader.Pattern
{
	public class FinderPattern
	{
		public const int UL = 0;
		public const int UR = 1;
		public const int DL = 2;
		internal static readonly int[] VersionInfoBit;
		internal static DebugCanvas canvas;
		internal Point[] center;
		internal int version;
		internal int[] sincos;
		internal int[] width;
		internal int[] moduleSize;
		public virtual int Version
		{
			get
			{
				return this.version;
			}
		}
		public virtual int SqrtNumModules
		{
			get
			{
				return 17 + 4 * this.version;
			}
		}
		public static FinderPattern findFinderPattern(bool[][] image)
		{
			Line[] lineAcross = FinderPattern.findLineAcross(image);
			Line[] lineCross = FinderPattern.findLineCross(lineAcross);
			Point[] center = null;
			try
			{
				center = FinderPattern.getCenter(lineCross);
			}
			catch (FinderPatternNotFoundException e)
			{
				throw e;
			}
			int[] sincos = FinderPattern.getAngle(center);
			center = FinderPattern.sort(center, sincos);
			int[] width = FinderPattern.getWidth(image, center, sincos);
			int[] moduleSize = new int[]
			{
				(width[0] << QRCodeImageReader.DECIMAL_POINT) / 7,
				(width[1] << QRCodeImageReader.DECIMAL_POINT) / 7,
				(width[2] << QRCodeImageReader.DECIMAL_POINT) / 7
			};
			int version = FinderPattern.calcRoughVersion(center, width);
			if (version > 6)
			{
				try
				{
					version = FinderPattern.calcExactVersion(center, sincos, moduleSize, image);
				}
				catch (VersionInformationException e_AC)
				{
				}
			}
			return new FinderPattern(center, version, sincos, width, moduleSize);
		}
		internal FinderPattern(Point[] center, int version, int[] sincos, int[] width, int[] moduleSize)
		{
			this.center = center;
			this.version = version;
			this.sincos = sincos;
			this.width = width;
			this.moduleSize = moduleSize;
		}
		public virtual Point[] getCenter()
		{
			return this.center;
		}
		public virtual Point getCenter(int position)
		{
			Point result;
			if (position >= 0 && position <= 2)
			{
				result = this.center[position];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public virtual int getWidth(int position)
		{
			return this.width[position];
		}
		public virtual int[] getAngle()
		{
			return this.sincos;
		}
		public virtual int getModuleSize()
		{
			return this.moduleSize[0];
		}
		public virtual int getModuleSize(int place)
		{
			return this.moduleSize[place];
		}
		internal static Line[] findLineAcross(bool[][] image)
		{
			int READ_HORIZONTAL = 0;
			int READ_VERTICAL = 1;
			int imageWidth = image.Length;
			int imageHeight = image[0].Length;
			Point current = new Point();
			ArrayList lineAcross = ArrayList.Synchronized(new ArrayList(10));
			int[] lengthBuffer = new int[5];
			int bufferPointer = 0;
			int direction = READ_HORIZONTAL;
			bool lastElement = false;
			while (true)
			{
				bool currentElement = image[current.X][current.Y];
				if (currentElement == lastElement)
				{
					lengthBuffer[bufferPointer]++;
				}
				else
				{
					if (!currentElement)
					{
						if (FinderPattern.checkPattern(lengthBuffer, bufferPointer))
						{
							int x;
							int x2;
							int y3;
							int y2;
							if (direction == READ_HORIZONTAL)
							{
								x = current.X;
								for (int i = 0; i < 5; i++)
								{
									x -= lengthBuffer[i];
								}
								x2 = current.X - 1;
								y2 = (y3 = current.Y);
							}
							else
							{
								x2 = (x = current.X);
								y3 = current.Y;
								for (int i = 0; i < 5; i++)
								{
									y3 -= lengthBuffer[i];
								}
								y2 = current.Y - 1;
							}
							lineAcross.Add(new Line(x, y3, x2, y2));
						}
					}
					bufferPointer = (bufferPointer + 1) % 5;
					lengthBuffer[bufferPointer] = 1;
					lastElement = !lastElement;
				}
				if (direction == READ_HORIZONTAL)
				{
					if (current.X < imageWidth - 1)
					{
						current.translate(1, 0);
					}
					else
					{
						if (current.Y < imageHeight - 1)
						{
							current.set_Renamed(0, current.Y + 1);
							lengthBuffer = new int[5];
						}
						else
						{
							current.set_Renamed(0, 0);
							lengthBuffer = new int[5];
							direction = READ_VERTICAL;
						}
					}
				}
				else
				{
					if (current.Y < imageHeight - 1)
					{
						current.translate(0, 1);
					}
					else
					{
						if (current.X >= imageWidth - 1)
						{
							break;
						}
						current.set_Renamed(current.X + 1, 0);
						lengthBuffer = new int[5];
					}
				}
			}
			Line[] foundLines = new Line[lineAcross.Count];
			for (int j = 0; j < foundLines.Length; j++)
			{
				foundLines[j] = (Line)lineAcross[j];
			}
			FinderPattern.canvas.drawLines(foundLines, Color_Fields.LIGHTGREEN);
			return foundLines;
		}
		internal static bool checkPattern(int[] buffer, int pointer)
		{
			int[] modelRatio = new int[]
			{
				1,
				1,
				3,
				1,
				1
			};
			int baselength = 0;
			for (int i = 0; i < 5; i++)
			{
				baselength += buffer[i];
			}
			baselength <<= QRCodeImageReader.DECIMAL_POINT;
			baselength /= 7;
			bool result;
			for (int i2 = 0; i2 < 5; i2++)
			{
				int leastlength = baselength * modelRatio[i2] - baselength / 2;
				int mostlength = baselength * modelRatio[i2] + baselength / 2;
				int targetlength = buffer[(pointer + i2 + 1) % 5] << QRCodeImageReader.DECIMAL_POINT;
				if (targetlength < leastlength || targetlength > mostlength)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		internal static Line[] findLineCross(Line[] lineAcross)
		{
			ArrayList crossLines = ArrayList.Synchronized(new ArrayList(10));
			ArrayList lineNeighbor = ArrayList.Synchronized(new ArrayList(10));
			ArrayList lineCandidate = ArrayList.Synchronized(new ArrayList(10));
			for (int i = 0; i < lineAcross.Length; i++)
			{
				lineCandidate.Add(lineAcross[i]);
			}
			for (int i = 0; i < lineCandidate.Count - 1; i++)
			{
				lineNeighbor.Clear();
				lineNeighbor.Add(lineCandidate[i]);
				for (int j = i + 1; j < lineCandidate.Count; j++)
				{
					if (Line.isNeighbor((Line)lineNeighbor[lineNeighbor.Count - 1], (Line)lineCandidate[j]))
					{
						lineNeighbor.Add(lineCandidate[j]);
						Line compareLine = (Line)lineNeighbor[lineNeighbor.Count - 1];
						if (lineNeighbor.Count * 5 > compareLine.Length && j == lineCandidate.Count - 1)
						{
							crossLines.Add(lineNeighbor[lineNeighbor.Count / 2]);
							for (int k = 0; k < lineNeighbor.Count; k++)
							{
								lineCandidate.Remove(lineNeighbor[k]);
							}
						}
					}
					else
					{
						if (FinderPattern.cantNeighbor((Line)lineNeighbor[lineNeighbor.Count - 1], (Line)lineCandidate[j]) || j == lineCandidate.Count - 1)
						{
							Line compareLine = (Line)lineNeighbor[lineNeighbor.Count - 1];
							if (lineNeighbor.Count * 6 > compareLine.Length)
							{
								crossLines.Add(lineNeighbor[lineNeighbor.Count / 2]);
								for (int k = 0; k < lineNeighbor.Count; k++)
								{
									lineCandidate.Remove(lineNeighbor[k]);
								}
							}
							break;
						}
					}
				}
			}
			Line[] foundLines = new Line[crossLines.Count];
			for (int i = 0; i < foundLines.Length; i++)
			{
				foundLines[i] = (Line)crossLines[i];
			}
			return foundLines;
		}
		internal static bool cantNeighbor(Line line1, Line line2)
		{
			bool result;
			if (Line.isCross(line1, line2))
			{
				result = true;
			}
			else
			{
				if (line1.Horizontal)
				{
					result = (Math.Abs(line1.getP1().Y - line2.getP1().Y) > 1);
				}
				else
				{
					result = (Math.Abs(line1.getP1().X - line2.getP1().X) > 1);
				}
			}
			return result;
		}
		internal static int[] getAngle(Point[] centers)
		{
			Line[] additionalLine = new Line[3];
			for (int i = 0; i < additionalLine.Length; i++)
			{
				additionalLine[i] = new Line(centers[i], centers[(i + 1) % additionalLine.Length]);
			}
			Line remoteLine = Line.getLongest(additionalLine);
			Point originPoint = new Point();
			for (int i = 0; i < centers.Length; i++)
			{
				if (!remoteLine.getP1().equals(centers[i]) && !remoteLine.getP2().equals(centers[i]))
				{
					originPoint = centers[i];
					break;
				}
			}
			FinderPattern.canvas.println("originPoint is: " + originPoint);
			Point remotePoint = new Point();
			if (originPoint.Y <= remoteLine.getP1().Y & originPoint.Y <= remoteLine.getP2().Y)
			{
				if (remoteLine.getP1().X < remoteLine.getP2().X)
				{
					remotePoint = remoteLine.getP2();
				}
				else
				{
					remotePoint = remoteLine.getP1();
				}
			}
			else
			{
				if (originPoint.X >= remoteLine.getP1().X & originPoint.X >= remoteLine.getP2().X)
				{
					if (remoteLine.getP1().Y < remoteLine.getP2().Y)
					{
						remotePoint = remoteLine.getP2();
					}
					else
					{
						remotePoint = remoteLine.getP1();
					}
				}
				else
				{
					if (originPoint.Y >= remoteLine.getP1().Y & originPoint.Y >= remoteLine.getP2().Y)
					{
						if (remoteLine.getP1().X < remoteLine.getP2().X)
						{
							remotePoint = remoteLine.getP1();
						}
						else
						{
							remotePoint = remoteLine.getP2();
						}
					}
					else
					{
						if (remoteLine.getP1().Y < remoteLine.getP2().Y)
						{
							remotePoint = remoteLine.getP1();
						}
						else
						{
							remotePoint = remoteLine.getP2();
						}
					}
				}
			}
			int r = new Line(originPoint, remotePoint).Length;
			return new int[]
			{
				(remotePoint.Y - originPoint.Y << QRCodeImageReader.DECIMAL_POINT) / r,
				(remotePoint.X - originPoint.X << QRCodeImageReader.DECIMAL_POINT) / r
			};
		}
		internal static Point[] getCenter(Line[] crossLines)
		{
			ArrayList centers = ArrayList.Synchronized(new ArrayList(10));
			for (int i = 0; i < crossLines.Length - 1; i++)
			{
				Line compareLine = crossLines[i];
				for (int j = i + 1; j < crossLines.Length; j++)
				{
					Line comparedLine = crossLines[j];
					if (Line.isCross(compareLine, comparedLine))
					{
						int x;
						int y;
						if (compareLine.Horizontal)
						{
							x = compareLine.Center.X;
							y = comparedLine.Center.Y;
						}
						else
						{
							x = comparedLine.Center.X;
							y = compareLine.Center.Y;
						}
						centers.Add(new Point(x, y));
					}
				}
			}
			Point[] foundPoints = new Point[centers.Count];
			for (int i = 0; i < foundPoints.Length; i++)
			{
				foundPoints[i] = (Point)centers[i];
			}
			if (foundPoints.Length == 3)
			{
				FinderPattern.canvas.drawPolygon(foundPoints, Color_Fields.RED);
				return foundPoints;
			}
			throw new FinderPatternNotFoundException("Invalid number of Finder Pattern detected");
		}
		internal static Point[] sort(Point[] centers, int[] angle)
		{
			Point[] sortedCenters = new Point[3];
			switch (FinderPattern.getURQuadant(angle))
			{
			case 1:
				sortedCenters[1] = FinderPattern.getPointAtSide(centers, 1, 2);
				sortedCenters[2] = FinderPattern.getPointAtSide(centers, 2, 4);
				break;
			case 2:
				sortedCenters[1] = FinderPattern.getPointAtSide(centers, 2, 4);
				sortedCenters[2] = FinderPattern.getPointAtSide(centers, 8, 4);
				break;
			case 3:
				sortedCenters[1] = FinderPattern.getPointAtSide(centers, 4, 8);
				sortedCenters[2] = FinderPattern.getPointAtSide(centers, 1, 8);
				break;
			case 4:
				sortedCenters[1] = FinderPattern.getPointAtSide(centers, 8, 1);
				sortedCenters[2] = FinderPattern.getPointAtSide(centers, 2, 1);
				break;
			}
			for (int i = 0; i < centers.Length; i++)
			{
				if (!centers[i].equals(sortedCenters[1]) && !centers[i].equals(sortedCenters[2]))
				{
					sortedCenters[0] = centers[i];
				}
			}
			return sortedCenters;
		}
		internal static int getURQuadant(int[] angle)
		{
			int sin = angle[0];
			int cos = angle[1];
			int result;
			if (sin >= 0 && cos > 0)
			{
				result = 1;
			}
			else
			{
				if (sin > 0 && cos <= 0)
				{
					result = 2;
				}
				else
				{
					if (sin <= 0 && cos < 0)
					{
						result = 3;
					}
					else
					{
						if (sin < 0 && cos >= 0)
						{
							result = 4;
						}
						else
						{
							result = 0;
						}
					}
				}
			}
			return result;
		}
		internal static Point getPointAtSide(Point[] points, int side1, int side2)
		{
			Point sidePoint = new Point();
			int x = (side1 == 1 || side2 == 1) ? 0 : 2147483647;
			int y = (side1 == 2 || side2 == 2) ? 0 : 2147483647;
			sidePoint = new Point(x, y);
			for (int i = 0; i < points.Length; i++)
			{
				switch (side1)
				{
				case 1:
					if (sidePoint.X < points[i].X)
					{
						sidePoint = points[i];
					}
					else
					{
						if (sidePoint.X == points[i].X)
						{
							if (side2 == 2)
							{
								if (sidePoint.Y < points[i].Y)
								{
									sidePoint = points[i];
								}
							}
							else
							{
								if (sidePoint.Y > points[i].Y)
								{
									sidePoint = points[i];
								}
							}
						}
					}
					break;
				case 2:
					if (sidePoint.Y < points[i].Y)
					{
						sidePoint = points[i];
					}
					else
					{
						if (sidePoint.Y == points[i].Y)
						{
							if (side2 == 1)
							{
								if (sidePoint.X < points[i].X)
								{
									sidePoint = points[i];
								}
							}
							else
							{
								if (sidePoint.X > points[i].X)
								{
									sidePoint = points[i];
								}
							}
						}
					}
					break;
				case 3:
					break;
				case 4:
					if (sidePoint.X > points[i].X)
					{
						sidePoint = points[i];
					}
					else
					{
						if (sidePoint.X == points[i].X)
						{
							if (side2 == 2)
							{
								if (sidePoint.Y < points[i].Y)
								{
									sidePoint = points[i];
								}
							}
							else
							{
								if (sidePoint.Y > points[i].Y)
								{
									sidePoint = points[i];
								}
							}
						}
					}
					break;
				default:
					if (side1 == 8)
					{
						if (sidePoint.Y > points[i].Y)
						{
							sidePoint = points[i];
						}
						else
						{
							if (sidePoint.Y == points[i].Y)
							{
								if (side2 == 1)
								{
									if (sidePoint.X < points[i].X)
									{
										sidePoint = points[i];
									}
								}
								else
								{
									if (sidePoint.X > points[i].X)
									{
										sidePoint = points[i];
									}
								}
							}
						}
					}
					break;
				}
			}
			return sidePoint;
		}
		internal static int[] getWidth(bool[][] image, Point[] centers, int[] sincos)
		{
			int[] width = new int[3];
			for (int i = 0; i < 3; i++)
			{
				bool flag = false;
				int y = centers[i].Y;
				int lx;
				for (lx = centers[i].X; lx > 0; lx--)
				{
					if (image[lx][y] && !image[lx - 1][y])
					{
						if (flag)
						{
							break;
						}
						flag = true;
					}
				}
				flag = false;
				int rx;
				for (rx = centers[i].X; rx < image.Length; rx++)
				{
					if (image[rx][y] && !image[rx + 1][y])
					{
						if (flag)
						{
							break;
						}
						flag = true;
					}
				}
				width[i] = rx - lx + 1;
			}
			return width;
		}
		internal static int calcRoughVersion(Point[] center, int[] width)
		{
			int dp = QRCodeImageReader.DECIMAL_POINT;
			int lengthAdditionalLine = new Line(center[0], center[1]).Length << dp;
			int avarageWidth = (width[0] + width[1] << dp) / 14;
			int roughVersion = (lengthAdditionalLine / avarageWidth - 10) / 4;
			if ((lengthAdditionalLine / avarageWidth - 10) % 4 >= 2)
			{
				roughVersion++;
			}
			return roughVersion;
		}
		internal static int calcExactVersion(Point[] centers, int[] angle, int[] moduleSize, bool[][] image)
		{
			bool[] versionInformation = new bool[18];
			Point[] points = new Point[18];
			Axis axis = new Axis(angle, moduleSize[1]);
			axis.Origin = centers[1];
			for (int y = 0; y < 6; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					Point target = axis.translate(x - 7, y - 3);
					versionInformation[x + y * 3] = image[target.X][target.Y];
					points[x + y * 3] = target;
				}
			}
			FinderPattern.canvas.drawPoints(points, Color_Fields.RED);
			int exactVersion = 0;
			try
			{
				exactVersion = FinderPattern.checkVersionInfo(versionInformation);
			}
			catch (InvalidVersionInfoException e_A9)
			{
				FinderPattern.canvas.println("Version info error. now retry with other place one.");
				axis.Origin = centers[2];
				axis.ModulePitch = moduleSize[2];
				for (int x = 0; x < 6; x++)
				{
					for (int y = 0; y < 3; y++)
					{
						Point target = axis.translate(x - 3, y - 7);
						versionInformation[y + x * 3] = image[target.X][target.Y];
						points[x + y * 3] = target;
					}
				}
				FinderPattern.canvas.drawPoints(points, Color_Fields.RED);
				try
				{
					exactVersion = FinderPattern.checkVersionInfo(versionInformation);
				}
				catch (VersionInformationException e2)
				{
					throw e2;
				}
			}
			return exactVersion;
		}
		internal static int checkVersionInfo(bool[] target)
		{
			int errorCount = 0;
			int versionBase;
			for (versionBase = 0; versionBase < FinderPattern.VersionInfoBit.Length; versionBase++)
			{
				errorCount = 0;
				for (int i = 0; i < 18; i++)
				{
					if (target[i] ^ (FinderPattern.VersionInfoBit[versionBase] >> i) % 2 == 1)
					{
						errorCount++;
					}
				}
				if (errorCount <= 3)
				{
					break;
				}
			}
			if (errorCount <= 3)
			{
				return 7 + versionBase;
			}
			throw new InvalidVersionInfoException("Too many errors in version information");
		}
		static FinderPattern()
		{
			FinderPattern.VersionInfoBit = new int[]
			{
				31892,
				34236,
				39577,
				42195,
				48118,
				51042,
				55367,
				58893,
				63784,
				68472,
				70749,
				76311,
				79154,
				84390,
				87683,
				92361,
				96236,
				102084,
				102881,
				110507,
				110734,
				117786,
				119615,
				126325,
				127568,
				133589,
				136944,
				141498,
				145311,
				150283,
				152622,
				158308,
				161089,
				167017
			};
			FinderPattern.canvas = QRCodeDecoder.Canvas;
		}
	}
}
