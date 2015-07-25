using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram
{
    public interface IGraphPart
    {
        Dot.Utility.Media.Diagram.Shapes.DiagramShape DiagramShape { get; set; }

        int UserFlags { get; set; }

        object UserObject { get; set; }

    }
}
