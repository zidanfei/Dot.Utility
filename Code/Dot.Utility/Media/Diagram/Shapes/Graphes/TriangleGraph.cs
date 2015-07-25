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
    public class TriangleGraph : DiagramGraph
    {
        public TriangleGraph()
        {
            this.myPoints = new PointF[3];
            base.InternalFlags |= 0x200;
            this.myPoints[1] = new PointF(10f, 0f);
            this.myPoints[2] = new PointF(5f, 10f);
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            if (!this.CanResize() || !this.CanReshape())
            {
                base.AddSelectionHandles(sel, selectedObj);
            }
            else
            {
                sel.RemoveHandles(this);
                sel.CreateResizeHandle(this, selectedObj, this.A, 0x2000, true);
                sel.CreateResizeHandle(this, selectedObj, this.B, 0x2001, true);
                sel.CreateResizeHandle(this, selectedObj, this.C, 0x2002, true);
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x597:
                    {
                        this.A = e.GetPoint(undo);
                        return;
                    }
                case 0x598:
                    {
                        this.B = e.GetPoint(undo);
                        return;
                    }
                case 0x599:
                    {
                        this.C = e.GetPoint(undo);
                        return;
                    }
                case 0x59a:
                    {
                        PointF[] tfArray1 = (PointF[])e.GetValue(undo);
                        this.myPoints = tfArray1;
                        base.ResetPath();
                        base.InvalidBounds = true;
                        this.Changed(0x59a, 0, null, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
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
            return new RectangleF(single1, single3, single2 - single1, single4 - single3);
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
            if (e.SubHint == 0x59a)
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
            TriangleGraph triangle1 = (TriangleGraph)base.CopyObject(env);
            if (triangle1 != null)
            {
                triangle1.myPoints = (PointF[])this.myPoints.Clone();
            }
            return triangle1;
        }

        public override void CopyOldValueForUndo(ChangedEventArgs e)
        {
            if (e.SubHint == 0x59a)
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
            if ((whichHandle >= 0x2000) && ((this.ResizesRealtime || (evttype == InputState.Finish)) || (evttype == InputState.Cancel)))
            {
                switch (whichHandle)
                {
                    case 0x2000:
                        {
                            this.A = newPoint;
                            return;
                        }
                    case 0x2001:
                        {
                            this.B = newPoint;
                            return;
                        }
                    case 0x2002:
                        {
                            this.C = newPoint;
                            return;
                        }
                }
            }
            else
            {
                base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
            }
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            PointF tf4;
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            PointF tf1 = DiagramGraph.ExpandPointOnEdge(this.A, ef1, single1);
            PointF tf2 = DiagramGraph.ExpandPointOnEdge(this.B, ef1, single1);
            PointF tf3 = DiagramGraph.ExpandPointOnEdge(this.C, ef1, single1);
            float single2 = p1.X;
            float single3 = p1.Y;
            float single4 = 1E+21f;
            PointF tf5 = new PointF();
            if (StrokeGraph.NearestIntersectionOnLine(tf1, tf2, p1, p2, out tf4))
            {
                float single5 = ((tf4.X - single2) * (tf4.X - single2)) + ((tf4.Y - single3) * (tf4.Y - single3));
                if (single5 < single4)
                {
                    single4 = single5;
                    tf5 = tf4;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf2, tf3, p1, p2, out tf4))
            {
                float single6 = ((tf4.X - single2) * (tf4.X - single2)) + ((tf4.Y - single3) * (tf4.Y - single3));
                if (single6 < single4)
                {
                    single4 = single6;
                    tf5 = tf4;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf3, tf1, p1, p2, out tf4))
            {
                float single7 = ((tf4.X - single2) * (tf4.X - single2)) + ((tf4.Y - single3) * (tf4.Y - single3));
                if (single7 < single4)
                {
                    single4 = single7;
                    tf5 = tf4;
                }
            }
            result = tf5;
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
                    bool flag1 = base.SuspendsUpdates;
                    if (!flag1)
                    {
                        this.Changing(0x59a);
                    }
                    base.SuspendsUpdates = true;
                    PointF tf1 = this.A;
                    tf1.X += single1;
                    tf1.Y += single2;
                    this.A = tf1;
                    PointF tf2 = this.B;
                    tf2.X += single1;
                    tf2.Y += single2;
                    this.B = tf2;
                    PointF tf3 = this.C;
                    tf3.X += single1;
                    tf3.Y += single2;
                    this.C = tf3;
                    base.InvalidBounds = false;
                    base.SuspendsUpdates = flag1;
                    if (flag1)
                    {
                        return;
                    }
                    this.Changed(0x59a, 0, null, old, 0, null, ef2);
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
                bool flag2 = base.SuspendsUpdates;
                if (!flag2)
                {
                    this.Changing(0x59a);
                }
                base.SuspendsUpdates = true;
                PointF tf4 = this.A;
                tf4.X = ef3.X + ((tf4.X - old.X) * single3);
                tf4.Y = ef3.Y + ((tf4.Y - old.Y) * single4);
                this.A = tf4;
                PointF tf5 = this.B;
                tf5.X = ef3.X + ((tf5.X - old.X) * single3);
                tf5.Y = ef3.Y + ((tf5.Y - old.Y) * single4);
                this.B = tf5;
                PointF tf6 = this.C;
                tf6.X = ef3.X + ((tf6.X - old.X) * single3);
                tf6.Y = ef3.Y + ((tf6.Y - old.Y) * single4);
                this.C = tf6;
                base.InvalidBounds = false;
                base.SuspendsUpdates = flag2;
                if (!flag2)
                {
                    this.Changed(0x59a, 0, null, old, 0, null, ef3);
                }
            }
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            if (this.Shadowed)
            {
                SizeF ef1 = this.GetShadowOffset(view);
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


        [Description("The first of three points in this triangle."), Category("Bounds"), TypeConverter(typeof(PointFConverter))]
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
                    base.ResetPath();
                    this.myPoints[0] = value;
                    base.InvalidBounds = true;
                    this.Changed(0x597, 0, null, DiagramShape.MakeRect(tf1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Category("Bounds"), TypeConverter(typeof(PointFConverter)), Description("The second of three points in this triangle.")]
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
                    base.ResetPath();
                    this.myPoints[1] = value;
                    base.InvalidBounds = true;
                    this.Changed(0x598, 0, null, DiagramShape.MakeRect(tf1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("The third of three points in this triangle."), TypeConverter(typeof(PointFConverter)), Category("Bounds")]
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
                    base.ResetPath();
                    this.myPoints[2] = value;
                    base.InvalidBounds = true;
                    this.Changed(0x599, 0, null, DiagramShape.MakeRect(tf1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }


        public const int ChangedAllPoints = 0x59a;
        public const int ChangedPointA = 0x597;
        public const int ChangedPointB = 0x598;
        public const int ChangedPointC = 0x599;
        private PointF[] myPoints;
    }
}
