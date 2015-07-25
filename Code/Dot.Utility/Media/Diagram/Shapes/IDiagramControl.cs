using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram.Shapes
{
    public interface IDiagramControl
    {
        Dot.Utility.Media.Diagram.Shapes.DiagramControl DiagramControl { get; set; }

        Dot.Utility.Media.Diagram.DiagramView DiagramView { get; set; }

    }
}
