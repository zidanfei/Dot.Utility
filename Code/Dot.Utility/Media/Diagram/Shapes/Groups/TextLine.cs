using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class TextLine : GroupShape, IDiagramLine, IGraphPart, IIdentifiablePart
    {
        public TextLine()
        {
            this.myRealLink = null;
            this.myFromLabel = null;
            this.myMidLabel = null;
            this.myToLabel = null;
            base.InternalFlags &= -5;
            Shapes.LineGraph link1 = this.CreateRealLink();
            if (link1 != null)
            {
                link1.Selectable = false;
                this.RealLink = link1;
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x51f:
                    {
                        this.RealLink = (Shapes.LineGraph)e.GetValue(undo);
                        return;
                    }
                case 0x520:
                    {
                        this.FromLabel = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0x521:
                    {
                        this.MidLabel = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0x522:
                    {
                        this.ToLabel = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0x523:
                    {
                        this.FromLabelCentered = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x524:
                    {
                        this.MidLabelCentered = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x525:
                    {
                        this.ToLabelCentered = (bool)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            base.CopyChildren(newgroup, env);
            TextLine link1 = (TextLine)newgroup;
            link1.myRealLink = (Shapes.LineGraph)env[this.myRealLink];
            link1.myFromLabel = (DiagramShape)env[this.myFromLabel];
            link1.myMidLabel = (DiagramShape)env[this.myMidLabel];
            link1.myToLabel = (DiagramShape)env[this.myToLabel];
        }

        public virtual LineGraph CreateRealLink()
        {
            return new Shapes.LineGraph();
        }

        public IDiagramNode GetOtherNode(IDiagramNode n)
        {
            return Shapes.LineGraph.GetOtherNode(this, n);
        }

        public IDiagramPort GetOtherPort(IDiagramPort p)
        {
            return Shapes.LineGraph.GetOtherPort(this, p);
        }

        public override void LayoutChildren(Dot.Utility.Media.Diagram.Shapes.DiagramShape childchanged)
        {
            if (!base.Initializing)
            {
                Shapes.LineGraph link1 = this.RealLink;
                if (link1 != null)
                {
                    int num1 = link1.PointsCount;
                    if (num1 >= 2)
                    {
                        DiagramShape obj1 = this.FromLabel;
                        if (obj1 != null)
                        {
                            PointF tf1 = link1.GetPoint(0);
                            PointF tf2 = link1.GetPoint(1);
                            if (num1 == 2)
                            {
                                this.PositionEndLabel(obj1, false, tf1, tf1, tf2);
                            }
                            else
                            {
                                this.PositionEndLabel(obj1, false, tf1, tf2, link1.GetPoint(2));
                            }
                        }
                        this.LayoutMidLabel(childchanged);
                        obj1 = this.ToLabel;
                        if (obj1 != null)
                        {
                            PointF tf3 = link1.GetPoint(num1 - 1);
                            PointF tf4 = link1.GetPoint(num1 - 2);
                            if (num1 == 2)
                            {
                                this.PositionEndLabel(obj1, true, tf3, tf3, tf4);
                            }
                            else
                            {
                                this.PositionEndLabel(obj1, true, tf3, tf4, link1.GetPoint(num1 - 3));
                            }
                        }
                    }
                }
            }
        }

        protected virtual void LayoutMidLabel(Dot.Utility.Media.Diagram.Shapes.DiagramShape childchanged)
        {
            DiagramShape obj1 = this.MidLabel;
            if (obj1 != null)
            {
                Shapes.LineGraph link1 = this.RealLink;
                int num1 = link1.PointsCount;
                if (num1 >= 2)
                {
                    if ((link1.Style == StrokeGraphStyle.Bezier) && (num1 < 7))
                    {
                        PointF tf5;
                        PointF tf6;
                        PointF tf1 = link1.GetPoint(0);
                        PointF tf2 = link1.GetPoint(1);
                        PointF tf3 = link1.GetPoint(num1 - 2);
                        PointF tf4 = link1.GetPoint(num1 - 1);
                        StrokeGraph.BezierMidPoint(tf1, tf2, tf3, tf4, out tf5, out tf6);
                        this.PositionMidLabel(obj1, tf5, tf6);
                    }
                    else
                    {
                        int num2 = num1 / 2;
                        if ((num1 % 2) == 0)
                        {
                            PointF tf7 = link1.GetPoint(num2 - 1);
                            PointF tf8 = link1.GetPoint(num2);
                            this.PositionMidLabel(obj1, tf7, tf8);
                        }
                        else
                        {
                            PointF tf9 = link1.GetPoint(num2 - 1);
                            PointF tf10 = link1.GetPoint(num2);
                            PointF tf11 = link1.GetPoint(num2 + 1);
                            float single1 = tf10.X - tf9.X;
                            float single2 = tf10.Y - tf9.Y;
                            float single3 = tf11.X - tf10.X;
                            float single4 = tf11.Y - tf10.Y;
                            if (((single1 * single1) + (single2 * single2)) >= ((single3 * single3) + (single4 * single4)))
                            {
                                this.PositionMidLabel(obj1, tf9, tf10);
                            }
                            else
                            {
                                this.PositionMidLabel(obj1, tf10, tf11);
                            }
                        }
                    }
                }
            }
        }

        protected override void MoveChildren(RectangleF old)
        {
            bool flag1 = base.Initializing;
            base.Initializing = true;
            base.MoveChildren(old);
            base.Initializing = flag1;
        }

        public virtual void OnPortChanged(IDiagramPort port, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            if (this.RealLink != null)
            {
                this.RealLink.OnPortChanged(port, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
            }
            this.LayoutChildren((port == null) ? null : port.DiagramShape);
        }

        public override Dot.Utility.Media.Diagram.Shapes.DiagramShape Pick(PointF p, bool selectableOnly)
        {
            if (this.CanView())
            {
                foreach (DiagramShape obj1 in base.Backwards)
                {
                    DiagramShape obj2 = obj1.Pick(p, selectableOnly);
                    if (obj2 != null)
                    {
                        return obj2;
                    }
                }
            }
            return null;
        }

        protected virtual void PositionEndLabel(Dot.Utility.Media.Diagram.Shapes.DiagramShape lab, bool atEnd, PointF a, PointF b, PointF c)
        {
            if ((!atEnd && this.FromLabelCentered) || (atEnd && this.ToLabelCentered))
            {
                if (base.IsApprox(a.X, b.X))
                {
                    if (a.Y < b.Y)
                    {
                        lab.SetSpotLocation(0x20, a);
                    }
                    else
                    {
                        lab.SetSpotLocation(0x80, a);
                    }
                }
                else if (base.IsApprox(a.Y, b.Y))
                {
                    if (a.X < b.X)
                    {
                        lab.SetSpotLocation(0x100, a);
                    }
                    else
                    {
                        lab.SetSpotLocation(0x40, a);
                    }
                }
                else if (a.X < b.X)
                {
                    if (a.Y < b.Y)
                    {
                        lab.SetSpotLocation(2, a);
                    }
                    else
                    {
                        lab.SetSpotLocation(0x10, a);
                    }
                }
                else if (a.Y < b.Y)
                {
                    lab.SetSpotLocation(8, a);
                }
                else
                {
                    lab.SetSpotLocation(4, a);
                }
            }
            else if (a.X < b.X)
            {
                if (b.Y <= c.Y)
                {
                    lab.SetSpotLocation(0x10, a);
                }
                else
                {
                    lab.SetSpotLocation(2, a);
                }
            }
            else if (a.X > b.X)
            {
                if (b.Y <= c.Y)
                {
                    lab.SetSpotLocation(8, a);
                }
                else
                {
                    lab.SetSpotLocation(4, a);
                }
            }
            else if (a.Y < b.Y)
            {
                if (b.X <= c.X)
                {
                    lab.SetSpotLocation(4, a);
                }
                else
                {
                    lab.SetSpotLocation(2, a);
                }
            }
            else if (a.Y > b.Y)
            {
                if (b.X <= c.X)
                {
                    lab.SetSpotLocation(8, a);
                }
                else
                {
                    lab.SetSpotLocation(0x10, a);
                }
            }
            else if (b.X <= c.X)
            {
                if (b.Y <= c.Y)
                {
                    lab.SetSpotLocation(0x10, b);
                }
                else
                {
                    lab.SetSpotLocation(2, b);
                }
            }
            else if (b.Y <= c.Y)
            {
                lab.SetSpotLocation(8, b);
            }
            else
            {
                lab.SetSpotLocation(4, b);
            }
        }

        protected virtual void PositionMidLabel(DiagramShape lab, PointF a, PointF b)
        {
            PointF tf1 = new PointF((a.X + b.X) / 2f, (a.Y + b.Y) / 2f);
            int num1 = 1;
            if (!this.MidLabelCentered)
            {
                if (a.X < b.X)
                {
                    if (base.IsApprox(a.Y, b.Y))
                    {
                        num1 = 0x80;
                    }
                    else if (a.Y < b.Y)
                    {
                        num1 = 0x10;
                    }
                    else
                    {
                        num1 = 8;
                    }
                }
                else if (base.IsApprox(a.Y, b.Y))
                {
                    num1 = 0x20;
                }
                else if (a.Y < b.Y)
                {
                    num1 = 2;
                }
                else
                {
                    num1 = 4;
                }
            }
            lab.SetSpotLocation(num1, tf1);
        }

        public override void Remove(DiagramShape obj)
        {
            if (obj != null)
            {
                if (obj == this.RealLink)
                {
                    this.RealLink = null;
                }
                else if (obj == this.FromLabel)
                {
                    this.FromLabel = null;
                }
                else if (obj == this.MidLabel)
                {
                    this.MidLabel = null;
                }
                else if (obj == this.ToLabel)
                {
                    this.ToLabel = null;
                }
            }
        }

        public virtual void Unlink()
        {
            DiagramLayer layer1 = base.Layer;
            if (layer1 != null)
            {
                layer1.Remove(this);
            }
        }


        [DefaultValue(0), Category("Behavior"), Description("How CalculateStroke behaves.")]
        public virtual LineAdjustingStyle AdjustingStyle
        {
            get
            {
                return this.RealLink.AdjustingStyle;
            }
            set
            {
                this.RealLink.AdjustingStyle = value;
            }
        }

        [DefaultValue(false), Description("Whether an Orthogonal link tries to avoid crossing over any nodes."), Category("Appearance")]
        public bool AvoidsNodes
        {
            get
            {
                return this.RealLink.AvoidsNodes;
            }
            set
            {
                this.RealLink.AvoidsNodes = value;
            }
        }

        [Category("Appearance"), Description("The brush used to fill any arrowhead.")]
        public System.Drawing.Brush Brush
        {
            get
            {
                return this.RealLink.Brush;
            }
            set
            {
                this.RealLink.Brush = value;
            }
        }

        [Category("Appearance"), Description("How rounded corners are for strokes of style RoundedLine"), DefaultValue((float)10f)]
        public float Curviness
        {
            get
            {
                return this.RealLink.Curviness;
            }
            set
            {
                this.RealLink.Curviness = value;
            }
        }

        [Description("Whether an arrow is drawn at the start of this stroke."), Category("Appearance"), DefaultValue(false)]
        public bool FromArrow
        {
            get
            {
                return this.RealLink.FromArrow;
            }
            set
            {
                this.RealLink.FromArrow = value;
            }
        }

        [Description("Whether the arrowhead is filled with the stroke's brush"), Category("Appearance"), DefaultValue(true)]
        public bool FromArrowFilled
        {
            get
            {
                return this.RealLink.FromArrowFilled;
            }
            set
            {
                this.RealLink.FromArrowFilled = value;
            }
        }

        [Description("The length of the arrowhead at the start of this stroke, along the shaft from the end point to the widest point."), Category("Appearance"), DefaultValue((float)10f)]
        public float FromArrowLength
        {
            get
            {
                return this.RealLink.FromArrowLength;
            }
            set
            {
                this.RealLink.FromArrowLength = value;
            }
        }

        [Category("Appearance"), Description("The length of the arrow along the shaft at the start of this stroke."), DefaultValue((float)8f)]
        public float FromArrowShaftLength
        {
            get
            {
                return this.RealLink.FromArrowShaftLength;
            }
            set
            {
                this.RealLink.FromArrowShaftLength = value;
            }
        }

        [Description("The general shape of the arrowhead at the start of this stroke."), Category("Appearance"), DefaultValue(0)]
        public StrokeArrowheadStyle FromArrowStyle
        {
            get
            {
                return this.RealLink.FromArrowStyle;
            }
            set
            {
                this.RealLink.FromArrowStyle = value;
            }
        }

        [Category("Appearance"), Description("The width at its widest point of the arrowhead at the start of this stroke."), DefaultValue((float)8f)]
        public float FromArrowWidth
        {
            get
            {
                return this.RealLink.FromArrowWidth;
            }
            set
            {
                this.RealLink.FromArrowWidth = value;
            }
        }

        [Category("Labels"), Description("The label object associated with the source end of the link.")]
        public virtual DiagramShape FromLabel
        {
            get
            {
                return this.myFromLabel;
            }
            set
            {
                DiagramShape obj1 = this.myFromLabel;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        base.Remove(obj1);
                    }
                    this.myFromLabel = value;
                    if (value != null)
                    {
                        base.Add(value);
                        if (value == this.MidLabel)
                        {
                            this.myMidLabel = null;
                            this.Changed(0x521, 0, value, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
                        }
                        else if (value == this.ToLabel)
                        {
                            this.myToLabel = null;
                            this.Changed(0x522, 0, value, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
                        }
                    }
                    this.Changed(0x520, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(false), Description("Whether the label at the start (or source end) of the link is positioned on top of the stroke"), Category("Labels")]
        public virtual bool FromLabelCentered
        {
            get
            {
                return ((base.InternalFlags & 0x1000000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x1000000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x1000000;
                    }
                    else
                    {
                        base.InternalFlags &= -16777217;
                    }
                    this.Changed(0x523, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.LayoutChildren(this.FromLabel);
                }
            }
        }

        [Description("The node that the link is coming from.")]
        public virtual IDiagramNode FromNode
        {
            get
            {
                return this.myRealLink.FromNode;
            }
        }

        [DefaultValue((string)null), Description("The port that the link is coming from.")]
        public virtual IDiagramPort FromPort
        {
            get
            {
                return this.myRealLink.FromPort;
            }
            set
            {
                this.myRealLink.FromPort = value;
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

        [Category("Appearance"), Description("Whether a highlight is shown along the path of this stroke."), DefaultValue(false)]
        public bool Highlight
        {
            get
            {
                return this.RealLink.Highlight;
            }
            set
            {
                this.RealLink.Highlight = value;
            }
        }

        [Description("The pen used to draw the highlight."), Category("Appearance"), DefaultValue((string)null)]
        public System.Drawing.Pen HighlightPen
        {
            get
            {
                return this.RealLink.HighlightPen;
            }
            set
            {
                this.RealLink.HighlightPen = value;
            }
        }

        [DefaultValue(0), Description("The width of the pen used to highlight the stroke."), Category("Appearance")]
        public float HighlightPenWidth
        {
            get
            {
                return this.RealLink.HighlightPenWidth;
            }
            set
            {
                this.RealLink.HighlightPenWidth = value;
            }
        }

        [Category("Behavior"), Description("Whether the highlight is shown when this stroke becomes selected."), DefaultValue(false)]
        public bool HighlightWhenSelected
        {
            get
            {
                return this.RealLink.HighlightWhenSelected;
            }
            set
            {
                this.RealLink.HighlightWhenSelected = value;
            }
        }

        [Category("Labels"), Description("The label object associated with the middle of the link.")]
        public virtual DiagramShape MidLabel
        {
            get
            {
                return this.myMidLabel;
            }
            set
            {
                DiagramShape obj1 = this.myMidLabel;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        base.Remove(obj1);
                    }
                    this.myMidLabel = value;
                    if (value != null)
                    {
                        base.Add(value);
                        if (value == this.FromLabel)
                        {
                            this.myFromLabel = null;
                            this.Changed(0x520, 0, value, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
                        }
                        else if (value == this.ToLabel)
                        {
                            this.myToLabel = null;
                            this.Changed(0x522, 0, value, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
                        }
                    }
                    this.Changed(0x521, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether the label at the middle of the link is positioned on top of the stroke"), DefaultValue(false), Category("Labels")]
        public virtual bool MidLabelCentered
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
                    this.Changed(0x524, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.LayoutChildren(this.MidLabel);
                }
            }
        }

        [Description("The Orthogonal property of the RealLink."), Category("Appearance"), DefaultValue(false)]
        public bool Orthogonal
        {
            get
            {
                return this.RealLink.Orthogonal;
            }
            set
            {
                this.RealLink.Orthogonal = value;
            }
        }

        [Category("Ownership"), Description("The unique ID of this part in its document.")]
        public virtual int PartID
        {
            get
            {
                return this.RealLink.PartID;
            }
            set
            {
                this.RealLink.PartID = value;
            }
        }

        [Description("The pen used to draw the stroke."), Category("Appearance")]
        public System.Drawing.Pen Pen
        {
            get
            {
                return this.RealLink.Pen;
            }
            set
            {
                this.RealLink.Pen = value;
            }
        }

        [DefaultValue(0), Category("Appearance"), Description("The width of the pen used to draw the stroke.")]
        public float PenWidth
        {
            get
            {
                return this.RealLink.PenWidth;
            }
            set
            {
                this.RealLink.PenWidth = value;
            }
        }

        [Description("The GoLink object in this group.")]
        public virtual LineGraph RealLink
        {
            get
            {
                return this.myRealLink;
            }
            set
            {
                Shapes.LineGraph link1 = this.myRealLink;
                if (link1 != value)
                {
                    if (link1 != null)
                    {
                        link1.AbstractLink = link1;
                        base.Remove(link1);
                    }
                    this.myRealLink = value;
                    if (value != null)
                    {
                        base.Add(value);
                        value.AbstractLink = this;
                    }
                    this.Changed(0x51f, 0, link1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(true), Description("The Relinkable property of the RealLink."), Category("Behavior")]
        public bool Relinkable
        {
            get
            {
                return this.RealLink.Relinkable;
            }
            set
            {
                this.RealLink.Relinkable = value;
            }
        }

        [DefaultValue(true), Description("The Reshapable property of the RealLink."), Category("Behavior")]
        public override bool Reshapable
        {
            get
            {
                return this.RealLink.Reshapable;
            }
            set
            {
                this.RealLink.Reshapable = value;
            }
        }

        [Description("The Resizable property of the RealLink."), Category("Behavior"), DefaultValue(true)]
        public override bool Resizable
        {
            get
            {
                return this.RealLink.Resizable;
            }
            set
            {
                this.RealLink.Resizable = value;
            }
        }

        public override DiagramShape SelectionObject
        {
            get
            {
                return this.RealLink;
            }
        }

        public override bool Shadowed
        {
            get
            {
                return this.SelectionObject.Shadowed;
            }
            set
            {
                this.SelectionObject.Shadowed = value;
            }
        }

        [Category("Appearance"), DefaultValue(0), Description("The Style property of the RealLink.")]
        public virtual StrokeGraphStyle Style
        {
            get
            {
                return this.RealLink.Style;
            }
            set
            {
                this.RealLink.Style = value;
            }
        }

        [Category("Appearance"), DefaultValue(false), Description("Whether an arrow is drawn at the end of this stroke.")]
        public bool ToArrow
        {
            get
            {
                return this.RealLink.ToArrow;
            }
            set
            {
                this.RealLink.ToArrow = value;
            }
        }

        [Description("Whether the arrowhead is filled with the stroke's brush"), Category("Appearance"), DefaultValue(true)]
        public bool ToArrowFilled
        {
            get
            {
                return this.RealLink.ToArrowFilled;
            }
            set
            {
                this.RealLink.ToArrowFilled = value;
            }
        }

        [Description("The length of the arrow at the end of this stroke, along the shaft from the end point to the widest point."), Category("Appearance"), DefaultValue((float)10f)]
        public float ToArrowLength
        {
            get
            {
                return this.RealLink.ToArrowLength;
            }
            set
            {
                this.RealLink.ToArrowLength = value;
            }
        }

        [DefaultValue((float)8f), Description("The length of the arrow along the shaft at the end of this stroke."), Category("Appearance")]
        public float ToArrowShaftLength
        {
            get
            {
                return this.RealLink.ToArrowShaftLength;
            }
            set
            {
                this.RealLink.ToArrowShaftLength = value;
            }
        }

        [DefaultValue(0), Category("Appearance"), Description("The general shape of the arrowhead at the end of this stroke.")]
        public StrokeArrowheadStyle ToArrowStyle
        {
            get
            {
                return this.RealLink.ToArrowStyle;
            }
            set
            {
                this.RealLink.ToArrowStyle = value;
            }
        }

        [Category("Appearance"), DefaultValue((float)8f), Description("The width of the arrowhead at the widest point.")]
        public float ToArrowWidth
        {
            get
            {
                return this.RealLink.ToArrowWidth;
            }
            set
            {
                this.RealLink.ToArrowWidth = value;
            }
        }

        [Category("Labels"), Description("The label object associated with the destination end of the link.")]
        public virtual DiagramShape ToLabel
        {
            get
            {
                return this.myToLabel;
            }
            set
            {
                DiagramShape obj1 = this.myToLabel;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        base.Remove(obj1);
                    }
                    this.myToLabel = value;
                    if (value != null)
                    {
                        base.Add(value);
                        if (value == this.MidLabel)
                        {
                            this.myMidLabel = null;
                            this.Changed(0x521, 0, value, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
                        }
                        else if (value == this.FromLabel)
                        {
                            this.myFromLabel = null;
                            this.Changed(0x520, 0, value, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
                        }
                    }
                    this.Changed(0x522, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether the label at the destination end of the link is positioned on top of the stroke"), DefaultValue(false), Category("Labels")]
        public virtual bool ToLabelCentered
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
                    this.Changed(0x525, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.LayoutChildren(this.ToLabel);
                }
            }
        }

        [Description("The node that the link is going to.")]
        public virtual IDiagramNode ToNode
        {
            get
            {
                return this.myRealLink.ToNode;
            }
        }

        [Description("The port that the link is going to."), DefaultValue((string)null)]
        public virtual IDiagramPort ToPort
        {
            get
            {
                return this.myRealLink.ToPort;
            }
            set
            {
                this.myRealLink.ToPort = value;
            }
        }

        [Description("An integer value associated with this port."), DefaultValue(0)]
        public virtual int UserFlags
        {
            get
            {
                return this.myRealLink.UserFlags;
            }
            set
            {
                this.myRealLink.UserFlags = value;
            }
        }

        [Description("An object associated with this port."), DefaultValue((string)null)]
        public virtual object UserObject
        {
            get
            {
                return this.myRealLink.UserObject;
            }
            set
            {
                this.myRealLink.UserObject = value;
            }
        }


        public const int ChangedFromLabel = 0x520;
        public const int ChangedFromLabelCentered = 0x523;
        public const int ChangedLink = 0x51f;
        public const int ChangedMidLabel = 0x521;
        public const int ChangedMidLabelCentered = 0x524;
        public const int ChangedToLabel = 0x522;
        public const int ChangedToLabelCentered = 0x525;
        private const bool DEFAULT_ARROW_FILLED = true;
        private const float DEFAULT_ARROW_LENGTH = 10f;
        private const float DEFAULT_ARROW_SHAFT_LENGTH = 8f;
        private const float DEFAULT_ARROW_WIDTH = 8f;
        private const int flagFromLabelCentered = 0x1000000;
        private const int flagMidLabelCentered = 0x2000000;
        private const int flagToLabelCentered = 0x4000000;
        private DiagramShape myFromLabel;
        private DiagramShape myMidLabel;
        private LineGraph myRealLink;
        private DiagramShape myToLabel;
    }
}
