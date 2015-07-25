using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class OverviewRectangleGraph : RectangleGraph
    {
        public OverviewRectangleGraph()
        {
            this.myChanging = false;
            this.Selectable = false;
            this.Resizable = false;
            this.Pen = new Pen(Color.DarkCyan, 0f);
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
        }

        public override PointF ComputeMove(PointF origLoc, PointF newLoc)
        {
            if (this.ObservedView != null)
            {
                PointF tf1 = this.ObservedView.DocumentTopLeft;
                SizeF ef1 = this.ObservedView.DocumentSize;
                if ((newLoc.X + base.Width) > (tf1.X + ef1.Width))
                {
                    newLoc.X = (tf1.X + ef1.Width) - base.Width;
                }
                if (newLoc.X < tf1.X)
                {
                    newLoc.X = tf1.X;
                }
                if ((newLoc.Y + base.Height) > (tf1.Y + ef1.Height))
                {
                    newLoc.Y = (tf1.Y + ef1.Height) - base.Height;
                }
                if (newLoc.Y < tf1.Y)
                {
                    newLoc.Y = tf1.Y;
                }
                if (this.ObservedView.ShowsNegativeCoordinates)
                {
                    return newLoc;
                }
                if (newLoc.X < 0f)
                {
                    newLoc.X = 0f;
                }
                if (newLoc.Y < 0f)
                {
                    newLoc.Y = 0f;
                }
            }
            return newLoc;
        }

        public override bool ContainsPoint(PointF p)
        {
            RectangleF ef1 = this.Bounds;
            float single1 = 4f / base.View.DocScale;
            DiagramShape.InflateRect(ref ef1, single1, single1);
            if (!DiagramShape.ContainsRect(ef1, p))
            {
                return false;
            }
            DiagramShape.InflateRect(ref ef1, -2f * single1, -2f * single1);
            return !DiagramShape.ContainsRect(ef1, p);
        }

        protected override void OnBoundsChanged(RectangleF old)
        {
            base.OnBoundsChanged(old);
            if ((this.ObservedView != null) && !this.myChanging)
            {
                this.myChanging = true;
                this.ObservedView.DocPosition = base.Position;
                this.myChanging = false;
            }
        }

        public override void OnGotSelection(DiagramSelection sel)
        {
        }

        public void UpdateRectFromView()
        {
            if ((this.ObservedView != null) && !this.myChanging)
            {
                this.myChanging = true;
                this.Bounds = this.ObservedView.DocExtent;
                if (base.View != null)
                {
                    base.View.ScrollRectangleToVisible(this.Bounds);
                }
                this.myChanging = false;
            }
        }


        public DiagramView ObservedView
        {
            get
            {
                DiagramOverview overview1 = base.View as DiagramOverview;
                if (overview1 != null)
                {
                    return overview1.Observed;
                }
                return null;
            }
        }


        [NonSerialized]
        private bool myChanging;
    }
}
