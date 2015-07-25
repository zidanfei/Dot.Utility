using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class SelectionEventArgs : EventArgs
    {
        public SelectionEventArgs(Dot.Utility.Media.Diagram.Shapes.DiagramShape DiagramShape)
        {
            this._DiagramShape = DiagramShape;
        }

        private Dot.Utility.Media.Diagram.Shapes.DiagramShape _DiagramShape;
        public Dot.Utility.Media.Diagram.Shapes.DiagramShape DiagramShape
        {
            get
            {
                return this._DiagramShape;
            }
        }
    }
}
