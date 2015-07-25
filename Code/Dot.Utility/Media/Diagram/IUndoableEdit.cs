using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility.Media.Diagram
{
    public interface IUndoableEdit
    {
        bool CanRedo();

        bool CanUndo();

        void Clear();

        void Redo();

        void Undo();


        string PresentationName { get; }

    }
}
