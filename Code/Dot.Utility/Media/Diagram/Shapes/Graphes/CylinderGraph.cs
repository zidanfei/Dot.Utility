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
    public class CylinderGraph : DiagramGraph
    {
        public CylinderGraph()
        {
            this.myPoints = new PointF[4];
            this.myMinorRadius = 10f;
            this.myOrientation = System.Windows.Forms.Orientation.Vertical;
            this.myPerspective = Perspective.TopLeft;
            base.InternalFlags |= 0x100200;
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            base.AddSelectionHandles(sel, selectedObj);
            if (this.CanReshape() && this.ResizableRadius)
            {
                RectangleF ef1 = this.Bounds;
                PointF tf1 = new PointF();
                float single1 = this.MinorRadius;
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    if ((this.Perspective == Perspective.TopLeft) || (this.Perspective == Perspective.TopRight))
                    {
                        tf1 = new PointF(ef1.X + (ef1.Width / 2f), ef1.Y + (2f * single1));
                    }
                    else
                    {
                        tf1 = new PointF(ef1.X + (ef1.Width / 2f), (ef1.Y + ef1.Height) - (2f * single1));
                    }
                }
                else if ((this.Perspective == Perspective.TopLeft) || (this.Perspective == Perspective.BottomLeft))
                {
                    tf1 = new PointF(ef1.X + (2f * single1), ef1.Y + (ef1.Height / 2f));
                }
                else
                {
                    tf1 = new PointF((ef1.X + ef1.Width) - (2f * single1), ef1.Y + (ef1.Height / 2f));
                }
                IShapeHandle handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, 0x408, true);
                base.MakeDiamondResizeHandle(handle1, (this.Orientation == System.Windows.Forms.Orientation.Horizontal) ? 0x40 : 0x80);
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x5c9:
                    {
                        this.MinorRadius = e.GetFloat(undo);
                        return;
                    }
                case 0x5ca:
                    {
                        this.Orientation = (System.Windows.Forms.Orientation)e.GetValue(undo);
                        return;
                    }
                case 0x5cb:
                    {
                        this.Perspective = (Perspective)e.GetValue(undo);
                        return;
                    }
                case 0x5cc:
                    {
                        this.ResizableRadius = (bool)e.GetValue(undo);
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
            GraphicsPath path1 = this.GetPath(0f, 0f);
            bool flag1 = path1.IsVisible(p);
            base.DisposePath(path1);
            return flag1;
        }

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            if ((whichHandle == 0x408) && ((this.ResizesRealtime || (evttype == InputState.Finish)) || (evttype == InputState.Cancel)))
            {
                RectangleF ef1 = this.Bounds;
                float single1 = this.MinorRadius;
                if (this.myOrientation == System.Windows.Forms.Orientation.Vertical)
                {
                    if ((this.Perspective == Perspective.TopLeft) || (this.Perspective == Perspective.TopRight))
                    {
                        if (newPoint.Y > (ef1.Y + ef1.Height))
                        {
                            single1 = ef1.Height / 2f;
                        }
                        else if (newPoint.Y < ef1.Y)
                        {
                            single1 = 0f;
                        }
                        else
                        {
                            single1 = (newPoint.Y - ef1.Y) / 2f;
                        }
                    }
                    else if (newPoint.Y > (ef1.Y + ef1.Height))
                    {
                        single1 = 0f;
                    }
                    else if (newPoint.Y < ef1.Y)
                    {
                        single1 = ef1.Height / 2f;
                    }
                    else
                    {
                        single1 = ((ef1.Y + ef1.Height) - newPoint.Y) / 2f;
                    }
                }
                else if ((this.Perspective == Perspective.TopLeft) || (this.Perspective == Perspective.BottomLeft))
                {
                    if (newPoint.X > (ef1.X + ef1.Width))
                    {
                        single1 = ef1.Width / 2f;
                    }
                    else if (newPoint.X < ef1.X)
                    {
                        single1 = 0f;
                    }
                    else
                    {
                        single1 = (newPoint.X - ef1.X) / 2f;
                    }
                }
                else if (newPoint.X > (ef1.X + ef1.Width))
                {
                    single1 = 0f;
                }
                else if (newPoint.X < ef1.X)
                {
                    single1 = ef1.Width / 2f;
                }
                else
                {
                    single1 = ((ef1.X + ef1.Width) - newPoint.X) / 2f;
                }
                this.MinorRadius = single1;
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
            RectangleF ef2;
            RectangleF ef3;
            float single3;
            float single4;
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            PointF[] tfArray1 = this.getPoints(0f, 0f);
            PointF tf1 = DiagramGraph.ExpandPointOnEdge(tfArray1[0], ef1, single1);
            PointF tf2 = DiagramGraph.ExpandPointOnEdge(tfArray1[1], ef1, single1);
            PointF tf3 = DiagramGraph.ExpandPointOnEdge(tfArray1[2], ef1, single1);
            PointF tf4 = DiagramGraph.ExpandPointOnEdge(tfArray1[3], ef1, single1);
            float single2 = 1E+21f;
            PointF tf6 = new PointF();
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                ef2 = new RectangleF(ef1.X, ef1.Y, ef1.Width, this.MinorRadius * 2f);
                ef3 = new RectangleF(ef1.X, (ef1.Y + ef1.Height) - (this.MinorRadius * 2f), ef1.Width, this.MinorRadius * 2f);
                single3 = 180f;
                single4 = 0f;
            }
            else
            {
                ef2 = new RectangleF(ef1.X, ef1.Y, this.MinorRadius * 2f, ef1.Height);
                ef3 = new RectangleF((ef1.X + ef1.Width) - (this.MinorRadius * 2f), ef1.Y, this.MinorRadius * 2f, ef1.Height);
                single3 = 90f;
                single4 = 270f;
            }
            if (EllipseGraph.NearestIntersectionOnArc(ef2, p1, p2, out tf5, single3, 180f))
            {
                float single5 = ((tf5.X - p1.X) * (tf5.X - p1.X)) + ((tf5.Y - p1.Y) * (tf5.Y - p1.Y));
                if (single5 < single2)
                {
                    single2 = single5;
                    tf6 = tf5;
                }
            }
            if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                if (EllipseGraph.NearestIntersectionOnArc(ef3, p1, p2, out tf5, 270f, 90f))
                {
                    float single6 = ((tf5.X - p1.X) * (tf5.X - p1.X)) + ((tf5.Y - p1.Y) * (tf5.Y - p1.Y));
                    if (single6 < single2)
                    {
                        single2 = single6;
                        tf6 = tf5;
                    }
                }
                if (EllipseGraph.NearestIntersectionOnArc(ef3, p1, p2, out tf5, 0f, 90f))
                {
                    float single7 = ((tf5.X - p1.X) * (tf5.X - p1.X)) + ((tf5.Y - p1.Y) * (tf5.Y - p1.Y));
                    if (single7 < single2)
                    {
                        single2 = single7;
                        tf6 = tf5;
                    }
                }
            }
            else if (EllipseGraph.NearestIntersectionOnArc(ef3, p1, p2, out tf5, single4, 180f))
            {
                float single8 = ((tf5.X - p1.X) * (tf5.X - p1.X)) + ((tf5.Y - p1.Y) * (tf5.Y - p1.Y));
                if (single8 < single2)
                {
                    single2 = single8;
                    tf6 = tf5;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf1, tf2, p1, p2, out tf5))
            {
                float single9 = ((tf5.X - p1.X) * (tf5.X - p1.X)) + ((tf5.Y - p1.Y) * (tf5.Y - p1.Y));
                if (single9 < single2)
                {
                    single2 = single9;
                    tf6 = tf5;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf3, tf4, p1, p2, out tf5))
            {
                float single10 = ((tf5.X - p1.X) * (tf5.X - p1.X)) + ((tf5.Y - p1.Y) * (tf5.Y - p1.Y));
                if (single10 < single2)
                {
                    single2 = single10;
                    tf6 = tf5;
                }
            }
            result = tf6;
            return (single2 < 1E+21f);
        }

        private GraphicsPath GetPath(float offx, float offy)
        {
            GraphicsPath path1;
            if ((offx != 0f) || (offy != 0f))
            {
                path1 = new GraphicsPath(FillMode.Winding);
            }
            else
            {
                if (this.myPath != null)
                {
                    return this.myPath;
                }
                path1 = new GraphicsPath(FillMode.Winding);
                this.myPath = path1;
            }
            RectangleF ef1 = this.Bounds;
            PointF[] tfArray1 = this.getPoints(offx, offy);
            float single1 = this.MinorRadius;
            if (single1 == 0f)
            {
                RectangleF ef2 = this.Bounds;
                ef2.X += offx;
                ef2.Y += offy;
                path1.AddRectangle(ef2);
                path1.CloseAllFigures();
                return path1;
            }
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                if (single1 > (ef1.Height / 2f))
                {
                    single1 = ef1.Height / 2f;
                }
                if ((this.Perspective == Perspective.TopLeft) || (this.Perspective == Perspective.TopRight))
                {
                    path1.AddEllipse(tfArray1[0].X, (float)(tfArray1[0].Y - single1), ef1.Width, (float)(single1 * 2f));
                    path1.AddLine(tfArray1[3], tfArray1[2]);
                    path1.AddArc(tfArray1[1].X, (float)(tfArray1[1].Y - single1), ef1.Width, (float)(single1 * 2f), (float)0f, (float)180f);
                    path1.AddLine(tfArray1[1], tfArray1[0]);
                    return path1;
                }
                path1.AddArc(tfArray1[0].X, (float)(tfArray1[0].Y - single1), ef1.Width, (float)(single1 * 2f), (float)180f, (float)180f);
                path1.AddLine(tfArray1[3], tfArray1[2]);
                path1.AddEllipse(tfArray1[1].X, (float)(tfArray1[1].Y - single1), ef1.Width, (float)(single1 * 2f));
                path1.AddArc(tfArray1[1].X, (float)(tfArray1[1].Y - single1), ef1.Width, (float)(single1 * 2f), (float)0f, (float)180f);
                path1.AddLine(tfArray1[1], tfArray1[0]);
                return path1;
            }
            if (single1 > (ef1.Width / 2f))
            {
                single1 = ef1.Width / 2f;
            }
            if ((this.Perspective == Perspective.TopLeft) || (this.Perspective == Perspective.BottomLeft))
            {
                path1.AddEllipse((float)(tfArray1[0].X - single1), tfArray1[0].Y, (float)(single1 * 2f), ef1.Height);
                path1.AddLine(tfArray1[0], tfArray1[1]);
                path1.AddArc((float)(tfArray1[1].X - single1), tfArray1[1].Y, (float)(single1 * 2f), ef1.Height, (float)270f, (float)180f);
                path1.AddLine(tfArray1[2], tfArray1[3]);
                return path1;
            }
            path1.AddArc((float)(tfArray1[0].X - single1), tfArray1[0].Y, (float)(single1 * 2f), ef1.Height, (float)90f, (float)180f);
            path1.AddLine(tfArray1[0], tfArray1[1]);
            path1.AddEllipse((float)(tfArray1[1].X - single1), tfArray1[1].Y, (float)(single1 * 2f), ef1.Height);
            path1.AddArc((float)(tfArray1[1].X - single1), tfArray1[1].Y, (float)(single1 * 2f), ef1.Height, (float)270f, (float)180f);
            path1.AddLine(tfArray1[2], tfArray1[3]);
            return path1;
        }

        private PointF[] getPoints(float offx, float offy)
        {
            RectangleF ef1 = this.Bounds;
            float single1 = this.MinorRadius;
            ef1.X += offx;
            ef1.Y += offy;
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                if (single1 > (ef1.Height / 2f))
                {
                    single1 = ef1.Height / 2f;
                }
                this.myPoints[0] = new PointF(ef1.X, ef1.Y + single1);
                this.myPoints[1] = new PointF(ef1.X, (ef1.Y + ef1.Height) - single1);
                this.myPoints[2] = new PointF(ef1.X + ef1.Width, (ef1.Y + ef1.Height) - single1);
                this.myPoints[3] = new PointF(ef1.X + ef1.Width, ef1.Y + single1);
            }
            else
            {
                if (single1 > (ef1.Width / 2f))
                {
                    single1 = ef1.Width / 2f;
                }
                this.myPoints[0] = new PointF(ef1.X + single1, ef1.Y);
                this.myPoints[1] = new PointF((ef1.X + ef1.Width) - single1, ef1.Y);
                this.myPoints[2] = new PointF((ef1.X + ef1.Width) - single1, ef1.Y + ef1.Height);
                this.myPoints[3] = new PointF(ef1.X + single1, ef1.Y + ef1.Height);
            }
            return this.myPoints;
        }

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = this.GetPath(0f, 0f);
            return (GraphicsPath)path1.Clone();
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            if (this.Shadowed)
            {
                SizeF ef1 = this.GetShadowOffset(view);
                GraphicsPath path1 = this.GetPath(ef1.Width, ef1.Height);
                if (this.Brush != null)
                {
                    Brush brush1 = this.GetShadowBrush(view);
                    DiagramGraph.DrawPath(g, view, null, brush1, path1);
                }
                else if (this.Pen != null)
                {
                    Pen pen1 = this.GetShadowPen(view, base.InternalPenWidth);
                    DiagramGraph.DrawPath(g, view, pen1, null, path1);
                }
                base.DisposePath(path1);
            }
            GraphicsPath path2 = this.GetPath(0f, 0f);
            DiagramGraph.DrawPath(g, view, this.Pen, this.Brush, path2);
            base.DisposePath(path2);
        }


        [Category("Appearance"), DefaultValue((float)10f), Description("The length of cylinder's ellipse's minor radius.")]
        public virtual float MinorRadius
        {
            get
            {
                return this.myMinorRadius;
            }
            set
            {
                float single1 = this.myMinorRadius;
                if (value < 0f)
                {
                    value = 0f;
                }
                if (single1 != value)
                {
                    this.myMinorRadius = value;
                    base.ResetPath();
                    this.Changed(0x5c9, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("Whether the parallel lines of the cylinder are drawn horizontally or vertically."), DefaultValue(1), Category("Appearance")]
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
                    base.ResetPath();
                    this.Changed(0x5ca, 0, orientation1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether the cylinder's full ellipse is drawn on the top or bottom of the cylinder."), DefaultValue(0), Category("Appearance")]
        public virtual Perspective Perspective
        {
            get
            {
                return this.myPerspective;
            }
            set
            {
                Perspective perspective1 = this.myPerspective;
                if (perspective1 != value)
                {
                    this.myPerspective = value;
                    base.ResetPath();
                    this.Changed(0x5cb, 0, perspective1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(true), Description("Whether to add the radius control handle."), Category("Behavior")]
        public virtual bool ResizableRadius
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
                    this.Changed(0x5cc, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedMinorRadius = 0x5c9;
        public const int ChangedOrientation = 0x5ca;
        public const int ChangedPerspective = 0x5cb;
        public const int ChangedResizableRadius = 0x5cc;
        private const int flagResizableRadius = 0x100000;
        private float myMinorRadius;
        private System.Windows.Forms.Orientation myOrientation;
        private Perspective myPerspective;
        private PointF[] myPoints;
        public const int RadiusHandleID = 0x408;
    }
}
