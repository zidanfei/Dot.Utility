using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public abstract class ToolLinking : DiagramTool
    {
        static ToolLinking()
        {
            ToolLinking.Valid = "Valid";
            ToolLinking.Invalid = "Invalid";
        }

        protected ToolLinking(DiagramView v)
            : base(v)
        {
            this.myForwardsOnly = false;
            this.myOrthogonal = false;
            this.myOrthogonalSet = false;
            this.myLinkingNew = true;
            this.myForwards = true;
            this.myOrigStartPort = null;
            this.myOrigEndPort = null;
            this.myTempStartPort = null;
            this.myTempEndPort = null;
            this.myTempLink = null;
            this.myValidPortsCache = new Hashtable();
        }

        protected virtual Shapes.IDiagramLine CreateTemporaryLink(Shapes.IDiagramPort fromPort, Shapes.IDiagramPort toPort)
        {
            Shapes.IDiagramLine link1 = (Shapes.IDiagramLine)Activator.CreateInstance(base.View.LineType);
            if ((link1 == null) || (link1.DiagramShape == null))
            {
                return null;
            }
            link1.FromPort = fromPort;
            link1.ToPort = toPort;
            Shapes.DiagramShape obj1 = link1.DiagramShape;
            if (obj1 is Shapes.LineGraph)
            {
                Shapes.LineGraph link2 = (Shapes.LineGraph)obj1;
                if (this.myOrthogonalSet)
                {
                    link2.Orthogonal = this.Orthogonal;
                }
                link2.AdjustingStyle = Shapes.LineAdjustingStyle.Calculate;
            }
            else if (obj1 is Shapes.TextLine)
            {
                Shapes.TextLine link3 = (Shapes.TextLine)obj1;
                if (this.myOrthogonalSet)
                {
                    link3.Orthogonal = this.Orthogonal;
                }
                link3.AdjustingStyle = Shapes.LineAdjustingStyle.Calculate;
            }
            base.View.Layers.Default.Add(obj1);
            return link1;
        }

        protected virtual Shapes.IDiagramPort CreateTemporaryPort(Shapes.IDiagramPort port, PointF pnt, bool forToPort, bool atEnd)
        {
            GoTemporaryPort port1 = new GoTemporaryPort();
            port1.Target = port as Shapes.DiagramPort;
            if ((port != null) && (port.DiagramShape != null))
            {
                port1.Size = port.DiagramShape.Size;
            }
            port1.Center = pnt;
            port1.Style = Shapes.DiagramPortStyle.None;
            base.View.Layers.Default.Add(port1);
            return port1;
        }

        public override void DoCancelMouse()
        {
            if (this.myLinkingNew)
            {
                if (this.Forwards)
                {
                    this.DoNoNewLink(this.StartPort, null);
                }
                else
                {
                    this.DoNoNewLink(null, this.StartPort);
                }
            }
            else if (this.OriginalEndPort == null)
            {
                if (this.Forwards)
                {
                    this.DoNoRelink(this.Link, this.StartPort, null);
                }
                else
                {
                    this.DoNoRelink(this.Link, null, this.StartPort);
                }
            }
            else if (this.Forwards)
            {
                this.DoCancelRelink(this.Link, this.OriginalStartPort, this.OriginalEndPort);
            }
            else
            {
                this.DoCancelRelink(this.Link, this.OriginalEndPort, this.OriginalStartPort);
            }
            base.View.Cursor = base.View.DefaultCursor;
            base.StopTool();
        }

        public virtual void DoCancelRelink(Shapes.IDiagramLine oldlink, Shapes.IDiagramPort fromPort, Shapes.IDiagramPort toPort)
        {
            oldlink.FromPort = fromPort;
            oldlink.ToPort = toPort;
            base.TransactionResult = null;
        }

        public virtual void DoLinking(PointF dc)
        {
            if (this.EndPort != null)
            {
                Shapes.DiagramShape obj1 = this.EndPort.DiagramShape;
                if (obj1 != null)
                {
                    RectangleF ef1;
                    Shapes.IDiagramPort port1 = this.PickNearestPort(dc);
                    GoTemporaryPort port2 = obj1 as GoTemporaryPort;
                    if (port2 != null)
                    {
                        port2.Target = port1 as Shapes.DiagramPort;
                    }
                    if ((port1 != null) && (port1.DiagramShape != null))
                    {
                        ef1 = port1.DiagramShape.Bounds;
                    }
                    else
                    {
                        ef1 = new RectangleF(dc.X, dc.Y, 0f, 0f);
                    }
                    obj1.Bounds = ef1;
                }
            }
        }

        public override void DoMouseMove()
        {
            this.DoLinking(base.LastInput.DocPoint);
            base.View.DoAutoScroll(base.LastInput.ViewPoint);
        }

        public override void DoMouseUp()
        {
            Shapes.IDiagramPort port1 = this.PickNearestPort(base.LastInput.DocPoint);
            if (port1 != null)
            {
                if (this.myLinkingNew)
                {
                    if (this.Forwards)
                    {
                        this.DoNewLink(this.OriginalStartPort, port1);
                    }
                    else
                    {
                        this.DoNewLink(port1, this.OriginalStartPort);
                    }
                }
                else if (this.Forwards)
                {
                    this.DoRelink(this.Link, this.OriginalStartPort, port1);
                }
                else
                {
                    this.DoRelink(this.Link, port1, this.OriginalStartPort);
                }
            }
            else
            {
                Shapes.IDiagramPort port2 = this.PickPort(base.LastInput.DocPoint);
                if (this.myLinkingNew)
                {
                    if (this.Forwards)
                    {
                        this.DoNoNewLink(this.OriginalStartPort, port2);
                    }
                    else
                    {
                        this.DoNoNewLink(port2, this.OriginalStartPort);
                    }
                }
                else if (this.Forwards)
                {
                    this.DoNoRelink(this.Link, this.OriginalStartPort, port2);
                }
                else
                {
                    this.DoNoRelink(this.Link, port2, this.OriginalStartPort);
                }
            }
            base.StopTool();
        }

        public virtual void DoNewLink(Shapes.IDiagramPort fromPort, Shapes.IDiagramPort toPort)
        {
            Shapes.IDiagramLine link1 = base.View.CreateLink(fromPort, toPort);
            if (link1 != null)
            {
                base.TransactionResult = "New Link";
                base.View.RaiseLinkCreated(link1.DiagramShape);
            }
            else
            {
                base.TransactionResult = null;
            }
        }

        public virtual void DoNoNewLink(Shapes.IDiagramPort fromPort, Shapes.IDiagramPort toPort)
        {
            base.TransactionResult = null;
        }

        public virtual void DoNoRelink(Shapes.IDiagramLine oldlink, Shapes.IDiagramPort fromPort, Shapes.IDiagramPort toPort)
        {
            Shapes.DiagramShape obj1 = oldlink.DiagramShape;
            if ((obj1 != null) && (obj1.Layer != null))
            {
                if (obj1.Movable)
                {
                    oldlink.FromPort = fromPort;
                    oldlink.ToPort = toPort;
                    base.TransactionResult = "Relink";
                    base.View.RaiseLinkRelinked(oldlink.DiagramShape);
                    return;
                }
                if (obj1.CanDelete())
                {
                    CancelEventArgs args1 = new CancelEventArgs();
                    base.View.RaiseSelectionDeleting(args1);
                    if (!args1.Cancel)
                    {
                        obj1.Remove();
                        base.View.RaiseSelectionDeleted();
                        base.TransactionResult = "Relink";
                        return;
                    }
                    this.DoCancelMouse();
                }
                else
                {
                    this.DoCancelMouse();
                }
            }
            base.TransactionResult = null;
        }

        public virtual void DoRelink(Shapes.IDiagramLine oldlink, Shapes.IDiagramPort fromPort, Shapes.IDiagramPort toPort)
        {
            oldlink.FromPort = fromPort;
            oldlink.ToPort = toPort;
            Shapes.SubGraphNode.ReparentToCommonSubGraph(oldlink.DiagramShape, (fromPort != null) ? fromPort.DiagramShape : null, (toPort != null) ? toPort.DiagramShape : null, true, base.View.Document.LinksLayer);
            base.TransactionResult = "Relink";
            base.View.RaiseLinkRelinked(oldlink.DiagramShape);
        }

        public virtual bool IsValidFromPort(Shapes.IDiagramPort fromPort)
        {
            return fromPort.CanLinkFrom();
        }

        public virtual bool IsValidLink(Shapes.IDiagramPort fromPort, Shapes.IDiagramPort toPort)
        {
            if ((fromPort != null) && (toPort != null))
            {
                return fromPort.IsValidLink(toPort);
            }
            return true;
        }

        public virtual bool IsValidToPort(Shapes.IDiagramPort toPort)
        {
            if (!this.ForwardsOnly)
            {
                return toPort.CanLinkTo();
            }
            return false;
        }

        public virtual Shapes.IDiagramPort PickNearestPort(PointF dc)
        {
            Shapes.IDiagramPort port1 = null;
            float single1 = base.View.PortGravity;
            single1 *= single1;
            LayerCollectionEnumerator enumerator1 = base.View.Layers.Backwards.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramLayer layer1 = enumerator1.Current;
                if (layer1.IsInDocument && layer1.CanViewObjects())
                {
                    LayerEnumerator enumerator3 = layer1.Backwards.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        Shapes.DiagramShape obj1 = enumerator3.Current;
                        port1 = this.pickNearestPort1(obj1, dc, port1, ref single1);
                    }
                }
            }
            return port1;
        }

        private Shapes.IDiagramPort pickNearestPort1(Shapes.DiagramShape obj, PointF dc, Shapes.IDiagramPort bestPort, ref float bestDist)
        {
            Shapes.IDiagramPort port1 = obj as Shapes.IDiagramPort;
            if (port1 != null)
            {
                PointF tf1 = this.PortPoint(port1, dc);
                float single1 = dc.X - tf1.X;
                float single2 = dc.Y - tf1.Y;
                float single3 = (single1 * single1) + (single2 * single2);
                if (single3 <= bestDist)
                {
                    object obj1 = null;
                    if (this.ValidPortsCache != null)
                    {
                        obj1 = this.ValidPortsCache[port1];
                    }
                    if (obj1 == ToolLinking.Valid)
                    {
                        bestPort = port1;
                        bestDist = single3;
                    }
                    else if (obj1 != ToolLinking.Invalid)
                    {
                        if ((this.Forwards && this.IsValidLink(this.OriginalStartPort, port1)) || (!this.Forwards && this.IsValidLink(port1, this.OriginalStartPort)))
                        {
                            if (this.ValidPortsCache != null)
                            {
                                this.ValidPortsCache[port1] = ToolLinking.Valid;
                            }
                            bestPort = port1;
                            bestDist = single3;
                        }
                        else if (this.ValidPortsCache != null)
                        {
                            this.ValidPortsCache[port1] = ToolLinking.Invalid;
                        }
                    }
                }
            }
            Shapes.GroupShape group1 = obj as Shapes.GroupShape;
            if (group1 != null)
            {
                Shapes.GroupEnumerator enumerator2 = group1.GetEnumerator();
                Shapes.GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj2 = enumerator1.Current;
                    bestPort = this.pickNearestPort1(obj2, dc, bestPort, ref bestDist);
                }
            }
            return bestPort;
        }

        public virtual Shapes.IDiagramPort PickPort(PointF dc)
        {
            Shapes.DiagramShape obj1 = base.View.PickObject(true, false, dc, false);
            return (obj1 as Shapes.IDiagramPort);
        }

        public virtual PointF PortPoint(Shapes.IDiagramPort port, PointF dc)
        {
            Shapes.DiagramPort port1 = port.DiagramShape as Shapes.DiagramPort;
            if (port1 == null)
            {
                return port.DiagramShape.Center;
            }
            Shapes.DiagramShape obj1 = port1.PortObject;
            if ((obj1 == null) || (obj1.Layer == null))
            {
                obj1 = port1;
            }
            SizeF ef1 = obj1.Size;
            float single1 = 10f / base.View.WorldScale.Width;
            if ((ef1.Width < single1) && (ef1.Height < single1))
            {
                return obj1.Center;
            }
            return port1.GetLinkPointFromPoint(dc);
        }

        public virtual void StartNewLink(Shapes.IDiagramPort port, PointF dc)
        {
            if (port != null)
            {
                base.StartTransaction();
                this.myLinkingNew = true;
                if (this.IsValidFromPort(port))
                {
                    this.Forwards = true;
                    this.StartPort = this.CreateTemporaryPort(port, port.DiagramShape.Center, false, false);
                    this.EndPort = this.CreateTemporaryPort(port, dc, true, true);
                    this.Link = this.CreateTemporaryLink(this.StartPort, this.EndPort);
                }
                else
                {
                    this.Forwards = false;
                    this.StartPort = this.CreateTemporaryPort(port, port.DiagramShape.Center, true, false);
                    this.EndPort = this.CreateTemporaryPort(port, dc, false, true);
                    this.Link = this.CreateTemporaryLink(this.EndPort, this.StartPort);
                }
                base.View.Cursor = Cursors.Hand;
            }
        }

        public virtual void StartRelink(Shapes.IDiagramLine oldlink, Shapes.IDiagramPort oldport, PointF dc)
        {
            if (oldlink != null)
            {
                Shapes.DiagramShape obj1 = oldlink.DiagramShape;
                if ((obj1 != null) && (obj1.Layer != null))
                {
                    base.StartTransaction();
                    this.myLinkingNew = false;
                    this.OriginalEndPort = oldport;
                    this.Link = oldlink;
                    if (oldlink.ToPort == oldport)
                    {
                        this.Forwards = true;
                        this.OriginalStartPort = oldlink.FromPort;
                        PointF tf1 = dc;
                        if (this.OriginalStartPort != null)
                        {
                            tf1 = this.OriginalStartPort.DiagramShape.Center;
                        }
                        else if (oldlink is Shapes.LineGraph)
                        {
                            Shapes.LineGraph link1 = (Shapes.LineGraph)oldlink;
                            if (link1.PointsCount > 0)
                            {
                                tf1 = link1.GetPoint(0);
                            }
                        }
                        else if (oldlink is Shapes.TextLine)
                        {
                            Shapes.TextLine link2 = (Shapes.TextLine)oldlink;
                            if (link2.RealLink.PointsCount > 0)
                            {
                                tf1 = link2.RealLink.GetPoint(0);
                            }
                        }
                        this.StartPort = this.CreateTemporaryPort(this.OriginalStartPort, tf1, false, false);
                        oldlink.FromPort = this.StartPort;
                        this.EndPort = this.CreateTemporaryPort(this.OriginalEndPort, dc, true, true);
                        oldlink.ToPort = this.EndPort;
                    }
                    else if (oldlink.FromPort == oldport)
                    {
                        this.Forwards = false;
                        this.OriginalStartPort = oldlink.ToPort;
                        PointF tf2 = dc;
                        if (this.OriginalStartPort != null)
                        {
                            tf2 = this.OriginalStartPort.DiagramShape.Center;
                        }
                        else if (oldlink is Shapes.LineGraph)
                        {
                            Shapes.LineGraph link3 = (Shapes.LineGraph)oldlink;
                            if (link3.PointsCount > 0)
                            {
                                tf2 = link3.GetPoint(link3.PointsCount - 1);
                            }
                        }
                        else if (oldlink is Shapes.TextLine)
                        {
                            Shapes.TextLine link4 = (Shapes.TextLine)oldlink;
                            if (link4.RealLink.PointsCount > 0)
                            {
                                tf2 = link4.RealLink.GetPoint(link4.RealLink.PointsCount - 1);
                            }
                        }
                        this.StartPort = this.CreateTemporaryPort(this.OriginalStartPort, tf2, true, false);
                        oldlink.ToPort = this.StartPort;
                        this.EndPort = this.CreateTemporaryPort(this.OriginalEndPort, dc, false, true);
                        oldlink.FromPort = this.EndPort;
                    }
                    base.View.Cursor = Cursors.Hand;
                }
            }
        }

        public override void Stop()
        {
            base.View.StopAutoScroll();
            this.Forwards = true;
            this.OriginalStartPort = null;
            this.OriginalEndPort = null;
            if (this.Link != null)
            {
                Shapes.DiagramShape obj1 = this.Link.DiagramShape;
                if ((obj1 != null) && obj1.IsInView)
                {
                    obj1.Remove();
                }
            }
            this.Link = null;
            if (this.StartPort != null)
            {
                Shapes.DiagramShape obj2 = this.StartPort.DiagramShape;
                if ((obj2 != null) && obj2.IsInView)
                {
                    obj2.Remove();
                }
            }
            this.StartPort = null;
            if (this.EndPort != null)
            {
                Shapes.DiagramShape obj3 = this.EndPort.DiagramShape;
                if ((obj3 != null) && obj3.IsInView)
                {
                    obj3.Remove();
                }
            }
            this.EndPort = null;
            if (this.ValidPortsCache != null)
            {
                this.ValidPortsCache.Clear();
            }
            base.StopTransaction();
        }


        public Shapes.IDiagramPort EndPort
        {
            get
            {
                return this.myTempEndPort;
            }
            set
            {
                this.myTempEndPort = value;
            }
        }

        public bool Forwards
        {
            get
            {
                return this.myForwards;
            }
            set
            {
                this.myForwards = value;
            }
        }

        public virtual bool ForwardsOnly
        {
            get
            {
                return this.myForwardsOnly;
            }
            set
            {
                this.myForwardsOnly = value;
            }
        }

        public Shapes.IDiagramLine Link
        {
            get
            {
                return this.myTempLink;
            }
            set
            {
                this.myTempLink = value;
            }
        }

        public Shapes.IDiagramPort OriginalEndPort
        {
            get
            {
                return this.myOrigEndPort;
            }
            set
            {
                this.myOrigEndPort = value;
            }
        }

        public Shapes.IDiagramPort OriginalStartPort
        {
            get
            {
                return this.myOrigStartPort;
            }
            set
            {
                this.myOrigStartPort = value;
            }
        }

        public virtual bool Orthogonal
        {
            get
            {
                return this.myOrthogonal;
            }
            set
            {
                this.myOrthogonal = value;
                this.myOrthogonalSet = true;
            }
        }

        public Shapes.IDiagramPort StartPort
        {
            get
            {
                return this.myTempStartPort;
            }
            set
            {
                this.myTempStartPort = value;
            }
        }

        public Hashtable ValidPortsCache
        {
            get
            {
                return this.myValidPortsCache;
            }
            set
            {
                this.myValidPortsCache = value;
            }
        }


        public static readonly object Invalid;
        [NonSerialized]
        private bool myForwards;
        private bool myForwardsOnly;
        [NonSerialized]
        private bool myLinkingNew;
        [NonSerialized]
        private Shapes.IDiagramPort myOrigEndPort;
        [NonSerialized]
        private Shapes.IDiagramPort myOrigStartPort;
        private bool myOrthogonal;
        private bool myOrthogonalSet;
        [NonSerialized]
        private Shapes.IDiagramPort myTempEndPort;
        [NonSerialized]
        private Shapes.IDiagramLine myTempLink;
        [NonSerialized]
        private Shapes.IDiagramPort myTempStartPort;
        [NonSerialized]
        private Hashtable myValidPortsCache;
        public static readonly object Valid;

        internal class GoTemporaryPort : Shapes.DiagramPort
        {
            internal GoTemporaryPort()
            {
                this.myTargetPort = null;
                this.PortObject = null;
                this.FromSpot = 0;
                this.ToSpot = 0;
                base.Size = new SizeF();
            }

            public override float GetFromLinkDir(Shapes.IDiagramLine link)
            {
                if (this.Target != null)
                {
                    return this.Target.GetFromLinkDir(link);
                }
                return base.GetFromLinkDir(link);
            }

            public override PointF GetFromLinkPoint(Shapes.IDiagramLine link)
            {
                if (this.Target != null)
                {
                    return this.Target.GetFromLinkPoint(link);
                }
                return base.GetFromLinkPoint(link);
            }

            public override PointF GetLinkPointFromPoint(PointF p)
            {
                if (this.Target != null)
                {
                    return this.Target.GetLinkPointFromPoint(p);
                }
                return base.GetLinkPointFromPoint(p);
            }

            public override float GetToLinkDir(Shapes.IDiagramLine link)
            {
                if (this.Target != null)
                {
                    return this.Target.GetToLinkDir(link);
                }
                return base.GetToLinkDir(link);
            }

            public override PointF GetToLinkPoint(Shapes.IDiagramLine link)
            {
                if (this.Target != null)
                {
                    return this.Target.GetToLinkPoint(link);
                }
                return base.GetToLinkPoint(link);
            }


            public override float EndSegmentLength
            {
                get
                {
                    if (this.Target != null)
                    {
                        return this.Target.EndSegmentLength;
                    }
                    return base.EndSegmentLength;
                }
            }

            public override int FromSpot
            {
                get
                {
                    if (this.Target != null)
                    {
                        return this.Target.FromSpot;
                    }
                    return base.FromSpot;
                }
            }

            public override Shapes.DiagramShape PortObject
            {
                get
                {
                    if (this.Target != null)
                    {
                        return this.Target.PortObject;
                    }
                    return base.PortObject;
                }
            }

            internal Shapes.DiagramPort Target
            {
                get
                {
                    return this.myTargetPort;
                }
                set
                {
                    this.myTargetPort = value;
                }
            }

            public override int ToSpot
            {
                get
                {
                    if (this.Target != null)
                    {
                        return this.Target.ToSpot;
                    }
                    return base.ToSpot;
                }
            }


            private Shapes.DiagramPort myTargetPort;
        }
    }
}
