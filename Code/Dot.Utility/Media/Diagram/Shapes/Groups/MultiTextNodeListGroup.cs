using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    internal sealed class MultiTextNodeListGroup : ListGroup
    {
        public MultiTextNodeListGroup()
        {
        }

        public override void Changed(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            base.Changed(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
            if (subhint == 0x41b)
            {
                GroupShape group1 = base.Parent;
                if (group1 == null)
                {
                    return;
                }
                group1.LayoutChildren(null);
            }
            else if (subhint == 0x41c)
            {
                MultiTextNode node1 = base.Parent as MultiTextNode;
                if (node1 != null)
                {
                    node1.RemoveOnlyPorts(oldI);
                }
            }
        }

        public override float LayoutItem(int i, RectangleF cell)
        {
            if ((this.MTN == null) || (this.MTN.ItemWidth <= 0f))
            {
                return base.LayoutItem(i, cell);
            }
            float single1 = this.MTN.ItemWidth;
            float single2 = cell.Y;
            DiagramShape obj1 = this[i];
            if (obj1 != null)
            {
                if (obj1.CanView())
                {
                    obj1.Bounds = new RectangleF(cell.X, cell.Y, single1, obj1.Height);
                    return (single2 + obj1.Height);
                }
                obj1.Position = new PointF(cell.X, cell.Y);
            }
            return single2;
        }


        public MultiTextNode MTN
        {
            get
            {
                return (MultiTextNode)base.Parent;
            }
        }

        public override System.Windows.Forms.Orientation Orientation
        {
            get
            {
                return System.Windows.Forms.Orientation.Vertical;
            }
            set
            {
            }
        }

    }
}
