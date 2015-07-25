using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public abstract class DiagramGraph : DiagramShape
    {
        static DiagramGraph()
        {
            DiagramGraph.myDrawers = new Hashtable();
            DiagramGraph.myInfos = new Hashtable();
            DiagramGraph.myCounter = 0;
            DiagramGraph.Pens_Black = Pens.Black;
            DiagramGraph.Pens_Gray = Pens.Gray;
            DiagramGraph.Pens_LightGray = Pens.LightGray;
            DiagramGraph.SystemPens_Control = SystemPens.Control;
            DiagramGraph.SystemPens_ControlDarkDark = SystemPens.ControlDarkDark;
            DiagramGraph.SystemPens_ControlDark = SystemPens.ControlDark;
            DiagramGraph.SystemPens_ControlLightLight = SystemPens.ControlLightLight;
            DiagramGraph.SystemPens_WindowFrame = SystemPens.WindowFrame;
            DiagramGraph.Brushes_Black = Brushes.Black;
            DiagramGraph.Brushes_Gray = Brushes.Gray;
            DiagramGraph.Brushes_LightGray = Brushes.LightGray;
            DiagramGraph.Brushes_White = Brushes.White;
            DiagramGraph.Brushes_Yellow = Brushes.Yellow;
            DiagramGraph.Brushes_LemonChiffon = Brushes.LemonChiffon;
            DiagramGraph.Brushes_Gold = Brushes.Gold;
            DiagramGraph.SystemBrushes_Control = SystemBrushes.Control;
        }

        protected DiagramGraph()
        {
            this.myPenInfo = DiagramGraph.GetPenInfo(DiagramGraph.Pens_Black);
            this.myBrushInfo = DiagramGraph.GetBrushInfo(null);
            this.myPath = null;
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            int num1 = e.SubHint;
            if (num1 != 0x3e9)
            {
                switch (num1)
                {
                    case 0x44d:
                        {
                            object obj1 = e.GetValue(undo);
                            if (!(obj1 is System.Drawing.Pen))
                            {
                                if (obj1 is GoPenInfo)
                                {
                                    this.Pen = ((GoPenInfo)obj1).GetPen();
                                }
                                return;
                            }
                            this.Pen = (System.Drawing.Pen)obj1;
                            return;
                        }
                    case 0x44e:
                        {
                            object obj2 = e.GetValue(undo);
                            if (!(obj2 is System.Drawing.Brush))
                            {
                                if (obj2 is GoBrushInfo)
                                {
                                    this.Brush = ((GoBrushInfo)obj2).GetBrush();
                                }
                                return;
                            }
                            this.Brush = (System.Drawing.Brush)obj2;
                            return;
                        }
                }
                base.ChangeValue(e, undo);
            }
            else
            {
                base.ChangeValue(e, undo);
                this.ResetPath();
            }
        }

        internal static void CleanInfos()
        {
            if (DiagramGraph.myCounter++ >= 100)
            {
                DiagramGraph.myCounter = 0;
                GC.Collect();
                ArrayList list1 = new ArrayList();
                IDictionaryEnumerator enumerator1 = DiagramGraph.myDrawers.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    WeakHashRef ref1 = (WeakHashRef)enumerator1.Key;
                    if (!ref1.IsAlive)
                    {
                        list1.Add(ref1);
                    }
                }
                foreach (WeakHashRef ref2 in list1)
                {
                    DiagramGraph.myDrawers.Remove(ref2);
                }
                list1.Clear();
                enumerator1 = DiagramGraph.myInfos.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    WeakHashRef ref3 = (WeakHashRef)enumerator1.Key;
                    if (!ref3.IsAlive)
                    {
                        list1.Add(ref3);
                    }
                }
                foreach (WeakHashRef ref4 in list1)
                {
                    DiagramGraph.myInfos.Remove(ref4);
                }
                list1.Clear();
            }
        }

        public override bool ContainedByRectangle(RectangleF r)
        {
            RectangleF ef1 = this.Bounds;
            float single1 = this.InternalPenWidth;
            DiagramShape.InflateRect(ref ef1, single1 / 2f, single1 / 2f);
            if ((((r.Width > 0f) && (r.Height > 0f)) && ((ef1.Width >= 0f) && (ef1.Height >= 0f))) && (((ef1.X >= r.X) && (ef1.Y >= r.Y)) && ((ef1.X + ef1.Width) <= (r.X + r.Width))))
            {
                return ((ef1.Y + ef1.Height) <= (r.Y + r.Height));
            }
            return false;
        }

        public override bool ContainsPoint(PointF p)
        {
            RectangleF ef1 = this.Bounds;
            float single1 = this.InternalPenWidth;
            DiagramShape.InflateRect(ref ef1, single1 / 2f, single1 / 2f);
            return DiagramShape.ContainsRect(ef1, p);
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            DiagramGraph shape1 = (DiagramGraph)base.CopyObject(env);
            if (shape1 != null)
            {
                shape1.myPath = null;
            }
            return shape1;
        }

        internal void DisposePath(GraphicsPath path)
        {
            if (path != this.myPath)
            {
                path.Dispose();
            }
        }

        public static void DrawBezier(Graphics g, DiagramView view, System.Drawing.Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            if (pen != null)
            {
                g.DrawBezier(pen, x1, y1, x2, y2, x3, y3, x4, y4);
            }
        }

        public static void DrawEllipse(Graphics g, DiagramView view, System.Drawing.Pen pen, System.Drawing.Brush brush, float x, float y, float width, float height)
        {
            if (brush != null)
            {
                g.FillEllipse(brush, x, y, width, height);
            }
            if (pen != null)
            {
                g.DrawEllipse(pen, x, y, width, height);
            }
        }

        public static void DrawLine(Graphics g, DiagramView view, System.Drawing.Pen pen, float x1, float y1, float x2, float y2)
        {
            if (pen != null)
            {
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        public static void DrawLines(Graphics g, DiagramView view, System.Drawing.Pen pen, PointF[] points)
        {
            if (pen != null)
            {
                g.DrawLines(pen, points);
            }
        }

        internal static void DrawPath(Graphics g, DiagramView view, System.Drawing.Pen pen, System.Drawing.Brush brush, GraphicsPath path)
        {
            if (brush != null)
            {
                g.FillPath(brush, path);
            }
            if (pen != null)
            {
                g.DrawPath(pen, path);
            }
        }

        public static void DrawPie(Graphics g, DiagramView view, System.Drawing.Pen pen, System.Drawing.Brush brush, float x, float y, float width, float height, float startangle, float sweepangle)
        {
            if (brush != null)
            {
                g.FillPie(brush, x, y, width, height, startangle, sweepangle);
            }
            if (pen != null)
            {
                g.DrawPie(pen, x, y, width, height, startangle, sweepangle);
            }
        }

        public static void DrawPolygon(Graphics g, DiagramView view, System.Drawing.Pen pen, System.Drawing.Brush brush, PointF[] points)
        {
            if (brush != null)
            {
                g.FillPolygon(brush, points);
            }
            if (pen != null)
            {
                g.DrawPolygon(pen, points);
            }
        }

        public static void DrawRectangle(Graphics g, DiagramView view, System.Drawing.Pen pen, System.Drawing.Brush brush, float x, float y, float width, float height)
        {
            if (brush != null)
            {
                g.FillRectangle(brush, x, y, width, height);
            }
            if (pen != null)
            {
                g.DrawRectangle(pen, x, y, width, height);
            }
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            if (this.Pen != null)
            {
                float single1 = System.Math.Max(System.Math.Max(this.InternalPenWidth, (float)1f), (float)(this.PenInfo.MiterLimit + 1f));
                DiagramShape.InflateRect(ref rect, single1, single1);
            }
            if (this.Shadowed)
            {
                SizeF ef1 = this.GetShadowOffset(view);
                if (ef1.Width < 0f)
                {
                    rect.X += ef1.Width;
                    rect.Width -= ef1.Width;
                }
                else
                {
                    rect.Width += ef1.Width;
                }
                if (ef1.Height < 0f)
                {
                    rect.Y += ef1.Height;
                    rect.Height -= ef1.Height;
                    return rect;
                }
                rect.Height += ef1.Height;
            }
            return rect;
        }

        public static PointF ExpandPointOnEdge(PointF p, RectangleF rect, float shift)
        {
            if (p.X <= rect.X)
            {
                p.X -= shift;
            }
            else if (p.X >= (rect.X + rect.Width))
            {
                p.X += shift;
            }
            if (p.Y <= rect.Y)
            {
                p.Y -= shift;
                return p;
            }
            if (p.Y >= (rect.Y + rect.Height))
            {
                p.Y += shift;
            }
            return p;
        }

        internal static GoBrushInfo GetBrushInfo(System.Drawing.Brush b)
        {
            GoBrushInfo info3;
            if (b == null)
            {
                return null;
            }
            lock (DiagramGraph.myDrawers)
            {
                WeakHashRef ref1 = new WeakHashRef(b);
                WeakReference reference1 = (WeakReference)DiagramGraph.myDrawers[ref1];
                GoBrushInfo info1 = null;
                if (reference1 != null)
                {
                    if (reference1.IsAlive)
                    {
                        info1 = reference1.Target as GoBrushInfo;
                    }
                    else
                    {
                        DiagramGraph.myDrawers.Remove(ref1);
                    }
                }
                if (info1 == null)
                {
                    info1 = new GoBrushInfo();
                    if (!info1.SetBrush(b))
                    {
                        return info1;
                    }
                    WeakHashRef ref2 = new WeakHashRef(info1);
                    reference1 = (WeakHashRef)DiagramGraph.myInfos[ref2];
                    if (reference1 != null)
                    {
                        if (reference1.IsAlive)
                        {
                            GoBrushInfo info2 = reference1.Target as GoBrushInfo;
                            if (info2 != null)
                            {
                                return info2;
                            }
                        }
                        else
                        {
                            DiagramGraph.myInfos.Remove(ref2);
                        }
                    }
                    DiagramGraph.myDrawers[ref1] = new WeakReference(info1);
                    DiagramGraph.myInfos[ref2] = ref2;
                    DiagramGraph.CleanInfos();
                }
                info3 = info1;
            }
            return info3;
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            RectangleF ef1 = this.Bounds;
            float single1 = this.InternalPenWidth;
            DiagramShape.InflateRect(ref ef1, single1 / 2f, single1 / 2f);
            return DiagramShape.GetNearestIntersectionPoint(ef1, p1, p2, out result);
        }

        protected GraphicsPath GetPath()
        {
            if (this.myPath == null)
            {
                this.myPath = this.MakePath();
            }
            return this.myPath;
        }

        internal static GoPenInfo GetPenInfo(System.Drawing.Pen p)
        {
            GoPenInfo info3;
            if (p == null)
            {
                return null;
            }
            lock (DiagramGraph.myDrawers)
            {
                WeakHashRef ref1 = new WeakHashRef(p);
                WeakReference reference1 = (WeakReference)DiagramGraph.myDrawers[ref1];
                GoPenInfo info1 = null;
                if (reference1 != null)
                {
                    if (reference1.IsAlive)
                    {
                        info1 = reference1.Target as GoPenInfo;
                    }
                    else
                    {
                        DiagramGraph.myDrawers.Remove(ref1);
                    }
                }
                if (info1 == null)
                {
                    info1 = new GoPenInfo();
                    if (!info1.SetPen(p))
                    {
                        return info1;
                    }
                    WeakHashRef ref2 = new WeakHashRef(info1);
                    reference1 = (WeakHashRef)DiagramGraph.myInfos[ref2];
                    if (reference1 != null)
                    {
                        if (reference1.IsAlive)
                        {
                            GoPenInfo info2 = reference1.Target as GoPenInfo;
                            if (info2 != null)
                            {
                                return info2;
                            }
                        }
                        else
                        {
                            DiagramGraph.myInfos.Remove(ref2);
                        }
                    }
                    DiagramGraph.myDrawers[ref1] = new WeakReference(info1);
                    DiagramGraph.myInfos[ref2] = ref2;
                    DiagramGraph.CleanInfos();
                }
                info3 = info1;
            }
            return info3;
        }

        internal static float GetPenWidth(System.Drawing.Pen pen)
        {
            if (pen == null)
            {
                return 0f;
            }
            return pen.Width;
        }

        public static float GetPenWidth(System.Drawing.Pen pen, DiagramView view)
        {
            if (pen == null)
            {
                return 0f;
            }
            GoPenInfo info1 = DiagramGraph.GetPenInfo(pen);
            float single1 = info1.Width;
            if (((single1 == 0f) && (view != null)) && (view.DocScale > 0f))
            {
                return (1f / view.DocScale);
            }
            return single1;
        }

        public virtual GraphicsPath MakePath()
        {
            GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
            path1.AddRectangle(this.Bounds);
            return path1;
        }

        protected void ResetPath()
        {
            if (this.myPath != null)
            {
                this.myPath.Dispose();
                this.myPath = null;
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

        [Category("Appearance"), Description("The brush used to fill the outline of this shape.")]
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
                GoBrushInfo info1 = this.myBrushInfo;
                GoBrushInfo info2 = DiagramGraph.GetBrushInfo(value);
                if (info1 != info2)
                {
                    this.myBrushInfo = info2;
                    this.Changed(0x44e, 0, info1, DiagramShape.NullRect, 0, info2, DiagramShape.NullRect);
                }
            }
        }

        internal float InternalPenWidth
        {
            get
            {
                if (this.PenInfo != null)
                {
                    return this.PenInfo.Width;
                }
                return 0f;
            }
        }

        [Description("The pen used to draw the outline of this shape."), Category("Appearance")]
        public virtual System.Drawing.Pen Pen
        {
            get
            {
                if (this.myPenInfo != null)
                {
                    return this.myPenInfo.GetPen();
                }
                return null;
            }
            set
            {
                GoPenInfo info1 = this.myPenInfo;
                GoPenInfo info2 = DiagramGraph.GetPenInfo(value);
                if (info1 != info2)
                {
                    this.myPenInfo = info2;
                    this.Changed(0x44d, 0, info1, DiagramShape.NullRect, 0, info2, DiagramShape.NullRect);
                    if (base.Parent != null)
                    {
                        base.Parent.InvalidatePaintBounds();
                    }
                }
            }
        }

        internal GoPenInfo PenInfo
        {
            get
            {
                return this.myPenInfo;
            }
        }


        internal static readonly System.Drawing.Brush Brushes_Black;
        internal static readonly System.Drawing.Brush Brushes_Gold;
        internal static readonly System.Drawing.Brush Brushes_Gray;
        internal static readonly System.Drawing.Brush Brushes_LemonChiffon;
        internal static readonly System.Drawing.Brush Brushes_LightGray;
        internal static readonly System.Drawing.Brush Brushes_White;
        internal static readonly System.Drawing.Brush Brushes_Yellow;
        public const int ChangedBrush = 0x44e;
        public const int ChangedPen = 0x44d;
        private GoBrushInfo myBrushInfo;
        internal static int myCounter;
        internal static Hashtable myDrawers;
        internal static Hashtable myInfos;
        [NonSerialized]
        internal GraphicsPath myPath;
        private GoPenInfo myPenInfo;
        internal static readonly System.Drawing.Pen Pens_Black;
        internal static readonly System.Drawing.Pen Pens_Gray;
        internal static readonly System.Drawing.Pen Pens_LightGray;
        internal static readonly System.Drawing.Brush SystemBrushes_Control;
        internal static readonly System.Drawing.Pen SystemPens_Control;
        internal static readonly System.Drawing.Pen SystemPens_ControlDark;
        internal static readonly System.Drawing.Pen SystemPens_ControlDarkDark;
        internal static readonly System.Drawing.Pen SystemPens_ControlLightLight;
        internal static readonly System.Drawing.Pen SystemPens_WindowFrame;

        [Serializable]
        internal sealed class GoBrushInfo
        {
            internal GoBrushInfo()
            {
            }

            public override bool Equals(object obj)
            {
                DiagramGraph.GoBrushInfo info1 = obj as DiagramGraph.GoBrushInfo;
                if ((info1 != null) && ((((this.myTypeName == info1.myTypeName) && (this.myColor == info1.myColor)) && ((this.myForeColor == info1.myForeColor) && (this.myHatchStyle == info1.myHatchStyle))) && (this.myImage == info1.myImage)))
                {
                    return (this.myWrapMode == info1.myWrapMode);
                }
                return false;
            }

            public Brush GetBrush()
            {
                if (this.myBrush == null)
                {
                    if (this.myTypeName == "SolidBrush")
                    {
                        this.myBrush = new SolidBrush(this.myColor);
                    }
                    else if (this.myTypeName == "HatchBrush")
                    {
                        this.myBrush = new HatchBrush(this.myHatchStyle, this.myForeColor, this.myColor);
                    }
                    else if (this.myTypeName == "TextureBrush")
                    {
                        this.myBrush = new TextureBrush(this.myImage, this.myWrapMode);
                    }
                    else
                    {
                        this.myBrush = DiagramGraph.Brushes_Gray;
                    }
                }
                return this.myBrush;
            }

            public override int GetHashCode()
            {
                return (int)(((((((HatchStyle)this.myTypeName.GetHashCode()) ^ ((HatchStyle)this.myColor.GetHashCode())) ^ ((HatchStyle)this.myForeColor.GetHashCode())) ^ this.myHatchStyle) ^ ((this.myImage != null) ? ((HatchStyle)this.myImage.GetHashCode()) : HatchStyle.Horizontal)) ^ ((HatchStyle)((int)this.myWrapMode)));
            }

            public bool SetBrush(Brush b)
            {
                this.myBrush = b;
                if (b is SolidBrush)
                {
                    SolidBrush brush1 = (SolidBrush)b;
                    this.myTypeName = "SolidBrush";
                    this.myColor = brush1.Color;
                }
                else if (b is HatchBrush)
                {
                    HatchBrush brush2 = (HatchBrush)b;
                    this.myTypeName = "HatchBrush";
                    this.myColor = brush2.BackgroundColor;
                    this.myForeColor = brush2.ForegroundColor;
                    this.myHatchStyle = brush2.HatchStyle;
                }
                else if (b is TextureBrush)
                {
                    TextureBrush brush3 = (TextureBrush)b;
                    this.myTypeName = "TextureBrush";
                    this.myImage = brush3.Image;
                    this.myWrapMode = brush3.WrapMode;
                }
                else
                {
                    this.myTypeName = "";
                    this.myImage = null;
                    return false;
                }
                return true;
            }

            public override string ToString()
            {
                string text1 = "BrushInfo: ";
                text1 = text1 + this.myTypeName;
                text1 = text1 + " ";
                return (text1 + this.myColor.ToString());
            }


            [NonSerialized]
            internal Brush myBrush;
            internal Color myColor;
            internal Color myForeColor;
            internal HatchStyle myHatchStyle;
            internal Image myImage;
            internal string myTypeName;
            internal WrapMode myWrapMode;
        }

        [Serializable]
        internal sealed class GoPenInfo
        {
            internal GoPenInfo()
            {
            }

            public override bool Equals(object obj)
            {
                DiagramGraph.GoPenInfo info1 = obj as DiagramGraph.GoPenInfo;
                if (info1 == null)
                {
                    return false;
                }
                bool flag1 = (((((this.myColor == info1.myColor) && (this.myWidth == info1.myWidth)) && ((this.myDashStyle == info1.myDashStyle) && (this.myDashCap == info1.myDashCap))) && (((this.myDashOffset == info1.myDashOffset) && (this.myAlignment == info1.myAlignment)) && ((this.myEndCap == info1.myEndCap) && (this.myStartCap == info1.myStartCap)))) && (this.myLineJoin == info1.myLineJoin)) && (this.myMiterLimit == info1.myMiterLimit);
                if (flag1 && (this.myDashStyle == DashStyle.Custom))
                {
                    if ((this.myDashPattern == null) && (info1.myDashPattern == null))
                    {
                        return true;
                    }
                    if ((this.myDashPattern == null) || (info1.myDashPattern == null))
                    {
                        return false;
                    }
                    if (this.myDashPattern.Length != info1.myDashPattern.Length)
                    {
                        return false;
                    }
                    for (int num1 = 0; num1 < this.myDashPattern.Length; num1++)
                    {
                        if (this.myDashPattern[num1] != info1.myDashPattern[num1])
                        {
                            return false;
                        }
                    }
                }
                return flag1;
            }

            public override int GetHashCode()
            {
                int num1 = (int)(((((((((((DashStyle)this.myColor.GetHashCode()) ^ ((DashStyle)this.myWidth.GetHashCode())) ^ this.myDashStyle) ^ ((DashStyle)((int)this.myDashCap))) ^ ((DashStyle)this.myDashOffset.GetHashCode())) ^ ((DashStyle)((int)this.myAlignment))) ^ ((DashStyle)((int)this.myEndCap))) ^ ((DashStyle)((int)this.myStartCap))) ^ ((DashStyle)((int)this.myLineJoin))) ^ ((DashStyle)this.myMiterLimit.GetHashCode()));
                if ((this.myDashStyle == DashStyle.Custom) && (this.myDashPattern != null))
                {
                    num1 ^= this.myDashPattern.GetHashCode();
                }
                return num1;
            }

            public Pen GetPen()
            {
                if (this.myPen == null)
                {
                    this.myPen = new Pen(this.myColor, this.myWidth);
                    this.myPen.DashStyle = this.myDashStyle;
                    this.myPen.DashCap = this.myDashCap;
                    this.myPen.DashOffset = this.myDashOffset;
                    if (this.myDashStyle == DashStyle.Custom)
                    {
                        this.myPen.DashPattern = this.myDashPattern;
                    }
                    this.myPen.Alignment = this.myAlignment;
                    this.myPen.EndCap = this.myEndCap;
                    this.myPen.StartCap = this.myStartCap;
                    this.myPen.LineJoin = this.myLineJoin;
                    this.myPen.MiterLimit = this.myMiterLimit;
                }
                return this.myPen;
            }

            public static System.Drawing.Color GetPenColor(Pen p, System.Drawing.Color def)
            {
                System.Drawing.Color color1;
                try
                {
                    color1 = p.Color;
                }
                catch (Exception)
                {
                    color1 = def;
                }
                return color1;
            }

            public bool SetPen(Pen p)
            {
                this.myPen = p;
                this.myColor = DiagramGraph.GoPenInfo.GetPenColor(p, System.Drawing.Color.Black);
                this.myWidth = p.Width;
                this.myDashStyle = p.DashStyle;
                this.myDashCap = p.DashCap;
                this.myDashOffset = p.DashOffset;
                if (this.myDashStyle == DashStyle.Custom)
                {
                    this.myDashPattern = p.DashPattern;
                }
                this.myAlignment = p.Alignment;
                this.myEndCap = p.EndCap;
                this.myStartCap = p.StartCap;
                this.myLineJoin = p.LineJoin;
                this.myMiterLimit = p.MiterLimit;
                return true;
            }

            public override string ToString()
            {
                string text1 = "PenInfo: ";
                text1 = text1 + this.myColor.ToString();
                text1 = text1 + " width ";
                text1 = text1 + this.myWidth.ToString();
                text1 = text1 + " align ";
                text1 = text1 + this.myAlignment.ToString();
                text1 = text1 + " dashstyle ";
                text1 = text1 + this.myDashStyle.ToString();
                text1 = text1 + " dashcap ";
                text1 = text1 + this.myDashCap.ToString();
                text1 = text1 + " dashoffset ";
                text1 = text1 + this.myDashOffset.ToString();
                if ((this.myDashStyle == DashStyle.Custom) && (this.myDashPattern != null))
                {
                    text1 = text1 + " dashpattern{";
                    for (int num1 = 0; num1 < this.myDashPattern.Length; num1++)
                    {
                        if (num1 > 0)
                        {
                            text1 = text1 + ", ";
                        }
                        text1 = text1 + this.myDashPattern[num1].ToString();
                    }
                    text1 = text1 + "}";
                }
                text1 = text1 + " endcap ";
                text1 = text1 + this.myEndCap.ToString();
                text1 = text1 + " startcap ";
                text1 = text1 + this.myStartCap.ToString();
                text1 = text1 + " join ";
                text1 = text1 + this.myLineJoin.ToString();
                text1 = text1 + " miterlim ";
                return (text1 + this.myMiterLimit.ToString());
            }


            public System.Drawing.Color Color
            {
                get
                {
                    return this.myColor;
                }
            }

            public float MiterLimit
            {
                get
                {
                    return this.myMiterLimit;
                }
            }

            public float Width
            {
                get
                {
                    return this.myWidth;
                }
            }


            internal PenAlignment myAlignment;
            internal System.Drawing.Color myColor;
            internal DashCap myDashCap;
            internal float myDashOffset;
            internal float[] myDashPattern;
            internal DashStyle myDashStyle;
            internal LineCap myEndCap;
            internal LineJoin myLineJoin;
            internal float myMiterLimit;
            [NonSerialized]
            internal Pen myPen;
            internal LineCap myStartCap;
            internal float myWidth;
        }

        internal sealed class WeakHashRef : WeakReference
        {
            internal WeakHashRef(object target)
                : base(target)
            {
                this.myHashed = false;
                this.myHashCode = 0;
                if (target == null)
                {
                    throw new ArgumentException("WeakHashRef created with null Target");
                }
            }

            public override bool Equals(object obj)
            {
                DiagramGraph.WeakHashRef ref1 = obj as DiagramGraph.WeakHashRef;
                if (!this.IsAlive)
                {
                    if ((this.myHashed && (ref1 != null)) && ref1.myHashed)
                    {
                        return (this.myHashCode == ref1.myHashCode);
                    }
                    return false;
                }
                if (ref1 != null)
                {
                    return this.Target.Equals(ref1.Target);
                }
                return (this.Target == obj);
            }

            public override int GetHashCode()
            {
                if (!this.myHashed)
                {
                    this.myHashed = true;
                    this.myHashCode = this.Target.GetHashCode();
                }
                return this.myHashCode;
            }


            private int myHashCode;
            private bool myHashed;
        }
    }
}
