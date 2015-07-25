using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class DiagramShapeEventArgs : InputEventArgs
    {
        public DiagramShapeEventArgs(Dot.Utility.Media.Diagram.Shapes.DiagramShape obj, InputEventArgs evt)
            : base(evt)
        {
            this.myObject = obj;
        }


        public Dot.Utility.Media.Diagram.Shapes.DiagramShape DiagramShape
        {
            get
            {
                return this.myObject;
            }
        }


        private Dot.Utility.Media.Diagram.Shapes.DiagramShape myObject;
    }
}
