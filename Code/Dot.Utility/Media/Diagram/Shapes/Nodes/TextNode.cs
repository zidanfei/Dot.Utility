using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class TextNode : DiagramNode
    {
        public TextNode()
        {
            this.myLabel = null;
            this.myBack = null;
            this.myTopPort = null;
            this.myRightPort = null;
            this.myBottomPort = null;
            this.myLeftPort = null;
            this.myTopLeftMargin = new SizeF(4f, 2f);
            this.myBottomRightMargin = new SizeF(4f, 2f);
            base.InternalFlags &= -17;
            base.InternalFlags |= 0x1020000;
            this.myBack = this.CreateBackground();
            this.Add(this.myBack);
            this.myLabel = this.CreateLabel();
            this.Add(this.myLabel);
            this.myTopPort = this.CreatePort(0x20);
            this.Add(this.myTopPort);
            this.myRightPort = this.CreatePort(0x40);
            this.Add(this.myRightPort);
            this.myBottomPort = this.CreatePort(0x80);
            this.Add(this.myBottomPort);
            this.myLeftPort = this.CreatePort(0x100);
            this.Add(this.myLeftPort);
            base.Initializing = false;
            this.LayoutChildren(null);
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0xaf1:
                    {
                        this.Label = (DiagramText)e.GetValue(undo);
                        return;
                    }
                case 0xaf2:
                    {
                        this.Background = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0xaf3:
                    {
                        this.TopPort = (DiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0xaf4:
                    {
                        this.RightPort = (DiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0xaf5:
                    {
                        this.BottomPort = (DiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0xaf6:
                    {
                        this.LeftPort = (DiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0xaf7:
                    {
                        base.Initializing = true;
                        this.TopLeftMargin = e.GetSize(undo);
                        base.Initializing = false;
                        return;
                    }
                case 0xaf8:
                    {
                        base.Initializing = true;
                        this.BottomRightMargin = e.GetSize(undo);
                        base.Initializing = false;
                        return;
                    }
                case 0xaf9:
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
            TextNode node1 = (TextNode)newgroup;
            node1.myBack = (DiagramShape)env[this.myBack];
            node1.myLabel = (DiagramText)env[this.myLabel];
            node1.myTopPort = (DiagramPort)env[this.myTopPort];
            node1.myRightPort = (DiagramPort)env[this.myRightPort];
            node1.myBottomPort = (DiagramPort)env[this.myBottomPort];
            node1.myLeftPort = (DiagramPort)env[this.myLeftPort];
        }

        protected virtual DiagramShape CreateBackground()
        {
            RectangleGraph rectangle1 = new RectangleGraph();
            rectangle1.Selectable = false;
            rectangle1.Resizable = false;
            rectangle1.Reshapable = false;
            rectangle1.Brush = DiagramGraph.Brushes_LightGray;
            return rectangle1;
        }

        protected virtual DiagramText CreateLabel()
        {
            DiagramText text1 = new DiagramText();
            text1.Selectable = false;
            text1.Multiline = true;
            return text1;
        }

        protected virtual DiagramPort CreatePort(int spot)
        {
            DiagramPort port1 = new DiagramPort();
            port1.Style = DiagramPortStyle.None;
            port1.Size = new SizeF(4f, 4f);
            port1.FromSpot = spot;
            port1.ToSpot = spot;
            return port1;
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing)
            {
                DiagramText text1 = this.Label;
                if (text1 != null)
                {
                    DiagramShape obj1 = this.Background;
                    if (obj1 != null)
                    {
                        SizeF ef1 = this.TopLeftMargin;
                        SizeF ef2 = this.BottomRightMargin;
                        if (this.AutoResizes)
                        {
                            obj1.Bounds = new RectangleF(text1.Left - ef1.Width, text1.Top - ef1.Height, (text1.Width + ef1.Width) + ef2.Width, (text1.Height + ef1.Height) + ef2.Height);
                        }
                        else
                        {
                            float single1 = System.Math.Max((float)(obj1.Width - (ef1.Width + ef2.Width)), (float)0f);
                            float single2 = System.Math.Max((float)(obj1.Height - (ef1.Height + ef2.Height)), (float)0f);
                            text1.Width = single1;
                            text1.WrappingWidth = single1;
                            text1.UpdateSize();
                            float single3 = System.Math.Min(text1.Height, single2);
                            float single4 = obj1.Left + ef1.Width;
                            float single5 = (obj1.Top + ef1.Height) + ((single2 - single3) / 2f);
                            text1.Bounds = new RectangleF(single4, single5, single1, single3);
                        }
                    }
                    if ((obj1 == null) && this.AutoResizes)
                    {
                        obj1 = text1;
                    }
                    if (obj1 != null)
                    {
                        if (this.TopPort != null)
                        {
                            this.TopPort.SetSpotLocation(0x20, obj1, 0x20);
                        }
                        if (this.RightPort != null)
                        {
                            this.RightPort.SetSpotLocation(0x40, obj1, 0x40);
                        }
                        if (this.BottomPort != null)
                        {
                            this.BottomPort.SetSpotLocation(0x80, obj1, 0x80);
                        }
                        if (this.LeftPort != null)
                        {
                            this.LeftPort.SetSpotLocation(0x100, obj1, 0x100);
                        }
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
                base.PropertiesDelegatedToSelectionObject = old;
            }
        }

        public override void Remove(DiagramShape obj)
        {
            base.Remove(obj);
            if (obj == this.myBack)
            {
                this.myBack = null;
            }
            else if (obj == this.myLabel)
            {
                this.myLabel = null;
            }
            else if (obj == this.myTopPort)
            {
                this.myTopPort = null;
            }
            else if (obj == this.myRightPort)
            {
                this.myRightPort = null;
            }
            else if (obj == this.myBottomPort)
            {
                this.myBottomPort = null;
            }
            else if (obj == this.myLeftPort)
            {
                this.myLeftPort = null;
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
                this.Changed(0xaf9, 0, flag1, DiagramShape.NullRect, 0, b, DiagramShape.NullRect);
                if (!undoing)
                {
                    this.OnAutoResizesChanged(flag1);
                }
            }
        }


        [Category("Behavior"), DefaultValue(true), Description("Whether the background changes size as the text changes")]
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

        public virtual DiagramShape Background
        {
            get
            {
                return this.myBack;
            }
            set
            {
                DiagramShape obj1 = this.myBack;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        if (value != null)
                        {
                            value.Selectable = obj1.Selectable;
                            value.Shadowed = obj1.Shadowed;
                        }
                        this.Remove(obj1);
                    }
                    this.myBack = value;
                    if (value != null)
                    {
                        this.InsertBefore(null, value);
                    }
                    this.Changed(0xaf2, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public virtual DiagramPort BottomPort
        {
            get
            {
                return this.myBottomPort;
            }
            set
            {
                DiagramPort port1 = this.myBottomPort;
                if (port1 != value)
                {
                    if (port1 != null)
                    {
                        this.Remove(port1);
                    }
                    this.myBottomPort = value;
                    if (value != null)
                    {
                        this.Add(value);
                    }
                    this.Changed(0xaf5, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [TypeConverter(typeof(SizeFConverter)), Category("Appearance"), Description("The margin around the text inside the background at the right side and the bottom")]
        public virtual SizeF BottomRightMargin
        {
            get
            {
                return this.myBottomRightMargin;
            }
            set
            {
                SizeF ef1 = this.myBottomRightMargin;
                if (ef1 != value)
                {
                    this.myBottomRightMargin = value;
                    this.Changed(0xaf8, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }

        [Category("Appearance"), Description("The Brush used by the background")]
        public System.Drawing.Brush Brush
        {
            get
            {
                if ((this.Background != null) && (this.Background is DiagramGraph))
                {
                    return ((DiagramGraph)this.Background).Brush;
                }
                return null;
            }
            set
            {
                if ((this.Background != null) && (this.Background is DiagramGraph))
                {
                    ((DiagramGraph)this.Background).Brush = value;
                }
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
                    this.Changed(0xaf1, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public virtual DiagramPort LeftPort
        {
            get
            {
                return this.myLeftPort;
            }
            set
            {
                DiagramPort port1 = this.myLeftPort;
                if (port1 != value)
                {
                    if (port1 != null)
                    {
                        this.Remove(port1);
                    }
                    this.myLeftPort = value;
                    if (value != null)
                    {
                        this.Add(value);
                    }
                    this.Changed(0xaf6, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The Pen used by the background"), Category("Appearance")]
        public System.Drawing.Pen Pen
        {
            get
            {
                if ((this.Background != null) && (this.Background is DiagramGraph))
                {
                    return ((DiagramGraph)this.Background).Pen;
                }
                return null;
            }
            set
            {
                if ((this.Background != null) && (this.Background is DiagramGraph))
                {
                    ((DiagramGraph)this.Background).Pen = value;
                }
            }
        }

        public virtual DiagramPort RightPort
        {
            get
            {
                return this.myRightPort;
            }
            set
            {
                DiagramPort port1 = this.myRightPort;
                if (port1 != value)
                {
                    if (port1 != null)
                    {
                        this.Remove(port1);
                    }
                    this.myRightPort = value;
                    if (value != null)
                    {
                        this.Add(value);
                    }
                    this.Changed(0xaf4, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public override DiagramShape SelectionObject
        {
            get
            {
                if ((this.Background != null) && !this.AutoResizes)
                {
                    return this.Background;
                }
                return this;
            }
        }

        public override bool Shadowed
        {
            get
            {
                if (this.Background != null)
                {
                    return this.Background.Shadowed;
                }
                return base.Shadowed;
            }
            set
            {
                if (this.Background != null)
                {
                    this.Background.Shadowed = value;
                }
                else
                {
                    base.Shadowed = value;
                }
            }
        }

        [Description("The margin around the text inside the background at the left side and the top"), Category("Appearance"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF TopLeftMargin
        {
            get
            {
                return this.myTopLeftMargin;
            }
            set
            {
                SizeF ef1 = this.myTopLeftMargin;
                if (ef1 != value)
                {
                    this.myTopLeftMargin = value;
                    this.Changed(0xaf7, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }

        public virtual DiagramPort TopPort
        {
            get
            {
                return this.myTopPort;
            }
            set
            {
                DiagramPort port1 = this.myTopPort;
                if (port1 != value)
                {
                    if (port1 != null)
                    {
                        this.Remove(port1);
                    }
                    this.myTopPort = value;
                    if (value != null)
                    {
                        this.Add(value);
                    }
                    this.Changed(0xaf3, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedAutoResizes = 0xaf9;
        public const int ChangedBackground = 0xaf2;
        public const int ChangedBottomPort = 0xaf5;
        public const int ChangedBottomRightMargin = 0xaf8;
        public const int ChangedLabel = 0xaf1;
        public const int ChangedLeftPort = 0xaf6;
        public const int ChangedRightPort = 0xaf4;
        public const int ChangedTopLeftMargin = 0xaf7;
        public const int ChangedTopPort = 0xaf3;
        private const int flagAutoResizes = 0x1000000;
        private DiagramShape myBack;
        private DiagramPort myBottomPort;
        private SizeF myBottomRightMargin;
        private DiagramText myLabel;
        private DiagramPort myLeftPort;
        private DiagramPort myRightPort;
        private SizeF myTopLeftMargin;
        private DiagramPort myTopPort;
    }
}
