using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class Balloon : CommentShape
    {
        public Balloon()
        {
            this.myAnchor = null;
            this.myCorner = new SizeF(4f, 4f);
            this.myBaseWidth = 30f;
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 2310:
                    {
                        base.Initializing = true;
                        this.Anchor = (DiagramShape)e.GetValue(undo);
                        base.Initializing = false;
                        return;
                    }
                case 0x907:
                    {
                        base.Initializing = true;
                        this.Corner = e.GetSize(undo);
                        base.Initializing = false;
                        return;
                    }
                case 0x908:
                    {
                        base.Initializing = true;
                        this.BaseWidth = e.GetFloat(undo);
                        base.Initializing = false;
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override RectangleF ComputeBounds()
        {
            DiagramText text1 = this.Label;
            if (text1 == null)
            {
                return base.ComputeBounds();
            }
            SizeF ef1 = this.TopLeftMargin;
            SizeF ef2 = this.BottomRightMargin;
            return new RectangleF(text1.Left - ef1.Width, text1.Top - ef1.Height, (text1.Width + ef1.Width) + ef2.Width, (text1.Height + ef1.Height) + ef2.Height);
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            Balloon balloon1 = (Balloon)base.CopyObject(env);
            if (balloon1 != null)
            {
                env.Delayeds.Add(this);
            }
            return balloon1;
        }

        public override void CopyObjectDelayed(CopyDictionary env, DiagramShape newobj)
        {
            base.CopyObjectDelayed(env, newobj);
            Balloon balloon1 = (Balloon)newobj;
            balloon1.myAnchor = env[this.myAnchor] as DiagramShape;
            balloon1.LayoutChildren(null);
        }

        protected override DiagramShape CreateBackground()
        {
            PolygonGraph polygon1 = new PolygonGraph();
            polygon1.Shadowed = true;
            polygon1.Selectable = false;
            polygon1.Pen = DiagramGraph.Pens_LightGray;
            polygon1.Brush = DiagramGraph.Brushes_LemonChiffon;
            return polygon1;
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            DiagramShape obj1 = base.Background;
            if (obj1 != null)
            {
                rect = DiagramShape.UnionRect(rect, obj1.Bounds);
                rect = obj1.ExpandPaintBounds(rect, view);
            }
            return rect;
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing)
            {
                DiagramText text1 = this.Label;
                if (text1 != null)
                {
                    PolygonGraph polygon1 = base.Background as PolygonGraph;
                    if ((polygon1 != null) && (childchanged != polygon1))
                    {
                        SizeF ef1 = this.TopLeftMargin;
                        SizeF ef2 = this.BottomRightMargin;
                        RectangleF ef3 = new RectangleF(text1.Left - ef1.Width, text1.Top - ef1.Height, (text1.Width + ef1.Width) + ef2.Width, (text1.Height + ef1.Height) + ef2.Height);
                        SizeF ef4 = this.Corner;
                        float single1 = ef4.Width;
                        if (single1 > (ef3.Width / 2f))
                        {
                            single1 = ef3.Width / 2f;
                        }
                        float single2 = ef4.Height;
                        if (single2 > (ef3.Height / 2f))
                        {
                            single2 = ef3.Height / 2f;
                        }
                        float single3 = ef3.X;
                        float single4 = ef3.Y;
                        float single5 = single3 + single1;
                        float single6 = single4 + single2;
                        float single7 = single3 + (ef3.Width / 2f);
                        float single8 = single4 + (ef3.Height / 2f);
                        float single9 = (single3 + ef3.Width) - single1;
                        float single10 = (single4 + ef3.Height) - single2;
                        float single11 = single3 + ef3.Width;
                        float single12 = single4 + ef3.Height;
                        RectangleF ef5 = polygon1.Bounds;
                        bool flag1 = base.SuspendsUpdates;
                        if (!flag1)
                        {
                            polygon1.Changing(0x584);
                        }
                        polygon1.SuspendsUpdates = true;
                        polygon1.ClearPoints();
                        if (this.Anchor != null)
                        {
                            PointF tf3;
                            PointF tf4;
                            float single13 = System.Math.Min((float)(ef3.Width - single1), this.BaseWidth);
                            float single14 = System.Math.Min((float)(ef3.Height - single2), this.BaseWidth);
                            float single15 = text1.Left;
                            float single16 = text1.Top;
                            float single17 = text1.Right;
                            float single18 = text1.Bottom;
                            PointF tf1 = text1.Center;
                            PointF tf2 = this.Anchor.Center;
                            text1.GetNearestIntersectionPoint(tf2, tf1, out tf3);
                            this.Anchor.GetNearestIntersectionPoint(tf1, tf2, out tf4);
                            if ((tf3.Y <= single16) && (tf3.X < single7))
                            {
                                polygon1.AddPoint(single3, single4);
                                polygon1.AddPoint(tf4);
                                polygon1.AddPoint(single3 + single13, single4);
                            }
                            else
                            {
                                polygon1.AddPoint(single5, single4);
                            }
                            if ((tf3.Y <= single16) && (tf3.X >= single7))
                            {
                                polygon1.AddPoint(single11 - single13, single4);
                                polygon1.AddPoint(tf4);
                                polygon1.AddPoint(single11, single4);
                            }
                            else
                            {
                                polygon1.AddPoint(single9, single4);
                            }
                            if ((tf3.X >= single17) & (tf3.Y < single8))
                            {
                                polygon1.AddPoint(single11, single4);
                                polygon1.AddPoint(tf4);
                                polygon1.AddPoint(single11, single4 + single14);
                            }
                            else
                            {
                                polygon1.AddPoint(single11, single6);
                            }
                            if ((tf3.X >= single17) & (tf3.Y >= single8))
                            {
                                polygon1.AddPoint(single11, single12 - single14);
                                polygon1.AddPoint(tf4);
                                polygon1.AddPoint(single11, single12);
                            }
                            else
                            {
                                polygon1.AddPoint(single11, single10);
                            }
                            if ((tf3.Y >= single18) && (tf3.X >= single7))
                            {
                                polygon1.AddPoint(single11, single12);
                                polygon1.AddPoint(tf4);
                                polygon1.AddPoint(single11 - single13, single12);
                            }
                            else
                            {
                                polygon1.AddPoint(single9, single12);
                            }
                            if ((tf3.Y >= single18) && (tf3.X < single7))
                            {
                                polygon1.AddPoint(single3 + single13, single12);
                                polygon1.AddPoint(tf4);
                                polygon1.AddPoint(single3, single12);
                            }
                            else
                            {
                                polygon1.AddPoint(single5, single12);
                            }
                            if ((tf3.X <= single15) && (tf3.Y >= single8))
                            {
                                polygon1.AddPoint(single3, single12);
                                polygon1.AddPoint(tf4);
                                polygon1.AddPoint(single3, single12 - single14);
                            }
                            else
                            {
                                polygon1.AddPoint(single3, single10);
                            }
                            if ((tf3.X <= single15) && (tf3.Y < single8))
                            {
                                polygon1.AddPoint(single3, single4 + single14);
                                polygon1.AddPoint(tf4);
                                polygon1.AddPoint(single3, single4);
                            }
                            else
                            {
                                polygon1.AddPoint(single3, single6);
                            }
                        }
                        else
                        {
                            polygon1.AddPoint(single5, single4);
                            polygon1.AddPoint(single9, single4);
                            polygon1.AddPoint(single11, single6);
                            polygon1.AddPoint(single11, single10);
                            polygon1.AddPoint(single9, single12);
                            polygon1.AddPoint(single5, single12);
                            polygon1.AddPoint(single3, single10);
                            polygon1.AddPoint(single3, single6);
                        }
                        polygon1.SuspendsUpdates = flag1;
                        if (!flag1)
                        {
                            polygon1.Changed(0x584, 0, null, ef5, 0, null, polygon1.Bounds);
                        }
                    }
                }
            }
        }

        protected override void MoveChildren(RectangleF old)
        {
            base.MoveChildren(old);
            this.LayoutChildren(null);
        }

        protected override void OnLayerChanged(DiagramLayer oldlayer, DiagramLayer newlayer, DiagramShape mainObj)
        {
            base.OnLayerChanged(oldlayer, newlayer, mainObj);
            if (((oldlayer != null) && (newlayer == null)) && (this.Anchor != null))
            {
                this.Anchor.RemoveObserver(this);
            }
        }

        protected override void OnObservedChanged(DiagramShape observed, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            base.OnObservedChanged(observed, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
            if ((subhint == 0x3e9) && (observed == this.Anchor))
            {
                this.LayoutChildren(null);
            }
        }


        [Description("The object that the balloon comment is pointing at")]
        public virtual DiagramShape Anchor
        {
            get
            {
                return this.myAnchor;
            }
            set
            {
                DiagramShape obj1 = this.myAnchor;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        obj1.RemoveObserver(this);
                    }
                    this.myAnchor = value;
                    if (value != null)
                    {
                        value.AddObserver(this);
                    }
                    this.Changed(2310, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.LayoutChildren(null);
                }
            }
        }

        [DefaultValue(30), Description("The width of the base of the balloon's pointer"), Category("Appearance")]
        public virtual float BaseWidth
        {
            get
            {
                return this.myBaseWidth;
            }
            set
            {
                float single1 = this.myBaseWidth;
                if ((single1 != value) && (value > 0f))
                {
                    this.myBaseWidth = value;
                    this.Changed(0x908, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }

        private SizeF Corner
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
                    this.Changed(0x907, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }


        public const int ChangedAnchor = 2310;
        public const int ChangedBaseWidth = 0x908;
        internal const int ChangedCorner = 0x907;
        private DiagramShape myAnchor;
        private float myBaseWidth;
        private SizeF myCorner;
    }
}
