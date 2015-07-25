using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class NodeIcon : DiagramImage, INodeIconConstraint
    {
        public NodeIcon()
        {
            this.myMinimumIconSize = new SizeF(1f, 1f);
            this.myMaximumIconSize = new SizeF(9999f, 9999f);
            base.InternalFlags &= -3;
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 2050:
                    {
                        this.MinimumIconSize = e.GetSize(undo);
                        return;
                    }
                case 0x803:
                    {
                        this.MaximumIconSize = e.GetSize(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            INodeIconConstraint constraint1 = this.Constraint;
            SizeF ef1 = constraint1.MinimumIconSize;
            SizeF ef2 = constraint1.MaximumIconSize;
            base.DoResize(view, origRect, newPoint, whichHandle, evttype, ef1, ef2);
        }


        public override RectangleF Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                INodeIconConstraint constraint1 = this.Constraint;
                SizeF ef1 = constraint1.MinimumIconSize;
                SizeF ef2 = constraint1.MaximumIconSize;
                float single1 = value.Width;
                if (single1 < ef1.Width)
                {
                    single1 = ef1.Width;
                }
                else if (single1 > ef2.Width)
                {
                    single1 = ef2.Width;
                }
                float single2 = value.Height;
                if (single2 < ef1.Height)
                {
                    single2 = ef1.Height;
                }
                else if (single2 > ef2.Height)
                {
                    single2 = ef2.Height;
                }
                base.Bounds = new RectangleF(value.X, value.Y, single1, single2);
            }
        }

        public virtual INodeIconConstraint Constraint
        {
            get
            {
                INodeIconConstraint constraint1 = base.Parent as INodeIconConstraint;
                if (constraint1 != null)
                {
                    return constraint1;
                }
                return this;
            }
        }

        [TypeConverter(typeof(SizeFConverter)), Description("The maximum size for the icon"), Category("Appearance")]
        public virtual SizeF MaximumIconSize
        {
            get
            {
                return this.myMaximumIconSize;
            }
            set
            {
                SizeF ef1 = this.myMaximumIconSize;
                if (ef1 != value)
                {
                    this.myMaximumIconSize = value;
                    this.Changed(0x803, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("The minimum size for the icon"), Category("Appearance"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF MinimumIconSize
        {
            get
            {
                return this.myMinimumIconSize;
            }
            set
            {
                SizeF ef1 = this.myMinimumIconSize;
                if (ef1 != value)
                {
                    this.myMinimumIconSize = value;
                    this.Changed(2050, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }


        public const int ChangedMaximumIconSize = 0x803;
        public const int ChangedMinimumIconSize = 2050;
        private SizeF myMaximumIconSize;
        private SizeF myMinimumIconSize;
    }
}
