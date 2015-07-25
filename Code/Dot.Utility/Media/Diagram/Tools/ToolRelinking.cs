using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Dot.Utility.Media.Diagram.Shapes;

namespace Dot.Utility.Media.Diagram.Tools
{
    [Serializable]
    public class ToolRelinking : ToolLinking
    {
        public ToolRelinking(DiagramView v)
            : base(v)
        {
            this.mySelectionHidden = false;
        }

        public override bool CanStart()
        {
            if (base.FirstInput.IsContextButton)
            {
                return false;
            }
            if (!base.View.CanLinkObjects())
            {
                return false;
            }
            IShapeHandle handle1 = this.PickRelinkHandle(base.FirstInput.DocPoint);
            if (handle1 == null)
            {
                return false;
            }
            if (handle1.HandleID == 0x400)
            {
                base.CurrentObject = handle1.HandledObject;
                Shapes.IDiagramLine link1 = handle1.SelectedObject as Shapes.IDiagramLine;
                if (link1 is Shapes.LineGraph)
                {
                    Shapes.LineGraph link2 = (Shapes.LineGraph)link1;
                    if (link2.AbstractLink != null)
                    {
                        link1 = link2.AbstractLink;
                    }
                }
                if (link1 == null)
                {
                    return false;
                }
                base.Link = link1;
                base.OriginalEndPort = base.Link.FromPort;
                return true;
            }
            if (handle1.HandleID != 0x401)
            {
                return false;
            }
            base.CurrentObject = handle1.HandledObject;
            Shapes.IDiagramLine link3 = handle1.SelectedObject as Shapes.IDiagramLine;
            if (link3 is Shapes.LineGraph)
            {
                Shapes.LineGraph link4 = (Shapes.LineGraph)link3;
                if (link4.AbstractLink != null)
                {
                    link3 = link4.AbstractLink;
                }
            }
            if (link3 == null)
            {
                return false;
            }
            base.Link = link3;
            base.OriginalEndPort = base.Link.ToPort;
            return true;
        }

        public virtual IShapeHandle PickRelinkHandle(PointF dc)
        {
            Shapes.DiagramShape obj1 = base.View.PickObject(false, true, dc, true);
            return (obj1 as IShapeHandle);
        }

        public override void Start()
        {
            base.Start();
            Shapes.DiagramShape obj1 = base.CurrentObject;
            if ((obj1 != null) && (base.Selection.GetHandleCount(obj1) > 0))
            {
                this.mySelectionHidden = true;
                obj1.RemoveSelectionHandles(base.Selection);
            }
            this.StartRelink(base.Link, base.OriginalEndPort, base.LastInput.DocPoint);
        }

        public override void Stop()
        {
            if (this.mySelectionHidden)
            {
                this.mySelectionHidden = false;
                Shapes.DiagramShape obj1 = base.CurrentObject;
                if ((obj1 != null) && (obj1.Document == base.View.Document))
                {
                    if (!base.Selection.Contains(base.Link.DiagramShape))
                    {
                        base.Selection.Add(base.Link.DiagramShape);
                    }
                    else
                    {
                        obj1.AddSelectionHandles(base.Selection, base.Link.DiagramShape);
                    }
                }
            }
            base.CurrentObject = null;
            base.Stop();
        }


        [NonSerialized]
        private bool mySelectionHidden;
    }
}
