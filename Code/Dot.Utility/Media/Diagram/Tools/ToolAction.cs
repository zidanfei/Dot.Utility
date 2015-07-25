using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ToolAction : DiagramTool
    {
        public ToolAction(DiagramView v)
            : base(v)
        {
            this.myActionObject = null;
        }

        public override bool CanStart()
        {
            if (base.FirstInput.IsContextButton)
            {
                return false;
            }
            return (this.PickActionObject() != null);
        }

        public override void DoMouseMove()
        {
            if (this.ActionObject != null)
            {
                this.ActionObject.OnActionAdjusted(base.View, base.LastInput);
            }
        }

        public override void DoMouseUp()
        {
            if ((this.ActionObject != null) && (this.ActionObject == this.PickActionObject()))
            {
                this.ActionObject.OnAction(base.View, base.LastInput);
            }
            base.StopTool();
        }

        public virtual IActionObject PickActionObject()
        {
            for (Shapes.DiagramShape obj1 = base.View.PickObject(true, false, base.LastInput.DocPoint, false); obj1 != null; obj1 = obj1.Parent)
            {
                IActionObject obj2 = obj1 as IActionObject;
                if ((obj2 != null) && obj2.ActionEnabled)
                {
                    base.CurrentObject = obj1;
                    return obj2;
                }
            }
            return null;
        }

        public override void Start()
        {
            this.ActionObject = this.PickActionObject();
            if (this.ActionObject == null)
            {
                base.StopTool();
            }
            else
            {
                this.ActionObject.ActionActivated = true;
            }
        }

        public override void Stop()
        {
            if (this.ActionObject != null)
            {
                this.ActionObject.ActionActivated = false;
            }
            this.ActionObject = null;
            base.CurrentObject = null;
        }


        public IActionObject ActionObject
        {
            get
            {
                return this.myActionObject;
            }
            set
            {
                this.myActionObject = value;
            }
        }


        [NonSerialized]
        private IActionObject myActionObject;
    }
}
