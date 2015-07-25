using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class DiagramDocument : Shapes.IDiagramShapeCollection, ICollection, IEnumerable, ILayerCollectionContainer, ILayerAbilities
    {
        [field: NonSerialized]
        public event ChangedEventHandler Changed;
        public event SelectionEventHandler ShapeAdded;
        public event SelectionEventHandler ShapeRemoved;

        static DiagramDocument()
        {
            DiagramDocument.NullRect = new RectangleF();
            DiagramDocument.myCycleMap = new Hashtable();
            DiagramDocument.myCaching = true;
        }

        public DiagramDocument()
        {
            this.myUserFlags = 0;
            this.myUserObject = null;
            this.myLayers = new LayerCollection();
            this.myLinksLayer = null;
            this.myName = "";
            this.myWorldScale = new SizeF(1f, 1f);
            this.myFixedSize = false;
            this.myPaperColor = Color.Empty;
            this.myDataFormat = null;
            this.myAllowSelect = true;
            this.myAllowMove = true;
            this.myAllowCopy = true;
            this.myAllowResize = true;
            this.myAllowReshape = true;
            this.myAllowDelete = true;
            this.myAllowInsert = true;
            this.myAllowLink = true;
            this.myAllowEdit = true;
            this.mySuspendsUpdates = false;
            this.mySkipsUndoManager = false;
            this.mySerializesUndoManager = false;
            this.Changed = null;
            this.myChangedEventArgs = null;
            this.myIsModified = false;
            this.myUndoManager = null;
            this.mySerializedUndoManager = null;
            this.myUndoEditIndex = -2;
            this.myValidCycle = DocumentValidCycle.All;
            this.myPositions = null;
            this.mySkippedAvoidable = null;
            this.myPartIDIndexable = false;
            this.myLastPartID = -1;
            this.myParts = null;
            this.myLayers.init(this);
            this.myLinksLayer = this.myLayers.Default;
            this.myIsModified = false;

            this.DefaultLayer.ShapeAdded += new SelectionEventHandler(DefaultLayer_ShapeAdded);
            this.DefaultLayer.ShapeRemoved += new SelectionEventHandler(DefaultLayer_ShapeRemoved);
        }

        void DefaultLayer_ShapeRemoved(object sender, SelectionEventArgs e)
        {
            if (this.ShapeRemoved != null)
            {
                this.ShapeRemoved(sender, e);
            }
        }

        void DefaultLayer_ShapeAdded(object sender, SelectionEventArgs e)
        {
            if (this.ShapeAdded != null)
            {
                this.ShapeAdded(sender, e);
            }
        }

        public virtual bool AbortTransaction()
        {
            DiagramUndoManager manager1 = this.UndoManager;
            if (manager1 != null)
            {
                return manager1.AbortTransaction();
            }
            return false;
        }

        public virtual void Add(Shapes.DiagramShape obj)
        {
            if (obj is Shapes.IDiagramLine)
            {
                this.LinksLayer.Add(obj);
            }
            else
            {
                this.DefaultLayer.Add(obj);
            }
        }

        internal void AddAllParts(Shapes.DiagramShape obj)
        {
            IIdentifiablePart part1 = obj as IIdentifiablePart;
            if (part1 != null)
            {
                this.AddPart(part1);
            }
            Shapes.GroupShape group1 = obj as Shapes.GroupShape;
            if (group1 != null)
            {
                Shapes.GroupEnumerator enumerator2 = group1.GetEnumerator();
                Shapes.GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj1 = enumerator1.Current;
                    this.AddAllParts(obj1);
                }
            }
        }

        private void AddAvoidableRectanglePorts(Shapes.DiagramShape obj, ref RectangleF rect)
        {
            Shapes.GroupShape group1 = obj as Shapes.GroupShape;
            if (group1 != null)
            {
                Shapes.GroupEnumerator enumerator1 = group1.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj1 = enumerator1.Current;
                    this.AddAvoidableRectanglePorts(obj1, ref rect);
                }
            }
            Shapes.DiagramPort port1 = obj as Shapes.DiagramPort;
            if (port1 != null)
            {
                Shapes.LineGraph link1 = null;
                Shapes.PortLinkEnumerator enumerator2 = port1.Links.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    link1 = enumerator2.Current as Shapes.LineGraph;
                    if ((link1 != null) && link1.AvoidsNodes)
                    {
                        break;
                    }
                }
                if (link1 != null)
                {
                    float single1 = port1.EndSegmentLength;
                    if (link1.FromPort == port1)
                    {
                        float single2 = port1.GetFromLinkDir(link1);
                        PointF tf1 = port1.GetFromLinkPoint(link1);
                        if (single2 == 0f)
                        {
                            tf1.X += single1;
                        }
                        else if (single2 == 90f)
                        {
                            tf1.Y += single1;
                        }
                        else if (single2 == 180f)
                        {
                            tf1.X -= single1;
                        }
                        else if (single2 == 270f)
                        {
                            tf1.Y -= single1;
                        }
                        rect = Shapes.DiagramShape.UnionRect(rect, tf1);
                    }
                    else
                    {
                        float single3 = port1.GetToLinkDir(link1);
                        PointF tf2 = port1.GetToLinkPoint(link1);
                        if (single3 == 0f)
                        {
                            tf2.X += single1;
                        }
                        else if (single3 == 90f)
                        {
                            tf2.Y += single1;
                        }
                        else if (single3 == 180f)
                        {
                            tf2.X -= single1;
                        }
                        else if (single3 == 270f)
                        {
                            tf2.Y -= single1;
                        }
                        rect = Shapes.DiagramShape.UnionRect(rect, tf2);
                    }
                }
            }
        }

        public Shapes.DiagramShape AddCopy(Shapes.DiagramShape obj, PointF loc)
        {
            PointF tf1 = obj.Location;
            Shapes.DiagramShapeCollection collection1 = new Shapes.DiagramShapeCollection();
            collection1.Add(obj);
            SizeF ef1 = DiagramTool.SubtractPoints(loc, tf1);
            CopyDictionary dictionary1 = this.CopyFromCollection(collection1, false, false, ef1, null);
            return (dictionary1[obj] as Shapes.DiagramShape);
        }

        internal void AddPart(IIdentifiablePart p)
        {
            if (this.myParts == null)
            {
                this.myParts = new Hashtable(1000);
            }
            int num1 = p.PartID;
            if (num1 == -1)
            {
                int num3;
                this.myLastPartID = num3 = this.myLastPartID + 1;
                int num2 = num3;
                while (this.myParts[num2] != null)
                {
                    this.myLastPartID = num3 = this.myLastPartID + 1;
                    num2 = num3;
                }
                this.myParts[num2] = p;
                p.PartID = num2;
            }
            else
            {
                IIdentifiablePart part1 = (IIdentifiablePart)this.myParts[num1];
                if (part1 == null)
                {
                    this.myParts[num1] = p;
                }
                else if (part1.PartID != num1)
                {
                    this.myParts[num1] = p;
                    part1.PartID = -1;
                    this.AddPart(part1);
                }
            }
        }

        private bool alreadyCopied(Hashtable copieds, Shapes.DiagramShape o)
        {
            for (Shapes.DiagramShape obj1 = o; obj1 != null; obj1 = obj1.Parent)
            {
                if (copieds.Contains(obj1))
                {
                    return true;
                }
            }
            return false;
        }

        public void BeginUpdateViews()
        {
            this.RaiseChanged(0x65, 0, null, 0, null, DiagramDocument.NullRect, 0, null, DiagramDocument.NullRect);
        }

        public virtual bool CanCopyObjects()
        {
            return this.AllowCopy;
        }

        public virtual bool CanDeleteObjects()
        {
            return this.AllowDelete;
        }

        public virtual bool CanEditObjects()
        {
            return this.AllowEdit;
        }

        public virtual bool CanInsertObjects()
        {
            return this.AllowInsert;
        }

        public virtual bool CanLinkObjects()
        {
            return this.AllowLink;
        }

        public virtual bool CanMoveObjects()
        {
            return this.AllowMove;
        }

        public virtual bool CanRedo()
        {
            DiagramUndoManager manager1 = this.UndoManager;
            if (manager1 != null)
            {
                return manager1.CanRedo();
            }
            return false;
        }

        public virtual bool CanReshapeObjects()
        {
            return this.AllowReshape;
        }

        public virtual bool CanResizeObjects()
        {
            return this.AllowResize;
        }

        public virtual bool CanSelectObjects()
        {
            return this.AllowSelect;
        }

        public virtual bool CanUndo()
        {
            DiagramUndoManager manager1 = this.UndoManager;
            if (manager1 != null)
            {
                return manager1.CanUndo();
            }
            return false;
        }

        public virtual void ChangeValue(ChangedEventArgs e, bool undo)
        {
            int num2 = e.Hint;
            if (num2 <= 0x324)
            {
                switch (num2)
                {
                    case 0xc9:
                        {
                            this.Name = (string)e.GetValue(undo);
                            return;
                        }
                    case 0xca:
                        {
                            this.Size = e.GetSize(undo);
                            return;
                        }
                    case 0xcb:
                        {
                            this.TopLeft = e.GetPoint(undo);
                            return;
                        }
                    case 0xcc:
                        {
                            this.FixedSize = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0xcd:
                        {
                            this.PaperColor = (Color)e.GetValue(undo);
                            return;
                        }
                    case 0xce:
                        {
                            this.DataFormat = (string)e.GetValue(undo);
                            return;
                        }
                    case 0xcf:
                        {
                            this.AllowSelect = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0xd0:
                        {
                            this.AllowMove = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0xd1:
                        {
                            this.AllowCopy = (bool)e.GetValue(undo);
                            return;
                        }
                    case 210:
                        {
                            this.AllowResize = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0xd3:
                        {
                            this.AllowReshape = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0xd4:
                        {
                            this.AllowDelete = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0xd5:
                        {
                            this.AllowInsert = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0xd6:
                        {
                            this.AllowLink = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0xd7:
                        {
                            this.AllowEdit = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0xd8:
                    case 0xd9:
                    case 0xda:
                    case 0xdb:
                        {
                            goto Label_0677;
                        }
                    case 220:
                        {
                            ArrayList list1 = (ArrayList)e.GetValue(undo);
                            for (int num1 = 0; num1 < list1.Count; num1 += 2)
                            {
                                Shapes.DiagramShape obj5 = (Shapes.DiagramShape)list1[num1];
                                if (obj5 is Shapes.IDiagramLine)
                                {
                                    Shapes.LineGraph link1 = obj5 as Shapes.LineGraph;
                                    if (link1 != null)
                                    {
                                        PointF[] tfArray1 = (PointF[])list1[num1 + 1];
                                        link1.SetPoints(tfArray1);
                                    }
                                    else
                                    {
                                        Shapes.TextLine link2 = obj5 as Shapes.TextLine;
                                        if (link2 != null)
                                        {
                                            PointF[] tfArray2 = (PointF[])list1[num1 + 1];
                                            link2.RealLink.SetPoints(tfArray2);
                                        }
                                    }
                                }
                                else
                                {
                                    RectangleF ef1 = (RectangleF)list1[num1 + 1];
                                    obj5.Bounds = ef1;
                                }
                            }
                            this.InvalidateViews();
                            return;
                        }
                    case 0xdd:
                        {
                            this.UserFlags = e.GetInt(undo);
                            return;
                        }
                    case 0xde:
                        {
                            this.UserObject = e.GetValue(undo);
                            return;
                        }
                    case 0xdf:
                        {
                            this.LinksLayer = (DiagramLayer)e.GetValue(undo);
                            return;
                        }
                    case 0xe0:
                        {
                            this.PartIDIndexable = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0xe1:
                        {
                            this.ValidCycle = (DocumentValidCycle)e.GetInt(undo);
                            return;
                        }
                    case 0xe2:
                        {
                            this.setWorldScale(e.GetSize(undo), true);
                            return;
                        }
                    case 0x321:
                        {
                            DiagramLayer layer16 = (DiagramLayer)e.Object;
                            if (!undo)
                            {
                                DiagramLayer layer17 = (DiagramLayer)e.OldValue;
                                if (e.OldInt == 1)
                                {
                                    this.Layers.InsertAfter(layer17, layer16);
                                    return;
                                }
                                this.Layers.InsertBefore(layer17, layer16);
                                return;
                            }
                            this.Layers.Remove(layer16);
                            return;
                        }
                    case 0x322:
                        {
                            DiagramLayer layer18 = (DiagramLayer)e.Object;
                            if (!undo)
                            {
                                this.Layers.Remove(layer18);
                                return;
                            }
                            DiagramLayer layer19 = (DiagramLayer)e.OldValue;
                            if (layer19 != null)
                            {
                                this.Layers.InsertBefore(layer19, layer18);
                                return;
                            }
                            this.Layers.InsertAfter(this.Layers.Top, layer18);
                            return;
                        }
                    case 0x323:
                        {
                            DiagramLayer layer20 = (DiagramLayer)e.Object;
                            DiagramLayer layer21 = (DiagramLayer)e.OldValue;
                            if (e.OldInt != 1)
                            {
                                this.Layers.MoveBefore(layer21, layer20);
                                return;
                            }
                            this.Layers.MoveAfter(layer21, layer20);
                            return;
                        }
                    case 0x324:
                        {
                            DiagramLayer layer22 = (DiagramLayer)e.GetValue(undo);
                            this.Layers.Default = layer22;
                            return;
                        }
                }
            }
            else
            {
                switch (num2)
                {
                    case 0x385:
                        {
                            e.DiagramShape.ChangeValue(e, undo);
                            return;
                        }
                    case 0x386:
                        {
                            DiagramLayer layer1 = (DiagramLayer)e.NewValue;
                            Shapes.DiagramShape obj2 = e.DiagramShape;
                            if (!undo)
                            {
                                layer1.addToLayer(obj2, true);
                                return;
                            }
                            layer1.removeFromLayer(obj2, true);
                            return;
                        }
                    case 0x387:
                        {
                            DiagramLayer layer2 = (DiagramLayer)e.OldValue;
                            Shapes.DiagramShape obj3 = e.DiagramShape;
                            if (!undo)
                            {
                                layer2.removeFromLayer(obj3, true);
                                return;
                            }
                            layer2.addToLayer(obj3, true);
                            return;
                        }
                    case 0x388:
                        {
                            Shapes.DiagramShape obj4 = e.DiagramShape;
                            DiagramLayer layer3 = (DiagramLayer)e.OldValue;
                            DiagramLayer layer4 = (DiagramLayer)e.NewValue;
                            if (!undo)
                            {
                                layer4.changeLayer(obj4, layer3, true);
                                return;
                            }
                            layer3.changeLayer(obj4, layer4, true);
                            return;
                        }
                    case 0x389:
                    case 0x38a:
                    case 0x38b:
                    case 0x38c:
                    case 0x38d:
                        {
                            goto Label_0677;
                        }
                    case 910:
                        {
                            DiagramLayer layer5 = (DiagramLayer)e.Object;
                            layer5.AllowView = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0x38f:
                        {
                            DiagramLayer layer6 = (DiagramLayer)e.Object;
                            layer6.AllowSelect = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0x390:
                        {
                            DiagramLayer layer7 = (DiagramLayer)e.Object;
                            layer7.AllowMove = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0x391:
                        {
                            DiagramLayer layer8 = (DiagramLayer)e.Object;
                            layer8.AllowCopy = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0x392:
                        {
                            DiagramLayer layer9 = (DiagramLayer)e.Object;
                            layer9.AllowResize = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0x393:
                        {
                            DiagramLayer layer10 = (DiagramLayer)e.Object;
                            layer10.AllowReshape = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0x394:
                        {
                            DiagramLayer layer11 = (DiagramLayer)e.Object;
                            layer11.AllowDelete = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0x395:
                        {
                            DiagramLayer layer12 = (DiagramLayer)e.Object;
                            layer12.AllowInsert = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0x396:
                        {
                            DiagramLayer layer13 = (DiagramLayer)e.Object;
                            layer13.AllowLink = (bool)e.GetValue(undo);
                            return;
                        }
                    case 0x397:
                        {
                            DiagramLayer layer14 = (DiagramLayer)e.Object;
                            layer14.AllowEdit = (bool)e.GetValue(undo);
                            return;
                        }
                    case 930:
                        {
                            DiagramLayer layer15 = (DiagramLayer)e.Object;
                            layer15.Identifier = e.GetValue(undo);
                            return;
                        }
                }
            }
        Label_0677:
            if (e.Hint >= 10000)
            {
                throw new ArgumentException("Unknown GoChangedEventArgs hint--override DiagramDocument.ChangeValue to handle the Hint: " + e.Hint.ToString(NumberFormatInfo.InvariantInfo));
            }
        }

        public virtual void Clear()
        {
            this.myParts = null;
            this.InvalidatePositionArray(null);
            LayerCollectionEnumerator enumerator1 = this.Layers.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                enumerator1.Current.Clear();
            }
        }

        public RectangleF ComputeBounds()
        {
            return DiagramDocument.ComputeBounds(this, null);
        }

        public static RectangleF ComputeBounds(Shapes.IDiagramShapeCollection coll, DiagramView view)
        {
            bool flag1 = false;
            float single1 = 0f;
            float single2 = 0f;
            float single3 = 0f;
            float single4 = 0f;
            bool flag2 = (view != null) ? view.IsPrinting : false;
            foreach (Shapes.DiagramShape obj1 in coll)
            {
                if (flag2 ? !obj1.CanPrint() : !obj1.CanView())
                {
                    continue;
                }
                RectangleF ef1 = obj1.Bounds;
                RectangleF ef2 = obj1.ExpandPaintBounds(ef1, view);
                if (!flag1)
                {
                    flag1 = true;
                    single1 = ef2.X;
                    single2 = ef2.Y;
                    single3 = ef2.X + ef2.Width;
                    single4 = ef2.Y + ef2.Height;
                    continue;
                }
                if (ef2.X < single1)
                {
                    single1 = ef2.X;
                }
                if (ef2.Y < single2)
                {
                    single2 = ef2.Y;
                }
                if ((ef2.X + ef2.Width) > single3)
                {
                    single3 = ef2.X + ef2.Width;
                }
                if ((ef2.Y + ef2.Height) > single4)
                {
                    single4 = ef2.Y + ef2.Height;
                }
            }
            if (flag1)
            {
                return new RectangleF(single1, single2, single3 - single1, single4 - single2);
            }
            return new RectangleF();
        }

        public virtual bool Contains(Shapes.DiagramShape obj)
        {
            if (obj != null)
            {
                DiagramLayer layer1 = obj.Layer;
                if (layer1 != null)
                {
                    return (layer1.Document == this);
                }
            }
            return false;
        }

        public Shapes.DiagramShape[] CopyArray()
        {
            Shapes.DiagramShape[] objArray1 = new Shapes.DiagramShape[this.Count];
            this.CopyTo(objArray1, 0);
            return objArray1;
        }

        public CopyDictionary CopyFromCollection(Shapes.IDiagramShapeCollection coll)
        {
            return this.CopyFromCollection(coll, false, false, new SizeF(), null);
        }

        public virtual CopyDictionary CopyFromCollection(Shapes.IDiagramShapeCollection coll, bool copyableOnly, bool dragging, SizeF offset, CopyDictionary env)
        {
            CollectionEnumerator enumerator2;
            if (env == null)
            {
                env = this.CreateCopyDictionary();
            }
            env.SourceCollection = coll;
            Hashtable hashtable1 = new Hashtable();
            Shapes.DiagramShapeCollection collection1 = null;
            Shapes.DiagramShapeCollection collection2 = new Shapes.DiagramShapeCollection();
            Shapes.DiagramShapeCollection collection3 = null;
            foreach (Shapes.DiagramShape obj1 in coll)
            {
                Shapes.DiagramShape obj2 = dragging ? obj1.DraggingObject : obj1;
                if (((obj2 != null) && (!copyableOnly || obj2.CanCopy())) && !this.alreadyCopied(hashtable1, obj2))
                {
                    if ((collection1 != null) && (obj2 is Shapes.GroupShape))
                    {
                        enumerator2 = collection1.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Shapes.DiagramShape obj3 = enumerator2.Current;
                            if (obj3.IsChildOf(obj2))
                            {
                                hashtable1.Remove(obj3);
                                if (collection3 == null)
                                {
                                    collection3 = new Shapes.DiagramShapeCollection();
                                }
                                collection3.Add(obj3);
                                collection2.Remove(obj3);
                            }
                        }
                        if ((collection3 != null) && !collection3.IsEmpty)
                        {
                            enumerator2 = collection3.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                Shapes.DiagramShape obj4 = enumerator2.Current;
                                collection1.Remove(obj4);
                            }
                            collection3.Clear();
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
                    collection2.Add(obj2);
                }
            }
            PointF tf1 = new PointF();
            enumerator2 = collection2.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                Shapes.DiagramShape obj5 = enumerator2.Current;
                Shapes.DiagramShape obj6 = env[obj5] as Shapes.DiagramShape;
                if (obj6 == null)
                {
                    obj6 = env.Copy(obj5);
                    if (obj6 != null)
                    {
                        tf1 = obj6.Location;
                        obj6.Location = obj6.ComputeMove(tf1, new PointF(tf1.X + offset.Width, tf1.Y + offset.Height));
                        DiagramLayer layer1 = obj5.Layer;
                        DiagramLayer layer2 = null;
                        if (layer1 != null)
                        {
                            if (layer1.Document == this)
                            {
                                layer2 = layer1;
                            }
                            else
                            {
                                layer2 = this.Layers.Find(layer1.Identifier);
                            }
                        }
                        if (layer2 == null)
                        {
                            layer2 = this.DefaultLayer;
                        }
                        if (!copyableOnly || layer2.CanInsertObjects())
                        {
                            layer2.Add(obj6);
                        }
                    }
                }
            }
            foreach (Shapes.DiagramShape obj8 in env.Delayeds)
            {
                if (obj8 != null)
                {
                    Shapes.DiagramShape obj9 = env[obj8] as Shapes.DiagramShape;
                    if (obj9 != null)
                    {
                        obj8.CopyObjectDelayed(env, obj9);
                    }
                }
            }
            return env;
        }

        public virtual void CopyNewValueForRedo(ChangedEventArgs e)
        {
            int num1 = e.Hint;
            if (num1 != 220)
            {
                if (num1 == 0x385)
                {
                    e.DiagramShape.CopyNewValueForRedo(e);
                }
            }
            else
            {
                ArrayList list1 = new ArrayList();
                LayerCollectionObjectEnumerator enumerator1 = this.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj2 = enumerator1.Current;
                    list1.Add(obj2);
                    if (obj2 is Shapes.IDiagramLine)
                    {
                        Shapes.LineGraph link1 = obj2 as Shapes.LineGraph;
                        if (link1 != null)
                        {
                            PointF[] tfArray1 = link1.CopyPointsArray();
                            list1.Add(tfArray1);
                            continue;
                        }
                        Shapes.TextLine link2 = obj2 as Shapes.TextLine;
                        if (link2 != null)
                        {
                            PointF[] tfArray2 = link2.RealLink.CopyPointsArray();
                            list1.Add(tfArray2);
                        }
                        continue;
                    }
                    RectangleF ef1 = obj2.Bounds;
                    list1.Add(ef1);
                }
                e.NewValue = list1;
            }
        }

        public virtual void CopyOldValueForUndo(ChangedEventArgs e)
        {
            int num1 = e.Hint;
            if (num1 != 220)
            {
                if (num1 == 0x385)
                {
                    e.DiagramShape.CopyOldValueForUndo(e);
                }
            }
            else if (!e.IsBeforeChanging)
            {
                ChangedEventArgs args1 = e.FindBeforeChangingEdit();
                if (args1 != null)
                {
                    e.OldValue = args1.NewValue;
                }
            }
        }

        public virtual void CopyTo(Array array, int index)
        {
            LayerCollectionEnumerator enumerator1 = this.Layers.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                LayerEnumerator enumerator2 = enumerator1.Current.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Shapes.DiagramShape obj1 = enumerator2.Current;
                    array.SetValue(obj1, index++);
                }
            }
        }

        public void CopyTo(Shapes.DiagramShape[] array, int index)
        {
            //modified by little
            //this.CopyTo(array, index);
            //added by little
            LayerCollectionEnumerator enumerator1 = this.Layers.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                LayerEnumerator enumerator2 = enumerator1.Current.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Shapes.DiagramShape obj1 = enumerator2.Current;
                    array[index++] = obj1;
                }
            }
        }

        public virtual CopyDictionary CreateCopyDictionary()
        {
            CopyDictionary dictionary1 = new CopyDictionary();
            dictionary1.DestinationDocument = this;
            return dictionary1;
        }

        public void EndUpdateViews()
        {
            this.RaiseChanged(0x66, 0, null, 0, null, DiagramDocument.NullRect, 0, null, DiagramDocument.NullRect);
        }

        public virtual void EnsureUniquePartID()
        {
            if (this.myParts == null)
            {
                this.myParts = new Hashtable(1000);
            }
            ArrayList list1 = new ArrayList();
            IDictionaryEnumerator enumerator1 = this.myParts.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DictionaryEntry entry1 = enumerator1.Entry;
                int num1 = (int)entry1.Key;
                IIdentifiablePart part1 = (IIdentifiablePart)entry1.Value;
                if (part1.PartID != num1)
                {
                    list1.Add(entry1);
                }
            }
            foreach (DictionaryEntry entry2 in list1)
            {
                int num2 = (int)entry2.Key;
                IIdentifiablePart part2 = (IIdentifiablePart)entry2.Value;
                int num3 = part2.PartID;
                if (this.myParts[num3] == null)
                {
                    this.myParts.Remove(num2);
                    this.myParts[num3] = part2;
                    continue;
                }
                part2.PartID = num2;
            }
            LayerCollectionObjectEnumerator enumerator3 = this.GetEnumerator();
            while (enumerator3.MoveNext())
            {
                Shapes.DiagramShape obj1 = enumerator3.Current;
                this.AddAllParts(obj1);
            }
        }

        public Shapes.DiagramShape FindNode(string s)
        {
            return this.FindNode(s, false, false, false);
        }

        public Shapes.DiagramShape FindNode(string s, bool prefix, bool ignorecase)
        {
            return this.FindNode(s, prefix, ignorecase, false);
        }

        public virtual Shapes.DiagramShape FindNode(string s, bool prefix, bool ignorecase, bool insidesubgraph)
        {
            string text1 = s;
            CultureInfo info1 = CultureInfo.CurrentCulture;
            if (ignorecase)
            {
                text1 = text1.ToUpper(info1);
            }
            return this.FindNodeInternal(this, text1, prefix, ignorecase, insidesubgraph, info1);
        }

        private Shapes.DiagramShape FindNodeInternal(Shapes.IDiagramShapeCollection coll, string search, bool prefix, bool ignorecase, bool insidesubgraph, CultureInfo ci)
        {
            foreach (Shapes.DiagramShape obj1 in coll)
            {
                Shapes.ITextNode node1 = obj1 as Shapes.ITextNode;
                if (node1 != null)
                {
                    string text1 = node1.Text;
                    if (ignorecase)
                    {
                        text1 = text1.ToUpper(ci);
                    }
                    if (prefix)
                    {
                        if (text1.StartsWith(search))
                        {
                            return obj1;
                        }
                    }
                    else if (text1 == search)
                    {
                        return obj1;
                    }
                    if (insidesubgraph)
                    {
                        Shapes.SubGraphNode graph1 = obj1 as Shapes.SubGraphNode;
                        if (graph1 != null)
                        {
                            this.FindNodeInternal(graph1, search, prefix, ignorecase, insidesubgraph, ci);
                        }
                    }
                }
            }
            return null;
        }

        public IIdentifiablePart FindPart(int id)
        {
            if (this.myParts != null)
            {
                return (IIdentifiablePart)this.myParts[id];
            }
            return null;
        }

        public virtual bool FinishTransaction(string tname)
        {
            DiagramUndoManager manager1 = this.UndoManager;
            if (manager1 != null)
            {
                return manager1.FinishTransaction(tname);
            }
            return false;
        }

        public virtual RectangleF GetAvoidableRectangle(Shapes.DiagramShape obj)
        {
            RectangleF ef1 = obj.Bounds;
            obj.ExpandPaintBounds(ef1, null);
            this.AddAvoidableRectanglePorts(obj, ref ef1);
            return ef1;
        }

        public virtual LayerCollectionObjectEnumerator GetEnumerator()
        {
            return this.Layers.GetObjectEnumerator(true);
        }

        internal PositionArray GetPositions()
        {
            return this.GetPositions(true, null);
        }

        internal PositionArray GetPositions(bool clearunoccupied, Shapes.DiagramShape skip)
        {
            if (this.myPositions == null)
            {
                this.myPositions = new PositionArray();
                this.myPositions.CellSize = new SizeF(this.myPositions.CellSize.Width / this.WorldScale.Width, this.myPositions.CellSize.Height / this.WorldScale.Height);
            }
            if (this.myPositions.Invalid)
            {
                RectangleF ef1 = this.ComputeBounds();
                Shapes.DiagramShape.InflateRect(ref ef1, 200f * this.WorldEpsilon, 200f * this.WorldEpsilon);
                this.myPositions.Initialize(ef1);
                LayerCollectionObjectEnumerator enumerator1 = this.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj1 = enumerator1.Current;
                    this.GetPositions1(obj1, skip);
                }
                this.myPositions.Invalid = false;
            }
            else if (clearunoccupied)
            {
                this.myPositions.SetAllUnoccupied(0x7fffffff);
            }
            return this.myPositions;
        }

        private void GetPositions1(Shapes.DiagramShape obj, Shapes.DiagramShape skip)
        {
            if (obj != skip)
            {
                if (obj is Shapes.SubGraphNode)
                {
                    Shapes.SubGraphNode graph1 = (Shapes.SubGraphNode)obj;
                    Shapes.GroupEnumerator enumerator1 = graph1.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        Shapes.DiagramShape obj1 = enumerator1.Current;
                        this.GetPositions1(obj1, skip);
                    }
                }
                else if (this.IsAvoidable(obj))
                {
                    RectangleF ef1 = this.GetAvoidableRectangle(obj);
                    float single1 = this.myPositions.CellSize.Width;
                    float single2 = this.myPositions.CellSize.Height;
                    for (float single3 = ef1.X; single3 <= (ef1.X + ef1.Width); single3 += single1)
                    {
                        for (float single4 = ef1.Y; single4 <= (ef1.Y + ef1.Height); single4 += single2)
                        {
                            this.myPositions.SetDist(single3, single4, 0);
                        }
                    }
                }
            }
        }

        private void InvalidatePositionArray(Shapes.DiagramShape obj)
        {
            this.mySkippedAvoidable = null;
            if (((this.myPositions != null) && !this.myPositions.Invalid) && ((obj == null) || this.IsAvoidable(obj)))
            {
                this.myPositions.Invalid = true;
            }
        }

        public void InvalidateViews()
        {
            this.RaiseChanged(100, 0, null, 0, null, DiagramDocument.NullRect, 0, null, DiagramDocument.NullRect);
        }

        private void invokeOnChanged(int hint, int subhint, object obj, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect, bool before)
        {
            if (!this.SuspendsUpdates)
            {
                ChangedEventArgs args1 = this.myChangedEventArgs;
                if (args1 == null)
                {
                    args1 = new ChangedEventArgs();
                    args1.Document = this;
                }
                args1.IsBeforeChanging = before;
                args1.Hint = hint;
                args1.SubHint = subhint;
                args1.Object = obj;
                args1.OldInt = oldI;
                args1.OldValue = oldVal;
                args1.OldRect = oldRect;
                args1.NewInt = newI;
                args1.NewValue = newVal;
                args1.NewRect = newRect;
                this.myChangedEventArgs = null;
                this.OnChanged(args1);
                this.myChangedEventArgs = args1;
                args1.Object = null;
                args1.OldValue = null;
                args1.NewValue = null;
            }
        }

        public virtual bool IsAvoidable(Shapes.DiagramShape obj)
        {
            return (obj is Shapes.IDiagramNode);
        }

        public bool IsUnoccupied(RectangleF r, Shapes.DiagramShape skip)
        {
            if (skip != this.mySkippedAvoidable)
            {
                this.InvalidatePositionArray(null);
                this.mySkippedAvoidable = skip;
            }
            PositionArray array1 = this.GetPositions(false, skip);
            return array1.IsUnoccupied(r.X, r.Y, r.Width, r.Height);
        }

        public static bool MakesDirectedCycle(Shapes.IDiagramNode a, Shapes.IDiagramNode b)
        {
            bool flag2;
            if (a == b)
            {
                return true;
            }
            lock (DiagramDocument.myCycleMap)
            {
                DiagramDocument.myCycleMap.Clear();
                DiagramDocument.myCycleMap.Add(a, null);
                bool flag1 = DiagramDocument.MakesDirectedCycle1(a, b, DiagramDocument.myCycleMap);
                DiagramDocument.myCycleMap.Clear();
                flag2 = flag1;
            }
            return flag2;
        }

        private static bool MakesDirectedCycle1(Shapes.IDiagramNode a, Shapes.IDiagramNode b, Hashtable map)
        {
            if (a == b)
            {
                return true;
            }
            if (!map.Contains(b))
            {
                map.Add(b, null);
                foreach (Shapes.IDiagramNode node1 in b.Destinations)
                {
                    if ((node1 != b) && DiagramDocument.MakesDirectedCycle1(a, node1, map))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool MakesDirectedCycleFast(Shapes.IDiagramNode a, Shapes.IDiagramNode b)
        {
            if (a == b)
            {
                return true;
            }
            foreach (Shapes.IDiagramNode node1 in b.Destinations)
            {
                if ((node1 != b) && DiagramDocument.MakesDirectedCycleFast(a, node1))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool MakesUndirectedCycle(Shapes.IDiagramNode a, Shapes.IDiagramNode b)
        {
            bool flag2;
            if (a == b)
            {
                return true;
            }
            lock (DiagramDocument.myCycleMap)
            {
                DiagramDocument.myCycleMap.Clear();
                DiagramDocument.myCycleMap.Add(a, null);
                bool flag1 = DiagramDocument.MakesUndirectedCycle1(a, b, DiagramDocument.myCycleMap);
                DiagramDocument.myCycleMap.Clear();
                flag2 = flag1;
            }
            return flag2;
        }

        private static bool MakesUndirectedCycle1(Shapes.IDiagramNode a, Shapes.IDiagramNode b, Hashtable map)
        {
            if (a == b)
            {
                return true;
            }
            if (!map.Contains(b))
            {
                map.Add(b, null);
                foreach (Shapes.IDiagramNode node1 in b.Nodes)
                {
                    if ((node1 != b) && DiagramDocument.MakesUndirectedCycle1(a, node1, map))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void MergeLayersFrom(DiagramDocument other)
        {
            LayerCollectionEnumerator enumerator1 = other.Layers.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                object obj1 = enumerator1.Current.Identifier;
                if ((obj1 != null) && (this.Layers.Find(obj1) == null))
                {
                    DiagramLayer layer2 = this.Layers.CreateNewLayerAfter(this.Layers.Top);
                    layer2.Identifier = obj1;
                }
            }
            object obj2 = other.DefaultLayer.Identifier;
            DiagramLayer layer3 = this.Layers.Find(obj2);
            if (layer3 != null)
            {
                this.DefaultLayer = layer3;
            }
        }

        IEnumerable Shapes.IDiagramShapeCollection.Backwards
        {
            get
            {
                return this.Layers.GetObjectEnumerator(false);
            }
        }

        protected virtual void OnChanged(ChangedEventArgs evt)
        {
            if (this.Changed != null)
            {
                this.Changed(this, evt);
            }
            int num1 = evt.Hint;
            if (!this.SkipsUndoManager)
            {
                DiagramUndoManager manager1 = this.UndoManager;
                if (manager1 != null)
                {
                    manager1.DocumentChanged(this, evt);
                }
                if (((num1 < 0) || (num1 >= 200)) && ((num1 != 0x385) || (evt.SubHint != 1000)))
                {
                    this.IsModified = true;
                }
            }
            if (num1 == 0x385)
            {
                if (evt.SubHint == 0x3e9)
                {
                    Shapes.DiagramShape obj1 = evt.DiagramShape;
                    this.UpdateDocumentBounds(obj1);
                    this.InvalidatePositionArray(obj1);
                    if (obj1.IsTopLevel)
                    {
                        DiagramLayer layer1 = obj1.Layer;
                        if (layer1 == null)
                        {
                            return;
                        }
                        layer1.UpdateCache(obj1, evt);
                    }
                }
                else if (evt.SubHint == 0x41b)
                {
                    if (this.PartIDIndexable)
                    {
                        Shapes.DiagramShape obj2 = evt.DiagramShape;
                        this.AddAllParts(obj2);
                    }
                }
                else if (evt.SubHint == 0x41c)
                {
                    Shapes.DiagramShape obj3 = evt.DiagramShape;
                    this.RemoveAllParts(obj3);
                }
            }
            else if (num1 == 0x386)
            {
                Shapes.DiagramShape obj4 = evt.DiagramShape;
                if (this.PartIDIndexable)
                {
                    this.AddAllParts(obj4);
                }
                this.UpdateDocumentBounds(obj4);
                this.InvalidatePositionArray(obj4);
            }
            else if (num1 == 0x387)
            {
                Shapes.DiagramShape obj5 = evt.DiagramShape;
                this.RemoveAllParts(obj5);
                this.InvalidatePositionArray(obj5);
            }
            else if (num1 == 0x321)
            {
                this.InvalidatePositionArray(null);
            }
            else if (num1 == 0x322)
            {
                this.InvalidatePositionArray(null);
                if (evt.Object == this.LinksLayer)
                {
                    this.LinksLayer = this.DefaultLayer;
                }
            }
        }

        public virtual Shapes.DiagramShape PickObject(PointF p, bool selectableOnly)
        {
            if (!selectableOnly || this.CanSelectObjects())
            {
                LayerCollectionEnumerator enumerator1 = this.Layers.Backwards.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj1 = enumerator1.Current.PickObject(p, selectableOnly);
                    if (obj1 != null)
                    {
                        return obj1;
                    }
                }
            }
            return null;
        }

        public virtual Shapes.IDiagramShapeCollection PickObjects(PointF p, bool selectableOnly, Shapes.IDiagramShapeCollection coll, int max)
        {
            if (selectableOnly && !this.CanSelectObjects())
            {
                return null;
            }
            if (coll == null)
            {
                coll = new Shapes.DiagramShapeCollection();
            }
            LayerCollectionEnumerator enumerator1 = this.Layers.Backwards.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramLayer layer1 = enumerator1.Current;
                if (coll.Count >= max)
                {
                    return coll;
                }
                layer1.PickObjects(p, selectableOnly, coll, max);
            }
            return coll;
        }

        public virtual void RaiseChanged(int hint, int subhint, object obj, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            this.invokeOnChanged(hint, subhint, obj, oldI, oldVal, oldRect, newI, newVal, newRect, false);
        }

        public virtual void RaiseChanging(int hint, int subhint, object obj)
        {
            this.invokeOnChanged(hint, subhint, obj, 0, null, DiagramDocument.NullRect, 0, null, DiagramDocument.NullRect, true);
        }

        public virtual void Redo()
        {
            if (this.CanRedo())
            {
                DiagramUndoManager manager1 = this.UndoManager;
                if (manager1 != null)
                {
                    manager1.Redo();
                }
            }
        }

        public virtual void Remove(Shapes.DiagramShape obj)
        {
            if (obj != null)
            {
                DiagramLayer layer1 = obj.Layer;
                if (layer1 != null)
                {
                    if (layer1.Document != this)
                    {
                        throw new ArgumentException("Cannot remove object that does not belong to this document");
                    }
                    layer1.Remove(obj);
                }
            }
        }

        internal void RemoveAllParts(Shapes.DiagramShape obj)
        {
            if (this.myParts != null)
            {
                IIdentifiablePart part1 = obj as IIdentifiablePart;
                if (part1 != null)
                {
                    this.RemovePart(part1);
                }
                Shapes.GroupShape group1 = obj as Shapes.GroupShape;
                if (group1 != null)
                {
                    Shapes.GroupEnumerator enumerator2 = group1.GetEnumerator();
                    Shapes.GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        Shapes.DiagramShape obj1 = enumerator1.Current;
                        this.RemoveAllParts(obj1);
                    }
                }
            }
        }

        internal void RemovePart(IIdentifiablePart p)
        {
            if (this.myParts != null)
            {
                this.myParts.Remove(p.PartID);
            }
        }

        public virtual void SetModifiable(bool b)
        {
            this.AllowMove = b;
            this.AllowResize = b;
            this.AllowReshape = b;
            this.AllowDelete = b;
            this.AllowInsert = b;
            this.AllowLink = b;
            this.AllowEdit = b;
        }

        private void setWorldScale(SizeF ws, bool undoing)
        {
            SizeF ef1 = this.myWorldScale;
            if (((ef1 != ws) && (ws.Width > 0f)) && (ws.Height > 0f))
            {
                this.myWorldScale = ws;
                this.InvalidatePositionArray(null);
                this.RaiseChanged(0xe2, 0, null, 0, null, Shapes.DiagramShape.MakeRect(ef1), 0, null, Shapes.DiagramShape.MakeRect(ws));
                if (!undoing)
                {
                    RectangleF ef2 = this.ComputeBounds();
                    this.TopLeft = new PointF(ef2.X, ef2.Y);
                    this.Size = new SizeF(ef2.Width, ef2.Height);
                    if (this.myPositions != null)
                    {
                        SizeF ef3 = this.myPositions.CellSize;
                        this.myPositions.CellSize = new SizeF((ef3.Width * ef1.Width) / ws.Width, (ef3.Height * ef1.Height) / ws.Height);
                    }
                }
            }
        }

        public virtual bool StartTransaction()
        {
            DiagramUndoManager manager1 = this.UndoManager;
            if (manager1 != null)
            {
                return manager1.StartTransaction();
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Layers.GetObjectEnumerator(true);
        }

        public virtual void Undo()
        {
            if (this.CanUndo())
            {
                DiagramUndoManager manager1 = this.UndoManager;
                if (manager1 != null)
                {
                    manager1.Undo();
                }
            }
        }

        public virtual void UpdateDocumentBounds(Shapes.DiagramShape obj)
        {
            if ((obj != null) && !this.FixedSize)
            {
                SizeF ef1 = this.Size;
                PointF tf1 = this.TopLeft;
                RectangleF ef2 = obj.Bounds;
                float single1 = System.Math.Min(tf1.X, ef2.X);
                float single2 = System.Math.Min(tf1.Y, ef2.Y);
                float single3 = System.Math.Max((float)(tf1.X + ef1.Width), (float)(ef2.X + ef2.Width));
                float single4 = System.Math.Max((float)(tf1.Y + ef1.Height), (float)(ef2.Y + ef2.Height));
                float single5 = single3 - single1;
                float single6 = single4 - single2;
                if ((single1 < tf1.X) || (single2 < tf1.Y))
                {
                    this.TopLeft = new PointF(single1, single2);
                }
                if ((single5 > ef1.Width) || (single6 > ef1.Height))
                {
                    this.Size = new SizeF(single5, single6);
                }
            }
        }

        public void UpdateViews()
        {
            this.RaiseChanged(0x67, 0, null, 0, null, DiagramDocument.NullRect, 0, null, DiagramDocument.NullRect);
        }


        [Description("Whether the user can copy selected objects in this document."), Category("Behavior"), DefaultValue(true)]
        public virtual bool AllowCopy
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
                    this.RaiseChanged(0xd1, 0, null, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("Whether the user can delete selected objects in this document."), DefaultValue(true)]
        public virtual bool AllowDelete
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
                    this.RaiseChanged(0xd4, 0, null, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [DefaultValue(true), Category("Behavior"), Description("Whether the user can edit objects in this document.")]
        public virtual bool AllowEdit
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
                    this.RaiseChanged(0xd7, 0, null, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Description("Whether the user can insert objects into this document."), Category("Behavior"), DefaultValue(true)]
        public virtual bool AllowInsert
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
                    this.RaiseChanged(0xd5, 0, null, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [DefaultValue(true), Category("Behavior"), Description("Whether the user can link ports in this document.")]
        public virtual bool AllowLink
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
                    this.RaiseChanged(0xd6, 0, null, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Description("Whether the user can move selected objects in this document."), Category("Behavior"), DefaultValue(true)]
        public virtual bool AllowMove
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
                    this.RaiseChanged(0xd0, 0, null, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Description("Whether the user can reshape resizable objects in this document."), Category("Behavior"), DefaultValue(true)]
        public virtual bool AllowReshape
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
                    this.RaiseChanged(0xd3, 0, null, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("Whether the user can resize selected objects in this document."), DefaultValue(true)]
        public virtual bool AllowResize
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
                    this.RaiseChanged(210, 0, null, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the user can select objects in this document.")]
        public virtual bool AllowSelect
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
                    this.RaiseChanged(0xcf, 0, this, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Browsable(false)]
        public virtual LayerCollectionObjectEnumerator Backwards
        {
            get
            {
                return this.Layers.GetObjectEnumerator(false);
            }
        }

        [Description("The total number of objects in all document layers.")]
        public virtual int Count
        {
            get
            {
                int num1 = 0;
                LayerCollectionEnumerator enumerator1 = this.Layers.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramLayer layer1 = enumerator1.Current;
                    num1 += layer1.Count;
                }
                return num1;
            }
        }

        [Description("The data format name used for the clipboard.")]
        public virtual string DataFormat
        {
            get
            {
                if (this.myDataFormat == null)
                {
                    this.myDataFormat = base.GetType().FullName;
                }
                return this.myDataFormat;
            }
            set
            {
                if (this.myDataFormat == null)
                {
                    this.myDataFormat = base.GetType().FullName;
                }
                string text1 = this.myDataFormat;
                if ((value != null) && (text1 != value))
                {
                    this.myDataFormat = value;
                    this.RaiseChanged(0xce, 0, null, 0, text1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Description("The default layer used when adding objects to the document.")]
        public virtual DiagramLayer DefaultLayer
        {
            get
            {
                return this.Layers.Default;
            }
            set
            {
                this.Layers.Default = value;
            }
        }

        [Description("Whether adding or moving objects in the document leaves the document size and top-left unchanged."), Category("Behavior"), DefaultValue(false)]
        public virtual bool FixedSize
        {
            get
            {
                return this.myFixedSize;
            }
            set
            {
                bool flag1 = this.myFixedSize;
                if (flag1 != value)
                {
                    this.myFixedSize = value;
                    this.RaiseChanged(0xcc, 0, null, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return (this.Count == 0);
            }
        }

        public bool IsModified
        {
            get
            {
                if (this.UndoManager == null)
                {
                    return this.myIsModified;
                }
                if (this.UndoManager.CurrentEdit != null)
                {
                    return true;
                }
                if (this.myIsModified)
                {
                    return (this.myUndoEditIndex != this.UndoManager.UndoEditIndex);
                }
                return false;
            }
            set
            {
                bool flag1 = this.myIsModified;
                this.myIsModified = value;
                if (!value && (this.UndoManager != null))
                {
                    this.myUndoEditIndex = this.UndoManager.UndoEditIndex;
                }
                if (flag1 != value)
                {
                    this.InvalidateViews();
                }
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

        [Browsable(false)]
        public virtual LayerCollection Layers
        {
            get
            {
                return this.myLayers;
            }
        }

        [Description("The default layer used when adding links to the document.")]
        public virtual DiagramLayer LinksLayer
        {
            get
            {
                return this.myLinksLayer;
            }
            set
            {
                DiagramLayer layer1 = this.myLinksLayer;
                if (layer1 != value)
                {
                    if ((value == null) || (value.Document != this))
                    {
                        throw new ArgumentException("The new value for DiagramDocument.LinksLayer must belong to this document.");
                    }
                    this.myLinksLayer = value;
                    this.RaiseChanged(0xdf, 0, null, 0, layer1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("Whether all the IGoIdentifiableParts in this document have a unique PartID"), DefaultValue(false)]
        public bool PartIDIndexable
        {
            get
            {
                return this.myPartIDIndexable;
            }
            set
            {
                bool flag1 = this.myPartIDIndexable;
                if (flag1 != value)
                {
                    this.myPartIDIndexable = value;
                    this.RaiseChanged(0xe0, 0, null, 0, flag1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                    if (value)
                    {
                        this.EnsureUniquePartID();
                    }
                    else
                    {
                        this.myParts = null;
                    }
                }
            }
        }

        [DefaultValue(""), Description("The user-visible name for this document.")]
        public virtual string Name
        {
            get
            {
                return this.myName;
            }
            set
            {
                string text1 = this.myName;
                if ((value != null) && (text1 != value))
                {
                    this.myName = value;
                    this.RaiseChanged(0xc9, 0, null, 0, text1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Description("The color of the document's background."), Category("Appearance")]
        public virtual Color PaperColor
        {
            get
            {
                return this.myPaperColor;
            }
            set
            {
                Color color1 = this.myPaperColor;
                if (color1 != value)
                {
                    this.myPaperColor = value;
                    this.RaiseChanged(0xcd, 0, null, 0, color1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Description("Whether the UndoManager is serialized along with the document")]
        public bool SerializesUndoManager
        {
            get
            {
                return this.mySerializesUndoManager;
            }
            set
            {
                this.mySerializesUndoManager = value;
                if (value)
                {
                    this.mySerializedUndoManager = this.myUndoManager;
                }
                else
                {
                    this.mySerializedUndoManager = null;
                }
            }
        }

        [Description("The size of this document.")]
        public virtual SizeF Size
        {
            get
            {
                return this.myDocumentSize;
            }
            set
            {
                if (value.Width < 0f)
                {
                    if (value.Width == -23f)
                    {
                        if (value.Height == -23f)
                        {
                            DiagramDocument.myCaching = true;
                        }
                        else if (value.Height == -24f)
                        {
                            DiagramDocument.myCaching = false;
                            LayerCollectionEnumerator enumerator1 = this.Layers.GetEnumerator();
                            while (enumerator1.MoveNext())
                            {
                                enumerator1.Current.ResetCache();
                            }
                        }
                    }
                    else if (value.Width == -24f)
                    {
                        this.WorldScale = new SizeF(value.Height, value.Height);
                    }
                    else if ((value.Width == -25f) && (this.myPositions != null))
                    {
                        this.myPositions.CellSize = new SizeF(value.Height, value.Height);
                    }
                }
                SizeF ef1 = this.myDocumentSize;
                if (((value.Width >= 0f) && (value.Height >= 0f)) && (ef1 != value))
                {
                    this.myDocumentSize = value;
                    this.RaiseChanged(0xca, 0, null, 0, null, Shapes.DiagramShape.MakeRect(ef1), 0, null, Shapes.DiagramShape.MakeRect(value));
                }
            }
        }

        [Browsable(false)]
        public bool SkipsUndoManager
        {
            get
            {
                return this.mySkipsUndoManager;
            }
            set
            {
                this.mySkipsUndoManager = value;
            }
        }

        [Browsable(false)]
        public bool SuspendsUpdates
        {
            get
            {
                return this.mySuspendsUpdates;
            }
            set
            {
                this.mySuspendsUpdates = value;
                if (!value)
                {
                    this.InvalidatePositionArray(null);
                    LayerCollectionEnumerator enumerator1 = this.Layers.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        enumerator1.Current.ResetCache();
                    }
                }
            }
        }

        [Browsable(false)]
        public virtual object SyncRoot
        {
            get
            {
                return this;
            }
        }

        [Description("The top-left corner position of this document.")]
        public virtual PointF TopLeft
        {
            get
            {
                return this.myDocumentTopLeft;
            }
            set
            {
                PointF tf1 = this.myDocumentTopLeft;
                if (tf1 != value)
                {
                    this.myDocumentTopLeft = value;
                    this.RaiseChanged(0xcb, 0, null, 0, null, Shapes.DiagramShape.MakeRect(tf1), 0, null, Shapes.DiagramShape.MakeRect(value));
                }
            }
        }

        [Description("The UndoManager for this document.")]
        public virtual DiagramUndoManager UndoManager
        {
            get
            {
                return this.myUndoManager;
            }
            set
            {
                if (this.myUndoManager != value)
                {
                    if (this.myUndoManager != null)
                    {
                        this.myUndoManager.RemoveDocument(this);
                    }
                    this.myUndoManager = value;
                    if (this.SerializesUndoManager)
                    {
                        this.mySerializedUndoManager = value;
                    }
                    this.myIsModified = false;
                    this.myUndoEditIndex = -2;
                    if (this.myUndoManager != null)
                    {
                        this.myUndoManager.AddDocument(this);
                    }
                }
            }
        }

        [Description("An integer value associated with this document."), DefaultValue(0)]
        public virtual int UserFlags
        {
            get
            {
                return this.myUserFlags;
            }
            set
            {
                int num1 = this.myUserFlags;
                if (num1 != value)
                {
                    this.myUserFlags = value;
                    this.RaiseChanged(0xdd, 0, null, num1, null, DiagramDocument.NullRect, value, null, DiagramDocument.NullRect);
                }
            }
        }

        [DefaultValue((string)null), Description("An object associated with this document.")]
        public virtual object UserObject
        {
            get
            {
                return this.myUserObject;
            }
            set
            {
                object obj1 = this.myUserObject;
                if (obj1 != value)
                {
                    this.myUserObject = value;
                    this.RaiseChanged(0xde, 0, null, 0, obj1, DiagramDocument.NullRect, 0, value, DiagramDocument.NullRect);
                }
            }
        }

        [Description("Whether a valid link can produce a cycle in the graph."), Category("Behavior"), DefaultValue(0)]
        public virtual DocumentValidCycle ValidCycle
        {
            get
            {
                return this.myValidCycle;
            }
            set
            {
                DocumentValidCycle cycle1 = this.myValidCycle;
                if (cycle1 != value)
                {
                    this.myValidCycle = value;
                    this.RaiseChanged(0xe1, 0, null, (int)cycle1, null, DiagramDocument.NullRect, (int)value, 0, DiagramDocument.NullRect);
                }
            }
        }

        internal float WorldEpsilon
        {
            get
            {
                return (0.5f / this.WorldScale.Width);
            }
        }

        internal SizeF WorldScale
        {
            get
            {
                return this.myWorldScale;
            }
            set
            {
                this.setWorldScale(value, false);
            }
        }


        public const int AllArranged = 220;
        public const int BeginUpdateAllViews = 0x65;
        public const int ChangedAllowCopy = 0xd1;
        public const int ChangedAllowDelete = 0xd4;
        public const int ChangedAllowEdit = 0xd7;
        public const int ChangedAllowInsert = 0xd5;
        public const int ChangedAllowLink = 0xd6;
        public const int ChangedAllowMove = 0xd0;
        public const int ChangedAllowReshape = 0xd3;
        public const int ChangedAllowResize = 210;
        public const int ChangedAllowSelect = 0xcf;
        public const int ChangedDataFormat = 0xce;
        public const int ChangedFixedSize = 0xcc;
        public const int ChangedLinksLayer = 0xdf;
        public const int ChangedMaintainsPartID = 0xe0;
        public const int ChangedName = 0xc9;
        public const int ChangedPaperColor = 0xcd;
        public const int ChangedSize = 0xca;
        public const int ChangedTopLeft = 0xcb;
        public const int ChangedUserFlags = 0xdd;
        public const int ChangedUserObject = 0xde;
        public const int ChangedValidCycle = 0xe1;
        internal const int ChangedWorldScale = 0xe2;
        private const float DOWN = 90f;
        public const int EndUpdateAllViews = 0x66;
        internal const int FirstStateChangedHint = 200;
        public const int LastHint = 10000;
        private const float LEFT = 180f;
        private bool myAllowCopy;
        private bool myAllowDelete;
        private bool myAllowEdit;
        private bool myAllowInsert;
        private bool myAllowLink;
        private bool myAllowMove;
        private bool myAllowReshape;
        private bool myAllowResize;
        private bool myAllowSelect;
        internal static bool myCaching;
        [NonSerialized]
        private ChangedEventArgs myChangedEventArgs;
        private static Hashtable myCycleMap;
        private string myDataFormat;
        private SizeF myDocumentSize;
        private PointF myDocumentTopLeft;
        private bool myFixedSize;
        [NonSerialized]
        private bool myIsModified;
        private int myLastPartID;
        private LayerCollection myLayers;
        private DiagramLayer myLinksLayer;
        private bool myPartIDIndexable;
        private string myName;
        private Color myPaperColor;
        [NonSerialized]
        private Hashtable myParts;
        [NonSerialized]
        private PositionArray myPositions;
        private DiagramUndoManager mySerializedUndoManager;
        private bool mySerializesUndoManager;
        [NonSerialized]
        private Shapes.DiagramShape mySkippedAvoidable;
        private bool mySkipsUndoManager;
        private bool mySuspendsUpdates;
        private int myUndoEditIndex;
        [NonSerialized]
        private DiagramUndoManager myUndoManager;
        private int myUserFlags;
        private object myUserObject;
        private DocumentValidCycle myValidCycle;
        private SizeF myWorldScale;
        protected static readonly RectangleF NullRect;
        public const int RepaintAll = 100;
        private const float RIGHT = 0f;
        private const float UP = 270f;
        public const int UpdateAllViews = 0x67;
    }
}
