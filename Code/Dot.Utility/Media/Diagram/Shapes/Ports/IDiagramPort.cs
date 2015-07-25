using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace Dot.Utility.Media.Diagram.Shapes
{
    public interface IDiagramPort : IGraphPart
    {
        void AddDestinationLink(IDiagramLine l);

        void AddSourceLink(IDiagramLine l);

        bool CanLinkFrom();

        bool CanLinkTo();

        void ClearLinks();

        bool ContainsLink(IDiagramLine l);

        IDiagramLine[] CopyLinksArray();

        bool IsValidLink(IDiagramPort toPort);

        void OnLinkChanged(IDiagramLine link, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect);

        void RemoveLink(IDiagramLine l);


        IEnumerable DestinationLinks { get; }

        int DestinationLinksCount { get; }

        IEnumerable Links { get; }

        int LinksCount { get; }

        IDiagramNode Node { get; }

        IEnumerable SourceLinks { get; }

        int SourceLinksCount { get; }

    }
}
