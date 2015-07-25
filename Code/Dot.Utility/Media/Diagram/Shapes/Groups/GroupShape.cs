using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class GroupShape : DiagramShape, IDiagramShapeCollection, IList, ICollection, IEnumerable
    {
        public GroupShape()
        {
            this.myObjects = new ArrayList();
            this.myPaintBoundsShadowOffset = new SizeF();
            this.myLeft = 0f;
            this.myTop = 0f;
            this.myRight = 0f;
            this.myBottom = 0f;
        }

        public virtual void Add(DiagramShape obj)
        {
            if (obj != null)
            {
                GroupShape group1 = obj.Parent;
                if (group1 == null)
                {
                    if (obj.Layer != null)
                    {
                        throw new ArgumentException("Cannot add an object to a group when it is already part of a document or view.");
                    }
                    this.insertAt(this.myObjects.Count, obj, false);
                }
                else if (group1 != this)
                {
                    throw new ArgumentException("Cannot move an object from one group to another without first removing it from its parent.");
                }
            }
        }

        public int Add(object obj)
        {
            this.Add((DiagramShape)obj);
            return (this.Count - 1);
        }

        public virtual IDiagramShapeCollection AddCollection(IDiagramShapeCollection coll, bool reparentLinks)
        {
            foreach (DiagramShape obj1 in coll)
            {
                if (!base.IsChildOf(obj1) && (this != obj1))
                {
                    continue;
                }
                throw new ArgumentException("Cannot add a group to itself or to one of its own children.");
            }
            DiagramShapeCollection collection1 = new DiagramShapeCollection();
            foreach (DiagramShape obj2 in coll)
            {
                collection1.Add(obj2);
            }
            CollectionEnumerator enumerator2 = collection1.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                DiagramShape obj3 = enumerator2.Current;
                bool flag1 = obj3.Layer != null;
                if (flag1)
                {
                    GroupShape.setAllNoClear(obj3, true);
                    obj3.Remove();
                }
                this.Add(obj3);
                if (flag1)
                {
                    GroupShape.setAllNoClear(obj3, false);
                }
            }
            if (reparentLinks && base.IsInDocument)
            {
                SubGraphNode.ReparentAllLinksToSubGraphs(collection1, true, base.Document.LinksLayer);
            }
            return collection1;
        }

        private void CalculatePaintBounds(DiagramView view)
        {
            base.InternalFlags &= -1048577;
            RectangleF ef1 = this.Bounds;
            float single1 = ef1.X;
            float single2 = ef1.Y;
            float single3 = single1 + ef1.Width;
            float single4 = single2 + ef1.Height;
            GroupEnumerator enumerator2 = this.GetEnumerator();
            GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj1 = enumerator1.Current;
                RectangleF ef2 = obj1.Bounds;
                ef2 = obj1.ExpandPaintBounds(ef2, view);
                single1 = System.Math.Min(single1, ef2.X);
                single2 = System.Math.Min(single2, ef2.Y);
                single3 = System.Math.Max(single3, (float)(ef2.X + ef2.Width));
                single4 = System.Math.Max(single4, (float)(ef2.Y + ef2.Height));
            }
            if (view != null)
            {
                this.myPaintBoundsShadowOffset = this.GetShadowOffset(view);
            }
            this.myLeft = ef1.X - single1;
            this.myTop = ef1.Y - single2;
            this.myRight = single3 - (ef1.X + ef1.Width);
            this.myBottom = single4 - (ef1.Y + ef1.Height);
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x41b:
                    {
                        int num1 = e.NewInt;
                        DiagramShape obj1 = e.NewValue as DiagramShape;
                        if (!undo)
                        {
                            if (num1 < 0)
                            {
                                num1 = this.myObjects.Count;
                            }
                            if (this.myObjects.IndexOf(obj1) < 0)
                            {
                                this.insertAt(num1, obj1, true);
                            }
                            return;
                        }
                        if (num1 < 0)
                        {
                            num1 = this.myObjects.IndexOf(obj1);
                        }
                        if (num1 >= 0)
                        {
                            this.removeAt(num1, obj1, true);
                        }
                        return;
                    }
                case 0x41c:
                    {
                        int num2 = e.OldInt;
                        DiagramShape obj2 = e.OldValue as DiagramShape;
                        if (!undo)
                        {
                            if (num2 < 0)
                            {
                                num2 = this.myObjects.IndexOf(obj2);
                            }
                            if (num2 >= 0)
                            {
                                this.removeAt(num2, obj2, true);
                            }
                            return;
                        }
                        if (num2 < 0)
                        {
                            num2 = this.myObjects.Count;
                        }
                        if (this.myObjects.IndexOf(obj2) < 0)
                        {
                            this.insertAt(num2, obj2, true);
                        }
                        return;
                    }
                case 0x41d:
                    {
                        DiagramShape obj3 = (DiagramShape)e.OldValue;
                        int num3 = e.OldInt;
                        int num4 = e.NewInt;
                        this.myObjects.Remove(obj3);
                        if (!undo)
                        {
                            this.moveTo(num4, obj3, num3);
                            return;
                        }
                        this.moveTo(num3, obj3, num4);
                        return;
                    }
                case 0x41e:
                    {
                        DiagramShape obj4 = (DiagramShape)e.OldValue;
                        DiagramShape obj5 = (DiagramShape)e.NewValue;
                        int num5 = e.OldInt;
                        if (!undo)
                        {
                            this.replaceAt(num5, obj5, true);
                            return;
                        }
                        this.replaceAt(num5, obj4, true);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        public virtual void Clear()
        {
            for (int num1 = this.myObjects.Count; num1 > 0; num1 = System.Math.Min(num1, this.myObjects.Count))
            {
                DiagramShape obj1 = (DiagramShape)this.myObjects[--num1];
                this.Remove(obj1);
            }
        }

        protected override RectangleF ComputeBounds()
        {
            RectangleF ef1 = this.Bounds;
            bool flag1 = false;
            GroupEnumerator enumerator2 = this.GetEnumerator();
            GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj1 = enumerator1.Current;
                if (!flag1)
                {
                    ef1 = obj1.Bounds;
                    flag1 = true;
                    continue;
                }
                ef1 = DiagramShape.UnionRect(ef1, obj1.Bounds);
            }
            return ef1;
        }

        public virtual bool Contains(DiagramShape obj)
        {
            if (obj != null)
            {
                return (obj.Parent == this);
            }
            return false;
        }

        public bool Contains(object obj)
        {
            return this.Contains((DiagramShape)obj);
        }

        public override bool ContainsPoint(PointF p)
        {
            if (DiagramShape.ContainsRect(this.Bounds, p))
            {
                GroupEnumerator enumerator2 = this.GetEnumerator();
                GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj1 = enumerator1.Current;
                    if (obj1.CanView() && obj1.ContainsPoint(p))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual DiagramShape[] CopyArray()
        {
            DiagramShape[] objArray1 = new DiagramShape[this.Count];
            this.CopyTo(objArray1, 0);
            return objArray1;
        }

        protected virtual void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            GroupEnumerator enumerator2 = this.GetEnumerator();
            GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj1 = enumerator1.Current;
                DiagramShape obj2 = env.Copy(obj1);
                newgroup.Add(obj2);
            }
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            GroupShape group1 = (GroupShape)base.CopyObject(env);
            if (group1 != null)
            {
                group1.myObjects = new ArrayList();
                bool flag1 = group1.Initializing;
                group1.Initializing = true;
                this.CopyChildren(group1, env);
                group1.Initializing = flag1;
            }
            return group1;
        }

        public void CopyTo(DiagramShape[] array, int index)
        {
            //modified by little
            //this.CopyTo(array, index);
            this.myObjects.CopyTo(array, index);
        }

        public virtual void CopyTo(Array array, int index)
        {
            this.myObjects.CopyTo(array, index);
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            if ((((base.InternalFlags & 0x100000) != 0) || (view == null)) || (this.myPaintBoundsShadowOffset != this.GetShadowOffset(view)))
            {
                this.CalculatePaintBounds(view);
            }
            return new RectangleF(rect.X - this.myLeft, rect.Y - this.myTop, (rect.Width + this.myLeft) + this.myRight, (rect.Height + this.myTop) + this.myBottom);
        }

        public GroupEnumerator GetEnumerator()
        {
            return new GroupEnumerator(this.myObjects, true);
        }

        public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            float single1 = 1E+21f;
            PointF tf2 = new PointF();
            GroupEnumerator enumerator2 = this.GetEnumerator();
            GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                PointF tf1;
                DiagramShape obj1 = enumerator1.Current;
                if (obj1.CanView() && obj1.GetNearestIntersectionPoint(p1, p2, out tf1))
                {
                    float single2 = ((tf1.X - p1.X) * (tf1.X - p1.X)) + ((tf1.Y - p1.Y) * (tf1.Y - p1.Y));
                    if (single2 < single1)
                    {
                        single1 = single2;
                        tf2 = tf1;
                    }
                }
            }
            result = tf2;
            return (single1 < 1E+21f);
        }

        public virtual int IndexOf(DiagramShape obj)
        {
            return this.myObjects.IndexOf(obj);
        }

        public int IndexOf(object obj)
        {
            return this.IndexOf((DiagramShape)obj);
        }

        public virtual void Insert(int index, DiagramShape obj)
        {
            if (index == this.Count)
            {
                this.Add(obj);
            }
            else
            {
                DiagramShape obj1 = (DiagramShape)this.myObjects[index];
                this.InsertBefore(obj1, obj);
            }
        }

        public void Insert(int index, object obj)
        {
            this.Insert(index, (DiagramShape)obj);
        }

        public virtual void InsertAfter(DiagramShape child, DiagramShape newobj)
        {
            if (newobj != null)
            {
                if ((child != null) && (child.Parent != this))
                {
                    throw new ArgumentException("Cannot insert an object into a group after a child that is not a member of the group.");
                }
                GroupShape group1 = newobj.Parent;
                if (group1 == null)
                {
                    if (newobj.Layer != null)
                    {
                        throw new ArgumentException("Cannot add an object to a group when it is already part of a document or view.");
                    }
                    int num1 = (child == null) ? (this.myObjects.Count - 1) : this.myObjects.IndexOf(child);
                    this.insertAt(num1 + 1, newobj, false);
                }
                else
                {
                    if (group1 != this)
                    {
                        throw new ArgumentException("Cannot move an object from one group to another without first removing it from its parent.");
                    }
                    if (newobj == child)
                    {
                        throw new ArgumentException("Cannot insert an object into a group after itself.");
                    }
                    int num2 = this.myObjects.IndexOf(newobj);
                    int num3 = (child == null) ? (this.myObjects.Count - 1) : this.myObjects.IndexOf(child);
                    if (num3 > num2)
                    {
                        num3--;
                    }
                    if ((num3 != num2) && ((num3 + 1) != num2))
                    {
                        this.myObjects.RemoveAt(num2);
                        this.moveTo(num3 + 1, newobj, num2);
                    }
                }
            }
        }

        private void insertAt(int idx, DiagramShape obj, bool undoing)
        {
            RectangleF ef1 = obj.Bounds;
            if (!undoing || (this.myObjects.IndexOf(obj) < 0))
            {
                if ((idx < 0) || (idx > this.myObjects.Count))
                {
                    idx = this.myObjects.Count;
                }
                this.myObjects.Insert(idx, obj);
            }
            obj.SetParent(this, undoing);
            this.Changed(0x41b, 0, null, DiagramShape.NullRect, idx, obj, ef1);
            if (!undoing)
            {
                this.LayoutChildren(obj);
                base.InvalidBounds = true;
                RectangleF ef2 = this.Bounds;
            }
        }

        public virtual void InsertBefore(DiagramShape child, DiagramShape newobj)
        {
            if (newobj != null)
            {
                if ((child != null) && (child.Parent != this))
                {
                    throw new ArgumentException("Cannot insert an object into a group before (behind) a child that is not a member of the group.");
                }
                GroupShape group1 = newobj.Parent;
                if (group1 == null)
                {
                    if (newobj.Layer != null)
                    {
                        throw new ArgumentException("Cannot add an object to a group when it is already part of a document or view.");
                    }
                    int num1 = (child == null) ? 0 : this.myObjects.IndexOf(child);
                    this.insertAt(num1, newobj, false);
                }
                else
                {
                    if (group1 != this)
                    {
                        throw new ArgumentException("Cannot move an object from one group to another without first removing it from its parent.");
                    }
                    if (newobj == child)
                    {
                        throw new ArgumentException("Cannot insert an object into a group before (behind) itself.");
                    }
                    int num2 = this.myObjects.IndexOf(newobj);
                    int num3 = (child == null) ? 0 : this.myObjects.IndexOf(child);
                    if (num3 > num2)
                    {
                        num3--;
                    }
                    if (num3 != num2)
                    {
                        this.myObjects.RemoveAt(num2);
                        this.moveTo(num3, newobj, num2);
                    }
                }
            }
        }

        internal void InvalidatePaintBounds()
        {
            base.InternalFlags |= 0x100000;
            if (base.Parent != null)
            {
                base.Parent.InvalidatePaintBounds();
            }
        }

        public virtual void LayoutChildren(DiagramShape childchanged)
        {
        }

        protected virtual void MoveChildren(RectangleF old)
        {
            float single1 = base.Left - old.X;
            float single2 = base.Top - old.Y;
            GroupEnumerator enumerator2 = this.GetEnumerator();
            GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj1 = enumerator1.Current;
                RectangleF ef1 = obj1.Bounds;
                obj1.Bounds = new RectangleF(ef1.X + single1, ef1.Y + single2, ef1.Width, ef1.Height);
            }
        }

        private void moveTo(int newidx, DiagramShape obj, int oldidx)
        {
            RectangleF ef1 = obj.Bounds;
            this.myObjects.Insert(newidx, obj);
            this.Changed(0x41d, oldidx, obj, ef1, newidx, obj, ef1);
        }

        protected override void OnBoundsChanged(RectangleF old)
        {
            base.OnBoundsChanged(old);
            SizeF ef1 = base.Size;
            if ((old.Width == ef1.Width) && (old.Height == ef1.Height))
            {
                this.MoveChildren(old);
            }
            else
            {
                this.RescaleChildren(old);
                this.LayoutChildren(null);
                base.InvalidBounds = true;
            }
        }

        protected internal virtual void OnChildBoundsChanged(DiagramShape child, RectangleF old)
        {
            this.LayoutChildren(child);
            base.InvalidBounds = true;
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            bool flag1 = view.IsPrinting;
            RectangleF ef1 = g.ClipBounds;
            bool flag2 = DiagramShape.ContainsRect(ef1, this.Bounds);
            GroupEnumerator enumerator2 = this.GetEnumerator();
            GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj1 = enumerator1.Current;
                if (flag1 ? obj1.CanPrint() : obj1.CanView())
                {
                    bool flag3 = flag2;
                    if (!flag3)
                    {
                        RectangleF ef2 = obj1.Bounds;
                        ef2 = obj1.ExpandPaintBounds(ef2, view);
                        flag3 = DiagramShape.IntersectsRect(ef2, ef1);
                    }
                    if (flag3)
                    {
                        obj1.Paint(g, view);
                    }
                }
            }
        }

        public override DiagramShape Pick(PointF p, bool selectableOnly)
        {
            if (this.CanView())
            {
                if (!DiagramShape.ContainsRect(this.Bounds, p))
                {
                    return null;
                }
                foreach (DiagramShape obj1 in this.Backwards)
                {
                    DiagramShape obj2 = obj1.Pick(p, selectableOnly);
                    if (obj2 != null)
                    {
                        return obj2;
                    }
                }
            }
            return null;
        }

        public virtual IDiagramShapeCollection PickObjects(PointF p, bool selectableOnly, IDiagramShapeCollection coll, int max)
        {
            if (coll == null)
            {
                coll = new DiagramShapeCollection();
            }
            if (coll.Count < max)
            {
                if (!this.CanView())
                {
                    return coll;
                }
                DiagramShape obj1 = this.Pick(p, selectableOnly);
                if (obj1 != null)
                {
                    coll.Add(obj1);
                }
            }
            return coll;
        }

        public virtual void Remove(DiagramShape obj)
        {
            if (obj != null)
            {
                GroupShape group1 = obj.Parent;
                if (group1 != null)
                {
                    if (group1 != this)
                    {
                        throw new ArgumentException("Cannot remove an object from a group if it doesn't belong to that group.");
                    }
                    int num1 = this.myObjects.IndexOf(obj);
                    if (num1 < 0)
                    {
                        return;
                    }
                    this.removeAt(num1, obj, false);
                }
            }
        }

        public void Remove(object obj)
        {
            this.Remove((DiagramShape)obj);
        }

        private void removeAt(int index, DiagramShape obj, bool undoing)
        {
            try
            {
                obj.SetBeingRemoved(true);
                if (undoing)
                {
                    int num1 = this.myObjects.IndexOf(obj);
                    if (num1 >= 0)
                    {
                        if ((index < 0) || (index >= this.myObjects.Count))
                        {
                            index = num1;
                        }
                        this.myObjects.RemoveAt(index);
                    }
                }
                else
                {
                    this.myObjects.RemoveAt(index);
                }
                RectangleF ef1 = obj.Bounds;
                this.Changed(0x41c, index, obj, ef1, 0, null, DiagramShape.NullRect);
                if (!undoing)
                {
                    this.LayoutChildren(obj);
                    base.InvalidBounds = true;
                    RectangleF ef2 = this.Bounds;
                }
            }
            catch (Exception exception1)
            {
                DiagramShape.Trace("GoGroup.Remove: " + exception1.ToString());
                throw exception1;
            }
            finally
            {
                obj.SetParent(null, undoing);
                obj.SetBeingRemoved(false);
            }
        }

        public void RemoveAt(int index)
        {
            this.Remove((DiagramShape)this.myObjects[index]);
        }

        private void replaceAt(int index, DiagramShape newobj, bool undoing)
        {
            DiagramShape obj1 = (DiagramShape)this.myObjects[index];
            obj1.SetBeingRemoved(true);
            obj1.SetParent(null, undoing);
            obj1.SetBeingRemoved(false);
            this.myObjects[index] = newobj;
            RectangleF ef1 = newobj.Bounds;
            newobj.SetParent(this, undoing);
            this.Changed(0x41e, index, obj1, DiagramShape.NullRect, index, newobj, DiagramShape.NullRect);
            if (!undoing)
            {
                this.LayoutChildren(newobj);
                base.InvalidBounds = true;
                RectangleF ef2 = this.Bounds;
            }
        }

        protected virtual void RescaleChildren(RectangleF old)
        {
            if ((old.Width > 0f) && (old.Height > 0f))
            {
                RectangleF ef1 = this.Bounds;
                float single1 = ef1.Width / old.Width;
                float single2 = ef1.Height / old.Height;
                GroupEnumerator enumerator2 = this.GetEnumerator();
                GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramShape obj1 = enumerator1.Current;
                    if (obj1.AutoRescales)
                    {
                        RectangleF ef2 = obj1.Bounds;
                        float single3 = ef1.X + ((ef2.X - old.X) * single1);
                        float single4 = ef1.Y + ((ef2.Y - old.Y) * single2);
                        float single5 = ef2.Width * single1;
                        float single6 = ef2.Height * single2;
                        obj1.Bounds = new RectangleF(single3, single4, single5, single6);
                    }
                }
            }
        }

        internal static void setAllNoClear(DiagramShape obj, bool b)
        {
            DiagramPort port1 = obj as DiagramPort;
            if (port1 != null)
            {
                port1.NoClearLinks = b;
            }
            else
            {
                LineGraph link1 = obj as LineGraph;
                if (link1 != null)
                {
                    link1.NoClearPorts = b;
                }
                else
                {
                    GroupShape group1 = obj as GroupShape;
                    if (group1 != null)
                    {
                        GroupEnumerator enumerator1 = group1.GetEnumerator();
                        while (enumerator1.MoveNext())
                        {
                            GroupShape.setAllNoClear(enumerator1.Current, b);
                        }
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new GroupEnumerator(this.myObjects, true);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (DiagramShape)value;
            }
        }

        [Browsable(false)]
        public IEnumerable Backwards
        {
            get
            {
                return new GroupEnumerator(this.myObjects, false);
            }
        }

        [Description("The number of child objects in this group.")]
        public virtual int Count
        {
            get
            {
                return this.myObjects.Count;
            }
        }

        [Description("The first child object of this group.")]
        public DiagramShape First
        {
            get
            {
                if (this.myObjects.Count == 0)
                {
                    return null;
                }
                return (DiagramShape)this.myObjects[0];
            }
        }

        [Browsable(false)]
        public virtual bool IsEmpty
        {
            get
            {
                return (this.Count == 0);
            }
        }

        [Browsable(false)]
        public virtual bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        [Browsable(false)]
        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        [Browsable(false)]
        public virtual bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public virtual DiagramShape this[int index]
        {
            get
            {
                return (DiagramShape)this.myObjects[index];
            }
            set
            {
                object obj1 = this.myObjects[index];
                if ((obj1 != value) && (value != null))
                {
                    DiagramShape obj2 = value;
                    if (obj2.Parent != this)
                    {
                        this.replaceAt(index, obj2, false);
                    }
                }
            }
        }

        [Description("The last child object of this group.")]
        public DiagramShape Last
        {
            get
            {
                int num1 = this.myObjects.Count;
                if (num1 == 0)
                {
                    return null;
                }
                return (DiagramShape)this.myObjects[num1 - 1];
            }
        }

        [Browsable(false)]
        public virtual object SyncRoot
        {
            get
            {
                return base.Document;
            }
        }


        public const int ChangedZOrder = 0x41d;
        private const int flagInvalidPaintBounds = 0x100000;
        public const int InsertedObject = 0x41b;
        [NonSerialized]
        private float myBottom;
        [NonSerialized]
        private float myLeft;
        private ArrayList myObjects;
        [NonSerialized]
        private SizeF myPaintBoundsShadowOffset;
        [NonSerialized]
        private float myRight;
        [NonSerialized]
        private float myTop;
        public const int RemovedObject = 0x41c;
        public const int ReplacedObject = 0x41e;
    }
}
