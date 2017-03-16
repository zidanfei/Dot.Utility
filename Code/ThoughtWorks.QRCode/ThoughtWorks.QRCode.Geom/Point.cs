using System;
using ThoughtWorks.QRCode.Codec.Util;
namespace ThoughtWorks.QRCode.Geom
{
	public class Point
	{
		public const int RIGHT = 1;
		public const int BOTTOM = 2;
		public const int LEFT = 4;
		public const int TOP = 8;
		internal int x;
		internal int y;
		public virtual int X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}
		public virtual int Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}
		public Point()
		{
			this.x = 0;
			this.y = 0;
		}
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		public virtual void translate(int dx, int dy)
		{
			this.x += dx;
			this.y += dy;
		}
		public virtual void set_Renamed(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"(",
				Convert.ToString(this.x),
				",",
				Convert.ToString(this.y),
				")"
			});
		}
		public static Point getCenter(Point p1, Point p2)
		{
			return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
		}
		public bool equals(Point compare)
		{
			return this.x == compare.x && this.y == compare.y;
		}
		public virtual int distanceOf(Point other)
		{
			int x2 = other.X;
			int y2 = other.Y;
			return QRCodeUtility.sqrt((this.x - x2) * (this.x - x2) + (this.y - y2) * (this.y - y2));
		}
	}
}
