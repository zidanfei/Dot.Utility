using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    public interface IDiagramLine : IGraphPart
    {
        IDiagramNode GetOtherNode(IDiagramNode n);

        IDiagramPort GetOtherPort(IDiagramPort p);

        void OnPortChanged(IDiagramPort port, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect);

        void Unlink();


        IDiagramNode FromNode { get; }

        IDiagramPort FromPort { get; set; }

        IDiagramNode ToNode { get; }

        IDiagramPort ToPort { get; set; }

    }
}
