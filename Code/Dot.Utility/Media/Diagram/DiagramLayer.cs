using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public sealed class DiagramLayer : Shapes.IDiagramShapeCollection, ICollection, IEnumerable, ILayerAbilities
    {
        static DiagramLayer()
        {
            DiagramLayer.NullRect = new RectangleF();
        }

        internal DiagramLayer()
        {
            this.myLayerCollectionContainer = null;
            this.myIsInDocument = false;
            this.myObjects = new ArrayList();
            this.myAllowView = true;
            this.myAllowPrint = true;
            this.myAllowSelect = true;
            this.myAllowMove = true;
            this.myAllowCopy = true;
            this.myAllowResize = true;
            this.myAllowReshape = true;
            this.myAllowDelete = true;
            this.myAllowInsert = true;
            this.myAllowLink = true;
            this.myAllowEdit = true;
            this.myIdentifier = null;
            this.myCaches = null;
        }

        public event SelectionEventHandler ShapeAdded;
        public void Add(Shapes.DiagramShape obj)
        {
            if (obj != null)
            {
                if (obj.Layer != null)
                {
                    if (obj.Layer.LayerCollectionContainer != this.LayerCollectionContainer)
                    {
                        throw new ArgumentException("Cannot add an object to a layer when it is already part of a different document's or view's layer.");
                    }
                    if (obj.Parent != null)
                    {
                        throw new ArgumentException("Cannot add an object to a layer when it is part of a group.");
                    }
                    DiagramLayer layer1 = obj.Layer;
                    if (layer1 != this)
                    {
                        this.changeLayer(obj, layer1, false);
                    }
                }
                else
                {
                    if (obj.Parent != null)
                    {
                        obj.Parent.Remove(obj);
                    }
                    this.addToLayer(obj, false);
                }
            }
            if (this.ShapeAdded != null)
            {
                this.ShapeAdded(this, new SelectionEventArgs(obj));
            }
        }

        public Shapes.IDiagramShapeCollection AddCollection(Shapes.IDiagramShapeCollection coll, bool reparentLinks)
        {
            Shapes.DiagramShapeCollection collection1 = new Shapes.DiagramShapeCollection();
            foreach (Shapes.DiagramShape obj1 in coll)
            {
                collection1.Add(obj1);
            }
            CollectionEnumerator enumerator2 = collection1.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                Shapes.DiagramShape obj2 = enumerator2.Current;
                bool flag1 = obj2.Layer != null;
                if (flag1)
                {
                    Shapes.GroupShape.setAllNoClear(obj2, true);
                    obj2.Remove();
                }
                this.Add(obj2);
                if (flag1)
                {
                    Shapes.GroupShape.setAllNoClear(obj2, false);
                }
            }
            if (reparentLinks && this.IsInDocument)
            {
                Shapes.SubGraphNode.ReparentAllLinksToSubGraphs(collection1, true, this.Document.LinksLayer);
            }
            return collection1;
        }

        internal void addToLayer(Shapes.DiagramShape obj, bool undoing)
        {
            this.myObjects.Add(obj);
            obj.SetLayer(this, obj, undoing);
            this.InsertIntoCache(obj);
            RectangleF ef1 = obj.Bounds;
            this.LayerCollectionContainer.RaiseChanged(0x386, 0, obj, 0, null, DiagramLayer.NullRect, 0, this, ef1);
        }

        private bool CacheWanted(DiagramView view)
        {
            if (this.IsInDocument && DiagramDocument.myCaching)
            {
                return !view.IsPrinting;
            }
            return false;
        }

        public bool CanCopyObjects()
        {
            if (!this.AllowCopy)
            {
                return false;
            }
            if (this.IsInDocument)
            {
                return this.LayerCollectionContainer.CanCopyObjects();
            }
            return true;
        }

        public bool CanDeleteObjects()
        {
            if (!this.AllowDelete)
            {
                return false;
            }
            if (this.IsInDocument)
            {
                return this.LayerCollectionContainer.CanDeleteObjects();
            }
            return true;
        }

        public bool CanEditObjects()
        {
            if (!this.AllowEdit)
            {
                return false;
            }
            if (this.IsInDocument)
            {
                return this.LayerCollectionContainer.CanEditObjects();
            }
            return true;
        }

        public bool CanInsertObjects()
        {
            if (!this.AllowInsert)
            {
                return false;
            }
            if (this.IsInDocument)
            {
                return this.LayerCollectionContainer.CanInsertObjects();
            }
            return true;
        }

        public bool CanLinkObjects()
        {
            if (!this.AllowLink)
            {
                return false;
            }
            if (this.IsInDocument)
            {
                return this.LayerCollectionContainer.CanLinkObjects();
            }
            return true;
        }

        public bool CanMoveObjects()
        {
            if (!this.AllowMove)
            {
                return false;
            }
            if (this.IsInDocument)
            {
                return this.LayerCollectionContainer.CanMoveObjects();
            }
            return true;
        }

        public bool CanPrintObjects()
        {
            return this.AllowPrint;
        }

        public bool CanReshapeObjects()
        {
            if (!this.AllowReshape)
            {
                return false;
            }
            if (this.IsInDocument)
            {
                return this.LayerCollectionContainer.CanReshapeObjects();
            }
            return true;
        }

        public bool CanResizeObjects()
        {
            if (!this.AllowResize)
            {
                return false;
            }
            if (this.IsInDocument)
            {
                return this.LayerCollectionContainer.CanResizeObjects();
            }
            return true;
        }

        public bool CanSelectObjects()
        {
            if (!this.AllowSelect)
            {
                return false;
            }
            if (this.IsInDocument)
            {
                return this.LayerCollectionContainer.CanSelectObjects();
            }
            return true;
        }

        public bool CanViewObjects()
        {
            return this.AllowView;
        }

        internal void changeLayer(Shapes.DiagramShape obj, DiagramLayer oldLayer, bool undoing)
        {
            oldLayer.RemoveFromCache(obj);
            int num1 = Shapes.DiagramShapeCollection.fastRemove(oldLayer.myObjects, obj);
            this.myObjects.Add(obj);
            obj.SetLayer(this, obj, undoing);
            this.InsertIntoCache(obj);
            RectangleF ef1 = obj.Bounds;
            this.LayerCollectionContainer.RaiseChanged(0x388, 0, obj, num1, oldLayer, ef1, -1, this, ef1);
        }

        public void Clear()
        {
            for (int num1 = this.myObjects.Count; num1 > 0; num1 = System.Math.Min(num1, this.myObjects.Count))
            {
                Shapes.DiagramShape obj1 = (Shapes.DiagramShape)this.myObjects[--num1];
                this.Remove(obj1);
            }
        }

        public bool Contains(Shapes.DiagramShape obj)
        {
            if (obj != null)
            {
                return (obj.Layer == this);
            }
            return false;
        }

        public Shapes.DiagramShape[] CopyArray()
        {
            Shapes.DiagramShape[] objArray1 = new Shapes.DiagramShape[this.Count];
            this.CopyTo(objArray1, 0);
            return objArray1;
        }

        public void CopyTo(Array array, int index)
        {
            this.myObjects.CopyTo(array, index);
        }

        public void CopyTo(Shapes.DiagramShape[] array, int index)
        {
            this.myObjects.CopyTo(array, index);
        }

        internal GoLayerCache FindCache(DiagramView view)
        {
            foreach (GoLayerCache cache1 in this.Caches)
            {
                if (cache1.View == view)
                {
                    return cache1;
                }
            }
            return null;
        }

        internal GoLayerCache FindCache(PointF p)
        {
            GoLayerCache cache1 = null;
            foreach (GoLayerCache cache2 in this.Caches)
            {
                if (!Shapes.DiagramShape.ContainsRect(cache2.Rect, p) || ((cache1 != null) && (cache2.Objects.Count >= cache1.Objects.Count)))
                {
                    continue;
                }
                cache1 = cache2;
            }
            return cache1;
        }

        internal GoLayerCache FindCache(RectangleF r)
        {
            GoLayerCache cache1 = null;
            foreach (GoLayerCache cache2 in this.Caches)
            {
                if (!Shapes.DiagramShape.ContainsRect(cache2.Rect, r) || ((cache1 != null) && (cache2.Objects.Count >= cache1.Objects.Count)))
                {
                    continue;
                }
                cache1 = cache2;
            }
            return cache1;
        }

        public LayerEnumerator GetEnumerator()
        {
            return new LayerEnumerator(this.myObjects, true);
        }

        internal void init(ILayerCollectionContainer lcc)
        {
            this.myLayerCollectionContainer = lcc;
            this.myIsInDocument = lcc is DiagramDocument;
            this.myAllowPrint = this.myIsInDocument;
        }

        internal void InsertIntoCache(Shapes.DiagramShape obj)
        {
            RectangleF ef1 = obj.Bounds;
            foreach (GoLayerCache cache1 in this.Caches)
            {
                RectangleF ef2 = obj.ExpandPaintBounds(ef1, cache1.View);
                if (Shapes.DiagramShape.IntersectsRect(cache1.Rect, ef2))
                {
                    cache1.Objects.Add(obj);
                }
            }
        }

        IEnumerable Shapes.IDiagramShapeCollection.Backwards
        {
            get
            {
                return new LayerEnumerator(this.myObjects, false);
            }
        }

        public void Paint(Graphics g, DiagramView view, RectangleF clipRect)
        {
            bool flag1 = view.IsPrinting;
            if (!(flag1 ? !this.CanPrintObjects() : !this.CanViewObjects()))
            {
                RectangleF ef1 = view.DocExtent;
                GoLayerCache cache1 = this.FindCache(view);
                if ((cache1 != null) && (cache1.Rect == ef1))
                {
                    foreach (Shapes.DiagramShape obj1 in cache1.Objects)
                    {
                        if (!(flag1 ? obj1.CanPrint() : obj1.CanView()))
                        {
                            continue;
                        }
                        RectangleF ef2 = obj1.Bounds;
                        ef2 = obj1.ExpandPaintBounds(ef2, view);
                        if (Shapes.DiagramShape.IntersectsRect(ef2, clipRect))
                        {
                            obj1.Paint(g, view);
                        }
                    }
                }
                else
                {
                    LayerEnumerator enumerator2;
                    if (this.CacheWanted(view))
                    {
                        if (cache1 == null)
                        {
                            cache1 = new GoLayerCache(view);
                            this.Caches.Add(cache1);
                        }
                        else
                        {
                            cache1.Reset();
                        }
                        cache1.Rect = ef1;
                        enumerator2 = this.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Shapes.DiagramShape obj2 = enumerator2.Current;
                            RectangleF ef3 = obj2.Bounds;
                            ef3 = obj2.ExpandPaintBounds(ef3, view);
                            if ((flag1 ? obj2.CanPrint() : obj2.CanView()) && Shapes.DiagramShape.IntersectsRect(ef3, clipRect))
                            {
                                obj2.Paint(g, view);
                            }
                            if (Shapes.DiagramShape.IntersectsRect(ef3, ef1))
                            {
                                cache1.Objects.Add(obj2);
                            }
                        }
                    }
                    else
                    {
                        enumerator2 = this.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Shapes.DiagramShape obj3 = enumerator2.Current;
                            if (flag1 ? obj3.CanPrint() : obj3.CanView())
                            {
                                RectangleF ef4 = obj3.Bounds;
                                ef4 = obj3.ExpandPaintBounds(ef4, view);
                                if (Shapes.DiagramShape.IntersectsRect(ef4, clipRect))
                                {
                                    obj3.Paint(g, view);
                                }
                            }
                        }
                    }
                }
            }
        }

        public Shapes.DiagramShape PickObject(PointF p, bool selectableOnly)
        {
            if (this.CanViewObjects())
            {
                if (selectableOnly && !this.CanSelectObjects())
                {
                    return null;
                }
                GoLayerCache cache1 = this.FindCache(p);
                if (cache1 != null)
                {
                    ArrayList list1 = cache1.Objects;
                    for (int num2 = list1.Count - 1; num2 >= 0; num2--)
                    {
                        Shapes.DiagramShape obj1 = (Shapes.DiagramShape)list1[num2];
                        Shapes.DiagramShape obj2 = obj1.Pick(p, selectableOnly);
                        if (obj2 != null)
                        {
                            return obj2;
                        }
                    }
                }
                else
                {
                    LayerEnumerator enumerator1 = this.Backwards.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        Shapes.DiagramShape obj4 = enumerator1.Current.Pick(p, selectableOnly);
                        if (obj4 != null)
                        {
                            return obj4;
                        }
                    }
                }
            }
            return null;
        }

        public Shapes.IDiagramShapeCollection PickObjects(PointF p, bool selectableOnly, Shapes.IDiagramShapeCollection coll, int max)
        {
            if (coll == null)
            {
                coll = new Shapes.DiagramShapeCollection();
            }
            if (coll.Count < max)
            {
                if (!this.CanViewObjects())
                {
                    return coll;
                }
                if (selectableOnly && !this.CanSelectObjects())
                {
                    return coll;
                }
                GoLayerCache cache1 = this.FindCache(p);
                if (cache1 != null)
                {
                    ArrayList list1 = cache1.Objects;
                    for (int num2 = list1.Count - 1; num2 >= 0; num2--)
                    {
                        Shapes.DiagramShape obj1 = (Shapes.DiagramShape)list1[num2];
                        Shapes.GroupShape group1 = obj1 as Shapes.GroupShape;
                        if (group1 != null)
                        {
                            group1.PickObjects(p, selectableOnly, coll, max);
                        }
                        else
                        {
                            Shapes.DiagramShape obj2 = obj1.Pick(p, selectableOnly);
                            if (obj2 != null)
                            {
                                coll.Add(obj2);
                                if (coll.Count >= max)
                                {
                                    return coll;
                                }
                            }
                        }
                    }
                    return coll;
                }
                LayerEnumerator enumerator1 = this.Backwards.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj3 = enumerator1.Current;
                    Shapes.GroupShape group2 = obj3 as Shapes.GroupShape;
                    if (group2 != null)
                    {
                        group2.PickObjects(p, selectableOnly, coll, max);
                        continue;
                    }
                    Shapes.DiagramShape obj4 = obj3.Pick(p, selectableOnly);
                    if (obj4 != null)
                    {
                        coll.Add(obj4);
                        if (coll.Count >= max)
                        {
                            return coll;
                        }
                    }
                }
            }
            return coll;
        }

        public event SelectionEventHandler ShapeRemoved;
        public void Remove(Shapes.DiagramShape obj)
        {
            if (obj != null)
            {
                DiagramLayer layer1 = obj.Layer;
                if (layer1 != null)
                {
                    if (layer1 != this)
                    {
                        throw new ArgumentException("Cannot remove an object from a layer if it does not belong to that layer.");
                    }
                    Shapes.GroupShape group1 = obj.Parent;
                    if (group1 != null)
                    {
                        group1.Remove(obj);
                    }
                    else
                    {
                        this.removeFromLayer(obj, false);
                    }
                }
            }
            if (this.ShapeRemoved != null)
            {
                this.ShapeRemoved(this, new SelectionEventArgs(obj));
            }
        }

        internal void RemoveFromCache(Shapes.DiagramShape obj)
        {
            RectangleF ef1 = obj.Bounds;
            foreach (GoLayerCache cache1 in this.Caches)
            {
                RectangleF ef2 = obj.ExpandPaintBounds(ef1, cache1.View);
                if (Shapes.DiagramShape.IntersectsRect(cache1.Rect, ef2))
                {
                    Shapes.DiagramShapeCollection.fastRemove(cache1.Objects, obj);
                    Shapes.DiagramShapeCollection.fastRemove(cache1.Strokes, obj);
                }
            }
        }

        internal void removeFromLayer(Shapes.DiagramShape obj, bool undoing)
        {
            try
            {
                obj.SetBeingRemoved(true);
                this.RemoveFromCache(obj);
                int num1 = Shapes.DiagramShapeCollection.fastRemove(this.myObjects, obj);
                RectangleF ef1 = obj.Bounds;
                this.LayerCollectionContainer.RaiseChanged(0x387, 0, obj, num1, this, ef1, 0, null, DiagramLayer.NullRect);
            }
            finally
            {
                obj.SetLayer(null, obj, undoing);
                obj.SetBeingRemoved(false);
            }
        }

        internal void ResetCache()
        {
            this.myCaches = new ArrayList();
        }

        public void SetModifiable(bool b)
        {
            this.AllowMove = b;
            this.AllowResize = b;
            this.AllowReshape = b;
            this.AllowDelete = b;
            this.AllowInsert = b;
            this.AllowLink = b;
            this.AllowEdit = b;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LayerEnumerator(this.myObjects, true);
        }

        internal void UpdateCache(Shapes.DiagramShape obj, ChangedEventArgs evt)
        {
            foreach (GoLayerCache cache1 in this.Caches)
            {
                RectangleF ef1 = evt.OldRect;
                ef1 = obj.ExpandPaintBounds(ef1, cache1.View);
                RectangleF ef2 = evt.NewRect;
                ef2 = obj.ExpandPaintBounds(ef2, cache1.View);
                bool flag1 = Shapes.DiagramShape.IntersectsRect(cache1.Rect, ef1);
                bool flag2 = Shapes.DiagramShape.IntersectsRect(cache1.Rect, ef2);
                if ((!flag1 && flag2) && !cache1.Objects.Contains(obj))
                {
                    cache1.Objects.Add(obj);
                }
            }
        }


        [Category("Behavior"), DefaultValue(true), Description("Whether the user can copy the selected objects in this layer.")]
        public bool AllowCopy
        {
            get
            {
                return this.myAllowCopy;
            }
            set
            {
                bool flag1 = this.myAllowCopy;
                if (flag1 != value)
                {
                    this.myAllowCopy = value;
                    this.LayerCollectionContainer.RaiseChanged(0x391, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the user can delete the selected objects in this layer.")]
        public bool AllowDelete
        {
            get
            {
                return this.myAllowDelete;
            }
            set
            {
                bool flag1 = this.myAllowDelete;
                if (flag1 != value)
                {
                    this.myAllowDelete = value;
                    this.LayerCollectionContainer.RaiseChanged(0x394, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the user can edit objects in this layer.")]
        public bool AllowEdit
        {
            get
            {
                return this.myAllowEdit;
            }
            set
            {
                bool flag1 = this.myAllowEdit;
                if (flag1 != value)
                {
                    this.myAllowEdit = value;
                    this.LayerCollectionContainer.RaiseChanged(0x397, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [DefaultValue(true), Category("Behavior"), Description("Whether the user can insert objects in this layer.")]
        public bool AllowInsert
        {
            get
            {
                return this.myAllowInsert;
            }
            set
            {
                bool flag1 = this.myAllowInsert;
                if (flag1 != value)
                {
                    this.myAllowInsert = value;
                    this.LayerCollectionContainer.RaiseChanged(0x395, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the user can link ports in this layer.")]
        public bool AllowLink
        {
            get
            {
                return this.myAllowLink;
            }
            set
            {
                bool flag1 = this.myAllowLink;
                if (flag1 != value)
                {
                    this.myAllowLink = value;
                    this.LayerCollectionContainer.RaiseChanged(0x396, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [DefaultValue(true), Description("Whether the user can move the selected objects in this layer."), Category("Behavior")]
        public bool AllowMove
        {
            get
            {
                return this.myAllowMove;
            }
            set
            {
                bool flag1 = this.myAllowMove;
                if (flag1 != value)
                {
                    this.myAllowMove = value;
                    this.LayerCollectionContainer.RaiseChanged(0x390, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [Description("Whether the view can print the objects in this layer."), Category("Behavior"), DefaultValue(true)]
        public bool AllowPrint
        {
            get
            {
                return this.myAllowPrint;
            }
            set
            {
                bool flag1 = this.myAllowPrint;
                if (flag1 != value)
                {
                    this.myAllowPrint = value;
                    this.LayerCollectionContainer.RaiseChanged(920, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [DefaultValue(true), Category("Behavior"), Description("Whether the user can reshape the resizable objects in this layer.")]
        public bool AllowReshape
        {
            get
            {
                return this.myAllowReshape;
            }
            set
            {
                bool flag1 = this.myAllowReshape;
                if (flag1 != value)
                {
                    this.myAllowReshape = value;
                    this.LayerCollectionContainer.RaiseChanged(0x393, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [Description("Whether the user can resize the selected objects in this layer."), Category("Behavior"), DefaultValue(true)]
        public bool AllowResize
        {
            get
            {
                return this.myAllowResize;
            }
            set
            {
                bool flag1 = this.myAllowResize;
                if (flag1 != value)
                {
                    this.myAllowResize = value;
                    this.LayerCollectionContainer.RaiseChanged(0x392, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("Whether the user can select objects in this layer."), DefaultValue(true)]
        public bool AllowSelect
        {
            get
            {
                return this.myAllowSelect;
            }
            set
            {
                bool flag1 = this.myAllowSelect;
                if (flag1 != value)
                {
                    this.myAllowSelect = value;
                    this.LayerCollectionContainer.RaiseChanged(0x38f, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("Whether the user can see the objects in this layer."), DefaultValue(true)]
        public bool AllowView
        {
            get
            {
                return this.myAllowView;
            }
            set
            {
                bool flag1 = this.myAllowView;
                if (flag1 != value)
                {
                    this.myAllowView = value;
                    this.LayerCollectionContainer.RaiseChanged(910, 0, this, 0, flag1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [Browsable(false)]
        public LayerEnumerator Backwards
        {
            get
            {
                return new LayerEnumerator(this.myObjects, false);
            }
        }

        internal ArrayList Caches
        {
            get
            {
                if (this.myCaches == null)
                {
                    this.myCaches = new ArrayList();
                }
                return this.myCaches;
            }
        }

        [Description("The number of objects in this layer.")]
        public int Count
        {
            get
            {
                return this.myObjects.Count;
            }
        }

        [Category("Ownership"), Description("The document in which this layer belongs.")]
        public DiagramDocument Document
        {
            get
            {
                if (this.myIsInDocument)
                {
                    return (DiagramDocument)this.myLayerCollectionContainer;
                }
                return null;
            }
        }

        [DefaultValue((string)null), Description("An identifier for this layer.")]
        public object Identifier
        {
            get
            {
                return this.myIdentifier;
            }
            set
            {
                object obj1 = this.myIdentifier;
                if (obj1 != value)
                {
                    this.myIdentifier = value;
                    this.LayerCollectionContainer.RaiseChanged(930, 0, this, 0, obj1, DiagramLayer.NullRect, 0, value, DiagramLayer.NullRect);
                }
            }
        }

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return (this.myObjects.Count == 0);
            }
        }

        [Browsable(false)]
        public bool IsInDocument
        {
            get
            {
                return this.myIsInDocument;
            }
        }

        [Browsable(false)]
        public bool IsInView
        {
            get
            {
                return !this.myIsInDocument;
            }
        }

        [Browsable(false)]
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        [Browsable(false)]
        public ILayerCollectionContainer LayerCollectionContainer
        {
            get
            {
                return this.myLayerCollectionContainer;
            }
        }

        [Browsable(false)]
        public object SyncRoot
        {
            get
            {
                return this.LayerCollectionContainer;
            }
        }

        [Description("The view in which this layer belongs."), Category("Ownership")]
        public DiagramView View
        {
            get
            {
                if (this.myIsInDocument)
                {
                    return null;
                }
                return (DiagramView)this.myLayerCollectionContainer;
            }
        }


        public const int ChangedAllowCopy = 0x391;
        public const int ChangedAllowDelete = 0x394;
        public const int ChangedAllowEdit = 0x397;
        public const int ChangedAllowInsert = 0x395;
        public const int ChangedAllowLink = 0x396;
        public const int ChangedAllowMove = 0x390;
        public const int ChangedAllowPrint = 920;
        public const int ChangedAllowReshape = 0x393;
        public const int ChangedAllowResize = 0x392;
        public const int ChangedAllowSelect = 0x38f;
        public const int ChangedAllowView = 910;
        public const int ChangedIdentifier = 930;
        public const int ChangedObject = 0x385;
        public const int ChangedObjectLayer = 0x388;
        public const int InsertedObject = 0x386;
        private bool myAllowCopy;
        private bool myAllowDelete;
        private bool myAllowEdit;
        private bool myAllowInsert;
        private bool myAllowLink;
        private bool myAllowMove;
        private bool myAllowPrint;
        private bool myAllowReshape;
        private bool myAllowResize;
        private bool myAllowSelect;
        private bool myAllowView;
        [NonSerialized]
        private ArrayList myCaches;
        private object myIdentifier;
        private bool myIsInDocument;
        private ILayerCollectionContainer myLayerCollectionContainer;
        private ArrayList myObjects;
        private static readonly RectangleF NullRect;
        public const int RemovedObject = 0x387;

        internal sealed class GoLayerCache
        {
            internal GoLayerCache(DiagramView view)
            {
                this.myView = null;
                this.myObjects = null;
                this.myRect = new RectangleF(0f, 0f, 0f, 0f);
                this.myStrokes = null;
                this.myView = view;
                this.myObjects = new ArrayList();
                this.myStrokes = new ArrayList();
            }

            internal void Reset()
            {
                this.myObjects.Clear();
                this.myStrokes.Clear();
                this.myRect = new RectangleF(0f, 0f, 0f, 0f);
            }


            internal ArrayList Objects
            {
                get
                {
                    return this.myObjects;
                }
            }

            internal RectangleF Rect
            {
                get
                {
                    return this.myRect;
                }
                set
                {
                    this.myRect = value;
                }
            }

            internal ArrayList Strokes
            {
                get
                {
                    return this.myStrokes;
                }
            }

            internal DiagramView View
            {
                get
                {
                    return this.myView;
                }
            }


            private ArrayList myObjects;
            private RectangleF myRect;
            private ArrayList myStrokes;
            private DiagramView myView;
        }
    }
}
