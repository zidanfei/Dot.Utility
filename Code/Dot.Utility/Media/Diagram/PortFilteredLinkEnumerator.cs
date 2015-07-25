using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace Dot.Utility.Media.Diagram
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PortFilteredLinkEnumerator : IEnumerator, IEnumerable
    {
        private Shapes.IDiagramPort myPort;
        private ArrayList myArray;
        private int myIndex;
        private bool myDest;
        internal PortFilteredLinkEnumerator(Shapes.IDiagramPort p, ArrayList a, bool dest)
        {
            this.myPort = p;
            this.myArray = a;
            this.myIndex = -1;
            this.myDest = dest;
            this.Reset();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            PortFilteredLinkEnumerator enumerator1 = this;
            enumerator1.Reset();
            return enumerator1;
        }
        public PortFilteredLinkEnumerator GetEnumerator()
        {
            PortFilteredLinkEnumerator enumerator1 = this;
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
        public Shapes.IDiagramLine Current
        {
            get
            {
                return this.GetCurrent();
            }
        }
        private Shapes.IDiagramLine GetCurrent()
        {
            if ((this.myIndex < 0) || (this.myIndex >= this.myArray.Count))
            {
                throw new InvalidOperationException("GoPort.GoPortFilteredLinkEnumerator is not at a valid position for the ArrayList");
            }
            return (Shapes.IDiagramLine)this.myArray[this.myIndex];
        }
        public bool MoveNext()
        {
            if ((this.myIndex + 1) >= this.myArray.Count)
            {
                return false;
            }
            this.myIndex++;
            if (this.myDest)
            {
                Shapes.IDiagramLine link1 = (Shapes.IDiagramLine)this.myArray[this.myIndex];
                if (link1.FromPort != this.myPort)
                {
                    return this.MoveNext();
                }
            }
            else
            {
                Shapes.IDiagramLine link2 = (Shapes.IDiagramLine)this.myArray[this.myIndex];
                if (link2.ToPort != this.myPort)
                {
                    return this.MoveNext();
                }
            }
            return true;
        }
        public void Reset()
        {
            this.myIndex = -1;
        }
    }
}
