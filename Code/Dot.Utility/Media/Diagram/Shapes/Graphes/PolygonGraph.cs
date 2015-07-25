using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class PolygonGraph : DiagramGraph
    {
        public PolygonGraph()
        {
            this.myStyle = PolygonGraphStyle.Line;
            this.myPointsCount = 0;
            this.myPoints = new PointF[6];
            base.InternalFlags |= 0x200;
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
            this.Changed(0x579, num2, null, DiagramShape.MakeRect(p), num2, null, DiagramShape.MakeRect(p));
            return num2;
        }

        public int AddPoint(float x, float y)
        {
            return this.AddPoint(new PointF(x, y));
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
                int num1 = this.PointsCount - 1;
                for (int num2 = 0; num2 <= num1; num2++)
                {
                    PointF tf1 = this.GetPoint(num2);
                    sel.CreateResizeHandle(this, selectedObj, tf1, 0x2000 + num2, true);
                }
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x579:
                    {
                        if (!undo)
                        {
                            this.InsertPoint(e.OldInt, new PointF(e.NewRect.X, e.NewRect.Y));
                            return;
                        }
                        this.RemovePoint(e.OldInt);
                        return;
                    }
                case 0x57a:
                    {
                        if (!undo)
                        {
                            this.RemovePoint(e.OldInt);
                            return;
                        }
                        this.InsertPoint(e.OldInt, new PointF(e.OldRect.X, e.OldRect.Y));
                        return;
                    }
                case 0x57b:
                    {
                        if (!undo)
                        {
                            this.SetPoint(e.OldInt, new PointF(e.NewRect.X, e.NewRect.Y));
                            return;
                        }
                        this.SetPoint(e.OldInt, new PointF(e.OldRect.X, e.OldRect.Y));
                        return;
                    }
                case 0x584:
                    {
                        PointF[] tfArray1 = (PointF[])e.GetValue(undo);
                        this.SetPoints(tfArray1);
                        return;
                    }
                case 0x586:
                    {
                        this.Style = (PolygonGraphStyle)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        public virtual void ClearPoints()
        {
            this.Changing(0x584);
            base.ResetPath();
            this.myPointsCount = 0;
            base.InvalidBounds = true;
            this.Changed(0x584, 0, null, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
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
            if (this.Style == PolygonGraphStyle.Bezier)
            {
                for (int num2 = 3; num2 < this.myPointsCount; num2 += 3)
                {
                    PointF tf3 = this.GetPoint(num2 - 3);
                    PointF tf4 = this.GetPoint(num2 - 2);
                    if ((num2 + 3) >= this.myPointsCount)
                    {
                        num2 = this.myPointsCount - 1;
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
            if (!base.ContainsPoint(p))
            {
                return false;
            }
            GraphicsPath path1 = this.GetPath(0f, 0f);
            bool flag1 = path1.IsVisible(p);
            base.DisposePath(path1);
            return flag1;
        }

        public override void CopyNewValueForRedo(ChangedEventArgs e)
        {
            if (e.SubHint == 0x584)
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
            PolygonGraph polygon1 = (PolygonGraph)base.CopyObject(env);
            if (polygon1 != null)
            {
                polygon1.myPoints = (PointF[])this.myPoints.Clone();
            }
            return polygon1;
        }

        public override void CopyOldValueForUndo(ChangedEventArgs e)
        {
            if (e.SubHint == 0x584)
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

        [Description("A copy of the array of points in this polygon."), Category("Appearance")]
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

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            PointF tf1;
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth / 2f;
            float single2 = 1E+21f;
            PointF tf2 = new PointF();
            if (this.Style == PolygonGraphStyle.Bezier)
            {
                for (int num1 = 3; num1 < this.myPointsCount; num1 += 3)
                {
                    PointF tf3 = this.GetPoint(num1 - 3);
                    PointF tf4 = this.GetPoint(num1 - 2);
                    if ((num1 + 3) >= this.myPointsCount)
                    {
                        num1 = this.myPointsCount - 1;
                    }
                    PointF tf5 = this.GetPoint(num1 - 1);
                    PointF tf6 = this.GetPoint(num1);
                    if (StrokeGraph.BezierNearestIntersectionOnLine(tf3, tf4, tf5, tf6, p1, p2, out tf1))
                    {
                        float single3 = ((tf1.X - p1.X) * (tf1.X - p1.X)) + ((tf1.Y - p1.Y) * (tf1.Y - p1.Y));
                        if (single3 < single2)
                        {
                            single2 = single3;
                            tf2 = tf1;
                        }
                    }
                }
            }
            else
            {
                for (int num2 = 0; num2 < this.PointsCount; num2++)
                {
                    PointF tf7 = DiagramGraph.ExpandPointOnEdge(this.GetPoint(num2), ef1, single1);
                    PointF tf8 = DiagramGraph.ExpandPointOnEdge(this.GetPoint(((num2 + 1) < this.PointsCount) ? (num2 + 1) : 0), ef1, single1);
                    if (StrokeGraph.NearestIntersectionOnLine(tf7, tf8, p1, p2, out tf1))
                    {
                        float single4 = ((tf1.X - p1.X) * (tf1.X - p1.X)) + ((tf1.Y - p1.Y) * (tf1.Y - p1.Y));
                        if (single4 < single2)
                        {
                            single2 = single4;
                            tf2 = tf1;
                        }
                    }
                }
            }
            result = tf2;
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
            int num1 = this.PointsCount;
            PointF[] tfArray1 = new PointF[num1];
            for (int num2 = 0; num2 < num1; num2++)
            {
                PointF tf1 = this.GetPoint(num2);
                tfArray1[num2].X = tf1.X + offx;
                tfArray1[num2].Y = tf1.Y + offy;
            }
            bool flag1 = this.Style == PolygonGraphStyle.Bezier;
            if (flag1 && ((num1 % 3) != 1))
            {
                DiagramShape.Trace("Polygon has wrong number of points: " + num1.ToString(NumberFormatInfo.InvariantInfo) + "; should have 3n+1 points");
                flag1 = false;
            }
            if (flag1)
            {
                path1.AddBeziers(tfArray1);
            }
            else
            {
                path1.AddLines(tfArray1);
            }
            path1.CloseAllFigures();
            return path1;
        }

        public virtual PointF GetPoint(int i)
        {
            if ((i < 0) || (i >= this.myPointsCount))
            {
                throw new ArgumentException("GoPolygon.GetPoint given an invalid index");
            }
            return this.myPoints[i];
        }

        public virtual void InsertPoint(int i, PointF p)
        {
            if (i < 0)
            {
                throw new ArgumentException("GoPolygon.InsertPoint given an invalid index, less than zero");
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
            this.Changed(0x579, i, null, DiagramShape.MakeRect(p), i, null, DiagramShape.MakeRect(p));
        }

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = this.GetPath(0f, 0f);
            return (GraphicsPath)path1.Clone();
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
                        this.Changing(0x584);
                    }
                    base.SuspendsUpdates = true;
                    for (int num1 = 0; num1 < this.PointsCount; num1++)
                    {
                        PointF tf1 = this.GetPoint(num1);
                        float single3 = tf1.X + single1;
                        float single4 = tf1.Y + single2;
                        this.SetPoint(num1, new PointF(single3, single4));
                    }
                    base.InvalidBounds = false;
                    base.SuspendsUpdates = flag1;
                    if (flag1)
                    {
                        return;
                    }
                    this.Changed(0x584, 0, null, old, 0, null, ef2);
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
                    this.Changing(0x584);
                }
                base.SuspendsUpdates = true;
                for (int num2 = 0; num2 < this.PointsCount; num2++)
                {
                    PointF tf2 = this.GetPoint(num2);
                    float single7 = ef3.X + ((tf2.X - old.X) * single5);
                    float single8 = ef3.Y + ((tf2.Y - old.Y) * single6);
                    this.SetPoint(num2, new PointF(single7, single8));
                }
                base.InvalidBounds = false;
                base.SuspendsUpdates = flag2;
                if (!flag2)
                {
                    this.Changed(0x584, 0, null, old, 0, null, ef3);
                }
            }
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            if (this.Shadowed)
            {
                SizeF ef1 = this.GetShadowOffset(view);
                if (this.Brush != null)
                {
                    Brush brush1 = this.GetShadowBrush(view);
                    if (brush1 != null)
                    {
                        GraphicsPath path1 = this.GetPath(ef1.Width, ef1.Height);
                        DiagramGraph.DrawPath(g, view, null, brush1, path1);
                        base.DisposePath(path1);
                    }
                }
                else if (this.Pen != null)
                {
                    Pen pen1 = this.GetShadowPen(view, base.InternalPenWidth);
                    if (pen1 != null)
                    {
                        GraphicsPath path2 = this.GetPath(ef1.Width, ef1.Height);
                        DiagramGraph.DrawPath(g, view, pen1, null, path2);
                        base.DisposePath(path2);
                    }
                }
            }
            GraphicsPath path3 = this.GetPath(0f, 0f);
            DiagramGraph.DrawPath(g, view, this.Pen, this.Brush, path3);
            base.DisposePath(path3);
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
                this.Changed(0x57a, i, null, DiagramShape.MakeRect(tf1), i, null, DiagramShape.MakeRect(tf1));
            }
        }

        public virtual void SetPoint(int i, PointF p)
        {
            if ((i < 0) || (i >= this.myPointsCount))
            {
                throw new ArgumentException("GoPolygon.SetPoint given an invalid index");
            }
            PointF tf1 = this.myPoints[i];
            if (tf1 != p)
            {
                base.ResetPath();
                this.myPoints[i] = p;
                base.InvalidBounds = true;
                this.Changed(0x57b, i, null, DiagramShape.MakeRect(tf1), i, null, DiagramShape.MakeRect(p));
            }
        }

        public virtual void SetPoints(PointF[] points)
        {
            this.Changing(0x584);
            base.ResetPath();
            int num1 = points.Length;
            if (num1 > this.myPoints.Length)
            {
                this.myPoints = new PointF[num1];
            }
            Array.Copy(points, 0, this.myPoints, 0, num1);
            this.myPointsCount = num1;
            base.InvalidBounds = true;
            this.Changed(0x584, 0, null, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
        }


        [Category("Appearance"), Description("The number of points in this polygon.")]
        public virtual int PointsCount
        {
            get
            {
                return this.myPointsCount;
            }
        }

        [Category("Appearance"), Description("The kind of line or curve drawn using this polygon's points."), DefaultValue(0)]
        public virtual PolygonGraphStyle Style
        {
            get
            {
                return this.myStyle;
            }
            set
            {
                PolygonGraphStyle style1 = this.myStyle;
                if (style1 != value)
                {
                    this.myStyle = value;
                    base.ResetPath();
                    this.Changed(0x586, 0, style1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedAddPoint = 0x579;
        public const int ChangedAllPoints = 0x584;
        public const int ChangedModifiedPoint = 0x57b;
        public const int ChangedRemovePoint = 0x57a;
        public const int ChangedStyle = 0x586;
        private PointF[] myPoints;
        private int myPointsCount;
        private PolygonGraphStyle myStyle;
    }
}
