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
    public class OctagonGraph : DiagramGraph
    {
        public OctagonGraph()
        {
            this.myCorner = new SizeF(10f, 10f);
            this.myPoints = new PointF[8];
            base.InternalFlags |= 0x100200;
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            base.AddSelectionHandles(sel, selectedObj);
            if (this.CanReshape() && this.ReshapableCorner)
            {
                RectangleF ef1 = this.Bounds;
                PointF tf1 = new PointF(ef1.X + this.Corner.Width, ef1.Y);
                IShapeHandle handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, 1030, true);
                base.MakeDiamondResizeHandle(handle1, 0x40);
                tf1 = new PointF(ef1.X, ef1.Y + this.Corner.Height);
                handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, 0x407, true);
                base.MakeDiamondResizeHandle(handle1, 0x80);
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x5bd:
                    {
                        this.Corner = e.GetSize(undo);
                        return;
                    }
                case 1470:
                    {
                        this.ReshapableCorner = (bool)e.GetValue(undo);
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

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            if ((whichHandle < 1030) || ((!this.ResizesRealtime && (evttype != InputState.Finish)) && (evttype != InputState.Cancel)))
            {
                base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
            }
            else
            {
                RectangleF ef1 = this.Bounds;
                SizeF ef2 = this.Corner;
                switch (whichHandle)
                {
                    case 1030:
                        {
                            if (newPoint.X >= ef1.X)
                            {
                                if (newPoint.X >= (ef1.X + (ef1.Width / 2f)))
                                {
                                    ef2.Width = ef1.Width / 2f;
                                    break;
                                }
                                ef2.Width = newPoint.X - ef1.X;
                                break;
                            }
                            ef2.Width = 0f;
                            break;
                        }
                    case 0x407:
                        {
                            if (newPoint.Y >= ef1.Y)
                            {
                                if (newPoint.Y >= (ef1.Y + (ef1.Height / 2f)))
                                {
                                    ef2.Height = ef1.Height / 2f;
                                }
                                else
                                {
                                    ef2.Height = newPoint.Y - ef1.Y;
                                }
                                break;
                            }
                            ef2.Height = 0f;
                            break;
                        }
                }
                this.Corner = ef2;
                base.ResetPath();
            }
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            PointF tf9;
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            PointF[] tfArray1 = this.getPoints();
            PointF tf1 = DiagramGraph.ExpandPointOnEdge(tfArray1[0], ef1, single1);
            PointF tf2 = DiagramGraph.ExpandPointOnEdge(tfArray1[1], ef1, single1);
            PointF tf3 = DiagramGraph.ExpandPointOnEdge(tfArray1[2], ef1, single1);
            PointF tf4 = DiagramGraph.ExpandPointOnEdge(tfArray1[3], ef1, single1);
            PointF tf5 = DiagramGraph.ExpandPointOnEdge(tfArray1[4], ef1, single1);
            PointF tf6 = DiagramGraph.ExpandPointOnEdge(tfArray1[5], ef1, single1);
            PointF tf7 = DiagramGraph.ExpandPointOnEdge(tfArray1[6], ef1, single1);
            PointF tf8 = DiagramGraph.ExpandPointOnEdge(tfArray1[7], ef1, single1);
            float single2 = p1.X;
            float single3 = p1.Y;
            float single4 = 1E+21f;
            PointF tf10 = new PointF();
            if (StrokeGraph.NearestIntersectionOnLine(tf1, tf2, p1, p2, out tf9))
            {
                float single5 = ((tf9.X - single2) * (tf9.X - single2)) + ((tf9.Y - single3) * (tf9.Y - single3));
                if (single5 < single4)
                {
                    single4 = single5;
                    tf10 = tf9;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf2, tf3, p1, p2, out tf9))
            {
                float single6 = ((tf9.X - single2) * (tf9.X - single2)) + ((tf9.Y - single3) * (tf9.Y - single3));
                if (single6 < single4)
                {
                    single4 = single6;
                    tf10 = tf9;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf3, tf4, p1, p2, out tf9))
            {
                float single7 = ((tf9.X - single2) * (tf9.X - single2)) + ((tf9.Y - single3) * (tf9.Y - single3));
                if (single7 < single4)
                {
                    single4 = single7;
                    tf10 = tf9;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf4, tf5, p1, p2, out tf9))
            {
                float single8 = ((tf9.X - single2) * (tf9.X - single2)) + ((tf9.Y - single3) * (tf9.Y - single3));
                if (single8 < single4)
                {
                    single4 = single8;
                    tf10 = tf9;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf5, tf6, p1, p2, out tf9))
            {
                float single9 = ((tf9.X - single2) * (tf9.X - single2)) + ((tf9.Y - single3) * (tf9.Y - single3));
                if (single9 < single4)
                {
                    single4 = single9;
                    tf10 = tf9;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf6, tf7, p1, p2, out tf9))
            {
                float single10 = ((tf9.X - single2) * (tf9.X - single2)) + ((tf9.Y - single3) * (tf9.Y - single3));
                if (single10 < single4)
                {
                    single4 = single10;
                    tf10 = tf9;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf7, tf8, p1, p2, out tf9))
            {
                float single11 = ((tf9.X - single2) * (tf9.X - single2)) + ((tf9.Y - single3) * (tf9.Y - single3));
                if (single11 < single4)
                {
                    single4 = single11;
                    tf10 = tf9;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf8, tf1, p1, p2, out tf9))
            {
                float single12 = ((tf9.X - single2) * (tf9.X - single2)) + ((tf9.Y - single3) * (tf9.Y - single3));
                if (single12 < single4)
                {
                    single4 = single12;
                    tf10 = tf9;
                }
            }
            result = tf10;
            return (single4 < 1E+21f);
        }

        private PointF[] getPoints()
        {
            RectangleF ef1 = this.Bounds;
            SizeF ef2 = this.Corner;
            if (ef2.Width > (ef1.Width / 2f))
            {
                ef2.Width = ef1.Width / 2f;
            }
            if (ef2.Height > (ef1.Height / 2f))
            {
                ef2.Height = ef1.Height / 2f;
            }
            this.myPoints[0] = new PointF(ef1.X + ef2.Width, ef1.Y);
            this.myPoints[1] = new PointF(ef1.X, ef1.Y + ef2.Height);
            this.myPoints[2] = new PointF(ef1.X, (ef1.Y + ef1.Height) - ef2.Height);
            this.myPoints[3] = new PointF(ef1.X + ef2.Width, ef1.Y + ef1.Height);
            this.myPoints[4] = new PointF((ef1.X + ef1.Width) - ef2.Width, ef1.Y + ef1.Height);
            this.myPoints[5] = new PointF(ef1.X + ef1.Width, (ef1.Y + ef1.Height) - ef2.Height);
            this.myPoints[6] = new PointF(ef1.X + ef1.Width, ef1.Y + ef2.Height);
            this.myPoints[7] = new PointF((ef1.X + ef1.Width) - ef2.Width, ef1.Y);
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


        [Description("The maximum width and height of each corner"), TypeConverter(typeof(SizeFConverter)), Category("Appearance")]
        public virtual SizeF Corner
        {
            get
            {
                return this.myCorner;
            }
            set
            {
                SizeF ef1 = this.myCorner;
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
                    this.myCorner = value;
                    base.ResetPath();
                    this.Changed(0x5bd, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Category("Behavior"), Description("Whether users can reshape the corner of this resizable object."), DefaultValue(true)]
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
                    this.Changed(1470, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedCorner = 0x5bd;
        public const int ChangedReshapableCorner = 1470;
        public const int CornerHeightHandleID = 0x407;
        public const int CornerWidthHandleID = 1030;
        private const int flagReshapableCorner = 0x100000;
        private SizeF myCorner;
        private PointF[] myPoints;
    }
}
