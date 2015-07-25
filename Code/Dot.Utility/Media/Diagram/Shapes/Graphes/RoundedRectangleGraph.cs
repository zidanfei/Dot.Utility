using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class RoundedRectangleGraph : RectangleGraph
    {
        public RoundedRectangleGraph()
        {
            this.myCorner = new SizeF(10f, 10f);
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            if (e.SubHint == 0x58d)
            {
                this.Corner = e.GetSize(undo);
            }
            else
            {
                base.ChangeValue(e, undo);
            }
        }

        private GraphicsPath GetPath(float offx, float offy)
        {
            GraphicsPath path1;
            if ((offx != 0f) || (offy != 0f))
            {
                path1 = new GraphicsPath(FillMode.Winding);
            }
            else
            {
                if (this.myPath != null)
                {
                    return this.myPath;
                }
                path1 = new GraphicsPath(FillMode.Winding);
                this.myPath = path1;
            }
            RectangleF ef1 = this.Bounds;
            SizeF ef2 = this.Corner;
            RoundedRectangleGraph.MakeRoundedRectangularPath(path1, offx, offy, ef1, ef2);
            return path1;
        }

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = this.GetPath(0f, 0f);
            return (GraphicsPath)path1.Clone();
        }

        internal static void MakeRoundedRectangularPath(GraphicsPath path, float offx, float offy, RectangleF rect, SizeF corner)
        {
            if (corner.Width > (rect.Width / 2f))
            {
                corner.Width = rect.Width / 2f;
            }
            if (corner.Height > (rect.Height / 2f))
            {
                corner.Height = rect.Height / 2f;
            }
            float single1 = corner.Width * 2f;
            float single2 = corner.Height * 2f;
            rect.X += offx;
            rect.Y += offy;
            float single3 = rect.X;
            float single4 = rect.Y;
            float single5 = single3 + single1;
            float single6 = single4 + single2;
            float single7 = (single3 + rect.Width) - single1;
            float single8 = (single4 + rect.Height) - single2;
            float single9 = single3 + rect.Width;
            float single10 = single4 + rect.Height;
            if ((single1 > 0f) && (single2 > 0f))
            {
                path.AddArc(single7, single4, single1, single2, (float)270f, (float)90f);
                if (single6 < single8)
                {
                    path.AddLine(single9, single6, single9, single8);
                }
                path.AddArc(single7, single8, single1, single2, (float)0f, (float)90f);
                if (single5 < single7)
                {
                    path.AddLine(single7, single10, single5, single10);
                }
                path.AddArc(single3, single8, single1, single2, (float)90f, (float)90f);
                if (single6 < single8)
                {
                    path.AddLine(single3, single8, single3, single6);
                }
                path.AddArc(single3, single4, single1, single2, (float)180f, (float)90f);
            }
            else
            {
                path.AddRectangle(rect);
            }
            path.CloseAllFigures();
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            if (this.Shadowed)
            {
                SizeF ef1 = this.GetShadowOffset(view);
                GraphicsPath path1 = this.GetPath(ef1.Width, ef1.Height);
                if (this.Brush != null)
                {
                    Brush brush1 = this.GetShadowBrush(view);
                    DiagramGraph.DrawPath(g, view, null, brush1, path1);
                }
                else if (this.Pen != null)
                {
                    Pen pen1 = this.GetShadowPen(view, base.InternalPenWidth);
                    DiagramGraph.DrawPath(g, view, pen1, null, path1);
                }
                base.DisposePath(path1);
            }
            GraphicsPath path2 = this.GetPath(0f, 0f);
            DiagramGraph.DrawPath(g, view, this.Pen, this.Brush, path2);
            base.DisposePath(path2);
        }


        [Category("Appearance"), Description("The maximum radial width and height of each corner"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF Corner
        {
            get
            {
                return this.myCorner;
            }
            set
            {
                SizeF ef1 = this.myCorner;
                if (((ef1 != value) && (value.Width >= 0f)) && (value.Height >= 0f))
                {
                    this.myCorner = value;
                    base.ResetPath();
                    this.Changed(0x58d, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                }
            }
        }


        public const int ChangedCorner = 0x58d;
        private SizeF myCorner;
    }
}
