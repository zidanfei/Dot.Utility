using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Dot.Utility.Media.Diagram
{
    [ToolboxBitmap(typeof(DiagramOverview), "Dot.Utility.Media.Diagram.DiagramOverview.bmp")]
    public class DiagramOverview : DiagramView
    {
        public DiagramOverview()
        {
            this.myObserved = null;
            this.myObservedDocument = null;
            this.myOverviewRect = null;
            this.myZoomTool = null;
            this.myDocChangedEventHandler = null;
            this.myViewResizedEventHandler = null;
            this.myViewPropertyChangedEventHandler = null;
            this.myZoomTool = new ToolZooming(this);
            this.ReplaceMouseTool(typeof(ToolRubberBanding), this.myZoomTool);
            this.SetModifiable(false);
            base.AllowSelect = false;
            base.AllowCopy = false;
            base.AllowMove = true;
            base.AllowDragOut = false;
            base.InitAllowDrop(false);
            this.DragsRealtime = true;
            this.DocScale = 0.125f;
        }

        private void AddListeners()
        {
            if (this.myDocChangedEventHandler == null)
            {
                this.myDocChangedEventHandler = new ChangedEventHandler(this.SafeOnDocumentChanged);
                this.myViewResizedEventHandler = new EventHandler(this.ComponentResized);
                this.myViewPropertyChangedEventHandler = new PropertyChangedEventHandler(this.ViewChanged);
            }
            if (this.myObservedDocument != null)
            {
                this.myObservedDocument.Changed += this.myDocChangedEventHandler;
            }
            if (this.myObserved != null)
            {
                this.myObserved.Resize += this.myViewResizedEventHandler;
                this.myObserved.PropertyChanged += this.myViewPropertyChangedEventHandler;
            }
        }

        protected void ComponentResized(object sender, EventArgs e)
        {
            if (this.OverviewRect != null)
            {
                this.OverviewRect.UpdateRectFromView();
            }
        }

        public virtual Shapes.OverviewRectangleGraph CreateOverviewRectangle(DiagramView observed)
        {
            return new Shapes.OverviewRectangleGraph();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.RemoveListeners();
            this.myObserved = null;
        }

        public override bool DoMouseOver(InputEventArgs evt)
        {
            if ((this.OverviewRect != null) && this.OverviewRect.ContainsPoint(evt.DocPoint))
            {
                this.Cursor = Cursors.SizeAll;
            }
            else
            {
                this.Cursor = this.DefaultCursor;
            }
            Shapes.DiagramShape obj1 = this.Document.PickObject(evt.DocPoint, false);
            this.DoToolTipObject(obj1);
            return true;
        }

        public override void InitializeLayersFromDocument()
        {
            base.InitializeLayersFromDocument();
            if (this.Observed != null)
            {
                this.myOverviewRect = this.CreateOverviewRectangle(this.Observed);
                this.myOverviewRect.Bounds = this.Observed.DocExtent;
                base.Layers.Default.Add(this.myOverviewRect);
            }
        }

        protected override void OnBackgroundSingleClicked(InputEventArgs evt)
        {
            base.OnBackgroundSingleClicked(evt);
            if (this.OverviewRect != null)
            {
                RectangleF ef1 = this.OverviewRect.Bounds;
                PointF tf1 = new PointF(evt.DocPoint.X - (ef1.Width / 2f), evt.DocPoint.Y - (ef1.Height / 2f));
                this.OverviewRect.Location = this.OverviewRect.ComputeMove(this.OverviewRect.Location, tf1);
            }
        }

        public override Shapes.DiagramShape PickObject(bool doc, bool view, PointF p, bool selectableOnly)
        {
            if ((this.OverviewRect != null) && this.OverviewRect.ContainsPoint(p))
            {
                return this.OverviewRect;
            }
            return null;
        }

        private void RemoveListeners()
        {
            if (this.myObservedDocument != null)
            {
                this.myObservedDocument.Changed -= this.myDocChangedEventHandler;
            }
            if (this.myObserved != null)
            {
                this.myObserved.Resize -= this.myViewResizedEventHandler;
                this.myObserved.PropertyChanged -= this.myViewPropertyChangedEventHandler;
            }
        }

        protected void ViewChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "DocPosition") || (e.PropertyName == "DocScale"))
            {
                if (this.OverviewRect != null)
                {
                    this.OverviewRect.UpdateRectFromView();
                }
            }
            else if ((e.PropertyName == "Document") && (sender is DiagramView))
            {
                if (this.myObservedDocument != null)
                {
                    this.myObservedDocument.Changed -= this.myDocChangedEventHandler;
                }
                this.myObservedDocument = ((DiagramView)sender).Document;
                if (this.myObservedDocument != null)
                {
                    this.myObservedDocument.Changed += this.myDocChangedEventHandler;
                }
                this.InitializeLayersFromDocument();
                if (this.OverviewRect != null)
                {
                    this.OverviewRect.UpdateRectFromView();
                }
            }
        }


        public override DiagramDocument Document
        {
            get
            {
                if (this.myObservedDocument != null)
                {
                    return this.myObservedDocument;
                }
                return base.Document;
            }
            set
            {
                base.Document = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual DiagramView Observed
        {
            get
            {
                return this.myObserved;
            }
            set
            {
                if (((value != this) && !(value is DiagramOverview)) && (this.myObserved != value))
                {
                    this.RemoveListeners();
                    this.myObserved = value;
                    if (this.myObserved != null)
                    {
                        this.myZoomTool.ZoomedView = this.myObserved;
                        this.myObservedDocument = this.myObserved.Document;
                        this.AddListeners();
                    }
                    else
                    {
                        this.myZoomTool.ZoomedView = this;
                        this.myObservedDocument = null;
                        this.myOverviewRect = null;
                    }
                    this.InitializeLayersFromDocument();
                    this.UpdateView();
                    base.RaisePropertyChangedEvent("Observed");
                }
            }
        }

        [Browsable(false)]
        public Shapes.OverviewRectangleGraph OverviewRect
        {
            get
            {
                return this.myOverviewRect;
            }
        }

        public override bool ShowsNegativeCoordinates
        {
            get
            {
                if (this.Observed != null)
                {
                    return this.Observed.ShowsNegativeCoordinates;
                }
                return false;
            }
            set
            {
                base.ShowsNegativeCoordinates = value;
            }
        }


        [NonSerialized]
        private ChangedEventHandler myDocChangedEventHandler;
        private DiagramView myObserved;
        private DiagramDocument myObservedDocument;
        private Shapes.OverviewRectangleGraph myOverviewRect;
        [NonSerialized]
        private PropertyChangedEventHandler myViewPropertyChangedEventHandler;
        [NonSerialized]
        private EventHandler myViewResizedEventHandler;
        private ToolZooming myZoomTool;
    }
}
