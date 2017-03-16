using System;
using ThoughtWorks.QRCode.Geom;
namespace ThoughtWorks.QRCode.Codec.Util
{
	public class DebugCanvasAdapter : DebugCanvas
	{
		public virtual void println(string string_Renamed)
		{
		}
		public virtual void drawPoint(Point point, int color)
		{
		}
		public virtual void drawCross(Point point, int color)
		{
		}
		public virtual void drawPoints(Point[] points, int color)
		{
		}
		public virtual void drawLine(Line line, int color)
		{
		}
		public virtual void drawLines(Line[] lines, int color)
		{
		}
		public virtual void drawPolygon(Point[] points, int color)
		{
		}
		public virtual void drawMatrix(bool[][] matrix)
		{
		}
	}
}
