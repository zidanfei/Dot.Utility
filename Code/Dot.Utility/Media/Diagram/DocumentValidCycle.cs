using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram
{
    public enum DocumentValidCycle
    {
        All = 0,
        DestinationTree = 4,
        NotDirected = 1,
        NotDirectedFast = 2,
        NotUndirected = 3,
        SourceTree = 5
    }
}
