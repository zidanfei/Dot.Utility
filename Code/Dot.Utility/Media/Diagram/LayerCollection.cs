using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public sealed class LayerCollection : ICollection, IEnumerable
    {
        static LayerCollection()
        {
            LayerCollection.NullRect = new RectangleF();
        }

        internal LayerCollection()
        {
            this.myLayerCollectionContainer = null;
            this.myLayers = new ArrayList();
            this.myDefaultLayer = null;
        }

        public DiagramLayer[] CopyArray()
        {
            DiagramLayer[] layerArray1 = new DiagramLayer[this.Count];
            this.CopyTo(layerArray1, 0);
            return layerArray1;
        }

        public void CopyTo(Array array, int index)
        {
            this.myLayers.CopyTo(array, index);
        }

        public void CopyTo(DiagramLayer[] array, int index)
        {
            this.myLayers.CopyTo(array, index);
        }

        public DiagramLayer CreateNewLayerAfter(DiagramLayer dest)
        {
            if ((dest != null) && (this.IndexOf(dest) < 0))
            {
                throw new ArgumentException("Cannot create a new layer after a layer that is not in this layer collection.");
            }
            DiagramLayer layer1 = new DiagramLayer();
            layer1.init(this.LayerCollectionContainer);
            this.InsertAfter(dest, layer1);
            return layer1;
        }

        public DiagramLayer CreateNewLayerBefore(DiagramLayer dest)
        {
            if ((dest != null) && (this.IndexOf(dest) < 0))
            {
                throw new ArgumentException("Cannot create a new layer before a layer that is not in this layer collection.");
            }
            DiagramLayer layer1 = new DiagramLayer();
            layer1.init(this.LayerCollectionContainer);
            this.InsertBefore(dest, layer1);
            return layer1;
        }

        public DiagramLayer Find(object identifier)
        {
            if (identifier != null)
            {
                LayerCollectionEnumerator enumerator1 = this.Backwards.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramLayer layer1 = enumerator1.Current;
                    object obj1 = layer1.Identifier;
                    if ((obj1 != null) && obj1.Equals(identifier))
                    {
                        return layer1;
                    }
                }
            }
            return null;
        }

        public LayerCollectionEnumerator GetEnumerator()
        {
            return new LayerCollectionEnumerator(this.myLayers, true);
        }

        public LayerCollectionObjectEnumerator GetObjectEnumerator(bool forward)
        {
            return new LayerCollectionObjectEnumerator(this.myLayers, forward);
        }

        internal int IndexOf(DiagramLayer layer)
        {
            return this.myLayers.IndexOf(layer);
        }

        internal void init(ILayerCollectionContainer lcc)
        {
            this.myLayerCollectionContainer = lcc;
            this.myDefaultLayer = new DiagramLayer();
            this.myDefaultLayer.init(this.myLayerCollectionContainer);
            this.myLayers.Add(this.myDefaultLayer);
            this.myDefaultLayer.Identifier = 0;
        }

        internal void InsertAfter(DiagramLayer dest, DiagramLayer newlayer)
        {
            if (dest == null)
            {
                dest = this.Top;
            }
            int num1 = this.IndexOf(dest);
            if (num1 >= 0)
            {
                this.myLayers.Insert(num1 + 1, newlayer);
                this.LayerCollectionContainer.RaiseChanged(0x321, 1, newlayer, num1, dest, LayerCollection.NullRect, num1 + 1, newlayer, LayerCollection.NullRect);
            }
        }

        internal void InsertBefore(DiagramLayer dest, DiagramLayer newlayer)
        {
            if (dest == null)
            {
                dest = this.Bottom;
            }
            int num1 = this.IndexOf(dest);
            if (num1 >= 0)
            {
                this.myLayers.Insert(num1, newlayer);
                this.LayerCollectionContainer.RaiseChanged(0x321, 0, newlayer, num1, dest, LayerCollection.NullRect, num1, newlayer, LayerCollection.NullRect);
            }
        }

        public void InsertDocumentLayerAfter(DiagramLayer dest, DiagramLayer doclayer)
        {
            int num1 = this.IndexOf(doclayer);
            if (num1 < 0)
            {
                if ((dest != null) && (this.IndexOf(dest) < 0))
                {
                    throw new ArgumentException("Cannot insert a document layer after a layer that is not in this layer collection.");
                }
                DiagramView view1 = this.View;
                if (view1 == null)
                {
                    throw new ArgumentException("Cannot insert a layer into a document layer collection.");
                }
                if (((doclayer == null) || !doclayer.IsInDocument) || (view1.Document != doclayer.Document))
                {
                    throw new ArgumentException("Layer to be inserted into a view layer collection must be a document layer in the view's document.");
                }
                this.InsertAfter(dest, doclayer);
            }
        }

        public void InsertDocumentLayerBefore(DiagramLayer dest, DiagramLayer doclayer)
        {
            int num1 = this.IndexOf(doclayer);
            if (num1 < 0)
            {
                if ((dest != null) && (this.IndexOf(dest) < 0))
                {
                    throw new ArgumentException("Cannot insert a document layer before a layer that is not in this layer collection.");
                }
                DiagramView view1 = this.View;
                if (view1 == null)
                {
                    throw new ArgumentException("Cannot insert a layer into a document layer collection.");
                }
                if (((doclayer == null) || !doclayer.IsInDocument) || (view1.Document != doclayer.Document))
                {
                    throw new ArgumentException("Layer to be inserted into a view layer collection must be a document layer in the view's document.");
                }
                this.InsertBefore(dest, doclayer);
            }
        }

        public void MoveAfter(DiagramLayer dest, DiagramLayer moving)
        {
            if (dest == null)
            {
                dest = this.Top;
            }
            if (dest == moving)
            {
                throw new ArgumentException("Cannot move a layer after (on top of) itself");
            }
            int num1 = this.IndexOf(moving);
            if (num1 < 0)
            {
                throw new ArgumentException("MoveAfter layer to be moved must be in the GoLayerCollection");
            }
            int num2 = this.IndexOf(dest);
            if (num2 < 0)
            {
                throw new ArgumentException("MoveAfter destination layer must be in the GoLayerCollection");
            }
            if (num2 > num1)
            {
                num2--;
            }
            if ((num2 + 1) != num1)
            {
                this.myLayers.RemoveAt(num1);
                this.myLayers.Insert(num2 + 1, moving);
                this.LayerCollectionContainer.RaiseChanged(0x323, 1, moving, num1, dest, LayerCollection.NullRect, num2 + 1, dest, LayerCollection.NullRect);
            }
        }

        public void MoveBefore(DiagramLayer dest, DiagramLayer moving)
        {
            if (dest == null)
            {
                dest = this.Bottom;
            }
            if (dest == moving)
            {
                throw new ArgumentException("Cannot move a layer before (behind) itself");
            }
            int num1 = this.IndexOf(moving);
            if (num1 < 0)
            {
                throw new ArgumentException("MoveBefore layer to be moved must be in the GoLayerCollection");
            }
            int num2 = this.IndexOf(dest);
            if (num2 < 0)
            {
                throw new ArgumentException("MoveBefore destination layer must be in the GoLayerCollection");
            }
            if (num2 > num1)
            {
                num2--;
            }
            if (num2 != num1)
            {
                this.myLayers.RemoveAt(num1);
                this.myLayers.Insert(num2, moving);
                this.LayerCollectionContainer.RaiseChanged(0x323, 0, moving, num1, dest, LayerCollection.NullRect, num2, dest, LayerCollection.NullRect);
            }
        }

        public void Remove(DiagramLayer layer)
        {
            if (layer != null)
            {
                int num1 = this.IndexOf(layer);
                if (num1 >= 0)
                {
                    if (layer.LayerCollectionContainer == this.LayerCollectionContainer)
                    {
                        layer.Clear();
                    }
                    DiagramLayer layer1 = null;
                    foreach (DiagramLayer layer2 in this.myLayers)
                    {
                        if ((layer2 != layer) && (layer2.LayerCollectionContainer == this.LayerCollectionContainer))
                        {
                            layer1 = layer2;
                            break;
                        }
                    }
                    if (layer1 != null)
                    {
                        DiagramLayer layer3 = null;
                        if ((num1 + 1) < this.myLayers.Count)
                        {
                            layer3 = (DiagramLayer)this.myLayers[num1 + 1];
                        }
                        this.myLayers.RemoveAt(num1);
                        this.LayerCollectionContainer.RaiseChanged(0x322, 0, layer, 0, layer3, LayerCollection.NullRect, 0, null, LayerCollection.NullRect);
                        if (layer == this.Default)
                        {
                            this.Default = layer1;
                        }
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LayerCollectionEnumerator(this.myLayers, true);
        }


        public LayerCollectionEnumerator Backwards
        {
            get
            {
                return new LayerCollectionEnumerator(this.myLayers, false);
            }
        }

        public DiagramLayer Bottom
        {
            get
            {
                return (DiagramLayer)this.myLayers[0];
            }
        }

        public int Count
        {
            get
            {
                return this.myLayers.Count;
            }
        }

        public DiagramLayer Default
        {
            get
            {
                return this.myDefaultLayer;
            }
            set
            {
                DiagramLayer layer1 = this.myDefaultLayer;
                if (layer1 != value)
                {
                    if ((value == null) || (value.LayerCollectionContainer != this.LayerCollectionContainer))
                    {
                        throw new ArgumentException("The new GoLayerCollection.Default layer must belong to the same document or view.");
                    }
                    this.myDefaultLayer = value;
                    this.LayerCollectionContainer.RaiseChanged(0x324, 0, null, 0, layer1, LayerCollection.NullRect, 0, value, LayerCollection.NullRect);
                }
            }
        }

        public DiagramDocument Document
        {
            get
            {
                return (this.myLayerCollectionContainer as DiagramDocument);
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public ILayerCollectionContainer LayerCollectionContainer
        {
            get
            {
                return this.myLayerCollectionContainer;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.LayerCollectionContainer;
            }
        }

        public DiagramLayer Top
        {
            get
            {
                return (DiagramLayer)this.myLayers[this.Count - 1];
            }
        }

        public DiagramView View
        {
            get
            {
                return (this.myLayerCollectionContainer as DiagramView);
            }
        }


        public const int ChangedDefault = 0x324;
        public const int InsertedLayer = 0x321;
        public const int MovedLayer = 0x323;
        private DiagramLayer myDefaultLayer;
        private ILayerCollectionContainer myLayerCollectionContainer;
        private ArrayList myLayers;
        private static readonly RectangleF NullRect;
        public const int RemovedLayer = 0x322;
    }
}
