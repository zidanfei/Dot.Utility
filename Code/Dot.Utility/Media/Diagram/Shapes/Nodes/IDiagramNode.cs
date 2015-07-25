using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Dot.Utility.Media.Diagram.Shapes
{
    public interface IDiagramNode : IGraphPart
    {
        IEnumerable DestinationLinks { get; }

        IEnumerable Destinations { get; }

        IEnumerable Links { get; }

        IEnumerable Nodes { get; }

        IEnumerable Ports { get; }

        IEnumerable SourceLinks { get; }

        IEnumerable Sources { get; }

    }
}
