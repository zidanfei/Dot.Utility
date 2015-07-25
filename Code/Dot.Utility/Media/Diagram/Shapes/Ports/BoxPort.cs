using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class BoxPort : DiagramPort
    {
        static BoxPort()
        {
            BoxPort.myComparer = new EndPositionComparer();
        }

        public BoxPort()
        {
            this.myLinkPointsSpread = false;
            this.mySortedLinks = null;
            this.myRespreading = false;
            this.Style = DiagramPortStyle.Rectangle;
            this.Pen = null;
            this.Brush = DiagramGraph.Brushes_Gray;
            this.FromSpot = 1;
            this.ToSpot = 1;
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            if (e.SubHint == 0x8a3)
            {
                this.LinkPointsSpread = (bool)e.GetValue(undo);
            }
            else
            {
                base.ChangeValue(e, undo);
            }
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            BoxPort port1 = (BoxPort)base.CopyObject(env);
            if (port1 != null)
            {
                port1.mySortedLinks = null;
                port1.myRespreading = false;
            }
            return port1;
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            rect = base.ExpandPaintBounds(rect, view);
            if (((this.Style != DiagramPortStyle.None) && (base.Parent != null)) && base.Parent.Shadowed)
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

        public virtual float GetAngle(IDiagramLine link)
        {
            if (link == null)
            {
                return 0f;
            }
            IDiagramPort port1 = link.GetOtherPort(this);
            if (port1 == null)
            {
                if (((link.FromPort != null) && (link.FromPort.DiagramShape != null)) && (link.FromPort.DiagramShape.Bounds == this.Bounds))
                {
                    port1 = link.ToPort;
                }
                else if (((link.ToPort != null) && (link.ToPort.DiagramShape != null)) && (link.ToPort.DiagramShape.Bounds == this.Bounds))
                {
                    port1 = link.FromPort;
                }
            }
            if (port1 == null)
            {
                return 0f;
            }
            DiagramShape obj1 = port1.DiagramShape;
            if (obj1 == null)
            {
                return 0f;
            }
            PointF tf1 = obj1.Center;
            PointF tf2 = base.Center;
            Shapes.LineGraph link1 = link as Shapes.LineGraph;
            if ((link1 == null) && (link is TextLine))
            {
                link1 = ((TextLine)link).RealLink;
            }
            if ((link1 != null) && (link1.PointsCount > 0))
            {
                if (link1.FromPort == port1)
                {
                    tf1 = link1.GetPoint(0);
                }
                else
                {
                    tf1 = link1.GetPoint(link1.PointsCount - 1);
                }
            }
            return StrokeGraph.GetAngle(tf1.X - tf2.X, tf1.Y - tf2.Y);
        }

        public virtual float GetDirection(IDiagramLine link)
        {
            if (link == null)
            {
                return 0f;
            }
            if (link.FromPort == this)
            {
                return this.GetFromLinkDir(link);
            }
            return this.GetToLinkDir(link);
        }

        public override float GetFromLinkDir(IDiagramLine link)
        {
            if ((this.FromSpot != 0) && (this.FromSpot != 1))
            {
                return this.GetLinkDir(this.FromSpot);
            }
            float single1 = this.GetAngle(link);
            if (this.IsOrthogonal(link))
            {
                if ((single1 >= 60f) && (single1 < 150f))
                {
                    return 90f;
                }
                if ((single1 >= 150f) && (single1 < 240f))
                {
                    return 180f;
                }
                if ((single1 >= 240f) && (single1 < 330f))
                {
                    return 270f;
                }
                return 0f;
            }
            if ((single1 > 45f) && (single1 < 135f))
            {
                return 90f;
            }
            if ((single1 >= 135f) && (single1 <= 225f))
            {
                return 180f;
            }
            if ((single1 > 225f) && (single1 < 315f))
            {
                return 270f;
            }
            return 0f;
        }

        public override PointF GetFromLinkPoint(IDiagramLine link)
        {
            DiagramShape obj1 = this.PortObject;
            if ((obj1 == null) || (obj1.Layer == null))
            {
                obj1 = this;
            }
            if ((this.FromSpot != 0) && (this.FromSpot != 1))
            {
                return obj1.GetSpotLocation(this.FromSpot);
            }
            if ((link == null) || (link.DiagramShape == null))
            {
                return obj1.Center;
            }
            if (this.LinkPointsSpread)
            {
                LinkInfo[] infoArray1 = this.SortLinks();
                int num1 = infoArray1.Length;
                for (int num2 = 0; num2 < num1; num2++)
                {
                    LinkInfo info1 = infoArray1[num2];
                    if (info1.Link == link)
                    {
                        return info1.LinkPoint;
                    }
                }
            }
            float single1 = this.GetAngle(link);
            int num3 = 1;
            if (this.IsOrthogonal(link))
            {
                if ((single1 >= 60f) && (single1 < 150f))
                {
                    num3 = 0x80;
                }
                else if ((single1 >= 150f) && (single1 < 240f))
                {
                    num3 = 0x100;
                }
                else if ((single1 >= 240f) && (single1 < 330f))
                {
                    num3 = 0x20;
                }
                else
                {
                    num3 = 0x40;
                }
            }
            else if ((single1 > 45f) && (single1 < 135f))
            {
                num3 = 0x80;
            }
            else if ((single1 >= 135f) && (single1 <= 225f))
            {
                num3 = 0x100;
            }
            else if ((single1 > 225f) && (single1 < 315f))
            {
                num3 = 0x20;
            }
            else
            {
                num3 = 0x40;
            }
            return obj1.GetSpotLocation(num3);
        }

        internal PointF GetSideLinkPoint(LinkInfo info)
        {
            PointF tf1;
            PointF tf2;
            float single1;
            DiagramShape obj1 = this.PortObject;
            if ((obj1 == null) || (obj1.Layer == null))
            {
                obj1 = this;
            }
            int num1 = info.Side;
            if (num1 <= 0x40)
            {
                if (num1 == 0x20)
                {
                    tf1 = obj1.GetSpotLocation(2);
                    tf2 = obj1.GetSpotLocation(4);
                    goto Label_008C;
                }
                if (num1 == 0x40)
                {
                }
            }
            else if (num1 != 0x80)
            {
                if (num1 == 0x100)
                {
                    tf1 = obj1.GetSpotLocation(0x10);
                    tf2 = obj1.GetSpotLocation(2);
                    goto Label_008C;
                }
            }
            else
            {
                tf1 = obj1.GetSpotLocation(8);
                tf2 = obj1.GetSpotLocation(0x10);
                goto Label_008C;
            }
            tf1 = obj1.GetSpotLocation(4);
            tf2 = obj1.GetSpotLocation(8);
        Label_008C:
            single1 = tf2.X - tf1.X;
            float single2 = tf2.Y - tf1.Y;
            float single3 = (info.IndexOnSide + 1f) / (info.NumOnSide + 1f);
            return new PointF(tf1.X + (single1 * single3), tf1.Y + (single2 * single3));
        }

        public override float GetToLinkDir(IDiagramLine link)
        {
            if ((this.ToSpot != 0) && (this.ToSpot != 1))
            {
                return this.GetLinkDir(this.ToSpot);
            }
            float single1 = this.GetAngle(link);
            if (this.IsOrthogonal(link))
            {
                if ((single1 >= 30f) && (single1 < 120f))
                {
                    return 90f;
                }
                if ((single1 >= 120f) && (single1 < 210f))
                {
                    return 180f;
                }
                if ((single1 >= 210f) && (single1 < 300f))
                {
                    return 270f;
                }
                return 0f;
            }
            if ((single1 > 45f) && (single1 < 135f))
            {
                return 90f;
            }
            if ((single1 >= 135f) && (single1 <= 225f))
            {
                return 180f;
            }
            if ((single1 > 225f) && (single1 < 315f))
            {
                return 270f;
            }
            return 0f;
        }

        public override PointF GetToLinkPoint(IDiagramLine link)
        {
            DiagramShape obj1 = this.PortObject;
            if ((obj1 == null) || (obj1.Layer == null))
            {
                obj1 = this;
            }
            if ((this.ToSpot != 0) && (this.ToSpot != 1))
            {
                return obj1.GetSpotLocation(this.ToSpot);
            }
            if ((link == null) || (link.DiagramShape == null))
            {
                return obj1.Center;
            }
            if (this.LinkPointsSpread)
            {
                LinkInfo[] infoArray1 = this.SortLinks();
                int num1 = infoArray1.Length;
                for (int num2 = 0; num2 < num1; num2++)
                {
                    LinkInfo info1 = infoArray1[num2];
                    if (info1.Link == link)
                    {
                        return info1.LinkPoint;
                    }
                }
            }
            float single1 = this.GetAngle(link);
            int num3 = 1;
            if (this.IsOrthogonal(link))
            {
                if ((single1 >= 30f) && (single1 < 120f))
                {
                    num3 = 0x80;
                }
                else if ((single1 >= 120f) && (single1 < 210f))
                {
                    num3 = 0x100;
                }
                else if ((single1 >= 210f) && (single1 < 300f))
                {
                    num3 = 0x20;
                }
                else
                {
                    num3 = 0x40;
                }
            }
            else if ((single1 > 45f) && (single1 < 135f))
            {
                num3 = 0x80;
            }
            else if ((single1 >= 135f) && (single1 <= 225f))
            {
                num3 = 0x100;
            }
            else if ((single1 > 225f) && (single1 < 315f))
            {
                num3 = 0x20;
            }
            else
            {
                num3 = 0x40;
            }
            return obj1.GetSpotLocation(num3);
        }

        public virtual bool IsOrthogonal(IDiagramLine link)
        {
            if (link != null)
            {
                Shapes.LineGraph link1 = link as Shapes.LineGraph;
                if (link1 != null)
                {
                    return link1.Orthogonal;
                }
                TextLine link2 = link as TextLine;
                if (link2 != null)
                {
                    return link2.Orthogonal;
                }
            }
            return false;
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            if (((this.Style != DiagramPortStyle.None) && (base.Parent != null)) && base.Parent.Shadowed)
            {
                RectangleF ef1 = this.Bounds;
                SizeF ef2 = base.Parent.GetShadowOffset(view);
                if (this.Brush != null)
                {
                    Brush brush1 = base.Parent.GetShadowBrush(view);
                    DiagramGraph.DrawRectangle(g, view, null, brush1, ef1.X + ef2.Width, ef1.Y + ef2.Height, ef1.Width, ef1.Height);
                }
                else if (this.Pen != null)
                {
                    Pen pen1 = base.Parent.GetShadowPen(view, DiagramGraph.GetPenWidth(this.Pen));
                    DiagramGraph.DrawRectangle(g, view, pen1, null, ef1.X + ef2.Width, ef1.Y + ef2.Height, ef1.Width, ef1.Height);
                }
            }
            base.Paint(g, view);
        }

        internal LinkInfo[] SortLinks()
        {
            if ((this.mySortedLinks == null) || (this.mySortedLinks.Length != this.LinksCount))
            {
                this.mySortedLinks = new LinkInfo[this.LinksCount];
            }
            if (!this.myRespreading)
            {
                bool flag1 = this.myRespreading;
                this.myRespreading = true;
                int num1 = 0;
                PortLinkEnumerator enumerator1 = this.Links.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    int num2;
                    IDiagramLine link1 = enumerator1.Current;
                    float single1 = this.GetDirection(link1);
                    float single2 = this.GetAngle(link1);
                    if (single1 == 0f)
                    {
                        num2 = 0x40;
                        if (single2 > 180f)
                        {
                            single2 -= 360f;
                        }
                    }
                    else if (single1 == 90f)
                    {
                        num2 = 0x80;
                    }
                    else if (single1 == 180f)
                    {
                        num2 = 0x100;
                    }
                    else
                    {
                        num2 = 0x20;
                    }
                    this.mySortedLinks[num1++] = new LinkInfo(link1, single2, num2, single1);
                }
                Array.Sort(this.mySortedLinks, 0, this.mySortedLinks.Length, BoxPort.myComparer);
                int num3 = this.mySortedLinks.Length;
                int num4 = -1;
                int num5 = 0;
                num1 = 0;
                while (num1 < num3)
                {
                    LinkInfo info1 = this.mySortedLinks[num1];
                    if (info1.Side != num4)
                    {
                        num4 = info1.Side;
                        num5 = 0;
                    }
                    info1.IndexOnSide = num5;
                    num5++;
                    num1++;
                }
                num4 = -1;
                num5 = 0;
                for (num1 = num3 - 1; num1 >= 0; num1--)
                {
                    LinkInfo info2 = this.mySortedLinks[num1];
                    if (info2.Side != num4)
                    {
                        num4 = info2.Side;
                        num5 = info2.IndexOnSide + 1;
                    }
                    info2.NumOnSide = num5;
                    info2.LinkPoint = this.GetSideLinkPoint(info2);
                }
                this.myRespreading = flag1;
            }
            return this.mySortedLinks;
        }


        [Description("Whether the link points are distributed evenly along each side"), Category("Appearance"), DefaultValue(false)]
        public virtual bool LinkPointsSpread
        {
            get
            {
                return this.myLinkPointsSpread;
            }
            set
            {
                bool flag1 = this.myLinkPointsSpread;
                if (flag1 != value)
                {
                    this.myLinkPointsSpread = value;
                    this.Changed(0x8a3, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.LinksOnPortChanged(0x8a3, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedLinkPointsSpread = 0x8a3;
        private static IComparer myComparer;
        private bool myLinkPointsSpread;
        [NonSerialized]
        private bool myRespreading;
        [NonSerialized]
        private LinkInfo[] mySortedLinks;

        [Serializable]
        internal sealed class EndPositionComparer : IComparer
        {
            internal EndPositionComparer()
            {
            }

            public int Compare(object x, object y)
            {
                BoxPort.LinkInfo info1 = x as BoxPort.LinkInfo;
                BoxPort.LinkInfo info2 = y as BoxPort.LinkInfo;
                if ((info1 != null) && (info2 != null))
                {
                    if (info1.Side < info2.Side)
                    {
                        return -1;
                    }
                    if (info1.Side > info2.Side)
                    {
                        return 1;
                    }
                    if (info1.Angle < info2.Angle)
                    {
                        return -1;
                    }
                    if (info1.Angle > info2.Angle)
                    {
                        return 1;
                    }
                }
                return 0;
            }

        }

        [Serializable]
        internal sealed class LinkInfo
        {
            internal LinkInfo(IDiagramLine l, float a, int s, float d)
            {
                this.Link = l;
                this.Angle = a;
                this.Side = s;
                this.Direction = d;
            }


            internal float Angle;
            internal float Direction;
            internal int IndexOnSide;
            internal IDiagramLine Link;
            internal PointF LinkPoint;
            internal int NumOnSide;
            internal int Side;
        }
    }
}
