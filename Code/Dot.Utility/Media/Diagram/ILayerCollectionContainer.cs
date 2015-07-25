using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Dot.Utility.Media.Diagram
{
    public interface ILayerCollectionContainer : ILayerAbilities
    {
        void RaiseChanged(int hint, int subhint, object obj, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect);


        LayerCollection Layers { get; }

    }
}
