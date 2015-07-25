using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class DiagramNode : GroupShape, IDiagramNode, IGraphPart, ITextNode, IIdentifiablePart
    {
        public DiagramNode()
        {
            this.myToolTipText = null;
            this.myParts = null;
            this.myUserFlags = 0;
            this.myUserObject = null;
            this.myPartID = -1;
        }

        private void addItem(ArrayList items, IGraphPart obj)
        {
            if ((obj != null) && !items.Contains(obj))
            {
                items.Add(obj);
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 2000:
                    {
                        this.UserFlags = e.GetInt(undo);
                        return;
                    }
                case 0x7d1:
                    {
                        this.UserObject = e.GetValue(undo);
                        return;
                    }
                case 0x7d2:
                    {
                        this.ToolTipText = (string)e.GetValue(undo);
                        return;
                    }
                case 0x7d3:
                    {
                        this.PropertiesDelegatedToSelectionObject = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x7d4:
                    {
                        this.PartID = e.GetInt(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        private void considerLink(IDiagramLine l, IDiagramPort p, Search s, ArrayList items)
        {
            bool flag1 = (s & Search.NotSelf) == ((Search)0);
            if ((l.FromPort == p) && ((flag1 || (l.ToPort.DiagramShape == null)) || !l.ToPort.DiagramShape.IsChildOf(this)))
            {
                if ((s & Search.LinksOut) != ((Search)0))
                {
                    this.addItem(items, l);
                }
                if ((s & Search.NodesOut) != ((Search)0))
                {
                    this.addItem(items, l.ToNode);
                }
            }
            if ((l.ToPort == p) && ((flag1 || (l.FromPort.DiagramShape == null)) || !l.FromPort.DiagramShape.IsChildOf(this)))
            {
                if ((s & Search.LinksIn) != ((Search)0))
                {
                    this.addItem(items, l);
                }
                if ((s & Search.NodesIn) != ((Search)0))
                {
                    this.addItem(items, l.FromNode);
                }
            }
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            DiagramNode node1 = (DiagramNode)base.CopyObject(env);
            if (node1 != null)
            {
                node1.myParts = null;
                node1.myPartID = -1;
            }
            return node1;
        }

        internal void CopyPropertiesFromSelectionObject(DiagramShape oldobj, DiagramShape newobj)
        {
            if (((oldobj != null) && (newobj != null)) && (oldobj == this.SelectionObject))
            {
                newobj.Center = oldobj.Center;
                newobj.Selectable = oldobj.Selectable;
                newobj.Resizable = oldobj.Resizable;
                newobj.Reshapable = oldobj.Reshapable;
                newobj.ResizesRealtime = oldobj.ResizesRealtime;
                newobj.Shadowed = oldobj.Shadowed;
            }
        }

        public override void DoBeginEdit(DiagramView view)
        {
            if (this.Label != null)
            {
                this.Label.DoBeginEdit(view);
            }
        }

        internal ArrayList findAll(Search s)
        {
            ArrayList list1 = this.myParts;
            if (list1 == null)
            {
                list1 = new ArrayList();
            }
            else
            {
                list1.Clear();
                this.myParts = null;
            }
            this.findAllAux(this, s, list1);
            return list1;
        }

        private void findAllAux(DiagramShape obj, Search s, ArrayList items)
        {
            IDiagramPort port1 = obj as IDiagramPort;
            if (port1 != null)
            {
                if ((s & Search.Ports) != ((Search)0))
                {
                    this.addItem(items, port1);
                }
                DiagramPort port2 = port1 as DiagramPort;
                if (port2 != null)
                {
                    PortLinkEnumerator enumerator1 = port2.Links.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        IDiagramLine link1 = enumerator1.Current;
                        this.considerLink(link1, port1, s, items);
                    }
                }
                else
                {
                    foreach (IDiagramLine link2 in port1.Links)
                    {
                        this.considerLink(link2, port1, s, items);
                    }
                }
            }
            GroupShape group1 = obj as GroupShape;
            if (group1 != null)
            {
                GroupEnumerator enumerator5 = group1.GetEnumerator();
                GroupEnumerator enumerator4 = enumerator5.GetEnumerator();
                while (enumerator4.MoveNext())
                {
                    DiagramShape obj1 = enumerator4.Current;
                    this.findAllAux(obj1, s, items);
                }
            }
        }

        internal static DiagramText FindLabel(DiagramShape obj)
        {
            DiagramText text1 = obj as DiagramText;
            if (text1 != null)
            {
                return text1;
            }
            GroupShape group1 = obj as GroupShape;
            if (group1 != null)
            {
                GroupEnumerator enumerator2 = group1.GetEnumerator();
                GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramText text2 = DiagramNode.FindLabel(enumerator1.Current);
                    if (text2 != null)
                    {
                        return text2;
                    }
                }
            }
            return null;
        }

        private NodeLinkEnumerator GetLinkEnumerator(Search s)
        {
            return new NodeLinkEnumerator(this, s);
        }

        private NodeNodeEnumerator GetNodeEnumerator(Search s)
        {
            return new NodeNodeEnumerator(this, s);
        }

        private NodePortEnumerator GetPortEnumerator()
        {
            return new NodePortEnumerator(this, Search.Ports);
        }

        public override string GetToolTip(DiagramView view)
        {
            return this.ToolTipText;
        }

        IEnumerable IDiagramNode.DestinationLinks
        {
            get
            {
                return this.GetLinkEnumerator(Search.LinksOut);
            }
        }

        IEnumerable IDiagramNode.Destinations
        {
            get
            {
                return this.GetNodeEnumerator(Search.NodesOut);
            }
        }

        IEnumerable IDiagramNode.Links
        {
            get
            {
                return this.GetLinkEnumerator(Search.LinksOut | Search.LinksIn);
            }
        }

        IEnumerable IDiagramNode.Nodes
        {
            get
            {
                return this.GetNodeEnumerator(Search.NodesOut | Search.NodesIn);
            }
        }

        IEnumerable IDiagramNode.Ports
        {
            get
            {
                return this.GetPortEnumerator();
            }
        }

        IEnumerable IDiagramNode.SourceLinks
        {
            get
            {
                return this.GetLinkEnumerator(Search.LinksIn);
            }
        }

        IEnumerable IDiagramNode.Sources
        {
            get
            {
                return this.GetNodeEnumerator(Search.NodesIn);
            }
        }


        [Description("Gets an enumerator over all of the links going out of this node.")]
        public virtual NodeLinkEnumerator DestinationLinks
        {
            get
            {
                return this.GetLinkEnumerator(Search.LinksOut);
            }
        }

        [Description("Gets an enumerator over all of the nodes that have links going out of this node.")]
        public virtual NodeNodeEnumerator Destinations
        {
            get
            {
                return this.GetNodeEnumerator(Search.NodesOut);
            }
        }

        [Description("Returns itself as a DiagramShape.")]
        public DiagramShape DiagramShape
        {
            get
            {
                return this;
            }
            set
            {
            }
        }

        public virtual DiagramText Label
        {
            get
            {
                return DiagramNode.FindLabel(this);
            }
            set
            {
            }
        }

        [Description("Gets an enumerator over all of the links connected to this node.")]
        public virtual NodeLinkEnumerator Links
        {
            get
            {
                return this.GetLinkEnumerator(Search.LinksOut | Search.LinksIn);
            }
        }

        public override PointF Location
        {
            get
            {
                if (this.PropertiesDelegatedToSelectionObject && (this.SelectionObject != this))
                {
                    return this.SelectionObject.Center;
                }
                return base.Position;
            }
            set
            {
                if (this.PropertiesDelegatedToSelectionObject && (this.SelectionObject != this))
                {
                    SizeF ef1 = DiagramTool.SubtractPoints(this.SelectionObject.Center, base.Position);
                    base.Position = new PointF(value.X - ef1.Width, value.Y - ef1.Height);
                }
                else
                {
                    base.Position = value;
                }
            }
        }

        [Description("Gets an enumerator over all of the nodes that are connected to this node.")]
        public virtual NodeNodeEnumerator Nodes
        {
            get
            {
                return this.GetNodeEnumerator(Search.NodesOut | Search.NodesIn);
            }
        }

        [Category("Ownership"), Description("The unique ID of this part in its document.")]
        public int PartID
        {
            get
            {
                return this.myPartID;
            }
            set
            {
                int num1 = this.myPartID;
                if (num1 != value)
                {
                    this.myPartID = value;
                    this.Changed(0x7d4, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [Description("Gets an enumerator over all of the ports that are part of this node.")]
        public virtual NodePortEnumerator Ports
        {
            get
            {
                return this.GetPortEnumerator();
            }
        }

        internal bool PropertiesDelegatedToSelectionObject
        {
            get
            {
                return ((base.InternalFlags & 0x400000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x400000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x400000;
                    }
                    else
                    {
                        base.InternalFlags &= -4194305;
                    }
                    this.Changed(0x7d3, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public override bool Reshapable
        {
            get
            {
                if (this.PropertiesDelegatedToSelectionObject && (this.SelectionObject != this))
                {
                    return this.SelectionObject.Reshapable;
                }
                return base.Reshapable;
            }
            set
            {
                if (this.PropertiesDelegatedToSelectionObject && (this.SelectionObject != this))
                {
                    this.SelectionObject.Reshapable = value;
                }
                else
                {
                    base.Reshapable = value;
                }
            }
        }

        public override bool Resizable
        {
            get
            {
                if (this.PropertiesDelegatedToSelectionObject && (this.SelectionObject != this))
                {
                    return this.SelectionObject.Resizable;
                }
                return base.Resizable;
            }
            set
            {
                if (this.PropertiesDelegatedToSelectionObject && (this.SelectionObject != this))
                {
                    this.SelectionObject.Resizable = value;
                }
                else
                {
                    base.Resizable = value;
                }
            }
        }

        public override bool ResizesRealtime
        {
            get
            {
                if (this.PropertiesDelegatedToSelectionObject && (this.SelectionObject != this))
                {
                    return this.SelectionObject.ResizesRealtime;
                }
                return base.ResizesRealtime;
            }
            set
            {
                if (this.PropertiesDelegatedToSelectionObject && (this.SelectionObject != this))
                {
                    this.SelectionObject.ResizesRealtime = value;
                }
                else
                {
                    base.ResizesRealtime = value;
                }
            }
        }

        public override bool Shadowed
        {
            get
            {
                if (this.PropertiesDelegatedToSelectionObject && (this.SelectionObject != this))
                {
                    return this.SelectionObject.Shadowed;
                }
                return base.Shadowed;
            }
            set
            {
                if (this.PropertiesDelegatedToSelectionObject && (this.SelectionObject != this))
                {
                    this.SelectionObject.Shadowed = value;
                }
                else
                {
                    base.Shadowed = value;
                }
            }
        }

        [Description("Gets an enumerator over all of the links coming into this node.")]
        public virtual NodeLinkEnumerator SourceLinks
        {
            get
            {
                return this.GetLinkEnumerator(Search.LinksIn);
            }
        }

        [Description("Gets an enumerator over all of the nodes that have links coming into this node.")]
        public virtual NodeNodeEnumerator Sources
        {
            get
            {
                return this.GetNodeEnumerator(Search.NodesIn);
            }
        }

        public virtual string Text
        {
            get
            {
                DiagramText text1 = this.Label;
                if (text1 != null)
                {
                    return text1.Text;
                }
                return "";
            }
            set
            {
                DiagramText text1 = this.Label;
                if (text1 != null)
                {
                    text1.Text = value;
                }
            }
        }

        [Description("A string to be displayed in a tooltip.")]
        public virtual string ToolTipText
        {
            get
            {
                return this.myToolTipText;
            }
            set
            {
                string text1 = this.myToolTipText;
                if (text1 != value)
                {
                    this.myToolTipText = value;
                    this.Changed(0x7d2, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("An integer value associated with this node."), DefaultValue(0)]
        public virtual int UserFlags
        {
            get
            {
                return this.myUserFlags;
            }
            set
            {
                int num1 = this.myUserFlags;
                if (num1 != value)
                {
                    this.myUserFlags = value;
                    this.Changed(2000, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue((string)null), Description("An object associated with this node.")]
        public virtual object UserObject
        {
            get
            {
                return this.myUserObject;
            }
            set
            {
                object obj1 = this.myUserObject;
                if (obj1 != value)
                {
                    this.myUserObject = value;
                    this.Changed(0x7d1, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedNodeUserFlags = 2000;
        public const int ChangedNodeUserObject = 0x7d1;
        public const int ChangedPartID = 0x7d4;
        internal const int ChangedPropertiesDelegatedToSelectionObject = 0x7d3;
        public const int ChangedToolTipText = 0x7d2;
        private const int flagPropsOnSelObj = 0x400000;
        private int myPartID;
        [NonSerialized]
        internal ArrayList myParts;
        private string myToolTipText;
        private int myUserFlags;
        private object myUserObject;

        [Flags]
        internal enum Search
        {
            LinksIn = 2,
            LinksOut = 4,
            NodesIn = 8,
            NodesOut = 0x10,
            NotSelf = 0x20,
            Ports = 1
        }
    }
}
