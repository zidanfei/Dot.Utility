using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class InputEventArgs : EventArgs
    {
        public InputEventArgs()
        {
            this.myButtons = MouseButtons.None;
            this.myModifiers = Keys.None;
            this.myKey = Keys.None;
            this.myMouseEventArgs = null;
            this.myDragEventArgs = null;
            this.myKeyEventArgs = null;
            this.myDoubleClick = false;
            this.myDelta = 0;
        }

        public InputEventArgs(InputEventArgs evt)
        {
            this.myButtons = MouseButtons.None;
            this.myModifiers = Keys.None;
            this.myKey = Keys.None;
            this.myMouseEventArgs = null;
            this.myDragEventArgs = null;
            this.myKeyEventArgs = null;
            this.myDoubleClick = false;
            this.myDelta = 0;
            this.ViewPoint = evt.ViewPoint;
            this.DocPoint = evt.DocPoint;
            this.Buttons = evt.Buttons;
            this.Modifiers = evt.Modifiers;
            this.Key = evt.Key;
            this.MouseEventArgs = evt.MouseEventArgs;
            this.DragEventArgs = evt.DragEventArgs;
            this.KeyEventArgs = evt.KeyEventArgs;
            this.DoubleClick = evt.DoubleClick;
            this.Delta = evt.Delta;
        }


        public virtual bool Alt
        {
            get
            {
                return ((this.Modifiers & Keys.Alt) == Keys.Alt);
            }
        }

        public MouseButtons Buttons
        {
            get
            {
                return this.myButtons;
            }
            set
            {
                this.myButtons = value;
            }
        }

        public virtual bool Control
        {
            get
            {
                return ((this.Modifiers & Keys.Control) == Keys.Control);
            }
        }

        public int Delta
        {
            get
            {
                return this.myDelta;
            }
            set
            {
                this.myDelta = value;
            }
        }

        public PointF DocPoint
        {
            get
            {
                return this.myDocPoint;
            }
            set
            {
                this.myDocPoint = value;
            }
        }

        public bool DoubleClick
        {
            get
            {
                return this.myDoubleClick;
            }
            set
            {
                this.myDoubleClick = value;
            }
        }

        public System.Windows.Forms.DragEventArgs DragEventArgs
        {
            get
            {
                return this.myDragEventArgs;
            }
            set
            {
                this.myDragEventArgs = value;
            }
        }

        public virtual bool IsContextButton
        {
            get
            {
                return (this.Buttons == MouseButtons.Right);
            }
        }

        public Keys Key
        {
            get
            {
                return this.myKey;
            }
            set
            {
                this.myKey = value;
            }
        }

        public System.Windows.Forms.KeyEventArgs KeyEventArgs
        {
            get
            {
                return this.myKeyEventArgs;
            }
            set
            {
                this.myKeyEventArgs = value;
            }
        }

        public Keys Modifiers
        {
            get
            {
                return this.myModifiers;
            }
            set
            {
                this.myModifiers = value;
            }
        }

        public System.Windows.Forms.MouseEventArgs MouseEventArgs
        {
            get
            {
                return this.myMouseEventArgs;
            }
            set
            {
                this.myMouseEventArgs = value;
            }
        }

        public virtual bool Shift
        {
            get
            {
                return ((this.Modifiers & Keys.Shift) == Keys.Shift);
            }
        }

        public Point ViewPoint
        {
            get
            {
                return this.myViewPoint;
            }
            set
            {
                this.myViewPoint = value;
            }
        }


        private MouseButtons myButtons;
        private int myDelta;
        private PointF myDocPoint;
        private bool myDoubleClick;
        private System.Windows.Forms.DragEventArgs myDragEventArgs;
        private Keys myKey;
        private System.Windows.Forms.KeyEventArgs myKeyEventArgs;
        private Keys myModifiers;
        private System.Windows.Forms.MouseEventArgs myMouseEventArgs;
        private Point myViewPoint;
    }
}
