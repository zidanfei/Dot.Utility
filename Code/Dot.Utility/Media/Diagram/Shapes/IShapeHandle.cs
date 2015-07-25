using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram.Shapes
{
    public interface IShapeHandle
    {
        Dot.Utility.Media.Diagram.Shapes.DiagramShape DiagramShape { get; }

        Dot.Utility.Media.Diagram.Shapes.DiagramShape HandledObject { get; }

        int HandleID { get; set; }

        Dot.Utility.Media.Diagram.Shapes.DiagramShape SelectedObject { get; set; }

    }
}
