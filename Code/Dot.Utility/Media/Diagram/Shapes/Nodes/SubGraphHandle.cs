using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class SubGraphHandle : RectangleGraph
    {
        public SubGraphHandle()
        {
            base.Size = new SizeF(10f, 10f);
            this.Brush = DiagramGraph.Brushes_Gold;
            this.Pen = DiagramGraph.Pens_Black;
            this.Selectable = false;
            this.Resizable = false;
            this.AutoRescales = false;
        }

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = base.MakePath();
            SubGraphNode graph1 = base.Parent as SubGraphNode;
            if (graph1 != null)
            {
                RectangleF ef1 = this.Bounds;
                if (graph1.Collapsible)
                {
                    float single1 = ef1.Y + (ef1.Height / 2f);
                    path1.StartFigure();
                    path1.AddLine((float)(ef1.X + (ef1.Width / 4f)), single1, (float)(ef1.X + ((ef1.Width * 3f) / 4f)), single1);
                    if (!graph1.IsExpanded)
                    {
                        float single2 = ef1.X + (ef1.Width / 2f);
                        path1.StartFigure();
                        path1.AddLine(single2, (float)(ef1.Y + (ef1.Height / 4f)), single2, (float)(ef1.Y + ((ef1.Height * 3f) / 4f)));
                    }
                    return path1;
                }
                path1.AddEllipse((float)(ef1.X + (ef1.Width / 4f)), (float)(ef1.Y + (ef1.Height / 4f)), (float)(ef1.Width / 2f), (float)(ef1.Height / 2f));
            }
            return path1;
        }

        public override bool OnSingleClick(InputEventArgs evt, DiagramView view)
        {
            SubGraphNode graph1 = base.Parent as SubGraphNode;
            if ((graph1 == null) || !graph1.Collapsible)
            {
                return false;
            }
            if (view != null)
            {
                view.StartTransaction();
            }
            string text1 = null;
            if (graph1.IsExpanded)
            {
                graph1.Collapse();
                text1 = "Collapsed SubGraph";
            }
            else if (evt.Control)
            {
                graph1.ExpandAll();
                text1 = "Expanded All SubGraphs";
            }
            else
            {
                graph1.Expand();
                text1 = "Expanded SubGraph";
            }
            if (view != null)
            {
                view.FinishTransaction(text1);
            }
            return true;
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            base.Paint(g, view);
            this.PaintHandle(g, view);
        }

        protected virtual void PaintHandle(Graphics g, DiagramView view)
        {
            SubGraphNode graph1 = base.Parent as SubGraphNode;
            if (graph1 != null)
            {
                RectangleF ef1 = this.Bounds;
                if (graph1.Collapsible)
                {
                    float single1 = ef1.Y + (ef1.Height / 2f);
                    DiagramGraph.DrawLine(g, view, this.Pen, ef1.X + (ef1.Width / 4f), single1, ef1.X + ((ef1.Width * 3f) / 4f), single1);
                    if (graph1.IsExpanded)
                    {
                        return;
                    }
                    float single2 = ef1.X + (ef1.Width / 2f);
                    DiagramGraph.DrawLine(g, view, this.Pen, single2, ef1.Y + (ef1.Height / 4f), single2, ef1.Y + ((ef1.Height * 3f) / 4f));
                }
                else
                {
                    DiagramGraph.DrawEllipse(g, view, this.Pen, null, ef1.X + (ef1.Width / 4f), ef1.Y + (ef1.Height / 4f), ef1.Width / 2f, ef1.Height / 2f);
                }
            }
        }

    }
}
