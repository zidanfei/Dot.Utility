using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class GeneralNodePort : DiagramPort
    {
        public GeneralNodePort()
        {
            this.myLeftSide = true;
            this.mySideIndex = -1;
            this.myName = "";
            this.myPortLabel = null;
            this.Style = DiagramPortStyle.Triangle;
            this.Pen = DiagramGraph.Pens_Gray;
            this.Brush = DiagramGraph.Brushes_LightGray;
            base.Size = new SizeF(8f, 8f);
            this.LeftSide = true;
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 2430:
                    {
                        this.Name = (string)e.GetValue(undo);
                        return;
                    }
                case 0x97f:
                    {
                        this.Label = (GeneralNodePortLabelText)e.GetValue(undo);
                        return;
                    }
                case 0x980:
                    {
                        this.SideIndex = e.GetInt(undo);
                        return;
                    }
                case 0x981:
                    {
                        this.LeftSide = (bool)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            GeneralNodePort port1 = (GeneralNodePort)base.CopyObject(env);
            if ((port1 != null) && (this.myPortLabel != null))
            {
                port1.myPortLabel = (GeneralNodePortLabelText)env.Copy(this.myPortLabel);
                if (port1.myPortLabel != null)
                {
                    port1.myPortLabel.Port = port1;
                }
            }
            return port1;
        }

        public override PointF GetFromLinkPoint(IDiagramLine link)
        {
            return this.GetLinkPoint(this.FromSpot);
        }

        public virtual PointF GetLinkPoint(int spot)
        {
            int num1 = spot;
            if (num1 <= 0x40)
            {
                if (num1 == 0x20)
                {
                    RectangleF ef3 = this.Bounds;
                    PointF tf3 = new PointF(ef3.X + (ef3.Width / 2f), ef3.Y);
                    GeneralNodePortLabelText label3 = this.Label;
                    if ((label3 != null) && label3.Visible)
                    {
                        tf3.Y -= (label3.Height + this.LabelSpacing);
                    }
                    return tf3;
                }
                if (num1 == 0x40)
                {
                    RectangleF ef2 = this.Bounds;
                    PointF tf2 = new PointF(ef2.X + ef2.Width, ef2.Y + (ef2.Height / 2f));
                    GeneralNodePortLabelText label2 = this.Label;
                    if ((label2 != null) && label2.Visible)
                    {
                        tf2.X += (label2.Width + this.LabelSpacing);
                    }
                    return tf2;
                }
            }
            else
            {
                if (num1 == 0x80)
                {
                    RectangleF ef4 = this.Bounds;
                    PointF tf4 = new PointF(ef4.X + (ef4.Width / 2f), ef4.Y + ef4.Height);
                    GeneralNodePortLabelText label4 = this.Label;
                    if ((label4 != null) && label4.Visible)
                    {
                        tf4.Y += (label4.Height + this.LabelSpacing);
                    }
                    return tf4;
                }
                if (num1 == 0x100)
                {
                    RectangleF ef1 = this.Bounds;
                    PointF tf1 = new PointF(ef1.X, ef1.Y + (ef1.Height / 2f));
                    GeneralNodePortLabelText label1 = this.Label;
                    if ((label1 != null) && label1.Visible)
                    {
                        tf1.X -= (label1.Width + this.LabelSpacing);
                    }
                    return tf1;
                }
            }
            return this.GetSpotLocation(spot);
        }

        public override PointF GetToLinkPoint(IDiagramLine link)
        {
            return this.GetLinkPoint(this.ToSpot);
        }

        public override string GetToolTip(DiagramView view)
        {
            return this.Name;
        }

        public virtual void LayoutLabel()
        {
            DiagramText text1 = this.Label;
            if (text1 != null)
            {
                GeneralNode node1 = base.Parent as GeneralNode;
                if ((node1 != null) && (node1.Orientation == Orientation.Vertical))
                {
                    if (this.LeftSide)
                    {
                        text1.Alignment = 1;
                        PointF tf1 = this.GetSpotLocation(0x20);
                        tf1.Y -= this.LabelSpacing;
                        text1.SetSpotLocation(0x80, tf1);
                    }
                    else
                    {
                        text1.Alignment = 1;
                        PointF tf2 = this.GetSpotLocation(0x80);
                        tf2.Y += this.LabelSpacing;
                        text1.SetSpotLocation(0x20, tf2);
                    }
                }
                else if (this.LeftSide)
                {
                    text1.Alignment = 0x40;
                    PointF tf3 = this.GetSpotLocation(0x100);
                    tf3.X -= this.LabelSpacing;
                    text1.SetSpotLocation(0x40, tf3);
                }
                else
                {
                    text1.Alignment = 0x100;
                    PointF tf4 = this.GetSpotLocation(0x40);
                    tf4.X += this.LabelSpacing;
                    text1.SetSpotLocation(0x100, tf4);
                }
            }
        }


        public virtual GeneralNodePortLabelText Label
        {
            get
            {
                return this.myPortLabel;
            }
            set
            {
                GeneralNodePortLabelText label1 = this.myPortLabel;
                if (label1 != value)
                {
                    if (label1 != null)
                    {
                        label1.Port = null;
                        if (label1.Parent != null)
                        {
                            label1.Parent.Remove(label1);
                        }
                    }
                    this.myPortLabel = value;
                    if (value != null)
                    {
                        value.Port = this;
                        if (this.LeftSide)
                        {
                            value.Alignment = 0x40;
                        }
                        else
                        {
                            value.Alignment = 0x100;
                        }
                        if (base.Parent != null)
                        {
                            base.Parent.Add(value);
                        }
                    }
                    this.LayoutLabel();
                    this.Changed(0x97f, 0, label1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public virtual float LabelSpacing
        {
            get
            {
                return 2f;
            }
        }

        public bool LeftSide
        {
            get
            {
                return this.myLeftSide;
            }
            set
            {
                bool flag1 = this.myLeftSide;
                if (flag1 != value)
                {
                    this.myLeftSide = value;
                    this.IsValidFrom = !value;
                    this.IsValidTo = value;
                    this.Changed(0x981, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public string Name
        {
            get
            {
                return this.myName;
            }
            set
            {
                string text1 = this.myName;
                if (text1 != value)
                {
                    this.myName = value;
                    this.Changed(2430, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    if (this.Label != null)
                    {
                        this.Label.Text = value;
                    }
                    this.LinksOnPortChanged(2430, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public virtual float PortAndLabelHeight
        {
            get
            {
                if (!this.Visible)
                {
                    return 0f;
                }
                GeneralNodePortLabelText label1 = this.Label;
                GeneralNode node1 = base.Parent as GeneralNode;
                if ((node1 != null) && (node1.Orientation == Orientation.Vertical))
                {
                    if ((label1 != null) && label1.Visible)
                    {
                        return ((base.Height + this.LabelSpacing) + label1.Height);
                    }
                    return base.Height;
                }
                if ((label1 != null) && label1.Visible)
                {
                    return System.Math.Max(base.Height, label1.Height);
                }
                return base.Height;
            }
        }

        public virtual float PortAndLabelWidth
        {
            get
            {
                if (!this.Visible)
                {
                    return 0f;
                }
                GeneralNodePortLabelText label1 = this.Label;
                GeneralNode node1 = base.Parent as GeneralNode;
                if ((node1 != null) && (node1.Orientation == Orientation.Vertical))
                {
                    if ((label1 != null) && label1.Visible)
                    {
                        return System.Math.Max(base.Width, label1.Width);
                    }
                    return base.Width;
                }
                if ((label1 != null) && label1.Visible)
                {
                    return ((base.Width + this.LabelSpacing) + label1.Width);
                }
                return base.Width;
            }
        }

        public int SideIndex
        {
            get
            {
                return this.mySideIndex;
            }
            set
            {
                int num1 = this.mySideIndex;
                if (num1 != value)
                {
                    this.mySideIndex = value;
                    this.Changed(0x980, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedLabel = 0x97f;
        public const int ChangedLeftSide = 0x981;
        public const int ChangedName = 2430;
        public const int ChangedSideIndex = 0x980;
        private bool myLeftSide;
        private string myName;
        private GeneralNodePortLabelText myPortLabel;
        private int mySideIndex;
    }
}
