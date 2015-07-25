using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class DiagramPort : DiagramGraph, IDiagramPort, IGraphPart, IIdentifiablePart
    {
        public DiagramPort()
        {
            this.myStyle = DiagramPortStyle.Ellipse;
            this.myPortObject = null;
            this.myFromLinkSpot = 0x40;
            this.myToLinkSpot = 0x100;
            this.myLinks = new ArrayList();
            this.myEndSegmentLength = 10f;
            this.myUserFlags = 0;
            this.myUserObject = null;
            this.myPartID = -1;
            base.InternalFlags &= -19;
            base.InternalFlags |= 36700160;
            this.Brush = DiagramGraph.Brushes_Black;
        }

        public virtual void AddDestinationLink(IDiagramLine link)
        {
            link.FromPort = this;
            if (link.FromPort == this)
            {
                this.addLink(link);
            }
        }

        private void addLink(IDiagramLine link)
        {
            if (!this.myLinks.Contains(link))
            {
                this.myLinks.Add(link);
                this.Changed(0x6ad, 0, link, DiagramShape.NullRect, 0, link, DiagramShape.NullRect);
                this.OnLinkChanged(link, 0x6ad, 0, link, DiagramShape.NullRect, 0, link, DiagramShape.NullRect);
            }
        }

        public virtual void AddSourceLink(IDiagramLine link)
        {
            link.ToPort = this;
            if (link.ToPort == this)
            {
                this.addLink(link);
            }
        }

        public virtual bool CanLinkFrom()
        {
            if (!this.IsValidFrom)
            {
                return false;
            }
            if (!this.CanView())
            {
                return false;
            }
            if ((base.Layer != null) && !base.Layer.CanLinkObjects())
            {
                return false;
            }
            return true;
        }

        public virtual bool CanLinkTo()
        {
            if (!this.IsValidTo)
            {
                return false;
            }
            if (!this.CanView())
            {
                return false;
            }
            if ((base.Layer != null) && !base.Layer.CanLinkObjects())
            {
                return false;
            }
            return true;
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 1700:
                    {
                        this.UserFlags = e.GetInt(undo);
                        return;
                    }
                case 0x6a5:
                    {
                        this.UserObject = e.GetValue(undo);
                        return;
                    }
                case 0x6a6:
                    {
                        this.Style = (DiagramPortStyle)e.GetInt(undo);
                        return;
                    }
                case 0x6a7:
                    {
                        this.PortObject = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0x6a8:
                    {
                        this.IsValidFrom = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x6a9:
                    {
                        this.IsValidTo = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x6aa:
                    {
                        this.IsValidSelfNode = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x6ab:
                    {
                        this.FromSpot = e.GetInt(undo);
                        return;
                    }
                case 0x6ac:
                    {
                        this.ToSpot = e.GetInt(undo);
                        return;
                    }
                case 0x6ad:
                    {
                        IDiagramLine link1 = (IDiagramLine)e.OldValue;
                        if (!undo)
                        {
                            this.addLink(link1);
                            return;
                        }
                        this.RemoveLink(link1);
                        return;
                    }
                case 1710:
                    {
                        IDiagramLine link2 = (IDiagramLine)e.OldValue;
                        if (!undo)
                        {
                            this.RemoveLink(link2);
                            return;
                        }
                        this.addLink(link2);
                        return;
                    }
                case 0x6af:
                    {
                        this.IsValidDuplicateLinks = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x6b0:
                    {
                        this.EndSegmentLength = e.GetFloat(undo);
                        return;
                    }
                case 0x6b1:
                    {
                        this.PartID = e.GetInt(undo);
                        return;
                    }
                case 0x6b2:
                    {
                        this.ClearsLinksWhenRemoved = (bool)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        public virtual void ClearLinks()
        {
            this.ClearLinks(null);
        }

        private void ClearLinks(Dot.Utility.Media.Diagram.Shapes.DiagramShape mainObj)
        {
            ILayerCollectionContainer container1 = (base.Layer != null) ? base.Layer.LayerCollectionContainer : null;
            for (int num1 = this.myLinks.Count; num1 > 0; num1 = System.Math.Min(num1, this.myLinks.Count))
            {
                IDiagramLine link1 = (IDiagramLine)this.myLinks[--num1];
                DiagramShape obj1 = link1.DiagramShape;
                if (((obj1 == null) || (obj1.Layer == null)) || ((obj1.Layer.LayerCollectionContainer == container1) && ((mainObj == null) || (!obj1.IsChildOf(mainObj) && !obj1.Movable))))
                {
                    link1.Unlink();
                }
            }
        }

        private void ComputeTrianglePoints(PointF[] v)
        {
            int num2;
            RectangleF ef1 = this.Bounds;
            int num1 = 1;
            DiagramPortStyle style1 = this.Style;
            if (style1 != DiagramPortStyle.Triangle)
            {
                switch (style1)
                {
                    case DiagramPortStyle.TriangleTopLeft:
                        {
                            num1 = 8;
                            goto Label_006F;
                        }
                    case DiagramPortStyle.TriangleTopRight:
                        {
                            num1 = 0x10;
                            goto Label_006F;
                        }
                    case DiagramPortStyle.TriangleBottomRight:
                        {
                            num1 = 2;
                            goto Label_006F;
                        }
                    case DiagramPortStyle.TriangleBottomLeft:
                        {
                            num1 = 4;
                            goto Label_006F;
                        }
                    case DiagramPortStyle.TriangleMiddleTop:
                        {
                            num1 = 0x80;
                            goto Label_006F;
                        }
                    case DiagramPortStyle.TriangleMiddleRight:
                        {
                            num1 = 0x100;
                            goto Label_006F;
                        }
                    case DiagramPortStyle.TriangleMiddleBottom:
                        {
                            num1 = 0x20;
                            goto Label_006F;
                        }
                    case DiagramPortStyle.TriangleMiddleLeft:
                        {
                            num1 = 0x40;
                            goto Label_006F;
                        }
                }
            }
            num1 = this.ToSpot;
        Label_006F:
            num2 = num1;
            if (num2 <= 0x10)
            {
                switch (num2)
                {
                    case 1:
                    case 3:
                        {
                            goto Label_0516;
                        }
                    case 2:
                        {
                            v[0].X = ef1.X + (ef1.Width / 2f);
                            v[0].Y = ef1.Y;
                            v[1].X = ef1.X + ef1.Width;
                            v[1].Y = ef1.Y + ef1.Height;
                            v[2].X = ef1.X;
                            v[2].Y = ef1.Y + (ef1.Height / 2f);
                            return;
                        }
                    case 4:
                        {
                            v[0].X = ef1.X + ef1.Width;
                            v[0].Y = ef1.Y + (ef1.Height / 2f);
                            v[1].X = ef1.X;
                            v[1].Y = ef1.Y + ef1.Height;
                            v[2].X = ef1.X + (ef1.Width / 2f);
                            v[2].Y = ef1.Y;
                            return;
                        }
                    case 8:
                        {
                            v[0].X = ef1.X + (ef1.Width / 2f);
                            v[0].Y = ef1.Y + ef1.Height;
                            v[1].X = ef1.X;
                            v[1].Y = ef1.Y;
                            v[2].X = ef1.X + ef1.Width;
                            v[2].Y = ef1.Y + (ef1.Height / 2f);
                            return;
                        }
                    case 0x10:
                        {
                            v[0].X = ef1.X;
                            v[0].Y = ef1.Y + (ef1.Height / 2f);
                            v[1].X = ef1.X + ef1.Width;
                            v[1].Y = ef1.Y;
                            v[2].X = ef1.X + (ef1.Width / 2f);
                            v[2].Y = ef1.Y + ef1.Height;
                            return;
                        }
                }
            }
            else if (num2 <= 0x40)
            {
                if (num2 == 0x20)
                {
                    v[0].X = ef1.X + ef1.Width;
                    v[0].Y = ef1.Y;
                    v[1].X = ef1.X + (ef1.Width / 2f);
                    v[1].Y = ef1.Y + ef1.Height;
                    v[2].X = ef1.X;
                    v[2].Y = ef1.Y;
                    return;
                }
                if (num2 == 0x40)
                {
                    v[0].X = ef1.X + ef1.Width;
                    v[0].Y = ef1.Y + ef1.Height;
                    v[1].X = ef1.X;
                    v[1].Y = ef1.Y + (ef1.Height / 2f);
                    v[2].X = ef1.X + ef1.Width;
                    v[2].Y = ef1.Y;
                    return;
                }
            }
            else
            {
                if (num2 == 0x80)
                {
                    v[0].X = ef1.X;
                    v[0].Y = ef1.Y + ef1.Height;
                    v[1].X = ef1.X + (ef1.Width / 2f);
                    v[1].Y = ef1.Y;
                    v[2].X = ef1.X + ef1.Width;
                    v[2].Y = ef1.Y + ef1.Height;
                    return;
                }
                if (num2 != 0x100)
                {
                }
            }
        Label_0516:
            v[0].X = ef1.X;
            v[0].Y = ef1.Y;
            v[1].X = ef1.X + ef1.Width;
            v[1].Y = ef1.Y + (ef1.Height / 2f);
            v[2].X = ef1.X;
            v[2].Y = ef1.Y + ef1.Height;
        }

        public virtual bool ContainsLink(IDiagramLine l)
        {
            return this.myLinks.Contains(l);
        }

        [Description("A array copy of all of the links connected at this port.")]
        public virtual IDiagramLine[] CopyLinksArray()
        {
            IDiagramLine[] linkArray1 = new IDiagramLine[this.LinksCount];
            this.myLinks.CopyTo(linkArray1, 0);
            return linkArray1;
        }

        public override Dot.Utility.Media.Diagram.Shapes.DiagramShape CopyObject(CopyDictionary env)
        {
            DiagramPort port1 = (DiagramPort)base.CopyObject(env);
            if (port1 != null)
            {
                port1.myLinks = new ArrayList();
                port1.myPartID = -1;
                if (this.myPortObject != null)
                {
                    env.Delayeds.Add(this);
                }
            }
            return port1;
        }

        public override void CopyObjectDelayed(CopyDictionary env, Dot.Utility.Media.Diagram.Shapes.DiagramShape newobj)
        {
            base.CopyObjectDelayed(env, newobj);
            DiagramPort port1 = (DiagramPort)newobj;
            DiagramShape obj1 = env[this.myPortObject] as DiagramShape;
            if (obj1 != null)
            {
                port1.myPortObject = obj1;
            }
        }

        private bool CycleOK(IDiagramPort toPort)
        {
            DiagramDocument document1 = base.Document;
            if (document1 != null)
            {
                switch (document1.ValidCycle)
                {
                    case DocumentValidCycle.NotDirected:
                        {
                            return !DiagramDocument.MakesDirectedCycle(this.Node, toPort.Node);
                        }
                    case DocumentValidCycle.NotDirectedFast:
                        {
                            return !DiagramDocument.MakesDirectedCycleFast(this.Node, toPort.Node);
                        }
                    case DocumentValidCycle.NotUndirected:
                        {
                            return !DiagramDocument.MakesUndirectedCycle(this.Node, toPort.Node);
                        }
                    case DocumentValidCycle.DestinationTree:
                        {
                            if (toPort.SourceLinksCount == 0)
                            {
                                return !DiagramDocument.MakesDirectedCycleFast(this.Node, toPort.Node);
                            }
                            return false;
                        }
                    case DocumentValidCycle.SourceTree:
                        {
                            if (this.DestinationLinksCount == 0)
                            {
                                return !DiagramDocument.MakesDirectedCycleFast(this.Node, toPort.Node);
                            }
                            return false;
                        }
                }
            }
            return true;
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            DiagramShape obj1 = this.PortObject;
            if ((((obj1 != null) && (obj1 != this)) && ((this.Style == DiagramPortStyle.Object) && (obj1.Layer == null))) && ((base.InternalFlags & 0x1000000) == 0))
            {
                base.InternalFlags |= 0x1000000;
                RectangleF ef1 = obj1.ExpandPaintBounds(rect, view);
                base.InternalFlags &= -16777217;
                return ef1;
            }
            return base.ExpandPaintBounds(rect, view);
        }

        internal SubGraphNode FindCollapsedSubGraph(Dot.Utility.Media.Diagram.Shapes.DiagramShape obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.Parent is SubGraphNode)
            {
                obj = obj.Parent;
                if (obj.CanView())
                {
                    return null;
                }
            }
            SubGraphNode graph1 = SubGraphNode.FindParentSubGraph(obj);
            SubGraphNode graph2 = null;
            while (graph1 != null)
            {
                if (graph1.IsExpanded)
                {
                    return graph2;
                }
                graph2 = graph1;
                graph1 = SubGraphNode.FindParentSubGraph(graph1);
            }
            return graph2;
        }

        public static IDiagramNode FindParentNode(Dot.Utility.Media.Diagram.Shapes.DiagramShape x)
        {
            if (x == null)
            {
                return null;
            }
            IDiagramNode node1 = x as IDiagramNode;
            if (node1 != null)
            {
                return node1;
            }
            return DiagramPort.FindParentNode(x.Parent);
        }

        public static IDiagramNode FindTopNode(Dot.Utility.Media.Diagram.Shapes.DiagramShape x)
        {
            if (x == null)
            {
                return null;
            }
            if (x.IsTopLevel)
            {
                return (x as IDiagramNode);
            }
            IDiagramNode node1 = DiagramPort.FindTopNode(x.Parent);
            if (node1 != null)
            {
                return node1;
            }
            return (x as IDiagramNode);
        }

        public virtual float GetFromLinkDir(IDiagramLine link)
        {
            if (((base.InternalFlags & 0x8000000) != 0) && !this.CanView())
            {
                SubGraphNode graph1 = this.FindCollapsedSubGraph(this);
                if ((graph1 != null) && (graph1.Port != null))
                {
                    return graph1.Port.GetFromLinkDir(link);
                }
            }
            int num1 = this.FromSpot;
            if ((num1 != 0) && (num1 != 1))
            {
                return this.GetLinkDir(num1);
            }
            if (((link == null) || (link.ToPort == null)) || (link.ToPort.DiagramShape == null))
            {
                return 0f;
            }
            PointF tf1 = link.ToPort.DiagramShape.Center;
            PointF tf2 = base.Center;
            if (System.Math.Abs((float)(tf1.X - tf2.X)) > System.Math.Abs((float)(tf1.Y - tf2.Y)))
            {
                if (tf1.X >= tf2.X)
                {
                    return 0f;
                }
                return 180f;
            }
            if (tf1.Y >= tf2.Y)
            {
                return 90f;
            }
            return 270f;
        }

        public virtual PointF GetFromLinkPoint(IDiagramLine link)
        {
            PointF tf1;
            if (((base.InternalFlags & 0x8000000) != 0) && !this.CanView())
            {
                SubGraphNode graph1 = this.FindCollapsedSubGraph(this);
                if ((graph1 != null) && (graph1.Port != null))
                {
                    return graph1.Port.GetFromLinkPoint(link);
                }
            }
            if (this.FromSpot != 0)
            {
                return this.GetSpotLocation(this.FromSpot);
            }
            if (((link == null) || (link.ToPort == null)) || (link.ToPort.DiagramShape == null))
            {
                return base.Center;
            }
            LineGraph link1 = link as LineGraph;
            if ((link1 == null) && (link is TextLine))
            {
                link1 = (link as TextLine).RealLink;
            }
            if ((link1 != null) && (link1.PointsCount > (link1.Orthogonal ? 6 : 2)))
            {
                tf1 = link1.GetPoint(1);
                if (link1.Orthogonal)
                {
                    tf1 = this.OrthoPointToward(tf1);
                    return this.GetLinkPointFromPoint(tf1);
                }
                PointF tf2 = this.GetLinkPointFromPoint(tf1);
                if (tf2 == base.Center)
                {
                    tf2 = this.GetLinkPointFromPoint(link1.GetPoint(2));
                    if ((tf2 == base.Center) && (link1.PointsCount > 3))
                    {
                        tf2 = this.GetLinkPointFromPoint(link1.GetPoint(link1.PointsCount - 1));
                    }
                }
                return tf2;
            }
            tf1 = link.ToPort.DiagramShape.Center;
            if ((link1 != null) && link1.Orthogonal)
            {
                tf1 = this.OrthoPointToward(tf1);
            }
            return this.GetLinkPointFromPoint(tf1);
        }

        public virtual float GetLinkDir(int spot)
        {
            int num1 = spot;
            if (num1 <= 0x10)
            {
                switch (num1)
                {
                    case 1:
                    case 3:
                        {
                            goto Label_004B;
                        }
                    case 2:
                        {
                            return 225f;
                        }
                    case 4:
                        {
                            return 315f;
                        }
                    case 8:
                        {
                            return 45f;
                        }
                    case 0x10:
                        {
                            return 135f;
                        }
                }
            }
            else if (num1 <= 0x40)
            {
                if (num1 == 0x20)
                {
                    return 270f;
                }
                if (num1 == 0x40)
                {
                    return 0f;
                }
            }
            else
            {
                if (num1 == 0x80)
                {
                    return 90f;
                }
                if (num1 == 0x100)
                {
                    return 180f;
                }
            }
        Label_004B:
            return 0f;
        }

        public virtual PointF GetLinkPointFromPoint(PointF p)
        {
            PointF tf1;
            if (((base.InternalFlags & 0x8000000) != 0) && !this.CanView())
            {
                SubGraphNode graph1 = this.FindCollapsedSubGraph(this);
                if ((graph1 != null) && (graph1.Port != null))
                {
                    return graph1.Port.GetLinkPointFromPoint(p);
                }
            }
            DiagramShape obj1 = this.PortObject;
            if ((obj1 == null) || (obj1.Layer == null))
            {
                obj1 = this;
            }
            if (!obj1.ContainsPoint(p) && this.GetNearestIntersectionPoint(p, base.Center, out tf1))
            {
                return tf1;
            }
            return obj1.Center;
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            DiagramShape obj1 = this.PortObject;
            if ((((obj1 != null) && (obj1 != this)) && ((this.Style != DiagramPortStyle.Object) && (obj1.Layer != null))) && ((base.InternalFlags & 0x1000000) == 0))
            {
                base.InternalFlags |= 0x1000000;
                bool flag1 = obj1.GetNearestIntersectionPoint(p1, p2, out result);
                base.InternalFlags &= -16777217;
                return flag1;
            }
            return base.GetNearestIntersectionPoint(p1, p2, out result);
        }

        public virtual float GetToLinkDir(IDiagramLine link)
        {
            if (((base.InternalFlags & 0x8000000) != 0) && !this.CanView())
            {
                SubGraphNode graph1 = this.FindCollapsedSubGraph(this);
                if ((graph1 != null) && (graph1.Port != null))
                {
                    return graph1.Port.GetToLinkDir(link);
                }
            }
            int num1 = this.ToSpot;
            if ((num1 != 0) && (num1 != 1))
            {
                return this.GetLinkDir(num1);
            }
            if (((link == null) || (link.FromPort == null)) || (link.FromPort.DiagramShape == null))
            {
                return 0f;
            }
            PointF tf1 = link.FromPort.DiagramShape.Center;
            PointF tf2 = base.Center;
            if (System.Math.Abs((float)(tf1.X - tf2.X)) > System.Math.Abs((float)(tf1.Y - tf2.Y)))
            {
                if (tf1.X >= tf2.X)
                {
                    return 0f;
                }
                return 180f;
            }
            if (tf1.Y >= tf2.Y)
            {
                return 90f;
            }
            return 270f;
        }

        public virtual PointF GetToLinkPoint(IDiagramLine link)
        {
            PointF tf1;
            if (((base.InternalFlags & 0x8000000) != 0) && !this.CanView())
            {
                SubGraphNode graph1 = this.FindCollapsedSubGraph(this);
                if ((graph1 != null) && (graph1.Port != null))
                {
                    return graph1.Port.GetToLinkPoint(link);
                }
            }
            if (this.ToSpot != 0)
            {
                return this.GetSpotLocation(this.ToSpot);
            }
            if (((link == null) || (link.FromPort == null)) || (link.FromPort.DiagramShape == null))
            {
                return base.Center;
            }
            LineGraph link1 = link as LineGraph;
            if ((link1 == null) && (link is TextLine))
            {
                link1 = (link as TextLine).RealLink;
            }
            if ((link1 != null) && (link1.PointsCount > (link1.Orthogonal ? 6 : 2)))
            {
                tf1 = link1.GetPoint(link1.PointsCount - 2);
                if (link1.Orthogonal)
                {
                    tf1 = this.OrthoPointToward(tf1);
                    return this.GetLinkPointFromPoint(tf1);
                }
                PointF tf2 = this.GetLinkPointFromPoint(tf1);
                if (tf2 == base.Center)
                {
                    tf2 = this.GetLinkPointFromPoint(link1.GetPoint(link1.PointsCount - 3));
                    if ((tf2 == base.Center) && (link1.PointsCount > 3))
                    {
                        tf2 = this.GetLinkPointFromPoint(link1.GetPoint(0));
                    }
                }
                return tf2;
            }
            tf1 = link.FromPort.DiagramShape.Center;
            if ((link1 != null) && link1.Orthogonal)
            {
                tf1 = this.OrthoPointToward(tf1);
            }
            return this.GetLinkPointFromPoint(tf1);
        }

        public virtual bool IsInSameNode(IDiagramPort p)
        {
            return DiagramPort.IsInSameNode(this, p);
        }

        public static bool IsInSameNode(IDiagramPort a, IDiagramPort b)
        {
            if ((a != null) && (b != null))
            {
                if (a == b)
                {
                    return true;
                }
                object obj1 = a.Node;
                if ((obj1 == null) && (a.DiagramShape != null))
                {
                    obj1 = a.DiagramShape.TopLevelObject;
                }
                object obj2 = b.Node;
                if ((obj2 == null) && (b.DiagramShape != null))
                {
                    obj2 = b.DiagramShape.TopLevelObject;
                }
                if (obj1 != null)
                {
                    return (obj1 == obj2);
                }
            }
            return false;
        }

        public virtual bool IsLinked(IDiagramPort p)
        {
            return DiagramPort.IsLinked(this, p);
        }

        public static bool IsLinked(IDiagramPort a, IDiagramPort b)
        {
            if ((a != null) && (b != null))
            {
                DiagramPort port1 = b as DiagramPort;
                if (port1 != null)
                {
                    PortLinkEnumerator enumerator1 = port1.Links.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        IDiagramLine link1 = enumerator1.Current;
                        IDiagramPort port2 = link1.FromPort;
                        IDiagramPort port3 = link1.ToPort;
                        if ((port2 == a) && (port3 == b))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    foreach (IDiagramLine link2 in b.Links)
                    {
                        IDiagramPort port4 = link2.FromPort;
                        IDiagramPort port5 = link2.ToPort;
                        if ((port4 == a) && (port5 == b))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public virtual bool IsValidLink(IDiagramPort toPort)
        {
            if ((((this.CanLinkFrom() && (toPort != null)) && toPort.CanLinkTo()) && (((this.IsValidSelfNode && (toPort.DiagramShape is DiagramPort)) && ((DiagramPort)toPort.DiagramShape).IsValidSelfNode) || !this.IsInSameNode(toPort))) && (((this.IsValidDuplicateLinks && (toPort.DiagramShape is DiagramPort)) && ((DiagramPort)toPort.DiagramShape).IsValidDuplicateLinks) || !this.IsLinked(toPort)))
            {
                return this.CycleOK(toPort);
            }
            return false;
        }

        public virtual void LinksOnPortChanged(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            PortLinkEnumerator enumerator1 = this.Links.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                IDiagramLine link1 = enumerator1.Current;
                if (link1 != null)
                {
                    link1.OnPortChanged(this, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
                }
            }
        }

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
            RectangleF ef1 = this.Bounds;
            switch (this.Style)
            {
                case DiagramPortStyle.None:
                    {
                        path1.AddLine(ef1.X, ef1.Y, ef1.X, ef1.Y);
                        return path1;
                    }
                case DiagramPortStyle.Object:
                    {
                        path1.AddLine(ef1.X, ef1.Y, ef1.X, ef1.Y);
                        return path1;
                    }
                case DiagramPortStyle.Triangle:
                case DiagramPortStyle.TriangleTopLeft:
                case DiagramPortStyle.TriangleTopRight:
                case DiagramPortStyle.TriangleBottomRight:
                case DiagramPortStyle.TriangleBottomLeft:
                case DiagramPortStyle.TriangleMiddleTop:
                case DiagramPortStyle.TriangleMiddleRight:
                case DiagramPortStyle.TriangleMiddleBottom:
                case DiagramPortStyle.TriangleMiddleLeft:
                    {
                        PointF[] tfArray1 = new PointF[3];
                        this.ComputeTrianglePoints(tfArray1);
                        path1.AddPolygon(tfArray1);
                        return path1;
                    }
                case DiagramPortStyle.Rectangle:
                    {
                        path1.AddRectangle(ef1);
                        return path1;
                    }
                case DiagramPortStyle.Diamond:
                    {
                        PointF[] tfArray2 = new PointF[4];
                        tfArray2[0].X = ef1.X + (ef1.Width / 2f);
                        tfArray2[0].Y = ef1.Y;
                        tfArray2[1].X = ef1.X + ef1.Width;
                        tfArray2[1].Y = ef1.Y + (ef1.Height / 2f);
                        tfArray2[2].X = tfArray2[0].X;
                        tfArray2[2].Y = ef1.Y + ef1.Height;
                        tfArray2[3].X = ef1.X;
                        tfArray2[3].Y = tfArray2[1].Y;
                        path1.AddPolygon(tfArray2);
                        return path1;
                    }
                case DiagramPortStyle.Plus:
                    {
                        path1.AddLine((float)(ef1.X + (ef1.Width / 2f)), ef1.Y, (float)(ef1.X + (ef1.Width / 2f)), (float)(ef1.Y + ef1.Height));
                        path1.StartFigure();
                        path1.AddLine(ef1.X, (float)(ef1.Y + (ef1.Height / 2f)), (float)(ef1.X + ef1.Width), (float)(ef1.Y + (ef1.Height / 2f)));
                        return path1;
                    }
                case DiagramPortStyle.Times:
                    {
                        path1.AddLine(ef1.X, ef1.Y, (float)(ef1.X + ef1.Width), (float)(ef1.Y + ef1.Height));
                        path1.StartFigure();
                        path1.AddLine((float)(ef1.X + ef1.Width), ef1.Y, ef1.X, (float)(ef1.Y + ef1.Height));
                        return path1;
                    }
                case DiagramPortStyle.PlusTimes:
                    {
                        path1.AddLine((float)(ef1.X + (ef1.Width / 2f)), ef1.Y, (float)(ef1.X + (ef1.Width / 2f)), (float)(ef1.Y + ef1.Height));
                        path1.StartFigure();
                        path1.AddLine(ef1.X, (float)(ef1.Y + (ef1.Height / 2f)), (float)(ef1.X + ef1.Width), (float)(ef1.Y + (ef1.Height / 2f)));
                        path1.StartFigure();
                        path1.AddLine(ef1.X, ef1.Y, (float)(ef1.X + ef1.Width), (float)(ef1.Y + ef1.Height));
                        path1.StartFigure();
                        path1.AddLine((float)(ef1.X + ef1.Width), ef1.Y, ef1.X, (float)(ef1.Y + ef1.Height));
                        return path1;
                    }
            }
            path1.AddEllipse(ef1.X, ef1.Y, ef1.Width, ef1.Height);
            return path1;
        }

        IEnumerable IDiagramPort.DestinationLinks
        {
            get
            {
                return new PortFilteredLinkEnumerator(this, this.myLinks, true);
            }
        }

        IEnumerable IDiagramPort.Links
        {
            get
            {
                return new PortLinkEnumerator(this.myLinks);
            }
        }

        IEnumerable IDiagramPort.SourceLinks
        {
            get
            {
                return new PortFilteredLinkEnumerator(this, this.myLinks, false);
            }
        }

        protected override void OnBoundsChanged(RectangleF old)
        {
            base.OnBoundsChanged(old);
            this.LinksOnPortChanged(0x3e9, 0, null, old, 0, null, this.Bounds);
        }

        protected override void OnLayerChanged(DiagramLayer oldlayer, DiagramLayer newlayer, Dot.Utility.Media.Diagram.Shapes.DiagramShape mainObj)
        {
            base.OnLayerChanged(oldlayer, newlayer, mainObj);
            if (((newlayer == null) && this.ClearsLinksWhenRemoved) && !this.NoClearLinks)
            {
                this.ClearLinks(mainObj);
            }
        }

        public virtual void OnLinkChanged(IDiagramLine l, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
        }

        public override bool OnMouseOver(InputEventArgs evt, DiagramView view)
        {
            if (!view.CanLinkObjects())
            {
                return false;
            }
            if (!this.CanLinkFrom() && !this.CanLinkTo())
            {
                return false;
            }
            view.Cursor = Cursors.Hand;
            return true;
        }

        private PointF OrthoPointToward(PointF p)
        {
            PointF tf1 = base.Center;
            if (System.Math.Abs((float)(p.X - tf1.X)) >= System.Math.Abs((float)(p.Y - tf1.Y)))
            {
                if (p.X >= tf1.X)
                {
                    p.X = 9999999f;
                }
                else
                {
                    p.X = -9999999f;
                }
                p.Y = tf1.Y;
                return p;
            }
            if (p.Y >= tf1.Y)
            {
                p.Y = 9999999f;
            }
            else
            {
                p.Y = -9999999f;
            }
            p.X = tf1.X;
            return p;
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            if (!this.PaintGreek(g, view))
            {
                RectangleF ef1 = this.Bounds;
                switch (this.Style)
                {
                    case DiagramPortStyle.None:
                        {
                            return;
                        }
                    case DiagramPortStyle.Object:
                        {
                            DiagramShape obj1 = this.PortObject;
                            if ((obj1 != null) && (obj1.Layer == null))
                            {
                                obj1.Bounds = ef1;
                                obj1.Paint(g, view);
                            }
                            return;
                        }
                    case DiagramPortStyle.Triangle:
                    case DiagramPortStyle.TriangleTopLeft:
                    case DiagramPortStyle.TriangleTopRight:
                    case DiagramPortStyle.TriangleBottomRight:
                    case DiagramPortStyle.TriangleBottomLeft:
                    case DiagramPortStyle.TriangleMiddleTop:
                    case DiagramPortStyle.TriangleMiddleRight:
                    case DiagramPortStyle.TriangleMiddleBottom:
                    case DiagramPortStyle.TriangleMiddleLeft:
                        {
                            PointF[] tfArray1 = view.AllocTempPointArray(3);
                            this.ComputeTrianglePoints(tfArray1);
                            DiagramGraph.DrawPolygon(g, view, this.Pen, this.Brush, tfArray1);
                            view.FreeTempPointArray(tfArray1);
                            return;
                        }
                    case DiagramPortStyle.Rectangle:
                        {
                            DiagramGraph.DrawRectangle(g, view, this.Pen, this.Brush, ef1.X, ef1.Y, ef1.Width, ef1.Height);
                            return;
                        }
                    case DiagramPortStyle.Diamond:
                        {
                            PointF[] tfArray2 = view.AllocTempPointArray(4);
                            tfArray2[0].X = ef1.X + (ef1.Width / 2f);
                            tfArray2[0].Y = ef1.Y;
                            tfArray2[1].X = ef1.X + ef1.Width;
                            tfArray2[1].Y = ef1.Y + (ef1.Height / 2f);
                            tfArray2[2].X = tfArray2[0].X;
                            tfArray2[2].Y = ef1.Y + ef1.Height;
                            tfArray2[3].X = ef1.X;
                            tfArray2[3].Y = tfArray2[1].Y;
                            DiagramGraph.DrawPolygon(g, view, this.Pen, this.Brush, tfArray2);
                            view.FreeTempPointArray(tfArray2);
                            return;
                        }
                    case DiagramPortStyle.Plus:
                        {
                            DiagramGraph.DrawLine(g, view, this.Pen, ef1.X + (ef1.Width / 2f), ef1.Y, ef1.X + (ef1.Width / 2f), ef1.Y + ef1.Height);
                            DiagramGraph.DrawLine(g, view, this.Pen, ef1.X, ef1.Y + (ef1.Height / 2f), ef1.X + ef1.Width, ef1.Y + (ef1.Height / 2f));
                            return;
                        }
                    case DiagramPortStyle.Times:
                        {
                            DiagramGraph.DrawLine(g, view, this.Pen, ef1.X, ef1.Y, ef1.X + ef1.Width, ef1.Y + ef1.Height);
                            DiagramGraph.DrawLine(g, view, this.Pen, ef1.X + ef1.Width, ef1.Y, ef1.X, ef1.Y + ef1.Height);
                            return;
                        }
                    case DiagramPortStyle.PlusTimes:
                        {
                            DiagramGraph.DrawLine(g, view, this.Pen, ef1.X + (ef1.Width / 2f), ef1.Y, ef1.X + (ef1.Width / 2f), ef1.Y + ef1.Height);
                            DiagramGraph.DrawLine(g, view, this.Pen, ef1.X, ef1.Y + (ef1.Height / 2f), ef1.X + ef1.Width, ef1.Y + (ef1.Height / 2f));
                            DiagramGraph.DrawLine(g, view, this.Pen, ef1.X, ef1.Y, ef1.X + ef1.Width, ef1.Y + ef1.Height);
                            DiagramGraph.DrawLine(g, view, this.Pen, ef1.X + ef1.Width, ef1.Y, ef1.X, ef1.Y + ef1.Height);
                            return;
                        }
                }
                DiagramGraph.DrawEllipse(g, view, this.Pen, this.Brush, ef1.X, ef1.Y, ef1.Width, ef1.Height);
            }
        }

        public virtual bool PaintGreek(Graphics g, DiagramView view)
        {
            float single1 = view.DocScale;
            float single2 = view.PaintNothingScale / view.WorldScale.Height;
            float single3 = view.PaintGreekScale / view.WorldScale.Height;
            if (view.IsPrinting)
            {
                single2 /= 4f;
                single3 /= 4f;
            }
            if (single1 > single2)
            {
                if (single1 > single3)
                {
                    return false;
                }
                if (this.Style != DiagramPortStyle.None)
                {
                    RectangleF ef1 = this.Bounds;
                    DiagramGraph.DrawRectangle(g, view, this.Pen, this.Brush, ef1.X, ef1.Y, ef1.Width, ef1.Height);
                }
            }
            return true;
        }

        public virtual void RemoveLink(IDiagramLine link)
        {
            int num1 = this.myLinks.IndexOf(link);
            if (num1 >= 0)
            {
                this.myLinks.RemoveAt(num1);
                this.Changed(1710, 0, link, DiagramShape.NullRect, 0, link, DiagramShape.NullRect);
                this.OnLinkChanged(link, 1710, 0, link, DiagramShape.NullRect, 0, link, DiagramShape.NullRect);
            }
        }


        [Category("Behavior"), DefaultValue(true), Description("Whether removing a port from its document causes its attached links to be removed too.")]
        public virtual bool ClearsLinksWhenRemoved
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
                    this.Changed(0x6b2, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Gets an enumerator over the links going out of this port.")]
        public virtual PortFilteredLinkEnumerator DestinationLinks
        {
            get
            {
                return new PortFilteredLinkEnumerator(this, this.myLinks, true);
            }
        }

        [Description("The number of links going out of this port.")]
        public virtual int DestinationLinksCount
        {
            get
            {
                int num1 = 0;
                PortFilteredLinkEnumerator enumerator1 = this.DestinationLinks.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    if (enumerator1.Current == null)
                    {
                        continue;
                    }
                    num1++;
                }
                return num1;
            }
        }

        [Description("The length of the link segment closest to this port.")]
        public virtual float EndSegmentLength
        {
            get
            {
                return this.myEndSegmentLength;
            }
            set
            {
                float single1 = this.myEndSegmentLength;
                if (single1 != value)
                {
                    this.myEndSegmentLength = value;
                    this.Changed(0x6b0, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                    this.LinksOnPortChanged(0x6b0, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [DefaultValue(0x40), Description("The spot for attaching links coming out from this port."), Category("Appearance")]
        public virtual int FromSpot
        {
            get
            {
                return this.myFromLinkSpot;
            }
            set
            {
                int num1 = this.myFromLinkSpot;
                if (num1 != value)
                {
                    this.myFromLinkSpot = value;
                    this.Changed(0x6ab, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                    this.LinksOnPortChanged(0x6ab, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [Description("Returns itself as a DiagramShape.")]
        public Dot.Utility.Media.Diagram.Shapes.DiagramShape DiagramShape
        {
            get
            {
                return this;
            }
            set
            {
            }
        }

        [Description("Whether a valid link can be made between two ports already connected by a link."), Category("Behavior"), DefaultValue(false)]
        public virtual bool IsValidDuplicateLinks
        {
            get
            {
                return ((base.InternalFlags & 0x800000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x800000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x800000;
                    }
                    else
                    {
                        base.InternalFlags &= -8388609;
                    }
                    this.Changed(0x6af, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(true), Description("A flag for whether a valid link can have this port as its FromPort."), Category("Behavior")]
        public virtual bool IsValidFrom
        {
            get
            {
                return ((base.InternalFlags & 0x100000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x100000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x100000;
                    }
                    else
                    {
                        base.InternalFlags &= -1048577;
                    }
                    this.Changed(0x6a8, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether a valid link can be made between two ports belonging to the same node."), DefaultValue(false), Category("Behavior")]
        public virtual bool IsValidSelfNode
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
                    this.Changed(0x6aa, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("A flag for whether a valid link can have this port as its ToPort."), Category("Behavior"), DefaultValue(true)]
        public virtual bool IsValidTo
        {
            get
            {
                return ((base.InternalFlags & 0x200000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x200000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x200000;
                    }
                    else
                    {
                        base.InternalFlags &= -2097153;
                    }
                    this.Changed(0x6a9, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Gets an enumerator over all of the links connected at this port.")]
        public virtual PortLinkEnumerator Links
        {
            get
            {
                return new PortLinkEnumerator(this.myLinks);
            }
        }

        [Description("The total number of links connected at this port.")]
        public virtual int LinksCount
        {
            get
            {
                return this.myLinks.Count;
            }
        }

        internal bool NoClearLinks
        {
            get
            {
                return ((base.InternalFlags & 0x4000000) != 0);
            }
            set
            {
                if (value)
                {
                    base.InternalFlags |= 0x4000000;
                }
                else
                {
                    base.InternalFlags &= -67108865;
                }
            }
        }

        [Description("The node that this port is part of.")]
        public virtual IDiagramNode Node
        {
            get
            {
                return DiagramPort.FindParentNode(this);
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
                    this.Changed(0x6b1, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [Description("The DiagramShape that may take this port's place and appearance or shape.")]
        public virtual Dot.Utility.Media.Diagram.Shapes.DiagramShape PortObject
        {
            get
            {
                return this.myPortObject;
            }
            set
            {
                DiagramShape obj1 = this.myPortObject;
                if (obj1 != value)
                {
                    this.myPortObject = value;
                    this.Changed(0x6a7, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.LinksOnPortChanged(0x6a7, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Gets an enumerator over the links coming into this port.")]
        public virtual PortFilteredLinkEnumerator SourceLinks
        {
            get
            {
                return new PortFilteredLinkEnumerator(this, this.myLinks, false);
            }
        }

        [Description("The number of links coming into this port.")]
        public virtual int SourceLinksCount
        {
            get
            {
                int num1 = 0;
                PortFilteredLinkEnumerator enumerator1 = this.SourceLinks.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    if (enumerator1.Current == null)
                    {
                        continue;
                    }
                    num1++;
                }
                return num1;
            }
        }

        [Description("The appearance style."), Category("Appearance"), DefaultValue(2)]
        public virtual DiagramPortStyle Style
        {
            get
            {
                return this.myStyle;
            }
            set
            {
                DiagramPortStyle style1 = this.myStyle;
                if (style1 != value)
                {
                    this.myStyle = value;
                    this.Changed(0x6a6, (int)style1, null, DiagramShape.NullRect, (int)value, null, DiagramShape.NullRect);
                    this.LinksOnPortChanged(0x6a6, (int)style1, null, DiagramShape.NullRect, (int)value, null, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), DefaultValue(0x100), Description("The spot for attaching links going into this port.")]
        public virtual int ToSpot
        {
            get
            {
                return this.myToLinkSpot;
            }
            set
            {
                if (value == -23)
                {
                    base.InternalFlags |= 0x8000000;
                }
                else if (value == -24)
                {
                    base.InternalFlags &= -134217729;
                }
                int num1 = this.myToLinkSpot;
                if (num1 != value)
                {
                    this.myToLinkSpot = value;
                    this.Changed(0x6ac, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                    this.LinksOnPortChanged(0x6ac, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [Description("An integer value associated with this port."), DefaultValue(0)]
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
                    this.Changed(1700, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue((string)null), Description("An object associated with this port.")]
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
                    this.Changed(0x6a5, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedAddedLink = 0x6ad;
        public const int ChangedClearsLinksWhenRemoved = 0x6b2;
        public const int ChangedEndSegmentLength = 0x6b0;
        public const int ChangedFromSpot = 0x6ab;
        public const int ChangedObject = 0x6a7;
        public const int ChangedPartID = 0x6b1;
        public const int ChangedPortUserFlags = 1700;
        public const int ChangedPortUserObject = 0x6a5;
        public const int ChangedRemovedLink = 1710;
        public const int ChangedStyle = 0x6a6;
        public const int ChangedToSpot = 0x6ac;
        public const int ChangedValidDuplicateLinks = 0x6af;
        public const int ChangedValidFrom = 0x6a8;
        public const int ChangedValidSelfNode = 0x6aa;
        public const int ChangedValidTo = 0x6a9;
        private const int flagClearsLinksWhenRemoved = 0x2000000;
        private const int flagNoClearLinks = 0x4000000;
        private const int flagRecursive = 0x1000000;
        private const int flagRedirectToSubGraphPort = 0x8000000;
        private const int flagValidDuplicateLinks = 0x800000;
        private const int flagValidFrom = 0x100000;
        private const int flagValidSelfNode = 0x400000;
        private const int flagValidTo = 0x200000;
        private float myEndSegmentLength;
        private int myFromLinkSpot;
        private ArrayList myLinks;
        private int myPartID;
        private Dot.Utility.Media.Diagram.Shapes.DiagramShape myPortObject;
        private DiagramPortStyle myStyle;
        private int myToLinkSpot;
        private int myUserFlags;
        private object myUserObject;
    }
}
