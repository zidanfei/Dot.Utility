using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GroupEnumerator : IEnumerator, IEnumerable
    {
        private ArrayList myArray;
        private bool myForward;
        private int myIndex;
        internal GroupEnumerator(ArrayList a, bool forward)
        {
            this.myArray = a;
            this.myForward = forward;
            this.myIndex = -1;
            this.Reset();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            GroupEnumerator enumerator1 = this;
            enumerator1.Reset();
            return enumerator1;
        }
        public GroupEnumerator GetEnumerator()
        {
            GroupEnumerator enumerator1 = this;
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
        public DiagramShape Current
        {
            get
            {
                return this.GetCurrent();
            }
        }
        private DiagramShape GetCurrent()
        {
            if ((this.myIndex < 0) || (this.myIndex >= this.myArray.Count))
            {
                throw new InvalidOperationException("GoGroup.GoGroupEnumerator is not at a valid position for the ArrayList");
            }
            return (DiagramShape)this.myArray[this.myIndex];
        }
        public bool MoveNext()
        {
            if (this.myForward)
            {
                if ((this.myIndex + 1) < this.myArray.Count)
                {
                    this.myIndex++;
                    return true;
                }
                return false;
            }
            if ((this.myIndex - 1) >= 0)
            {
                this.myIndex--;
                return true;
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
        }
    }
}
