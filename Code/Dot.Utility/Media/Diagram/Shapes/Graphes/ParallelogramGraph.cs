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
    public class ParallelogramGraph : DiagramGraph
    {
        public ParallelogramGraph()
        {
            this.mySkew = new SizeF(10f, 0f);
            this.myPoints = new PointF[4];
            base.InternalFlags |= 3146240;
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            base.AddSelectionHandles(sel, selectedObj);
            if (this.CanReshape() && this.ReshapableSkew)
            {
                RectangleF ef1 = this.Bounds;
                SizeF ef2 = this.Skew;
                PointF tf1 = new PointF();
                if (this.Direction)
                {
                    tf1 = new PointF(ef1.X + ef2.Width, ef1.Y + ef2.Height);
                }
                else
                {
                    tf1 = new PointF((ef1.X + ef1.Width) - ef2.Width, ef1.Y + ef2.Height);
                }
                IShapeHandle handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, 0x40e, true);
                base.MakeDiamondResizeHandle(handle1, 1);
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x5ba:
                    {
                        this.Skew = e.GetSize(undo);
                        return;
                    }
                case 0x5bb:
                    {
                        this.ReshapableSkew = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5bc:
                    {
                        this.Direction = (bool)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
            base.ResetPath();
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
            ParallelogramGraph parallelogram1 = (ParallelogramGraph)base.CopyObject(env);
            if (parallelogram1 != null)
            {
                parallelogram1.myPoints = (PointF[])this.myPoints.Clone();
            }
            return parallelogram1;
        }

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            if ((whichHandle == 0x40e) && ((this.ResizesRealtime || (evttype == InputState.Finish)) || (evttype == InputState.Cancel)))
            {
                RectangleF ef1 = this.Bounds;
                float single1 = ef1.Height / ef1.Width;
                float single2 = ef1.Left;
                float single3 = ef1.Top;
                SizeF ef2 = this.Skew;
                if (this.Direction)
                {
                    if (newPoint.X < ef1.X)
                    {
                        ef2.Width = 0f;
                    }
                    else if (newPoint.X >= (ef1.X + ef1.Width))
                    {
                        ef2.Width = ef1.Width;
                    }
                    else
                    {
                        ef2.Width = newPoint.X - ef1.X;
                    }
                }
                else if (newPoint.X >= (ef1.X + ef1.Width))
                {
                    ef2.Width = 0f;
                }
                else if (newPoint.X < ef1.X)
                {
                    ef2.Width = ef1.Width;
                }
                else
                {
                    ef2.Width = (ef1.X + ef1.Width) - newPoint.X;
                }
                if (newPoint.Y < ef1.Y)
                {
                    ef2.Height = 0f;
                }
                else if (newPoint.Y >= (ef1.Y + ef1.Height))
                {
                    ef2.Height = ef1.Height;
                }
                else
                {
                    ef2.Height = newPoint.Y - ef1.Y;
                }
                this.Skew = ef2;
                base.ResetPath();
            }
            else
            {
                base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
                base.ResetPath();
            }
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
            float single1 = ef1.X;
            float single2 = ef1.Y;
            float single3 = ef1.X + ef1.Width;
            float single4 = ef1.Y + ef1.Height;
            SizeF ef2 = this.Skew;
            bool flag1 = this.Direction;
            float single5 = System.Math.Min(ef2.Width, ef1.Width);
            float single6 = System.Math.Min(ef2.Height, ef1.Height);
            this.myPoints[0].X = single1 + (flag1 ? single5 : 0f);
            this.myPoints[0].Y = single2 + (flag1 ? single6 : 0f);
            this.myPoints[1].X = single3 - (flag1 ? 0f : single5);
            this.myPoints[1].Y = single2 + (flag1 ? 0f : single6);
            this.myPoints[2].X = single3 - (flag1 ? single5 : 0f);
            this.myPoints[2].Y = single4 - (flag1 ? single6 : 0f);
            this.myPoints[3].X = single1 + (flag1 ? 0f : single5);
            this.myPoints[3].Y = single4 - (flag1 ? 0f : single6);
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
            PointF[] tfArray1 = this.getPoints();
            if (this.Shadowed)
            {
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
                for (int num3 = 0; num3 < num1; num3++)
                {
                    tfArray1[num3].X -= ef1.Width;
                    tfArray1[num3].Y -= ef1.Height;
                }
            }
            DiagramGraph.DrawPolygon(g, view, this.Pen, this.Brush, tfArray1);
        }


        [DefaultValue(true), Description("Determines the direction of the fixed diagonal."), Category("Appearance")]
        public virtual bool Direction
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
                    this.Changed(0x5bc, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether users can reshape the skew of this resizable object."), Category("Behavior"), DefaultValue(true)]
        public virtual bool ReshapableSkew
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
                    this.Changed(0x5bb, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), TypeConverter(typeof(SizeFConverter)), Description("The tiltedness of the parallelogram")]
        public SizeF Skew
        {
            get
            {
                return this.mySkew;
            }
            set
            {
                SizeF ef1 = this.mySkew;
                if (value.Width < 0f)
                {
                    value.Width = 0f;
                }
                if (value.Height < 0f)
                {
                    value.Height = 0f;
                }
                if (ef1 != value)
                {
                    this.mySkew = value;
                    base.ResetPath();
                    this.Changed(0x5ba, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }


        public const int ChangedDirection = 0x5bc;
        public const int ChangedReshapableSkew = 0x5bb;
        public const int ChangedSkew = 0x5ba;
        private const int flagDirection = 0x200000;
        private const int flagReshapableSkew = 0x100000;
        private PointF[] myPoints;
        private SizeF mySkew;
        public const int SkewHandleID = 0x40e;
    }
}
