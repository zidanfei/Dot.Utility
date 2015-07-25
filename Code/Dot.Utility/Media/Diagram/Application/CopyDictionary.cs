using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class CopyDictionary : Hashtable
    {
        private CopyDelayedsCollection _Delayeds;
        public virtual CopyDelayedsCollection Delayeds
        {
            get
            {
                return this._Delayeds;
            }
        }

        private DiagramDocument _DestinationDocument;
        public virtual DiagramDocument DestinationDocument
        {
            get
            {
                return this._DestinationDocument;
            }
            set
            {
                this._DestinationDocument = value;
            }
        }

        private Shapes.IDiagramShapeCollection _SourceCollection;
        public virtual Shapes.IDiagramShapeCollection SourceCollection
        {
            get
            {
                return this._SourceCollection;
            }
            set
            {
                this._SourceCollection = value;
            }
        }

        public CopyDictionary()
        {
            this._SourceCollection = null;
            this._DestinationDocument = null;
            this._Delayeds = new CopyDelayedsCollection();
        }

        public virtual Shapes.DiagramShape Copy(Shapes.DiagramShape obj)
        {
            if (obj == null)
            {
                return null;
            }
            Shapes.DiagramShape obj1 = this[obj] as Shapes.DiagramShape;
            if (obj1 == null)
            {
                obj1 = obj.CopyObject(this);
            }
            return obj1;
        }

        public override object this[object key]
        {
            get
            {
                if (key == null)
                {
                    return null;
                }
                return base[key];
            }
            set
            {
                if (key != null)
                {
                    base[key] = value;
                }
            }
        }
    }
}
