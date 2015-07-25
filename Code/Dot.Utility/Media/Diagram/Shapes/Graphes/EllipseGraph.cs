using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class EllipseGraph : DiagramGraph
    {
        public EllipseGraph()
        {
        }

        public override bool ContainsPoint(PointF p)
        {
            if (!base.ContainsPoint(p))
            {
                return false;
            }
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            float single2 = ef1.Width / 2f;
            float single3 = ef1.Height / 2f;
            float single4 = ef1.X + single2;
            float single5 = ef1.Y + single3;
            single2 += single1;
            single3 += single1;
            if ((single2 == 0f) || (single3 == 0f))
            {
                return false;
            }
            float single6 = p.X - single4;
            float single7 = p.Y - single5;
            return ((((single6 * single6) / (single2 * single2)) + ((single7 * single7) / (single3 * single3))) <= 1f);
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            DiagramShape.InflateRect(ref ef1, single1, single1);
            return EllipseGraph.NearestIntersectionOnEllipse(ef1, p1, p2, out result);
        }

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
            RectangleF ef1 = this.Bounds;
            path1.AddEllipse(ef1.X, ef1.Y, ef1.Width, ef1.Height);
            return path1;
        }

        public static bool NearestIntersectionOnArc(RectangleF rect, PointF p1, PointF p2, out PointF result, float startAngle, float sweepAngle)
        {
            float single5;
            float single6;
            float single1 = rect.Width / 2f;
            float single2 = rect.Height / 2f;
            float single3 = rect.X + single1;
            float single4 = rect.Y + single2;
            if (sweepAngle < 0f)
            {
                single5 = startAngle + sweepAngle;
                single6 = -sweepAngle;
            }
            else
            {
                single5 = startAngle;
                single6 = sweepAngle;
            }
            if (p1.X != p2.X)
            {
                float single7;
                if (p1.X > p2.X)
                {
                    single7 = (p1.Y - p2.Y) / (p1.X - p2.X);
                }
                else
                {
                    single7 = (p2.Y - p1.Y) / (p2.X - p1.X);
                }
                float single8 = (p1.Y - single4) - (single7 * (p1.X - single3));
                float single9 = (float)System.Math.Sqrt((double)((((single1 * single1) * (single7 * single7)) + (single2 * single2)) - (single8 * single8)));
                float single10 = ((-(((single1 * single1) * single7) * single8) + ((single1 * single2) * single9)) / ((single2 * single2) + ((single1 * single1) * (single7 * single7)))) + single3;
                float single11 = ((-(((single1 * single1) * single7) * single8) - ((single1 * single2) * single9)) / ((single2 * single2) + ((single1 * single1) * (single7 * single7)))) + single3;
                float single12 = ((single7 * (single10 - single3)) + single8) + single4;
                float single13 = ((single7 * (single11 - single3)) + single8) + single4;
                float single14 = StrokeGraph.GetAngle(single10 - single3, single12 - single4);
                float single15 = StrokeGraph.GetAngle(single11 - single3, single13 - single4);
                if (single14 < single5)
                {
                    single14 += 360f;
                }
                if (single15 < single5)
                {
                    single15 += 360f;
                }
                if (single14 > (single5 + single6))
                {
                    single14 -= 360f;
                }
                if (single15 > (single5 + single6))
                {
                    single15 -= 360f;
                }
                bool flag1 = (single14 >= single5) && (single14 <= (single5 + single6));
                bool flag2 = (single15 >= single5) && (single15 <= (single5 + single6));
                if (flag1 && flag2)
                {
                    float single16 = System.Math.Abs((float)((p1.X - single10) * (p1.X - single10))) + System.Math.Abs((float)((p1.Y - single12) * (p1.Y - single12)));
                    float single17 = System.Math.Abs((float)((p1.X - single11) * (p1.X - single11))) + System.Math.Abs((float)((p1.Y - single13) * (p1.Y - single13)));
                    if (single16 < single17)
                    {
                        result = new PointF(single10, single12);
                    }
                    else
                    {
                        result = new PointF(single11, single13);
                    }
                    return true;
                }
                if (flag1 && !flag2)
                {
                    result = new PointF(single10, single12);
                    return true;
                }
                if (!flag1 && flag2)
                {
                    result = new PointF(single11, single13);
                    return true;
                }
                result = new PointF();
                return false;
            }
            float single18 = (float)System.Math.Sqrt((double)((single2 * single2) - (((single2 * single2) / (single1 * single1)) * ((p1.X - single3) * (p1.X - single3)))));
            float single19 = single4 + single18;
            float single20 = single4 - single18;
            float single21 = StrokeGraph.GetAngle(p1.X - single3, single19 - single4);
            float single22 = StrokeGraph.GetAngle(p1.X - single3, single20 - single4);
            if (single21 < single5)
            {
                single21 += 360f;
            }
            if (single22 < single5)
            {
                single22 += 360f;
            }
            if (single21 > (single5 + single6))
            {
                single21 -= 360f;
            }
            if (single22 > (single5 + single6))
            {
                single22 -= 360f;
            }
            bool flag3 = (single21 >= single5) && (single21 <= (single5 + single6));
            bool flag4 = (single22 >= single5) && (single22 <= (single5 + single6));
            if (flag3 && flag4)
            {
                float single23 = System.Math.Abs((float)(single19 - p1.Y));
                float single24 = System.Math.Abs((float)(single20 - p1.Y));
                if (single23 < single24)
                {
                    result = new PointF(p1.X, single19);
                }
                else
                {
                    result = new PointF(p1.X, single20);
                }
                return true;
            }
            if (flag3 && !flag4)
            {
                result = new PointF(p1.X, single19);
                return true;
            }
            if (!flag3 && flag4)
            {
                result = new PointF(p1.X, single20);
                return true;
            }
            result = new PointF();
            return false;
        }

        public static bool NearestIntersectionOnEllipse(RectangleF rect, PointF p1, PointF p2, out PointF result)
        {
            if (rect.Width == 0f)
            {
                return StrokeGraph.NearestIntersectionOnLine(new PointF(rect.X, rect.Y), new PointF(rect.X, rect.Y + rect.Height), p1, p2, out result);
            }
            if (rect.Height == 0f)
            {
                return StrokeGraph.NearestIntersectionOnLine(new PointF(rect.X, rect.Y), new PointF(rect.X + rect.Width, rect.Y), p1, p2, out result);
            }
            float single1 = rect.Width / 2f;
            float single2 = rect.Height / 2f;
            float single3 = rect.X + single1;
            float single4 = rect.Y + single2;
            if (p1.X != p2.X)
            {
                float single5;
                if (p1.X > p2.X)
                {
                    single5 = (p1.Y - p2.Y) / (p1.X - p2.X);
                }
                else
                {
                    single5 = (p2.Y - p1.Y) / (p2.X - p1.X);
                }
                float single6 = (p1.Y - single4) - (single5 * (p1.X - single3));
                if (((((single1 * single1) * (single5 * single5)) + (single2 * single2)) - (single6 * single6)) < 0f)
                {
                    result = new PointF();
                    return false;
                }
                float single7 = (float)System.Math.Sqrt((double)((((single1 * single1) * (single5 * single5)) + (single2 * single2)) - (single6 * single6)));
                float single8 = ((-(((single1 * single1) * single5) * single6) + ((single1 * single2) * single7)) / ((single2 * single2) + ((single1 * single1) * (single5 * single5)))) + single3;
                float single9 = ((-(((single1 * single1) * single5) * single6) - ((single1 * single2) * single7)) / ((single2 * single2) + ((single1 * single1) * (single5 * single5)))) + single3;
                float single10 = ((single5 * (single8 - single3)) + single6) + single4;
                float single11 = ((single5 * (single9 - single3)) + single6) + single4;
                float single12 = System.Math.Abs((float)((p1.X - single8) * (p1.X - single8))) + System.Math.Abs((float)((p1.Y - single10) * (p1.Y - single10)));
                float single13 = System.Math.Abs((float)((p1.X - single9) * (p1.X - single9))) + System.Math.Abs((float)((p1.Y - single11) * (p1.Y - single11)));
                if (single12 < single13)
                {
                    result = new PointF(single8, single10);
                }
                else
                {
                    result = new PointF(single9, single11);
                }
            }
            else
            {
                float single14 = single2 * single2;
                float single15 = single1 * single1;
                float single16 = p1.X - single3;
                float single17 = single14 - ((single14 / single15) * (single16 * single16));
                if (single17 < 0f)
                {
                    result = new PointF();
                    return false;
                }
                float single18 = (float)System.Math.Sqrt((double)single17);
                float single19 = single4 + single18;
                float single20 = single4 - single18;
                float single21 = System.Math.Abs((float)(single19 - p1.Y));
                float single22 = System.Math.Abs((float)(single20 - p1.Y));
                if (single21 < single22)
                {
                    result = new PointF(p1.X, single19);
                }
                else
                {
                    result = new PointF(p1.X, single20);
                }
            }
            return true;
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            RectangleF ef1 = this.Bounds;
            if (this.Shadowed)
            {
                SizeF ef2 = this.GetShadowOffset(view);
                if (this.Brush != null)
                {
                    Brush brush1 = this.GetShadowBrush(view);
                    DiagramGraph.DrawEllipse(g, view, null, brush1, ef1.X + ef2.Width, ef1.Y + ef2.Height, ef1.Width, ef1.Height);
                }
                else if (this.Pen != null)
                {
                    Pen pen1 = this.GetShadowPen(view, base.InternalPenWidth);
                    DiagramGraph.DrawEllipse(g, view, pen1, null, ef1.X + ef2.Width, ef1.Y + ef2.Height, ef1.Width, ef1.Height);
                }
            }
            DiagramGraph.DrawEllipse(g, view, this.Pen, this.Brush, ef1.X, ef1.Y, ef1.Width, ef1.Height);
        }

    }
}
