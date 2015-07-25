using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class HexagonGraph : DiagramGraph
    {
        public HexagonGraph()
        {
            this.myPoints = new PointF[6];
            this.myDistanceLeft = 10f;
            this.myDistanceRight = 10f;
            this.myDistanceTop = 10f;
            this.myDistanceBottom = 10f;
            this.myOrientation = System.Windows.Forms.Orientation.Horizontal;
            this.myReshapeBehavior = HexagonGraphReshapeBehavior.CompleteSymmetry;
            base.InternalFlags |= 0x700200;
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            base.AddSelectionHandles(sel, selectedObj);
            if (this.CanReshape() && this.ReshapableCorner)
            {
                RectangleF ef1 = this.Bounds;
                bool flag1 = this.Orientation == System.Windows.Forms.Orientation.Horizontal;
                float single1 = this.DistanceLeft;
                float single2 = this.DistanceRight;
                float single3 = this.DistanceTop;
                float single4 = this.DistanceBottom;
                bool flag2 = false;
                bool flag3 = false;
                this.DetermineReshapeBehavior(ref flag2, ref flag3);
                PointF[] tfArray1 = this.getPoints();
                PointF tf1 = new PointF();
                if (flag1)
                {
                    tf1 = tfArray1[1];
                }
                else
                {
                    tf1 = tfArray1[5];
                }
                IShapeHandle handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, 0x402, true);
                base.MakeDiamondResizeHandle(handle1, flag1 ? 0x40 : 0x80);
                tf1 = tfArray1[0];
                handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, 0x404, true);
                base.MakeDiamondResizeHandle(handle1, flag3 ? (flag1 ? 0x40 : 0x80) : 1);
                if (!flag2)
                {
                    if (flag1)
                    {
                        tf1 = tfArray1[4];
                    }
                    else
                    {
                        tf1 = tfArray1[2];
                    }
                    handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, 0x403, true);
                    base.MakeDiamondResizeHandle(handle1, flag1 ? 0x40 : 0x80);
                    tf1 = tfArray1[3];
                    handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, 0x405, true);
                    base.MakeDiamondResizeHandle(handle1, flag3 ? (flag1 ? 0x40 : 0x80) : 1);
                }
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x5a2:
                    {
                        this.DistanceLeft = e.GetFloat(undo);
                        return;
                    }
                case 0x5a3:
                    {
                        this.DistanceRight = e.GetFloat(undo);
                        return;
                    }
                case 0x5a4:
                    {
                        this.DistanceTop = e.GetFloat(undo);
                        return;
                    }
                case 0x5a5:
                    {
                        this.DistanceBottom = e.GetFloat(undo);
                        return;
                    }
                case 0x5a6:
                    {
                        this.Orientation = (System.Windows.Forms.Orientation)e.GetValue(undo);
                        return;
                    }
                case 0x5a7:
                    {
                        this.ReshapeBehavior = (HexagonGraphReshapeBehavior)e.GetValue(undo);
                        return;
                    }
                case 0x5a8:
                    {
                        this.ReshapableCorner = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5a9:
                    {
                        this.KeepsLengthwiseSymmetry = (bool)e.GetValue(undo);
                        return;
                    }
                case 1450:
                    {
                        this.KeepsCrosswiseSymmetry = (bool)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
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

        private void DetermineReshapeBehavior(ref bool cross, ref bool length)
        {
            cross = this.KeepsCrosswiseSymmetry;
            length = this.KeepsLengthwiseSymmetry;
            switch (this.ReshapeBehavior)
            {
                case HexagonGraphReshapeBehavior.FreeForm:
                    {
                        return;
                    }
                case HexagonGraphReshapeBehavior.CrosswiseSymmetry:
                    {
                        cross = true;
                        return;
                    }
                case HexagonGraphReshapeBehavior.LengthwiseSymmetry:
                    {
                        length = true;
                        return;
                    }
                case HexagonGraphReshapeBehavior.CompleteSymmetry:
                    {
                        length = true;
                        cross = true;
                        return;
                    }
            }
        }

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            bool flag1 = false;
            bool flag2 = false;
            this.DetermineReshapeBehavior(ref flag1, ref flag2);
            if (((whichHandle >= 0x402) && (whichHandle <= 0x405)) && ((this.ResizesRealtime || (evttype == InputState.Finish)) || (evttype == InputState.Cancel)))
            {
                RectangleF ef1 = this.Bounds;
                RectangleF ef2 = this.Bounds;
                bool flag3 = this.Orientation == System.Windows.Forms.Orientation.Horizontal;
                PointF tf1 = new PointF();
                float single1 = this.DistanceLeft;
                float single2 = this.DistanceRight;
                float single3 = this.DistanceTop;
                float single4 = this.DistanceBottom;
                float single5 = this.DistanceLeft;
                float single6 = this.DistanceRight;
                float single7 = this.DistanceTop;
                float single8 = this.DistanceBottom;
                if (whichHandle == 0x402)
                {
                    tf1 = this.myPoints[0];
                    if (flag3)
                    {
                        single5 = newPoint.X - tf1.X;
                        if (single5 > (ef1.Width - System.Math.Abs(single6)))
                        {
                            single5 = ef1.Width - System.Math.Abs(single6);
                        }
                        if (single5 < 0f)
                        {
                            ef2.X = newPoint.X;
                            ef2.Y = ef1.Y;
                            if (single1 < 0f)
                            {
                                if (!flag1)
                                {
                                    ef2.Width = (ef1.Width + single1) - single5;
                                    ef2.Height = ef1.Height;
                                }
                                else
                                {
                                    ef2.Width = (ef1.Width + (single1 * 2f)) - (single5 * 2f);
                                    ef2.Height = ef1.Height;
                                }
                            }
                            else if (!flag1)
                            {
                                ef2.Width = ef1.Width - single5;
                                ef2.Height = ef1.Height;
                            }
                            else
                            {
                                ef2.Width = ef1.Width - (single5 * 2f);
                                ef2.Height = ef1.Height;
                            }
                        }
                        else
                        {
                            ef2.X = tf1.X;
                            ef2.Y = ef1.Y;
                            if (single1 < 0f)
                            {
                                if (!flag1)
                                {
                                    ef2.Width = ef1.Width + single1;
                                    ef2.Height = ef1.Height;
                                }
                                else
                                {
                                    ef2.Width = ef1.Width + (single1 * 2f);
                                    ef2.Height = ef1.Height;
                                }
                            }
                            else
                            {
                                ef2.Width = ef1.Width;
                                ef2.Height = ef1.Height;
                            }
                        }
                    }
                    else
                    {
                        single7 = newPoint.Y - tf1.Y;
                        if (single7 > (ef1.Height - System.Math.Abs(single8)))
                        {
                            single7 = ef1.Height - System.Math.Abs(single8);
                        }
                        if (single7 < 0f)
                        {
                            ef2.Y = newPoint.Y;
                            ef2.X = ef1.X;
                            if (single3 < 0f)
                            {
                                if (!flag1)
                                {
                                    ef2.Height = (ef1.Height + single3) - single7;
                                    ef2.Width = ef1.Width;
                                }
                                else
                                {
                                    ef2.Height = (ef1.Height + (single3 * 2f)) - (single7 * 2f);
                                    ef2.Width = ef1.Width;
                                }
                            }
                            else if (!flag1)
                            {
                                ef2.Height = ef1.Height - single7;
                                ef2.Width = ef1.Width;
                            }
                            else
                            {
                                ef2.Height = ef1.Height - (single7 * 2f);
                                ef2.Width = ef1.Width;
                            }
                        }
                        else
                        {
                            ef2.Y = tf1.Y;
                            ef2.X = ef1.X;
                            if (single3 < 0f)
                            {
                                if (!flag1)
                                {
                                    ef2.Height = ef1.Height + single3;
                                    ef2.Width = ef1.Width;
                                }
                                else
                                {
                                    ef2.Height = ef1.Height + (single3 * 2f);
                                    ef2.Width = ef1.Width;
                                }
                            }
                            else
                            {
                                ef2.Height = ef1.Height;
                                ef2.Width = ef1.Width;
                            }
                        }
                    }
                }
                else if (whichHandle == 0x403)
                {
                    tf1 = this.myPoints[3];
                    if (flag3)
                    {
                        single6 = tf1.X - newPoint.X;
                        if (single6 > (ef1.Width - System.Math.Abs(single5)))
                        {
                            single6 = ef1.Width - System.Math.Abs(single5);
                        }
                        if (single6 < 0f)
                        {
                            ef2.X = ef1.X;
                            ef2.Y = ef1.Y;
                            if (single2 < 0f)
                            {
                                ef2.Width = (ef1.Width + single2) - single6;
                                ef2.Height = ef1.Height;
                            }
                            else
                            {
                                ef2.Width = ef1.Width - single6;
                                ef2.Height = ef1.Height;
                            }
                        }
                        else
                        {
                            ef2.X = ef1.X;
                            ef2.Y = ef1.Y;
                            if (single2 < 0f)
                            {
                                ef2.Width = ef1.Width + single2;
                                ef2.Height = ef1.Height;
                            }
                            else
                            {
                                ef2.Width = ef1.Width;
                                ef2.Height = ef1.Height;
                            }
                        }
                    }
                    else
                    {
                        single8 = tf1.Y - newPoint.Y;
                        if (single8 > (ef1.Height - System.Math.Abs(single7)))
                        {
                            single8 = ef1.Height - System.Math.Abs(single7);
                        }
                        if (single8 < 0f)
                        {
                            ef2.Y = ef1.Y;
                            ef2.X = ef1.X;
                            if (single4 < 0f)
                            {
                                ef2.Height = (ef1.Height + single4) - single8;
                                ef2.Width = ef1.Width;
                            }
                            else
                            {
                                ef2.Height = ef1.Height - single8;
                                ef2.Width = ef1.Width;
                            }
                        }
                        else
                        {
                            ef2.Y = ef1.Y;
                            ef2.X = ef1.X;
                            if (single4 < 0f)
                            {
                                ef2.Height = ef1.Height + single4;
                                ef2.Width = ef1.Width;
                            }
                            else
                            {
                                ef2.Height = ef1.Height;
                                ef2.Width = ef1.Width;
                            }
                        }
                    }
                }
                else if (whichHandle == 0x404)
                {
                    tf1 = this.myPoints[1];
                    if (flag3)
                    {
                        single5 = tf1.X - newPoint.X;
                        if (single5 < -(ef1.Width - System.Math.Abs(single6)))
                        {
                            single5 = -(ef1.Width - System.Math.Abs(single6));
                        }
                        if (single5 <= 0f)
                        {
                            ef2.X = tf1.X;
                            ef2.Y = ef1.Y;
                            if (single1 < 0f)
                            {
                                ef2.Width = ef1.Width;
                                ef2.Height = ef1.Height;
                            }
                            else if (!flag1)
                            {
                                ef2.Width = ef1.Width - single1;
                                ef2.Height = ef1.Height;
                            }
                            else
                            {
                                ef2.Width = ef1.Width - (single1 * 2f);
                                ef2.Height = ef1.Height;
                            }
                        }
                        else
                        {
                            ef2.X = newPoint.X;
                            ef2.Y = ef1.Y;
                            if (single1 < 0f)
                            {
                                if (!flag1)
                                {
                                    ef2.Width = ef1.Width + single5;
                                }
                                else
                                {
                                    ef2.Width = ef1.Width + (single5 * 2f);
                                }
                                ef2.Height = ef1.Height;
                            }
                            else if (!flag1)
                            {
                                ef2.Width = (ef1.Width - single1) + single5;
                                ef2.Height = ef1.Height;
                            }
                            else
                            {
                                ef2.Width = (ef1.Width - (single1 * 2f)) + (single5 * 2f);
                                ef2.Height = ef1.Height;
                            }
                        }
                        if (!flag2)
                        {
                            if (newPoint.Y < ef1.Y)
                            {
                                single7 = 0f;
                            }
                            else if (newPoint.Y > (ef1.Y + ef1.Height))
                            {
                                single7 = ef1.Height;
                            }
                            else
                            {
                                single7 = newPoint.Y - ef1.Y;
                            }
                        }
                    }
                    else
                    {
                        single7 = tf1.Y - newPoint.Y;
                        if (single7 < -(ef1.Height - System.Math.Abs(single8)))
                        {
                            single7 = -(ef1.Height - System.Math.Abs(single8));
                        }
                        if (single7 <= 0f)
                        {
                            ef2.Y = tf1.Y;
                            ef2.X = ef1.X;
                            if (single3 < 0f)
                            {
                                ef2.Height = ef1.Height;
                                ef2.Width = ef1.Width;
                            }
                            else if (!flag1)
                            {
                                ef2.Height = ef1.Height - single3;
                                ef2.Width = ef1.Width;
                            }
                            else
                            {
                                ef2.Height = ef1.Height - (single3 * 2f);
                                ef2.Width = ef1.Width;
                            }
                        }
                        else
                        {
                            ef2.Y = newPoint.Y;
                            ef2.X = ef1.X;
                            if (single3 < 0f)
                            {
                                if (!flag1)
                                {
                                    ef2.Height = ef1.Height + single7;
                                    ef2.Width = ef1.Width;
                                }
                                else
                                {
                                    ef2.Height = ef1.Height + (single7 * 2f);
                                    ef2.Width = ef1.Width;
                                }
                            }
                            else if (!flag1)
                            {
                                ef2.Height = (ef1.Height - single3) + single7;
                                ef2.Width = ef1.Width;
                            }
                            else
                            {
                                ef2.Height = (ef1.Height - (single3 * 2f)) + (single7 * 2f);
                                ef2.Width = ef1.Width;
                            }
                        }
                        if (!flag2)
                        {
                            if (newPoint.X < ef1.X)
                            {
                                single5 = 0f;
                            }
                            else if (newPoint.X > (ef1.X + ef1.Width))
                            {
                                single5 = ef1.Width;
                            }
                            else
                            {
                                single5 = newPoint.X - ef1.X;
                            }
                        }
                    }
                }
                else if (whichHandle == 0x405)
                {
                    tf1 = this.myPoints[2];
                    if (flag3)
                    {
                        single6 = newPoint.X - tf1.X;
                        if (single6 < -(ef1.Width - System.Math.Abs(single5)))
                        {
                            single6 = -(ef1.Width - System.Math.Abs(single5));
                        }
                        if (single6 < 0f)
                        {
                            if (single2 < 0f)
                            {
                                ef2 = ef1;
                            }
                            else
                            {
                                ef2.X = ef1.X;
                                ef2.Y = ef1.Y;
                                ef2.Width = ef1.Width - single2;
                                ef2.Height = ef1.Height;
                            }
                        }
                        else
                        {
                            ef2.X = ef1.X;
                            ef2.Y = ef1.Y;
                            if (single2 < 0f)
                            {
                                ef2.Width = ef1.Width + single6;
                                ef2.Height = ef1.Height;
                            }
                            else
                            {
                                ef2.Width = (ef1.Width - single2) + single6;
                                ef2.Height = ef1.Height;
                            }
                        }
                        if (!flag2)
                        {
                            if (newPoint.Y < ef1.Y)
                            {
                                single8 = ef1.Height;
                            }
                            else if (newPoint.Y > (ef1.Y + ef1.Height))
                            {
                                single8 = 0f;
                            }
                            else
                            {
                                single8 = (ef1.Y + ef1.Height) - newPoint.Y;
                            }
                        }
                    }
                    else
                    {
                        single8 = newPoint.Y - tf1.Y;
                        if (single8 < -(ef1.Height - System.Math.Abs(single7)))
                        {
                            single8 = -(ef1.Height - System.Math.Abs(single7));
                        }
                        if (single8 < 0f)
                        {
                            ef2.Y = ef1.Y;
                            ef2.X = ef1.X;
                            if (single4 < 0f)
                            {
                                ef2.Height = ef1.Height;
                                ef2.Width = ef1.Width;
                            }
                            else
                            {
                                ef2.Height = ef1.Height - single4;
                                ef2.Width = ef1.Width;
                            }
                        }
                        else
                        {
                            ef2.Y = ef1.Y;
                            ef2.X = ef1.X;
                            if (single4 < 0f)
                            {
                                ef2.Height = ef1.Height + single8;
                                ef2.Width = ef1.Width;
                            }
                            else
                            {
                                ef2.Height = (ef1.Height - single4) + single8;
                                ef2.Width = ef1.Width;
                            }
                        }
                        if (!flag2)
                        {
                            if (newPoint.X < ef1.X)
                            {
                                single6 = ef1.Width;
                            }
                            else if (newPoint.X > (ef1.X + ef1.Width))
                            {
                                single6 = 0f;
                            }
                            else
                            {
                                single6 = (ef1.X + ef1.Width) - newPoint.X;
                            }
                        }
                    }
                }
                if (flag1)
                {
                    if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        if (System.Math.Abs(single5) > (ef2.Width / 2f))
                        {
                            single5 = (ef2.Width / 2f) * System.Math.Sign(single5);
                        }
                        single6 = single5;
                    }
                    else
                    {
                        if (System.Math.Abs(single7) > (ef2.Height / 2f))
                        {
                            single7 = (ef2.Height / 2f) * System.Math.Sign(single7);
                        }
                        single8 = single7;
                    }
                }
                this.DistanceLeft = single5;
                this.DistanceTop = single7;
                if (!flag1)
                {
                    this.DistanceRight = single6;
                    this.DistanceBottom = single8;
                }
                this.Bounds = ef2;
                base.ResetPath();
            }
            else
            {
                RectangleF ef3 = this.Bounds;
                RectangleF ef4 = new RectangleF();
                base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
                ef4 = this.Bounds;
                if (!this.CanReshape())
                {
                    float single9 = ef4.Width / ef3.Width;
                    float single10 = ef4.Height / ef3.Height;
                    this.DistanceLeft *= single9;
                    this.DistanceTop *= single10;
                    if (!flag1)
                    {
                        this.DistanceRight *= single9;
                        this.DistanceBottom *= single10;
                    }
                }
                if (flag1)
                {
                    if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                    {
                        this.DistanceRight = this.Bounds.Width - ((this.DistanceLeft < 0f) ? 0f : this.DistanceLeft);
                        this.DistanceBottom = this.DistanceTop;
                    }
                    else
                    {
                        this.DistanceBottom = this.Bounds.Height - ((this.DistanceTop < 0f) ? 0f : this.DistanceTop);
                        this.DistanceRight = this.DistanceLeft;
                    }
                }
                if (flag2)
                {
                    if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                    {
                        this.DistanceLeft = this.Bounds.Width / 2f;
                        this.DistanceRight = this.Bounds.Width / 2f;
                    }
                    else
                    {
                        this.DistanceTop = this.Bounds.Height / 2f;
                        this.DistanceBottom = this.Bounds.Height / 2f;
                    }
                }
                base.ResetPath();
            }
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            PointF tf7;
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            PointF[] tfArray1 = this.getPoints();
            PointF tf1 = DiagramGraph.ExpandPointOnEdge(tfArray1[0], ef1, single1);
            PointF tf2 = DiagramGraph.ExpandPointOnEdge(tfArray1[1], ef1, single1);
            PointF tf3 = DiagramGraph.ExpandPointOnEdge(tfArray1[2], ef1, single1);
            PointF tf4 = DiagramGraph.ExpandPointOnEdge(tfArray1[3], ef1, single1);
            PointF tf5 = DiagramGraph.ExpandPointOnEdge(tfArray1[4], ef1, single1);
            PointF tf6 = DiagramGraph.ExpandPointOnEdge(tfArray1[5], ef1, single1);
            float single2 = p1.X;
            float single3 = p1.Y;
            float single4 = 1E+21f;
            PointF tf8 = new PointF();
            if (StrokeGraph.NearestIntersectionOnLine(tf1, tf2, p1, p2, out tf7))
            {
                float single5 = ((tf7.X - single2) * (tf7.X - single2)) + ((tf7.Y - single3) * (tf7.Y - single3));
                if (single5 < single4)
                {
                    single4 = single5;
                    tf8 = tf7;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf2, tf3, p1, p2, out tf7))
            {
                float single6 = ((tf7.X - single2) * (tf7.X - single2)) + ((tf7.Y - single3) * (tf7.Y - single3));
                if (single6 < single4)
                {
                    single4 = single6;
                    tf8 = tf7;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf3, tf4, p1, p2, out tf7))
            {
                float single7 = ((tf7.X - single2) * (tf7.X - single2)) + ((tf7.Y - single3) * (tf7.Y - single3));
                if (single7 < single4)
                {
                    single4 = single7;
                    tf8 = tf7;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf4, tf5, p1, p2, out tf7))
            {
                float single8 = ((tf7.X - single2) * (tf7.X - single2)) + ((tf7.Y - single3) * (tf7.Y - single3));
                if (single8 < single4)
                {
                    single4 = single8;
                    tf8 = tf7;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf5, tf6, p1, p2, out tf7))
            {
                float single9 = ((tf7.X - single2) * (tf7.X - single2)) + ((tf7.Y - single3) * (tf7.Y - single3));
                if (single9 < single4)
                {
                    single4 = single9;
                    tf8 = tf7;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf6, tf1, p1, p2, out tf7))
            {
                float single10 = ((tf7.X - single2) * (tf7.X - single2)) + ((tf7.Y - single3) * (tf7.Y - single3));
                if (single10 < single4)
                {
                    single4 = single10;
                    tf8 = tf7;
                }
            }
            result = tf8;
            return (single4 < 1E+21f);
        }

        private PointF[] getPoints()
        {
            RectangleF ef1 = this.Bounds;
            float single1 = this.DistanceLeft;
            float single2 = this.DistanceRight;
            float single3 = this.DistanceTop;
            float single4 = this.DistanceBottom;
            if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                if (single3 > ef1.Height)
                {
                    single3 = ef1.Height;
                }
                else if (single3 < 0f)
                {
                    single3 = 0f;
                }
                if (single4 > ef1.Height)
                {
                    single4 = ef1.Height;
                }
                else if (single4 < 0f)
                {
                    single4 = 0f;
                }
                if (this.KeepsCrosswiseSymmetry)
                {
                    if (single1 < -(ef1.Width / 2f))
                    {
                        single1 = -(ef1.Width / 2f);
                    }
                    if (single1 > (ef1.Width / 2f))
                    {
                        single1 = ef1.Width / 2f;
                    }
                    if (single2 < -(ef1.Width / 2f))
                    {
                        single2 = -(ef1.Width / 2f);
                    }
                    if (single2 > (ef1.Width / 2f))
                    {
                        single2 = ef1.Width / 2f;
                    }
                    if (single1 >= 0f)
                    {
                        this.myPoints[0] = new PointF(ef1.X, ef1.Y + single3);
                        this.myPoints[1] = new PointF(ef1.X + single1, ef1.Y);
                        this.myPoints[2] = new PointF((ef1.X + ef1.Width) - single1, ef1.Y);
                        this.myPoints[3] = new PointF(ef1.X + ef1.Width, ef1.Y + single3);
                        this.myPoints[4] = new PointF((ef1.X + ef1.Width) - single1, ef1.Y + ef1.Height);
                        this.myPoints[5] = new PointF(ef1.X + single1, ef1.Y + ef1.Height);
                    }
                    else
                    {
                        this.myPoints[0] = new PointF(ef1.X - single1, ef1.Y + single3);
                        this.myPoints[1] = new PointF(ef1.X, ef1.Y);
                        this.myPoints[2] = new PointF(ef1.X + ef1.Width, ef1.Y);
                        this.myPoints[3] = new PointF((ef1.X + ef1.Width) + single1, ef1.Y + single3);
                        this.myPoints[4] = new PointF(ef1.X + ef1.Width, ef1.Y + ef1.Height);
                        this.myPoints[5] = new PointF(ef1.X, ef1.Y + ef1.Height);
                    }
                }
                else
                {
                    if (System.Math.Abs(single1) > ef1.Width)
                    {
                        if (single1 <= 0f)
                        {
                            single1 = -ef1.Width;
                        }
                        else
                        {
                            single1 = ef1.Width;
                        }
                        single2 = 0f;
                    }
                    if (single2 < -(ef1.Width - System.Math.Abs(single1)))
                    {
                        single2 = -(ef1.Width - System.Math.Abs(single1));
                    }
                    else if (single2 > (ef1.Width - System.Math.Abs(single1)))
                    {
                        single2 = ef1.Width - System.Math.Abs(single1);
                    }
                    if (single1 >= 0f)
                    {
                        if (single2 >= 0f)
                        {
                            this.myPoints[0] = new PointF(ef1.X, ef1.Y + single3);
                            this.myPoints[1] = new PointF(ef1.X + single1, ef1.Y);
                            this.myPoints[2] = new PointF((ef1.X + ef1.Width) - single2, ef1.Y);
                            this.myPoints[3] = new PointF(ef1.X + ef1.Width, (ef1.Y + ef1.Height) - single4);
                            this.myPoints[4] = new PointF((ef1.X + ef1.Width) - single2, ef1.Y + ef1.Height);
                            this.myPoints[5] = new PointF(ef1.X + single1, ef1.Y + ef1.Height);
                        }
                        else
                        {
                            this.myPoints[0] = new PointF(ef1.X, ef1.Y + single3);
                            this.myPoints[1] = new PointF(ef1.X + single1, ef1.Y);
                            this.myPoints[2] = new PointF(ef1.X + ef1.Width, ef1.Y);
                            this.myPoints[3] = new PointF((ef1.X + ef1.Width) + single2, (ef1.Y + ef1.Height) - single4);
                            this.myPoints[4] = new PointF(ef1.X + ef1.Width, ef1.Y + ef1.Height);
                            this.myPoints[5] = new PointF(ef1.X + single1, ef1.Y + ef1.Height);
                        }
                    }
                    else if (single2 >= 0f)
                    {
                        this.myPoints[0] = new PointF(ef1.X - single1, ef1.Y + single3);
                        this.myPoints[1] = new PointF(ef1.X, ef1.Y);
                        this.myPoints[2] = new PointF((ef1.X + ef1.Width) - single2, ef1.Y);
                        this.myPoints[3] = new PointF(ef1.X + ef1.Width, (ef1.Y + ef1.Height) - single4);
                        this.myPoints[4] = new PointF((ef1.X + ef1.Width) - single2, ef1.Y + ef1.Height);
                        this.myPoints[5] = new PointF(ef1.X, ef1.Y + ef1.Height);
                    }
                    else
                    {
                        this.myPoints[0] = new PointF(ef1.X - single1, ef1.Y + single3);
                        this.myPoints[1] = new PointF(ef1.X, ef1.Y);
                        this.myPoints[2] = new PointF(ef1.X + ef1.Width, ef1.Y);
                        this.myPoints[3] = new PointF((ef1.X + ef1.Width) + single2, (ef1.Y + ef1.Height) - single4);
                        this.myPoints[4] = new PointF(ef1.X + ef1.Width, ef1.Y + ef1.Height);
                        this.myPoints[5] = new PointF(ef1.X, ef1.Y + ef1.Height);
                    }
                }
            }
            else
            {
                if (single1 > ef1.Width)
                {
                    single1 = ef1.Width;
                }
                if (single1 < 0f)
                {
                    single1 = 0f;
                }
                if (single2 > ef1.Width)
                {
                    single2 = ef1.Width;
                }
                if (single2 < 0f)
                {
                    single2 = 0f;
                }
                if (this.KeepsCrosswiseSymmetry)
                {
                    if (single3 < -(ef1.Height / 2f))
                    {
                        single3 = -(ef1.Height / 2f);
                    }
                    if (single3 > (ef1.Height / 2f))
                    {
                        single3 = ef1.Height / 2f;
                    }
                    if (single4 < -(ef1.Height / 2f))
                    {
                        single4 = -(ef1.Height / 2f);
                    }
                    if (single4 > (ef1.Height / 2f))
                    {
                        single4 = ef1.Height / 2f;
                    }
                    if (single3 >= 0f)
                    {
                        this.myPoints[0] = new PointF(ef1.X + single1, ef1.Y);
                        this.myPoints[1] = new PointF(ef1.X + ef1.Width, ef1.Y + single3);
                        this.myPoints[2] = new PointF(ef1.X + ef1.Width, (ef1.Y + ef1.Height) - single3);
                        this.myPoints[3] = new PointF(ef1.X + single1, ef1.Y + ef1.Height);
                        this.myPoints[4] = new PointF(ef1.X, (ef1.Y + ef1.Height) - single3);
                        this.myPoints[5] = new PointF(ef1.X, ef1.Y + single3);
                    }
                    else
                    {
                        this.myPoints[0] = new PointF(ef1.X + single1, ef1.Y - single3);
                        this.myPoints[1] = new PointF(ef1.X + ef1.Width, ef1.Y);
                        this.myPoints[2] = new PointF(ef1.X + ef1.Width, ef1.Y + ef1.Height);
                        this.myPoints[3] = new PointF(ef1.X + single1, (ef1.Y + ef1.Height) + single3);
                        this.myPoints[4] = new PointF(ef1.X, ef1.Y + ef1.Height);
                        this.myPoints[5] = new PointF(ef1.X, ef1.Y);
                    }
                }
                else
                {
                    if (System.Math.Abs(single3) > ef1.Height)
                    {
                        if (single3 <= 0f)
                        {
                            single3 = -ef1.Height;
                        }
                        else
                        {
                            single3 = ef1.Height;
                        }
                        single4 = 0f;
                    }
                    if (single4 < -(ef1.Height - System.Math.Abs(single3)))
                    {
                        single4 = -(ef1.Height - System.Math.Abs(single3));
                    }
                    if (single4 > (ef1.Height - System.Math.Abs(single3)))
                    {
                        single4 = ef1.Height - System.Math.Abs(single3);
                    }
                    if (single3 >= 0f)
                    {
                        if (single4 >= 0f)
                        {
                            this.myPoints[0] = new PointF(ef1.X + single1, ef1.Y);
                            this.myPoints[1] = new PointF(ef1.X + ef1.Width, ef1.Y + single3);
                            this.myPoints[2] = new PointF(ef1.X + ef1.Width, (ef1.Y + ef1.Height) - single4);
                            this.myPoints[3] = new PointF((ef1.X + ef1.Width) - single2, ef1.Y + ef1.Height);
                            this.myPoints[4] = new PointF(ef1.X, (ef1.Y + ef1.Height) - single4);
                            this.myPoints[5] = new PointF(ef1.X, ef1.Y + single3);
                        }
                        else
                        {
                            this.myPoints[0] = new PointF(ef1.X + single1, ef1.Y);
                            this.myPoints[1] = new PointF(ef1.X + ef1.Width, ef1.Y + single3);
                            this.myPoints[2] = new PointF(ef1.X + ef1.Width, ef1.Y + ef1.Height);
                            this.myPoints[3] = new PointF((ef1.X + ef1.Width) - single2, (ef1.Y + ef1.Height) + single4);
                            this.myPoints[4] = new PointF(ef1.X, ef1.Y + ef1.Height);
                            this.myPoints[5] = new PointF(ef1.X, ef1.Y + single3);
                        }
                    }
                    else if (single4 >= 0f)
                    {
                        this.myPoints[0] = new PointF(ef1.X + single1, ef1.Y - single3);
                        this.myPoints[1] = new PointF(ef1.X + ef1.Width, ef1.Y);
                        this.myPoints[2] = new PointF(ef1.X + ef1.Width, (ef1.Y + ef1.Height) - single4);
                        this.myPoints[3] = new PointF((ef1.X + ef1.Width) - single2, ef1.Y + ef1.Height);
                        this.myPoints[4] = new PointF(ef1.X, (ef1.Y + ef1.Height) - single4);
                        this.myPoints[5] = new PointF(ef1.X, ef1.Y);
                    }
                    else
                    {
                        this.myPoints[0] = new PointF(ef1.X + single1, ef1.Y - single3);
                        this.myPoints[1] = new PointF(ef1.X + ef1.Width, ef1.Y);
                        this.myPoints[2] = new PointF(ef1.X + ef1.Width, ef1.Y + ef1.Height);
                        this.myPoints[3] = new PointF((ef1.X + ef1.Width) - single2, (ef1.Y + ef1.Height) + single4);
                        this.myPoints[4] = new PointF(ef1.X, ef1.Y + ef1.Height);
                        this.myPoints[5] = new PointF(ef1.X, ef1.Y);
                    }
                }
            }
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
            SizeF ef1 = this.GetShadowOffset(view);
            PointF[] tfArray1 = this.getPoints();
            if (this.Shadowed)
            {
                int num1 = tfArray1.Length;
                for (int num2 = 0; num2 < num1; num2++)
                {
                    this.myPoints[num2].X += ef1.Width;
                    this.myPoints[num2].Y += ef1.Height;
                }
                if (this.Brush != null)
                {
                    Brush brush1 = this.GetShadowBrush(view);
                    DiagramGraph.DrawPolygon(g, view, null, brush1, this.myPoints);
                }
                else if (this.Pen != null)
                {
                    Pen pen1 = this.GetShadowPen(view, base.InternalPenWidth);
                    DiagramGraph.DrawPolygon(g, view, pen1, null, this.myPoints);
                }
                for (int num3 = 0; num3 < num1; num3++)
                {
                    this.myPoints[num3].X -= ef1.Width;
                    this.myPoints[num3].Y -= ef1.Height;
                }
            }
            DiagramGraph.DrawPolygon(g, view, this.Pen, this.Brush, this.myPoints);
        }


        [Description("The distance between the right/bottom point and the Hexagon's bottom border."), DefaultValue((float)10f), Category("Appearance")]
        public virtual float DistanceBottom
        {
            get
            {
                return this.myDistanceBottom;
            }
            set
            {
                float single1 = this.myDistanceBottom;
                if (single1 != value)
                {
                    this.myDistanceBottom = value;
                    if (this.KeepsCrosswiseSymmetry)
                    {
                        if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                        {
                            this.DistanceTop = value;
                        }
                        else
                        {
                            this.DistanceTop = this.Bounds.Height - value;
                        }
                    }
                    if (this.KeepsLengthwiseSymmetry && (this.Orientation == System.Windows.Forms.Orientation.Horizontal))
                    {
                        this.myDistanceBottom = this.Bounds.Height / 2f;
                        this.DistanceTop = this.Bounds.Height / 2f;
                    }
                    base.ResetPath();
                    if (single1 != this.myDistanceBottom)
                    {
                        this.Changed(0x5a5, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(this.myDistanceBottom));
                    }
                }
            }
        }

        [DefaultValue((float)10f), Description("The distance between the left/top point and the Hexagon's left border."), Category("Appearance")]
        public virtual float DistanceLeft
        {
            get
            {
                return this.myDistanceLeft;
            }
            set
            {
                float single1 = this.myDistanceLeft;
                if (single1 != value)
                {
                    this.myDistanceLeft = value;
                    if (this.KeepsCrosswiseSymmetry)
                    {
                        if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                        {
                            this.DistanceRight = value;
                        }
                        else
                        {
                            this.DistanceRight = this.Bounds.Width - value;
                        }
                    }
                    if (this.KeepsLengthwiseSymmetry && (this.Orientation == System.Windows.Forms.Orientation.Vertical))
                    {
                        this.myDistanceLeft = this.Bounds.Width / 2f;
                        this.DistanceRight = this.Bounds.Width / 2f;
                    }
                    base.ResetPath();
                    if (single1 != this.myDistanceLeft)
                    {
                        this.Changed(0x5a2, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(this.myDistanceLeft));
                    }
                }
            }
        }

        [Category("Appearance"), Description("The distance between the right/bottom point and the Hexagon's right border."), DefaultValue((float)10f)]
        public virtual float DistanceRight
        {
            get
            {
                return this.myDistanceRight;
            }
            set
            {
                float single1 = this.myDistanceRight;
                if (single1 != value)
                {
                    this.myDistanceRight = value;
                    if (this.KeepsCrosswiseSymmetry)
                    {
                        if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                        {
                            this.DistanceLeft = value;
                        }
                        else
                        {
                            this.DistanceLeft = this.Bounds.Width - value;
                        }
                    }
                    if (this.KeepsLengthwiseSymmetry && (this.Orientation == System.Windows.Forms.Orientation.Vertical))
                    {
                        this.myDistanceRight = this.Bounds.Width / 2f;
                        this.DistanceLeft = this.Bounds.Width / 2f;
                    }
                    base.ResetPath();
                    if (single1 != this.myDistanceRight)
                    {
                        this.Changed(0x5a3, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(this.myDistanceRight));
                    }
                }
            }
        }

        [DefaultValue((float)10f), Category("Appearance"), Description("The distance between the left/top point and the Hexagon's top border.")]
        public virtual float DistanceTop
        {
            get
            {
                return this.myDistanceTop;
            }
            set
            {
                float single1 = this.myDistanceTop;
                if (single1 != value)
                {
                    this.myDistanceTop = value;
                    if (this.KeepsCrosswiseSymmetry)
                    {
                        if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                        {
                            this.DistanceBottom = value;
                        }
                        else
                        {
                            this.DistanceBottom = this.Bounds.Height - value;
                        }
                    }
                    if (this.KeepsLengthwiseSymmetry && (this.Orientation == System.Windows.Forms.Orientation.Horizontal))
                    {
                        this.myDistanceTop = this.Bounds.Height / 2f;
                        this.DistanceBottom = this.Bounds.Height / 2f;
                    }
                    base.ResetPath();
                    if (single1 != this.myDistanceTop)
                    {
                        this.Changed(0x5a4, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(this.myDistanceTop));
                    }
                }
            }
        }

        [DefaultValue(true), Description("Whether to maintain symmetry in respect to the crosswise axis."), Category("Appearance")]
        public virtual bool KeepsCrosswiseSymmetry
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
                        if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                        {
                            this.DistanceBottom = this.DistanceTop;
                            this.DistanceRight = base.Width - ((this.DistanceLeft < 0f) ? 0f : this.DistanceLeft);
                        }
                        else
                        {
                            this.DistanceRight = this.DistanceLeft;
                            this.DistanceBottom = base.Height - ((this.DistanceTop < 0f) ? 0f : this.DistanceTop);
                        }
                    }
                    else
                    {
                        base.InternalFlags &= -2097153;
                    }
                    this.Changed(1450, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Whether to maintain symmetry in respect to the lengthwise axis.")]
        public virtual bool KeepsLengthwiseSymmetry
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
                        if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                        {
                            this.DistanceLeft = this.Bounds.Width / 2f;
                            this.DistanceRight = this.Bounds.Width / 2f;
                        }
                        else
                        {
                            this.DistanceTop = this.Bounds.Height / 2f;
                            this.DistanceBottom = this.Bounds.Height / 2f;
                        }
                    }
                    else
                    {
                        base.InternalFlags &= -4194305;
                    }
                    this.Changed(0x5a9, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(0), Description("Whether the pair of parallel lines run vertically or horizontally"), Category("Appearance")]
        public virtual System.Windows.Forms.Orientation Orientation
        {
            get
            {
                return this.myOrientation;
            }
            set
            {
                System.Windows.Forms.Orientation orientation1 = this.myOrientation;
                if (orientation1 != value)
                {
                    this.myOrientation = value;
                    if (this.KeepsCrosswiseSymmetry)
                    {
                        if (value == System.Windows.Forms.Orientation.Vertical)
                        {
                            this.DistanceRight = this.Bounds.Width - ((this.DistanceLeft < 0f) ? 0f : this.DistanceLeft);
                            this.DistanceBottom = this.DistanceTop;
                        }
                        else
                        {
                            this.DistanceBottom = this.Bounds.Height - ((this.DistanceTop < 0f) ? 0f : this.DistanceTop);
                            this.DistanceRight = this.DistanceLeft;
                        }
                    }
                    if (this.KeepsLengthwiseSymmetry)
                    {
                        if (value == System.Windows.Forms.Orientation.Vertical)
                        {
                            this.DistanceLeft = this.Bounds.Width / 2f;
                            this.DistanceRight = this.Bounds.Width / 2f;
                        }
                        else
                        {
                            this.DistanceTop = this.Bounds.Height / 2f;
                            this.DistanceBottom = this.Bounds.Height / 2f;
                        }
                    }
                    base.ResetPath();
                    this.Changed(0x5a6, 0, orientation1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(true), Category("Behavior"), Description("Whether users can reshape the corner of this resizable object.")]
        public virtual bool ReshapableCorner
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
                    this.Changed(0x5a8, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Behavior"), DefaultValue(3), Description("What kind of symmetry to maintain when reshaping")]
        public virtual HexagonGraphReshapeBehavior ReshapeBehavior
        {
            get
            {
                return this.myReshapeBehavior;
            }
            set
            {
                HexagonGraphReshapeBehavior behavior1 = this.myReshapeBehavior;
                if (behavior1 != value)
                {
                    this.myReshapeBehavior = value;
                    this.Changed(0x5a7, 0, behavior1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedDistanceBottom = 0x5a5;
        public const int ChangedDistanceLeft = 0x5a2;
        public const int ChangedDistanceRight = 0x5a3;
        public const int ChangedDistanceTop = 0x5a4;
        public const int ChangedKeepsCrosswiseSymmetry = 1450;
        public const int ChangedKeepsLengthwiseSymmetry = 0x5a9;
        public const int ChangedOrientation = 0x5a6;
        public const int ChangedReshapableCorner = 0x5a8;
        public const int ChangedReshapeBehavior = 0x5a7;
        private const int flagCrosswiseSymmetry = 0x200000;
        private const int flagLengthwiseSymmetry = 0x400000;
        private const int flagReshapableCorner = 0x100000;
        public const int LeftTopPointHandleID = 0x404;
        public const int LeftTopSideHandleID = 0x402;
        private float myDistanceBottom;
        private float myDistanceLeft;
        private float myDistanceRight;
        private float myDistanceTop;
        private System.Windows.Forms.Orientation myOrientation;
        private PointF[] myPoints;
        private HexagonGraphReshapeBehavior myReshapeBehavior;
        public const int RightBottomPointHandleID = 0x405;
        public const int RightBottomSideHandleID = 0x403;
    }
}
