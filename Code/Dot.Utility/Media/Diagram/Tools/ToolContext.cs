using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ToolContext : DiagramTool
    {
        public ToolContext(DiagramView v)
            : base(v)
        {
            this.myBackgroundContextMenu = null;
            this.mySingleSelection = true;
        }

        public override bool CanStart()
        {
            return base.LastInput.IsContextButton;
        }

        public override void DoMouseUp()
        {
            this.DoSelect(base.LastInput);
            base.View.DoContextClick(base.LastInput);
            base.StopTool();
        }

        public override void DoSelect(InputEventArgs evt)
        {
            if ((this.SingleSelection || evt.Control) || evt.Shift)
            {
                base.DoSelect(evt);
            }
            else
            {
                base.CurrentObject = base.View.PickObject(true, false, evt.DocPoint, true);
                if (base.CurrentObject == null)
                {
                    base.Selection.Clear();
                }
                else if (!base.Selection.Contains(base.CurrentObject))
                {
                    base.Selection.Select(base.CurrentObject);
                }
            }
        }

        public override void Start()
        {
            ContextMenu menu1 = base.View.ContextMenu;
            if (menu1 != null)
            {
                base.CurrentObject = base.View.PickObject(true, false, base.LastInput.DocPoint, false);
                if (base.CurrentObject != null)
                {
                    this.myBackgroundContextMenu = menu1;
                    base.View.ContextMenu = null;
                }
            }
        }

        public override void Stop()
        {
            if (this.myBackgroundContextMenu != null)
            {
                base.View.ContextMenu = this.myBackgroundContextMenu;
                this.myBackgroundContextMenu = null;
            }
            base.CurrentObject = null;
        }


        public ContextMenu BackgroundContextMenu
        {
            get
            {
                return this.myBackgroundContextMenu;
            }
        }

        public virtual bool SingleSelection
        {
            get
            {
                return this.mySingleSelection;
            }
            set
            {
                this.mySingleSelection = value;
            }
        }


        [NonSerialized]
        private ContextMenu myBackgroundContextMenu;
        private bool mySingleSelection;
    }
}
