using System;
using ThoughtWorks.QRCode.Codec.Util;
using ThoughtWorks.QRCode.ExceptionHandler;
using ThoughtWorks.QRCode.Geom;
namespace ThoughtWorks.QRCode.Codec.Reader.Pattern
{
	public class AlignmentPattern
	{
		internal const int RIGHT = 1;
		internal const int BOTTOM = 2;
		internal const int LEFT = 3;
		internal const int TOP = 4;
		internal static DebugCanvas canvas;
		internal Point[][] center;
		internal int patternDistance;
		public virtual int LogicalDistance
		{
			get
			{
				return this.patternDistance;
			}
		}
		internal AlignmentPattern(Point[][] center, int patternDistance)
		{
			this.center = center;
			this.patternDistance = patternDistance;
		}
		public static AlignmentPattern findAlignmentPattern(bool[][] image, FinderPattern finderPattern)
		{
			Point[][] logicalCenters = AlignmentPattern.getLogicalCenter(finderPattern);
			int logicalDistance = logicalCenters[1][0].X - logicalCenters[0][0].X;
			Point[][] centers = AlignmentPattern.getCenter(image, finderPattern, logicalCenters);
			return new AlignmentPattern(centers, logicalDistance);
		}
		public virtual Point[][] getCenter()
		{
			return this.center;
		}
		public virtual void setCenter(Point[][] center)
		{
			this.center = center;
		}
		internal static Point[][] getCenter(bool[][] image, FinderPattern finderPattern, Point[][] logicalCenters)
		{
			int moduleSize = finderPattern.getModuleSize();
			Axis axis = new Axis(finderPattern.getAngle(), moduleSize);
			int sqrtCenters = logicalCenters.Length;
			Point[][] centers = new Point[sqrtCenters][];
			for (int i = 0; i < sqrtCenters; i++)
			{
				centers[i] = new Point[sqrtCenters];
			}
			axis.Origin = finderPattern.getCenter(0);
			centers[0][0] = axis.translate(3, 3);
			AlignmentPattern.canvas.drawCross(centers[0][0], Color_Fields.BLUE);
			axis.Origin = finderPattern.getCenter(1);
			centers[sqrtCenters - 1][0] = axis.translate(-3, 3);
			AlignmentPattern.canvas.drawCross(centers[sqrtCenters - 1][0], Color_Fields.BLUE);
			axis.Origin = finderPattern.getCenter(2);
			centers[0][sqrtCenters - 1] = axis.translate(3, -3);
			AlignmentPattern.canvas.drawCross(centers[0][sqrtCenters - 1], Color_Fields.BLUE);
			Point tmpPoint = centers[0][0];
			for (int y = 0; y < sqrtCenters; y++)
			{
				for (int x = 0; x < sqrtCenters; x++)
				{
					if ((x != 0 || y != 0) && (x != 0 || y != sqrtCenters - 1) && (x != sqrtCenters - 1 || y != 0))
					{
						Point target = null;
						if (y == 0)
						{
							if (x > 0 && x < sqrtCenters - 1)
							{
								target = axis.translate(centers[x - 1][y], logicalCenters[x][y].X - logicalCenters[x - 1][y].X, 0);
							}
							centers[x][y] = new Point(target.X, target.Y);
							AlignmentPattern.canvas.drawCross(centers[x][y], Color_Fields.RED);
						}
						else
						{
							if (x == 0)
							{
								if (y > 0 && y < sqrtCenters - 1)
								{
									target = axis.translate(centers[x][y - 1], 0, logicalCenters[x][y].Y - logicalCenters[x][y - 1].Y);
								}
								centers[x][y] = new Point(target.X, target.Y);
								AlignmentPattern.canvas.drawCross(centers[x][y], Color_Fields.RED);
							}
							else
							{
								Point t = axis.translate(centers[x - 1][y], logicalCenters[x][y].X - logicalCenters[x - 1][y].X, 0);
								Point t2 = axis.translate(centers[x][y - 1], 0, logicalCenters[x][y].Y - logicalCenters[x][y - 1].Y);
								centers[x][y] = new Point((t.X + t2.X) / 2, (t.Y + t2.Y) / 2 + 1);
							}
						}
						if (finderPattern.Version > 1)
						{
							Point precisionCenter = AlignmentPattern.getPrecisionCenter(image, centers[x][y]);
							if (centers[x][y].distanceOf(precisionCenter) < 6)
							{
								AlignmentPattern.canvas.drawCross(centers[x][y], Color_Fields.RED);
								int dx = precisionCenter.X - centers[x][y].X;
								int dy = precisionCenter.Y - centers[x][y].Y;
								AlignmentPattern.canvas.println(string.Concat(new object[]
								{
									"Adjust AP(",
									x,
									",",
									y,
									") to d(",
									dx,
									",",
									dy,
									")"
								}));
								centers[x][y] = precisionCenter;
							}
						}
						AlignmentPattern.canvas.drawCross(centers[x][y], Color_Fields.BLUE);
						AlignmentPattern.canvas.drawLine(new Line(tmpPoint, centers[x][y]), Color_Fields.LIGHTBLUE);
						tmpPoint = centers[x][y];
					}
				}
			}
			return centers;
		}
		internal static Point getPrecisionCenter(bool[][] image, Point targetPoint)
		{
			int tx = targetPoint.X;
			int ty = targetPoint.Y;
			if (tx < 0 || ty < 0 || tx > image.Length - 1 || ty > image[0].Length - 1)
			{
				throw new AlignmentPatternNotFoundException("Alignment Pattern finder exceeded out of image");
			}
			if (!image[targetPoint.X][targetPoint.Y])
			{
				int scope = 0;
				bool found = false;
				while (!found)
				{
					scope++;
					for (int dy = scope; dy > -scope; dy--)
					{
						for (int dx = scope; dx > -scope; dx--)
						{
							int x = targetPoint.X + dx;
							int y = targetPoint.Y + dy;
							if (x < 0 || y < 0 || x > image.Length - 1 || y > image[0].Length - 1)
							{
								throw new AlignmentPatternNotFoundException("Alignment Pattern finder exceeded out of image");
							}
							if (image[x][y])
							{
								targetPoint = new Point(targetPoint.X + dx, targetPoint.Y + dy);
								found = true;
							}
						}
					}
				}
			}
			int rx;
			int x2;
			int lx = x2 = (rx = targetPoint.X);
			int dy2;
			int y2;
			int uy = y2 = (dy2 = targetPoint.Y);
			while (lx >= 1 && !AlignmentPattern.targetPointOnTheCorner(image, lx, y2, lx - 1, y2))
			{
				lx--;
			}
			while (rx < image.Length - 1 && !AlignmentPattern.targetPointOnTheCorner(image, rx, y2, rx + 1, y2))
			{
				rx++;
			}
			while (uy >= 1 && !AlignmentPattern.targetPointOnTheCorner(image, x2, uy, x2, uy - 1))
			{
				uy--;
			}
			while (dy2 < image[0].Length - 1 && !AlignmentPattern.targetPointOnTheCorner(image, x2, dy2, x2, dy2 + 1))
			{
				dy2++;
			}
			return new Point((lx + rx + 1) / 2, (uy + dy2 + 1) / 2);
		}
		internal static bool targetPointOnTheCorner(bool[][] image, int x, int y, int nx, int ny)
		{
			if (x < 0 || y < 0 || nx < 0 || ny < 0 || x > image.Length || y > image[0].Length || nx > image.Length || ny > image[0].Length)
			{
				throw new AlignmentPatternNotFoundException("Alignment Pattern Finder exceeded image edge");
			}
			return !image[x][y] && image[nx][ny];
		}
		public static Point[][] getLogicalCenter(FinderPattern finderPattern)
		{
			int version = finderPattern.Version;
			Point[][] logicalCenters = new Point[1][];
			for (int i = 0; i < 1; i++)
			{
				logicalCenters[i] = new Point[1];
			}
			int[] logicalSeeds = new int[1];
			logicalSeeds = LogicalSeed.getSeed(version);
			logicalCenters = new Point[logicalSeeds.Length][];
			for (int i2 = 0; i2 < logicalSeeds.Length; i2++)
			{
				logicalCenters[i2] = new Point[logicalSeeds.Length];
			}
			for (int col = 0; col < logicalCenters.Length; col++)
			{
				for (int row = 0; row < logicalCenters.Length; row++)
				{
					logicalCenters[row][col] = new Point(logicalSeeds[row], logicalSeeds[col]);
				}
			}
			return logicalCenters;
		}
		static AlignmentPattern()
		{
			AlignmentPattern.canvas = QRCodeDecoder.Canvas;
		}
	}
}
