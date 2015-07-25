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
    public class IconicNode : DiagramNode
    {
        public IconicNode()
        {
            this.myIcon = null;
            this.myLabel = null;
            this.myPort = null;
            this.myDraggableLabel = false;
            this.myLabelOffset = new SizeF(-99999f, -99999f);
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0xa5b:
                    {
                        this.setDraggableLabel((bool)e.GetValue(undo), true);
                        return;
                    }
                case 0xa5c:
                    {
                        this.Icon = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0xa5d:
                    {
                        this.Label = (DiagramText)e.GetValue(undo);
                        return;
                    }
                case 0xa5e:
                    {
                        this.Port = (DiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0xa5f:
                    {
                        this.setLabelOffset(e.GetSize(undo), true);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            base.CopyChildren(newgroup, env);
            IconicNode node1 = (IconicNode)newgroup;
            node1.myIcon = (DiagramShape)env[this.myIcon];
            node1.myLabel = (DiagramText)env[this.myLabel];
            node1.myPort = (DiagramPort)env[this.myPort];
        }

        protected virtual DiagramShape CreateIcon(ResourceManager res, string iconname)
        {
            DiagramImage image1 = new DiagramImage();
            if (res != null)
            {
                image1.ResourceManager = res;
            }
            image1.Name = iconname;
            image1.Selectable = false;
            image1.Resizable = false;
            return image1;
        }

        protected virtual DiagramShape CreateIcon(ImageList imglist, int imgindex)
        {
            DiagramImage image1 = new DiagramImage();
            image1.ImageList = imglist;
            image1.Index = imgindex;
            image1.Selectable = false;
            image1.Resizable = false;
            return image1;
        }

        protected virtual DiagramText CreateLabel(string name)
        {
            DiagramText text1 = null;
            if (name != null)
            {
                text1 = new DiagramText();
                text1.Text = name;
                text1.Selectable = this.DraggableLabel;
                text1.Alignment = 0x20;
            }
            return text1;
        }

        protected virtual DiagramPort CreatePort()
        {
            DiagramPort port1 = new DiagramPort();
            port1.Style = DiagramPortStyle.None;
            port1.Size = new SizeF(6f, 6f);
            port1.FromSpot = 0;
            port1.ToSpot = 0;
            port1.PortObject = this;
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
            this.myLabel = this.CreateLabel(name);
            this.Add(this.myLabel);
            this.myPort = this.CreatePort();
            this.Add(this.myPort);
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
                    DiagramText text1 = this.Label;
                    if (text1 != null)
                    {
                        if (this.DraggableLabel && (childchanged == text1))
                        {
                            this.myLabelOffset = new SizeF(text1.Left - obj1.Left, text1.Top - obj1.Top);
                            return;
                        }
                        if (this.myLabelOffset.Width > -99999f)
                        {
                            text1.Position = new PointF(obj1.Left + this.myLabelOffset.Width, obj1.Top + this.myLabelOffset.Height);
                        }
                        else
                        {
                            text1.SetSpotLocation(0x20, obj1, 0x80);
                        }
                    }
                    if (this.Port != null)
                    {
                        this.Port.SetSpotLocation(1, obj1, 1);
                    }
                }
            }
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
                this.myLabel = null;
            }
            else if (obj == this.myPort)
            {
                this.myPort = null;
            }
        }

        private void setDraggableLabel(bool d, bool undoing)
        {
            bool flag1 = this.myDraggableLabel;
            if (flag1 != d)
            {
                this.myDraggableLabel = d;
                this.Changed(0xa5b, 0, flag1, DiagramShape.NullRect, 0, d, DiagramShape.NullRect);
                if (!undoing && (this.Label != null))
                {
                    this.Label.Selectable = d;
                }
            }
        }

        private void setLabelOffset(SizeF v, bool undoing)
        {
            SizeF ef1 = this.myLabelOffset;
            if (ef1 != v)
            {
                this.myLabelOffset = v;
                this.Changed(0xa5f, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(v));
                if (!undoing)
                {
                    this.LayoutChildren(null);
                }
            }
        }


        [DefaultValue(false), Description("Whether users can drag the label independently of the node"), Category("Behavior")]
        public virtual bool DraggableLabel
        {
            get
            {
                return this.myDraggableLabel;
            }
            set
            {
                this.setDraggableLabel(value, false);
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
                    this.Changed(0xa5c, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    if ((this.Port != null) && (this.Port.PortObject == obj1))
                    {
                        this.Port.PortObject = value;
                    }
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
                    this.Changed(0xa5d, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), Description("The offset of the Label relative to the Icon"), TypeConverter(typeof(SizeFConverter))]
        public SizeF LabelOffset
        {
            get
            {
                return this.myLabelOffset;
            }
            set
            {
                this.setLabelOffset(value, false);
            }
        }

        public DiagramPort Port
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
                    this.Changed(0xa5e, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    if ((value != null) && (value.PortObject == null))
                    {
                        value.PortObject = this.Icon;
                    }
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


        public const int ChangedDraggableLabel = 0xa5b;
        public const int ChangedIcon = 0xa5c;
        public const int ChangedLabel = 0xa5d;
        public const int ChangedLabelOffset = 0xa5f;
        public const int ChangedPort = 0xa5e;
        private bool myDraggableLabel;
        private DiagramShape myIcon;
        private DiagramText myLabel;
        private SizeF myLabelOffset;
        private DiagramPort myPort;
    }
}
