using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ToolRubberBanding : DiagramTool
    {
        public ToolRubberBanding(DiagramView v)
            : base(v)
        {
            this.myModal = false;
            this.myAutoScrolling = false;
        }

        private void Activate()
        {
            this.myActive = true;
            this.Box = new Rectangle(base.FirstInput.ViewPoint.X, base.FirstInput.ViewPoint.Y, 0, 0);
            if (!base.FirstInput.Shift && !base.Selection.IsEmpty)
            {
                base.Selection.Clear();
                base.View.Refresh();
            }
        }

        public override bool CanStart()
        {
            if (!base.View.CanSelectObjects())
            {
                return false;
            }
            if (base.LastInput.IsContextButton)
            {
                return false;
            }
            Size size1 = base.DragSize;
            Point point1 = base.FirstInput.ViewPoint;
            Point point2 = base.LastInput.ViewPoint;
            if ((System.Math.Abs((int)(point1.X - point2.X)) <= (size1.Width / 2)) && (System.Math.Abs((int)(point1.Y - point2.Y)) <= (size1.Height / 2)))
            {
                return false;
            }
            Shapes.DiagramShape obj1 = base.View.PickObject(true, false, base.FirstInput.DocPoint, true);
            return (obj1 == null);
        }

        public virtual Rectangle ComputeRubberBandBox()
        {
            PointF tf1 = base.FirstInput.DocPoint;
            PointF tf2 = base.LastInput.DocPoint;
            RectangleF ef1 = new RectangleF(System.Math.Min(tf2.X, tf1.X), System.Math.Min(tf2.Y, tf1.Y), System.Math.Abs((float)(tf2.X - tf1.X)), System.Math.Abs((float)(tf2.Y - tf1.Y)));
            return base.View.ConvertDocToView(ef1);
        }

        public override void DoMouseDown()
        {
            if (this.CanStart())
            {
                this.Activate();
            }
        }

        public override void DoMouseMove()
        {
            if (!this.myActive)
            {
                if (!this.Modal)
                {
                    this.Activate();
                }
            }
            else
            {
                this.Box = this.ComputeRubberBandBox();
                base.View.DrawXorBox(this.Box, true);
                if (this.AutoScrolling)
                {
                    base.View.DoAutoScroll(base.LastInput.ViewPoint);
                }
            }
        }

        public override void DoMouseUp()
        {
            if (this.myActive)
            {
                this.Box = this.ComputeRubberBandBox();
                this.DoRubberBand(this.Box);
            }
            base.StopTool();
        }

        public virtual void DoRubberBand(Rectangle box)
        {
            Size size1 = base.DragSize;
            if ((box.Width <= (size1.Width / 2)) && (box.Height <= (size1.Height / 2)))
            {
                this.DoSelect(base.LastInput);
                this.DoClick(base.LastInput);
            }
            else
            {
                RectangleF ef1 = base.View.ConvertViewToDoc(box);
                base.View.SelectInRectangle(ef1);
            }
        }

        public override void Stop()
        {
            base.View.DrawXorBox(this.Box, false);
            base.View.StopAutoScroll();
            this.myActive = false;
        }


        public virtual bool AutoScrolling
        {
            get
            {
                return this.myAutoScrolling;
            }
            set
            {
                this.myAutoScrolling = value;
            }
        }

        public Rectangle Box
        {
            get
            {
                return this.myBox;
            }
            set
            {
                this.myBox = value;
            }
        }

        public virtual bool Modal
        {
            get
            {
                return this.myModal;
            }
            set
            {
                this.myModal = value;
            }
        }


        [NonSerialized]
        private bool myActive;
        private bool myAutoScrolling;
        [NonSerialized]
        private Rectangle myBox;
        private bool myModal;
    }
}
