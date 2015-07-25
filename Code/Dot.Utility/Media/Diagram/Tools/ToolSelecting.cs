using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ToolSelecting : DiagramTool
    {
        public ToolSelecting(DiagramView v)
            : base(v)
        {
        }

        public override void Start()
        {
            this.DoSelect(base.LastInput);
            this.DoClick(base.LastInput);
            base.StopTool();
        }

    }
}
