using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class BoxIconNode : DiagramNode
    {
        public BoxIconNode()
        {
            this.myTopLeftMargin = new SizeF(4f, 2f);
            this.myBottomRightMargin = new SizeF(4f, 2f);
            base.InternalFlags &= -17;
            base.InternalFlags |= 0x1020000;

            Dot.Utility.Media.Diagram.Shapes.RoundedRectangleGraph background = new Dot.Utility.Media.Diagram.Shapes.RoundedRectangleGraph();
            background.Corner = new SizeF(3f, 3f);
            background.Brush = System.Drawing.Brushes.WhiteSmoke;
            background.Pen = new Pen(System.Drawing.Brushes.DarkBlue);
            background.Pen.Width = 1.5F;
            background.Selectable = false;
            this.myBack = background;
            this.Add(this.myBack);

            this.myLabel = this.CreateLabel();
            this.Label.Selectable = false;
            this.Label.Alignment = 1;
            this.Add(this.myLabel);

            base.Initializing = false;

            this.LayoutChildren(null);
        }

        public void Initialize(
            System.Resources.ResourceManager ResourceManager,
            string IconName,
            float Width,
            float Height)
        {
            this.Icon.Initialize(
                ResourceManager,
                IconName,
                null,
                null,
                0,
                0);
            this.Icon.Icon.Size = new System.Drawing.SizeF(25, 25);
            this.Icon.Selectable = false;
            this.Icon.Editable = false;

            this.AutoRescales = false;
            this.AutoResizes = false;

            this.Size = new SizeF(Width, Height);

            this.Shadowed = true;

            this.Icon.Location = new PointF(13, 13);
            this.Add(this.Icon);

            this.LeftPorts = new DiagramPort[DefaultYPortCount];
            for (int count = 0; count < this.LeftPorts.Length; count++)
            {
                this.LeftPorts[count] = this.CreatePort(0x100);
            }
            this.RightPorts = new DiagramPort[DefaultYPortCount];
            for (int count = 0; count < this.LeftPorts.Length; count++)
            {
                this.RightPorts[count] = this.CreatePort(0x40);
            }

            this.TopPorts = new DiagramPort[DefaultXPortCount];
            for (int count = 0; count < this.TopPorts.Length; count++)
            {
                this.TopPorts[count] = this.CreatePort(0x20);
            }

            this.BottomPorts = new DiagramPort[DefaultXPortCount];
            for (int count = 0; count < this.BottomPorts.Length; count++)
            {
                this.BottomPorts[count] = this.CreatePort(0x80);
            }

            this.LayoutChildren(null);
        }

        private Dot.Utility.Media.Diagram.Shapes.GeneralNode _Icon = new Dot.Utility.Media.Diagram.Shapes.GeneralNode();
        public Dot.Utility.Media.Diagram.Shapes.GeneralNode Icon
        {
            get
            {
                return this._Icon;
            }
        }

        public float FontSize
        {
            get
            {
                return this.Label.FontSize;
            }
            set
            {
                this.Label.FontSize = value;
            }
        }

        protected virtual DiagramPort CreatePort(int Spot)
        {
            DiagramPort port = new DiagramPort();
            port.Style = DiagramPortStyle.None;
            port.Size = new SizeF(4f, 4f);
            port.FromSpot = Spot;
            port.ToSpot = Spot;
            this.Add(port);

            return port;
        }

        public const int DefaultXPortCount = 25;
        public const int DefaultYPortCount = 10;


        private DiagramPort[] _LeftPorts;
        public virtual DiagramPort[] LeftPorts
        {
            get
            {
                return this._LeftPorts;
            }
            set
            {
                DiagramPort[] ports = this._LeftPorts;
                if (ports != null)
                {
                    foreach (DiagramPort port in ports)
                    {
                        if (port != null)
                        {
                            this.Remove(port);
                        }
                    }
                }
                this._LeftPorts = value;
                if (value != null)
                {
                    foreach (DiagramPort port in this._LeftPorts)
                    {
                        if (port != null)
                        {
                            this.Add(port);
                        }
                    }
                }
                this.Changed(0xafa, 0, ports, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
            }
        }




        private DiagramPort[] _RightPorts;
        public virtual DiagramPort[] RightPorts
        {
            get
            {
                return this._RightPorts;
            }
            set
            {
                DiagramPort[] ports = this._RightPorts;
                if (ports != null)
                {
                    foreach (DiagramPort port in ports)
                    {
                        if (port != null)
                        {
                            this.Remove(port);
                        }
                    }
                }
                this._RightPorts = value;
                if (value != null)
                {
                    foreach (DiagramPort port in this._RightPorts)
                    {
                        if (port != null)
                        {
                            this.Add(port);
                        }
                    }
                }
                this.Changed(0xafb, 0, ports, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
            }
        }




        private DiagramPort[] _TopPorts;
        public virtual DiagramPort[] TopPorts
        {
            get
            {
                return this._TopPorts;
            }
            set
            {
                DiagramPort[] ports = this._TopPorts;
                if (ports != null)
                {
                    foreach (DiagramPort port in ports)
                    {
                        if (port != null)
                        {
                            this.Remove(port);
                        }
                    }
                }
                this._TopPorts = value;
                if (value != null)
                {
                    foreach (DiagramPort port in this._TopPorts)
                    {
                        if (port != null)
                        {
                            this.Add(port);
                        }
                    }
                }
                this.Changed(0xafc, 0, ports, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
            }
        }




        private DiagramPort[] _BottomPorts;
        public virtual DiagramPort[] BottomPorts
        {
            get
            {
                return this._BottomPorts;
            }
            set
            {
                DiagramPort[] ports = this._BottomPorts;
                if (ports != null)
                {
                    foreach (DiagramPort port in ports)
                    {
                        if (port != null)
                        {
                            this.Remove(port);
                        }
                    }
                }
                this._BottomPorts = value;
                if (value != null)
                {
                    foreach (DiagramPort port in this._BottomPorts)
                    {
                        if (port != null)
                        {
                            this.Add(port);
                        }
                    }
                }
                this.Changed(0xafd, 0, ports, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
            }
        }



        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            base.CopyChildren(newgroup, env);

            BoxIconNode node = (BoxIconNode)newgroup;

            if (this._LeftPorts != null)
            {
                node._LeftPorts = new DiagramPort[DefaultYPortCount];
                for (int count = 0; count < this._LeftPorts.Length; count++)
                {
                    node._LeftPorts[count] = (DiagramPort)env[this._LeftPorts[count]];
                }
            }

            if (this._RightPorts != null)
            {
                node._RightPorts = new DiagramPort[DefaultYPortCount];
                for (int count = 0; count < this._RightPorts.Length; count++)
                {
                    node._RightPorts[count] = (DiagramPort)env[this._RightPorts[count]];
                }
            }

            if (this._TopPorts != null)
            {
                node._TopPorts = new DiagramPort[DefaultXPortCount];
                for (int count = 0; count < this._TopPorts.Length; count++)
                {
                    node._TopPorts[count] = (DiagramPort)env[this._TopPorts[count]];
                }
            }

            if (this._BottomPorts != null)
            {
                node._BottomPorts = new DiagramPort[DefaultXPortCount];
                for (int count = 0; count < this._BottomPorts.Length; count++)
                {
                    node._BottomPorts[count] = (DiagramPort)env[this._BottomPorts[count]];
                }
            }

            node.myBack = (DiagramShape)env[this.myBack];
            node.myLabel = (DiagramText)env[this.myLabel];
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
            else
            {
                if (this.LeftPorts != null)
                {
                    for (int count = 0; count < this.LeftPorts.Length; count++)
                    {
                        if (this.LeftPorts[count] != null && this.LeftPorts[count] == obj)
                        {
                            this.LeftPorts[count] = null;
                            return;
                        }
                    }
                }

                if (this.RightPorts != null)
                {
                    for (int count = 0; count < this.RightPorts.Length; count++)
                    {
                        if (this.RightPorts[count] != null && this.RightPorts[count] == obj)
                        {
                            this.RightPorts[count] = null;
                            return;
                        }
                    }
                }

                if (this.BottomPorts != null)
                {
                    for (int count = 0; count < this.BottomPorts.Length; count++)
                    {
                        if (this.BottomPorts[count] != null && this.BottomPorts[count] == obj)
                        {
                            this.BottomPorts[count] = null;
                            return;
                        }
                    }
                }

                if (this.TopPorts != null)
                {
                    for (int count = 0; count < this.TopPorts.Length; count++)
                    {
                        if (this.TopPorts[count] != null && this.TopPorts[count] == obj)
                        {
                            this.TopPorts[count] = null;
                            return;
                        }
                    }
                }
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0xaf1:
                    this.Label = (DiagramText)e.GetValue(undo);
                    return;
                case 0xaf2:
                    this.Background = (DiagramShape)e.GetValue(undo);
                    return;
                case 0xaf7:
                    base.Initializing = true;
                    this.TopLeftMargin = e.GetSize(undo);
                    base.Initializing = false;
                    return;
                case 0xaf8:
                    base.Initializing = true;
                    this.BottomRightMargin = e.GetSize(undo);
                    base.Initializing = false;
                    return;
                case 0xaf9:
                    this.setAutoResizes((bool)e.GetValue(undo), true);
                    return;
                case 0xafa:
                    this.LeftPorts = (DiagramPort[])e.GetValue(undo);
                    return;
                case 0xafb:
                    this.RightPorts = (DiagramPort[])e.GetValue(undo);
                    return;
                case 0xafc:
                    this.TopPorts = (DiagramPort[])e.GetValue(undo);
                    return;
                case 0xafd:
                    this.BottomPorts = (DiagramPort[])e.GetValue(undo);
                    return;
                default:
                    base.ChangeValue(e, undo);
                    return;
            }
        }

        protected virtual DiagramText CreateLabel()
        {
            DiagramText text1 = new DiagramText();
            text1.Selectable = false;
            text1.Multiline = true;
            return text1;
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
        private SizeF myBottomRightMargin;
        private DiagramText myLabel;
        private SizeF myTopLeftMargin;

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing && this.Background != null)
            {
                if (this.Label != null)
                {
                    if (this.AutoResizes)
                    {
                        this.Background.Bounds = new RectangleF(
                            this.Label.Left - this.TopLeftMargin.Width,
                            this.Label.Top - this.TopLeftMargin.Height,
                            (this.Label.Width + this.TopLeftMargin.Width) + this.BottomRightMargin.Width,
                            (this.Label.Height + this.TopLeftMargin.Height) + this.BottomRightMargin.Height);
                    }
                    else
                    {
                        float width = System.Math.Max((float)(this.Background.Width - (this.TopLeftMargin.Width + this.BottomRightMargin.Width + this.Icon.Width) - 8f), (float)0f);
                        float wrappingHeight = System.Math.Max((float)(this.Background.Height - (this.TopLeftMargin.Height + this.BottomRightMargin.Height)), (float)0f);
                        this.Label.Width = width;
                        this.Label.WrappingWidth = System.Math.Max((float)(this.Background.Width - (this.TopLeftMargin.Width + this.BottomRightMargin.Width + this.Icon.Width)), (float)0f); ;
                        this.Label.UpdateSize();
                        float height = System.Math.Min(this.Label.Height, wrappingHeight);
                        float x = this.Background.Left + this.TopLeftMargin.Width + this.Icon.Width;
                        float offset = wrappingHeight > height ? (wrappingHeight - height) / 2 : 0;
                        float y = this.Background.Bounds.Y + this.TopLeftMargin.Height + offset; this.Label.Bounds = new RectangleF(x, y, width, height);
                    }
                }

                if (this.LeftPorts != null && this.LeftPorts.Length > 0)
                {
                    float deltaY = this.Background.Bounds.Height / this.LeftPorts.Length;

                    for (int count = 0; count < this.LeftPorts.Length; count++)
                    {
                        DiagramPort port = this.LeftPorts[count];
                        if (port == null)
                        {
                            continue;
                        }

                        PointF tf1 = new PointF();

                        tf1.X = this.Background.Bounds.X;
                        tf1.Y = this.Background.Bounds.Y + deltaY * ((float)count + 0.5f);

                        RectangleF ef1 = port.Bounds;
                        ef1.X = tf1.X;
                        ef1.Y = tf1.Y - (ef1.Height / 2f);
                        port.Bounds = ef1;
                    }
                }

                if (this.RightPorts != null && this.RightPorts.Length > 0)
                {
                    float deltaY = this.Background.Bounds.Height / this.RightPorts.Length;

                    for (int count = 0; count < this.RightPorts.Length; count++)
                    {
                        DiagramPort port = this.RightPorts[count];
                        if (port == null)
                        {
                            continue;
                        }

                        PointF tf1 = new PointF();

                        tf1.X = this.Background.Bounds.X + this.Background.Bounds.Width;
                        tf1.Y = this.Background.Bounds.Y + deltaY * ((float)count + 0.5f);

                        RectangleF ef1 = port.Bounds;
                        ef1.X = tf1.X - ef1.Width;
                        ef1.Y = tf1.Y - (ef1.Height / 2f);
                        port.Bounds = ef1;
                    }
                }

                if (this.TopPorts != null && this.TopPorts.Length > 0)
                {
                    float deltaX = this.Background.Bounds.Width / this.TopPorts.Length;

                    for (int count = 0; count < this.TopPorts.Length; count++)
                    {
                        DiagramPort port = this.TopPorts[count];
                        if (port != null)
                        {
                            PointF tf1 = new PointF();

                            tf1.X = this.Background.Bounds.X + deltaX * ((float)count + 0.5f);
                            tf1.Y = this.Background.Bounds.Y;

                            RectangleF ef1 = port.Bounds;
                            ef1.X = tf1.X - (ef1.Width / 2f);
                            ef1.Y = tf1.Y;
                            port.Bounds = ef1;
                        }
                    }
                }

                if (this.BottomPorts != null && this.BottomPorts.Length > 0)
                {
                    float deltaX = this.Background.Bounds.Width / this.BottomPorts.Length;

                    for (int count = 0; count < this.BottomPorts.Length; count++)
                    {
                        DiagramPort port = this.BottomPorts[count];
                        if (port != null)
                        {
                            PointF tf1 = new PointF();

                            tf1.X = this.Background.Bounds.X + deltaX * ((float)count + 0.5f);
                            tf1.Y = this.Background.Bounds.Y + this.Background.Bounds.Height;

                            RectangleF ef1 = port.Bounds;
                            ef1.X = tf1.X - ef1.Width / 2f;
                            ef1.Y = tf1.Y - ef1.Height;
                            port.Bounds = ef1;
                        }
                    }
                }
            }
        }
    }
}
