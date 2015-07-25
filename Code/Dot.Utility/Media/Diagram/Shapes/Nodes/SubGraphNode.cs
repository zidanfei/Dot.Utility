using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class SubGraphNode : DiagramNode
    {
        public SubGraphNode()
        {
            this.myState = SubGraphNodeState.Expanded;
            this.myHandle = null;
            this.myLabel = null;
            this.myPort = null;
            this.myCollapsedObject = null;
            this.myBackgroundColor = Color.LightBlue;
            this.myOpacity = 25f;
            this.myLabelSpot = 0x20;
            this.myCollapsedLabelSpot = 1;
            this.myCorner = new SizeF(0f, 0f);
            this.myCollapsedCorner = new SizeF(0f, 0f);
            this.myTopLeftMargin = new SizeF(4f, 4f);
            this.myBottomRightMargin = new SizeF(4f, 4f);
            this.myCollapsedTopLeftMargin = new SizeF(0f, 0f);
            this.myCollapsedBottomRightMargin = new SizeF(0f, 0f);
            this.myBorderPenInfo = DiagramGraph.GetPenInfo(null);
            this.myBoundsHashtable = new Hashtable();
            this.myPathsHashtable = new Hashtable();
            base.InternalFlags |= 0x2020000;
            base.InternalFlags &= -17;
            this.myHandle = this.CreateHandle();
            this.Add(this.myHandle);
            this.myCollapsedObject = this.CreateCollapsedObject();
            this.Add(this.myCollapsedObject);
            this.myLabel = this.CreateLabel();
            this.Add(this.myLabel);
            this.myPort = this.CreatePort();
            this.InsertBefore(null, this.myPort);
            base.Initializing = false;
            this.LayoutChildren(null);
        }

        public override void Add(DiagramShape obj)
        {
            if ((this.Handle != null) && (this.Count >= 1))
            {
                this.InsertBefore(this.Handle, obj);
            }
            else
            {
                base.Add(obj);
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0xa8e:
                    {
                        this.Label = (DiagramText)e.GetValue(undo);
                        return;
                    }
                case 0xa8f:
                    {
                        this.Collapsible = (bool)e.GetValue(undo);
                        return;
                    }
                case 0xa90:
                    {
                        this.BackgroundColor = (Color)e.GetValue(undo);
                        return;
                    }
                case 0xa91:
                    {
                        this.Opacity = e.GetFloat(undo);
                        return;
                    }
                case 0xa92:
                    {
                        this.setLabelSpot(e.GetInt(undo), true);
                        return;
                    }
                case 0xa93:
                    {
                        this.setTopLeftMargin(e.GetSize(undo), true);
                        return;
                    }
                case 0xa94:
                    {
                        object obj1 = e.GetValue(undo);
                        if (!(obj1 is Pen))
                        {
                            if (obj1 is DiagramGraph.GoPenInfo)
                            {
                                this.BorderPen = ((DiagramGraph.GoPenInfo)obj1).GetPen();
                            }
                            return;
                        }
                        this.BorderPen = (Pen)obj1;
                        return;
                    }
                case 0xa95:
                    {
                        this.PickableBackground = (bool)e.GetValue(undo);
                        return;
                    }
                case 2710:
                    {
                        this.Corner = e.GetSize(undo);
                        return;
                    }
                case 0xa97:
                    {
                        this.Port = (DiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0xa98:
                    {
                        this.setBottomRightMargin(e.GetSize(undo), true);
                        return;
                    }
                case 0xa99:
                    {
                        this.setCollapsedTopLeftMargin(e.GetSize(undo), true);
                        return;
                    }
                case 0xa9a:
                    {
                        this.setCollapsedBottomRightMargin(e.GetSize(undo), true);
                        return;
                    }
                case 0xa9b:
                    {
                        this.CollapsedCorner = e.GetSize(undo);
                        return;
                    }
                case 0xa9c:
                    {
                        this.setCollapsedLabelSpot(e.GetInt(undo), true);
                        return;
                    }
                case 0xa9d:
                    {
                        this.CollapsedObject = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0xa9e:
                    {
                        SubGraphNodeState state1 = (SubGraphNodeState)e.GetInt(undo);
                        this.State = state1;
                        base.Initializing = (state1 == SubGraphNodeState.Collapsing) || (state1 == SubGraphNodeState.Expanding);
                        if (base.Initializing)
                        {
                            ChangedEventArgs args1 = new ChangedEventArgs(e);
                            args1.SubHint = 0x3e9;
                            base.ChangeValue(args1, undo);
                        }
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        public virtual void Collapse()
        {
            if ((this.State == SubGraphNodeState.Expanded) && this.Collapsible)
            {
                this.State = SubGraphNodeState.Collapsing;
                base.Initializing = true;
                this.PrepareCollapse();
                SizeF ef1 = this.ComputeCollapsedSize(true);
                RectangleF ef2 = this.ComputeCollapsedRectangle(ef1);
                GroupEnumerator enumerator1 = base.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj1 = enumerator1.Current;
                    this.SaveChildBounds(obj1, ef2);
                }
                enumerator1 = base.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj2 = enumerator1.Current;
                    this.CollapseChild(obj2, ef2);
                }
                this.FinishCollapse(ef2);
                base.Initializing = false;
                base.InvalidBounds = true;
                this.State = SubGraphNodeState.Collapsed;
                this.LayoutChildren(null);
            }
        }

        protected virtual void CollapseChild(DiagramShape child, RectangleF sgrect)
        {
            if ((((child != this.Handle) && (child != this.Label)) && (child != this.Port)) && (child != this.CollapsedObject))
            {
                if (!(child is IDiagramLine))
                {
                    PointF tf1 = new PointF(sgrect.X + (sgrect.Width / 2f), sgrect.Y + (sgrect.Height / 2f));
                    SubGraphNode graph1 = child as SubGraphNode;
                    if (graph1 != null)
                    {
                        SizeF ef1 = graph1.ComputeCollapsedSize(false);
                        RectangleF ef2 = graph1.ComputeCollapsedRectangle(ef1);
                        PointF tf2 = new PointF(ef2.X + (ef2.Width / 2f), ef2.Y + (ef2.Height / 2f));
                        SizeF ef3 = DiagramTool.SubtractPoints(tf1, tf2);
                        graph1.Position = new PointF(graph1.Left + ef3.Width, graph1.Top + ef3.Height);
                    }
                    else
                    {
                        child.Center = tf1;
                    }
                }
                child.Visible = false;
            }
        }

        protected override RectangleF ComputeBounds()
        {
            RectangleF ef1 = this.Bounds;
            if (!base.Initializing)
            {
                SizeF ef4;
                SizeF ef5;
                bool flag1 = false;
                GroupEnumerator enumerator1 = base.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj1 = enumerator1.Current;
                    if (!this.ComputeBoundsSkip(obj1))
                    {
                        RectangleF ef2;
                        SubGraphNode graph1 = obj1 as SubGraphNode;
                        if ((graph1 != null) && !graph1.CanView())
                        {
                            SizeF ef3 = graph1.ComputeCollapsedSize(false);
                            ef2 = graph1.ComputeCollapsedRectangle(ef3);
                        }
                        else
                        {
                            ef2 = obj1.Bounds;
                        }
                        if (!flag1)
                        {
                            ef1 = ef2;
                            flag1 = true;
                            continue;
                        }
                        ef1 = DiagramShape.UnionRect(ef1, ef2);
                    }
                }
                if (!flag1)
                {
                    return ef1;
                }
                if (this.IsExpanded)
                {
                    ef4 = this.TopLeftMargin;
                    ef5 = this.BottomRightMargin;
                }
                else
                {
                    ef4 = this.CollapsedTopLeftMargin;
                    ef5 = this.CollapsedBottomRightMargin;
                }
                ef1.X -= ef4.Width;
                ef1.Y -= ef4.Height;
                ef1.Width += (ef4.Width + ef5.Width);
                ef1.Height += (ef4.Height + ef5.Height);
            }
            return ef1;
        }

        protected virtual bool ComputeBoundsSkip(DiagramShape child)
        {
            if (child == this.Handle)
            {
                return true;
            }
            if (child == this.Label)
            {
                if ((base.InternalFlags & 0x1000000) == 0)
                {
                    return !child.CanView();
                }
                return true;
            }
            if (child == this.Port)
            {
                return true;
            }
            if (child == this.CollapsedObject)
            {
                return !child.CanView();
            }
            IDiagramLine link1 = child as IDiagramLine;
            if (link1 != null)
            {
                if (!child.CanView())
                {
                    return true;
                }
                if ((this.Port != null) && ((link1.FromPort == this.Port) || (link1.ToPort == this.Port)))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual RectangleF ComputeCollapsedRectangle(SizeF s)
        {
            PointF tf1 = this.ComputeReferencePoint();
            return new RectangleF(tf1.X, tf1.Y, s.Width, s.Height);
        }

        public virtual SizeF ComputeCollapsedSize(bool visible)
        {
            SizeF ef1 = new SizeF(0f, 0f);
            if (visible && (this.CollapsedObject != null))
            {
                ef1 = this.CollapsedObject.Size;
            }
            GroupEnumerator enumerator1 = base.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj1 = enumerator1.Current;
                if (!this.ComputeCollapsedSizeSkip(obj1))
                {
                    SizeF ef2 = obj1.Size;
                    SubGraphNode graph1 = obj1 as SubGraphNode;
                    if (graph1 != null)
                    {
                        ef2 = graph1.ComputeCollapsedSize(false);
                    }
                    ef1.Width = System.Math.Max(ef1.Width, ef2.Width);
                    ef1.Height = System.Math.Max(ef1.Height, ef2.Height);
                }
            }
            return ef1;
        }

        protected virtual bool ComputeCollapsedSizeSkip(DiagramShape child)
        {
            if (child == this.Handle)
            {
                return true;
            }
            if (child == this.Label)
            {
                return true;
            }
            if (child == this.Port)
            {
                return true;
            }
            if (child == this.CollapsedObject)
            {
                return true;
            }
            if (child is IDiagramLine)
            {
                return true;
            }
            return false;
        }

        protected virtual PointF ComputeReferencePoint()
        {
            if (this.Handle != null)
            {
                return this.Handle.Position;
            }
            return base.Position;
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            SubGraphNode graph1 = (SubGraphNode)newgroup;
            graph1.myHandle = null;
            graph1.myLabel = null;
            graph1.myPort = null;
            graph1.myCollapsedObject = null;
            GroupEnumerator enumerator2 = base.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                DiagramShape obj1 = enumerator2.Current;
                env.Copy(obj1);
            }
            enumerator2 = base.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                DiagramShape obj2 = enumerator2.Current;
                DiagramShape obj3 = (DiagramShape)env[obj2];
                graph1.Add(obj3);
                if (obj2 == this.myHandle)
                {
                    graph1.myHandle = (SubGraphHandle)obj3;
                    continue;
                }
                if (obj2 == this.myLabel)
                {
                    graph1.myLabel = (DiagramText)obj3;
                    continue;
                }
                if (obj2 == this.myPort)
                {
                    graph1.myPort = (DiagramPort)obj3;
                    continue;
                }
                if (obj2 == this.myCollapsedObject)
                {
                    graph1.myCollapsedObject = obj3;
                }
            }
            graph1.myBoundsHashtable = new Hashtable();
            IDictionaryEnumerator enumerator1 = this.myBoundsHashtable.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj4 = (DiagramShape)enumerator1.Key;
                DiagramShape obj5 = (DiagramShape)env[obj4];
                if (obj5 != null)
                {
                    RectangleF ef1 = (RectangleF)enumerator1.Value;
                    graph1.myBoundsHashtable[obj5] = ef1;
                }
            }
            graph1.myPathsHashtable = new Hashtable();
            enumerator1 = this.myPathsHashtable.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                StrokeGraph stroke1 = (StrokeGraph)enumerator1.Key;
                StrokeGraph stroke2 = (StrokeGraph)env[stroke1];
                if (stroke2 != null)
                {
                    PointF[] tfArray1 = (PointF[])enumerator1.Value;
                    graph1.myPathsHashtable[stroke2] = tfArray1.Clone();
                }
            }
        }

        protected virtual DiagramShape CreateCollapsedObject()
        {
            return null;
        }

        protected virtual Shapes.SubGraphHandle CreateHandle()
        {
            return new SubGraphHandle();
        }

        protected virtual DiagramText CreateLabel()
        {
            DiagramText text1 = new DiagramText();
            text1.Selectable = false;
            text1.Alignment = 0x80;
            text1.Wrapping = true;
            text1.Bold = true;
            text1.Editable = true;
            return text1;
        }

        protected virtual DiagramPort CreatePort()
        {
            return null;
        }

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            SizeF ef2;
            SizeF ef3;
            RectangleF ef4;
            RectangleF ef1 = this.ComputeBounds();
            if (this.IsExpanded)
            {
                ef2 = this.TopLeftMargin;
                ef3 = this.BottomRightMargin;
            }
            else
            {
                ef2 = this.CollapsedTopLeftMargin;
                ef3 = this.CollapsedBottomRightMargin;
            }
            ef1.X += ef2.Width;
            ef1.Y += ef2.Height;
            ef1.Width -= (ef2.Width + ef3.Width);
            ef1.Height -= (ef2.Height + ef3.Height);
            if (evttype == InputState.Cancel)
            {
                ef4 = origRect;
            }
            else
            {
                ef4 = this.ComputeResize(origRect, newPoint, whichHandle, new SizeF(ef1.Width, ef1.Height), max, true);
            }
            if (this.ResizesRealtime || (evttype == InputState.Cancel))
            {
                SizeF ef5 = new SizeF(System.Math.Max((float)0f, (float)(ef4.Right - ef1.Right)), System.Math.Max((float)0f, (float)(ef4.Bottom - ef1.Bottom)));
                SizeF ef6 = new SizeF(System.Math.Max((float)0f, (float)(ef1.X - ef4.X)), System.Math.Max((float)0f, (float)(ef1.Y - ef4.Y)));
                if (this.IsExpanded)
                {
                    this.BottomRightMargin = ef5;
                    this.TopLeftMargin = ef6;
                }
                else
                {
                    this.CollapsedBottomRightMargin = ef5;
                    this.CollapsedTopLeftMargin = ef6;
                }
            }
            else
            {
                Rectangle rectangle1 = view.ConvertDocToView(ef4);
                view.DrawXorBox(rectangle1, evttype != InputState.Finish);
                if (evttype == InputState.Finish)
                {
                    SizeF ef7 = new SizeF(System.Math.Max((float)0f, (float)(ef4.Right - ef1.Right)), System.Math.Max((float)0f, (float)(ef4.Bottom - ef1.Bottom)));
                    SizeF ef8 = new SizeF(System.Math.Max((float)0f, (float)(ef1.X - ef4.X)), System.Math.Max((float)0f, (float)(ef1.Y - ef4.Y)));
                    if (this.IsExpanded)
                    {
                        this.BottomRightMargin = ef7;
                        this.TopLeftMargin = ef8;
                    }
                    else
                    {
                        this.CollapsedBottomRightMargin = ef7;
                        this.CollapsedTopLeftMargin = ef8;
                    }
                }
            }
        }

        public virtual void Expand()
        {
            if ((this.State == SubGraphNodeState.Collapsed) && this.Collapsible)
            {
                this.State = SubGraphNodeState.Expanding;
                base.Initializing = true;
                this.PrepareExpand();
                PointF tf1 = this.ComputeReferencePoint();
                GroupEnumerator enumerator1 = base.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj1 = enumerator1.Current;
                    if (!(obj1 is Shapes.LineGraph) && !(obj1 is TextLine))
                    {
                        this.ExpandChild(obj1, tf1);
                    }
                }
                enumerator1 = base.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj2 = enumerator1.Current;
                    if ((obj2 is Shapes.LineGraph) || (obj2 is TextLine))
                    {
                        this.ExpandChild(obj2, tf1);
                    }
                }
                this.FinishExpand(tf1);
                base.Initializing = false;
                base.InvalidBounds = true;
                this.State = SubGraphNodeState.Expanded;
                this.LayoutChildren(null);
            }
        }

        public virtual void ExpandAll()
        {
            this.Expand();
            GroupEnumerator enumerator1 = base.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                SubGraphNode graph1 = enumerator1.Current as SubGraphNode;
                if (graph1 != null)
                {
                    graph1.ExpandAll();
                }
            }
        }

        protected virtual void ExpandChild(DiagramShape child, PointF hpos)
        {
            if (child != this.CollapsedObject)
            {
                if ((child is Shapes.LineGraph) || (child is TextLine))
                {
                    StrokeGraph stroke1 = null;
                    if (child is Shapes.LineGraph)
                    {
                        stroke1 = (Shapes.LineGraph)child;
                    }
                    else
                    {
                        stroke1 = ((TextLine)child).RealLink;
                    }
                    if (this.SavedPaths.ContainsKey(stroke1))
                    {
                        PointF[] tfArray1 = (PointF[])this.SavedPaths[stroke1];
                        for (int num1 = 0; num1 < tfArray1.Length; num1++)
                        {
                            PointF tf1 = tfArray1[num1];
                            tf1.X += hpos.X;
                            tf1.Y += hpos.Y;
                            tfArray1[num1] = tf1;
                        }
                        stroke1.SetPoints(tfArray1);
                    }
                }
                else if (this.SavedBounds.ContainsKey(child))
                {
                    RectangleF ef1 = (RectangleF)this.SavedBounds[child];
                    child.Bounds = new RectangleF(hpos.X + ef1.X, hpos.Y + ef1.Y, ef1.Width, ef1.Height);
                }
                child.Visible = true;
            }
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            if (this.Shadowed)
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

        public static SubGraphNode FindParentSubGraph(DiagramShape obj)
        {
            if (obj != null)
            {
                for (DiagramShape obj1 = obj.Parent; obj1 != null; obj1 = obj1.Parent)
                {
                    if (obj1 is SubGraphNode)
                    {
                        return (SubGraphNode)obj1;
                    }
                }
            }
            return null;
        }

        protected virtual void FinishCollapse(RectangleF sgrect)
        {
            if (this.CollapsedObject != null)
            {
                this.CollapsedObject.Visible = true;
                this.CollapsedObject.Printable = true;
            }
            if (this.Label != null)
            {
                this.Label.Position = new PointF(sgrect.X, sgrect.Y);
            }
            if (this.Resizable)
            {
                base.InternalFlags |= 0x8000000;
                this.Resizable = false;
            }
        }

        protected virtual void FinishExpand(PointF hpos)
        {
            if (this.CollapsedObject != null)
            {
                this.CollapsedObject.Visible = false;
                this.CollapsedObject.Printable = false;
            }
            if ((base.InternalFlags & 0x8000000) != 0)
            {
                base.InternalFlags &= -134217729;
                this.Resizable = true;
            }
            this.SavedBounds.Clear();
            this.SavedPaths.Clear();
        }

        private NodeLinkEnumerator GetLinkEnumerator(DiagramNode.Search s)
        {
            return new NodeLinkEnumerator(this, s);
        }

        private NodeNodeEnumerator GetNodeEnumerator(DiagramNode.Search s)
        {
            return new NodeNodeEnumerator(this, s);
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if ((!base.Initializing && ((childchanged != this.Handle) || (childchanged == null))) && ((childchanged != this.Port) || (childchanged == null)))
            {
                this.LayoutLabel();
                this.LayoutHandle();
                this.LayoutPort();
            }
        }

        public virtual void LayoutHandle()
        {
            if (this.IsExpanded)
            {
                SubGraphHandle handle1 = this.Handle;
                if ((handle1 != null) && handle1.CanView())
                {
                    SizeF ef2;
                    RectangleF ef1 = this.ComputeBounds();
                    if (this.IsExpanded)
                    {
                        ef2 = this.TopLeftMargin;
                    }
                    else
                    {
                        ef2 = this.CollapsedTopLeftMargin;
                    }
                    handle1.Position = new PointF(ef1.X + ef2.Width, ef1.Y + ef2.Height);
                }
            }
        }

        public virtual void LayoutLabel()
        {
            DiagramText text1 = this.Label;
            if ((text1 != null) && text1.CanView())
            {
                SizeF ef2;
                SizeF ef3;
                int num1;
                base.InternalFlags |= 0x1000000;
                RectangleF ef1 = this.ComputeBounds();
                base.InternalFlags &= -16777217;
                if (this.IsExpanded)
                {
                    ef2 = this.TopLeftMargin;
                    ef3 = this.BottomRightMargin;
                }
                else
                {
                    ef2 = this.CollapsedTopLeftMargin;
                    ef3 = this.CollapsedBottomRightMargin;
                }
                ef1.X += ef2.Width;
                ef1.Y += ef2.Height;
                ef1.Width -= (ef2.Width + ef3.Width);
                ef1.Height -= (ef2.Height + ef3.Height);
                if (!this.IsExpanded)
                {
                    num1 = this.CollapsedLabelSpot;
                    SizeF ef4 = this.ComputeCollapsedSize(true);
                    RectangleF ef5 = this.ComputeCollapsedRectangle(ef4);
                    ef1 = ef5;
                }
                else
                {
                    num1 = this.LabelSpot;
                }
                DiagramShape obj1 = this.CollapsedObject;
                if (obj1 != null)
                {
                    PointF tf1 = new PointF(ef1.X + (ef1.Width / 2f), ef1.Y + (ef1.Height / 2f));
                    obj1.Center = tf1;
                    if (!this.IsExpanded)
                    {
                        ef1 = obj1.Bounds;
                    }
                }
                PointF tf2 = this.GetRectangleSpotLocation(ef1, num1);
                this.PositionLabel(text1, num1, tf2);
            }
        }

        public virtual void LayoutPort()
        {
            DiagramPort port1 = this.Port;
            if ((port1 != null) && port1.CanView())
            {
                if (this.Handle != null)
                {
                    RectangleF ef1 = this.Handle.Bounds;
                    port1.Bounds = ef1;
                }
                else if (this.Label != null)
                {
                    port1.Bounds = this.Label.Bounds;
                }
                else
                {
                    SizeF ef3;
                    RectangleF ef2 = this.ComputeBounds();
                    if (this.IsExpanded)
                    {
                        ef3 = this.TopLeftMargin;
                    }
                    else
                    {
                        ef3 = this.CollapsedTopLeftMargin;
                    }
                    port1.Position = new PointF(ef2.X + ef3.Width, ef2.Y + ef3.Height);
                }
            }
        }

        protected override void MoveChildren(RectangleF prevRect)
        {
            float single1 = base.Left - prevRect.X;
            float single2 = base.Top - prevRect.Y;
            GroupEnumerator enumerator1 = base.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj1 = enumerator1.Current;
                if (obj1 is IDiagramLine)
                {
                    RectangleF ef1 = obj1.Bounds;
                    obj1.Bounds = new RectangleF(ef1.X + single1, ef1.Y + single2, ef1.Width, ef1.Height);
                }
            }
            enumerator1 = base.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj2 = enumerator1.Current;
                if (!(obj2 is IDiagramLine))
                {
                    RectangleF ef2 = obj2.Bounds;
                    obj2.Bounds = new RectangleF(ef2.X + single1, ef2.Y + single2, ef2.Width, ef2.Height);
                }
            }
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            if ((this.CollapsedObject == null) || (view.IsPrinting ? !this.CollapsedObject.CanPrint() : !this.CollapsedObject.CanView()))
            {
                this.PaintDecoration(g, view);
            }
            base.Paint(g, view);
        }

        protected virtual void PaintDecoration(Graphics g, DiagramView view)
        {
            SizeF ef1;
            if (this.IsExpanded)
            {
                ef1 = this.Corner;
            }
            else
            {
                ef1 = this.CollapsedCorner;
            }
            GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
            if (this.Opacity > 0f)
            {
                RoundedRectangleGraph.MakeRoundedRectangularPath(path1, 0f, 0f, this.Bounds, ef1);
                if (this.Shadowed)
                {
                    SizeF ef2 = this.GetShadowOffset(view);
                    GraphicsPath path2 = new GraphicsPath(FillMode.Winding);
                    RoundedRectangleGraph.MakeRoundedRectangularPath(path2, ef2.Width, ef2.Height, this.Bounds, ef1);
                    Region region1 = new Region(path2);
                    region1.Exclude(path1);
                    Brush brush1 = this.GetShadowBrush(view);
                    g.FillRegion(brush1, region1);
                    region1.Dispose();
                    path2.Dispose();
                }
                int num1 = (int)System.Math.Round((double)((this.Opacity / 100f) * 255f));
                Color color1 = Color.FromArgb(num1, this.BackgroundColor);
                Brush brush2 = new SolidBrush(color1);
                DiagramGraph.DrawPath(g, view, null, brush2, path1);
                brush2.Dispose();
                path1.Reset();
            }
            if (this.BorderPen != null)
            {
                RectangleF ef3 = this.Bounds;
                float single1 = (this.BorderPenInfo != null) ? this.BorderPenInfo.Width : this.BorderPen.Width;
                DiagramShape.InflateRect(ref ef3, -single1 / 2f, -single1 / 2f);
                RoundedRectangleGraph.MakeRoundedRectangularPath(path1, 0f, 0f, ef3, ef1);
                DiagramGraph.DrawPath(g, view, this.BorderPen, null, path1);
            }
            path1.Dispose();
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
                if (this.PickableBackground)
                {
                    if (!selectableOnly)
                    {
                        return this;
                    }
                    if (this.CanSelect())
                    {
                        return this;
                    }
                }
            }
            return null;
        }

        public override IDiagramShapeCollection PickObjects(PointF p, bool selectableOnly, IDiagramShapeCollection coll, int max)
        {
            if (coll == null)
            {
                coll = new DiagramShapeCollection();
            }
            if (coll.Count < max)
            {
                if (!this.CanView())
                {
                    return coll;
                }
                foreach (DiagramShape obj1 in base.Backwards)
                {
                    DiagramShape obj2 = obj1.Pick(p, selectableOnly);
                    if (obj2 != null)
                    {
                        coll.Add(obj2);
                        if (coll.Count >= max)
                        {
                            return coll;
                        }
                    }
                }
                DiagramShape obj3 = this.Pick(p, selectableOnly);
                if (obj3 != null)
                {
                    coll.Add(obj3);
                }
            }
            return coll;
        }

        private void PositionLabel(DiagramText lab, int spot, PointF pt)
        {
            if (spot == 2)
            {
                lab.Alignment = spot;
                lab.SetSpotLocation(0x10, pt);
                if ((this.Handle != null) && DiagramShape.IntersectsRect(this.Handle.Bounds, lab.Bounds))
                {
                    pt.X = this.Handle.Right + 2f;
                    lab.SetSpotLocation(0x10, pt);
                }
            }
            else if (spot == 4)
            {
                lab.Alignment = spot;
                lab.SetSpotLocation(8, pt);
            }
            else if (spot == 8)
            {
                lab.Alignment = spot;
                lab.SetSpotLocation(4, pt);
            }
            else if (spot == 0x10)
            {
                lab.Alignment = spot;
                lab.SetSpotLocation(2, pt);
            }
            else
            {
                lab.Alignment = this.SpotOpposite(spot);
                lab.SetSpotLocation(this.SpotOpposite(spot), pt);
            }
        }

        protected virtual void PrepareCollapse()
        {
            GroupEnumerator enumerator1 = base.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                SubGraphNode graph1 = enumerator1.Current as SubGraphNode;
                if (graph1 != null)
                {
                    graph1.Collapse();
                }
            }
        }

        protected virtual void PrepareExpand()
        {
        }

        public override void Remove(DiagramShape obj)
        {
            base.Remove(obj);
            if (obj == this.myHandle)
            {
                this.myHandle = null;
            }
            else if (obj == this.myLabel)
            {
                this.myLabel = null;
            }
            else if (obj == this.myPort)
            {
                this.myPort = null;
            }
            else if (obj == this.myCollapsedObject)
            {
                this.myCollapsedObject = null;
            }
            if (this.SavedBounds.ContainsKey(obj))
            {
                this.SavedBounds.Remove(obj);
            }
            if (this.SavedPaths.ContainsKey(obj))
            {
                this.SavedPaths.Remove(obj);
            }
        }

        public static void ReparentAllLinksToSubGraphs(IDiagramShapeCollection coll, bool behind, DiagramLayer layer)
        {
            foreach (DiagramShape obj1 in coll)
            {
                IDiagramNode node1 = obj1 as IDiagramNode;
                if (node1 != null)
                {
                    foreach (IDiagramLine link1 in node1.Links)
                    {
                        if (((link1 != null) && (link1.FromPort != null)) && (link1.ToPort != null))
                        {
                            SubGraphNode.ReparentToCommonSubGraph(link1.DiagramShape, link1.FromPort.DiagramShape, link1.ToPort.DiagramShape, behind, layer);
                        }
                    }
                    continue;
                }
                IDiagramPort port1 = obj1 as IDiagramPort;
                if (port1 != null)
                {
                    foreach (IDiagramLine link2 in port1.Links)
                    {
                        if (((link2 != null) && (link2.FromPort != null)) && (link2.ToPort != null))
                        {
                            SubGraphNode.ReparentToCommonSubGraph(link2.DiagramShape, link2.FromPort.DiagramShape, link2.ToPort.DiagramShape, behind, layer);
                        }
                    }
                    continue;
                }
                IDiagramLine link3 = obj1 as IDiagramLine;
                if (((link3 != null) && (link3.FromPort != null)) && (link3.ToPort != null))
                {
                    SubGraphNode.ReparentToCommonSubGraph(link3.DiagramShape, link3.FromPort.DiagramShape, link3.ToPort.DiagramShape, behind, layer);
                }
            }
        }

        public static void ReparentToCommonSubGraph(DiagramShape obj, DiagramShape child1, DiagramShape child2, bool behind, DiagramLayer layer)
        {
            SubGraphNode graph1 = SubGraphNode.FindParentSubGraph(child1);
            SubGraphNode graph2 = SubGraphNode.FindParentSubGraph(child2);
            DiagramShape obj1 = DiagramShape.FindCommonParent(graph1, graph2);
            while ((obj1 != null) && !(obj1 is SubGraphNode))
            {
                obj1 = obj1.Parent;
            }
            SubGraphNode graph3 = obj1 as SubGraphNode;
            if ((obj.Parent != graph3) || (obj.Layer == null))
            {
                if ((obj.Parent == null) && (obj.Layer == null))
                {
                    if (graph3 != null)
                    {
                        if (behind)
                        {
                            graph3.InsertBefore(null, obj);
                        }
                        else
                        {
                            graph3.InsertAfter(null, obj);
                        }
                    }
                    else
                    {
                        layer.Add(obj);
                    }
                }
                else
                {
                    DiagramShapeCollection collection1 = new DiagramShapeCollection();
                    collection1.Add(obj);
                    if (graph3 != null)
                    {
                        graph3.AddCollection(collection1, false);
                    }
                    else
                    {
                        layer.AddCollection(collection1, false);
                    }
                }
            }
        }

        protected override void RescaleChildren(RectangleF prevRect)
        {
            if ((prevRect.Width > 0f) && (prevRect.Height > 0f))
            {
                RectangleF ef1 = this.Bounds;
                float single1 = ef1.Width / prevRect.Width;
                float single2 = ef1.Height / prevRect.Height;
                GroupEnumerator enumerator1 = base.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj1 = enumerator1.Current;
                    if ((obj1 is IDiagramLine) && obj1.AutoRescales)
                    {
                        RectangleF ef2 = obj1.Bounds;
                        float single3 = ef1.X + ((ef2.X - prevRect.X) * single1);
                        float single4 = ef1.Y + ((ef2.Y - prevRect.Y) * single2);
                        float single5 = ef2.Width * single1;
                        float single6 = ef2.Height * single2;
                        obj1.Bounds = new RectangleF(single3, single4, single5, single6);
                    }
                }
                enumerator1 = base.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj2 = enumerator1.Current;
                    if (!(obj2 is IDiagramLine) && obj2.AutoRescales)
                    {
                        RectangleF ef3 = obj2.Bounds;
                        float single7 = ef1.X + ((ef3.X - prevRect.X) * single1);
                        float single8 = ef1.Y + ((ef3.Y - prevRect.Y) * single2);
                        float single9 = ef3.Width * single1;
                        float single10 = ef3.Height * single2;
                        obj2.Bounds = new RectangleF(single7, single8, single9, single10);
                    }
                }
            }
        }

        protected virtual void SaveChildBounds(DiagramShape child, RectangleF sgrect)
        {
            if ((((child != this.Handle) && (child != this.Label)) && (child != this.Port)) && (child != this.CollapsedObject))
            {
                if ((child is Shapes.LineGraph) || (child is TextLine))
                {
                    StrokeGraph stroke1 = null;
                    if (child is Shapes.LineGraph)
                    {
                        stroke1 = (Shapes.LineGraph)child;
                    }
                    else
                    {
                        stroke1 = ((TextLine)child).RealLink;
                    }
                    PointF[] tfArray1 = stroke1.CopyPointsArray();
                    for (int num1 = 0; num1 < tfArray1.Length; num1++)
                    {
                        PointF tf1 = tfArray1[num1];
                        tf1.X -= sgrect.X;
                        tf1.Y -= sgrect.Y;
                        tfArray1[num1] = tf1;
                    }
                    this.myPathsHashtable[child] = tfArray1;
                }
                else
                {
                    SizeF ef1 = child.Size;
                    SizeF ef2 = DiagramTool.SubtractPoints(child.Position, new PointF(sgrect.X, sgrect.Y));
                    this.myBoundsHashtable[child] = new RectangleF(ef2.Width, ef2.Height, ef1.Width, ef1.Height);
                }
            }
        }

        private void setBottomRightMargin(SizeF margin, bool undoing)
        {
            SizeF ef1 = this.myBottomRightMargin;
            if (((ef1 != margin) && (margin.Width >= 0f)) && (margin.Height >= 0f))
            {
                this.myBottomRightMargin = margin;
                this.Changed(0xa98, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(margin));
                if (!undoing)
                {
                    base.InvalidBounds = true;
                }
            }
        }

        private void setCollapsedBottomRightMargin(SizeF margin, bool undoing)
        {
            SizeF ef1 = this.myCollapsedBottomRightMargin;
            if (((ef1 != margin) && (margin.Width >= 0f)) && (margin.Height >= 0f))
            {
                this.myCollapsedBottomRightMargin = margin;
                this.Changed(0xa9a, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(margin));
                if (!undoing)
                {
                    base.InvalidBounds = true;
                }
            }
        }

        private void setCollapsedLabelSpot(int spot, bool undoing)
        {
            int num1 = this.myCollapsedLabelSpot;
            if (num1 != spot)
            {
                this.myCollapsedLabelSpot = spot;
                this.Changed(0xa9c, num1, null, DiagramShape.NullRect, spot, null, DiagramShape.NullRect);
                if (!undoing)
                {
                    this.LayoutChildren(null);
                }
            }
        }

        private void setCollapsedTopLeftMargin(SizeF margin, bool undoing)
        {
            SizeF ef1 = this.myCollapsedTopLeftMargin;
            if (((ef1 != margin) && (margin.Width >= 0f)) && (margin.Height >= 0f))
            {
                this.myCollapsedTopLeftMargin = margin;
                this.Changed(0xa99, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(margin));
                if (!undoing)
                {
                    base.InvalidBounds = true;
                }
            }
        }

        private void setLabelSpot(int spot, bool undoing)
        {
            int num1 = this.myLabelSpot;
            if (num1 != spot)
            {
                this.myLabelSpot = spot;
                this.Changed(0xa92, num1, null, DiagramShape.NullRect, spot, null, DiagramShape.NullRect);
                if (!undoing)
                {
                    this.LayoutChildren(null);
                }
            }
        }

        private void setTopLeftMargin(SizeF margin, bool undoing)
        {
            SizeF ef1 = this.myTopLeftMargin;
            if (((ef1 != margin) && (margin.Width >= 0f)) && (margin.Height >= 0f))
            {
                this.myTopLeftMargin = margin;
                this.Changed(0xa93, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(margin));
                if (!undoing)
                {
                    base.InvalidBounds = true;
                }
            }
        }

        public void Toggle()
        {
            if (this.State == SubGraphNodeState.Expanded)
            {
                this.Collapse();
            }
            else if (this.State == SubGraphNodeState.Collapsed)
            {
                this.Expand();
            }
        }


        [Description("The background color for the group; the opacity is specified separately"), Category("Appearance")]
        public virtual Color BackgroundColor
        {
            get
            {
                return this.myBackgroundColor;
            }
            set
            {
                Color color1 = this.myBackgroundColor;
                if (color1 != value)
                {
                    this.myBackgroundColor = value;
                    this.Changed(0xa90, 0, color1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), Description("The pen used to draw an outline for this node.")]
        public virtual Pen BorderPen
        {
            get
            {
                if (this.myBorderPenInfo != null)
                {
                    return this.myBorderPenInfo.GetPen();
                }
                return null;
            }
            set
            {
                DiagramGraph.GoPenInfo info1 = this.myBorderPenInfo;
                DiagramGraph.GoPenInfo info2 = DiagramGraph.GetPenInfo(value);
                if (info1 != info2)
                {
                    this.myBorderPenInfo = info2;
                    this.Changed(0xa94, 0, info1, DiagramShape.NullRect, 0, info2, DiagramShape.NullRect);
                }
            }
        }

        internal DiagramGraph.GoPenInfo BorderPenInfo
        {
            get
            {
                return this.myBorderPenInfo;
            }
        }

        [Description("The margin around the text inside the background at the right side and the bottom"), Category("Appearance"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF BottomRightMargin
        {
            get
            {
                return this.myBottomRightMargin;
            }
            set
            {
                this.setBottomRightMargin(value, false);
            }
        }

        [Description("The margin around the text inside the background at the right side and the bottom of a collapsed subgraph"), Category("Appearance"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF CollapsedBottomRightMargin
        {
            get
            {
                return this.myCollapsedBottomRightMargin;
            }
            set
            {
                this.setCollapsedBottomRightMargin(value, false);
            }
        }

        [Category("Appearance"), TypeConverter(typeof(SizeFConverter)), Description("The maximum radial width and height of each corner of a collapsed node")]
        public virtual SizeF CollapsedCorner
        {
            get
            {
                return this.myCollapsedCorner;
            }
            set
            {
                SizeF ef1 = this.myCollapsedCorner;
                if (((ef1 != value) && (value.Width >= 0f)) && (value.Height >= 0f))
                {
                    this.myCollapsedCorner = value;
                    this.Changed(0xa9b, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Category("Appearance"), Description("The spot where the label should be positioned when the node is collapsed"), DefaultValue(1)]
        public virtual int CollapsedLabelSpot
        {
            get
            {
                return this.myCollapsedLabelSpot;
            }
            set
            {
                this.setCollapsedLabelSpot(value, false);
            }
        }

        public DiagramShape CollapsedObject
        {
            get
            {
                return this.myCollapsedObject;
            }
            set
            {
                DiagramShape obj1 = this.myCollapsedObject;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        this.Remove(obj1);
                    }
                    this.myCollapsedObject = value;
                    if (value != null)
                    {
                        this.InsertBefore(null, value);
                    }
                    this.Changed(0xa9d, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    if (this.CollapsedObject != null)
                    {
                        this.CollapsedObject.Visible = !this.IsExpanded;
                        this.CollapsedObject.Printable = !this.IsExpanded;
                    }
                }
            }
        }

        [Description("The margin around the text inside the background at the left side and the top of a collapsed subgraph"), Category("Appearance"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF CollapsedTopLeftMargin
        {
            get
            {
                return this.myCollapsedTopLeftMargin;
            }
            set
            {
                this.setCollapsedTopLeftMargin(value, false);
            }
        }

        [Description("Whether the user is allowed to expand and collapse this subgraph"), Category("Behavior"), DefaultValue(true)]
        public virtual bool Collapsible
        {
            get
            {
                return ((base.InternalFlags & 0x2000000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x2000000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x2000000;
                    }
                    else
                    {
                        base.InternalFlags &= -33554433;
                    }
                    this.Changed(0xa8f, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The maximum radial width and height of each corner"), Category("Appearance"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF Corner
        {
            get
            {
                return this.myCorner;
            }
            set
            {
                SizeF ef1 = this.myCorner;
                if (((ef1 != value) && (value.Width >= 0f)) && (value.Height >= 0f))
                {
                    this.myCorner = value;
                    this.Changed(2710, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("Gets an enumerator over all of the links going out of this node.")]
        public virtual NodeLinkEnumerator ExternalDestinationLinks
        {
            get
            {
                return this.GetLinkEnumerator(DiagramNode.Search.NotSelf | DiagramNode.Search.LinksOut);
            }
        }

        [Description("Gets an enumerator over all of the nodes that have links going out of this node.")]
        public virtual NodeNodeEnumerator ExternalDestinations
        {
            get
            {
                return this.GetNodeEnumerator(DiagramNode.Search.NotSelf | DiagramNode.Search.NodesOut);
            }
        }

        [Description("Gets an enumerator over all of the links connected to this node.")]
        public virtual NodeLinkEnumerator ExternalLinks
        {
            get
            {
                return this.GetLinkEnumerator(DiagramNode.Search.NotSelf | (DiagramNode.Search.LinksOut | DiagramNode.Search.LinksIn));
            }
        }

        [Description("Gets an enumerator over all of the nodes that are connected to this node.")]
        public virtual NodeNodeEnumerator ExternalNodes
        {
            get
            {
                return this.GetNodeEnumerator(DiagramNode.Search.NotSelf | (DiagramNode.Search.NodesOut | DiagramNode.Search.NodesIn));
            }
        }

        [Description("Gets an enumerator over all of the links coming into this node.")]
        public virtual NodeLinkEnumerator ExternalSourceLinks
        {
            get
            {
                return this.GetLinkEnumerator(DiagramNode.Search.NotSelf | DiagramNode.Search.LinksIn);
            }
        }

        [Description("Gets an enumerator over all of the nodes that have links coming into this node.")]
        public virtual NodeNodeEnumerator ExternalSources
        {
            get
            {
                return this.GetNodeEnumerator(DiagramNode.Search.NotSelf | DiagramNode.Search.NodesIn);
            }
        }

        public Shapes.SubGraphHandle Handle
        {
            get
            {
                return this.myHandle;
            }
        }

        [Category("Appearance"), Description("Whether this subgraph is in an expanded state"), DefaultValue(true)]
        public bool IsExpanded
        {
            get
            {
                return (this.State == SubGraphNodeState.Expanded);
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
                    this.Changed(0xa8e, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The spot where the label should be positioned"), Category("Appearance"), DefaultValue(0x20)]
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

        [Browsable(false)]
        public virtual SizeF Margin
        {
            get
            {
                return this.myTopLeftMargin;
            }
            set
            {
                this.TopLeftMargin = value;
                this.BottomRightMargin = value;
            }
        }

        [DefaultValue((float)20f), Category("Appearance"), Description("The opaqueness of the background; the background color is specified separately")]
        public virtual float Opacity
        {
            get
            {
                return this.myOpacity;
            }
            set
            {
                float single1 = this.myOpacity;
                if (((single1 != value) && (value >= 0f)) && (value <= 100f))
                {
                    this.myOpacity = value;
                    this.Changed(0xa91, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("Whether picking in the background of this node selects the node."), Category("Behavior"), DefaultValue(false)]
        public virtual bool PickableBackground
        {
            get
            {
                return ((base.InternalFlags & 0x4000000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x4000000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x4000000;
                    }
                    else
                    {
                        base.InternalFlags &= -67108865;
                    }
                    this.Changed(0xa95, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
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
                        this.InsertBefore(null, value);
                    }
                    this.Changed(0xa97, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public Hashtable SavedBounds
        {
            get
            {
                return this.myBoundsHashtable;
            }
        }

        public Hashtable SavedPaths
        {
            get
            {
                return this.myPathsHashtable;
            }
        }

        protected SubGraphNodeState State
        {
            get
            {
                return this.myState;
            }
            set
            {
                SubGraphNodeState state1 = this.myState;
                if (state1 != value)
                {
                    this.myState = value;
                    this.Changed(0xa9e, (int)state1, null, this.Bounds, (int)value, null, this.Bounds);
                }
            }
        }

        [TypeConverter(typeof(SizeFConverter)), Description("The margin around the text inside the background at the left side and the top"), Category("Appearance")]
        public virtual SizeF TopLeftMargin
        {
            get
            {
                return this.myTopLeftMargin;
            }
            set
            {
                this.setTopLeftMargin(value, false);
            }
        }


        public const int ChangedBackgroundColor = 0xa90;
        public const int ChangedBorderPen = 0xa94;
        public const int ChangedBottomRightMargin = 0xa98;
        public const int ChangedCollapsedBottomRightMargin = 0xa9a;
        public const int ChangedCollapsedCorner = 0xa9b;
        public const int ChangedCollapsedLabelSpot = 0xa9c;
        public const int ChangedCollapsedObject = 0xa9d;
        public const int ChangedCollapsedTopLeftMargin = 0xa99;
        public const int ChangedCollapsible = 0xa8f;
        public const int ChangedCorner = 2710;
        public const int ChangedLabel = 0xa8e;
        public const int ChangedLabelSpot = 0xa92;
        public const int ChangedOpacity = 0xa91;
        public const int ChangedPickableBackground = 0xa95;
        public const int ChangedPort = 0xa97;
        public const int ChangedState = 0xa9e;
        public const int ChangedTopLeftMargin = 0xa93;
        private const int flagCollapsible = 0x2000000;
        private const int flagExpandedResizable = 0x8000000;
        private const int flagIgnoreLabel = 0x1000000;
        private const int flagPickableBackground = 0x4000000;
        private Color myBackgroundColor;
        private DiagramGraph.GoPenInfo myBorderPenInfo;
        private SizeF myBottomRightMargin;
        private Hashtable myBoundsHashtable;
        private SizeF myCollapsedBottomRightMargin;
        private SizeF myCollapsedCorner;
        private int myCollapsedLabelSpot;
        private DiagramShape myCollapsedObject;
        private SizeF myCollapsedTopLeftMargin;
        private SizeF myCorner;
        private Shapes.SubGraphHandle myHandle;
        private DiagramText myLabel;
        private int myLabelSpot;
        private float myOpacity;
        private Hashtable myPathsHashtable;
        private DiagramPort myPort;
        private SubGraphNodeState myState;
        private SizeF myTopLeftMargin;
    }
}
