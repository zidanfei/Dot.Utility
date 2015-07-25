using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dot.Utility.Media.Diagram.Shapes;
using System.Drawing;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ToolResizing : DiagramTool
    {
        public ToolResizing(DiagramView v)
            : base(v)
        {
            this.myMinimumSize = new SizeF(1f, 1f);
            this.myMaximumSize = new SizeF(1E+21f, 1E+21f);
            this.myHidesSelectionHandles = true;
            this.myResizeHandle = null;
            this.mySelectionHidden = false;
            this.mySelectedObject = null;
        }

        public override bool CanStart()
        {
            if (base.FirstInput.IsContextButton)
            {
                return false;
            }
            if (!base.View.CanResizeObjects())
            {
                return false;
            }
            IShapeHandle handle1 = this.PickResizeHandle(base.FirstInput.DocPoint);
            if ((handle1 == null) || (handle1.HandledObject == null))
            {
                return false;
            }
            if (!handle1.HandledObject.CanResize())
            {
                return handle1.HandledObject.CanReshape();
            }
            return true;
        }

        public override void DoCancelMouse()
        {
            if (base.CurrentObject != null)
            {
                base.CurrentObject.DoResize(base.View, this.OriginalBounds, this.OriginalPoint, this.ResizeHandle.HandleID, InputState.Cancel, this.MinimumSize, this.MaximumSize);
            }
            base.TransactionResult = null;
            base.StopTool();
        }

        public override void DoMouseMove()
        {
            this.DoResizing(InputState.Continue);
        }

        public override void DoMouseUp()
        {
            this.DoResizing(InputState.Finish);
            base.TransactionResult = "Resize";
            base.View.RaiseShapeResized(base.CurrentObject);
            base.StopTool();
        }

        public virtual void DoResizing(InputState evttype)
        {
            if (base.CurrentObject != null)
            {
                InputEventArgs args1 = base.LastInput;
                DiagramViewSnapStyle style1 = base.View.GridSnapResize;
                if ((style1 == DiagramViewSnapStyle.Jump) || ((style1 == DiagramViewSnapStyle.After) && (evttype == InputState.Finish)))
                {
                    args1.DocPoint = base.View.FindNearestGridPoint(args1.DocPoint);
                    args1.ViewPoint = base.View.ConvertDocToView(args1.DocPoint);
                }
                Shapes.DiagramShape obj1 = base.CurrentObject;
                RectangleF ef1 = obj1.Bounds;
                obj1.DoResize(base.View, this.OriginalBounds, args1.DocPoint, this.ResizeHandle.HandleID, evttype, this.MinimumSize, this.MaximumSize);
                if ((!this.mySelectionHidden && (ef1 == obj1.Bounds)) && (obj1.Document == base.View.Document))
                {
                    obj1.AddSelectionHandles(base.Selection, this.mySelectedObject);
                }
            }
        }

        public virtual IShapeHandle PickResizeHandle(PointF dc)
        {
            Shapes.DiagramShape obj1 = base.View.PickObject(false, true, dc, true);
            return (obj1 as IShapeHandle);
        }

        public override void Start()
        {
            IShapeHandle handle1 = this.PickResizeHandle(base.FirstInput.DocPoint);
            if (handle1 != null)
            {
                Shapes.DiagramShape obj1 = handle1.HandledObject;
                if (obj1 != null)
                {
                    base.CurrentObject = obj1;
                    base.StartTransaction();
                    if (base.Selection.GetHandleCount(obj1) > 0)
                    {
                        this.mySelectedObject = handle1.SelectedObject;
                        if (this.HidesSelectionHandles || !handle1.SelectedObject.ResizesRealtime)
                        {
                            this.mySelectionHidden = true;
                            obj1.RemoveSelectionHandles(base.Selection);
                        }
                    }
                    this.ResizeHandle = handle1;
                    this.OriginalBounds = obj1.Bounds;
                    this.OriginalPoint = handle1.DiagramShape.GetSpotLocation(1);
                }
            }
        }

        public override void Stop()
        {
            base.View.DrawXorBox(new Rectangle(), false);
            if (this.mySelectionHidden)
            {
                this.mySelectionHidden = false;
                Shapes.DiagramShape obj1 = base.CurrentObject;
                if ((obj1 != null) && (obj1.Document == base.View.Document))
                {
                    if (!base.Selection.Contains(this.mySelectedObject))
                    {
                        base.Selection.Add(this.mySelectedObject);
                    }
                    else
                    {
                        obj1.AddSelectionHandles(base.Selection, this.mySelectedObject);
                    }
                }
            }
            this.mySelectedObject = null;
            base.CurrentObject = null;
            this.ResizeHandle = null;
            base.StopTransaction();
        }


        public virtual bool HidesSelectionHandles
        {
            get
            {
                return this.myHidesSelectionHandles;
            }
            set
            {
                this.myHidesSelectionHandles = value;
            }
        }

        public virtual SizeF MaximumSize
        {
            get
            {
                return this.myMaximumSize;
            }
            set
            {
                this.myMaximumSize = value;
            }
        }

        public virtual SizeF MinimumSize
        {
            get
            {
                return this.myMinimumSize;
            }
            set
            {
                this.myMinimumSize = value;
            }
        }

        public RectangleF OriginalBounds
        {
            get
            {
                return this.myOriginalBounds;
            }
            set
            {
                this.myOriginalBounds = value;
            }
        }

        public PointF OriginalPoint
        {
            get
            {
                return this.myOriginalPoint;
            }
            set
            {
                this.myOriginalPoint = value;
            }
        }

        public IShapeHandle ResizeHandle
        {
            get
            {
                return this.myResizeHandle;
            }
            set
            {
                this.myResizeHandle = value;
            }
        }


        private bool myHidesSelectionHandles;
        private SizeF myMaximumSize;
        private SizeF myMinimumSize;
        [NonSerialized]
        private RectangleF myOriginalBounds;
        [NonSerialized]
        private PointF myOriginalPoint;
        [NonSerialized]
        private IShapeHandle myResizeHandle;
        [NonSerialized]
        private Shapes.DiagramShape mySelectedObject;
        [NonSerialized]
        private bool mySelectionHidden;
    }
}
