using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class BasicNode : DiagramNode
    {
        static BasicNode()
        {
            BasicNode.DefaultPortSize = new SizeF(7f, 7f);
            BasicNode.DefaultShapeMargin = new SizeF(7f, 7f);
        }

        public BasicNode()
        {
            this.myShape = null;
            this.myLabel = null;
            this.myPort = null;
            this.myLabelSpot = 0x20;
            this.myMiddleLabelMargin = new SizeF(20f, 10f);
            base.InternalFlags |= 0x1020000;
            this.myPort = this.CreatePort();
            this.myShape = this.CreateShape(this.myPort);
            this.Add(this.myShape);
            this.Add(this.myPort);
            if (this.myPort != null)
            {
                this.myPort.PortObject = this.myShape;
            }
            base.PropertiesDelegatedToSelectionObject = true;
            base.Initializing = false;
            this.LayoutChildren(null);
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x835:
                    {
                        this.setLabelSpot(e.GetInt(undo), true);
                        return;
                    }
                case 0x836:
                    {
                        this.Shape = (DiagramGraph)e.GetValue(undo);
                        return;
                    }
                case 0x837:
                    {
                        this.Label = (DiagramText)e.GetValue(undo);
                        return;
                    }
                case 0x838:
                    {
                        this.Port = (DiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0x839:
                    {
                        base.Initializing = true;
                        this.MiddleLabelMargin = e.GetSize(undo);
                        base.Initializing = false;
                        return;
                    }
                case 0x83a:
                    {
                        this.setAutoResizes((bool)e.GetValue(undo), true);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            base.CopyChildren(newgroup, env);
            BasicNode node1 = (BasicNode)newgroup;
            node1.myShape = (DiagramGraph)env[this.myShape];
            node1.myPort = (DiagramPort)env[this.myPort];
            node1.myLabel = (DiagramText)env[this.myLabel];
        }

        protected virtual DiagramText CreateLabel(string name)
        {
            DiagramText text1 = new DiagramText();
            text1.Text = name;
            text1.Selectable = false;
            return text1;
        }

        protected virtual DiagramPort CreatePort()
        {
            DiagramPort port1 = new DiagramPort();
            port1.Style = DiagramPortStyle.Ellipse;
            port1.FromSpot = 0;
            port1.ToSpot = 0;
            port1.Size = BasicNode.DefaultPortSize;
            return port1;
        }

        protected virtual DiagramGraph CreateShape(DiagramPort p)
        {
            DiagramGraph shape1 = new EllipseGraph();
            SizeF ef1 = p.Size;
            shape1.Size = new SizeF(ef1.Width + (2f * BasicNode.DefaultShapeMargin.Width), ef1.Height + (2f * BasicNode.DefaultShapeMargin.Height));
            shape1.Selectable = false;
            shape1.Resizable = false;
            shape1.Reshapable = false;
            shape1.Brush = DiagramGraph.Brushes_White;
            return shape1;
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing)
            {
                DiagramGraph shape1 = this.Shape;
                if (shape1 != null)
                {
                    DiagramText text1 = this.Label;
                    if (text1 != null)
                    {
                        if (this.LabelSpot == 1)
                        {
                            PointF tf1 = shape1.Center;
                            SizeF ef1 = this.MiddleLabelMargin;
                            if (this.AutoResizes)
                            {
                                float single1 = text1.Width + ef1.Width;
                                float single2 = text1.Height + ef1.Height;
                                shape1.Bounds = new RectangleF(tf1.X - (single1 / 2f), tf1.Y - (single2 / 2f), single1, single2);
                            }
                            else
                            {
                                float single3 = System.Math.Max((float)(shape1.Width - (ef1.Width + ef1.Width)), (float)0f);
                                float single4 = System.Math.Max((float)(shape1.Height - (ef1.Height + ef1.Height)), (float)0f);
                                text1.Width = single3;
                                text1.WrappingWidth = single3;
                                text1.UpdateSize();
                                float single5 = System.Math.Min(text1.Height, single4);
                                float single6 = shape1.Left + ef1.Width;
                                float single7 = (shape1.Top + ef1.Height) + ((single4 - single5) / 2f);
                                text1.Bounds = new RectangleF(single6, single7, single3, single5);
                            }
                            text1.Alignment = 1;
                            text1.Center = tf1;
                            if (this.Port != null)
                            {
                                this.Port.Bounds = shape1.Bounds;
                            }
                        }
                        else
                        {
                            text1.Alignment = this.SpotOpposite(this.LabelSpot);
                            text1.SetSpotLocation(this.SpotOpposite(this.LabelSpot), shape1, this.LabelSpot);
                        }
                    }
                    if (this.Port != null)
                    {
                        this.Port.SetSpotLocation(1, shape1, 1);
                    }
                }
            }
        }

        public virtual void OnAutoResizesChanged(bool old)
        {
            DiagramText text1 = this.Label;
            if (text1 != null)
            {
                text1.Wrapping = old;
                text1.Clipping = old;
            }
        }

        public virtual void OnLabelSpotChanged(int old)
        {
            if (this.Port != null)
            {
                if (this.LabelSpot == 1)
                {
                    this.Port.Style = DiagramPortStyle.None;
                    this.Resizable = false;
                }
                else if (old == 1)
                {
                    this.Port.Style = DiagramPortStyle.Ellipse;
                    RectangleF ef1 = new RectangleF(this.Shape.Center.X - (BasicNode.DefaultPortSize.Width / 2f), this.Shape.Center.Y - (BasicNode.DefaultPortSize.Height / 2f), BasicNode.DefaultPortSize.Width, BasicNode.DefaultPortSize.Height);
                    RectangleF ef2 = new RectangleF((this.Shape.Center.X - (ef1.Width / 2f)) - BasicNode.DefaultShapeMargin.Width, (this.Shape.Center.Y - (ef1.Height / 2f)) - BasicNode.DefaultShapeMargin.Height, ef1.Width + (2f * BasicNode.DefaultShapeMargin.Width), ef1.Height + (2f * BasicNode.DefaultShapeMargin.Height));
                    this.Shape.Bounds = ef2;
                    this.Port.Bounds = ef1;
                }
            }
            this.LayoutChildren(this.Label);
        }

        public override void Remove(DiagramShape obj)
        {
            base.Remove(obj);
            if (obj == this.myShape)
            {
                this.myShape = null;
            }
            else if (obj == this.myLabel)
            {
                this.myLabel = null;
            }
            else if (obj == this.myPort)
            {
                this.myPort = null;
            }
        }

        private void setAutoResizes(bool b, bool undoing)
        {
            bool flag1 = (base.InternalFlags & 0x1000000) != 0;
            if (flag1 != b)
            {
                if (b)
                {
                    base.InternalFlags |= 0x1000000;
                }
                else
                {
                    base.InternalFlags &= -16777217;
                }
                this.Changed(0x83a, 0, flag1, DiagramShape.NullRect, 0, b, DiagramShape.NullRect);
                if (!undoing)
                {
                    this.OnAutoResizesChanged(flag1);
                }
            }
        }

        private void setLabelSpot(int spot, bool undoing)
        {
            int num1 = this.myLabelSpot;
            if (num1 != spot)
            {
                this.myLabelSpot = spot;
                this.Changed(0x835, num1, null, DiagramShape.NullRect, spot, null, DiagramShape.NullRect);
                if (!undoing)
                {
                    this.OnLabelSpotChanged(num1);
                }
            }
        }


        [DefaultValue(true), Category("Behavior"), Description("Whether the background changes size as the text changes")]
        public virtual bool AutoResizes
        {
            get
            {
                return ((base.InternalFlags & 0x1000000) != 0);
            }
            set
            {
                this.setAutoResizes(value, false);
            }
        }

        [Category("Appearance"), Description("The Brush used by the shape")]
        public System.Drawing.Brush Brush
        {
            get
            {
                return this.Shape.Brush;
            }
            set
            {
                this.Shape.Brush = value;
            }
        }

        public override DiagramText Label
        {
            get
            {
                return this.myLabel;
            }
            set
            {
                DiagramText text1 = this.myLabel;
                if (text1 != value)
                {
                    if (text1 != null)
                    {
                        this.Remove(text1);
                    }
                    this.myLabel = value;
                    if (value != null)
                    {
                        this.Add(value);
                    }
                    this.Changed(0x837, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The spot at which any label is positioned relative to the shape"), Category("Appearance"), DefaultValue(0x20)]
        public virtual int LabelSpot
        {
            get
            {
                return this.myLabelSpot;
            }
            set
            {
                this.setLabelSpot(value, false);
            }
        }

        [Description("The margin of the shape around the label, when the LabelSpot is Middle"), TypeConverter(typeof(SizeFConverter)), Category("Appearance")]
        public virtual SizeF MiddleLabelMargin
        {
            get
            {
                return this.myMiddleLabelMargin;
            }
            set
            {
                SizeF ef1 = this.myMiddleLabelMargin;
                if (ef1 != value)
                {
                    this.myMiddleLabelMargin = value;
                    this.Changed(0x839, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }

        [Category("Appearance"), Description("The Pen used by the shape")]
        public System.Drawing.Pen Pen
        {
            get
            {
                return this.Shape.Pen;
            }
            set
            {
                this.Shape.Pen = value;
            }
        }

        public virtual DiagramPort Port
        {
            get
            {
                return this.myPort;
            }
            set
            {
                DiagramPort port1 = this.myPort;
                if (port1 != value)
                {
                    if (port1 != null)
                    {
                        this.Remove(port1);
                    }
                    this.myPort = value;
                    if (value != null)
                    {
                        this.Add(value);
                    }
                    this.Changed(0x838, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    if ((value != null) && (value.PortObject == null))
                    {
                        value.PortObject = this.Shape;
                    }
                }
            }
        }

        public override DiagramShape SelectionObject
        {
            get
            {
                if (this.Shape != null)
                {
                    return this.Shape;
                }
                return this;
            }
        }

        public virtual DiagramGraph Shape
        {
            get
            {
                return this.myShape;
            }
            set
            {
                DiagramGraph shape1 = this.myShape;
                if (shape1 != value)
                {
                    base.CopyPropertiesFromSelectionObject(shape1, value);
                    if (shape1 != null)
                    {
                        this.Remove(shape1);
                    }
                    this.myShape = value;
                    if (value != null)
                    {
                        this.InsertBefore(null, value);
                    }
                    this.Changed(0x836, 0, shape1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    if ((this.Port != null) && (this.Port.PortObject == shape1))
                    {
                        this.Port.PortObject = value;
                    }
                }
            }
        }

        public override string Text
        {
            get
            {
                if (this.Label != null)
                {
                    return this.Label.Text;
                }
                return "";
            }
            set
            {
                if (value == null)
                {
                    this.Remove(this.myLabel);
                }
                else if (this.Label == null)
                {
                    this.myLabel = this.CreateLabel(value);
                    this.Add(this.myLabel);
                }
                else
                {
                    this.Label.Text = value;
                }
            }
        }


        public const int ChangedAutoResizes = 0x83a;
        public const int ChangedLabel = 0x837;
        public const int ChangedLabelSpot = 0x835;
        public const int ChangedMiddleLabelMargin = 0x839;
        public const int ChangedPort = 0x838;
        public const int ChangedShape = 0x836;
        private static readonly SizeF DefaultPortSize;
        private static readonly SizeF DefaultShapeMargin;
        private const int flagAutoResizes = 0x1000000;
        private DiagramText myLabel;
        private int myLabelSpot;
        private SizeF myMiddleLabelMargin;
        private DiagramPort myPort;
        private DiagramGraph myShape;
    }
}
