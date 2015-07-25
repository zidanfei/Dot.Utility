using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class TrapezoidGraph : DiagramGraph
    {
        public TrapezoidGraph()
        {
            this.myPoints = new PointF[4];
            this.myOrientation = System.Windows.Forms.Orientation.Horizontal;
            base.InternalFlags |= 0x200;
            this.myPoints[1] = new PointF(8f, 0f);
            this.myPoints[2] = new PointF(10f, 10f);
            this.myPoints[3] = new PointF(0f, 10f);
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            if (!this.CanResize() || !this.CanReshape())
            {
                base.AddSelectionHandles(sel, selectedObj);
            }
            else
            {
                System.Windows.Forms.Orientation orientation1 = this.Orientation;
                PointF tf2 = new PointF();
                PointF tf3 = new PointF();
                PointF tf4 = new PointF();
                PointF tf5 = new PointF();
                bool flag1 = this.A.X <= this.B.X;
                bool flag2 = this.A.Y <= this.D.Y;
                sel.RemoveHandles(this);
                if (flag1 && flag2)
                {
                    tf2 = this.A;
                    tf3 = this.B;
                    tf4 = this.D;
                    tf5 = this.C;
                }
                else if (!flag1 && flag2)
                {
                    tf2 = this.B;
                    tf3 = this.A;
                    tf4 = this.C;
                    tf5 = this.D;
                }
                else if (flag1 && !flag2)
                {
                    tf2 = this.D;
                    tf3 = this.C;
                    tf4 = this.A;
                    tf5 = this.B;
                }
                else
                {
                    tf2 = this.C;
                    tf3 = this.D;
                    tf4 = this.B;
                    tf5 = this.A;
                }
                PointF tf1 = new PointF((tf2.X + tf4.X) / 2f, (tf2.Y + tf4.Y) / 2f);
                sel.CreateResizeHandle(this, selectedObj, tf1, 0x100, true);
                tf1 = new PointF((tf3.X + tf5.X) / 2f, (tf3.Y + tf5.Y) / 2f);
                sel.CreateResizeHandle(this, selectedObj, tf1, 0x40, true);
                tf1 = new PointF((tf2.X + tf3.X) / 2f, (tf2.Y + tf3.Y) / 2f);
                sel.CreateResizeHandle(this, selectedObj, tf1, 0x20, true);
                tf1 = new PointF((tf4.X + tf5.X) / 2f, (tf4.Y + tf5.Y) / 2f);
                sel.CreateResizeHandle(this, selectedObj, tf1, 0x80, true);
                sel.CreateResizeHandle(this, selectedObj, this.A, 0x40a, true);
                sel.CreateResizeHandle(this, selectedObj, this.B, 0x40b, true);
                sel.CreateResizeHandle(this, selectedObj, this.C, 0x40c, true);
                sel.CreateResizeHandle(this, selectedObj, this.D, 0x40d, true);
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 1460:
                    {
                        this.A = e.GetPoint(undo);
                        return;
                    }
                case 0x5b5:
                    {
                        this.B = e.GetPoint(undo);
                        return;
                    }
                case 0x5b6:
                    {
                        this.C = e.GetPoint(undo);
                        return;
                    }
                case 0x5b7:
                    {
                        this.D = e.GetPoint(undo);
                        return;
                    }
                case 0x5b8:
                    {
                        this.SetPoints((PointF[])e.GetValue(undo));
                        return;
                    }
                case 0x5b9:
                    {
                        this.Orientation = (System.Windows.Forms.Orientation)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override RectangleF ComputeBounds()
        {
            float single1 = this.myPoints[0].X;
            float single2 = single1;
            if (this.myPoints[1].X < single1)
            {
                single1 = this.myPoints[1].X;
            }
            else if (this.myPoints[1].X > single2)
            {
                single2 = this.myPoints[1].X;
            }
            if (this.myPoints[2].X < single1)
            {
                single1 = this.myPoints[2].X;
            }
            else if (this.myPoints[2].X > single2)
            {
                single2 = this.myPoints[2].X;
            }
            if (this.myPoints[3].X < single1)
            {
                single1 = this.myPoints[3].X;
            }
            else if (this.myPoints[3].X > single2)
            {
                single2 = this.myPoints[3].X;
            }
            float single3 = this.myPoints[0].Y;
            float single4 = single3;
            if (this.myPoints[1].Y < single3)
            {
                single3 = this.myPoints[1].Y;
            }
            else if (this.myPoints[1].Y > single4)
            {
                single4 = this.myPoints[1].Y;
            }
            if (this.myPoints[2].Y < single3)
            {
                single3 = this.myPoints[2].Y;
            }
            else if (this.myPoints[2].Y > single4)
            {
                single4 = this.myPoints[2].Y;
            }
            if (this.myPoints[3].Y < single3)
            {
                single3 = this.myPoints[3].Y;
            }
            else if (this.myPoints[3].Y > single4)
            {
                single4 = this.myPoints[3].Y;
            }
            return new RectangleF(single1, single3, single2 - single1, single4 - single3);
        }

        public override RectangleF ComputeResize(RectangleF origRect, PointF newPoint, int handle, SizeF min, SizeF max, bool reshape)
        {
            float single5;
            if (handle <= 0x10)
            {
                return base.ComputeResize(origRect, newPoint, handle, min, max, reshape);
            }
            float single1 = origRect.X;
            float single2 = origRect.Y;
            float single3 = origRect.X + origRect.Width;
            float single4 = origRect.Y + origRect.Height;
            RectangleF ef1 = origRect;
            int num1 = handle;
            if (num1 <= 0x40)
            {
                if (num1 == 0x20)
                {
                    if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        single5 = 0f;
                    }
                    else
                    {
                        single5 = System.Math.Abs((float)(this.A.Y - this.B.Y)) / 2f;
                    }
                    ef1.Y = System.Math.Max((float)(newPoint.Y - single5), (float)(single4 - max.Height));
                    ef1.Y = System.Math.Min(ef1.Y, (float)(single4 - min.Height));
                    ef1.Height = single4 - ef1.Y;
                    if (ef1.Height <= 0f)
                    {
                        ef1.Height = 1f;
                    }
                    return ef1;
                }
                if (num1 == 0x40)
                {
                    if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        single5 = System.Math.Abs((float)(this.B.X - this.C.X)) / 2f;
                    }
                    else
                    {
                        single5 = 0f;
                    }
                    ef1.Width = System.Math.Min((float)((newPoint.X + single5) - single1), max.Width);
                    ef1.Width = System.Math.Max(ef1.Width, min.Width);
                }
                return ef1;
            }
            if (num1 == 0x80)
            {
                if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    single5 = 0f;
                }
                else
                {
                    single5 = System.Math.Abs((float)(this.C.Y - this.D.Y)) / 2f;
                }
                ef1.Height = System.Math.Min((float)(newPoint.Y - single2), max.Height);
                ef1.Height = System.Math.Max(ef1.Height, min.Height);
                return ef1;
            }
            if (num1 == 0x100)
            {
                if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    single5 = System.Math.Abs((float)(this.A.X - this.D.X)) / 2f;
                }
                else
                {
                    single5 = 0f;
                }
                ef1.X = System.Math.Max((float)(newPoint.X - single5), (float)(single3 - max.Width));
                ef1.X = System.Math.Min(ef1.X, (float)(single3 - min.Width));
                ef1.Width = single3 - ef1.X;
                if (ef1.Width <= 0f)
                {
                    ef1.Width = 1f;
                }
            }
            return ef1;
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

        public override void CopyNewValueForRedo(ChangedEventArgs e)
        {
            if (e.SubHint == 0x5b8)
            {
                e.NewValue = this.myPoints.Clone();
            }
            else
            {
                base.CopyNewValueForRedo(e);
            }
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            TrapezoidGraph trapezoid1 = (TrapezoidGraph)base.CopyObject(env);
            if (trapezoid1 != null)
            {
                trapezoid1.myPoints = (PointF[])this.myPoints.Clone();
            }
            return trapezoid1;
        }

        public override void CopyOldValueForUndo(ChangedEventArgs e)
        {
            if (e.SubHint == 0x5b8)
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

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            float single2;
            PointF tf9;
            PointF tf10;
            PointF tf11;
            PointF tf12;
            bool flag4;
            bool flag5;
            int num1;
            bool flag1 = this.Orientation == System.Windows.Forms.Orientation.Horizontal;
            if ((((whichHandle < 0x40a) || (whichHandle > 0x40d)) || !this.CanReshape()) || ((!this.ResizesRealtime && (evttype != InputState.Finish)) && (evttype != InputState.Cancel)))
            {
                if ((flag1 && ((whichHandle == 0x100) || (whichHandle == 0x40))) && (this.CanReshape() && ((this.ResizesRealtime || (evttype == InputState.Finish)) || (evttype == InputState.Cancel))))
                {
                    PointF tf5 = new PointF();
                    PointF tf6 = new PointF();
                    PointF tf7 = new PointF();
                    PointF tf8 = new PointF();
                    bool flag2 = this.A.X <= this.B.X;
                    bool flag3 = true;
                    num1 = whichHandle;
                    if (num1 != 0x40)
                    {
                        if (num1 == 0x100)
                        {
                            if (flag2)
                            {
                                tf5 = this.A;
                                tf6 = this.D;
                                tf7 = this.B;
                                tf8 = this.C;
                            }
                            else
                            {
                                tf5 = this.B;
                                tf6 = this.C;
                                tf7 = this.A;
                                tf8 = this.D;
                            }
                            flag3 = true;
                        }
                    }
                    else
                    {
                        if (flag2)
                        {
                            tf5 = this.B;
                            tf6 = this.C;
                            tf7 = this.A;
                            tf8 = this.D;
                        }
                        else
                        {
                            tf5 = this.A;
                            tf6 = this.D;
                            tf7 = this.B;
                            tf8 = this.C;
                        }
                        flag3 = false;
                    }
                    float single1 = tf5.X - tf6.X;
                    tf5.X = newPoint.X + (single1 / 2f);
                    tf6.X = newPoint.X - (single1 / 2f);
                    if (flag3)
                    {
                        if (tf5.X > tf7.X)
                        {
                            tf5.X = tf7.X;
                            tf6.X = tf5.X - single1;
                        }
                        if (tf6.X > tf8.X)
                        {
                            tf6.X = tf8.X;
                            tf5.X = tf6.X + single1;
                        }
                    }
                    else
                    {
                        if (tf5.X <= tf7.X)
                        {
                            tf5.X = tf7.X;
                            tf6.X = tf5.X - single1;
                        }
                        if (tf6.X < tf8.X)
                        {
                            tf6.X = tf8.X;
                            tf5.X = tf6.X + single1;
                        }
                    }
                    if (flag3)
                    {
                        if (flag2)
                        {
                            this.A = tf5;
                            this.D = tf6;
                            return;
                        }
                        this.B = tf5;
                        this.C = tf6;
                        return;
                    }
                    if (flag2)
                    {
                        this.B = tf5;
                        this.C = tf6;
                        return;
                    }
                    this.A = tf5;
                    this.D = tf6;
                    return;
                }
                if ((flag1 || ((whichHandle != 0x20) && (whichHandle != 0x80))) || (!this.CanReshape() || ((!this.ResizesRealtime && (evttype != InputState.Finish)) && (evttype != InputState.Cancel))))
                {
                    base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
                    return;
                }
                tf9 = new PointF();
                tf10 = new PointF();
                tf11 = new PointF();
                tf12 = new PointF();
                flag4 = this.A.Y <= this.D.Y;
                flag5 = true;
                num1 = whichHandle;
                if (num1 != 0x20)
                {
                    if (num1 == 0x80)
                    {
                        if (flag4)
                        {
                            tf9 = this.D;
                            tf10 = this.C;
                            tf11 = this.A;
                            tf12 = this.B;
                        }
                        else
                        {
                            tf9 = this.A;
                            tf10 = this.B;
                            tf11 = this.D;
                            tf12 = this.C;
                        }
                        flag5 = false;
                    }
                    goto Label_0553;
                }
                if (flag4)
                {
                    tf9 = this.A;
                    tf10 = this.B;
                    tf11 = this.D;
                    tf12 = this.C;
                }
                else
                {
                    tf9 = this.D;
                    tf10 = this.C;
                    tf11 = this.A;
                    tf12 = this.B;
                }
                flag5 = true;
                goto Label_0553;
            }
            PointF tf1 = this.A;
            PointF tf2 = this.B;
            PointF tf3 = this.C;
            PointF tf4 = this.D;
            num1 = whichHandle;
            switch (num1)
            {
                case 0x40a:
                    {
                        tf1 = newPoint;
                        if (!flag1)
                        {
                            if (tf1.Y > tf4.Y)
                            {
                                tf1.Y = tf4.Y;
                            }
                            break;
                        }
                        if (tf1.X > tf2.X)
                        {
                            tf1.X = tf2.X;
                        }
                        break;
                    }
                case 0x40b:
                    {
                        tf2 = newPoint;
                        if (!flag1)
                        {
                            if (tf2.Y > tf3.Y)
                            {
                                tf2.Y = tf3.Y;
                            }
                            goto Label_010E;
                        }
                        if (tf2.X < tf1.X)
                        {
                            tf2.X = tf1.X;
                        }
                        goto Label_010E;
                    }
                case 0x40c:
                    {
                        tf3 = newPoint;
                        if (!flag1)
                        {
                            if (tf3.Y < tf2.Y)
                            {
                                tf3.Y = tf2.Y;
                            }
                            goto Label_0159;
                        }
                        if (tf3.X < tf4.X)
                        {
                            tf3.X = tf4.X;
                        }
                        goto Label_0159;
                    }
                case 0x40d:
                    {
                        tf4 = newPoint;
                        if (!flag1)
                        {
                            if (tf4.Y < tf1.Y)
                            {
                                tf4.Y = tf1.Y;
                            }
                            goto Label_01A5;
                        }
                        if (tf4.X > tf3.X)
                        {
                            tf4.X = tf3.X;
                        }
                        goto Label_01A5;
                    }
                default:
                    {
                        return;
                    }
            }
            this.A = tf1;
            return;
        Label_010E:
            this.B = tf2;
            return;
        Label_0159:
            this.C = tf3;
            return;
        Label_01A5:
            this.D = tf4;
            return;
        Label_0553:
            single2 = tf9.Y - tf10.Y;
            tf9.Y = newPoint.Y + (single2 / 2f);
            tf10.Y = newPoint.Y - (single2 / 2f);
            if (flag5)
            {
                if (tf9.Y > tf11.Y)
                {
                    tf9.Y = tf11.Y;
                    tf10.Y = tf9.Y - single2;
                }
                if (tf10.Y > tf12.Y)
                {
                    tf10.Y = tf12.Y;
                    tf9.Y = tf10.Y + single2;
                }
            }
            else
            {
                if (tf9.Y < tf11.Y)
                {
                    tf9.Y = tf11.Y;
                    tf10.Y = tf9.Y - single2;
                }
                if (tf10.Y < tf12.Y)
                {
                    tf10.Y = tf12.Y;
                    tf9.Y = tf10.Y + single2;
                }
            }
            if (flag5)
            {
                if (flag4)
                {
                    this.A = tf9;
                    this.B = tf10;
                }
                else
                {
                    this.D = tf9;
                    this.C = tf10;
                }
            }
            else if (flag4)
            {
                this.D = tf9;
                this.C = tf10;
            }
            else
            {
                this.A = tf9;
                this.B = tf10;
            }
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            PointF tf5;
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            PointF tf1 = DiagramGraph.ExpandPointOnEdge(this.myPoints[0], ef1, single1);
            PointF tf2 = DiagramGraph.ExpandPointOnEdge(this.myPoints[1], ef1, single1);
            PointF tf3 = DiagramGraph.ExpandPointOnEdge(this.myPoints[2], ef1, single1);
            PointF tf4 = DiagramGraph.ExpandPointOnEdge(this.myPoints[3], ef1, single1);
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

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
            path1.AddLines(this.myPoints);
            path1.CloseAllFigures();
            return path1;
        }

        protected override void OnBoundsChanged(RectangleF old)
        {
            base.OnBoundsChanged(old);
            SizeF ef1 = base.Size;
            if ((old.Width == ef1.Width) && (old.Height == ef1.Height))
            {
                RectangleF ef2 = this.Bounds;
                float single1 = ef2.X - old.X;
                float single2 = ef2.Y - old.Y;
                if ((single1 != 0f) || (single2 != 0f))
                {
                    this.Changing(0x5b8);
                    bool flag1 = base.SuspendsUpdates;
                    base.SuspendsUpdates = true;
                    PointF tf1 = this.A;
                    tf1.X += single1;
                    tf1.Y += single2;
                    PointF tf2 = this.B;
                    tf2.X += single1;
                    tf2.Y += single2;
                    PointF tf3 = this.C;
                    tf3.X += single1;
                    tf3.Y += single2;
                    PointF tf4 = this.D;
                    tf4.X += single1;
                    tf4.Y += single2;
                    this.A = tf1;
                    this.B = tf2;
                    this.C = tf3;
                    this.D = tf4;
                    base.InvalidBounds = false;
                    base.SuspendsUpdates = flag1;
                    if (flag1)
                    {
                        return;
                    }
                    this.Changed(0x5b8, 0, null, old, 0, null, ef2);
                }
            }
            else
            {
                RectangleF ef3 = this.Bounds;
                float single3 = 1f;
                if (old.Width != 0f)
                {
                    single3 = ef3.Width / old.Width;
                }
                float single4 = 1f;
                if (old.Height != 0f)
                {
                    single4 = ef3.Height / old.Height;
                }
                this.Changing(0x5b8);
                bool flag2 = base.SuspendsUpdates;
                base.SuspendsUpdates = true;
                PointF tf5 = this.A;
                tf5.X = ef3.X + ((tf5.X - old.X) * single3);
                tf5.Y = ef3.Y + ((tf5.Y - old.Y) * single4);
                PointF tf6 = this.B;
                tf6.X = ef3.X + ((tf6.X - old.X) * single3);
                tf6.Y = ef3.Y + ((tf6.Y - old.Y) * single4);
                PointF tf7 = this.C;
                tf7.X = ef3.X + ((tf7.X - old.X) * single3);
                tf7.Y = ef3.Y + ((tf7.Y - old.Y) * single4);
                PointF tf8 = this.D;
                tf8.X = ef3.X + ((tf8.X - old.X) * single3);
                tf8.Y = ef3.Y + ((tf8.Y - old.Y) * single4);
                this.A = tf5;
                this.B = tf6;
                this.C = tf7;
                this.D = tf8;
                base.InvalidBounds = false;
                base.SuspendsUpdates = flag2;
                if (!flag2)
                {
                    this.Changed(0x5b8, 0, null, old, 0, null, ef3);
                }
            }
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            SizeF ef1 = this.GetShadowOffset(view);
            if (this.Shadowed)
            {
                int num1 = this.myPoints.Length;
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

        public virtual void SetPoints(PointF[] points)
        {
            if ((points == null) || (points.Length != 4))
            {
                throw new ArgumentException("Trapezoids always have four points");
            }
            if (((points[0] != this.myPoints[0]) || (points[1] != this.myPoints[1])) || ((points[2] != this.myPoints[2]) || (points[3] != this.myPoints[3])))
            {
                this.Changing(0x5b8);
                base.ResetPath();
                Array.Copy(points, 0, this.myPoints, 0, 4);
                base.InvalidBounds = true;
                this.Changed(0x5b8, 0, null, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
            }
        }


        [Description("The first point in this trapezoid."), Category("Bounds"), TypeConverter(typeof(PointFConverter))]
        public PointF A
        {
            get
            {
                return this.myPoints[0];
            }
            set
            {
                PointF tf1 = this.myPoints[0];
                if (tf1 != value)
                {
                    this.myPoints[0] = value;
                    PointF tf2 = new PointF();
                    if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        tf2 = this.B;
                        if (this.myPoints[0].X > this.B.X)
                        {
                            tf2.X = this.myPoints[0].X;
                        }
                        tf2.Y = this.myPoints[0].Y;
                        this.B = tf2;
                    }
                    else
                    {
                        tf2 = this.D;
                        if (this.myPoints[0].Y > this.D.Y)
                        {
                            tf2.Y = this.myPoints[0].Y;
                        }
                        tf2.X = this.myPoints[0].X;
                        this.D = tf2;
                    }
                    base.InvalidBounds = true;
                    base.ResetPath();
                    this.Changed(1460, 0, null, DiagramShape.MakeRect(tf1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Category("Bounds"), TypeConverter(typeof(PointFConverter)), Description("The second point in this trapezoid.")]
        public PointF B
        {
            get
            {
                return this.myPoints[1];
            }
            set
            {
                PointF tf1 = this.myPoints[1];
                if (tf1 != value)
                {
                    this.myPoints[1] = value;
                    PointF tf2 = new PointF();
                    if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        tf2 = this.A;
                        if (this.myPoints[1].X < this.A.X)
                        {
                            tf2.X = this.myPoints[1].X;
                        }
                        tf2.Y = this.myPoints[1].Y;
                        this.A = tf2;
                    }
                    else
                    {
                        tf2 = this.C;
                        if (this.myPoints[1].Y > this.C.Y)
                        {
                            tf2.Y = this.myPoints[1].Y;
                        }
                        tf2.X = this.myPoints[1].X;
                        this.C = tf2;
                    }
                    base.InvalidBounds = true;
                    base.ResetPath();
                    this.Changed(0x5b5, 0, null, DiagramShape.MakeRect(tf1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("The third point in this trapezoid."), TypeConverter(typeof(PointFConverter)), Category("Bounds")]
        public PointF C
        {
            get
            {
                return this.myPoints[2];
            }
            set
            {
                PointF tf1 = this.myPoints[2];
                if (tf1 != value)
                {
                    this.myPoints[2] = value;
                    PointF tf2 = new PointF();
                    if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        tf2 = this.D;
                        if (this.myPoints[2].X < this.D.X)
                        {
                            tf2.X = this.myPoints[2].X;
                        }
                        tf2.Y = this.myPoints[2].Y;
                        this.D = tf2;
                    }
                    else
                    {
                        tf2 = this.B;
                        if (this.myPoints[2].Y < this.B.Y)
                        {
                            tf2.Y = this.myPoints[2].Y;
                        }
                        tf2.X = this.myPoints[2].X;
                        this.B = tf2;
                    }
                    base.InvalidBounds = true;
                    base.ResetPath();
                    this.Changed(0x5b6, 0, null, DiagramShape.MakeRect(tf1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("The fourth point in this trapezoid."), Category("Bounds"), TypeConverter(typeof(PointFConverter))]
        public PointF D
        {
            get
            {
                return this.myPoints[3];
            }
            set
            {
                PointF tf1 = this.myPoints[3];
                if (tf1 != value)
                {
                    this.myPoints[3] = value;
                    PointF tf2 = new PointF();
                    if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        tf2 = this.C;
                        if (this.myPoints[3].X > this.C.X)
                        {
                            tf2.X = this.myPoints[3].X;
                        }
                        tf2.Y = this.myPoints[3].Y;
                        this.C = tf2;
                    }
                    else
                    {
                        tf2 = this.A;
                        if (this.myPoints[3].Y < this.A.Y)
                        {
                            tf2.Y = this.myPoints[3].Y;
                        }
                        tf2.X = this.myPoints[3].X;
                        this.A = tf2;
                    }
                    base.InvalidBounds = true;
                    base.ResetPath();
                    this.Changed(0x5b7, 0, null, DiagramShape.MakeRect(tf1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("Whether the prominent pair of verticies point vertically or horizontally"), Category("Appearance"), DefaultValue(0)]
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
                    RectangleF ef1 = this.Bounds;
                    this.A = new PointF(ef1.X, ef1.Y);
                    this.B = new PointF(ef1.X + ef1.Width, ef1.Y);
                    this.C = new PointF(ef1.X + ef1.Width, ef1.Y + ef1.Height);
                    this.D = new PointF(ef1.X, ef1.Y + ef1.Height);
                    this.Changed(0x5b9, 0, orientation1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    base.ResetPath();
                }
            }
        }


        public const int ChangedMultiplePoints = 0x5b8;
        public const int ChangedOrientation = 0x5b9;
        public const int ChangedPointA = 1460;
        public const int ChangedPointB = 0x5b5;
        public const int ChangedPointC = 0x5b6;
        public const int ChangedPointD = 0x5b7;
        private System.Windows.Forms.Orientation myOrientation;
        private PointF[] myPoints;
        public const int PointAHandleID = 0x40a;
        public const int PointBHandleID = 0x40b;
        public const int PointCHandleID = 0x40c;
        public const int PointDHandleID = 0x40d;
    }
}
