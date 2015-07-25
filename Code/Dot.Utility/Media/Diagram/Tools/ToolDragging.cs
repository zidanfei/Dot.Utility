using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Security;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ToolDragging : DiagramTool
    {
        public ToolDragging(DiagramView v)
            : base(v)
        {
            this.myCopiesEffectiveSelection = false;
            this.myHidesSelectionHandles = true;
            this.myEffectiveSelection = null;
            this.myDragSelection = null;
            this.myDragSelectionOrigObj = null;
            this.myMoveOffset = new SizeF();
            this.mySelectionHidden = false;
            this.myModalDropped = false;
            this.mySelectionSet = false;
        }

        private bool alreadyDragged(Hashtable draggeds, Shapes.DiagramShape o)
        {
            for (Shapes.DiagramShape obj1 = o; obj1 != null; obj1 = obj1.Parent)
            {
                if (draggeds.Contains(obj1))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CanStart()
        {
            if ((!base.View.CanMoveObjects() && !base.View.CanCopyObjects()) && !base.View.AllowDragOut)
            {
                return false;
            }
            if (base.LastInput.IsContextButton)
            {
                return false;
            }
            Size size1 = base.DragSize;
            Point point1 = base.FirstInput.ViewPoint;
            Point point2 = base.LastInput.ViewPoint;
            if ((System.Math.Abs((int)(point1.X - point2.X)) <= (size1.Width / 2)) && (System.Math.Abs((int)(point1.Y - point2.Y)) <= (size1.Height / 2)))
            {
                return false;
            }
            Shapes.DiagramShape obj1 = base.View.PickObject(true, false, base.FirstInput.DocPoint, true);
            if (obj1 == null)
            {
                return false;
            }
            if (!obj1.CanMove() && !obj1.CanCopy())
            {
                return false;
            }
            return true;
        }

        public virtual void ClearDragSelection()
        {
            if (this.DragSelection != null)
            {
                CollectionEnumerator enumerator1 = this.DragSelection.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    enumerator1.Current.Remove();
                }
                this.DragSelection = null;
                base.CurrentObject = this.DragSelectionOriginalObject;
                this.DragSelectionOriginalObject = null;
            }
        }

        public virtual DiagramSelection ComputeEffectiveSelection(Shapes.IDiagramShapeCollection coll, bool move)
        {
            Hashtable hashtable1 = new Hashtable();
            Shapes.DiagramShapeCollection collection1 = null;
            DiagramSelection selection1 = new DiagramSelection(null);
            Shapes.DiagramShapeCollection collection2 = null;
            foreach (Shapes.DiagramShape obj1 in coll)
            {
                Shapes.DiagramShape obj2 = obj1.DraggingObject;
                if (((obj2 != null) && !(move ? !obj2.CanMove() : !obj2.CanCopy())) && !this.alreadyDragged(hashtable1, obj2))
                {
                    if ((collection1 != null) && (obj2 is Shapes.GroupShape))
                    {
                        CollectionEnumerator enumerator2 = collection1.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Shapes.DiagramShape obj3 = enumerator2.Current;
                            if (obj3.IsChildOf(obj2))
                            {
                                hashtable1.Remove(7);
                                if (collection2 == null)
                                {
                                    collection2 = new Shapes.DiagramShapeCollection();
                                }
                                collection2.Add(obj3);
                                selection1.Remove(obj3);
                            }
                        }
                        if ((collection2 != null) && !collection2.IsEmpty)
                        {
                            enumerator2 = collection2.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                Shapes.DiagramShape obj4 = enumerator2.Current;
                                collection1.Remove(obj4);
                            }
                            collection2.Clear();
                        }
                    }
                    hashtable1.Add(obj2, obj2);
                    if (!obj2.IsTopLevel)
                    {
                        if (collection1 == null)
                        {
                            collection1 = new Shapes.DiagramShapeCollection();
                        }
                        collection1.Add(obj2);
                    }
                    selection1.Add(obj2);
                }
            }
            Shapes.DiagramShape[] objArray1 = selection1.CopyArray();
            for (int num1 = 0; num1 < objArray1.Length; num1++)
            {
                Shapes.DiagramShape obj5 = objArray1[num1];
                Shapes.IDiagramNode node1 = obj5 as Shapes.IDiagramNode;
                if (node1 != null)
                {
                    foreach (Shapes.IDiagramLine link1 in node1.DestinationLinks)
                    {
                        if (!this.alreadyDragged(hashtable1, link1.DiagramShape) && ((link1.ToPort == null) || this.alreadyDragged(hashtable1, link1.ToPort.DiagramShape)))
                        {
                            hashtable1.Add(link1.DiagramShape, link1.DiagramShape);
                            selection1.Add(link1.DiagramShape);
                        }
                    }
                    foreach (Shapes.IDiagramLine link2 in node1.SourceLinks)
                    {
                        if (!this.alreadyDragged(hashtable1, link2.DiagramShape) && ((link2.FromPort == null) || this.alreadyDragged(hashtable1, link2.FromPort.DiagramShape)))
                        {
                            hashtable1.Add(link2.DiagramShape, link2.DiagramShape);
                            selection1.Add(link2.DiagramShape);
                        }
                    }
                }
            }
            return selection1;
        }

        public virtual DiagramSelection CreateDragSelection()
        {
            DiagramSelection selection1 = new DiagramSelection(null);
            Shapes.RectangleGraph rectangle1 = new Shapes.RectangleGraph();
            rectangle1.Bounds = base.CurrentObject.Bounds;
            rectangle1.Visible = false;
            base.View.Layers.Default.Add(rectangle1);
            selection1.Add(rectangle1);
            Shapes.DiagramShapeCollection collection1 = new Shapes.DiagramShapeCollection();
            CollectionEnumerator enumerator1 = ((this.EffectiveSelection != null) ? this.EffectiveSelection : base.Selection).GetEnumerator();
            while (enumerator1.MoveNext())
            {
                Shapes.DiagramShape obj1 = enumerator1.Current;
                collection1.Add(obj1.DraggingObject);
            }
            RectangleF ef1 = DiagramDocument.ComputeBounds(collection1, base.View);
            float single1 = base.View.WorldScale.Width;
            if (((ef1.Width * single1) > 2000f) || ((ef1.Height * single1) > 2000f))
            {
                single1 *= System.Math.Min((float)(2000f / (ef1.Width * single1)), (float)(2000f / (ef1.Height * single1)));
            }
            Bitmap bitmap1 = base.View.GetBitmapFromCollection(collection1, ef1, single1, false);
            Shapes.DiagramImage image1 = new Shapes.DiagramImage();
            image1.Image = bitmap1;
            image1.Bounds = new RectangleF(ef1.X, ef1.Y, ((float)bitmap1.Width) / single1, ((float)bitmap1.Height) / single1);
            base.View.Layers.Default.Add(image1);
            selection1.Add(image1);
            return selection1;
        }

        public override void DoCancelMouse()
        {
            if ((base.CurrentObject != null) && (this.DragSelection == null))
            {
                SizeF ef1 = DiagramTool.SubtractPoints(base.FirstInput.DocPoint, this.MoveOffset);
                base.View.MoveSelection((this.EffectiveSelection != null) ? this.EffectiveSelection : base.Selection, DiagramTool.SubtractPoints(ef1, base.CurrentObject.Position), false);
            }
            base.TransactionResult = null;
            base.StopTool();
        }

        public virtual void DoDragDrop(Shapes.IDiagramShapeCollection coll, DragDropEffects allow)
        {
            base.View.DoDragDrop(coll, allow);
        }

        public virtual void DoDragging(InputState evttype)
        {
            if (base.CurrentObject != null)
            {
                SizeF ef1 = DiagramTool.SubtractPoints(base.LastInput.DocPoint, base.CurrentObject.Position);
                SizeF ef2 = new SizeF(ef1.Width - this.MoveOffset.Width, ef1.Height - this.MoveOffset.Height);
                bool flag1 = this.MustBeCopying();
                DiagramViewSnapStyle style1 = base.View.GridSnapDrag;
                if (this.EffectiveSelection == null)
                {
                    this.myEffectiveSelection = this.ComputeEffectiveSelection(base.Selection, !flag1);
                }
                if (evttype != InputState.Finish)
                {
                    bool flag2 = style1 == DiagramViewSnapStyle.Jump;
                    if (flag1 || !base.View.DragsRealtime)
                    {
                        this.MakeDragSelection();
                        base.View.MoveSelection(this.DragSelection, ef2, flag2);
                    }
                    else
                    {
                        this.ClearDragSelection();
                        base.View.MoveSelection(this.EffectiveSelection, ef2, flag2);
                    }
                }
                else
                {
                    SizeF ef3 = new SizeF();
                    if (this.DragSelection != null)
                    {
                        ef3 = DiagramTool.SubtractPoints(base.CurrentObject.Position, this.DragSelectionOriginalObject.Position);
                        this.ClearDragSelection();
                    }
                    else
                    {
                        ef3 = ef2;
                    }
                    bool flag3 = (style1 == DiagramViewSnapStyle.Jump) || (style1 == DiagramViewSnapStyle.After);
                    if (flag1)
                    {
                        if (this.CopiesEffectiveSelection)
                        {
                            base.View.CopySelection(this.ComputeEffectiveSelection(base.Selection, false), ef3, flag3);
                        }
                        else
                        {
                            base.View.CopySelection(base.Selection, ef3, flag3);
                        }
                    }
                    else
                    {
                        if (this.EffectiveSelection == null)
                        {
                            this.myEffectiveSelection = this.ComputeEffectiveSelection(base.Selection, true);
                        }
                        base.View.MoveSelection(this.EffectiveSelection, ef3, flag3);
                    }
                }
            }
        }

        public override void DoMouseMove()
        {
            DragEventArgs args1 = base.LastInput.DragEventArgs;
            if (args1 != null)
            {
                if (this.MustBeCopying())
                {
                    if (this.MayBeCopying())
                    {
                        args1.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        args1.Effect = DragDropEffects.None;
                    }
                }
                else if (this.MustBeMoving())
                {
                    if (this.MayBeMoving())
                    {
                        args1.Effect = DragDropEffects.Move;
                    }
                    else
                    {
                        args1.Effect = DragDropEffects.None;
                    }
                }
                else if (!this.MayBeMoving() && !this.MayBeCopying())
                {
                    args1.Effect = DragDropEffects.None;
                }
                else
                {
                    args1.Effect = DragDropEffects.Move;
                }
            }
            this.DoDragging(InputState.Continue);
            base.View.DoAutoScroll(base.LastInput.ViewPoint);
        }

        public override void DoMouseUp()
        {
            this.myModalDropped = true;
            if (this.MustBeCopying())
            {
                this.DoDragging(InputState.Finish);
                base.TransactionResult = "Copy Selection";
                base.View.RaiseSelectionCopied();
            }
            else
            {
                this.DoDragging(InputState.Finish);
                base.TransactionResult = "Move Selection";
                base.View.RaiseSelectionMoved();
            }
            base.StopTool();
        }

        public virtual void MakeDragSelection()
        {
            if (this.DragSelection == null)
            {
                this.DragSelectionOriginalObject = base.CurrentObject;
                this.DragSelection = this.CreateDragSelection();
                if ((this.DragSelection == null) || this.DragSelection.IsEmpty)
                {
                    this.DragSelectionOriginalObject = null;
                    this.DragSelection = null;
                }
                else
                {
                    base.View.MoveSelection((this.EffectiveSelection != null) ? this.EffectiveSelection : base.Selection, DiagramTool.SubtractPoints(DiagramTool.SubtractPoints(base.FirstInput.DocPoint, this.MoveOffset), this.DragSelectionOriginalObject.Position), false);
                    if (base.CurrentObject.View != base.View)
                    {
                        base.CurrentObject = this.DragSelection.Primary;
                    }
                }
            }
        }

        public virtual bool MayBeCopying()
        {
            if (!base.LastInput.Shift)
            {
                if (!base.View.CanInsertObjects())
                {
                    return false;
                }
                CollectionEnumerator enumerator1 = base.Selection.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    if (!enumerator1.Current.CanCopy())
                    {
                        continue;
                    }
                    return true;
                }
            }
            return false;
        }

        public virtual bool MayBeMoving()
        {
            if (!base.LastInput.Control)
            {
                if (!base.View.CanMoveObjects())
                {
                    return false;
                }
                CollectionEnumerator enumerator1 = base.Selection.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    if (!enumerator1.Current.CanMove())
                    {
                        continue;
                    }
                    return true;
                }
            }
            return false;
        }

        public virtual bool MustBeCopying()
        {
            bool flag1 = base.LastInput.Control;
            bool flag2 = base.View.CanInsertObjects();
            if (flag1)
            {
                return flag2;
            }
            return false;
        }

        public virtual bool MustBeMoving()
        {
            bool flag1 = base.LastInput.Shift;
            bool flag2 = base.View.CanMoveObjects();
            if (flag1)
            {
                return flag2;
            }
            return false;
        }

        public override void Start()
        {
            if (!this.mySelectionSet)
            {
                base.CurrentObject = base.View.PickObject(true, false, base.FirstInput.DocPoint, true);
                if (base.CurrentObject == null)
                {
                    return;
                }
                this.MoveOffset = DiagramTool.SubtractPoints(base.FirstInput.DocPoint, base.CurrentObject.Position);
            }
            base.StartTransaction();
            if (!this.mySelectionSet && !base.Selection.Contains(base.CurrentObject))
            {
                if (base.FirstInput.Shift || base.FirstInput.Control)
                {
                    base.Selection.Add(base.CurrentObject);
                }
                else
                {
                    base.Selection.Select(base.CurrentObject);
                }
            }
            if (this.HidesSelectionHandles)
            {
                this.mySelectionHidden = true;
                base.Selection.RemoveAllSelectionHandles();
            }
            if (!this.mySelectionSet && base.View.AllowDragOut)
            {
                this.myModalDropped = false;
                try
                {
                    SizeF ef1 = DiagramTool.SubtractPoints(base.LastInput.DocPoint, base.Selection.Primary.Position);
                    base.Selection.HotSpot = ef1;
                    this.DoDragDrop(base.Selection, DragDropEffects.Move | (DragDropEffects.Copy | DragDropEffects.Scroll));
                }
                catch (SecurityException exception1)
                {
                    Shapes.DiagramShape.Trace("GoToolDragging Start: " + exception1.ToString());
                    return;
                }
                finally
                {
                    if (!this.myModalDropped)
                    {
                        this.DoCancelMouse();
                    }
                    else
                    {
                        base.StopTool();
                    }
                    base.Selection.HotSpot = new SizeF();
                }
            }
        }

        public override void Stop()
        {
            base.View.StopAutoScroll();
            if (this.mySelectionHidden)
            {
                this.mySelectionHidden = false;
                base.Selection.AddAllSelectionHandles();
            }
            this.ClearDragSelection();
            this.myEffectiveSelection = null;
            this.MoveOffset = new SizeF();
            base.CurrentObject = null;
            this.mySelectionSet = false;
            base.StopTransaction();
        }


        public virtual bool CopiesEffectiveSelection
        {
            get
            {
                return this.myCopiesEffectiveSelection;
            }
            set
            {
                this.myCopiesEffectiveSelection = value;
            }
        }

        public DiagramSelection DragSelection
        {
            get
            {
                return this.myDragSelection;
            }
            set
            {
                this.myDragSelection = value;
            }
        }

        public Shapes.DiagramShape DragSelectionOriginalObject
        {
            get
            {
                return this.myDragSelectionOrigObj;
            }
            set
            {
                this.myDragSelectionOrigObj = value;
            }
        }

        public DiagramSelection EffectiveSelection
        {
            get
            {
                return this.myEffectiveSelection;
            }
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

        public SizeF MoveOffset
        {
            get
            {
                return this.myMoveOffset;
            }
            set
            {
                this.myMoveOffset = value;
            }
        }


        private bool myCopiesEffectiveSelection;
        [NonSerialized]
        private DiagramSelection myDragSelection;
        [NonSerialized]
        private Shapes.DiagramShape myDragSelectionOrigObj;
        [NonSerialized]
        private DiagramSelection myEffectiveSelection;
        private bool myHidesSelectionHandles;
        [NonSerialized]
        private bool myModalDropped;
        [NonSerialized]
        private SizeF myMoveOffset;
        [NonSerialized]
        private bool mySelectionHidden;
        [NonSerialized]
        internal bool mySelectionSet;
    }
}
