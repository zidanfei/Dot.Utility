using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class CopyDelayedsCollection : ICollection, IEnumerable
    {
        private Hashtable ObjectTable;

        public CopyDelayedsCollection()
        {
            this.ObjectTable = new Hashtable();
        }

        public virtual void Add(object obj)
        {
            if (obj != null)
            {
                this.ObjectTable[obj] = obj;
            }
        }

        public virtual void Clear()
        {
            this.ObjectTable.Clear();
        }

        public virtual bool Contains(object obj)
        {
            if (obj != null)
            {
                return (this.ObjectTable[obj] == obj);
            }
            return false;
        }

        public virtual object[] CopyArray()
        {
            object[] objArray1 = new object[this.Count];
            this.CopyTo(objArray1, 0);
            return objArray1;
        }

        public virtual void CopyTo(Array array, int index)
        {
            IDictionaryEnumerator enumerator1 = this.ObjectTable.GetEnumerator();
            int num1 = index;
            while (enumerator1.MoveNext())
            {
                array.SetValue(enumerator1.Key, num1++);
            }
        }

        public void CopyTo(Shapes.DiagramShape[] array, int index)
        {
            this.CopyTo(array, index);
        }

        public virtual IEnumerator GetEnumerator()
        {
            object[] objArray1 = this.CopyArray();
            return objArray1.GetEnumerator();
        }

        public virtual void Remove(object obj)
        {
            if (obj != null)
            {
                this.ObjectTable.Remove(obj);
            }
        }


        public virtual int Count
        {
            get
            {
                return this.ObjectTable.Count;
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return (this.Count == 0);
            }
        }

        public virtual bool IsSynchronized
        {
            get
            {
                return this.ObjectTable.IsSynchronized;
            }
        }

        public virtual object SyncRoot
        {
            get
            {
                return this.ObjectTable;
            }
        }
    }
}
