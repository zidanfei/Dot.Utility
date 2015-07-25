using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram
{
    public interface IActionObject
    {
        void OnAction(DiagramView view, InputEventArgs e);

        void OnActionAdjusted(DiagramView view, InputEventArgs e);

        bool ActionActivated { get; set; }

        bool ActionEnabled { get; set; }

    }
}
