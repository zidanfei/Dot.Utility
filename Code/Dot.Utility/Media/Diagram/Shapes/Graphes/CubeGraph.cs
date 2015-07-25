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
    public class CubeGraph : DiagramGraph
    {
        public CubeGraph()
        {
            this.myPoints = new PointF[7];
            this.myDepth = new SizeF(10f, 10f);
            this.myPerspective = Perspective.TopLeft;
            base.InternalFlags |= 0x100200;
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            base.AddSelectionHandles(sel, selectedObj);
            if (this.CanReshape() && this.ReshapableDepth)
            {
                RectangleF ef1 = this.Bounds;
                PointF tf1 = new PointF();
                SizeF ef2 = this.Depth;
                tf1 = this.getPoints(0f, 0f)[1];
                IShapeHandle handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, 0x409, true);
                base.MakeDiamondResizeHandle(handle1, 1);
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x5d3:
                    {
                        this.Depth = e.GetSize(undo);
                        return;
                    }
                case 0x5d4:
                    {
                        this.Perspective = (Perspective)e.GetValue(undo);
                        return;
                    }
                case 0x5d5:
                    {
                        this.ReshapableDepth = (bool)e.GetValue(undo);
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
            GraphicsPath path1 = this.GetPath(0f, 0f);
            bool flag1 = path1.IsVisible(p);
            base.DisposePath(path1);
            return flag1;
        }

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            RectangleF ef1 = this.Bounds;
            SizeF ef2 = this.Depth;
            if ((whichHandle == 0x409) && ((this.ResizesRealtime || (evttype == InputState.Finish)) || (evttype == InputState.Cancel)))
            {
                if (this.Perspective == Perspective.TopRight)
                {
                    if (newPoint.Y > (ef1.Y + ef1.Height))
                    {
                        ef2.Height = ef1.Height;
                    }
                    else if (newPoint.Y < ef1.Y)
                    {
                        ef2.Height = 0f;
                    }
                    else
                    {
                        ef2.Height = newPoint.Y - ef1.Y;
                    }
                    if (newPoint.X > (ef1.X + ef1.Width))
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
                }
                else if (this.Perspective == Perspective.BottomRight)
                {
                    if (newPoint.Y < ef1.Y)
                    {
                        ef2.Height = ef1.Height;
                    }
                    else if (newPoint.Y > (ef1.Y + ef1.Height))
                    {
                        ef2.Height = 0f;
                    }
                    else
                    {
                        ef2.Height = (ef1.Y + ef1.Height) - newPoint.Y;
                    }
                    if (newPoint.X < ef1.X)
                    {
                        ef2.Width = ef1.Width;
                    }
                    else if (newPoint.X > (ef1.X + ef1.Width))
                    {
                        ef2.Width = 0f;
                    }
                    else
                    {
                        ef2.Width = (ef1.X + ef1.Width) - newPoint.X;
                    }
                }
                else if (this.Perspective == Perspective.TopLeft)
                {
                    if (newPoint.Y > (ef1.Y + ef1.Height))
                    {
                        ef2.Height = ef1.Height;
                    }
                    else if (newPoint.Y < ef1.Y)
                    {
                        ef2.Height = 0f;
                    }
                    else
                    {
                        ef2.Height = newPoint.Y - ef1.Y;
                    }
                    if (newPoint.X > (ef1.X + ef1.Width))
                    {
                        ef2.Width = ef1.Width;
                    }
                    else if (newPoint.X < ef1.X)
                    {
                        ef2.Width = 0f;
                    }
                    else
                    {
                        ef2.Width = newPoint.X - ef1.X;
                    }
                }
                else if (this.Perspective == Perspective.BottomLeft)
                {
                    if (newPoint.Y < ef1.Y)
                    {
                        ef2.Height = ef1.Height;
                    }
                    else if (newPoint.Y > (ef1.Y + ef1.Height))
                    {
                        ef2.Height = 0f;
                    }
                    else
                    {
                        ef2.Height = (ef1.Y + ef1.Height) - newPoint.Y;
                    }
                    if (newPoint.X > (ef1.X + ef1.Width))
                    {
                        ef2.Width = ef1.Width;
                    }
                    else if (newPoint.X < ef1.X)
                    {
                        ef2.Width = 0f;
                    }
                    else
                    {
                        ef2.Width = newPoint.X - ef1.X;
                    }
                }
                this.Depth = ef2;
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
            PointF tf7;
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            PointF[] tfArray1 = this.getPoints(0f, 0f);
            PointF tf1 = DiagramGraph.ExpandPointOnEdge(tfArray1[0], ef1, single1);
            PointF tf2 = DiagramGraph.ExpandPointOnEdge(tfArray1[4], ef1, single1);
            PointF tf3 = DiagramGraph.ExpandPointOnEdge(tfArray1[5], ef1, single1);
            PointF tf4 = DiagramGraph.ExpandPointOnEdge(tfArray1[6], ef1, single1);
            PointF tf5 = DiagramGraph.ExpandPointOnEdge(tfArray1[2], ef1, single1);
            PointF tf6 = DiagramGraph.ExpandPointOnEdge(tfArray1[3], ef1, single1);
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
            path1.AddLine(tfArray1[0], tfArray1[1]);
            path1.AddLine(tfArray1[1], tfArray1[2]);
            path1.AddLine(tfArray1[2], tfArray1[3]);
            path1.AddLine(tfArray1[3], tfArray1[0]);
            path1.AddLine(tfArray1[0], tfArray1[4]);
            path1.AddLine(tfArray1[4], tfArray1[5]);
            path1.AddLine(tfArray1[5], tfArray1[1]);
            path1.AddLine(tfArray1[2], tfArray1[6]);
            path1.AddLine(tfArray1[6], tfArray1[5]);
            path1.AddLine(tfArray1[5], tfArray1[1]);
            return path1;
        }

        private PointF[] getPoints(float offx, float offy)
        {
            RectangleF ef1 = this.Bounds;
            SizeF ef2 = this.Depth;
            ef1.X += offx;
            ef1.Y += offy;
            if (ef2.Width > ef1.Width)
            {
                ef2.Width = ef1.Width;
            }
            if (ef2.Height > ef1.Height)
            {
                ef2.Height = ef1.Height;
            }
            if (this.Perspective == Perspective.TopRight)
            {
                this.myPoints[0] = new PointF(ef1.X, ef1.Y + ef2.Height);
                this.myPoints[1] = new PointF((ef1.X + ef1.Width) - ef2.Width, ef1.Y + ef2.Height);
                this.myPoints[2] = new PointF((ef1.X + ef1.Width) - ef2.Width, ef1.Y + ef1.Height);
                this.myPoints[3] = new PointF(ef1.X, ef1.Y + ef1.Height);
                this.myPoints[4] = new PointF(ef1.X + ef2.Width, ef1.Y);
                this.myPoints[5] = new PointF(ef1.X + ef1.Width, ef1.Y);
                this.myPoints[6] = new PointF(ef1.X + ef1.Width, (ef1.Y + ef1.Height) - ef2.Height);
            }
            else if (this.Perspective == Perspective.BottomRight)
            {
                this.myPoints[0] = new PointF((ef1.X + ef1.Width) - ef2.Width, ef1.Y);
                this.myPoints[1] = new PointF((ef1.X + ef1.Width) - ef2.Width, (ef1.Y + ef1.Height) - ef2.Height);
                this.myPoints[2] = new PointF(ef1.X, (ef1.Y + ef1.Height) - ef2.Height);
                this.myPoints[3] = new PointF(ef1.X, ef1.Y);
                this.myPoints[4] = new PointF(ef1.X + ef1.Width, ef1.Y + ef2.Height);
                this.myPoints[5] = new PointF(ef1.X + ef1.Width, ef1.Y + ef1.Height);
                this.myPoints[6] = new PointF(ef1.X + ef2.Width, ef1.Y + ef1.Height);
            }
            else if (this.Perspective == Perspective.TopLeft)
            {
                this.myPoints[0] = new PointF(ef1.X + ef2.Width, ef1.Y + ef1.Height);
                this.myPoints[1] = new PointF(ef1.X + ef2.Width, ef1.Y + ef2.Height);
                this.myPoints[2] = new PointF(ef1.X + ef1.Width, ef1.Y + ef2.Height);
                this.myPoints[3] = new PointF(ef1.X + ef1.Width, ef1.Y + ef1.Height);
                this.myPoints[4] = new PointF(ef1.X, (ef1.Y + ef1.Height) - ef2.Height);
                this.myPoints[5] = new PointF(ef1.X, ef1.Y);
                this.myPoints[6] = new PointF((ef1.X + ef1.Width) - ef2.Width, ef1.Y);
            }
            else if (this.Perspective == Perspective.BottomLeft)
            {
                this.myPoints[0] = new PointF(ef1.X + ef1.Width, (ef1.Y + ef1.Height) - ef2.Height);
                this.myPoints[1] = new PointF(ef1.X + ef2.Width, (ef1.Y + ef1.Height) - ef2.Height);
                this.myPoints[2] = new PointF(ef1.X + ef2.Width, ef1.Y);
                this.myPoints[3] = new PointF(ef1.X + ef1.Width, ef1.Y);
                this.myPoints[4] = new PointF((ef1.X + ef1.Width) - ef2.Width, ef1.Y + ef1.Height);
                this.myPoints[5] = new PointF(ef1.X, ef1.Y + ef1.Height);
                this.myPoints[6] = new PointF(ef1.X, ef1.Y + ef2.Height);
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


        [TypeConverter(typeof(SizeFConverter)), Category("Appearance"), Description("The offset of the back square from the forward square giving the impression of depth.")]
        public virtual SizeF Depth
        {
            get
            {
                return this.myDepth;
            }
            set
            {
                SizeF ef1 = this.myDepth;
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
                    this.myDepth = value;
                    base.ResetPath();
                    this.Changed(0x5d3, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("The direction the back square is offset from the front square."), Category("Appearance"), DefaultValue(0)]
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
                    this.Changed(0x5d4, 0, perspective1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(true), Category("Behavior"), Description("Whether to add the depth control handle.")]
        public virtual bool ReshapableDepth
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
                    this.Changed(0x5d5, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedDepth = 0x5d3;
        public const int ChangedPerspective = 0x5d4;
        public const int ChangedReshapableDepth = 0x5d5;
        public const int DepthHandleID = 0x409;
        private const int flagReshapableDepth = 0x100000;
        private SizeF myDepth;
        private Perspective myPerspective;
        private PointF[] myPoints;
    }
}
