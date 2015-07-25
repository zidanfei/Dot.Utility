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
    public class ListGroup : GroupShape
    {
        public ListGroup()
        {
            this.myOrientation = System.Windows.Forms.Orientation.Vertical;
            this.mySpacing = 0f;
            this.myAlignment = 2;
            this.myLinePenInfo = DiagramGraph.GetPenInfo(null);
            this.myBorderPenInfo = DiagramGraph.GetPenInfo(null);
            this.myBrushInfo = DiagramGraph.GetBrushInfo(null);
            this.myPath = null;
            this.myCorner = new SizeF(0f, 0f);
            this.myTopLeftMargin = new SizeF(2f, 2f);
            this.myBottomRightMargin = new SizeF(2f, 2f);
            base.InternalFlags &= -17;
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            int num1 = e.SubHint;
            if (num1 != 0x3e9)
            {
                switch (num1)
                {
                    case 0x9c5:
                        {
                            base.Initializing = true;
                            this.Spacing = e.GetFloat(undo);
                            base.Initializing = false;
                            return;
                        }
                    case 0x9c6:
                        {
                            this.Alignment = e.GetInt(undo);
                            return;
                        }
                    case 0x9c7:
                        {
                            base.Initializing = true;
                            object obj1 = e.GetValue(undo);
                            if (!(obj1 is Pen))
                            {
                                if (obj1 is DiagramGraph.GoPenInfo)
                                {
                                    this.LinePen = ((DiagramGraph.GoPenInfo)obj1).GetPen();
                                }
                                goto Label_00BB;
                            }
                            this.LinePen = (Pen)obj1;
                            goto Label_00BB;
                        }
                    case 0x9c8:
                        {
                            object obj2 = e.GetValue(undo);
                            if (!(obj2 is Pen))
                            {
                                if (obj2 is DiagramGraph.GoPenInfo)
                                {
                                    this.BorderPen = ((DiagramGraph.GoPenInfo)obj2).GetPen();
                                }
                                return;
                            }
                            this.BorderPen = (Pen)obj2;
                            return;
                        }
                    case 0x9c9:
                        {
                            object obj3 = e.GetValue(undo);
                            if (!(obj3 is System.Drawing.Brush))
                            {
                                if (obj3 is DiagramGraph.GoBrushInfo)
                                {
                                    this.Brush = ((DiagramGraph.GoBrushInfo)obj3).GetBrush();
                                }
                                return;
                            }
                            this.Brush = (System.Drawing.Brush)obj3;
                            return;
                        }
                    case 0x9ca:
                        {
                            this.Corner = e.GetSize(undo);
                            return;
                        }
                    case 0x9cb:
                        {
                            base.Initializing = true;
                            this.TopLeftMargin = e.GetSize(undo);
                            base.Initializing = false;
                            return;
                        }
                    case 0x9cc:
                        {
                            base.Initializing = true;
                            this.BottomRightMargin = e.GetSize(undo);
                            base.Initializing = false;
                            return;
                        }
                    case 0x9cd:
                        {
                            base.Initializing = true;
                            this.Orientation = (System.Windows.Forms.Orientation)e.GetInt(undo);
                            base.Initializing = false;
                            return;
                        }
                }
                base.ChangeValue(e, undo);
                return;
            }
            base.ChangeValue(e, undo);
            this.ResetPath();
            return;
        Label_00BB:
            base.Initializing = false;
        }

        protected override RectangleF ComputeBounds()
        {
            GroupEnumerator enumerator1;
            RectangleF ef1 = this.Bounds;
            SizeF ef2 = this.TopLeftMargin;
            SizeF ef3 = this.BottomRightMargin;
            float single1 = 0f;
            float single2 = 0f;
            float single3 = this.Spacing;
            if (this.LinePenInfo != null)
            {
                single3 = System.Math.Max(this.LinePenInfo.Width, single3);
            }
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                enumerator1 = base.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj1 = enumerator1.Current;
                    if (obj1 != null)
                    {
                        single1 = System.Math.Max(single1, obj1.Width);
                        if (obj1.CanView())
                        {
                            if (single2 > 0f)
                            {
                                single2 += single3;
                            }
                            single2 += obj1.Height;
                        }
                    }
                }
            }
            else
            {
                enumerator1 = base.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj2 = enumerator1.Current;
                    if (obj2 != null)
                    {
                        single2 = System.Math.Max(single2, obj2.Height);
                        if (obj2.CanView())
                        {
                            if (single1 > 0f)
                            {
                                single1 += single3;
                            }
                            single1 += obj2.Width;
                        }
                    }
                }
            }
            ef1.Width = (single1 + ef2.Width) + ef3.Width;
            ef1.Height = (single2 + ef2.Height) + ef3.Height;
            return ef1;
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            ListGroup group1 = (ListGroup)newgroup;
            if (group1 != null)
            {
                group1.myPath = null;
            }
            base.CopyChildren(newgroup, env);
        }

        private void DisposePath(GraphicsPath path)
        {
            if (path != this.myPath)
            {
                path.Dispose();
            }
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            rect = base.ExpandPaintBounds(rect, view);
            if (this.BorderPenInfo != null)
            {
                float single1 = this.BorderPenInfo.Width / 2f;
                DiagramShape.InflateRect(ref rect, single1, single1);
            }
            return rect;
        }

        private GraphicsPath GetPath(float offx, float offy)
        {
            if ((offx != 0f) || (offy != 0f))
            {
                GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
                RoundedRectangleGraph.MakeRoundedRectangularPath(path1, offx, offy, this.Bounds, this.Corner);
                return path1;
            }
            if (this.myPath == null)
            {
                this.myPath = new GraphicsPath(FillMode.Winding);
                RoundedRectangleGraph.MakeRoundedRectangularPath(this.myPath, 0f, 0f, this.Bounds, this.Corner);
            }
            return this.myPath;
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing)
            {
                this.ResetPath();
                float single1 = base.Left;
                float single2 = base.Top;
                float single3 = 0f;
                float single4 = 0f;
                float single5 = this.Spacing;
                if (this.LinePenInfo != null)
                {
                    single5 = System.Math.Max(this.LinePenInfo.Width, single5);
                }
                GroupEnumerator enumerator1 = base.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj1 = enumerator1.Current;
                    if ((obj1 != null) && obj1.CanView())
                    {
                        single3 = System.Math.Max(single3, obj1.Width);
                        single4 = System.Math.Max(single4, obj1.Height);
                    }
                }
                SizeF ef1 = this.TopLeftMargin;
                SizeF ef2 = this.BottomRightMargin;
                float single6 = single1 + ef1.Width;
                float single7 = single2 + ef1.Height;
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    float single8 = single7;
                    for (int num1 = 0; num1 < this.Count; num1++)
                    {
                        float single9 = single8;
                        single8 = System.Math.Max(single8, this.LayoutItem(num1, new RectangleF(single6, single8, single3, single4 - single8)));
                        if (single8 > single9)
                        {
                            single8 += single5;
                        }
                    }
                }
                else
                {
                    float single10 = single6;
                    for (int num2 = 0; num2 < this.Count; num2++)
                    {
                        float single11 = single10;
                        single10 = System.Math.Max(single10, this.LayoutItem(num2, new RectangleF(single10, single7, single3 - single10, single4)));
                        if (single10 > single11)
                        {
                            single10 += single5;
                        }
                    }
                }
                base.InvalidBounds = true;
            }
        }

        public virtual float LayoutItem(int i, RectangleF cell)
        {
            PointF tf1;
            float single2;
            DiagramShape obj2;
            PointF tf2;
            int num1;
            if (this.Orientation != System.Windows.Forms.Orientation.Vertical)
            {
                single2 = cell.X;
                obj2 = this[i];
                if ((obj2 == null) || !obj2.CanView())
                {
                    obj2.Position = new PointF(cell.X, cell.Y);
                    return single2;
                }
                num1 = this.Alignment;
                if (num1 <= 0x10)
                {
                    switch (num1)
                    {
                        case 1:
                        case 3:
                            {
                                goto Label_019A;
                            }
                        case 2:
                        case 4:
                            {
                                goto Label_0189;
                            }
                        case 8:
                        case 0x10:
                            {
                                goto Label_01C1;
                            }
                    }
                    goto Label_019A;
                }
                if (num1 <= 0x40)
                {
                    if (num1 == 0x20)
                    {
                        goto Label_0189;
                    }
                    if (num1 == 0x40)
                    {
                    }
                    goto Label_019A;
                }
                if (num1 == 0x80)
                {
                    goto Label_01C1;
                }
                if (num1 != 0x100)
                {
                }
                goto Label_019A;
            }
            float single1 = cell.Y;
            DiagramShape obj1 = this[i];
            if ((obj1 == null) || !obj1.CanView())
            {
                obj1.Position = new PointF(cell.X, cell.Y);
                return single1;
            }
            num1 = this.Alignment;
            if (num1 <= 0x10)
            {
                switch (num1)
                {
                    case 1:
                    case 3:
                        {
                            goto Label_0098;
                        }
                    case 2:
                    case 0x10:
                        {
                            goto Label_0087;
                        }
                    case 4:
                    case 8:
                        {
                            goto Label_00BE;
                        }
                }
                goto Label_0098;
            }
            if (num1 <= 0x40)
            {
                if ((num1 != 0x20) && (num1 == 0x40))
                {
                    goto Label_00BE;
                }
                goto Label_0098;
            }
            if ((num1 == 0x80) || (num1 != 0x100))
            {
                goto Label_0098;
            }
        Label_0087:
            tf1 = new PointF(cell.X, single1);
            goto Label_00DC;
        Label_0098:
            tf1 = new PointF(cell.X + ((cell.Width - obj1.Width) / 2f), single1);
            goto Label_00DC;
        Label_00BE:
            tf1 = new PointF((cell.X + cell.Width) - obj1.Width, single1);
        Label_00DC:
            obj1.Position = tf1;
            return (single1 + obj1.Height);
        Label_0189:
            tf2 = new PointF(single2, cell.Y);
            goto Label_01E0;
        Label_019A:
            tf2 = new PointF(single2, cell.Y + ((cell.Height - obj2.Height) / 2f));
            goto Label_01E0;
        Label_01C1:
            tf2 = new PointF(single2, (cell.Y + cell.Height) - obj2.Height);
        Label_01E0:
            obj2.Position = tf2;
            return (single2 + obj2.Width);
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            this.PaintDecoration(g, view);
            base.Paint(g, view);
        }

        public virtual void PaintDecoration(Graphics g, DiagramView view)
        {
            if (this.Shadowed)
            {
                SizeF ef1 = this.GetShadowOffset(view);
                GraphicsPath path1 = this.GetPath(ef1.Width, ef1.Height);
                if (this.Brush != null)
                {
                    System.Drawing.Brush brush1 = this.GetShadowBrush(view);
                    DiagramGraph.DrawPath(g, view, null, brush1, path1);
                }
                else if (this.BorderPen != null)
                {
                    Pen pen1 = this.GetShadowPen(view, DiagramGraph.GetPenWidth(this.BorderPen));
                    DiagramGraph.DrawPath(g, view, pen1, null, path1);
                }
                this.DisposePath(path1);
            }
            GraphicsPath path2 = this.GetPath(0f, 0f);
            DiagramGraph.DrawPath(g, view, this.BorderPen, this.Brush, path2);
            Pen pen2 = this.LinePen;
            if (pen2 != null)
            {
                float single1 = base.Left;
                float single2 = base.Top;
                float single3 = base.Width;
                float single4 = base.Height;
                float single5 = System.Math.Max(DiagramGraph.GetPenWidth(pen2), this.Spacing);
                SizeF ef2 = this.TopLeftMargin;
                SizeF ef3 = this.BottomRightMargin;
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    float single6 = single2 + ef2.Height;
                    float single7 = 0f;
                    for (int num1 = 0; num1 < this.Count; num1++)
                    {
                        DiagramShape obj1 = this[num1];
                        if ((obj1 != null) && obj1.CanView())
                        {
                            if (single7 > 0f)
                            {
                                DiagramGraph.DrawLine(g, view, pen2, single1, (single6 + single7) + (single5 / 2f), single1 + single3, (single6 + single7) + (single5 / 2f));
                                single7 += single5;
                            }
                            single7 += obj1.Height;
                        }
                    }
                }
                else
                {
                    float single8 = single1 + ef2.Width;
                    float single9 = 0f;
                    for (int num2 = 0; num2 < this.Count; num2++)
                    {
                        DiagramShape obj2 = this[num2];
                        if ((obj2 != null) && obj2.CanView())
                        {
                            if (single9 > 0f)
                            {
                                DiagramGraph.DrawLine(g, view, pen2, (single8 + single9) + (single5 / 2f), single2, (single8 + single9) + (single5 / 2f), single2 + single4);
                                single9 += single5;
                            }
                            single9 += obj2.Width;
                        }
                    }
                }
            }
            this.DisposePath(path2);
        }

        private void ResetPath()
        {
            if (this.myPath != null)
            {
                this.myPath.Dispose();
                this.myPath = null;
            }
        }


        [Description("How each item is positioned along the X axis."), Category("Appearance"), DefaultValue(2)]
        public virtual int Alignment
        {
            get
            {
                return this.myAlignment;
            }
            set
            {
                int num1 = this.myAlignment;
                if (num1 != value)
                {
                    this.myAlignment = value;
                    this.Changed(0x9c6, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue((string)null), Description("The pen used to draw an outline for this node."), Category("Appearance")]
        public virtual Pen BorderPen
        {
            get
            {
                if (this.myBorderPenInfo != null)
                {
                    return this.myBorderPenInfo.GetPen();
                }
                return null;
            }
            set
            {
                DiagramGraph.GoPenInfo info1 = this.myBorderPenInfo;
                DiagramGraph.GoPenInfo info2 = DiagramGraph.GetPenInfo(value);
                if (info1 != info2)
                {
                    this.myBorderPenInfo = info2;
                    this.ResetPath();
                    this.Changed(0x9c8, 0, info1, DiagramShape.NullRect, 0, info2, DiagramShape.NullRect);
                    base.InvalidatePaintBounds();
                }
            }
        }

        internal DiagramGraph.GoPenInfo BorderPenInfo
        {
            get
            {
                return this.myBorderPenInfo;
            }
        }

        [Category("Appearance"), Description("The margin around the text inside the background at the right side and the bottom"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF BottomRightMargin
        {
            get
            {
                return this.myBottomRightMargin;
            }
            set
            {
                SizeF ef1 = this.myBottomRightMargin;
                if (((ef1 != value) && (value.Width >= 0f)) && (value.Height >= 0f))
                {
                    this.myBottomRightMargin = value;
                    this.Changed(0x9cc, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }

        public override RectangleF Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                this.ResetPath();
                base.Bounds = value;
            }
        }

        [DefaultValue((string)null), Description("The brush used to fill the outline of this shape."), Category("Appearance")]
        public virtual System.Drawing.Brush Brush
        {
            get
            {
                if (this.myBrushInfo != null)
                {
                    return this.myBrushInfo.GetBrush();
                }
                return null;
            }
            set
            {
                DiagramGraph.GoBrushInfo info1 = this.myBrushInfo;
                DiagramGraph.GoBrushInfo info2 = DiagramGraph.GetBrushInfo(value);
                if (info1 != info2)
                {
                    this.myBrushInfo = info2;
                    this.Changed(0x9c9, 0, info1, DiagramShape.NullRect, 0, info2, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), TypeConverter(typeof(SizeFConverter)), Description("The maximum radial width and height of each corner")]
        public virtual SizeF Corner
        {
            get
            {
                return this.myCorner;
            }
            set
            {
                SizeF ef1 = this.myCorner;
                if (((ef1 != value) && (value.Width >= 0f)) && (value.Height >= 0f))
                {
                    this.myCorner = value;
                    this.ResetPath();
                    this.Changed(0x9ca, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("The pen used to draw lines separating the items."), Category("Appearance"), DefaultValue((string)null)]
        public virtual Pen LinePen
        {
            get
            {
                if (this.myLinePenInfo != null)
                {
                    return this.myLinePenInfo.GetPen();
                }
                return null;
            }
            set
            {
                DiagramGraph.GoPenInfo info1 = this.myLinePenInfo;
                DiagramGraph.GoPenInfo info2 = DiagramGraph.GetPenInfo(value);
                if (info1 != info2)
                {
                    this.myLinePenInfo = info2;
                    this.Changed(0x9c7, 0, info1, DiagramShape.NullRect, 0, info2, DiagramShape.NullRect);
                    this.LayoutChildren(null);
                }
            }
        }

        internal DiagramGraph.GoPenInfo LinePenInfo
        {
            get
            {
                return this.myLinePenInfo;
            }
        }

        [DefaultValue(1), Category("Appearance"), Description("How LayoutChildren will position the items.")]
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
                    this.Changed(0x9cd, (int)orientation1, null, DiagramShape.NullRect, (int)value, null, DiagramShape.NullRect);
                    this.LayoutChildren(null);
                }
            }
        }

        [Description("The additional vertical distance between items."), Category("Appearance"), DefaultValue(0)]
        public virtual float Spacing
        {
            get
            {
                return this.mySpacing;
            }
            set
            {
                float single1 = this.mySpacing;
                if (single1 != value)
                {
                    this.mySpacing = value;
                    this.Changed(0x9c5, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }

        [Description("The margin around the text inside the background at the left side and the top"), TypeConverter(typeof(SizeFConverter)), Category("Appearance")]
        public virtual SizeF TopLeftMargin
        {
            get
            {
                return this.myTopLeftMargin;
            }
            set
            {
                SizeF ef1 = this.myTopLeftMargin;
                if (((ef1 != value) && (value.Width >= 0f)) && (value.Height >= 0f))
                {
                    this.myTopLeftMargin = value;
                    this.Changed(0x9cb, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }


        public const int ChangedAlignment = 0x9c6;
        public const int ChangedBorderPen = 0x9c8;
        public const int ChangedBottomRightMargin = 0x9cc;
        public const int ChangedBrush = 0x9c9;
        public const int ChangedCorner = 0x9ca;
        public const int ChangedLinePen = 0x9c7;
        public const int ChangedOrientation = 0x9cd;
        public const int ChangedSpacing = 0x9c5;
        public const int ChangedTopLeftMargin = 0x9cb;
        private int myAlignment;
        private DiagramGraph.GoPenInfo myBorderPenInfo;
        private SizeF myBottomRightMargin;
        private DiagramGraph.GoBrushInfo myBrushInfo;
        private SizeF myCorner;
        private DiagramGraph.GoPenInfo myLinePenInfo;
        private System.Windows.Forms.Orientation myOrientation;
        [NonSerialized]
        private GraphicsPath myPath;
        private float mySpacing;
        private SizeF myTopLeftMargin;
    }
}
