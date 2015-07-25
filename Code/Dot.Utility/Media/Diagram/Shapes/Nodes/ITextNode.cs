using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram.Shapes
{
    public interface ITextNode
    {
        DiagramText Label { get; }

        string Text { get; }

    }
}
