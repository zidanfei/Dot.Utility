using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ToolLinkingNew : ToolLinking
    {
        public ToolLinkingNew(DiagramView v)
            : base(v)
        {
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
            Shapes.IDiagramPort port1 = this.PickPort(base.FirstInput.DocPoint);
            base.OriginalStartPort = port1;
            if (port1 == null)
            {
                return false;
            }
            if (!this.IsValidFromPort(port1))
            {
                return this.IsValidToPort(port1);
            }
            return true;
        }

        public override void Start()
        {
            base.Start();
            this.StartNewLink(base.OriginalStartPort, base.LastInput.DocPoint);
        }

    }
}
