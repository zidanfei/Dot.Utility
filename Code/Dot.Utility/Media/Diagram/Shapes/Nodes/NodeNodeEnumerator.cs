using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NodeNodeEnumerator : IEnumerator, IEnumerable
    {
        private DiagramNode myNode;
        private DiagramNode.Search mySearch;
        private ArrayList myArray;
        private int myIndex;
        internal NodeNodeEnumerator(DiagramNode n, DiagramNode.Search s)
        {
            this.myNode = n;
            this.mySearch = s;
            this.myArray = null;
            this.myIndex = -1;
            this.Reset();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
        public NodeNodeEnumerator GetEnumerator()
        {
            return this;
        }
        object IEnumerator.Current
        {
            get
            {
                return this.GetCurrent();
            }
        }
        public IDiagramNode Current
        {
            get
            {
                return this.GetCurrent();
            }
        }
        private IDiagramNode GetCurrent()
        {
            if ((this.myIndex < 0) || (this.myIndex >= this.myArray.Count))
            {
                throw new InvalidOperationException("GoNode.GoNodeNodeEnumerator is not at a valid position for the ArrayList");
            }
            return (IDiagramNode)this.myArray[this.myIndex];
        }
        public bool MoveNext()
        {
            if ((this.myIndex + 1) < this.myArray.Count)
            {
                this.myIndex++;
                return true;
            }
            this.myNode.myParts = this.myArray;
            return false;
        }
        public void Reset()
        {
            this.myArray = this.myNode.findAll(this.mySearch);
            this.myIndex = -1;
        }
    }
}
