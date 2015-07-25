using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class DiagramUndoManager
    {
        public DiagramUndoManager()
        {
            this.myCompEdits = new ArrayList();
            this.myMaximumEditCount = -1;
            this.myCurrentEditIndex = -1;
            this.myIncompleteEdit = null;
            this.myLevel = 0;
            this.myIsUndoing = false;
            this.myIsRedoing = false;
            this.myDocuments = new ArrayList();
            this.myChecksTransactionLevel = false;
            this.myResourceManager = null;
        }

        public bool AbortTransaction()
        {
            return this.EndTransaction(false, null);
        }

        public virtual void AddDocument(DiagramDocument doc)
        {
            if (!this.myDocuments.Contains(doc))
            {
                this.myDocuments.Add(doc);
            }
        }

        public virtual bool CanRedo()
        {
            if (this.TransactionLevel <= 0)
            {
                if (this.IsUndoing)
                {
                    return false;
                }
                if (this.IsRedoing)
                {
                    return false;
                }
                IUndoableEdit edit1 = this.EditToRedo;
                if (edit1 != null)
                {
                    return edit1.CanRedo();
                }
            }
            return false;
        }

        public virtual bool CanUndo()
        {
            if (this.TransactionLevel <= 0)
            {
                if (this.IsUndoing)
                {
                    return false;
                }
                if (this.IsRedoing)
                {
                    return false;
                }
                IUndoableEdit edit1 = this.EditToUndo;
                if (edit1 != null)
                {
                    return edit1.CanUndo();
                }
            }
            return false;
        }

        public virtual void Clear()
        {
            for (int num1 = this.myCompEdits.Count - 1; num1 >= 0; num1--)
            {
                IUndoableEdit edit1 = (IUndoableEdit)this.myCompEdits[num1];
                edit1.Clear();
            }
            this.myCompEdits.Clear();
            this.myCurrentEditIndex = -1;
            this.myIncompleteEdit = null;
            this.myLevel = 0;
            this.myIsUndoing = false;
            this.myIsRedoing = false;
        }

        public virtual void DocumentChanged(object sender, ChangedEventArgs e)
        {
            if ((!this.IsUndoing && !this.IsRedoing) && !this.SkipEvent(e))
            {
                UndoManagerCompoundEdit edit1 = this.CurrentEdit;
                if (edit1 == null)
                {
                    edit1 = new UndoManagerCompoundEdit();
                    this.CurrentEdit = edit1;
                }
                ChangedEventArgs args1 = new ChangedEventArgs(e);
                edit1.AddEdit(args1);
                if (this.ChecksTransactionLevel && (this.TransactionLevel <= 0))
                {
                    Shapes.DiagramShape.Trace("Change not within a transaction: " + args1.ToString());
                }
            }
        }

        public virtual bool EndTransaction(bool commit, string pname)
        {
            if (this.myLevel > 0)
            {
                this.myLevel--;
            }
            UndoManagerCompoundEdit edit1 = this.CurrentEdit;
            if ((this.myLevel == 0) && (edit1 != null))
            {
                if (commit)
                {
                    edit1.IsComplete = true;
                    if (edit1.AllEdits.Count > 0)
                    {
                        if (pname != null)
                        {
                            edit1.PresentationName = pname;
                        }
                        for (int num1 = this.myCompEdits.Count - 1; num1 > this.myCurrentEditIndex; num1--)
                        {
                            IUndoableEdit edit2 = (IUndoableEdit)this.myCompEdits[num1];
                            edit2.Clear();
                            this.myCompEdits.RemoveAt(num1);
                        }
                        if ((this.MaximumEditCount > 0) && (this.myCompEdits.Count >= this.MaximumEditCount))
                        {
                            IUndoableEdit edit3 = (IUndoableEdit)this.myCompEdits[0];
                            edit3.Clear();
                            this.myCompEdits.RemoveAt(0);
                            this.myCurrentEditIndex--;
                        }
                        this.myCompEdits.Add(edit1);
                        this.myCurrentEditIndex++;
                    }
                    this.CurrentEdit = null;
                    return true;
                }
                edit1.Clear();
                this.CurrentEdit = null;
            }
            return false;
        }

        public bool FinishTransaction(string tname)
        {
            return this.EndTransaction(true, this.GetPresentationName(tname));
        }

        public virtual string GetPresentationName(string tname)
        {
            if (tname == null)
            {
                return "";
            }
            string text1 = null;
            if (this.ResourceManager != null)
            {
                text1 = this.ResourceManager.GetString(tname, CultureInfo.CurrentCulture);
            }
            if (text1 == null)
            {
                text1 = tname;
            }
            return text1;
        }

        public virtual void Redo()
        {
            if (this.CanRedo())
            {
                try
                {
                    this.myIsRedoing = true;
                    IUndoableEdit edit1 = this.EditToRedo;
                    this.myCurrentEditIndex++;
                    edit1.Redo();
                    foreach (DiagramDocument document1 in this.Documents)
                    {
                        document1.InvalidateViews();
                    }
                }
                catch (Exception exception1)
                {
                    Shapes.DiagramShape.Trace("Redo: " + exception1.ToString());
                    throw exception1;
                }
                finally
                {
                    this.myIsRedoing = false;
                }
            }
        }

        public virtual void RemoveDocument(DiagramDocument doc)
        {
            this.myDocuments.Remove(doc);
        }

        public virtual bool SkipEvent(ChangedEventArgs evt)
        {
            if ((((evt.Document != null) && !evt.Document.SkipsUndoManager) && ((evt.Hint < 0) || (evt.Hint >= 200))) && ((evt.Hint != 0x385) || (((evt.DiagramShape != null) && !evt.DiagramShape.SkipsUndoManager) && (evt.SubHint != 1000))))
            {
                return false;
            }
            return true;
        }

        public virtual bool StartTransaction()
        {
            this.myLevel++;
            return (this.myLevel == 1);
        }

        public virtual void Undo()
        {
            if (this.CanUndo())
            {
                try
                {
                    this.myIsUndoing = true;
                    IUndoableEdit edit1 = this.EditToUndo;
                    this.myCurrentEditIndex--;
                    edit1.Undo();
                    foreach (DiagramDocument document1 in this.Documents)
                    {
                        document1.InvalidateViews();
                    }
                }
                catch (Exception exception1)
                {
                    Shapes.DiagramShape.Trace("Undo: " + exception1.ToString());
                    throw exception1;
                }
                finally
                {
                    this.myIsUndoing = false;
                }
            }
        }


        public virtual IList AllEdits
        {
            get
            {
                return this.myCompEdits;
            }
        }

        public bool ChecksTransactionLevel
        {
            get
            {
                return this.myChecksTransactionLevel;
            }
            set
            {
                this.myChecksTransactionLevel = value;
            }
        }

        public virtual UndoManagerCompoundEdit CurrentEdit
        {
            get
            {
                return this.myIncompleteEdit;
            }
            set
            {
                this.myIncompleteEdit = value;
            }
        }

        public virtual IEnumerable Documents
        {
            get
            {
                return this.myDocuments;
            }
        }

        public virtual IUndoableEdit EditToRedo
        {
            get
            {
                if (this.myCurrentEditIndex < (this.myCompEdits.Count - 1))
                {
                    return (IUndoableEdit)this.myCompEdits[this.myCurrentEditIndex + 1];
                }
                return null;
            }
        }

        public virtual IUndoableEdit EditToUndo
        {
            get
            {
                if ((this.myCurrentEditIndex >= 0) && (this.myCurrentEditIndex <= (this.myCompEdits.Count - 1)))
                {
                    return (IUndoableEdit)this.myCompEdits[this.myCurrentEditIndex];
                }
                return null;
            }
        }

        public virtual bool IsRedoing
        {
            get
            {
                return this.myIsRedoing;
            }
        }

        public virtual bool IsUndoing
        {
            get
            {
                return this.myIsUndoing;
            }
        }

        public virtual int MaximumEditCount
        {
            get
            {
                return this.myMaximumEditCount;
            }
            set
            {
                if (value == 0)
                {
                    value = 1;
                }
                this.myMaximumEditCount = value;
            }
        }

        public virtual string RedoPresentationName
        {
            get
            {
                IUndoableEdit edit1 = this.EditToRedo;
                if (edit1 != null)
                {
                    return edit1.PresentationName;
                }
                return "";
            }
        }

        public virtual System.Resources.ResourceManager ResourceManager
        {
            get
            {
                return this.myResourceManager;
            }
            set
            {
                this.myResourceManager = value;
            }
        }

        public virtual int TransactionLevel
        {
            get
            {
                return this.myLevel;
            }
        }

        public virtual int UndoEditIndex
        {
            get
            {
                return this.myCurrentEditIndex;
            }
        }

        public virtual string UndoPresentationName
        {
            get
            {
                IUndoableEdit edit1 = this.EditToUndo;
                if (edit1 != null)
                {
                    return edit1.PresentationName;
                }
                return "";
            }
        }


        public const string CollapsedSubGraphName = "Collapsed SubGraph";
        public const string CopyName = "Copy";
        public const string CopySelectionName = "Copy Selection";
        public const string CutName = "Cut";
        public const string DeleteSelectionName = "Delete Selection";
        public const string DropName = "Drop";
        public const string ExpandedAllSubGraphsName = "Expanded All SubGraphs";
        public const string ExpandedSubGraphName = "Expanded SubGraph";
        public const string MoveSelectionName = "Move Selection";
        private bool myChecksTransactionLevel;
        private ArrayList myCompEdits;
        private int myCurrentEditIndex;
        private ArrayList myDocuments;
        private UndoManagerCompoundEdit myIncompleteEdit;
        private bool myIsRedoing;
        private bool myIsUndoing;
        private int myLevel;
        private int myMaximumEditCount;
        [NonSerialized]
        private System.Resources.ResourceManager myResourceManager;
        public const string NewLinkName = "New Link";
        public const string PasteName = "Paste";
        public const string RelinkName = "Relink";
        public const string ResizeName = "Resize";
        public const string TextEditName = "Text Edit";
    }
}
