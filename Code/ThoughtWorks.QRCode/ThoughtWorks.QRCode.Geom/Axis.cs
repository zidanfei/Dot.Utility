using System;
using ThoughtWorks.QRCode.Codec.Reader;
namespace ThoughtWorks.QRCode.Geom
{
	public class Axis
	{
		internal int sin;
		internal int cos;
		internal int modulePitch;
		internal Point origin;
		public virtual Point Origin
		{
			set
			{
				this.origin = value;
			}
		}
		public virtual int ModulePitch
		{
			set
			{
				this.modulePitch = value;
			}
		}
		public Axis(int[] angle, int modulePitch)
		{
			this.sin = angle[0];
			this.cos = angle[1];
			this.modulePitch = modulePitch;
			this.origin = new Point();
		}
		public virtual Point translate(Point offset)
		{
			int moveX = offset.X;
			int moveY = offset.Y;
			return this.translate(moveX, moveY);
		}
		public virtual Point translate(Point origin, Point offset)
		{
			this.Origin = origin;
			int moveX = offset.X;
			int moveY = offset.Y;
			return this.translate(moveX, moveY);
		}
		public virtual Point translate(Point origin, int moveX, int moveY)
		{
			this.Origin = origin;
			return this.translate(moveX, moveY);
		}
		public virtual Point translate(Point origin, int modulePitch, int moveX, int moveY)
		{
			this.Origin = origin;
			this.modulePitch = modulePitch;
			return this.translate(moveX, moveY);
		}
		public virtual Point translate(int moveX, int moveY)
		{
			long dp = (long)QRCodeImageReader.DECIMAL_POINT;
			Point point = new Point();
			int dx = (moveX == 0) ? 0 : (this.modulePitch * moveX >> (int)dp);
			int dy = (moveY == 0) ? 0 : (this.modulePitch * moveY >> (int)dp);
			point.translate(dx * this.cos - dy * this.sin >> (int)dp, dx * this.sin + dy * this.cos >> (int)dp);
			point.translate(this.origin.X, this.origin.Y);
			return point;
		}
	}
}
