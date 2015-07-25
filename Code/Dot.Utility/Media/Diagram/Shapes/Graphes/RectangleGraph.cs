using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class RectangleGraph : DiagramGraph
    {
        public RectangleGraph()
        {
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            RectangleF ef1 = this.Bounds;
            if (this.Shadowed)
            {
                SizeF ef2 = this.GetShadowOffset(view);
                if (this.Brush != null)
                {
                    Brush brush1 = this.GetShadowBrush(view);
                    DiagramGraph.DrawRectangle(g, view, null, brush1, ef1.X + ef2.Width, ef1.Y + ef2.Height, ef1.Width, ef1.Height);
                }
                else if (this.Pen != null)
                {
                    Pen pen1 = this.GetShadowPen(view, base.InternalPenWidth);
                    DiagramGraph.DrawRectangle(g, view, pen1, null, ef1.X + ef2.Width, ef1.Y + ef2.Height, ef1.Width, ef1.Height);
                }
            }
            DiagramGraph.DrawRectangle(g, view, this.Pen, this.Brush, ef1.X, ef1.Y, ef1.Width, ef1.Height);
        }

    }
}
