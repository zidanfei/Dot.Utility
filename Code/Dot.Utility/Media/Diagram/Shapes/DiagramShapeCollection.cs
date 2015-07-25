using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class DiagramShapeCollection : IDiagramShapeCollection, ICollection, IEnumerable
    {
        public DiagramShapeCollection()
        {
            this.myObjects = new ArrayList();
        }

        public virtual void Add(DiagramShape obj)
        {
            if ((obj != null) && !this.Contains(obj))
            {
                this.myObjects.Add(obj);
            }
        }

        public virtual void Clear()
        {
            for (int num1 = this.myObjects.Count; num1 > 0; num1 = System.Math.Min(num1, this.myObjects.Count))
            {
                DiagramShape obj1 = (DiagramShape)this.myObjects[--num1];
                this.Remove(obj1);
            }
        }

        public virtual bool Contains(DiagramShape obj)
        {
            if (obj == null)
            {
                return false;
            }
            return this.myObjects.Contains(obj);
        }

        public virtual DiagramShape[] CopyArray()
        {
            DiagramShape[] objArray1 = new DiagramShape[this.Count];
            this.CopyTo(objArray1, 0);
            return objArray1;
        }

        public virtual void CopyTo(Array array, int index)
        {
            this.myObjects.CopyTo(array, index);
        }

        public void CopyTo(DiagramShape[] array, int index)
        {
            //modified by little
            //this.CopyTo(array, index);
            this.myObjects.CopyTo(array, index);
        }

        internal static int fastRemove(ArrayList a, object o)
        {
            int num1 = -1;
            int num2 = a.Count;
            if (num2 > 1000)
            {
                num1 = a.IndexOf(o, num2 - 50, 50);
            }
            if (num1 < 0)
            {
                num1 = a.IndexOf(o);
            }
            if (num1 >= 0)
            {
                a.RemoveAt(num1);
            }
            return num1;
        }

        public virtual CollectionEnumerator GetEnumerator()
        {
            return new CollectionEnumerator(this.myObjects, true);
        }

        IEnumerable IDiagramShapeCollection.Backwards
        {
            get
            {
                return this.Backwards;
            }
        }

        public virtual void Remove(DiagramShape obj)
        {
            if (obj != null)
            {
                DiagramShapeCollection.fastRemove(this.myObjects, obj);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual CollectionEnumerator Backwards
        {
            get
            {
                return new CollectionEnumerator(this.myObjects, false);
            }
        }

        public virtual int Count
        {
            get
            {
                return this.myObjects.Count;
            }
        }

        public virtual DiagramShape First
        {
            get
            {
                if (this.IsEmpty)
                {
                    return null;
                }
                return (DiagramShape)this.myObjects[0];
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return (this.myObjects.Count == 0);
            }
        }

        public virtual bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public virtual DiagramShape Last
        {
            get
            {
                if (this.IsEmpty)
                {
                    return null;
                }
                return (DiagramShape)this.myObjects[this.Count - 1];
            }
        }

        public virtual object SyncRoot
        {
            get
            {
                return this;
            }
        }

        private ArrayList myObjects;
    }
}
