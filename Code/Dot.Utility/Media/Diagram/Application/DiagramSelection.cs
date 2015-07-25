using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dot.Utility.Media.Diagram.Shapes;
using System.Collections;
using System.Drawing;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class DiagramSelection : Shapes.DiagramShapeCollection
    {
        public DiagramSelection(DiagramView view)
        {
            this.myView = null;
            this.myObjTable = new Hashtable();
            this.myHotSpot = new SizeF();
            this.myHandles = null;
            this.myBoundingHandlePen = null;
            this.myResizeHandlePen = null;
            this.myResizeHandlePenColor = Color.Black;
            this.myResizeHandleBrush = null;
            this.myFocused = true;
            this.myView = view;
        }

        public override void Add(Shapes.DiagramShape obj)
        {
            if (obj != null)
            {
                DiagramView view1 = this.View;
                if ((((view1 == null) || (view1.Selection != this)) || (this.Count < view1.MaximumSelectionCount)) && !this.Contains(obj))
                {
                    if (((view1 != null) && (obj.Document != view1.Document)) && (obj.View != view1))
                    {
                        throw new ArgumentException("Selected objects must belong to the view or its document");
                    }
                    this.addToSelection(obj);
                }
            }
        }

        public void AddAllSelectionHandles()
        {
            CollectionEnumerator enumerator1 = this.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                Shapes.DiagramShape obj1 = enumerator1.Current;
                Shapes.DiagramShape obj2 = obj1.SelectionObject;
                if (obj2 != null)
                {
                    if (obj1.CanView())
                    {
                        obj2.AddSelectionHandles(this, obj1);
                        continue;
                    }
                    obj2.RemoveSelectionHandles(this);
                }
            }
        }

        public virtual void AddHandle(Shapes.DiagramShape obj, IShapeHandle handle)
        {
            if (this.myHandles == null)
            {
                this.myHandles = new Hashtable();
            }
            object obj1 = this.myHandles[obj];
            if (obj1 == null)
            {
                this.myHandles[obj] = handle;
            }
            else if (obj1 is ArrayList)
            {
                ArrayList list1 = (ArrayList)obj1;
                list1.Add(handle);
            }
            else
            {
                ArrayList list2 = new ArrayList();
                list2.Add(obj1);
                list2.Add(handle);
                this.myHandles[obj] = list2;
            }
            if (this.View != null)
            {
                this.View.Layers.Default.Add(handle.DiagramShape);
            }
        }

        private void addToSelection(Shapes.DiagramShape obj)
        {
            base.Add(obj);
            this.myObjTable[obj] = null;
            DiagramView view1 = this.View;
            if (view1 != null)
            {
                if (obj.IsInDocument)
                {
                    obj.OnGotSelection(this);
                }
                view1.RaiseShapeSelected(obj);
            }
        }

        public override bool Contains(Shapes.DiagramShape obj)
        {
            if (obj == null)
            {
                return false;
            }
            return this.myObjTable.ContainsKey(obj);
        }

        public virtual IShapeHandle CreateBoundingHandle(Shapes.DiagramShape obj, Shapes.DiagramShape selectedObj)
        {
            IShapeHandle handle1 = obj.CreateBoundingHandle();
            if (handle1 == null)
            {
                return null;
            }
            handle1.SelectedObject = selectedObj;
            Shapes.DiagramShape obj1 = handle1.DiagramShape;
            if (obj1 == null)
            {
                return null;
            }
            obj1.Selectable = false;
            Shapes.DiagramGraph shape1 = obj1 as Shapes.DiagramGraph;
            if (shape1 != null)
            {
                Color color1 = Color.LightGray;
                DiagramView view1 = this.View;
                if (view1 != null)
                {
                    if (this.Focused)
                    {
                        if ((this.Primary != null) && (this.Primary.SelectionObject == obj))
                        {
                            color1 = view1.PrimarySelectionColor;
                        }
                        else
                        {
                            color1 = view1.SecondarySelectionColor;
                        }
                    }
                    else
                    {
                        color1 = view1.NoFocusSelectionColor;
                    }
                }
                float single1 = view1.BoundingHandlePenWidth;
                float single2 = (single1 == 0f) ? 0f : (single1 / view1.WorldScale.Width);
                if (((this.myBoundingHandlePen == null) || (Shapes.DiagramGraph.GoPenInfo.GetPenColor(this.myBoundingHandlePen, color1) != color1)) || (this.myBoundingHandlePen.Width != single2))
                {
                    this.myBoundingHandlePen = new Pen(color1, single2);
                }
                shape1.Pen = this.myBoundingHandlePen;
                shape1.Brush = null;
            }
            this.AddHandle(obj, handle1);
            return handle1;
        }

        public virtual IShapeHandle CreateResizeHandle(Shapes.DiagramShape obj, Shapes.DiagramShape selectedObj, PointF loc, int handleid, bool filled)
        {
            IShapeHandle handle1 = obj.CreateResizeHandle(handleid);
            if (handle1 == null)
            {
                return null;
            }
            handle1.HandleID = handleid;
            handle1.SelectedObject = selectedObj;
            Shapes.DiagramShape obj1 = handle1.DiagramShape;
            if (obj1 == null)
            {
                return null;
            }
            DiagramView view1 = this.View;
            SizeF ef1 = obj1.Size;
            if ((ef1.Width <= 0f) || (ef1.Height <= 0f))
            {
                if (view1 != null)
                {
                    ef1 = view1.ResizeHandleSize;
                }
                else
                {
                    ef1 = new SizeF(6f, 6f);
                }
            }
            if (view1 != null)
            {
                ef1.Width /= view1.WorldScale.Width;
                ef1.Height /= view1.WorldScale.Height;
            }
            obj1.Bounds = new RectangleF(loc.X - (ef1.Width / 2f), loc.Y - (ef1.Height / 2f), ef1.Width, ef1.Height);
            if (handleid == 0)
            {
                obj1.Selectable = false;
            }
            else
            {
                obj1.Selectable = true;
            }
            Shapes.DiagramGraph shape1 = obj1 as Shapes.DiagramGraph;
            if (shape1 != null)
            {
                Color color1 = Color.LightGray;
                if (view1 != null)
                {
                    if (this.Focused)
                    {
                        if ((this.Primary != null) && (this.Primary.SelectionObject == obj))
                        {
                            color1 = view1.PrimarySelectionColor;
                        }
                        else
                        {
                            color1 = view1.SecondarySelectionColor;
                        }
                    }
                    else
                    {
                        color1 = view1.NoFocusSelectionColor;
                    }
                }
                if (filled)
                {
                    float single1 = view1.ResizeHandlePenWidth;
                    float single2 = (single1 == 0f) ? 0f : (single1 / view1.WorldScale.Width);
                    if (((this.myResizeHandlePen == null) || (Shapes.DiagramGraph.GoPenInfo.GetPenColor(this.myResizeHandlePen, this.myResizeHandlePenColor) != this.myResizeHandlePenColor)) || (this.myResizeHandlePen.Width != single2))
                    {
                        this.myResizeHandlePen = new Pen(this.myResizeHandlePenColor, single2);
                    }
                    shape1.Pen = this.myResizeHandlePen;
                    if ((this.myResizeHandleBrush == null) || (this.myResizeHandleBrush.Color != color1))
                    {
                        this.myResizeHandleBrush = new SolidBrush(color1);
                    }
                    shape1.Brush = this.myResizeHandleBrush;
                }
                else
                {
                    float single3 = view1.ResizeHandlePenWidth;
                    float single4 = (single3 == 0f) ? 0f : ((single3 + 1f) / view1.WorldScale.Width);
                    if (((this.myResizeHandlePen == null) || (Shapes.DiagramGraph.GoPenInfo.GetPenColor(this.myResizeHandlePen, color1) != color1)) || (this.myResizeHandlePen.Width != single4))
                    {
                        this.myResizeHandlePen = new Pen(color1, single4);
                    }
                    shape1.Pen = this.myResizeHandlePen;
                    shape1.Brush = null;
                }
            }
            this.AddHandle(obj, handle1);
            return handle1;
        }

        public virtual IShapeHandle FindHandleByID(Shapes.DiagramShape obj, int id)
        {
            if (this.myHandles != null)
            {
                object obj1 = this.myHandles[obj];
                if (obj1 == null)
                {
                    return null;
                }
                if (obj1 is ArrayList)
                {
                    ArrayList list1 = (ArrayList)obj1;
                    foreach (IShapeHandle handle1 in list1)
                    {
                        if (handle1.HandleID == id)
                        {
                            return handle1;
                        }
                    }
                    return null;
                }
                IShapeHandle handle2 = (IShapeHandle)obj1;
                if (handle2.HandleID == id)
                {
                    return handle2;
                }
            }
            return null;
        }

        public virtual IShapeHandle GetAnExistingHandle(Shapes.DiagramShape obj)
        {
            if (this.myHandles != null)
            {
                object obj1 = this.myHandles[obj];
                if (obj1 == null)
                {
                    return null;
                }
                if (!(obj1 is ArrayList))
                {
                    return (IShapeHandle)obj1;
                }
                ArrayList list1 = (ArrayList)obj1;
                if (list1.Count > 0)
                {
                    return (IShapeHandle)list1[0];
                }
            }
            return null;
        }

        public virtual int GetHandleCount(Shapes.DiagramShape obj)
        {
            if (this.myHandles == null)
            {
                return 0;
            }
            object obj1 = this.myHandles[obj];
            if (obj1 == null)
            {
                return 0;
            }
            if (obj1 is ArrayList)
            {
                ArrayList list1 = (ArrayList)obj1;
                return list1.Count;
            }
            return 1;
        }

        public virtual IEnumerable GetHandleEnumerable(Shapes.DiagramShape obj)
        {
            if (this.myHandles == null)
            {
                return new ArrayList();
            }
            object obj1 = this.myHandles[obj];
            if (obj1 == null)
            {
                return new ArrayList();
            }
            if (obj1 is ArrayList)
            {
                return (ArrayList)obj1;
            }
            ArrayList list1 = new ArrayList();
            list1.Add(obj1);
            return list1;
        }

        public virtual void OnGotFocus()
        {
            this.myFocused = true;
            if (this.View != null)
            {
                if (this.View.HidesSelection)
                {
                    this.AddAllSelectionHandles();
                }
                else if (this.View.NoFocusSelectionColor != this.View.PrimarySelectionColor)
                {
                    this.RemoveAllSelectionHandles();
                    this.AddAllSelectionHandles();
                }
            }
        }

        public virtual void OnLostFocus()
        {
            this.myFocused = false;
            if (this.View != null)
            {
                if (this.View.HidesSelection)
                {
                    this.RemoveAllSelectionHandles();
                }
                else if (this.View.NoFocusSelectionColor != this.View.PrimarySelectionColor)
                {
                    this.RemoveAllSelectionHandles();
                    this.AddAllSelectionHandles();
                }
            }
        }

        public override void Remove(Shapes.DiagramShape obj)
        {
            if (obj != null)
            {
                if (this.Contains(obj))
                {
                    this.removeFromSelection(obj);
                }
                else
                {
                    this.RemoveHandles(obj);
                }
            }
        }

        public void RemoveAllSelectionHandles()
        {
            CollectionEnumerator enumerator1 = this.Backwards.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                Shapes.DiagramShape obj2 = enumerator1.Current.SelectionObject;
                if (obj2 != null)
                {
                    obj2.RemoveSelectionHandles(this);
                }
            }
        }

        private void removeFromSelection(Shapes.DiagramShape obj)
        {
            Shapes.DiagramShape obj1 = this.Primary;
            this.myObjTable.Remove(obj);
            base.Remove(obj);
            DiagramView view1 = this.View;
            if (view1 != null)
            {
                if (obj.IsInDocument)
                {
                    obj.OnLostSelection(this);
                }
                view1.RaiseShapeLostSelection(obj);
                if ((obj1 == obj) && obj1.IsInDocument)
                {
                    obj1 = this.Primary;
                    if (obj1 != null)
                    {
                        obj1.OnLostSelection(this);
                        view1.RaiseShapeLostSelection(obj1);
                        obj1.OnGotSelection(this);
                        view1.RaiseShapeSelected(obj1);
                    }
                }
            }
        }

        public virtual void RemoveHandles(Shapes.DiagramShape obj)
        {
            if (this.myHandles != null)
            {
                object obj1 = this.myHandles[obj];
                if (obj1 != null)
                {
                    if (this.View != null)
                    {
                        ArrayList list1 = obj1 as ArrayList;
                        if (list1 != null)
                        {
                            for (int num1 = 0; num1 < list1.Count; num1++)
                            {
                                IShapeHandle handle1 = (IShapeHandle)list1[num1];
                                Shapes.DiagramShape obj2 = handle1.DiagramShape;
                                handle1.SelectedObject = null;
                                if (obj2 != null)
                                {
                                    DiagramLayer layer1 = obj2.Layer;
                                    if (layer1 != null)
                                    {
                                        layer1.Remove(obj2);
                                    }
                                }
                            }
                        }
                        else
                        {
                            IShapeHandle handle2 = (IShapeHandle)obj1;
                            handle2.SelectedObject = null;
                            Shapes.DiagramShape obj3 = handle2.DiagramShape;
                            if (obj3 != null)
                            {
                                DiagramLayer layer2 = obj3.Layer;
                                if (layer2 != null)
                                {
                                    layer2.Remove(obj3);
                                }
                            }
                        }
                    }
                    this.myHandles.Remove(obj);
                }
            }
        }

        public virtual Shapes.DiagramShape Select(Shapes.DiagramShape obj)
        {
            if (obj == null)
            {
                return null;
            }
            if ((this.Primary != obj) || (this.Count != 1))
            {
                this.Clear();
                this.Add(obj);
            }
            return obj;
        }

        public virtual void Toggle(Shapes.DiagramShape obj)
        {
            if (obj != null)
            {
                if (this.Contains(obj))
                {
                    this.Remove(obj);
                }
                else
                {
                    this.Add(obj);
                }
            }
        }


        public virtual bool Focused
        {
            get
            {
                return this.myFocused;
            }
            set
            {
                this.myFocused = value;
            }
        }

        public virtual SizeF HotSpot
        {
            get
            {
                return this.myHotSpot;
            }
            set
            {
                this.myHotSpot = value;
            }
        }

        public virtual Shapes.DiagramShape Primary
        {
            get
            {
                return this.First;
            }
        }

        public override object SyncRoot
        {
            get
            {
                if (this.View != null)
                {
                    return this.View;
                }
                return this;
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
        private Pen myBoundingHandlePen;
        [NonSerialized]
        private bool myFocused;
        [NonSerialized]
        private Hashtable myHandles;
        private SizeF myHotSpot;
        private Hashtable myObjTable;
        [NonSerialized]
        private SolidBrush myResizeHandleBrush;
        [NonSerialized]
        private Pen myResizeHandlePen;
        private Color myResizeHandlePenColor;
        [NonSerialized]
        private DiagramView myView;
    }
}
