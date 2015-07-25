using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Dot.Utility.Media.Diagram.Shapes
{
    public interface IDiagramShapeCollection : ICollection, IEnumerable
    {
        void Add(DiagramShape obj);

        void Clear();

        bool Contains(DiagramShape obj);

        Shapes.DiagramShape[] CopyArray();

        void CopyTo(DiagramShape[] array, int index);

        void Remove(DiagramShape obj);

        IEnumerable Backwards { get; }

        bool IsEmpty { get; }

    }
}
