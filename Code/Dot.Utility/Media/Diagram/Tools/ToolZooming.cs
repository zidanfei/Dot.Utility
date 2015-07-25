using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ToolZooming : ToolRubberBanding
    {
        public ToolZooming(DiagramView v)
            : base(v)
        {
            this.myZoomedView = v;
        }

        public override bool CanStart()
        {
            if (base.FirstInput.IsContextButton)
            {
                return false;
            }
            Shapes.DiagramShape obj1 = base.View.PickObject(true, false, base.FirstInput.DocPoint, true);
            return (obj1 == null);
        }

        public override Rectangle ComputeRubberBandBox()
        {
            int num3;
            int num4;
            Point point1 = base.FirstInput.ViewPoint;
            Point point2 = base.LastInput.ViewPoint;
            int num1 = point2.X - point1.X;
            int num2 = point2.Y - point1.Y;
            DiagramView view1 = this.ZoomedView;
            if (((view1 == null) || (view1.DisplayRectangle.Height == 0)) || (num2 == 0))
            {
                return new Rectangle(System.Math.Min(point2.X, point1.X), System.Math.Min(point2.Y, point1.Y), System.Math.Abs((int)(point2.X - point1.X)), System.Math.Abs((int)(point2.Y - point1.Y)));
            }
            Rectangle rectangle1 = view1.DisplayRectangle;
            float single1 = ((float)rectangle1.Width) / ((float)rectangle1.Height);
            if (System.Math.Abs((float)(((float)num1) / ((float)num2))) < single1)
            {
                num3 = point1.X + num1;
                num4 = point1.Y + (((int)System.Math.Ceiling((double)(((float)System.Math.Abs(num1)) / single1))) * ((num2 < 0) ? -1 : 1));
            }
            else
            {
                num3 = point1.X + (((int)System.Math.Ceiling((double)(System.Math.Abs(num2) * single1))) * ((num1 < 0) ? -1 : 1));
                num4 = point1.Y + num2;
            }
            return new Rectangle(System.Math.Min(num3, point1.X), System.Math.Min(num4, point1.Y), System.Math.Abs((int)(num3 - point1.X)), System.Math.Abs((int)(num4 - point1.Y)));
        }

        public override void DoRubberBand(Rectangle box)
        {
            if ((box.Width >= 4) && (box.Height >= 4))
            {
                DiagramView view1 = this.ZoomedView;
                if (view1 != null)
                {
                    RectangleF ef1 = base.View.ConvertViewToDoc(box);
                    Rectangle rectangle1 = view1.DisplayRectangle;
                    view1.DocScale = ((float)rectangle1.Width) / ef1.Width;
                    view1.DocPosition = new PointF(ef1.X, ef1.Y);
                }
            }
        }


        public DiagramView ZoomedView
        {
            get
            {
                return this.myZoomedView;
            }
            set
            {
                this.myZoomedView = value;
            }
        }


        [NonSerialized]
        private DiagramView myZoomedView;
    }
}
