using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace Dot.Utility.Media.Diagram
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LayerCollectionObjectEnumerator : IEnumerator, IEnumerable
    {
        private ArrayList myArray;
        private bool myForward;
        private int myIndex;
        private bool myEnumeratorValid;
        private LayerEnumerator myEnumerator;
        internal LayerCollectionObjectEnumerator(ArrayList a, bool forward)
        {
            this.myArray = a;
            this.myForward = forward;
            this.myIndex = -1;
            this.myEnumerator = new LayerEnumerator(a, true);
            this.myEnumeratorValid = false;
            this.Reset();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            LayerCollectionObjectEnumerator enumerator1 = this;
            enumerator1.Reset();
            return enumerator1;
        }
        public LayerCollectionObjectEnumerator GetEnumerator()
        {
            LayerCollectionObjectEnumerator enumerator1 = this;
            enumerator1.Reset();
            return enumerator1;
        }
        object IEnumerator.Current
        {
            get
            {
                return this.GetCurrent();
            }
        }
        public Shapes.DiagramShape Current
        {
            get
            {
                return this.GetCurrent();
            }
        }
        private Shapes.DiagramShape GetCurrent()
        {
            if (!this.myEnumeratorValid)
            {
                throw new InvalidOperationException("GoLayerCollectionObjectEnumerator is not at a valid position for the ArrayList of Layers");
            }
            return this.myEnumerator.Current;
        }
        public bool MoveNext()
        {
            if (this.myEnumeratorValid)
            {
                if (this.myEnumerator.MoveNext())
                {
                    return true;
                }
                this.myEnumeratorValid = false;
            }
            if (this.myForward)
            {
                while ((this.myIndex + 1) < this.myArray.Count)
                {
                    this.myIndex++;
                    DiagramLayer layer1 = (DiagramLayer)this.myArray[this.myIndex];
                    this.myEnumerator = layer1.GetEnumerator();
                    this.myEnumeratorValid = true;
                    if (this.myEnumerator.MoveNext())
                    {
                        return true;
                    }
                }
                return false;
            }
            while ((this.myIndex - 1) >= 0)
            {
                this.myIndex--;
                DiagramLayer layer2 = (DiagramLayer)this.myArray[this.myIndex];
                this.myEnumerator = layer2.Backwards;
                this.myEnumeratorValid = true;
                if (this.myEnumerator.MoveNext())
                {
                    return true;
                }
            }
            return false;
        }
        public void Reset()
        {
            if (this.myForward)
            {
                this.myIndex = -1;
            }
            else
            {
                this.myIndex = this.myArray.Count;
            }
            this.myEnumeratorValid = false;
        }
    }
}
