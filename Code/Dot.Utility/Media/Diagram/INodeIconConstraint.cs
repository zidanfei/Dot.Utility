using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Dot.Utility.Media.Diagram
{
    public interface INodeIconConstraint
    {
        SizeF MaximumIconSize { get; }

        SizeF MinimumIconSize { get; }

    }
}
