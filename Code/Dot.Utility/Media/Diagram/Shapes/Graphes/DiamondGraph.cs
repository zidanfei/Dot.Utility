using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class DiamondGraph : DiagramGraph
    {
        public DiamondGraph()
        {
            this.myPoints = new PointF[4];
        }

        public override bool ContainsPoint(PointF p)
        {
            if (!base.ContainsPoint(p))
            {
                return false;
            }
            GraphicsPath path1 = base.GetPath();
            return path1.IsVisible(p);
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            DiamondGraph diamond1 = (DiamondGraph)base.CopyObject(env);
            if (diamond1 != null)
            {
                diamond1.myPoints = (PointF[])this.myPoints.Clone();
            }
            return diamond1;
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            PointF tf5;
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            PointF[] tfArray1 = this.getPoints();
            PointF tf1 = DiagramGraph.ExpandPointOnEdge(tfArray1[0], ef1, single1);
            PointF tf2 = DiagramGraph.ExpandPointOnEdge(tfArray1[1], ef1, single1);
            PointF tf3 = DiagramGraph.ExpandPointOnEdge(tfArray1[2], ef1, single1);
            PointF tf4 = DiagramGraph.ExpandPointOnEdge(tfArray1[3], ef1, single1);
            float single2 = p1.X;
            float single3 = p1.Y;
            float single4 = 1E+21f;
            PointF tf6 = new PointF();
            if (StrokeGraph.NearestIntersectionOnLine(tf1, tf2, p1, p2, out tf5))
            {
                float single5 = ((tf5.X - single2) * (tf5.X - single2)) + ((tf5.Y - single3) * (tf5.Y - single3));
                if (single5 < single4)
                {
                    single4 = single5;
                    tf6 = tf5;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf2, tf3, p1, p2, out tf5))
            {
                float single6 = ((tf5.X - single2) * (tf5.X - single2)) + ((tf5.Y - single3) * (tf5.Y - single3));
                if (single6 < single4)
                {
                    single4 = single6;
                    tf6 = tf5;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf3, tf4, p1, p2, out tf5))
            {
                float single7 = ((tf5.X - single2) * (tf5.X - single2)) + ((tf5.Y - single3) * (tf5.Y - single3));
                if (single7 < single4)
                {
                    single4 = single7;
                    tf6 = tf5;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf4, tf1, p1, p2, out tf5))
            {
                float single8 = ((tf5.X - single2) * (tf5.X - single2)) + ((tf5.Y - single3) * (tf5.Y - single3));
                if (single8 < single4)
                {
                    single4 = single8;
                    tf6 = tf5;
                }
            }
            result = tf6;
            return (single4 < 1E+21f);
        }

        private PointF[] getPoints()
        {
            RectangleF ef1 = this.Bounds;
            this.myPoints[0].X = ef1.X + (ef1.Width / 2f);
            this.myPoints[0].Y = ef1.Y;
            this.myPoints[1].X = ef1.X + ef1.Width;
            this.myPoints[1].Y = ef1.Y + (ef1.Height / 2f);
            this.myPoints[2].X = this.myPoints[0].X;
            this.myPoints[2].Y = ef1.Y + ef1.Height;
            this.myPoints[3].X = ef1.X;
            this.myPoints[3].Y = this.myPoints[1].Y;
            return this.myPoints;
        }

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
            path1.AddLines(this.getPoints());
            path1.CloseAllFigures();
            return path1;
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            if (this.Shadowed)
            {
                PointF[] tfArray1 = this.getPoints();
                int num1 = tfArray1.Length;
                SizeF ef1 = this.GetShadowOffset(view);
                for (int num2 = 0; num2 < num1; num2++)
                {
                    tfArray1[num2].X += ef1.Width;
                    tfArray1[num2].Y += ef1.Height;
                }
                if (this.Brush != null)
                {
                    Brush brush1 = this.GetShadowBrush(view);
                    DiagramGraph.DrawPolygon(g, view, null, brush1, tfArray1);
                }
                else if (this.Pen != null)
                {
                    Pen pen1 = this.GetShadowPen(view, base.InternalPenWidth);
                    DiagramGraph.DrawPolygon(g, view, pen1, null, tfArray1);
                }
            }
            DiagramGraph.DrawPolygon(g, view, this.Pen, this.Brush, this.getPoints());
        }


        private PointF[] myPoints;
    }
}
