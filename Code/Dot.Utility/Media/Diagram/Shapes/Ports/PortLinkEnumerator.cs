using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PortLinkEnumerator : IEnumerator, IEnumerable
    {
        private ArrayList myArray;
        private int myIndex;
        internal PortLinkEnumerator(ArrayList a)
        {
            this.myArray = a;
            this.myIndex = -1;
            this.Reset();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            PortLinkEnumerator enumerator1 = this;
            enumerator1.Reset();
            return enumerator1;
        }
        public PortLinkEnumerator GetEnumerator()
        {
            PortLinkEnumerator enumerator1 = this;
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
        public IDiagramLine Current
        {
            get
            {
                return this.GetCurrent();
            }
        }
        private IDiagramLine GetCurrent()
        {
            if ((this.myIndex < 0) || (this.myIndex >= this.myArray.Count))
            {
                throw new InvalidOperationException("GoPort.GoPortLinkEnumerator is not at a valid position for the ArrayList");
            }
            return (IDiagramLine)this.myArray[this.myIndex];
        }
        public bool MoveNext()
        {
            if ((this.myIndex + 1) < this.myArray.Count)
            {
                this.myIndex++;
                return true;
            }
            return false;
        }
        public void Reset()
        {
            this.myIndex = -1;
        }
    }
}
