using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class UndoManagerCompoundEdit : IUndoableEdit
    {
        public UndoManagerCompoundEdit()
        {
            this.myEdits = new ArrayList();
            this.myIsComplete = false;
            this.myName = "";
        }

        public virtual void AddEdit(IUndoableEdit edit)
        {
            if (!this.IsComplete)
            {
                this.myEdits.Add(edit);
            }
        }

        public virtual bool CanRedo()
        {
            if (this.IsComplete)
            {
                return (this.myEdits.Count > 0);
            }
            return false;
        }

        public virtual bool CanUndo()
        {
            if (this.IsComplete)
            {
                return (this.myEdits.Count > 0);
            }
            return false;
        }

        public virtual void Clear()
        {
            for (int num1 = this.myEdits.Count - 1; num1 >= 0; num1--)
            {
                IUndoableEdit edit1 = (IUndoableEdit)this.myEdits[num1];
                edit1.Clear();
            }
            this.myEdits.Clear();
        }

        public virtual void Redo()
        {
            if (this.CanRedo())
            {
                for (int num1 = 0; num1 <= (this.myEdits.Count - 1); num1++)
                {
                    IUndoableEdit edit1 = (IUndoableEdit)this.myEdits[num1];
                    edit1.Redo();
                }
            }
        }

        public virtual void Undo()
        {
            if (this.CanUndo())
            {
                for (int num1 = this.myEdits.Count - 1; num1 >= 0; num1--)
                {
                    IUndoableEdit edit1 = (IUndoableEdit)this.myEdits[num1];
                    edit1.Undo();
                }
            }
        }


        public virtual IList AllEdits
        {
            get
            {
                return this.myEdits;
            }
        }

        public virtual bool IsComplete
        {
            get
            {
                return this.myIsComplete;
            }
            set
            {
                if (value)
                {
                    this.myIsComplete = true;
                }
            }
        }

        public virtual string PresentationName
        {
            get
            {
                return this.myName;
            }
            set
            {
                this.myName = value;
            }
        }


        private ArrayList myEdits;
        private bool myIsComplete;
        private string myName;
    }
}
