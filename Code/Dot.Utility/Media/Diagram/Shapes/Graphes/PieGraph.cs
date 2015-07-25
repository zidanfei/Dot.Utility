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
    public class PieGraph : DiagramGraph
    {
        public PieGraph()
        {
            this.myStartAngle = 0f;
            this.mySweepAngle = 60f;
            base.InternalFlags |= 3146240;
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            base.AddSelectionHandles(sel, selectedObj);
            if (this.CanReshape())
            {
                if (this.ResizableStartAngle)
                {
                    RectangleF ef1 = this.Bounds;
                    PointF tf1 = this.GetPointAtAngle(this.StartAngle);
                    IShapeHandle handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, 0x40f, true);
                    base.MakeDiamondResizeHandle(handle1, 1);
                }
                if (this.ResizableEndAngle)
                {
                    RectangleF ef2 = this.Bounds;
                    PointF tf2 = this.GetPointAtAngle(this.StartAngle + this.SweepAngle);
                    IShapeHandle handle2 = sel.CreateResizeHandle(this, selectedObj, tf2, 1040, true);
                    base.MakeDiamondResizeHandle(handle2, 1);
                }
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x5ab:
                    {
                        this.StartAngle = e.GetFloat(undo);
                        return;
                    }
                case 0x5ac:
                    {
                        this.SweepAngle = e.GetFloat(undo);
                        return;
                    }
                case 0x5ad:
                    {
                        this.ResizableStartAngle = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5ae:
                    {
                        this.ResizableEndAngle = (bool)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        public override bool ContainsPoint(PointF p)
        {
            if (base.ContainsPoint(p))
            {
                float single9;
                float single10;
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
                if ((((single6 * single6) / (single2 * single2)) + ((single7 * single7) / (single3 * single3))) > 1f)
                {
                    return false;
                }
                float single8 = StrokeGraph.GetAngle(p.X - single4, p.Y - single5);
                if (this.SweepAngle < 0f)
                {
                    single9 = this.StartAngle + this.SweepAngle;
                    single10 = -this.SweepAngle;
                }
                else
                {
                    single9 = this.StartAngle;
                    single10 = this.SweepAngle;
                }
                if (single10 > 360f)
                {
                    return true;
                }
                if ((single9 + single10) > 360f)
                {
                    if (single8 < single9)
                    {
                        return (single8 <= ((single9 + single10) - 360f));
                    }
                    return true;
                }
                if (single8 >= single9)
                {
                    return (single8 <= (single9 + single10));
                }
            }
            return false;
        }

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            if (((whichHandle == 0x40f) || (whichHandle == 1040)) && ((this.ResizesRealtime || (evttype == InputState.Finish)) || (evttype == InputState.Cancel)))
            {
                if (whichHandle == 0x40f)
                {
                    RectangleF ef1 = this.Bounds;
                    float single1 = ef1.Width / 2f;
                    float single2 = ef1.Height / 2f;
                    float single3 = ef1.X + single1;
                    float single4 = ef1.Y + single2;
                    float single5 = StrokeGraph.GetAngle(newPoint.X - single3, newPoint.Y - single4);
                    float single6 = this.SweepAngle - (single5 - this.StartAngle);
                    if (this.SweepAngle >= 0f)
                    {
                        if (single6 < 0f)
                        {
                            single6 += 360f;
                        }
                    }
                    else if (single6 >= 0f)
                    {
                        single6 -= 360f;
                    }
                    this.SweepAngle = single6;
                    this.StartAngle = single5;
                }
                else if (whichHandle == 1040)
                {
                    RectangleF ef2 = this.Bounds;
                    float single7 = ef2.Width / 2f;
                    float single8 = ef2.Height / 2f;
                    float single9 = ef2.X + single7;
                    float single10 = ef2.Y + single8;
                    float single11 = StrokeGraph.GetAngle(newPoint.X - single9, newPoint.Y - single10);
                    float single12 = single11 - this.StartAngle;
                    if (this.SweepAngle >= 0f)
                    {
                        if (single12 < 0f)
                        {
                            single12 += 360f;
                        }
                    }
                    else if (single12 >= 0f)
                    {
                        single12 -= 360f;
                    }
                    this.SweepAngle = single12;
                }
            }
            else
            {
                base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
            }
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            PointF tf1;
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            DiagramShape.InflateRect(ref ef1, single1, single1);
            float single2 = ef1.Width / 2f;
            float single3 = ef1.Height / 2f;
            float single4 = ef1.X + single2;
            float single5 = ef1.Y + single3;
            float single6 = p1.X - single4;
            float single7 = p1.Y - single5;
            float single8 = this.StartAngle;
            float single9 = this.SweepAngle;
            float single10 = single8 + single9;
            if (single10 > 360f)
            {
                single10 -= 360f;
            }
            bool flag1 = false;
            float single11 = 1E+21f;
            result = new PointF();
            if ((-0.01f < single6) && (single6 < 0.01f))
            {
                single6 = 0.01f;
            }
            if ((-0.01f < single7) && (single7 < 0.01f))
            {
                single7 = 0.01f;
            }
            if (single9 >= 360f)
            {
                if (EllipseGraph.NearestIntersectionOnEllipse(ef1, p1, p2, out tf1))
                {
                    float single12 = ((p1.X - tf1.X) * (p1.X - tf1.X)) + ((p1.Y - tf1.Y) * (p1.Y - tf1.Y));
                    if (single12 < single11)
                    {
                        flag1 = true;
                        result = tf1;
                        single11 = single12;
                    }
                }
            }
            else if ((single9 + single8) > 360f)
            {
                if (EllipseGraph.NearestIntersectionOnArc(ef1, p1, p2, out tf1, single8, 360f - single8))
                {
                    float single13 = ((p1.X - tf1.X) * (p1.X - tf1.X)) + ((p1.Y - tf1.Y) * (p1.Y - tf1.Y));
                    if (single13 < single11)
                    {
                        flag1 = true;
                        result = tf1;
                        single11 = single13;
                    }
                }
                if (EllipseGraph.NearestIntersectionOnArc(ef1, p1, p2, out tf1, 0f, single9 - (360f - single8)))
                {
                    float single14 = ((p1.X - tf1.X) * (p1.X - tf1.X)) + ((p1.Y - tf1.Y) * (p1.Y - tf1.Y));
                    if (single14 < single11)
                    {
                        flag1 = true;
                        result = tf1;
                        single11 = single14;
                    }
                }
            }
            else if (EllipseGraph.NearestIntersectionOnArc(ef1, p1, p2, out tf1, single8, single9))
            {
                float single15 = ((p1.X - tf1.X) * (p1.X - tf1.X)) + ((p1.Y - tf1.Y) * (p1.Y - tf1.Y));
                if (single15 < single11)
                {
                    flag1 = true;
                    result = tf1;
                    single11 = single15;
                }
            }
            PointF tf2 = this.GetPointAtAngle(single8);
            if (StrokeGraph.NearestIntersectionOnLine(new PointF(single4, single5), tf2, p1, p2, out tf1))
            {
                float single16 = ((p1.X - tf1.X) * (p1.X - tf1.X)) + ((p1.Y - tf1.Y) * (p1.Y - tf1.Y));
                if (single16 < single11)
                {
                    flag1 = true;
                    result = tf1;
                    single11 = single16;
                }
            }
            PointF tf3 = this.GetPointAtAngle(single10);
            if (StrokeGraph.NearestIntersectionOnLine(new PointF(single4, single5), tf3, p1, p2, out tf1))
            {
                float single17 = ((p1.X - tf1.X) * (p1.X - tf1.X)) + ((p1.Y - tf1.Y) * (p1.Y - tf1.Y));
                if (single17 < single11)
                {
                    flag1 = true;
                    result = tf1;
                    single11 = single17;
                }
            }
            return flag1;
        }

        internal PointF GetPointAtAngle(float ang)
        {
            RectangleF ef1 = this.Bounds;
            float single1 = ef1.Width / 2f;
            float single2 = ef1.Height / 2f;
            float single3 = ef1.X + single1;
            float single4 = ef1.Y + single2;
            if (single1 == 0f)
            {
                return new PointF(single3, single4);
            }
            float single5 = (float)System.Math.Cos((ang / 180f) * 3.1415926535897931);
            float single6 = 1f - ((single2 * single2) / (single1 * single1));
            float single7 = (float)(single1 * System.Math.Sqrt((double)((1f - single6) / (1f - ((single6 * single5) * single5)))));
            float single8 = single7 * single5;
            return new PointF(single3 + single8, single4 + ((float)(System.Math.Tan((ang / 180f) * 3.1415926535897931) * single8)));
        }

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
            RectangleF ef1 = this.Bounds;
            if ((ef1.Width > 0f) && (ef1.Height > 0f))
            {
                path1.AddPie(ef1.X, ef1.Y, ef1.Width, ef1.Height, this.StartAngle, this.SweepAngle);
            }
            return path1;
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            float single1 = this.StartAngle;
            float single2 = this.SweepAngle;
            RectangleF ef1 = this.Bounds;
            if (this.Shadowed)
            {
                SizeF ef2 = this.GetShadowOffset(view);
                if (this.Brush != null)
                {
                    Brush brush1 = this.GetShadowBrush(view);
                    DiagramGraph.DrawPie(g, view, null, brush1, ef1.X + ef2.Width, ef1.Y + ef2.Height, ef1.Width, ef1.Height, single1, single2);
                }
                else if (this.Pen != null)
                {
                    Pen pen1 = this.GetShadowPen(view, base.InternalPenWidth);
                    DiagramGraph.DrawPie(g, view, pen1, null, ef1.X + ef2.Width, ef1.Y + ef2.Height, ef1.Width, ef1.Height, single1, single2);
                }
            }
            DiagramGraph.DrawPie(g, view, this.Pen, this.Brush, ef1.X, ef1.Y, ef1.Width, ef1.Height, single1, single2);
        }


        [Category("Behavior"), Description("Whether users can resize the end angle of this resizable object."), DefaultValue(true)]
        public virtual bool ResizableEndAngle
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
                    this.Changed(0x5ae, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether users can resize the start angle of this resizable object."), DefaultValue(true), Category("Behavior")]
        public virtual bool ResizableStartAngle
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
                    this.Changed(0x5ad, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The start angle for the side of the pie slice"), DefaultValue(0), Category("Appearance")]
        public float StartAngle
        {
            get
            {
                return this.myStartAngle;
            }
            set
            {
                float single1 = this.myStartAngle;
                if (value < 0f)
                {
                    value = 360f - (-value % 360f);
                }
                else if (value >= 360f)
                {
                    value = value % 360f;
                }
                if (single1 != value)
                {
                    this.myStartAngle = value;
                    base.ResetPath();
                    this.Changed(0x5ab, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Category("Appearance"), Description("The sweep angle for the body of the pie slice"), DefaultValue(60)]
        public float SweepAngle
        {
            get
            {
                return this.mySweepAngle;
            }
            set
            {
                float single1 = this.mySweepAngle;
                if ((value > 360f) || (value < -360f))
                {
                    value = value % 360f;
                }
                if (single1 != value)
                {
                    this.mySweepAngle = value;
                    base.ResetPath();
                    this.Changed(0x5ac, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }


        public const int ChangedResizableEndAngle = 0x5ae;
        public const int ChangedResizableStartAngle = 0x5ad;
        public const int ChangedStartAngle = 0x5ab;
        public const int ChangedSweepAngle = 0x5ac;
        public const int EndAngleHandleID = 1040;
        private const int flagResizableEndAngle = 0x200000;
        private const int flagResizableStartAngle = 0x100000;
        private float myStartAngle;
        private float mySweepAngle;
        public const int StartAngleHandleID = 0x40f;
    }
}
