using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Collections;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class MultiTextNode : DiagramNode
    {
        public MultiTextNode()
        {
            this.myListGroup = null;
            this.myTopPort = null;
            this.myBottomPort = null;
            this.myLeftPorts = new ArrayList();
            this.myRightPorts = new ArrayList();
            this.myItemWidth = -1f;
            base.Initializing = true;
            this.myListGroup = new MultiTextNodeListGroup();
            this.myListGroup.Selectable = false;
            this.myListGroup.LinePen = DiagramGraph.Pens_Black;
            this.myListGroup.BorderPen = DiagramGraph.Pens_Black;
            this.myListGroup.Alignment = 1;
            this.Add(this.myListGroup);
            this.myTopPort = this.CreateEndPort(true);
            this.Add(this.myTopPort);
            this.myBottomPort = this.CreateEndPort(false);
            this.Add(this.myBottomPort);
            base.InternalFlags &= -17;
            base.Initializing = false;
            this.LayoutChildren(null);
        }

        public void AddItem(DiagramShape item, DiagramShape leftport, DiagramShape rightport)
        {
            this.InsertItem(this.ItemCount, item, leftport, rightport);
        }

        public virtual DiagramText AddString(string s)
        {
            int num1 = this.ItemCount;
            DiagramText text1 = this.CreateText(s, num1);
            this.AddItem(text1, this.CreatePort(true, num1), this.CreatePort(false, num1));
            return text1;
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0xbb9:
                    {
                        int num1 = e.NewInt;
                        DiagramShape obj1 = (DiagramShape)e.GetValue(undo);
                        if (!undo)
                        {
                            this.myLeftPorts.Insert(num1, obj1);
                            this.Add(obj1);
                        }
                        return;
                    }
                case 0xbba:
                    {
                        int num2 = e.NewInt;
                        DiagramShape obj2 = (DiagramShape)e.GetValue(undo);
                        if (!undo)
                        {
                            this.myRightPorts.Insert(num2, obj2);
                            this.Add(obj2);
                        }
                        return;
                    }
                case 0xbbb:
                    {
                        int num3 = e.OldInt;
                        DiagramShape obj3 = (DiagramShape)e.GetValue(undo);
                        if (undo)
                        {
                            this.myLeftPorts.Insert(num3, obj3);
                            this.Add(obj3);
                        }
                        return;
                    }
                case 0xbbc:
                    {
                        int num4 = e.OldInt;
                        DiagramShape obj4 = (DiagramShape)e.GetValue(undo);
                        if (undo)
                        {
                            this.myRightPorts.Insert(num4, obj4);
                            this.Add(obj4);
                        }
                        return;
                    }
                case 0xbbd:
                    {
                        int num5 = e.OldInt;
                        if (num5 >= 0)
                        {
                            this.SetRightPort(num5, (DiagramShape)e.GetValue(undo));
                            return;
                        }
                        num5 = -num5 - 1;
                        this.SetLeftPort(num5, (DiagramShape)e.GetValue(undo));
                        return;
                    }
                case 0xbbe:
                    {
                        this.TopPort = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0xbbf:
                    {
                        this.BottomPort = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0xbc0:
                    {
                        this.setItemWidth(e.GetFloat(undo), true);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            MultiTextNode node1 = (MultiTextNode)newgroup;
            base.CopyChildren(newgroup, env);
            node1.myLeftPorts = new ArrayList();
            node1.myRightPorts = new ArrayList();
            node1.myListGroup = (ListGroup)env[this.myListGroup];
            node1.myTopPort = (DiagramShape)env[this.myTopPort];
            node1.myBottomPort = (DiagramShape)env[this.myBottomPort];
            for (int num1 = 0; num1 < this.myLeftPorts.Count; num1++)
            {
                DiagramShape obj1 = (DiagramShape)this.myLeftPorts[num1];
                DiagramShape obj2 = (DiagramShape)env[obj1];
                node1.myLeftPorts.Add(obj2);
            }
            for (int num2 = 0; num2 < this.myRightPorts.Count; num2++)
            {
                DiagramShape obj3 = (DiagramShape)this.myRightPorts[num2];
                DiagramShape obj4 = (DiagramShape)env[obj3];
                node1.myRightPorts.Add(obj4);
            }
        }

        public virtual DiagramShape CreateEndPort(bool top)
        {
            DiagramPort port1 = new DiagramPort();
            port1.Size = new SizeF(5f, 3f);
            port1.Style = DiagramPortStyle.None;
            if (top)
            {
                port1.FromSpot = 0x20;
                port1.ToSpot = 0x20;
                return port1;
            }
            port1.FromSpot = 0x80;
            port1.ToSpot = 0x80;
            return port1;
        }

        public virtual DiagramShape CreatePort(bool left, int idx)
        {
            DiagramPort port1 = new DiagramPort();
            port1.Size = new SizeF(3f, 5f);
            port1.Style = DiagramPortStyle.None;
            if (left)
            {
                port1.FromSpot = 0x100;
                port1.ToSpot = 0x100;
                return port1;
            }
            port1.FromSpot = 0x40;
            port1.ToSpot = 0x40;
            return port1;
        }

        public virtual DiagramText CreateText(string s, int idx)
        {
            DiagramText text1 = new DiagramText();
            text1.Selectable = false;
            text1.Alignment = 1;
            text1.Multiline = true;
            text1.BackgroundOpaqueWhenSelected = true;
            text1.BackgroundColor = Color.LightBlue;
            text1.DragsNode = true;
            text1.Text = s;
            text1.Wrapping = true;
            if (this.ItemWidth > 0f)
            {
                text1.WrappingWidth = this.ItemWidth;
                text1.Width = this.ItemWidth;
            }
            return text1;
        }

        public DiagramShape GetItem(int i)
        {
            return this.myListGroup[i];
        }

        public virtual DiagramShape GetLeftPort(int i)
        {
            if ((i >= 0) && (i < this.myLeftPorts.Count))
            {
                return (DiagramShape)this.myLeftPorts[i];
            }
            return null;
        }

        public virtual DiagramShape GetRightPort(int i)
        {
            if ((i >= 0) && (i < this.myRightPorts.Count))
            {
                return (DiagramShape)this.myRightPorts[i];
            }
            return null;
        }

        public virtual string GetString(int i)
        {
            DiagramText text1 = this.myListGroup[i] as DiagramText;
            if (text1 != null)
            {
                return text1.Text;
            }
            return "";
        }

        public virtual void InsertItem(int i, DiagramShape item, DiagramShape leftport, DiagramShape rightport)
        {
            if ((i >= 0) && (i <= this.myListGroup.Count))
            {
                this.myListGroup.Insert(i, item);
                if ((i >= 0) && (i <= this.myLeftPorts.Count))
                {
                    this.myLeftPorts.Insert(i, leftport);
                    this.Add(leftport);
                    this.Changed(0xbb9, i, null, DiagramShape.NullRect, i, leftport, DiagramShape.NullRect);
                }
                if ((i >= 0) && (i <= this.myRightPorts.Count))
                {
                    this.myRightPorts.Insert(i, rightport);
                    this.Add(rightport);
                    this.Changed(0xbba, i, null, DiagramShape.NullRect, i, rightport, DiagramShape.NullRect);
                }
            }
        }

        public virtual DiagramText InsertString(int i, string s)
        {
            if ((i < 0) || (i >= this.ItemCount))
            {
                return null;
            }
            DiagramText text1 = this.CreateText(s, i);
            this.InsertItem(i, text1, this.CreatePort(true, i), this.CreatePort(false, i));
            return text1;
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing && (this.myListGroup != null))
            {
                base.Initializing = true;
                if (this.myTopPort != null)
                {
                    this.myTopPort.SetSpotLocation(0x80, this.myListGroup, 0x20);
                }
                if (this.myBottomPort != null)
                {
                    this.myBottomPort.SetSpotLocation(0x20, this.myListGroup, 0x80);
                }
                int num1 = 0;
                GroupEnumerator enumerator1 = this.myListGroup.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj1 = enumerator1.Current;
                    if ((obj1 != null) && (num1 < this.myLeftPorts.Count))
                    {
                        DiagramShape obj2 = (DiagramShape)this.myLeftPorts[num1];
                        if (obj2 != null)
                        {
                            PointF tf1 = obj1.GetSpotLocation(0x100);
                            tf1.X = this.myListGroup.Left;
                            obj2.SetSpotLocation(0x40, tf1);
                        }
                    }
                    if ((obj1 != null) && (num1 < this.myRightPorts.Count))
                    {
                        DiagramShape obj3 = (DiagramShape)this.myRightPorts[num1];
                        if (obj3 != null)
                        {
                            PointF tf2 = obj1.GetSpotLocation(0x40);
                            tf2.X = this.myListGroup.Right;
                            obj3.SetSpotLocation(0x100, tf2);
                        }
                    }
                    num1++;
                }
                base.Initializing = false;
            }
        }

        public virtual void OnItemWidthChanged(float old)
        {
            float single1 = this.ItemWidth;
            GroupEnumerator enumerator1 = this.ListGroup.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj1 = enumerator1.Current;
                DiagramText text1 = obj1 as DiagramText;
                if ((text1 != null) && (single1 > 0f))
                {
                    text1.WrappingWidth = single1;
                }
                if ((obj1 != null) && (single1 > 0f))
                {
                    obj1.Width = single1;
                }
            }
        }

        public override DiagramShape Pick(PointF p, bool selectableOnly)
        {
            if (this.CanView())
            {
                if (!DiagramShape.ContainsRect(this.Bounds, p))
                {
                    return null;
                }
                foreach (DiagramShape obj1 in base.Backwards)
                {
                    DiagramShape obj2 = obj1.Pick(p, selectableOnly);
                    if (obj2 != null)
                    {
                        return obj2;
                    }
                }
                if (!selectableOnly)
                {
                    return this;
                }
                if (this.CanSelect())
                {
                    return this;
                }
            }
            return null;
        }

        public override void Remove(DiagramShape obj)
        {
            int num1 = this.myLeftPorts.IndexOf(obj);
            if (num1 >= 0)
            {
                this.myLeftPorts[num1] = null;
            }
            else
            {
                int num2 = this.myRightPorts.IndexOf(obj);
                if (num2 >= 0)
                {
                    this.myRightPorts[num2] = null;
                }
            }
            base.Remove(obj);
        }

        public virtual void RemoveItem(int i)
        {
            this.myListGroup.RemoveAt(i);
        }

        internal void RemoveOnlyPorts(int i)
        {
            if ((i >= 0) && (i < this.myLeftPorts.Count))
            {
                DiagramShape obj1 = (DiagramShape)this.myLeftPorts[i];
                this.myLeftPorts.RemoveAt(i);
                if (obj1 != null)
                {
                    base.Remove(obj1);
                }
                this.Changed(0xbbb, i, obj1, DiagramShape.NullRect, i, null, DiagramShape.NullRect);
            }
            if ((i >= 0) && (i < this.myRightPorts.Count))
            {
                DiagramShape obj2 = (DiagramShape)this.myRightPorts[i];
                this.myRightPorts.RemoveAt(i);
                if (obj2 != null)
                {
                    base.Remove(obj2);
                }
                this.Changed(0xbbc, i, obj2, DiagramShape.NullRect, i, null, DiagramShape.NullRect);
            }
        }

        public void SetItem(int i, DiagramShape obj)
        {
            this.myListGroup[i] = obj;
        }

        private void setItemWidth(float w, bool undoing)
        {
            float single1 = this.myItemWidth;
            if (single1 != w)
            {
                this.myItemWidth = w;
                this.Changed(0xbc0, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(w));
                if (!undoing)
                {
                    this.OnItemWidthChanged(single1);
                }
            }
        }

        public virtual void SetLeftPort(int i, DiagramShape p)
        {
            DiagramShape obj1 = this.GetLeftPort(i);
            if (obj1 != p)
            {
                if (obj1 != null)
                {
                    if (p != null)
                    {
                        p.Bounds = obj1.Bounds;
                    }
                    base.Remove(obj1);
                }
                this.myLeftPorts[i] = p;
                this.Add(p);
                this.Changed(0xbbd, -(i + 1), obj1, DiagramShape.NullRect, -(i + 1), p, DiagramShape.NullRect);
            }
        }

        public virtual void SetRightPort(int i, DiagramShape p)
        {
            DiagramShape obj1 = this.GetRightPort(i);
            if (obj1 != p)
            {
                if (obj1 != null)
                {
                    if (p != null)
                    {
                        p.Bounds = obj1.Bounds;
                    }
                    base.Remove(obj1);
                }
                this.myRightPorts[i] = p;
                this.Add(p);
                this.Changed(0xbbd, i, obj1, DiagramShape.NullRect, i, p, DiagramShape.NullRect);
            }
        }

        public virtual void SetString(int i, string s)
        {
            DiagramText text1 = this.myListGroup[i] as DiagramText;
            if (text1 != null)
            {
                text1.Text = s;
            }
        }


        [Category("Appearance"), DefaultValue(0x100), Description("How each item is positioned along the X axis.")]
        public int Alignment
        {
            get
            {
                return this.ListGroup.Alignment;
            }
            set
            {
                this.ListGroup.Alignment = value;
            }
        }

        [Category("Appearance"), Description("The pen used to draw an outline for this node.")]
        public Pen BorderPen
        {
            get
            {
                return this.ListGroup.BorderPen;
            }
            set
            {
                this.ListGroup.BorderPen = value;
            }
        }

        public virtual DiagramShape BottomPort
        {
            get
            {
                return this.myBottomPort;
            }
            set
            {
                DiagramShape obj1 = this.myBottomPort;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        base.Remove(obj1);
                    }
                    this.myBottomPort = value;
                    if (value != null)
                    {
                        base.Add(value);
                    }
                    this.Changed(0xbbf, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The margin around the text inside the background at the right side and the bottom"), TypeConverter(typeof(SizeFConverter)), Category("Appearance")]
        public SizeF BottomRightMargin
        {
            get
            {
                return this.ListGroup.BottomRightMargin;
            }
            set
            {
                this.ListGroup.BottomRightMargin = value;
            }
        }

        [Category("Appearance"), DefaultValue((string)null), Description("The brush used to fill the outline of this shape.")]
        public System.Drawing.Brush Brush
        {
            get
            {
                return this.ListGroup.Brush;
            }
            set
            {
                this.ListGroup.Brush = value;
            }
        }

        [TypeConverter(typeof(SizeFConverter)), Description("The maximum radial width and height of each corner"), Category("Appearance")]
        public SizeF Corner
        {
            get
            {
                return this.ListGroup.Corner;
            }
            set
            {
                this.ListGroup.Corner = value;
            }
        }

        public int ItemCount
        {
            get
            {
                return this.myListGroup.Count;
            }
        }

        [DefaultValue(-1), Category("Appearance"), Description("The width for all items, and the wrapping width for all text items")]
        public virtual float ItemWidth
        {
            get
            {
                return this.myItemWidth;
            }
            set
            {
                this.setItemWidth(value, false);
            }
        }

        [Category("Appearance"), Description("The pen used to draw lines separating the items.")]
        public Pen LinePen
        {
            get
            {
                return this.ListGroup.LinePen;
            }
            set
            {
                this.ListGroup.LinePen = value;
            }
        }

        public ListGroup ListGroup
        {
            get
            {
                return this.myListGroup;
            }
        }

        public override bool Shadowed
        {
            get
            {
                return this.ListGroup.Shadowed;
            }
            set
            {
                this.ListGroup.Shadowed = value;
            }
        }

        [Category("Appearance"), DefaultValue(0), Description("The additional vertical distance between items.")]
        public float Spacing
        {
            get
            {
                return this.ListGroup.Spacing;
            }
            set
            {
                this.ListGroup.Spacing = value;
            }
        }

        [Category("Appearance"), Description("The margin around the text inside the background at the left side and the top"), TypeConverter(typeof(SizeFConverter))]
        public SizeF TopLeftMargin
        {
            get
            {
                return this.ListGroup.TopLeftMargin;
            }
            set
            {
                this.ListGroup.TopLeftMargin = value;
            }
        }

        public virtual DiagramShape TopPort
        {
            get
            {
                return this.myTopPort;
            }
            set
            {
                DiagramShape obj1 = this.myTopPort;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        base.Remove(obj1);
                    }
                    this.myTopPort = value;
                    if (value != null)
                    {
                        base.Add(value);
                    }
                    this.Changed(0xbbe, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedBottomPort = 0xbbf;
        public const int ChangedItemWidth = 0xbc0;
        public const int ChangedTopPort = 0xbbe;
        public const int InsertedLeftPort = 0xbb9;
        public const int InsertedRightPort = 0xbba;
        private DiagramShape myBottomPort;
        private float myItemWidth;
        private ArrayList myLeftPorts;
        private ListGroup myListGroup;
        private ArrayList myRightPorts;
        private DiagramShape myTopPort;
        public const int RemovedLeftPort = 0xbbb;
        public const int RemovedRightPort = 0xbbc;
        public const int ReplacedPort = 0xbbd;
    }
}
