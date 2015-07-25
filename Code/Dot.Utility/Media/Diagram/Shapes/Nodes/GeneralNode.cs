using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class GeneralNode : DiagramNode, INodeIconConstraint
    {
        public GeneralNode()
        {
            this.myTopLabel = null;
            this.myBottomLabel = null;
            this.myIcon = null;
            this.myLeftPorts = new ArrayList();
            this.myRightPorts = new ArrayList();
            this.myOrientation = System.Windows.Forms.Orientation.Horizontal;
        }

        public void AddLeftPort(GeneralNodePort p)
        {
            this.InsertLeftPort(this.LeftPortsCount, p);
        }

        public void AddRightPort(GeneralNodePort p)
        {
            this.InsertRightPort(this.RightPortsCount, p);
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x961:
                    {
                        int num1 = e.OldInt;
                        if (num1 >= 0)
                        {
                            if (undo)
                            {
                                if (num1 < this.RightPortsCount)
                                {
                                    this.myRightPorts.RemoveAt(num1);
                                }
                                return;
                            }
                            if (num1 < this.RightPortsCount)
                            {
                                this.myRightPorts.Insert(num1, e.OldValue);
                                return;
                            }
                            this.myRightPorts.Add(e.OldValue);
                            return;
                        }
                        num1 = -num1 - 1;
                        if (!undo)
                        {
                            if (num1 < this.LeftPortsCount)
                            {
                                this.myLeftPorts.Insert(num1, e.OldValue);
                                return;
                            }
                            this.myLeftPorts.Add(e.OldValue);
                            return;
                        }
                        if (num1 < this.LeftPortsCount)
                        {
                            this.myLeftPorts.RemoveAt(num1);
                        }
                        return;
                    }
                case 0x962:
                    {
                        int num2 = e.OldInt;
                        if (num2 >= 0)
                        {
                            if (undo)
                            {
                                if (num2 < this.RightPortsCount)
                                {
                                    this.myRightPorts.Insert(num2, e.OldValue);
                                    return;
                                }
                                this.myRightPorts.Add(e.OldValue);
                                return;
                            }
                            if (num2 < this.RightPortsCount)
                            {
                                this.myRightPorts.RemoveAt(num2);
                            }
                            return;
                        }
                        num2 = -num2 - 1;
                        if (!undo)
                        {
                            if (num2 < this.LeftPortsCount)
                            {
                                this.myLeftPorts.RemoveAt(num2);
                            }
                            return;
                        }
                        if (num2 >= this.LeftPortsCount)
                        {
                            this.myLeftPorts.Add(e.OldValue);
                            return;
                        }
                        this.myLeftPorts.Insert(num2, e.OldValue);
                        return;
                    }
                case 0x963:
                    {
                        int num3 = e.OldInt;
                        if (num3 >= 0)
                        {
                            if (num3 < this.RightPortsCount)
                            {
                                this.myRightPorts[num3] = e.GetValue(undo);
                            }
                            return;
                        }
                        num3 = -num3 - 1;
                        if (num3 < this.LeftPortsCount)
                        {
                            this.myLeftPorts[num3] = e.GetValue(undo);
                        }
                        return;
                    }
                case 0x964:
                    {
                        this.TopLabel = (DiagramText)e.GetValue(undo);
                        return;
                    }
                case 0x965:
                    {
                        this.BottomLabel = (DiagramText)e.GetValue(undo);
                        return;
                    }
                case 0x966:
                    {
                        this.Icon = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0x967:
                    {
                        this.setOrientation((System.Windows.Forms.Orientation)e.GetInt(undo), true);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            GeneralNode node1 = (GeneralNode)newgroup;
            base.CopyChildren(newgroup, env);
            node1.myLeftPorts = new ArrayList();
            node1.myRightPorts = new ArrayList();
            node1.myIcon = (DiagramShape)env[this.myIcon];
            node1.myTopLabel = (DiagramText)env[this.myTopLabel];
            node1.myBottomLabel = (DiagramText)env[this.myBottomLabel];
            for (int num1 = 0; num1 < this.myLeftPorts.Count; num1++)
            {
                GeneralNodePort port1 = (GeneralNodePort)this.myLeftPorts[num1];
                if (port1 != null)
                {
                    GeneralNodePort port2 = (GeneralNodePort)env[port1];
                    if (port2 != null)
                    {
                        node1.myLeftPorts.Add(port2);
                        port2.SideIndex = node1.myLeftPorts.Count - 1;
                        port2.LeftSide = true;
                    }
                }
            }
            for (int num2 = 0; num2 < this.myRightPorts.Count; num2++)
            {
                GeneralNodePort port3 = (GeneralNodePort)this.myRightPorts[num2];
                if (port3 != null)
                {
                    GeneralNodePort port4 = (GeneralNodePort)env[port3];
                    if (port4 != null)
                    {
                        node1.myRightPorts.Add(port4);
                        port4.SideIndex = node1.myRightPorts.Count - 1;
                        port4.LeftSide = false;
                    }
                }
            }
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
                icon1.MaximumIconSize = new SizeF(1000f, 2000f);
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
            icon1.MaximumIconSize = new SizeF(1000f, 2000f);
            icon1.Size = icon1.MinimumIconSize;
            return icon1;
        }

        protected virtual DiagramText CreateLabel(bool top, string text)
        {
            DiagramText text1 = null;
            if (text != null)
            {
                text1 = new DiagramText();
                text1.Text = text;
                text1.Selectable = false;
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    if (top)
                    {
                        text1.Alignment = 0x40;
                    }
                    else
                    {
                        text1.Alignment = 0x100;
                    }
                }
                else if (top)
                {
                    text1.Alignment = 0x80;
                }
                else
                {
                    text1.Alignment = 0x20;
                }
                text1.Editable = true;
                this.Editable = true;
            }
            return text1;
        }

        protected virtual GeneralNodePort CreatePort(bool input)
        {
            GeneralNodePort port1 = new GeneralNodePort();
            port1.LeftSide = input;
            port1.IsValidFrom = !input;
            port1.IsValidTo = input;
            return port1;
        }

        protected virtual GeneralNodePortLabelText CreatePortLabel(bool input)
        {
            return new GeneralNodePortLabelText();
        }

        public virtual GeneralNodePort GetLeftPort(int i)
        {
            if ((i >= 0) && (i < this.myLeftPorts.Count))
            {
                return (GeneralNodePort)this.myLeftPorts[i];
            }
            return null;
        }

        public virtual GeneralNodePort GetRightPort(int i)
        {
            if ((i >= 0) && (i < this.myRightPorts.Count))
            {
                return (GeneralNodePort)this.myRightPorts[i];
            }
            return null;
        }

        public virtual void Initialize(ResourceManager res, string iconname, string top, string bottom, int numinports, int numoutports)
        {
            base.Initializing = true;
            this.Icon = this.CreateIcon(res, iconname);
            this.initializeCommon(top, bottom, numinports, numoutports);
        }

        public virtual void Initialize(ImageList imglist, int imgindex, string top, string bottom, int numinports, int numoutports)
        {
            base.Initializing = true;
            this.Icon = this.CreateIcon(imglist, imgindex);
            this.initializeCommon(top, bottom, numinports, numoutports);
        }

        private void initializeCommon(string top, string bottom, int numinports, int numoutports)
        {
            this.TopLabel = this.CreateLabel(true, top);
            this.BottomLabel = this.CreateLabel(false, bottom);
            for (int num1 = 0; num1 < numinports; num1++)
            {
                GeneralNodePort port1 = this.MakePort(true);
                this.AddLeftPort(port1);
            }
            for (int num2 = 0; num2 < numoutports; num2++)
            {
                GeneralNodePort port2 = this.MakePort(false);
                this.AddRightPort(port2);
            }
            base.PropertiesDelegatedToSelectionObject = true;
            base.Initializing = false;
            this.LayoutChildren(null);
        }

        private void initializePort(GeneralNodePort p)
        {
            if ((p != null) && (p.Parent == null))
            {
                this.Add(p);
                if (p.Label != null)
                {
                    this.Add(p.Label);
                }
            }
        }

        public virtual void InsertLeftPort(int i, GeneralNodePort p)
        {
            if ((p != null) && (i >= 0))
            {
                p.LeftSide = true;
                if (i < this.LeftPortsCount)
                {
                    this.myLeftPorts.Insert(i, p);
                    p.SideIndex = i;
                }
                else
                {
                    this.myLeftPorts.Add(p);
                    i = this.LeftPortsCount - 1;
                    p.SideIndex = i;
                }
                this.initializePort(p);
                this.Changed(0x961, -(i + 1), p, DiagramShape.NullRect, -(i + 1), p, DiagramShape.NullRect);
            }
        }

        public virtual void InsertRightPort(int i, GeneralNodePort p)
        {
            if ((p != null) && (i >= 0))
            {
                p.LeftSide = false;
                if (i < this.RightPortsCount)
                {
                    this.myRightPorts.Insert(i, p);
                    p.SideIndex = i;
                }
                else
                {
                    this.myRightPorts.Add(p);
                    i = this.RightPortsCount - 1;
                    p.SideIndex = i;
                }
                this.initializePort(p);
                this.Changed(0x961, i, p, DiagramShape.NullRect, i, p, DiagramShape.NullRect);
            }
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing)
            {
                base.Initializing = true;
                DiagramShape obj1 = this.Icon;
                DiagramShape obj2 = this.TopLabel;
                DiagramShape obj3 = this.BottomLabel;
                if (this.myOrientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    int num1 = this.LeftPortsCount;
                    float single1 = 0f;
                    float single2 = 0f;
                    for (int num2 = 0; num2 < num1; num2++)
                    {
                        GeneralNodePort port1 = this.GetLeftPort(num2);
                        if (port1.Visible)
                        {
                            single1 += port1.PortAndLabelHeight;
                            single2 = System.Math.Max(single2, port1.PortAndLabelWidth);
                        }
                    }
                    if (obj1 != null)
                    {
                        SizeF ef1 = this.MinimumIconSize;
                        float single3 = System.Math.Max(ef1.Width, obj1.Width);
                        float single4 = System.Math.Max(ef1.Height, obj1.Height);
                        obj1.Bounds = new RectangleF(obj1.Left - ((single3 - obj1.Width) / 2f), obj1.Top - ((single4 - obj1.Height) / 2f), single3, single4);
                    }
                    float single5 = 0f;
                    float single6 = 0f;
                    if (obj1 != null)
                    {
                        single5 = obj1.Left;
                    }
                    else
                    {
                        single5 = base.Left;
                    }
                    if (obj1 != null)
                    {
                        single6 = obj1.Top;
                    }
                    else
                    {
                        single6 = base.Top + ((obj2 != null) ? obj2.Height : 0f);
                    }
                    if ((obj1 != null) && (obj1.Height > single1))
                    {
                        single6 += ((obj1.Height - single1) / 2f);
                    }
                    float single7 = 0f;
                    for (int num3 = 0; num3 < num1; num3++)
                    {
                        GeneralNodePort port2 = this.GetLeftPort(num3);
                        if (port2.Visible)
                        {
                            single7 += (port2.PortAndLabelHeight / 2f);
                            port2.SetSpotLocation(0x40, new PointF(single5, single6 + single7));
                            port2.LayoutLabel();
                            single7 += (port2.PortAndLabelHeight / 2f);
                        }
                    }
                    num1 = this.RightPortsCount;
                    single1 = 0f;
                    for (int num4 = 0; num4 < num1; num4++)
                    {
                        GeneralNodePort port3 = this.GetRightPort(num4);
                        if (port3.Visible)
                        {
                            single1 += port3.PortAndLabelHeight;
                        }
                    }
                    if (obj1 != null)
                    {
                        single5 = obj1.Right;
                    }
                    else
                    {
                        single5 = base.Right;
                    }
                    if (obj1 != null)
                    {
                        single6 = obj1.Top;
                    }
                    else
                    {
                        single6 = base.Top + ((obj2 != null) ? obj2.Height : 0f);
                    }
                    if ((obj1 != null) && (obj1.Height > single1))
                    {
                        single6 += ((obj1.Height - single1) / 2f);
                    }
                    single7 = 0f;
                    for (int num5 = 0; num5 < num1; num5++)
                    {
                        GeneralNodePort port4 = this.GetRightPort(num5);
                        if (port4.Visible)
                        {
                            single7 += (port4.PortAndLabelHeight / 2f);
                            port4.SetSpotLocation(0x100, new PointF(single5, single6 + single7));
                            port4.LayoutLabel();
                            single7 += (port4.PortAndLabelHeight / 2f);
                        }
                    }
                    if (obj2 != null)
                    {
                        if (obj1 != null)
                        {
                            obj2.SetSpotLocation(0x80, obj1, 0x20);
                        }
                        else
                        {
                            obj2.SetSpotLocation(0x20, this, 0x20);
                        }
                    }
                    if (obj3 != null)
                    {
                        if (obj1 != null)
                        {
                            obj3.SetSpotLocation(0x20, obj1, 0x80);
                        }
                        else
                        {
                            obj3.SetSpotLocation(0x80, this, 0x80);
                        }
                    }
                }
                else
                {
                    int num6 = this.LeftPortsCount;
                    float single8 = 0f;
                    float single9 = 0f;
                    for (int num7 = 0; num7 < num6; num7++)
                    {
                        GeneralNodePort port5 = this.GetLeftPort(num7);
                        if (port5.Visible)
                        {
                            single8 += port5.PortAndLabelWidth;
                            single9 = System.Math.Max(single9, port5.PortAndLabelHeight);
                        }
                    }
                    if (obj1 != null)
                    {
                        SizeF ef2 = this.MinimumIconSize;
                        float single10 = System.Math.Max(ef2.Width, obj1.Width);
                        float single11 = System.Math.Max(ef2.Height, obj1.Height);
                        obj1.Bounds = new RectangleF(obj1.Left - ((single10 - obj1.Width) / 2f), obj1.Top - ((single11 - obj1.Height) / 2f), single10, single11);
                    }
                    float single12 = 0f;
                    float single13 = 0f;
                    if (obj1 != null)
                    {
                        single12 = obj1.Left;
                    }
                    else
                    {
                        single12 = base.Left + ((obj2 != null) ? obj2.Width : 0f);
                    }
                    if (obj1 != null)
                    {
                        single13 = obj1.Top;
                    }
                    else
                    {
                        single13 = base.Top;
                    }
                    if ((obj1 != null) && (obj1.Width > single8))
                    {
                        single12 += ((obj1.Width - single8) / 2f);
                    }
                    float single14 = 0f;
                    for (int num8 = 0; num8 < num6; num8++)
                    {
                        GeneralNodePort port6 = this.GetLeftPort(num8);
                        if (port6.Visible)
                        {
                            single14 += (port6.PortAndLabelWidth / 2f);
                            port6.SetSpotLocation(0x80, new PointF(single12 + single14, single13));
                            port6.LayoutLabel();
                            single14 += (port6.PortAndLabelWidth / 2f);
                        }
                    }
                    num6 = this.RightPortsCount;
                    single8 = 0f;
                    for (int num9 = 0; num9 < num6; num9++)
                    {
                        GeneralNodePort port7 = this.GetRightPort(num9);
                        if (port7.Visible)
                        {
                            single8 += port7.PortAndLabelWidth;
                        }
                    }
                    if (obj1 != null)
                    {
                        single12 = obj1.Left;
                    }
                    else
                    {
                        single12 = base.Left + ((obj2 != null) ? obj2.Width : 0f);
                    }
                    if (obj1 != null)
                    {
                        single13 = obj1.Bottom;
                    }
                    else
                    {
                        single13 = base.Bottom;
                    }
                    if ((obj1 != null) && (obj1.Width > single8))
                    {
                        single12 += ((obj1.Width - single8) / 2f);
                    }
                    single14 = 0f;
                    for (int num10 = 0; num10 < num6; num10++)
                    {
                        GeneralNodePort port8 = this.GetRightPort(num10);
                        if (port8.Visible)
                        {
                            single14 += (port8.PortAndLabelWidth / 2f);
                            port8.SetSpotLocation(0x20, new PointF(single12 + single14, single13));
                            port8.LayoutLabel();
                            single14 += (port8.PortAndLabelWidth / 2f);
                        }
                    }
                    if (obj2 != null)
                    {
                        if (obj1 != null)
                        {
                            obj2.SetSpotLocation(0x40, obj1, 0x100);
                        }
                        else
                        {
                            obj2.SetSpotLocation(0x100, this, 0x100);
                        }
                    }
                    if (obj3 != null)
                    {
                        if (obj1 != null)
                        {
                            obj3.SetSpotLocation(0x100, obj1, 0x40);
                        }
                        else
                        {
                            obj3.SetSpotLocation(0x40, this, 0x40);
                        }
                    }
                }
                base.Initializing = false;
            }
        }

        public virtual GeneralNodePort MakePort(bool input)
        {
            GeneralNodePort port1 = this.CreatePort(input);
            if (port1 != null)
            {
                PointF tf1;
                GeneralNodePortLabelText label1 = this.CreatePortLabel(input);
                port1.Label = label1;
                if (label1 != null)
                {
                    label1.Port = port1;
                }
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    port1.ToSpot = 0x20;
                    port1.FromSpot = 0x80;
                }
                else
                {
                    port1.ToSpot = 0x100;
                    port1.FromSpot = 0x40;
                }
                if (input)
                {
                    port1.Name = this.LeftPortsCount.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    port1.Name = this.RightPortsCount.ToString(CultureInfo.CurrentCulture);
                }
                if (this.Icon != null)
                {
                    tf1 = this.Icon.Position;
                    if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                    {
                        if (input)
                        {
                            tf1.Y -= port1.Height;
                        }
                        else
                        {
                            tf1.Y = this.Icon.Bottom;
                        }
                    }
                    else if (input)
                    {
                        tf1.X -= port1.Width;
                    }
                    else
                    {
                        tf1.X = this.Icon.Right;
                    }
                }
                else
                {
                    tf1 = base.Position;
                }
                port1.Position = tf1;
                if (label1 != null)
                {
                    label1.Position = tf1;
                }
            }
            return port1;
        }

        public virtual void OnOrientationChanged(System.Windows.Forms.Orientation old)
        {
            int num1 = this.LeftPortsCount;
            for (int num2 = 0; num2 < num1; num2++)
            {
                GeneralNodePort port1 = this.GetLeftPort(num2);
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    port1.ToSpot = 0x20;
                    port1.FromSpot = 0x20;
                }
                else
                {
                    port1.ToSpot = 0x100;
                    port1.FromSpot = 0x100;
                }
            }
            num1 = this.RightPortsCount;
            for (int num3 = 0; num3 < num1; num3++)
            {
                GeneralNodePort port2 = this.GetRightPort(num3);
                if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    port2.ToSpot = 0x80;
                    port2.FromSpot = 0x80;
                }
                else
                {
                    port2.ToSpot = 0x40;
                    port2.FromSpot = 0x40;
                }
            }
            this.LayoutChildren(null);
        }

        public override void Remove(DiagramShape obj)
        {
            GeneralNodePort port1 = obj as GeneralNodePort;
            if (port1 != null)
            {
                int num1 = this.myLeftPorts.IndexOf(port1);
                if (num1 >= 0)
                {
                    this.myLeftPorts.RemoveAt(num1);
                    if (port1.Label != null)
                    {
                        this.Remove(port1.Label);
                    }
                    base.Remove(port1);
                    port1.SideIndex = -1;
                    this.Changed(0x962, -(num1 + 1), port1, DiagramShape.NullRect, -(num1 + 1), port1, DiagramShape.NullRect);
                    return;
                }
                int num2 = this.myRightPorts.IndexOf(port1);
                if (num2 >= 0)
                {
                    this.myRightPorts.RemoveAt(num2);
                    if (port1.Label != null)
                    {
                        this.Remove(port1.Label);
                    }
                    base.Remove(port1);
                    port1.SideIndex = -1;
                    this.Changed(0x962, num2, port1, DiagramShape.NullRect, num2, port1, DiagramShape.NullRect);
                    return;
                }
            }
            base.Remove(obj);
            if (obj == this.myTopLabel)
            {
                this.myTopLabel = null;
            }
            else if (obj == this.myBottomLabel)
            {
                this.myBottomLabel = null;
            }
            else if (obj == this.myIcon)
            {
                this.myIcon = null;
            }
        }

        public virtual void RemoveLeftPort(int i)
        {
            if ((i >= 0) && (i < this.LeftPortsCount))
            {
                GeneralNodePort port1 = (GeneralNodePort)this.myLeftPorts[i];
                this.Remove(port1);
            }
        }

        public virtual void RemoveRightPort(int i)
        {
            if ((i >= 0) && (i < this.RightPortsCount))
            {
                GeneralNodePort port1 = (GeneralNodePort)this.myRightPorts[i];
                this.Remove(port1);
            }
        }

        public virtual void SetLeftPort(int i, GeneralNodePort p)
        {
            GeneralNodePort port1 = this.GetLeftPort(i);
            if (port1 != p)
            {
                if (port1 != null)
                {
                    if (p != null)
                    {
                        p.Bounds = port1.Bounds;
                    }
                    if (port1.Label != null)
                    {
                        this.Remove(port1.Label);
                    }
                    base.Remove(port1);
                    port1.SideIndex = -1;
                }
                this.myLeftPorts[i] = p;
                p.LeftSide = true;
                p.SideIndex = i;
                this.initializePort(p);
                this.Changed(0x963, -(i + 1), port1, DiagramShape.NullRect, -(i + 1), p, DiagramShape.NullRect);
            }
        }

        private void setOrientation(System.Windows.Forms.Orientation o, bool undoing)
        {
            System.Windows.Forms.Orientation orientation1 = this.myOrientation;
            if (orientation1 != o)
            {
                this.myOrientation = o;
                this.Changed(0x967, (int)orientation1, null, DiagramShape.NullRect, (int)o, null, DiagramShape.NullRect);
                this.OnOrientationChanged(orientation1);
            }
        }

        public virtual void SetRightPort(int i, GeneralNodePort p)
        {
            GeneralNodePort port1 = this.GetRightPort(i);
            if (port1 != p)
            {
                if (port1 != null)
                {
                    if (p != null)
                    {
                        p.Bounds = port1.Bounds;
                    }
                    if (port1.Label != null)
                    {
                        this.Remove(port1.Label);
                    }
                    base.Remove(port1);
                    port1.SideIndex = -1;
                }
                this.myRightPorts[i] = p;
                p.LeftSide = false;
                p.SideIndex = i;
                this.initializePort(p);
                this.Changed(0x963, i, port1, DiagramShape.NullRect, i, p, DiagramShape.NullRect);
            }
        }


        public virtual DiagramText BottomLabel
        {
            get
            {
                return this.myBottomLabel;
            }
            set
            {
                DiagramText text1 = this.myBottomLabel;
                if (text1 != value)
                {
                    if (text1 != null)
                    {
                        this.Remove(text1);
                    }
                    this.myBottomLabel = value;
                    if (value != null)
                    {
                        this.Add(value);
                    }
                    this.Changed(0x965, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
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
                    this.Changed(0x966, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public virtual DiagramImage Image
        {
            get
            {
                return (this.Icon as DiagramImage);
            }
        }

        public override DiagramText Label
        {
            get
            {
                if (this.BottomLabel != null)
                {
                    return this.BottomLabel;
                }
                if (this.TopLabel != null)
                {
                    return this.TopLabel;
                }
                return null;
            }
            set
            {
                if (this.BottomLabel != null)
                {
                    this.BottomLabel = value;
                }
                else if (this.TopLabel != null)
                {
                    this.TopLabel = value;
                }
                else
                {
                    this.BottomLabel = value;
                }
            }
        }

        public int LeftPortsCount
        {
            get
            {
                return this.myLeftPorts.Count;
            }
        }

        [Category("Appearance"), Description("The maximum size for the icon"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF MaximumIconSize
        {
            get
            {
                NodeIcon icon1 = this.Icon as NodeIcon;
                if (icon1 != null)
                {
                    return icon1.MaximumIconSize;
                }
                return new SizeF(1000f, 2000f);
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

        [Description("The minimum size for the icon"), Category("Appearance"), TypeConverter(typeof(SizeFConverter))]
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
                    int num1 = this.LeftPortsCount;
                    float single3 = 0f;
                    for (int num2 = 0; num2 < num1; num2++)
                    {
                        GeneralNodePort port1 = this.GetLeftPort(num2);
                        if ((port1 != null) && port1.Visible)
                        {
                            single3 += port1.PortAndLabelHeight;
                        }
                    }
                    single2 = System.Math.Max(single2, single3);
                    num1 = this.RightPortsCount;
                    single3 = 0f;
                    for (int num3 = 0; num3 < num1; num3++)
                    {
                        GeneralNodePort port2 = this.GetRightPort(num3);
                        if ((port2 != null) && port2.Visible)
                        {
                            single3 += port2.PortAndLabelHeight;
                        }
                    }
                    single2 = System.Math.Max(single2, single3);
                    return new SizeF(single1, single2);
                }
                float single4 = 20f;
                float single5 = 20f;
                NodeIcon icon2 = this.Icon as NodeIcon;
                if (icon2 != null)
                {
                    single4 = icon2.MinimumIconSize.Width;
                    single5 = icon2.MinimumIconSize.Height;
                }
                int num4 = this.LeftPortsCount;
                float single6 = 0f;
                for (int num5 = 0; num5 < num4; num5++)
                {
                    GeneralNodePort port3 = this.GetLeftPort(num5);
                    if ((port3 != null) && port3.Visible)
                    {
                        single6 += port3.PortAndLabelWidth;
                    }
                }
                single4 = System.Math.Max(single4, single6);
                num4 = this.RightPortsCount;
                single6 = 0f;
                for (int num6 = 0; num6 < num4; num6++)
                {
                    GeneralNodePort port4 = this.GetRightPort(num6);
                    if ((port4 != null) && port4.Visible)
                    {
                        single6 += port4.PortAndLabelWidth;
                    }
                }
                single4 = System.Math.Max(single4, single6);
                return new SizeF(single4, single5);
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

        [Category("Appearance"), Description("The general orientation of the node and how links connect to it"), DefaultValue(0)]
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

        public int RightPortsCount
        {
            get
            {
                return this.myRightPorts.Count;
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

        public virtual DiagramText TopLabel
        {
            get
            {
                return this.myTopLabel;
            }
            set
            {
                DiagramText text1 = this.myTopLabel;
                if (text1 != value)
                {
                    if (text1 != null)
                    {
                        this.Remove(text1);
                    }
                    this.myTopLabel = value;
                    if (value != null)
                    {
                        this.Add(value);
                    }
                    this.Changed(0x964, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedBottomLabel = 0x965;
        public const int ChangedIcon = 0x966;
        public const int ChangedOrientation = 0x967;
        public const int ChangedTopLabel = 0x964;
        public const int InsertedPort = 0x961;
        private DiagramText myBottomLabel;
        private DiagramShape myIcon;
        private ArrayList myLeftPorts;
        private System.Windows.Forms.Orientation myOrientation;
        private ArrayList myRightPorts;
        private DiagramText myTopLabel;
        public const int RemovedPort = 0x962;
        public const int ReplacedPort = 0x963;
    }
}
