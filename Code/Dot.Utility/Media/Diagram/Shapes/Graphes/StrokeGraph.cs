using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class StrokeGraph : DiagramGraph
    {
        static StrokeGraph()
        {
            StrokeGraph.myIntersections = new float[50];
        }

        public StrokeGraph()
        {
            this.myStyle = StrokeGraphStyle.Line;
            this.myPointsCount = 0;
            this.myPoints = new PointF[6];
            this.myToArrowInfo = null;
            this.myFromArrowInfo = null;
            this.myCurviness = 10f;
            this.myHighlightPenInfo = DiagramGraph.GetPenInfo(null);
            base.InternalFlags |= 0x200;
            this.Brush = DiagramGraph.Brushes_Black;
        }

        private void addLine(GraphicsPath path, float offx, float offy, PointF from, PointF to)
        {
            if (this.Style != StrokeGraphStyle.RoundedLineWithJumpOvers)
            {
                path.AddLine((float)(from.X + offx), (float)(from.Y + offy), (float)(to.X + offx), (float)(to.Y + offy));
            }
            else
            {
                float single1 = 10f;
                float single2 = single1 / 2f;
                int num6 = this.PointsCount;
                lock (StrokeGraph.myIntersections)
                {
                    float[] singleArray1 = StrokeGraph.myIntersections;
                    int num1 = this.getIntersections(from, to, singleArray1);
                    PointF tf1 = from;
                    if (num1 > 0)
                    {
                        if (base.IsApprox(from.Y, to.Y))
                        {
                            if (from.X < to.X)
                            {
                                int num2 = 0;
                                while (num2 < num1)
                                {
                                    float single3 = System.Math.Max(from.X, System.Math.Min((float)(singleArray1[num2++] - single2), (float)(to.X - single1)));
                                    path.AddLine((float)(tf1.X + offx), (float)(tf1.Y + offy), (float)(single3 + offx), (float)(to.Y + offy));
                                    tf1 = new PointF(single3 + offx, to.Y + offy);
                                    float single4 = System.Math.Min((float)(single3 + single1), to.X);
                                    while (num2 < num1)
                                    {
                                        float single5 = singleArray1[num2];
                                        if (single5 >= (single4 + single1))
                                        {
                                            break;
                                        }
                                        num2++;
                                        single4 = System.Math.Min((float)(single5 + single2), to.X);
                                    }
                                    PointF tf2 = new PointF((single3 + single4) / 2f, to.Y - single1);
                                    PointF tf3 = new PointF(single4, to.Y);
                                    path.AddBezier(tf1.X, tf1.Y, tf1.X, tf2.Y, tf3.X, tf2.Y, tf3.X, tf3.Y);
                                    tf1 = tf3;
                                }
                            }
                            else
                            {
                                int num3 = num1 - 1;
                                while (num3 >= 0)
                                {
                                    float single6 = System.Math.Min(from.X, System.Math.Max((float)(singleArray1[num3--] + single2), (float)(to.X + single1)));
                                    path.AddLine((float)(tf1.X + offx), (float)(tf1.Y + offy), (float)(single6 + offx), (float)(to.Y + offy));
                                    tf1 = new PointF(single6 + offx, to.Y + offy);
                                    float single7 = System.Math.Max((float)(single6 - single1), to.X);
                                    while (num3 >= 0)
                                    {
                                        float single8 = singleArray1[num3];
                                        if (single8 <= (single7 - single1))
                                        {
                                            break;
                                        }
                                        num3--;
                                        single7 = System.Math.Max((float)(single8 - single2), to.X);
                                    }
                                    PointF tf4 = new PointF((single6 + single7) / 2f, to.Y - single1);
                                    PointF tf5 = new PointF(single7, to.Y);
                                    path.AddBezier(tf1.X, tf1.Y, tf1.X, tf4.Y, tf5.X, tf4.Y, tf5.X, tf5.Y);
                                    tf1 = tf5;
                                }
                            }
                        }
                        else if (base.IsApprox(from.X, to.X))
                        {
                            if (from.Y < to.Y)
                            {
                                int num4 = 0;
                                while (num4 < num1)
                                {
                                    float single9 = System.Math.Max(from.Y, System.Math.Min((float)(singleArray1[num4++] - single2), (float)(to.Y - single1)));
                                    path.AddLine((float)(tf1.X + offx), (float)(tf1.Y + offy), (float)(to.X + offx), (float)(single9 + offy));
                                    tf1 = new PointF(to.X + offx, single9 + offy);
                                    float single10 = System.Math.Min((float)(single9 + single1), to.Y);
                                    while (num4 < num1)
                                    {
                                        float single11 = singleArray1[num4];
                                        if (single11 >= (single10 + single1))
                                        {
                                            break;
                                        }
                                        num4++;
                                        single10 = System.Math.Min((float)(single11 + single2), to.Y);
                                    }
                                    PointF tf6 = new PointF(to.X - single1, (single9 + single10) / 2f);
                                    PointF tf7 = new PointF(to.X, single10);
                                    path.AddBezier(tf1.X, tf1.Y, tf6.X, tf1.Y, tf6.X, tf7.Y, tf7.X, tf7.Y);
                                    tf1 = tf7;
                                }
                            }
                            else
                            {
                                int num5 = num1 - 1;
                                while (num5 >= 0)
                                {
                                    float single12 = System.Math.Min(from.Y, System.Math.Max((float)(singleArray1[num5--] + single2), (float)(to.Y + single1)));
                                    path.AddLine((float)(tf1.X + offx), (float)(tf1.Y + offy), (float)(to.X + offx), (float)(single12 + offy));
                                    tf1 = new PointF(to.X + offx, single12 + offy);
                                    float single13 = System.Math.Max((float)(single12 - single1), to.Y);
                                    while (num5 >= 0)
                                    {
                                        float single14 = singleArray1[num5];
                                        if (single14 <= (single13 - single1))
                                        {
                                            break;
                                        }
                                        num5--;
                                        single13 = System.Math.Max((float)(single14 - single2), to.Y);
                                    }
                                    PointF tf8 = new PointF(to.X - single1, (single12 + single13) / 2f);
                                    PointF tf9 = new PointF(to.X, single13);
                                    path.AddBezier(tf1.X, tf1.Y, tf8.X, tf1.Y, tf8.X, tf9.Y, tf9.X, tf9.Y);
                                    tf1 = tf9;
                                }
                            }
                        }
                    }
                    path.AddLine((float)(tf1.X + offx), (float)(tf1.Y + offy), (float)(to.X + offx), (float)(to.Y + offy));
                }
            }
        }

        private PointF addLineAndCorner(GraphicsPath path, float offx, float offy, PointF a, PointF b, PointF c)
        {
            if (base.IsApprox(a.Y, b.Y) && base.IsApprox(b.X, c.X))
            {
                float single4;
                float single1 = System.Math.Abs(this.Curviness);
                float single2 = System.Math.Min(single1, (float)(System.Math.Abs((float)(b.X - a.X)) / 2f));
                float single3 = System.Math.Min(single2, (float)(System.Math.Abs((float)(c.Y - b.Y)) / 2f));
                single2 = single3;
                if (base.IsApprox(single2, (float)0f))
                {
                    this.addLine(path, offx, offy, a, b);
                    return b;
                }
                PointF tf1 = b;
                PointF tf2 = b;
                float single5 = 90f;
                RectangleF ef1 = new RectangleF(0f, 0f, single2 * 2f, single3 * 2f);
                if (b.X > a.X)
                {
                    tf1.X = b.X - single2;
                    if (c.Y > b.Y)
                    {
                        tf2.Y = b.Y + single3;
                        single4 = 270f;
                        ef1.X = b.X - (single2 * 2f);
                        ef1.Y = b.Y;
                    }
                    else
                    {
                        tf2.Y = b.Y - single3;
                        single4 = 90f;
                        single5 = -90f;
                        ef1.X = b.X - (single2 * 2f);
                        ef1.Y = b.Y - (single3 * 2f);
                    }
                }
                else
                {
                    tf1.X = b.X + single2;
                    if (c.Y > b.Y)
                    {
                        tf2.Y = b.Y + single3;
                        single4 = 270f;
                        single5 = -90f;
                        ef1.X = b.X;
                        ef1.Y = b.Y;
                    }
                    else
                    {
                        tf2.Y = b.Y - single3;
                        single4 = 90f;
                        ef1.X = b.X;
                        ef1.Y = b.Y - (single3 * 2f);
                    }
                }
                this.addLine(path, offx, offy, a, tf1);
                ef1.X += offx;
                ef1.Y += offy;
                path.AddArc(ef1, single4, single5);
                return tf2;
            }
            if (base.IsApprox(a.X, b.X) && base.IsApprox(b.Y, c.Y))
            {
                float single9;
                float single6 = System.Math.Abs(this.Curviness);
                float single7 = System.Math.Min(single6, (float)(System.Math.Abs((float)(b.Y - a.Y)) / 2f));
                float single8 = System.Math.Min(single7, (float)(System.Math.Abs((float)(c.X - b.X)) / 2f));
                single7 = single8;
                if (base.IsApprox(single8, (float)0f))
                {
                    this.addLine(path, offx, offy, a, b);
                    return b;
                }
                PointF tf3 = b;
                PointF tf4 = b;
                float single10 = 90f;
                RectangleF ef2 = new RectangleF(0f, 0f, single8 * 2f, single7 * 2f);
                if (b.Y > a.Y)
                {
                    tf3.Y = b.Y - single7;
                    if (c.X > b.X)
                    {
                        tf4.X = b.X + single8;
                        single9 = 180f;
                        single10 = -90f;
                        ef2.Y = b.Y - (single7 * 2f);
                        ef2.X = b.X;
                    }
                    else
                    {
                        tf4.X = b.X - single8;
                        single9 = 0f;
                        ef2.Y = b.Y - (single7 * 2f);
                        ef2.X = b.X - (single8 * 2f);
                    }
                }
                else
                {
                    tf3.Y = b.Y + single7;
                    if (c.X > b.X)
                    {
                        tf4.X = b.X + single8;
                        single9 = 180f;
                        ef2.Y = b.Y;
                        ef2.X = b.X;
                    }
                    else
                    {
                        tf4.X = b.X - single8;
                        single9 = 0f;
                        single10 = -90f;
                        ef2.Y = b.Y;
                        ef2.X = b.X - (single8 * 2f);
                    }
                }
                this.addLine(path, offx, offy, a, tf3);
                ef2.X += offx;
                ef2.Y += offy;
                path.AddArc(ef2, single9, single10);
                return tf4;
            }
            this.addLine(path, offx, offy, a, b);
            return b;
        }

        public virtual int AddPoint(PointF p)
        {
            base.ResetPath();
            int num1 = this.myPoints.Length;
            if (this.myPointsCount >= num1)
            {
                PointF[] tfArray1 = new PointF[System.Math.Max((int)(num1 * 2), (int)(this.myPointsCount + 1))];
                Array.Copy(this.myPoints, 0, tfArray1, 0, num1);
                this.myPoints = tfArray1;
            }
            int num2 = this.myPointsCount++;
            this.myPoints[num2] = p;
            base.InvalidBounds = true;
            this.Changed(0x4b1, num2, null, DiagramShape.MakeRect(p), num2, null, DiagramShape.MakeRect(p));
            return num2;
        }

        public int AddPoint(float x, float y)
        {
            return this.AddPoint(new PointF(x, y));
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            sel.RemoveHandles(this);
            if (this.HighlightWhenSelected)
            {
                bool flag1 = base.SkipsUndoManager;
                base.SkipsUndoManager = true;
                this.Highlight = true;
                base.SkipsUndoManager = flag1;
            }
            else
            {
                int num1 = this.LastPickIndex;
                if (this.CanResize())
                {
                    if (this.CanReshape())
                    {
                        for (int num2 = this.FirstPickIndex; num2 <= num1; num2++)
                        {
                            PointF tf1 = this.GetPoint(num2);
                            sel.CreateResizeHandle(this, selectedObj, tf1, 0x2000 + num2, true);
                        }
                    }
                    else
                    {
                        base.AddSelectionHandles(sel, selectedObj);
                    }
                }
                else
                {
                    for (int num3 = this.FirstPickIndex; num3 <= num1; num3++)
                    {
                        PointF tf2 = this.GetPoint(num3);
                        sel.CreateResizeHandle(this, selectedObj, tf2, 0, false);
                    }
                }
            }
        }

        private void addStroke(GraphicsPath path, float offx, float offy, PointF[] fromPoly, PointF[] toPoly)
        {
            int num1 = this.PointsCount;
            if ((this.Style == StrokeGraphStyle.Bezier) && (num1 >= 4))
            {
                for (int num2 = 3; num2 < num1; num2 += 3)
                {
                    PointF tf1;
                    PointF tf4;
                    if (((fromPoly != null) && ((num2 - 3) == 0)) && ((this.FromArrowShaftLength > 0f) && (fromPoly[2] == this.GetPoint(0))))
                    {
                        tf1 = fromPoly[0];
                    }
                    else
                    {
                        tf1 = this.GetPoint(num2 - 3);
                    }
                    PointF tf2 = this.GetPoint(num2 - 2);
                    if ((num2 + 3) >= num1)
                    {
                        num2 = num1 - 1;
                    }
                    PointF tf3 = this.GetPoint(num2 - 1);
                    if (((toPoly != null) && (num2 == (num1 - 1))) && ((this.ToArrowShaftLength > 0f) && (toPoly[2] == this.GetPoint(num2))))
                    {
                        tf4 = toPoly[0];
                    }
                    else
                    {
                        tf4 = this.GetPoint(num2);
                    }
                    path.AddBezier((float)(tf1.X + offx), (float)(tf1.Y + offy), (float)(tf2.X + offx), (float)(tf2.Y + offy), (float)(tf3.X + offx), (float)(tf3.Y + offy), (float)(tf4.X + offx), (float)(tf4.Y + offy));
                }
            }
            else if (num1 >= 2)
            {
                if (((num1 == 2) || (this.Style == StrokeGraphStyle.Line)) || ((this.Style == StrokeGraphStyle.Bezier) || (base.IsApprox(this.Curviness, (float)0f) && (this.Style != StrokeGraphStyle.RoundedLineWithJumpOvers))))
                {
                    for (int num3 = 1; num3 < num1; num3++)
                    {
                        PointF tf5;
                        PointF tf6;
                        if (((fromPoly != null) && ((num3 - 1) == 0)) && ((this.FromArrowShaftLength > 0f) && (fromPoly[2] == this.GetPoint(0))))
                        {
                            tf5 = fromPoly[0];
                        }
                        else
                        {
                            tf5 = this.GetPoint(num3 - 1);
                        }
                        if (((toPoly != null) && (num3 == (num1 - 1))) && ((this.ToArrowShaftLength > 0f) && (toPoly[2] == this.GetPoint(num3))))
                        {
                            tf6 = toPoly[0];
                        }
                        else
                        {
                            tf6 = this.GetPoint(num3);
                        }
                        path.AddLine((float)(tf5.X + offx), (float)(tf5.Y + offy), (float)(tf6.X + offx), (float)(tf6.Y + offy));
                    }
                }
                else
                {
                    int num5;
                    PointF tf7 = this.GetPoint(0);
                    if (((fromPoly != null) && (this.FromArrowShaftLength > 0f)) && (fromPoly[2] == tf7))
                    {
                        tf7 = fromPoly[0];
                    }
                    for (int num4 = 1; num4 < num1; num4 = num5)
                    {
                        num4 = this.furthestPoint(tf7, num4, num4 > 1);
                        PointF tf8 = this.GetPoint(num4);
                        if (num4 >= (num1 - 1))
                        {
                            if (((toPoly != null) && (this.ToArrowShaftLength > 0f)) && (toPoly[2] == tf8))
                            {
                                tf8 = toPoly[0];
                            }
                            if (tf7 != tf8)
                            {
                                this.addLine(path, offx, offy, tf7, tf8);
                            }
                            return;
                        }
                        num5 = this.furthestPoint(tf8, num4 + 1, num4 < (num1 - 3));
                        PointF tf9 = this.GetPoint(num5);
                        if (((toPoly != null) && (num5 == (num1 - 1))) && ((this.ToArrowShaftLength > 0f) && (toPoly[2] == tf9)))
                        {
                            tf9 = toPoly[0];
                        }
                        tf7 = this.addLineAndCorner(path, offx, offy, tf7, tf8, tf9);
                    }
                }
            }
        }

        internal static RectangleF BezierBounds(PointF b0, PointF b1, PointF b2, PointF b3)
        {
            PointF tf1 = b0;
            PointF tf2 = new PointF((b0.X + b1.X) / 2f, (b0.Y + b1.Y) / 2f);
            PointF tf3 = new PointF((b1.X + b2.X) / 2f, (b1.Y + b2.Y) / 2f);
            PointF tf4 = new PointF((b2.X + b3.X) / 2f, (b2.Y + b3.Y) / 2f);
            PointF tf5 = b3;
            PointF tf6 = tf1;
            PointF tf7 = new PointF((tf1.X + tf2.X) / 2f, (tf1.Y + tf2.Y) / 2f);
            PointF tf8 = new PointF((tf2.X + tf3.X) / 2f, (tf2.Y + tf3.Y) / 2f);
            PointF tf9 = new PointF((tf3.X + tf4.X) / 2f, (tf3.Y + tf4.Y) / 2f);
            PointF tf10 = new PointF((tf4.X + tf5.X) / 2f, (tf4.Y + tf5.Y) / 2f);
            PointF tf11 = tf5;
            float single1 = tf6.X;
            float single2 = tf6.X;
            if (tf7.X < single1)
            {
                single1 = tf7.X;
            }
            else if (tf7.X > single2)
            {
                single2 = tf7.X;
            }
            if (tf8.X < single1)
            {
                single1 = tf8.X;
            }
            else if (tf8.X > single2)
            {
                single2 = tf8.X;
            }
            if (tf9.X < single1)
            {
                single1 = tf9.X;
            }
            else if (tf9.X > single2)
            {
                single2 = tf9.X;
            }
            if (tf10.X < single1)
            {
                single1 = tf10.X;
            }
            else if (tf10.X > single2)
            {
                single2 = tf10.X;
            }
            if (tf11.X < single1)
            {
                single1 = tf11.X;
            }
            else if (tf11.X > single2)
            {
                single2 = tf11.X;
            }
            float single3 = tf6.Y;
            float single4 = tf6.Y;
            if (tf7.Y < single3)
            {
                single3 = tf7.Y;
            }
            else if (tf7.Y > single4)
            {
                single4 = tf7.Y;
            }
            if (tf8.Y < single3)
            {
                single3 = tf8.Y;
            }
            else if (tf8.Y > single4)
            {
                single4 = tf8.Y;
            }
            if (tf9.Y < single3)
            {
                single3 = tf9.Y;
            }
            else if (tf9.Y > single4)
            {
                single4 = tf9.Y;
            }
            if (tf10.Y < single3)
            {
                single3 = tf10.Y;
            }
            else if (tf10.Y > single4)
            {
                single4 = tf10.Y;
            }
            if (tf11.Y < single3)
            {
                single3 = tf11.Y;
            }
            else if (tf11.Y > single4)
            {
                single4 = tf11.Y;
            }
            return new RectangleF(single1 - 10f, single3 - 10f, (single2 - single1) + 20f, (single4 - single3) + 20f);
        }

        internal static bool BezierContainsPoint(PointF b0, PointF b1, PointF b2, PointF b3, float fuzz, PointF p)
        {
            PointF tf1 = b0;
            PointF tf2 = new PointF((b0.X + b1.X) / 2f, (b0.Y + b1.Y) / 2f);
            PointF tf3 = new PointF((b1.X + b2.X) / 2f, (b1.Y + b2.Y) / 2f);
            PointF tf4 = new PointF((b2.X + b3.X) / 2f, (b2.Y + b3.Y) / 2f);
            PointF tf5 = b3;
            PointF tf6 = tf1;
            PointF tf7 = new PointF((tf1.X + tf2.X) / 2f, (tf1.Y + tf2.Y) / 2f);
            PointF tf8 = new PointF((tf2.X + tf3.X) / 2f, (tf2.Y + tf3.Y) / 2f);
            PointF tf9 = new PointF((tf3.X + tf4.X) / 2f, (tf3.Y + tf4.Y) / 2f);
            PointF tf10 = new PointF((tf4.X + tf5.X) / 2f, (tf4.Y + tf5.Y) / 2f);
            PointF tf11 = tf5;
            if (StrokeGraph.LineContainsPoint(tf6, tf7, fuzz, p))
            {
                return true;
            }
            if (StrokeGraph.LineContainsPoint(tf7, tf8, fuzz, p))
            {
                return true;
            }
            if (StrokeGraph.LineContainsPoint(tf8, tf9, fuzz, p))
            {
                return true;
            }
            if (StrokeGraph.LineContainsPoint(tf9, tf10, fuzz, p))
            {
                return true;
            }
            if (StrokeGraph.LineContainsPoint(tf10, tf11, fuzz, p))
            {
                return true;
            }
            return false;
        }

        internal static void BezierMidPoint(PointF b0, PointF b1, PointF b2, PointF b3, out PointF v, out PointF w)
        {
            PointF tf1 = new PointF((b0.X + b1.X) / 2f, (b0.Y + b1.Y) / 2f);
            PointF tf2 = new PointF((b1.X + b2.X) / 2f, (b1.Y + b2.Y) / 2f);
            PointF tf3 = new PointF((b2.X + b3.X) / 2f, (b2.Y + b3.Y) / 2f);
            v = new PointF((tf1.X + tf2.X) / 2f, (tf1.Y + tf2.Y) / 2f);
            w = new PointF((tf2.X + tf3.X) / 2f, (tf2.Y + tf3.Y) / 2f);
        }

        internal static bool BezierNearestIntersectionOnLine(PointF b0, PointF b1, PointF b2, PointF b3, PointF p1, PointF p2, out PointF result)
        {
            PointF tf12;
            PointF tf1 = b0;
            PointF tf2 = new PointF((b0.X + b1.X) / 2f, (b0.Y + b1.Y) / 2f);
            PointF tf3 = new PointF((b1.X + b2.X) / 2f, (b1.Y + b2.Y) / 2f);
            PointF tf4 = new PointF((b2.X + b3.X) / 2f, (b2.Y + b3.Y) / 2f);
            PointF tf5 = b3;
            PointF tf6 = tf1;
            PointF tf7 = new PointF((tf1.X + tf2.X) / 2f, (tf1.Y + tf2.Y) / 2f);
            PointF tf8 = new PointF((tf2.X + tf3.X) / 2f, (tf2.Y + tf3.Y) / 2f);
            PointF tf9 = new PointF((tf3.X + tf4.X) / 2f, (tf3.Y + tf4.Y) / 2f);
            PointF tf10 = new PointF((tf4.X + tf5.X) / 2f, (tf4.Y + tf5.Y) / 2f);
            PointF tf11 = tf5;
            float single1 = 1E+21f;
            PointF tf13 = new PointF();
            if (StrokeGraph.NearestIntersectionOnLine(tf6, tf7, p1, p2, out tf12))
            {
                float single2 = ((tf12.X - p1.X) * (tf12.X - p1.X)) + ((tf12.Y - p1.Y) * (tf12.Y - p1.Y));
                if (single2 < single1)
                {
                    single1 = single2;
                    tf13 = tf12;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf7, tf8, p1, p2, out tf12))
            {
                float single3 = ((tf12.X - p1.X) * (tf12.X - p1.X)) + ((tf12.Y - p1.Y) * (tf12.Y - p1.Y));
                if (single3 < single1)
                {
                    single1 = single3;
                    tf13 = tf12;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf8, tf9, p1, p2, out tf12))
            {
                float single4 = ((tf12.X - p1.X) * (tf12.X - p1.X)) + ((tf12.Y - p1.Y) * (tf12.Y - p1.Y));
                if (single4 < single1)
                {
                    single1 = single4;
                    tf13 = tf12;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf9, tf10, p1, p2, out tf12))
            {
                float single5 = ((tf12.X - p1.X) * (tf12.X - p1.X)) + ((tf12.Y - p1.Y) * (tf12.Y - p1.Y));
                if (single5 < single1)
                {
                    single1 = single5;
                    tf13 = tf12;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf10, tf11, p1, p2, out tf12))
            {
                float single6 = ((tf12.X - p1.X) * (tf12.X - p1.X)) + ((tf12.Y - p1.Y) * (tf12.Y - p1.Y));
                if (single6 < single1)
                {
                    single1 = single6;
                    tf13 = tf12;
                }
            }
            result = tf13;
            return (single1 < 1E+21f);
        }

        public virtual void CalculateArrowhead(PointF anchor, PointF endPoint, bool atEnd, PointF[] poly)
        {
            float single1 = endPoint.X;
            float single2 = endPoint.Y;
            float single3 = single1 - anchor.X;
            float single4 = single2 - anchor.Y;
            float single5 = (float)System.Math.Sqrt((double)((single3 * single3) + (single4 * single4)));
            if (base.IsApprox(single5, (float)0f))
            {
                poly[0].X = single1;
                poly[0].Y = single2;
                poly[1].X = single1;
                poly[1].Y = single2;
                poly[2].X = single1;
                poly[2].Y = single2;
                poly[3].X = single1;
                poly[3].Y = single2;
            }
            else
            {
                float single8;
                float single9;
                float single10;
                float single6 = single3 / single5;
                float single7 = single4 / single5;
                if (atEnd)
                {
                    single8 = this.ToArrowLength;
                    single9 = this.ToArrowShaftLength;
                    single10 = this.ToArrowWidth;
                }
                else
                {
                    single8 = this.FromArrowLength;
                    single9 = this.FromArrowShaftLength;
                    single10 = this.FromArrowWidth;
                }
                single10 /= 2f;
                float single11 = System.Math.Max(single8, single9);
                if (((single11 > 0f) && (single5 < single11)) && (this.Style != StrokeGraphStyle.Bezier))
                {
                    float single12 = single5 / single11;
                    single8 *= single12;
                    single9 *= single12;
                    single10 *= single12;
                }
                float single13 = -single9;
                float single14 = 0f;
                float single15 = -single8;
                float single16 = -single10;
                float single17 = -single8;
                float single18 = single10;
                poly[0].X = single1 + ((single6 * single13) - (single7 * single14));
                poly[0].Y = single2 + ((single7 * single13) + (single6 * single14));
                poly[1].X = single1 + ((single6 * single15) - (single7 * single16));
                poly[1].Y = single2 + ((single7 * single15) + (single6 * single16));
                poly[2].X = single1;
                poly[2].Y = single2;
                poly[3].X = single1 + ((single6 * single17) - (single7 * single18));
                poly[3].Y = single2 + ((single7 * single17) + (single6 * single18));
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x4b1:
                    {
                        if (!undo)
                        {
                            this.InsertPoint(e.OldInt, new PointF(e.NewRect.X, e.NewRect.Y));
                            return;
                        }
                        this.RemovePoint(e.OldInt);
                        return;
                    }
                case 0x4b2:
                    {
                        if (!undo)
                        {
                            this.RemovePoint(e.OldInt);
                            return;
                        }
                        this.InsertPoint(e.OldInt, new PointF(e.OldRect.X, e.OldRect.Y));
                        return;
                    }
                case 0x4b3:
                    {
                        if (!undo)
                        {
                            this.SetPoint(e.OldInt, new PointF(e.NewRect.X, e.NewRect.Y));
                            return;
                        }
                        this.SetPoint(e.OldInt, new PointF(e.OldRect.X, e.OldRect.Y));
                        return;
                    }
                case 0x4b4:
                    {
                        PointF[] tfArray1 = (PointF[])e.GetValue(undo);
                        this.SetPoints(tfArray1);
                        return;
                    }
                case 0x4b5:
                    {
                        this.Style = (StrokeGraphStyle)e.GetValue(undo);
                        return;
                    }
                case 0x4b6:
                    {
                        this.Curviness = e.GetFloat(undo);
                        return;
                    }
                case 0x4d4:
                    {
                        object obj1 = e.GetValue(undo);
                        if (!(obj1 is Pen))
                        {
                            if (obj1 is DiagramGraph.GoPenInfo)
                            {
                                this.HighlightPen = ((DiagramGraph.GoPenInfo)obj1).GetPen();
                            }
                            return;
                        }
                        this.HighlightPen = (Pen)obj1;
                        return;
                    }
                case 0x4d5:
                    {
                        this.Highlight = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x4d6:
                    {
                        this.HighlightWhenSelected = (bool)e.GetValue(undo);
                        return;
                    }
                case 1250:
                    {
                        this.ToArrow = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x4e3:
                    {
                        this.ToArrowLength = e.GetFloat(undo);
                        return;
                    }
                case 0x4e4:
                    {
                        this.ToArrowShaftLength = e.GetFloat(undo);
                        return;
                    }
                case 0x4e5:
                    {
                        this.ToArrowWidth = e.GetFloat(undo);
                        return;
                    }
                case 0x4e6:
                    {
                        this.ToArrowFilled = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x4e7:
                    {
                        this.ToArrowStyle = (StrokeArrowheadStyle)e.GetValue(undo);
                        return;
                    }
                case 1260:
                    {
                        this.FromArrow = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x4ed:
                    {
                        this.FromArrowLength = e.GetFloat(undo);
                        return;
                    }
                case 0x4ee:
                    {
                        this.FromArrowShaftLength = e.GetFloat(undo);
                        return;
                    }
                case 0x4ef:
                    {
                        this.FromArrowWidth = e.GetFloat(undo);
                        return;
                    }
                case 0x4f0:
                    {
                        this.FromArrowFilled = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x4f1:
                    {
                        this.FromArrowStyle = (StrokeArrowheadStyle)e.GetValue(undo);
                        return;
                    }
                case 0x4f3:
                    {
                        this.HighlightPenWidth = e.GetFloat(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        public virtual void ClearPoints()
        {
            this.Changing(0x4b4);
            base.ResetPath();
            this.myPointsCount = 0;
            base.InvalidBounds = true;
            this.Changed(0x4b4, 0, null, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
        }

        protected override RectangleF ComputeBounds()
        {
            int num1 = this.PointsCount;
            if (num1 <= 0)
            {
                PointF tf1 = base.Position;
                return new RectangleF(tf1.X, tf1.Y, 0f, 0f);
            }
            PointF tf2 = this.GetPoint(0);
            float single1 = tf2.X;
            float single2 = tf2.Y;
            float single3 = tf2.X;
            float single4 = tf2.Y;
            if ((this.Style == StrokeGraphStyle.Bezier) && (num1 >= 4))
            {
                for (int num2 = 3; num2 < num1; num2 += 3)
                {
                    PointF tf3 = this.GetPoint(num2 - 3);
                    PointF tf4 = this.GetPoint(num2 - 2);
                    if ((num2 + 3) >= num1)
                    {
                        num2 = num1 - 1;
                    }
                    PointF tf5 = this.GetPoint(num2 - 1);
                    PointF tf6 = this.GetPoint(num2);
                    RectangleF ef1 = StrokeGraph.BezierBounds(tf3, tf4, tf5, tf6);
                    single1 = System.Math.Min(single1, ef1.X);
                    single2 = System.Math.Min(single2, ef1.Y);
                    single3 = System.Math.Max(single3, (float)(ef1.X + ef1.Width));
                    single4 = System.Math.Max(single4, (float)(ef1.Y + ef1.Height));
                }
            }
            else
            {
                for (int num3 = 1; num3 < num1; num3++)
                {
                    tf2 = this.GetPoint(num3);
                    single1 = System.Math.Min(single1, tf2.X);
                    single2 = System.Math.Min(single2, tf2.Y);
                    single3 = System.Math.Max(single3, tf2.X);
                    single4 = System.Math.Max(single4, tf2.Y);
                }
            }
            return new RectangleF(single1, single2, single3 - single1, single4 - single2);
        }

        public override bool ContainsPoint(PointF p)
        {
            int num1 = this.GetSegmentNearPoint(p);
            return (num1 >= 0);
        }

        public override void CopyNewValueForRedo(ChangedEventArgs e)
        {
            if (e.SubHint == 0x4b4)
            {
                PointF[] tfArray1 = this.CopyPointsArray();
                e.NewValue = tfArray1;
            }
            else
            {
                base.CopyNewValueForRedo(e);
            }
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            StrokeGraph stroke1 = (StrokeGraph)base.CopyObject(env);
            if (stroke1 != null)
            {
                stroke1.myPoints = (PointF[])this.myPoints.Clone();
                if (this.myToArrowInfo != null)
                {
                    stroke1.myToArrowInfo = (ArrowInfo)this.myToArrowInfo.Clone();
                }
                if (this.myFromArrowInfo != null)
                {
                    stroke1.myFromArrowInfo = (ArrowInfo)this.myFromArrowInfo.Clone();
                }
            }
            return stroke1;
        }

        public override void CopyOldValueForUndo(ChangedEventArgs e)
        {
            if (e.SubHint == 0x4b4)
            {
                if (!e.IsBeforeChanging)
                {
                    ChangedEventArgs args1 = e.FindBeforeChangingEdit();
                    if (args1 != null)
                    {
                        e.OldValue = args1.NewValue;
                    }
                }
            }
            else
            {
                base.CopyOldValueForUndo(e);
            }
        }

        [Category("Appearance"), Description("A copy of the array of points in this stroke.")]
        public virtual PointF[] CopyPointsArray()
        {
            PointF[] tfArray1 = new PointF[this.myPointsCount];
            Array.Copy(this.myPoints, 0, tfArray1, 0, this.myPointsCount);
            return tfArray1;
        }

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            if ((whichHandle >= 0x2000) && ((this.ResizesRealtime || (evttype == InputState.Finish)) || (evttype == InputState.Cancel)))
            {
                this.SetPoint(whichHandle - 0x2000, newPoint);
            }
            else
            {
                base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
            }
        }

        protected virtual void DrawArrowhead(Graphics g, DiagramView view, Pen pen, Brush brush, bool atEnd, float offsetw, float offseth, PointF[] poly)
        {
            Brush brush1 = null;
            if ((poly[0] != poly[2]) && (atEnd ? this.ToArrowFilled : this.FromArrowFilled))
            {
                brush1 = brush;
            }
            switch ((atEnd ? this.ToArrowStyle : this.FromArrowStyle))
            {
                case StrokeArrowheadStyle.Polygon:
                    {
                        if ((offsetw == 0f) && (offseth == 0f))
                        {
                            DiagramGraph.DrawPolygon(g, view, pen, brush1, poly);
                            return;
                        }
                        int num1 = poly.Length;
                        for (int num2 = 0; num2 < num1; num2++)
                        {
                            poly[num2].X += offsetw;
                            poly[num2].Y += offseth;
                        }
                        DiagramGraph.DrawPolygon(g, view, pen, brush1, poly);
                        for (int num3 = 0; num3 < num1; num3++)
                        {
                            poly[num3].X -= offsetw;
                            poly[num3].Y -= offseth;
                        }
                        return;
                    }
                case StrokeArrowheadStyle.Circle:
                    {
                        float single1 = poly[0].X;
                        float single2 = poly[0].Y;
                        float single3 = poly[2].X;
                        float single4 = poly[2].Y;
                        float single5 = ((single1 + single3) / 2f) + offsetw;
                        float single6 = ((single2 + single4) / 2f) + offseth;
                        float single7 = (float)System.Math.Sqrt((double)(((single1 - single3) * (single1 - single3)) + ((single2 - single4) * (single2 - single4))));
                        DiagramGraph.DrawEllipse(g, view, pen, brush1, single5 - (single7 / 2f), single6 - (single7 / 2f), single7, single7);
                        return;
                    }
                case StrokeArrowheadStyle.Cross:
                    {
                        float single8 = poly[1].X + offsetw;
                        float single9 = poly[1].Y + offseth;
                        float single10 = poly[3].X + offsetw;
                        float single11 = poly[3].Y + offseth;
                        DiagramGraph.DrawLine(g, view, pen, single8, single9, single10, single11);
                        return;
                    }
            }
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            if (this.Pen != null)
            {
                float single1 = ((System.Math.Max(base.PenInfo.Width, (float)1f) / 2f) * base.PenInfo.MiterLimit) + 1f;
                if (this.HighlightPen != null)
                {
                    float single2 = ((System.Math.Max(this.HighlightPenInfo.Width, (float)1f) / 2f) * this.HighlightPenInfo.MiterLimit) + 1f;
                    single1 = System.Math.Max(single1, single2);
                }
                if (this.ToArrow)
                {
                    single1 = System.Math.Max(single1, this.ToArrowLength);
                    single1 = System.Math.Max(single1, this.ToArrowWidth);
                }
                if (this.FromArrow)
                {
                    single1 = System.Math.Max(single1, this.FromArrowLength);
                    single1 = System.Math.Max(single1, this.FromArrowWidth);
                }
                DiagramShape.InflateRect(ref rect, single1, single1);
                if (!this.Shadowed)
                {
                    return rect;
                }
                SizeF ef1 = this.GetShadowOffset(view);
                if (ef1.Width < 0f)
                {
                    rect.X += ef1.Width;
                    rect.Width -= ef1.Width;
                }
                else
                {
                    rect.Width += ef1.Width;
                }
                if (ef1.Height < 0f)
                {
                    rect.Y += ef1.Height;
                    rect.Height -= ef1.Height;
                    return rect;
                }
                rect.Height += ef1.Height;
            }
            return rect;
        }

        private int furthestPoint(PointF a, int i, bool oneway)
        {
            int num1 = this.PointsCount;
            PointF tf1 = a;
            while (base.IsApprox(a.X, tf1.X) && base.IsApprox(a.Y, tf1.Y))
            {
                if (i >= num1)
                {
                    return (num1 - 1);
                }
                tf1 = this.GetPoint(i++);
            }
            if (!base.IsApprox(a.X, tf1.X) && !base.IsApprox(a.Y, tf1.Y))
            {
                return (i - 1);
            }
            for (PointF tf2 = tf1; ((base.IsApprox(a.X, tf1.X) && base.IsApprox(tf1.X, tf2.X)) && (!oneway || ((a.Y >= tf1.Y) ? (tf1.Y >= tf2.Y) : (tf1.Y <= tf2.Y)))) || ((base.IsApprox(a.Y, tf1.Y) && base.IsApprox(tf1.Y, tf2.Y)) && (!oneway || ((a.X >= tf1.X) ? (tf1.X >= tf2.X) : (tf1.X <= tf2.X)))); tf2 = this.GetPoint(i++))
            {
                if (i >= num1)
                {
                    return (num1 - 1);
                }
            }
            return (i - 2);
        }

        public static float GetAngle(float x, float y)
        {
            if (x == 0f)
            {
                if (y > 0f)
                {
                    return 90f;
                }
                return 270f;
            }
            if (y == 0f)
            {
                if (x > 0f)
                {
                    return 0f;
                }
                return 180f;
            }
            float single1 = (float)((System.Math.Atan((double)System.Math.Abs((float)(y / x))) * 180) / 3.1415926535897931);
            if (x < 0f)
            {
                if (y < 0f)
                {
                    return (single1 + 180f);
                }
                return (180f - single1);
            }
            if (y < 0f)
            {
                single1 = 360f - single1;
            }
            return single1;
        }

        public virtual int GetArrowheadPointsCount(bool atEnd)
        {
            return 4;
        }

        private int getIntersections(PointF A, PointF B, float[] v)
        {
            DiagramDocument document1 = base.Document;
            if (document1 == null)
            {
                return 0;
            }
            float single1 = System.Math.Min(A.X, B.X);
            float single2 = System.Math.Min(A.Y, B.Y);
            float single3 = System.Math.Max(A.X, B.X);
            float single4 = System.Math.Max(A.Y, B.Y);
            RectangleF ef1 = new RectangleF(single1, single2, single3 - single1, single4 - single2);
            int num1 = 0;
            LayerCollectionEnumerator enumerator1 = document1.Layers.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramLayer layer1 = enumerator1.Current;
                if (layer1.CanViewObjects())
                {
                    DiagramLayer.GoLayerCache cache1 = layer1.FindCache(ef1);
                    if (cache1 != null)
                    {
                        ArrayList list1 = null;
                        foreach (StrokeGraph stroke1 in cache1.Strokes)
                        {
                            if (stroke1.Layer == null)
                            {
                                if (list1 == null)
                                {
                                    list1 = new ArrayList();
                                }
                                list1.Add(stroke1);
                                continue;
                            }
                            if (stroke1 == this)
                            {
                                if (list1 != null)
                                {
                                    foreach (DiagramShape obj1 in list1)
                                    {
                                        DiagramShapeCollection.fastRemove(cache1.Strokes, obj1);
                                    }
                                }
                                Array.Sort(v, 0, num1, Comparer.Default);
                                return num1;
                            }
                            num1 = this.getIntersections2(A, B, v, num1, stroke1);
                        }
                        if (list1 == null)
                        {
                            continue;
                        }
                        foreach (DiagramShape obj2 in list1)
                        {
                            DiagramShapeCollection.fastRemove(cache1.Strokes, obj2);
                        }
                        continue;
                    }
                    LayerEnumerator enumerator4 = layer1.GetEnumerator();
                    while (enumerator4.MoveNext())
                    {
                        DiagramShape obj3 = enumerator4.Current;
                        StrokeGraph stroke2 = obj3 as StrokeGraph;
                        if (stroke2 == null)
                        {
                            TextLine link1 = obj3 as TextLine;
                            if (link1 == null)
                            {
                                continue;
                            }
                            stroke2 = link1.RealLink;
                        }
                        if ((stroke2.Style == StrokeGraphStyle.RoundedLineWithJumpOvers) && stroke2.CanView())
                        {
                            if (stroke2 == this)
                            {
                                Array.Sort(v, 0, num1, Comparer.Default);
                                return num1;
                            }
                            num1 = this.getIntersections2(A, B, v, num1, stroke2);
                        }
                    }
                }
            }
            Array.Sort(v, 0, num1, Comparer.Default);
            return num1;
        }

        private int getIntersections2(PointF A, PointF B, float[] v, int numints, StrokeGraph link)
        {
            if (link.CanView())
            {
                int num1 = link.PointsCount;
                for (int num2 = 1; num2 < num1; num2++)
                {
                    PointF tf1 = link.GetPoint(num2 - 1);
                    PointF tf2 = link.GetPoint(num2);
                    PointF tf3 = new PointF();
                    if (this.getOrthoSegmentIntersection(A, B, tf1, tf2, ref tf3) && (numints < v.Length))
                    {
                        if (base.IsApprox(A.Y, B.Y))
                        {
                            v[numints++] = tf3.X;
                        }
                        else
                        {
                            v[numints++] = tf3.Y;
                        }
                    }
                }
            }
            return numints;
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            PointF tf1;
            int num1 = this.PointsCount;
            float single1 = 1E+21f;
            PointF tf2 = new PointF();
            if ((this.Style == StrokeGraphStyle.Bezier) && (num1 >= 4))
            {
                for (int num2 = 3; num2 < num1; num2 += 3)
                {
                    PointF tf3 = this.GetPoint(num2 - 3);
                    PointF tf4 = this.GetPoint(num2 - 2);
                    if ((num2 + 3) >= num1)
                    {
                        num2 = num1 - 1;
                    }
                    PointF tf5 = this.GetPoint(num2 - 1);
                    PointF tf6 = this.GetPoint(num2);
                    if (StrokeGraph.BezierNearestIntersectionOnLine(tf3, tf4, tf5, tf6, p1, p2, out tf1))
                    {
                        float single2 = ((tf1.X - p1.X) * (tf1.X - p1.X)) + ((tf1.Y - p1.Y) * (tf1.Y - p1.Y));
                        if (single2 < single1)
                        {
                            single1 = single2;
                            tf2 = tf1;
                        }
                    }
                }
            }
            else
            {
                for (int num3 = 0; num3 < (num1 - 1); num3++)
                {
                    PointF tf7 = this.GetPoint(num3);
                    PointF tf8 = this.GetPoint(num3 + 1);
                    if (StrokeGraph.NearestIntersectionOnLine(tf7, tf8, p1, p2, out tf1))
                    {
                        float single3 = ((tf1.X - p1.X) * (tf1.X - p1.X)) + ((tf1.Y - p1.Y) * (tf1.Y - p1.Y));
                        if (single3 < single1)
                        {
                            single1 = single3;
                            tf2 = tf1;
                        }
                    }
                }
            }
            result = tf2;
            return (single1 < 1E+21f);
        }

        private bool getOrthoSegmentIntersection(PointF A, PointF B, PointF C, PointF D, ref PointF result)
        {
            if (!base.IsApprox(A.X, B.X))
            {
                if (((!base.IsApprox(C.X, D.X) || (System.Math.Min(A.X, B.X) >= C.X)) || ((System.Math.Max(A.X, B.X) <= C.X) || (System.Math.Min(C.Y, D.Y) >= A.Y))) || (System.Math.Max(C.Y, D.Y) <= A.Y))
                {
                    goto Label_0173;
                }
                result.X = C.X;
                result.Y = A.Y;
                return true;
            }
            if (((base.IsApprox(C.Y, D.Y) && (System.Math.Min(A.Y, B.Y) < C.Y)) && ((System.Math.Max(A.Y, B.Y) > C.Y) && (System.Math.Min(C.X, D.X) < A.X))) && (System.Math.Max(C.X, D.X) > A.X))
            {
                result.X = A.X;
                result.Y = C.Y;
                return true;
            }
        Label_0173:
            result.X = 0f;
            result.Y = 0f;
            return false;
        }

        private GraphicsPath GetPath(float offx, float offy, PointF[] fromPoly, PointF[] toPoly)
        {
            if (((offx != 0f) || (offy != 0f)) || (this.Style == StrokeGraphStyle.RoundedLineWithJumpOvers))
            {
                GraphicsPath path1 = new GraphicsPath();
                this.addStroke(path1, offx, offy, fromPoly, toPoly);
                return path1;
            }
            if (this.myPath == null)
            {
                this.myPath = new GraphicsPath();
                this.addStroke(this.myPath, 0f, 0f, fromPoly, toPoly);
            }
            return this.myPath;
        }

        public virtual PointF GetPoint(int i)
        {
            if ((i < 0) || (i >= this.myPointsCount))
            {
                throw new ArgumentException("StrokeShape.GetPoint given an invalid index");
            }
            return this.myPoints[i];
        }

        public int GetSegmentNearPoint(PointF pnt)
        {
            RectangleF ef1 = this.Bounds;
            float single1 = System.Math.Max(base.InternalPenWidth, (float)1f);
            single1 += this.PickMargin;
            if (((pnt.X >= (ef1.X - single1)) && (pnt.X <= ((ef1.X + ef1.Width) + single1))) && ((pnt.Y >= (ef1.Y - single1)) && (pnt.Y <= ((ef1.Y + ef1.Height) + single1))))
            {
                int num1 = this.PointsCount;
                if (num1 <= 1)
                {
                    return -1;
                }
                single1 -= (this.PickMargin / 2f);
                if ((this.Style == StrokeGraphStyle.Bezier) && (num1 >= 4))
                {
                    single1 *= System.Math.Max((float)1f, (float)(System.Math.Max(ef1.Width, ef1.Height) / 100f));
                    for (int num2 = 3; num2 < num1; num2 += 3)
                    {
                        int num3 = num2;
                        PointF tf1 = this.GetPoint(num2 - 3);
                        PointF tf2 = this.GetPoint(num2 - 2);
                        if ((num2 + 3) >= num1)
                        {
                            num2 = num1 - 1;
                        }
                        PointF tf3 = this.GetPoint(num2 - 1);
                        PointF tf4 = this.GetPoint(num2);
                        if (StrokeGraph.BezierContainsPoint(tf1, tf2, tf3, tf4, single1, pnt))
                        {
                            return num3;
                        }
                    }
                }
                else
                {
                    for (int num4 = 0; num4 < (num1 - 1); num4++)
                    {
                        PointF tf5 = this.GetPoint(num4);
                        PointF tf6 = this.GetPoint(num4 + 1);
                        if (StrokeGraph.LineContainsPoint(tf5, tf6, single1, pnt))
                        {
                            return num4;
                        }
                    }
                }
            }
            return -1;
        }

        public virtual void InsertPoint(int i, PointF p)
        {
            if (i < 0)
            {
                throw new ArgumentException("StrokeShape.InsertPoint given an invalid index, less than zero");
            }
            if (i > this.myPointsCount)
            {
                i = this.myPointsCount;
            }
            base.ResetPath();
            int num1 = this.myPoints.Length;
            if (this.myPointsCount >= num1)
            {
                PointF[] tfArray1 = new PointF[System.Math.Max((int)(num1 * 2), (int)(this.myPointsCount + 1))];
                Array.Copy(this.myPoints, 0, tfArray1, 0, num1);
                this.myPoints = tfArray1;
            }
            if (this.myPointsCount > i)
            {
                Array.Copy(this.myPoints, i, this.myPoints, (int)(i + 1), (int)(this.myPointsCount - i));
            }
            this.myPointsCount++;
            this.myPoints[i] = p;
            base.InvalidBounds = true;
            this.Changed(0x4b1, i, null, DiagramShape.MakeRect(p), i, null, DiagramShape.MakeRect(p));
        }

        internal static bool LineContainsPoint(PointF a, PointF b, float fuzz, PointF p)
        {
            float single1;
            float single2;
            float single3;
            float single4;
            if (a.X < b.X)
            {
                single2 = a.X;
                single1 = b.X;
            }
            else
            {
                single2 = b.X;
                single1 = a.X;
            }
            if (a.Y < b.Y)
            {
                single4 = a.Y;
                single3 = b.Y;
            }
            else
            {
                single4 = b.Y;
                single3 = a.Y;
            }
            if ((((a.X == b.X) && ((a.X - fuzz) <= p.X)) && ((p.X <= (a.X + fuzz)) && (single4 <= p.Y))) && (p.Y <= single3))
            {
                return true;
            }
            if ((((a.Y == b.Y) && ((a.Y - fuzz) <= p.Y)) && ((p.Y <= (a.Y + fuzz)) && (single2 <= p.X))) && (p.X <= single1))
            {
                return true;
            }
            float single5 = single1 + fuzz;
            float single6 = single2 - fuzz;
            if ((single6 <= p.X) && (p.X <= single5))
            {
                float single7 = single3 + fuzz;
                float single8 = single4 - fuzz;
                if ((single8 <= p.Y) && (p.Y <= single7))
                {
                    if ((single5 - single6) > (single7 - single8))
                    {
                        if (System.Math.Abs((float)(a.X - b.X)) > fuzz)
                        {
                            float single9 = (b.Y - a.Y) / (b.X - a.X);
                            float single10 = (single9 * (p.X - a.X)) + a.Y;
                            if (((single10 - fuzz) <= p.Y) && (p.Y <= (single10 + fuzz)))
                            {
                                return true;
                            }
                            goto Label_0237;
                        }
                        return true;
                    }
                    if (System.Math.Abs((float)(a.Y - b.Y)) > fuzz)
                    {
                        float single11 = (b.X - a.X) / (b.Y - a.Y);
                        float single12 = (single11 * (p.Y - a.Y)) + a.X;
                        if (((single12 - fuzz) <= p.X) && (p.X <= (single12 + fuzz)))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        Label_0237:
            return false;
        }

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = this.GetPath(0f, 0f, null, null);
            return (GraphicsPath)path1.Clone();
        }

        public static bool NearestIntersectionOnLine(PointF a, PointF b, PointF p, PointF q, out PointF result)
        {
            float single1 = a.X;
            float single2 = a.Y;
            float single3 = b.X;
            float single4 = b.Y;
            float single5 = p.X;
            float single6 = p.Y;
            float single7 = q.X;
            float single8 = q.Y;
            if (single5 == single7)
            {
                if (single1 == single3)
                {
                    StrokeGraph.NearestPointOnLine(a, b, p, out result);
                    return false;
                }
                float single9 = (single4 - single2) / (single3 - single1);
                float single10 = (single9 * (single5 - single1)) + single2;
                return StrokeGraph.NearestPointOnLine(a, b, new PointF(single5, single10), out result);
            }
            float single11 = (single8 - single6) / (single7 - single5);
            if (single1 == single3)
            {
                float single12 = (single11 * (single1 - single5)) + single6;
                if (single12 < System.Math.Min(single2, single4))
                {
                    result = new PointF(single1, System.Math.Min(single2, single4));
                    return false;
                }
                if (single12 > System.Math.Max(single2, single4))
                {
                    result = new PointF(single1, System.Math.Max(single2, single4));
                    return false;
                }
                result = new PointF(single1, single12);
                return true;
            }
            float single13 = (single4 - single2) / (single3 - single1);
            if (single11 == single13)
            {
                StrokeGraph.NearestPointOnLine(a, b, p, out result);
                return false;
            }
            float single14 = ((((single13 * single1) - (single11 * single5)) + single6) - single2) / (single13 - single11);
            if (single13 == 0f)
            {
                if (single14 < System.Math.Min(single1, single3))
                {
                    result = new PointF(System.Math.Min(single1, single3), single2);
                    return false;
                }
                if (single14 > System.Math.Max(single1, single3))
                {
                    result = new PointF(System.Math.Max(single1, single3), single2);
                    return false;
                }
                result = new PointF(single14, single2);
                return true;
            }
            float single15 = (single13 * (single14 - single1)) + single2;
            return StrokeGraph.NearestPointOnLine(a, b, new PointF(single14, single15), out result);
        }

        public static bool NearestPointOnLine(PointF a, PointF b, PointF p, out PointF result)
        {
            float single1 = a.X;
            float single2 = a.Y;
            float single3 = b.X;
            float single4 = b.Y;
            float single5 = p.X;
            float single6 = p.Y;
            if (single1 == single3)
            {
                float single7;
                float single8;
                if (single2 < single4)
                {
                    single7 = single2;
                    single8 = single4;
                }
                else
                {
                    single7 = single4;
                    single8 = single2;
                }
                float single9 = single6;
                if (single9 < single7)
                {
                    result = new PointF(single1, single7);
                    return false;
                }
                if (single9 > single8)
                {
                    result = new PointF(single1, single8);
                    return false;
                }
                result = new PointF(single1, single9);
                return true;
            }
            if (single2 == single4)
            {
                float single10;
                float single11;
                if (single1 < single3)
                {
                    single10 = single1;
                    single11 = single3;
                }
                else
                {
                    single10 = single3;
                    single11 = single1;
                }
                float single12 = single5;
                if (single12 < single10)
                {
                    result = new PointF(single10, single2);
                    return false;
                }
                if (single12 > single11)
                {
                    result = new PointF(single11, single2);
                    return false;
                }
                result = new PointF(single12, single2);
                return true;
            }
            float single13 = ((single3 - single1) * (single3 - single1)) + ((single4 - single2) * (single4 - single2));
            float single14 = (((single1 - single5) * (single1 - single3)) + ((single2 - single6) * (single2 - single4))) / single13;
            if (single14 < 0f)
            {
                result = a;
                return false;
            }
            if (single14 > 1f)
            {
                result = b;
                return false;
            }
            float single15 = single1 + (single14 * (single3 - single1));
            float single16 = single2 + (single14 * (single4 - single2));
            result = new PointF(single15, single16);
            return true;
        }

        protected override void OnBoundsChanged(RectangleF old)
        {
            base.OnBoundsChanged(old);
            int num1 = this.PointsCount;
            SizeF ef1 = base.Size;
            if ((old.Width == ef1.Width) && (old.Height == ef1.Height))
            {
                RectangleF ef2 = this.Bounds;
                float single1 = ef2.X - old.X;
                float single2 = ef2.Y - old.Y;
                if ((single1 != 0f) || (single2 != 0f))
                {
                    bool flag1 = base.SuspendsUpdates;
                    if (!flag1)
                    {
                        this.Changing(0x4b4);
                    }
                    base.SuspendsUpdates = true;
                    for (int num2 = 0; num2 < num1; num2++)
                    {
                        PointF tf1 = this.GetPoint(num2);
                        float single3 = tf1.X + single1;
                        float single4 = tf1.Y + single2;
                        this.SetPoint(num2, new PointF(single3, single4));
                    }
                    base.InvalidBounds = false;
                    base.SuspendsUpdates = flag1;
                    if (flag1)
                    {
                        return;
                    }
                    this.Changed(0x4b4, 0, null, old, 0, null, ef2);
                }
            }
            else
            {
                RectangleF ef3 = this.Bounds;
                float single5 = 1f;
                if (old.Width != 0f)
                {
                    single5 = ef3.Width / old.Width;
                }
                float single6 = 1f;
                if (old.Height != 0f)
                {
                    single6 = ef3.Height / old.Height;
                }
                bool flag2 = base.SuspendsUpdates;
                if (!flag2)
                {
                    this.Changing(0x4b4);
                }
                base.SuspendsUpdates = true;
                for (int num3 = 0; num3 < num1; num3++)
                {
                    PointF tf2 = this.GetPoint(num3);
                    float single7 = ef3.X + ((tf2.X - old.X) * single5);
                    float single8 = ef3.Y + ((tf2.Y - old.Y) * single6);
                    this.SetPoint(num3, new PointF(single7, single8));
                }
                base.InvalidBounds = false;
                base.SuspendsUpdates = flag2;
                if (!flag2)
                {
                    this.Changed(0x4b4, 0, null, old, 0, null, ef3);
                }
            }
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            Pen pen1 = this.Pen;
            if (pen1 != null)
            {
                Pen pen2 = pen1;
                Brush brush1 = this.Brush;
                int num1 = this.PointsCount;
                PointF[] tfArray1 = null;
                PointF[] tfArray2 = null;
                if (this.FromArrow && (num1 >= 2))
                {
                    if (this.myFromArrowInfo == null)
                    {
                        this.myFromArrowInfo = new ArrowInfo();
                    }
                    tfArray1 = this.myFromArrowInfo.GetPoly(this.GetArrowheadPointsCount(false));
                    PointF tf1 = this.FromArrowAnchorPoint;
                    PointF tf2 = this.FromArrowEndPoint;
                    this.CalculateArrowhead(tf1, tf2, false, tfArray1);
                }
                if (this.ToArrow && (num1 >= 2))
                {
                    if (this.myToArrowInfo == null)
                    {
                        this.myToArrowInfo = new ArrowInfo();
                    }
                    tfArray2 = this.myToArrowInfo.GetPoly(this.GetArrowheadPointsCount(true));
                    PointF tf3 = this.ToArrowAnchorPoint;
                    PointF tf4 = this.ToArrowEndPoint;
                    this.CalculateArrowhead(tf3, tf4, true, tfArray2);
                }
                if (this.Shadowed && (this.Pen != null))
                {
                    SizeF ef1 = this.GetShadowOffset(view);
                    Pen pen3 = this.GetShadowPen(view, base.InternalPenWidth);
                    if (pen3 != null)
                    {
                        GraphicsPath path1 = this.GetPath(ef1.Width, ef1.Height, tfArray1, tfArray2);
                        DiagramGraph.DrawPath(g, view, pen3, null, path1);
                        base.DisposePath(path1);
                    }
                    Brush brush2 = this.GetShadowBrush(view);
                    if (tfArray1 != null)
                    {
                        this.DrawArrowhead(g, view, pen3, brush2, false, ef1.Width, ef1.Height, tfArray1);
                    }
                    if (tfArray2 != null)
                    {
                        this.DrawArrowhead(g, view, pen3, brush2, true, ef1.Width, ef1.Height, tfArray2);
                    }
                }
                GraphicsPath path2 = this.GetPath(0f, 0f, tfArray1, tfArray2);
                Pen pen4 = this.HighlightPen;
                if ((pen4 != null) && this.Highlight)
                {
                    DiagramGraph.DrawPath(g, view, pen4, null, path2);
                }
                if (pen1 != null)
                {
                    DiagramGraph.DrawPath(g, view, pen1, null, path2);
                }
                base.DisposePath(path2);
                if ((tfArray1 != null) || (tfArray2 != null))
                {
                    if ((pen2.DashStyle != DashStyle.Solid) || (pen2.Width > 1f))
                    {
                        Color color1 = Color.Black;
                        try
                        {
                            color1 = pen2.Color;
                        }
                        catch (Exception)
                        {
                            color1 = Color.Black;
                        }
                        pen2 = new Pen(color1);
                        if (tfArray1 != null)
                        {
                            this.DrawArrowhead(g, view, pen2, brush1, false, 0f, 0f, tfArray1);
                        }
                        if (tfArray2 != null)
                        {
                            this.DrawArrowhead(g, view, pen2, brush1, true, 0f, 0f, tfArray2);
                        }
                        pen2.Dispose();
                    }
                    else
                    {
                        if (tfArray1 != null)
                        {
                            this.DrawArrowhead(g, view, pen2, brush1, false, 0f, 0f, tfArray1);
                        }
                        if (tfArray2 != null)
                        {
                            this.DrawArrowhead(g, view, pen2, brush1, true, 0f, 0f, tfArray2);
                        }
                    }
                }
                if (((base.Layer != null) && (view != null)) && (this.Style == StrokeGraphStyle.RoundedLineWithJumpOvers))
                {
                    DiagramLayer.GoLayerCache cache1 = base.Layer.FindCache(view);
                    if ((cache1 != null) && !cache1.Strokes.Contains(this))
                    {
                        cache1.Strokes.Add(this);
                    }
                }
            }
        }

        public virtual void RemovePoint(int i)
        {
            if ((i >= 0) && (i < this.myPointsCount))
            {
                base.ResetPath();
                PointF tf1 = this.myPoints[i];
                if (this.myPointsCount > (i + 1))
                {
                    Array.Copy(this.myPoints, (int)(i + 1), this.myPoints, i, (int)((this.myPointsCount - i) - 1));
                }
                this.myPointsCount--;
                base.InvalidBounds = true;
                this.Changed(0x4b2, i, null, DiagramShape.MakeRect(tf1), i, null, DiagramShape.MakeRect(tf1));
            }
        }

        public override void RemoveSelectionHandles(DiagramSelection sel)
        {
            if (this.HighlightWhenSelected)
            {
                bool flag1 = base.SkipsUndoManager;
                base.SkipsUndoManager = true;
                this.Highlight = false;
                base.SkipsUndoManager = flag1;
            }
            base.RemoveSelectionHandles(sel);
        }

        public virtual void SetPoint(int i, PointF p)
        {
            if ((i < 0) || (i >= this.myPointsCount))
            {
                throw new ArgumentException("StrokeShape.SetPoint given an invalid index");
            }
            PointF tf1 = this.myPoints[i];
            if (tf1 != p)
            {
                base.ResetPath();
                this.myPoints[i] = p;
                base.InvalidBounds = true;
                this.Changed(0x4b3, i, null, DiagramShape.MakeRect(tf1), i, null, DiagramShape.MakeRect(p));
            }
        }

        public virtual void SetPoints(PointF[] points)
        {
            this.Changing(0x4b4);
            base.ResetPath();
            int num1 = points.Length;
            if (num1 > this.myPoints.Length)
            {
                this.myPoints = new PointF[num1];
            }
            Array.Copy(points, 0, this.myPoints, 0, num1);
            this.myPointsCount = num1;
            base.InvalidBounds = true;
            this.Changed(0x4b4, 0, null, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
        }


        [Description("How rounded corners are for strokes of style RoundedLine and how curved Bezier links are."), Category("Appearance"), DefaultValue((float)10f)]
        public virtual float Curviness
        {
            get
            {
                return this.myCurviness;
            }
            set
            {
                float single1 = this.myCurviness;
                if (single1 != value)
                {
                    this.myCurviness = value;
                    base.ResetPath();
                    this.Changed(0x4b6, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("The index of the first point getting a selection handle."), Category("Behavior")]
        public virtual int FirstPickIndex
        {
            get
            {
                return 0;
            }
        }

        [DefaultValue(false), Description("Whether an arrow is drawn at the start of this stroke."), Category("Appearance")]
        public virtual bool FromArrow
        {
            get
            {
                return ((base.InternalFlags & 0x100000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x100000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x100000;
                    }
                    else
                    {
                        base.InternalFlags &= -1048577;
                    }
                    base.InvalidBounds = true;
                    base.ResetPath();
                    this.Changed(1260, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), Description("A point specifying the direction from which comes the arrow at the start of this stroke."), TypeConverter(typeof(PointFConverter))]
        public virtual PointF FromArrowAnchorPoint
        {
            get
            {
                return this.GetPoint(1);
            }
        }

        [Description("The point at the tip of the arrow at the start of this stroke."), TypeConverter(typeof(PointFConverter)), Category("Appearance")]
        public virtual PointF FromArrowEndPoint
        {
            get
            {
                return this.GetPoint(0);
            }
        }

        [DefaultValue(true), Category("Appearance"), Description("Whether the arrowhead is filled with the stroke's brush")]
        public virtual bool FromArrowFilled
        {
            get
            {
                if (this.myFromArrowInfo != null)
                {
                    return this.myFromArrowInfo.Filled;
                }
                return true;
            }
            set
            {
                bool flag1;
                if (this.myFromArrowInfo != null)
                {
                    flag1 = this.myFromArrowInfo.Filled;
                }
                else
                {
                    flag1 = true;
                }
                if (flag1 != value)
                {
                    if (this.myFromArrowInfo == null)
                    {
                        this.myFromArrowInfo = new ArrowInfo();
                    }
                    this.myFromArrowInfo.Filled = value;
                    this.Changed(0x4f0, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The length of the arrowhead at the start of this stroke, along the shaft from the end point to the widest point."), DefaultValue((float)10f), Category("Appearance")]
        public virtual float FromArrowLength
        {
            get
            {
                if (this.myFromArrowInfo != null)
                {
                    return this.myFromArrowInfo.ArrowLength;
                }
                return 10f;
            }
            set
            {
                float single1;
                if (this.myFromArrowInfo != null)
                {
                    single1 = this.myFromArrowInfo.ArrowLength;
                }
                else
                {
                    single1 = 10f;
                }
                if (single1 != value)
                {
                    if (this.myFromArrowInfo == null)
                    {
                        this.myFromArrowInfo = new ArrowInfo();
                    }
                    this.myFromArrowInfo.ArrowLength = value;
                    base.InvalidBounds = true;
                    base.ResetPath();
                    this.Changed(0x4ed, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("The length of the arrow along the shaft at the start of this stroke."), Category("Appearance"), DefaultValue((float)8f)]
        public virtual float FromArrowShaftLength
        {
            get
            {
                if (this.myFromArrowInfo != null)
                {
                    return this.myFromArrowInfo.ShaftLength;
                }
                return 8f;
            }
            set
            {
                float single1;
                if (this.myFromArrowInfo != null)
                {
                    single1 = this.myFromArrowInfo.ShaftLength;
                }
                else
                {
                    single1 = 8f;
                }
                if (single1 != value)
                {
                    if (this.myFromArrowInfo == null)
                    {
                        this.myFromArrowInfo = new ArrowInfo();
                    }
                    this.myFromArrowInfo.ShaftLength = value;
                    base.InvalidBounds = true;
                    base.ResetPath();
                    this.Changed(0x4ee, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Category("Appearance"), DefaultValue(0), Description("Specifies the general shape of the arrowhead")]
        public virtual StrokeArrowheadStyle FromArrowStyle
        {
            get
            {
                if (this.myFromArrowInfo != null)
                {
                    return this.myFromArrowInfo.Style;
                }
                return StrokeArrowheadStyle.Polygon;
            }
            set
            {
                StrokeArrowheadStyle style1;
                if (this.myFromArrowInfo != null)
                {
                    style1 = this.myFromArrowInfo.Style;
                }
                else
                {
                    style1 = StrokeArrowheadStyle.Polygon;
                }
                if (style1 != value)
                {
                    if (this.myFromArrowInfo == null)
                    {
                        this.myFromArrowInfo = new ArrowInfo();
                    }
                    this.myFromArrowInfo.Style = value;
                    base.ResetPath();
                    this.Changed(0x4f1, 0, style1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), Description("The width at its widest point of the arrowhead at the start of this stroke."), DefaultValue((float)8f)]
        public virtual float FromArrowWidth
        {
            get
            {
                if (this.myFromArrowInfo != null)
                {
                    return this.myFromArrowInfo.Width;
                }
                return 8f;
            }
            set
            {
                float single1;
                if (this.myFromArrowInfo != null)
                {
                    single1 = this.myFromArrowInfo.Width;
                }
                else
                {
                    single1 = 8f;
                }
                if ((single1 != value) && (value >= 0f))
                {
                    if (this.myFromArrowInfo == null)
                    {
                        this.myFromArrowInfo = new ArrowInfo();
                    }
                    this.myFromArrowInfo.Width = value;
                    base.InvalidBounds = true;
                    this.Changed(0x4ef, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("Whether a highlight is shown along the path of this stroke."), DefaultValue(false), Category("Appearance")]
        public virtual bool Highlight
        {
            get
            {
                return ((base.InternalFlags & 0x400000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x400000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x400000;
                    }
                    else
                    {
                        base.InternalFlags &= -4194305;
                    }
                    this.Changed(0x4d5, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The pen used to draw the highlight."), DefaultValue((string)null), Category("Appearance")]
        public virtual Pen HighlightPen
        {
            get
            {
                if (this.myHighlightPenInfo != null)
                {
                    return this.myHighlightPenInfo.GetPen();
                }
                return null;
            }
            set
            {
                DiagramGraph.GoPenInfo info1 = this.myHighlightPenInfo;
                DiagramGraph.GoPenInfo info2 = DiagramGraph.GetPenInfo(value);
                if (info1 != info2)
                {
                    this.myHighlightPenInfo = info2;
                    this.Changed(0x4d4, 0, info1, DiagramShape.NullRect, 0, info2, DiagramShape.NullRect);
                    if (base.Parent != null)
                    {
                        base.Parent.InvalidatePaintBounds();
                    }
                }
            }
        }

        internal DiagramGraph.GoPenInfo HighlightPenInfo
        {
            get
            {
                return this.myHighlightPenInfo;
            }
        }

        [Description("[Only supported in GoDiagram Pocket]")]
        public virtual float HighlightPenWidth
        {
            get
            {
                if (this.HighlightPenInfo != null)
                {
                    return this.HighlightPenInfo.Width;
                }
                return 0f;
            }
            set
            {
                float single1 = 0f;
                if (this.HighlightPenInfo != null)
                {
                    single1 = this.HighlightPenInfo.Width;
                }
                if (single1 != value)
                {
                    Pen pen1 = this.HighlightPen;
                    if (pen1 != null)
                    {
                        Pen pen2 = (Pen)pen1.Clone();
                        pen2.Width = value;
                        this.HighlightPen = pen2;
                    }
                }
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Whether the highlight is shown when this stroke becomes selected.")]
        public virtual bool HighlightWhenSelected
        {
            get
            {
                return ((base.InternalFlags & 0x800000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x800000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x800000;
                    }
                    else
                    {
                        base.InternalFlags &= -8388609;
                    }
                    this.Changed(0x4d6, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("The index of the last point getting a selection handle.")]
        public virtual int LastPickIndex
        {
            get
            {
                return (this.PointsCount - 1);
            }
        }

        [Description("[Only supported in GoDiagram Pocket]")]
        public virtual float PenWidth
        {
            get
            {
                return base.InternalPenWidth;
            }
            set
            {
                if (base.InternalPenWidth != value)
                {
                    Pen pen1 = this.Pen;
                    if (pen1 != null)
                    {
                        Pen pen2 = (Pen)pen1.Clone();
                        pen2.Width = value;
                        this.Pen = pen2;
                    }
                }
            }
        }

        [Category("Behavior"), Description("About how close users need to be to the stroke to pick it")]
        public virtual float PickMargin
        {
            get
            {
                return 3f;
            }
        }

        [Description("The number of points in this stroke."), Category("Appearance")]
        public virtual int PointsCount
        {
            get
            {
                return this.myPointsCount;
            }
        }

        [Description("The kind of curve drawn using this stroke's points."), Category("Appearance"), DefaultValue(0)]
        public virtual StrokeGraphStyle Style
        {
            get
            {
                return this.myStyle;
            }
            set
            {
                StrokeGraphStyle style1 = this.myStyle;
                if (style1 != value)
                {
                    this.myStyle = value;
                    base.ResetPath();
                    this.Changed(0x4b5, 0, style1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(false), Description("Whether an arrow is drawn at the end of this stroke."), Category("Appearance")]
        public virtual bool ToArrow
        {
            get
            {
                return ((base.InternalFlags & 0x200000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x200000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x200000;
                    }
                    else
                    {
                        base.InternalFlags &= -2097153;
                    }
                    base.InvalidBounds = true;
                    base.ResetPath();
                    this.Changed(1250, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("A point which specifies the direction the arrow is coming from."), TypeConverter(typeof(PointFConverter)), Category("Appearance")]
        public virtual PointF ToArrowAnchorPoint
        {
            get
            {
                int num1 = this.PointsCount;
                return this.GetPoint(num1 - 2);
            }
        }

        [Category("Appearance"), TypeConverter(typeof(PointFConverter)), Description("The point at the tip of the arrowhead at the end of this stroke.")]
        public virtual PointF ToArrowEndPoint
        {
            get
            {
                return this.GetPoint(this.PointsCount - 1);
            }
        }

        [Description("Whether the arrowhead is filled with the stroke's brush"), DefaultValue(true), Category("Appearance")]
        public virtual bool ToArrowFilled
        {
            get
            {
                if (this.myToArrowInfo != null)
                {
                    return this.myToArrowInfo.Filled;
                }
                return true;
            }
            set
            {
                bool flag1;
                if (this.myToArrowInfo != null)
                {
                    flag1 = this.myToArrowInfo.Filled;
                }
                else
                {
                    flag1 = true;
                }
                if (flag1 != value)
                {
                    if (this.myToArrowInfo == null)
                    {
                        this.myToArrowInfo = new ArrowInfo();
                    }
                    this.myToArrowInfo.Filled = value;
                    this.Changed(0x4e6, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The length of the arrow at the end of this stroke, along the shaft from the end point to the widest point."), Category("Appearance"), DefaultValue((float)10f)]
        public virtual float ToArrowLength
        {
            get
            {
                if (this.myToArrowInfo != null)
                {
                    return this.myToArrowInfo.ArrowLength;
                }
                return 10f;
            }
            set
            {
                float single1;
                if (this.myToArrowInfo != null)
                {
                    single1 = this.myToArrowInfo.ArrowLength;
                }
                else
                {
                    single1 = 10f;
                }
                if (single1 != value)
                {
                    if (this.myToArrowInfo == null)
                    {
                        this.myToArrowInfo = new ArrowInfo();
                    }
                    this.myToArrowInfo.ArrowLength = value;
                    base.InvalidBounds = true;
                    base.ResetPath();
                    this.Changed(0x4e3, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("The length of the arrow along the shaft at the end of this stroke."), Category("Appearance"), DefaultValue((float)8f)]
        public virtual float ToArrowShaftLength
        {
            get
            {
                if (this.myToArrowInfo != null)
                {
                    return this.myToArrowInfo.ShaftLength;
                }
                return 8f;
            }
            set
            {
                float single1;
                if (this.myToArrowInfo != null)
                {
                    single1 = this.myToArrowInfo.ShaftLength;
                }
                else
                {
                    single1 = 8f;
                }
                if (single1 != value)
                {
                    if (this.myToArrowInfo == null)
                    {
                        this.myToArrowInfo = new ArrowInfo();
                    }
                    this.myToArrowInfo.ShaftLength = value;
                    base.InvalidBounds = true;
                    base.ResetPath();
                    this.Changed(0x4e4, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("Specifies the general shape of the arrowhead"), Category("Appearance"), DefaultValue(0)]
        public virtual StrokeArrowheadStyle ToArrowStyle
        {
            get
            {
                if (this.myToArrowInfo != null)
                {
                    return this.myToArrowInfo.Style;
                }
                return StrokeArrowheadStyle.Polygon;
            }
            set
            {
                StrokeArrowheadStyle style1;
                if (this.myToArrowInfo != null)
                {
                    style1 = this.myToArrowInfo.Style;
                }
                else
                {
                    style1 = StrokeArrowheadStyle.Polygon;
                }
                if (style1 != value)
                {
                    if (this.myToArrowInfo == null)
                    {
                        this.myToArrowInfo = new ArrowInfo();
                    }
                    this.myToArrowInfo.Style = value;
                    base.ResetPath();
                    this.Changed(0x4e7, 0, style1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue((float)8f), Description("The width of the arrowhead at the widest point."), Category("Appearance")]
        public virtual float ToArrowWidth
        {
            get
            {
                if (this.myToArrowInfo != null)
                {
                    return this.myToArrowInfo.Width;
                }
                return 8f;
            }
            set
            {
                float single1;
                if (this.myToArrowInfo != null)
                {
                    single1 = this.myToArrowInfo.Width;
                }
                else
                {
                    single1 = 8f;
                }
                if ((single1 != value) && (value >= 0f))
                {
                    if (this.myToArrowInfo == null)
                    {
                        this.myToArrowInfo = new ArrowInfo();
                    }
                    this.myToArrowInfo.Width = value;
                    base.InvalidBounds = true;
                    this.Changed(0x4e5, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }


        public const int ChangedAddPoint = 0x4b1;
        public const int ChangedAllPoints = 0x4b4;
        public const int ChangedCurviness = 0x4b6;
        public const int ChangedFromArrowFilled = 0x4f0;
        public const int ChangedFromArrowHead = 1260;
        public const int ChangedFromArrowLength = 0x4ed;
        public const int ChangedFromArrowShaftLength = 0x4ee;
        public const int ChangedFromArrowStyle = 0x4f1;
        public const int ChangedFromArrowWidth = 0x4ef;
        public const int ChangedHighlight = 0x4d5;
        public const int ChangedHighlightPen = 0x4d4;
        public const int ChangedHighlightPenWidth = 0x4f3;
        public const int ChangedHighlightWhenSelected = 0x4d6;
        public const int ChangedModifiedPoint = 0x4b3;
        public const int ChangedPenWidth = 0x4f2;
        public const int ChangedRemovePoint = 0x4b2;
        public const int ChangedStyle = 0x4b5;
        public const int ChangedToArrowFilled = 0x4e6;
        public const int ChangedToArrowHead = 1250;
        public const int ChangedToArrowLength = 0x4e3;
        public const int ChangedToArrowShaftLength = 0x4e4;
        public const int ChangedToArrowStyle = 0x4e7;
        public const int ChangedToArrowWidth = 0x4e5;
        private const bool DEFAULT_ARROW_FILLED = true;
        private const float DEFAULT_ARROW_LENGTH = 10f;
        private const int DEFAULT_ARROW_POLYGON_SIDES = 4;
        private const float DEFAULT_ARROW_SHAFT_LENGTH = 8f;
        private const StrokeArrowheadStyle DEFAULT_ARROW_STYLE = StrokeArrowheadStyle.Polygon;
        private const float DEFAULT_ARROW_WIDTH = 8f;
        private const int flagHighlightWhenSelected = 0x800000;
        private const int flagStrokeArrowEnd = 0x200000;
        private const int flagStrokeArrowStart = 0x100000;
        private const int flagStrokeHighlight = 0x400000;
        private const int LINE_FUZZ = 3;
        private float myCurviness;
        private ArrowInfo myFromArrowInfo;
        private DiagramGraph.GoPenInfo myHighlightPenInfo;
        private static float[] myIntersections;
        private PointF[] myPoints;
        private int myPointsCount;
        private StrokeGraphStyle myStyle;
        private ArrowInfo myToArrowInfo;

        [Serializable]
        internal sealed class ArrowInfo : ICloneable
        {
            internal ArrowInfo()
            {
                this.ArrowLength = 10f;
                this.ShaftLength = 8f;
                this.Width = 8f;
                this.myFlags = 0x10000;
                this.myPolyPoints = null;
            }

            public object Clone()
            {
                StrokeGraph.ArrowInfo info1 = (StrokeGraph.ArrowInfo)base.MemberwiseClone();
                if (this.myPolyPoints != null)
                {
                    info1.myPolyPoints = (PointF[])this.myPolyPoints.Clone();
                }
                return info1;
            }

            internal PointF[] GetPoly(int n)
            {
                if ((this.myPolyPoints == null) || (this.myPolyPoints.Length < n))
                {
                    this.myPolyPoints = new PointF[n];
                }
                return this.myPolyPoints;
            }


            internal bool Filled
            {
                get
                {
                    return ((this.myFlags & 0x10000) != 0);
                }
                set
                {
                    if (value)
                    {
                        this.myFlags |= 0x10000;
                    }
                    else
                    {
                        this.myFlags &= -65537;
                    }
                }
            }

            internal StrokeArrowheadStyle Style
            {
                get
                {
                    return (((StrokeArrowheadStyle)this.myFlags) & ((StrokeArrowheadStyle)0xffff));
                }
                set
                {
                    this.myFlags = (int)((((StrokeArrowheadStyle)this.myFlags) & ((StrokeArrowheadStyle)(-65536))) | (value & ((StrokeArrowheadStyle)0xffff)));
                }
            }


            internal float ArrowLength;
            internal const int flagFilled = 0x10000;
            internal const int flagStyleMask = 0xffff;
            private int myFlags;
            private PointF[] myPolyPoints;
            internal float ShaftLength;
            internal float Width;
        }
    }
}
