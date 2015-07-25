using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class SimpleNode : DiagramNode, INodeIconConstraint
    {
        public SimpleNode()
        {
            this.myText = "";
            this.myIcon = null;
            this.myLabel = null;
            this.myInPort = null;
            this.myOutPort = null;
            this.myOrientation = System.Windows.Forms.Orientation.Horizontal;
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0xa29:
                    {
                        this.Text = (string)e.GetValue(undo);
                        return;
                    }
                case 0xa2a:
                    {
                        this.Icon = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0xa2b:
                    {
                        this.Label = (DiagramText)e.GetValue(undo);
                        return;
                    }
                case 0xa2c:
                    {
                        this.InPort = (DiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0xa2d:
                    {
                        this.OutPort = (DiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0xa2e:
                    {
                        this.setOrientation((System.Windows.Forms.Orientation)e.GetInt(undo), true);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            base.CopyChildren(newgroup, env);
            SimpleNode node1 = (SimpleNode)newgroup;
            node1.myIcon = (DiagramShape)env[this.myIcon];
            node1.myLabel = (DiagramText)env[this.myLabel];
            node1.myInPort = (DiagramPort)env[this.myInPort];
            node1.myOutPort = (DiagramPort)env[this.myOutPort];
        }

        protected virtual DiagramShape CreateIcon(ResourceManager res, string iconname)
        {
            if (iconname != null)
            {
                NodeIcon icon1 = new NodeIcon();
                if (res != null)
                {
                    icon1.ResourceManager = res;
                }
                icon1.Name = iconname;
                icon1.MinimumIconSize = new SizeF(20f, 20f);
                icon1.MaximumIconSize = new SizeF(100f, 200f);
                icon1.Size = icon1.MinimumIconSize;
                return icon1;
            }
            RectangleGraph rectangle1 = new RectangleGraph();
            rectangle1.Selectable = false;
            rectangle1.Size = new SizeF(20f, 20f);
            return rectangle1;
        }

        protected virtual DiagramShape CreateIcon(ImageList imglist, int imgindex)
        {
            NodeIcon icon1 = new NodeIcon();
            icon1.ImageList = imglist;
            icon1.Index = imgindex;
            icon1.MinimumIconSize = new SizeF(20f, 20f);
            icon1.MaximumIconSize = new SizeF(100f, 200f);
            icon1.Size = icon1.MinimumIconSize;
            return icon1;
        }

        protected virtual DiagramText CreateLabel(string name)
        {
            DiagramText text1 = null;
            if (name != null)
            {
                text1 = new DiagramText();
                text1.Text = name;
                text1.Selectable = false;
                text1.Alignment = 1;
            }
            return text1;
        }

        protected virtual DiagramPort CreatePort(bool input)
        {
            DiagramPort port1 = new DiagramPort();
            port1.Size = new SizeF(6f, 6f);
            port1.IsValidFrom = !input;
            port1.IsValidTo = input;
            return port1;
        }

        public virtual void Initialize(ResourceManager res, string iconname, string name)
        {
            base.Initializing = true;
            this.myIcon = this.CreateIcon(res, iconname);
            this.Add(this.myIcon);
            this.initializeCommon(name);
        }

        public virtual void Initialize(ImageList imglist, int imgindex, string name)
        {
            base.Initializing = true;
            this.myIcon = this.CreateIcon(imglist, imgindex);
            this.Add(this.myIcon);
            this.initializeCommon(name);
        }

        private void initializeCommon(string name)
        {
            this.myText = name;
            this.myLabel = this.CreateLabel(name);
            this.Add(this.myLabel);
            if (this.myLabel != null)
            {
                this.myLabel.AddObserver(this);
            }
            this.myInPort = this.CreatePort(true);
            this.Add(this.myInPort);
            this.myOutPort = this.CreatePort(false);
            this.Add(this.myOutPort);
            base.PropertiesDelegatedToSelectionObject = true;
            base.Initializing = false;
            this.LayoutChildren(null);
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing)
            {
                DiagramShape obj1 = this.Icon;
                if (obj1 != null)
                {
                    if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        if (this.Label != null)
                        {
                            this.Label.SetSpotLocation(0x20, obj1, 0x80);
                        }
                        if (this.InPort != null)
                        {
                            this.InPort.SetSpotLocation(0x40, obj1, 0x100);
                        }
                        if (this.OutPort != null)
                        {
                            this.OutPort.SetSpotLocation(0x100, obj1, 0x40);
                        }
                    }
                    else
                    {
                        if (this.Label != null)
                        {
                            this.Label.SetSpotLocation(0x100, obj1, 0x40);
                        }
                        if (this.InPort != null)
                        {
                            this.InPort.SetSpotLocation(0x80, obj1, 0x20);
                        }
                        if (this.OutPort != null)
                        {
                            this.OutPort.SetSpotLocation(0x20, obj1, 0x80);
                        }
                    }
                }
            }
        }

        protected override void OnObservedChanged(DiagramShape observed, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            base.OnObservedChanged(observed, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
            if ((subhint == 0x5dd) && (observed == this.Label))
            {
                this.Text = (string)newVal;
            }
        }

        public virtual void OnOrientationChanged(System.Windows.Forms.Orientation old)
        {
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                if (this.InPort != null)
                {
                    this.InPort.ToSpot = 0x20;
                    this.InPort.FromSpot = 0x20;
                }
                if (this.OutPort != null)
                {
                    this.OutPort.ToSpot = 0x80;
                    this.OutPort.FromSpot = 0x80;
                }
            }
            else
            {
                if (this.InPort != null)
                {
                    this.InPort.ToSpot = 0x100;
                    this.InPort.FromSpot = 0x100;
                }
                if (this.OutPort != null)
                {
                    this.OutPort.ToSpot = 0x40;
                    this.OutPort.FromSpot = 0x40;
                }
            }
            this.LayoutChildren(null);
        }

        public override void Remove(DiagramShape obj)
        {
            base.Remove(obj);
            if (obj == this.myIcon)
            {
                this.myIcon = null;
            }
            else if (obj == this.myLabel)
            {
                this.myLabel.RemoveObserver(this);
                this.myLabel = null;
            }
            else if (obj == this.myInPort)
            {
                this.myInPort = null;
            }
            else if (obj == this.myOutPort)
            {
                this.myOutPort = null;
            }
        }

        private void setOrientation(System.Windows.Forms.Orientation o, bool undoing)
        {
            System.Windows.Forms.Orientation orientation1 = this.myOrientation;
            if (orientation1 != o)
            {
                this.myOrientation = o;
                this.Changed(0xa2e, (int)orientation1, null, DiagramShape.NullRect, (int)o, null, DiagramShape.NullRect);
                if (!undoing)
                {
                    this.OnOrientationChanged(orientation1);
                }
            }
        }


        public virtual DiagramShape Icon
        {
            get
            {
                return this.myIcon;
            }
            set
            {
                DiagramShape obj1 = this.myIcon;
                if (obj1 != value)
                {
                    base.CopyPropertiesFromSelectionObject(obj1, value);
                    if (obj1 != null)
                    {
                        this.Remove(obj1);
                    }
                    this.myIcon = value;
                    if (value != null)
                    {
                        this.InsertBefore(null, value);
                    }
                    this.Changed(0xa2a, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public virtual DiagramImage Image
        {
            get
            {
                return (this.myIcon as DiagramImage);
            }
        }

        public DiagramPort InPort
        {
            get
            {
                return this.myInPort;
            }
            set
            {
                DiagramPort port1 = this.myInPort;
                if (port1 != value)
                {
                    if (port1 != null)
                    {
                        this.Remove(port1);
                    }
                    this.myInPort = value;
                    if (value != null)
                    {
                        this.Add(value);
                    }
                    this.Changed(0xa2c, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
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
                        text1.RemoveObserver(this);
                    }
                    this.myLabel = value;
                    if (value != null)
                    {
                        this.Add(value);
                        value.AddObserver(this);
                    }
                    this.Changed(0xa2b, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [TypeConverter(typeof(SizeFConverter)), Description("The maximum size for the icon"), Category("Appearance")]
        public virtual SizeF MaximumIconSize
        {
            get
            {
                NodeIcon icon1 = this.Icon as NodeIcon;
                if (icon1 != null)
                {
                    return icon1.MaximumIconSize;
                }
                return new SizeF(100f, 200f);
            }
            set
            {
                NodeIcon icon1 = this.Icon as NodeIcon;
                if (icon1 != null)
                {
                    icon1.MaximumIconSize = value;
                }
            }
        }

        [Description("The minimum size for the icon"), TypeConverter(typeof(SizeFConverter)), Category("Appearance")]
        public virtual SizeF MinimumIconSize
        {
            get
            {
                if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    float single1 = 20f;
                    float single2 = 20f;
                    NodeIcon icon1 = this.Icon as NodeIcon;
                    if (icon1 != null)
                    {
                        single1 = icon1.MinimumIconSize.Width;
                        single2 = icon1.MinimumIconSize.Height;
                    }
                    if (this.InPort != null)
                    {
                        single2 = System.Math.Max(single2, this.InPort.Height);
                    }
                    if (this.OutPort != null)
                    {
                        single2 = System.Math.Max(single2, this.OutPort.Height);
                    }
                    return new SizeF(single1, single2);
                }
                float single3 = 20f;
                float single4 = 20f;
                NodeIcon icon2 = this.Icon as NodeIcon;
                if (icon2 != null)
                {
                    single3 = icon2.MinimumIconSize.Width;
                    single4 = icon2.MinimumIconSize.Height;
                }
                if (this.InPort != null)
                {
                    single3 = System.Math.Max(single3, this.InPort.Width);
                }
                if (this.OutPort != null)
                {
                    single3 = System.Math.Max(single3, this.OutPort.Width);
                }
                return new SizeF(single3, single4);
            }
            set
            {
                NodeIcon icon1 = this.Icon as NodeIcon;
                if (icon1 != null)
                {
                    icon1.MinimumIconSize = value;
                }
            }
        }

        [Category("Appearance"), DefaultValue(0), Description("The general orientation of the node and how links connect to it")]
        public System.Windows.Forms.Orientation Orientation
        {
            get
            {
                return this.myOrientation;
            }
            set
            {
                this.setOrientation(value, false);
            }
        }

        public DiagramPort OutPort
        {
            get
            {
                return this.myOutPort;
            }
            set
            {
                DiagramPort port1 = this.myOutPort;
                if (port1 != value)
                {
                    if (port1 != null)
                    {
                        this.Remove(port1);
                    }
                    this.myOutPort = value;
                    if (value != null)
                    {
                        this.Add(value);
                    }
                    this.Changed(0xa2d, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public override DiagramShape SelectionObject
        {
            get
            {
                if (this.Icon != null)
                {
                    return this.Icon;
                }
                return this;
            }
        }

        public override string Text
        {
            get
            {
                return this.myText;
            }
            set
            {
                string text1 = this.myText;
                if (text1 != value)
                {
                    this.myText = value;
                    this.Changed(0xa29, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    if (this.Label != null)
                    {
                        this.Label.Text = value;
                    }
                }
            }
        }


        public const int ChangedIcon = 0xa2a;
        public const int ChangedInPort = 0xa2c;
        public const int ChangedLabel = 0xa2b;
        public const int ChangedOrientation = 0xa2e;
        public const int ChangedOutPort = 0xa2d;
        public const int ChangedText = 0xa29;
        private DiagramShape myIcon;
        private DiagramPort myInPort;
        private DiagramText myLabel;
        private System.Windows.Forms.Orientation myOrientation;
        private DiagramPort myOutPort;
        private string myText;
    }
}
