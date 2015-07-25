using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Collections;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ChangedEventArgs : EventArgs, IUndoableEdit
    {
        public ChangedEventArgs()
        {
        }

        public ChangedEventArgs(ChangedEventArgs e)
        {
            this.myIsBeforeChanging = e.IsBeforeChanging;
            this.myDocument = e.Document;
            this.myHint = e.Hint;
            this.mySubHint = e.SubHint;
            this.myObject = e.Object;
            this.myOldInt = e.OldInt;
            this.myOldValue = e.OldValue;
            this.myOldRect = e.OldRect;
            this.myNewInt = e.NewInt;
            this.myNewValue = e.NewValue;
            this.myNewRect = e.NewRect;
            if (this.myDocument != null)
            {
                this.myDocument.CopyOldValueForUndo(this);
                this.myDocument.CopyNewValueForRedo(this);
            }
        }

        public bool CanRedo()
        {
            if (!this.IsBeforeChanging)
            {
                return (this.Document != null);
            }
            return false;
        }

        public bool CanUndo()
        {
            if (!this.IsBeforeChanging)
            {
                return (this.Document != null);
            }
            return false;
        }

        public void Clear()
        {
            this.myDocument = null;
            this.myObject = null;
            this.myOldValue = null;
            this.myNewValue = null;
        }

        public ChangedEventArgs FindBeforeChangingEdit()
        {
            if (!this.IsBeforeChanging)
            {
                DiagramDocument document1 = this.Document;
                if (document1 == null)
                {
                    return null;
                }
                DiagramUndoManager manager1 = document1.UndoManager;
                if (manager1 == null)
                {
                    return null;
                }
                UndoManagerCompoundEdit edit1 = manager1.CurrentEdit;
                if (edit1 == null)
                {
                    return null;
                }
                IList list1 = edit1.AllEdits;
                for (int num1 = list1.Count - 1; num1 >= 0; num1--)
                {
                    ChangedEventArgs args1 = list1[num1] as ChangedEventArgs;
                    if ((((args1 != null) && args1.IsBeforeChanging) && ((args1.Document == this.Document) && (args1.Hint == this.Hint))) && ((args1.SubHint == this.SubHint) && (args1.Object == this.Object)))
                    {
                        return args1;
                    }
                }
            }
            return null;
        }

        public float GetFloat(bool undo)
        {
            if (undo)
            {
                return this.OldRect.X;
            }
            return this.NewRect.X;
        }

        public int GetInt(bool undo)
        {
            if (undo)
            {
                return this.OldInt;
            }
            return this.NewInt;
        }

        public PointF GetPoint(bool undo)
        {
            if (undo)
            {
                return new PointF(this.OldRect.X, this.OldRect.Y);
            }
            return new PointF(this.NewRect.X, this.NewRect.Y);
        }

        public RectangleF GetRect(bool undo)
        {
            if (undo)
            {
                return this.OldRect;
            }
            return this.NewRect;
        }

        public SizeF GetSize(bool undo)
        {
            if (undo)
            {
                return new SizeF(this.OldRect.Width, this.OldRect.Height);
            }
            return new SizeF(this.NewRect.Width, this.NewRect.Height);
        }

        public object GetValue(bool undo)
        {
            if (undo)
            {
                return this.OldValue;
            }
            return this.NewValue;
        }

        public void Redo()
        {
            if (this.CanRedo())
            {
                this.Document.ChangeValue(this, false);
            }
        }

        public override string ToString()
        {
            string text2;
            string[] textArray1;
            string text1 = this.PresentationName + ": " + this.SubHint.ToString(NumberFormatInfo.InvariantInfo);
            if (this.Object != null)
            {
                text1 = text1 + " " + this.Object.ToString();
            }
            if (this.IsBeforeChanging)
            {
                text1 = text1 + " (before)";
            }
            if (this.OldInt != 0)
            {
                text1 = text1 + " " + this.OldInt.ToString(NumberFormatInfo.InvariantInfo);
            }
            if (this.OldValue != null)
            {
                text1 = text1 + " (" + this.OldValue.ToString() + ")";
            }
            RectangleF ef1 = new RectangleF();
            if (this.OldRect != ef1)
            {
                text2 = text1;
                textArray1 = new string[10];
                textArray1[0] = text2;
                textArray1[1] = " [";
                textArray1[2] = this.OldRect.X.ToString(NumberFormatInfo.InvariantInfo);
                textArray1[3] = ",";
                textArray1[4] = this.OldRect.Y.ToString(NumberFormatInfo.InvariantInfo);
                textArray1[5] = " ";
                textArray1[6] = this.OldRect.Width.ToString(NumberFormatInfo.InvariantInfo);
                textArray1[7] = "x";
                ef1 = this.OldRect;
                textArray1[8] = ef1.Height.ToString(NumberFormatInfo.InvariantInfo);
                textArray1[9] = "]";
                text1 = string.Concat(textArray1);
            }
            text1 = text1 + " --> ";
            if (this.NewInt != 0)
            {
                text1 = text1 + " " + this.NewInt.ToString(NumberFormatInfo.InvariantInfo);
            }
            if (this.NewValue != null)
            {
                text1 = text1 + " (" + this.NewValue.ToString() + ")";
            }
            if (this.NewRect != new RectangleF())
            {
                text2 = text1;
                textArray1 = new string[10] { text2, " [", this.NewRect.X.ToString(NumberFormatInfo.InvariantInfo), ",", this.NewRect.Y.ToString(NumberFormatInfo.InvariantInfo), " ", this.NewRect.Width.ToString(NumberFormatInfo.InvariantInfo), "x", this.NewRect.Height.ToString(NumberFormatInfo.InvariantInfo), "]" };
                text1 = string.Concat(textArray1);
            }
            return text1;
        }

        public void Undo()
        {
            if (this.CanUndo())
            {
                this.Document.ChangeValue(this, true);
            }
        }


        public DiagramDocument Document
        {
            get
            {
                return this.myDocument;
            }
            set
            {
                this.myDocument = value;
            }
        }

        public Dot.Utility.Media.Diagram.Shapes.DiagramShape DiagramShape
        {
            get
            {
                return (this.myObject as Dot.Utility.Media.Diagram.Shapes.DiagramShape);
            }
        }

        public int Hint
        {
            get
            {
                return this.myHint;
            }
            set
            {
                this.myHint = value;
            }
        }

        public bool IsBeforeChanging
        {
            get
            {
                return this.myIsBeforeChanging;
            }
            set
            {
                this.myIsBeforeChanging = value;
            }
        }

        public int NewInt
        {
            get
            {
                return this.myNewInt;
            }
            set
            {
                this.myNewInt = value;
            }
        }

        public RectangleF NewRect
        {
            get
            {
                return this.myNewRect;
            }
            set
            {
                this.myNewRect = value;
            }
        }

        public object NewValue
        {
            get
            {
                return this.myNewValue;
            }
            set
            {
                this.myNewValue = value;
            }
        }

        public object Object
        {
            get
            {
                return this.myObject;
            }
            set
            {
                this.myObject = value;
            }
        }

        public int OldInt
        {
            get
            {
                return this.myOldInt;
            }
            set
            {
                this.myOldInt = value;
            }
        }

        public RectangleF OldRect
        {
            get
            {
                return this.myOldRect;
            }
            set
            {
                this.myOldRect = value;
            }
        }

        public object OldValue
        {
            get
            {
                return this.myOldValue;
            }
            set
            {
                this.myOldValue = value;
            }
        }

        public string PresentationName
        {
            get
            {
                return this.myHint.ToString(CultureInfo.CurrentCulture);
            }
        }

        public int SubHint
        {
            get
            {
                return this.mySubHint;
            }
            set
            {
                this.mySubHint = value;
            }
        }


        private DiagramDocument myDocument;
        private int myHint;
        private bool myIsBeforeChanging;
        private int myNewInt;
        private RectangleF myNewRect;
        private object myNewValue;
        private object myObject;
        private int myOldInt;
        private RectangleF myOldRect;
        private object myOldValue;
        private int mySubHint;
    }
}
