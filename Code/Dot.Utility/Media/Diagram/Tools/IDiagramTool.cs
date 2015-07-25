using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram
{
    public interface IDiagramTool
    {
        bool CanStart();

        void DoCancelMouse();

        void DoKeyDown();

        void DoMouseDown();

        void DoMouseHover();

        void DoMouseMove();

        void DoMouseUp();

        void DoMouseWheel();

        void Start();

        void Stop();


        DiagramView View { get; set; }

    }
}
