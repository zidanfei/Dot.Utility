using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram.Shapes
{
    public enum DiagramPortStyle
    {
        None = 0,
        Object = 1,
        Ellipse = 2,
        Triangle = 3,
        Rectangle = 4,
        Diamond = 5,
        Plus = 6,
        Times = 7,
        PlusTimes = 8,

        TriangleBottomLeft = 0x17,
        TriangleBottomRight = 0x16,
        TriangleMiddleBottom = 0x1a,
        TriangleMiddleLeft = 0x1b,
        TriangleMiddleRight = 0x19,
        TriangleMiddleTop = 0x18,
        TriangleTopLeft = 20,
        TriangleTopRight = 0x15
    }
}
