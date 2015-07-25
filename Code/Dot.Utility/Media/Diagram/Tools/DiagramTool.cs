using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public abstract class DiagramTool : IDiagramTool
    {
        protected DiagramTool(DiagramView view)
        {
            this.myView = null;
            this.myDragSize = SystemInformation.DragSize;
            this.myStopTransactionName = null;
            this.myCurrentObject = null;
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            this.myView = view;
        }

        public virtual bool CanStart()
        {
            return true;
        }

        public virtual void DoCancelMouse()
        {
            this.StopTool();
        }

        public virtual bool DoClick(InputEventArgs evt)
        {
            if (evt.DoubleClick)
            {
                return this.View.DoDoubleClick(evt);
            }
            return this.View.DoSingleClick(evt);
        }

        public virtual void DoKeyDown()
        {
            if (this.LastInput.Key == Keys.Escape)
            {
                this.DoCancelMouse();
            }
        }

        public virtual void DoMouseDown()
        {
        }

        public virtual void DoMouseHover()
        {
        }

        public virtual void DoMouseMove()
        {
        }

        public virtual void DoMouseUp()
        {
            this.StopTool();
        }

        public virtual void DoMouseWheel()
        {
        }

        public virtual void DoSelect(InputEventArgs evt)
        {
            this.CurrentObject = this.View.PickObject(true, false, evt.DocPoint, true);
            if (this.CurrentObject != null)
            {
                if (evt.Control)
                {
                    this.Selection.Toggle(this.CurrentObject);
                }
                else if (evt.Shift)
                {
                    this.Selection.Add(this.CurrentObject);
                }
                else
                {
                    this.Selection.Select(this.CurrentObject);
                }
            }
            else if (!evt.Control && !evt.Shift)
            {
                this.Selection.Clear();
            }
        }

        public virtual void Start()
        {
        }

        public bool StartTransaction()
        {
            this.TransactionResult = null;
            return this.View.StartTransaction();
        }

        public virtual void Stop()
        {
        }

        public void StopTool()
        {
            if (this.View.Tool == this)
            {
                this.View.Tool = null;
            }
        }

        public bool StopTransaction()
        {
            if (this.TransactionResult == null)
            {
                return this.View.AbortTransaction();
            }
            return this.View.FinishTransaction(this.TransactionResult);
        }

        public static SizeF SubtractPoints(PointF a, PointF b)
        {
            return new SizeF(a.X - b.X, a.Y - b.Y);
        }

        public static SizeF SubtractPoints(PointF a, SizeF b)
        {
            return new SizeF(a.X - b.Width, a.Y - b.Height);
        }

        public static SizeF SubtractPoints(SizeF a, PointF b)
        {
            return new SizeF(a.Width - b.X, a.Height - b.Y);
        }


        public Shapes.DiagramShape CurrentObject
        {
            get
            {
                return this.myCurrentObject;
            }
            set
            {
                this.myCurrentObject = value;
            }
        }

        internal Size DragSize
        {
            get
            {
                return this.myDragSize;
            }
            set
            {
                this.myDragSize = value;
            }
        }

        public InputEventArgs FirstInput
        {
            get
            {
                return this.View.FirstInput;
            }
        }

        public InputEventArgs LastInput
        {
            get
            {
                return this.View.LastInput;
            }
        }

        public DiagramSelection Selection
        {
            get
            {
                return this.View.Selection;
            }
        }

        public string TransactionResult
        {
            get
            {
                return this.myStopTransactionName;
            }
            set
            {
                this.myStopTransactionName = value;
            }
        }

        public DiagramView View
        {
            get
            {
                return this.myView;
            }
            set
            {
                if (value != null)
                {
                    this.myView = value;
                }
            }
        }


        [NonSerialized]
        private Shapes.DiagramShape myCurrentObject;
        private Size myDragSize;
        [NonSerialized]
        private string myStopTransactionName;
        [NonSerialized]
        private DiagramView myView;
    }
}
