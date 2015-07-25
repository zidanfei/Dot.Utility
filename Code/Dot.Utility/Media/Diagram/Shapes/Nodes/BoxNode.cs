using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class BoxNode : DiagramNode
    {
        public BoxNode()
        {
            this.myBody = null;
            this.myPortBorderMargin = new SizeF(4f, 4f);
            this.myPort = null;
            base.InternalFlags |= 0x20000;
            base.InternalFlags &= -17;
            this.myPort = this.CreatePort();
            this.Add(this.myPort);
            this.myBody = this.CreateBody();
            this.Add(this.myBody);
            base.Initializing = false;
            this.LayoutChildren(null);
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x899:
                    {
                        this.Body = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0x89a:
                    {
                        base.Initializing = true;
                        this.PortBorderMargin = e.GetSize(undo);
                        base.Initializing = false;
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            base.CopyChildren(newgroup, env);
            BoxNode node1 = (BoxNode)newgroup;
            node1.myPort = (DiagramPort)env[this.myPort];
            node1.myBody = (DiagramShape)env[this.myBody];
        }

        protected virtual DiagramShape CreateBody()
        {
            DiagramText text1 = new DiagramText();
            text1.TransparentBackground = false;
            text1.BackgroundColor = Color.White;
            text1.Multiline = true;
            text1.Selectable = false;
            return text1;
        }

        protected virtual DiagramPort CreatePort()
        {
            return new BoxPort();
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing)
            {
                DiagramShape obj1 = this.Body;
                if (obj1 != null)
                {
                    DiagramShape obj2 = this.Port;
                    if (obj2 != null)
                    {
                        RectangleF ef1 = obj1.Bounds;
                        SizeF ef2 = this.PortBorderMargin;
                        DiagramShape.InflateRect(ref ef1, ef2.Width, ef2.Height);
                        obj2.Bounds = ef1;
                    }
                }
            }
        }

        public override void Remove(DiagramShape obj)
        {
            base.Remove(obj);
            if (obj == this.myBody)
            {
                this.myBody = null;
            }
            else if (obj == this.myPort)
            {
                this.myPort = null;
            }
        }


        public virtual DiagramShape Body
        {
            get
            {
                return this.myBody;
            }
            set
            {
                DiagramShape obj1 = this.myBody;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        this.Remove(obj1);
                    }
                    this.myBody = value;
                    if (this.myBody != null)
                    {
                        if (obj1 != null)
                        {
                            this.myBody.Center = obj1.Center;
                        }
                        this.Add(this.myBody);
                    }
                    this.Changed(0x899, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether the link points are distributed evenly along each side"), Category("Appearance"), DefaultValue(false)]
        public virtual bool LinkPointsSpread
        {
            get
            {
                if (this.Port is BoxPort)
                {
                    return ((BoxPort)this.Port).LinkPointsSpread;
                }
                return false;
            }
            set
            {
                if (this.Port is BoxPort)
                {
                    ((BoxPort)this.Port).LinkPointsSpread = value;
                }
            }
        }

        public DiagramPort Port
        {
            get
            {
                return this.myPort;
            }
        }

        [TypeConverter(typeof(SizeFConverter)), Category("Appearance"), Description("The margin that is always visible for the port on each side of the body")]
        public virtual SizeF PortBorderMargin
        {
            get
            {
                return this.myPortBorderMargin;
            }
            set
            {
                SizeF ef1 = this.myPortBorderMargin;
                if (((ef1 != value) && (value.Width >= 0f)) && (value.Height >= 0f))
                {
                    this.myPortBorderMargin = value;
                    this.Changed(0x89a, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }


        public const int ChangedBody = 0x899;
        public const int ChangedPortBorderMargin = 0x89a;
        private DiagramShape myBody;
        private DiagramPort myPort;
        private SizeF myPortBorderMargin;
    }
}
