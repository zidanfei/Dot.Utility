using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ToolManager : DiagramTool
    {
        public ToolManager(DiagramView v)
            : base(v)
        {
            this.myStarted = false;
        }

        public override void DoKeyDown()
        {
            InputEventArgs args1 = base.LastInput;
            bool flag1 = args1.Control;
            Keys keys1 = args1.Key;
            if (keys1 == Keys.Delete)
            {
                base.View.EditDelete();
            }
            else if (flag1 && (keys1 == Keys.A))
            {
                base.View.SelectAll();
            }
            else if (flag1 && (keys1 == Keys.C))
            {
                base.View.EditCopy();
            }
            else if (flag1 && (keys1 == Keys.X))
            {
                base.View.EditCut();
            }
            else if (flag1 && (keys1 == Keys.V))
            {
                base.View.EditPaste();
            }
            else if (keys1 == Keys.F2)
            {
                base.View.EditEdit();
            }
            else if (keys1 == Keys.Next)
            {
                if (args1.Shift)
                {
                    base.View.ScrollPage(1f, 0f);
                }
                else
                {
                    base.View.ScrollPage(0f, 1f);
                }
            }
            else if (keys1 == Keys.Prior)
            {
                if (args1.Shift)
                {
                    base.View.ScrollPage(-1f, 0f);
                }
                else
                {
                    base.View.ScrollPage(0f, -1f);
                }
            }
            else if (keys1 == Keys.Home)
            {
                RectangleF ef1 = base.View.ComputeDocumentBounds();
                if (flag1)
                {
                    base.View.DocPosition = new PointF(ef1.X, ef1.Y);
                }
                else
                {
                    base.View.DocPosition = new PointF(ef1.X, base.View.DocPosition.Y);
                }
            }
            else if (keys1 == Keys.End)
            {
                PointF tf1;
                RectangleF ef2 = base.View.ComputeDocumentBounds();
                SizeF ef3 = base.View.DocExtentSize;
                if (flag1)
                {
                    tf1 = new PointF((ef2.X + ef2.Width) - ef3.Width, (ef2.Y + ef2.Height) - ef3.Height);
                }
                else
                {
                    tf1 = new PointF((ef2.X + ef2.Width) - ef3.Width, base.View.DocPosition.Y);
                }
                base.View.DocPosition = new PointF(System.Math.Max((float)0f, tf1.X), System.Math.Max((float)0f, tf1.Y));
            }
            else if (flag1 && (keys1 == Keys.Z))
            {
                base.View.Undo();
            }
            else if (flag1 && (keys1 == Keys.Y))
            {
                base.View.Redo();
            }
            else if (keys1 == Keys.Escape)
            {
                if (base.View.CanSelectObjects())
                {
                    base.Selection.Clear();
                }
                base.DoKeyDown();
            }
            else
            {
                bool flag2 = false;
                if ((!flag1 && !args1.Alt) && base.View.SelectsByFirstChar)
                {
                    TypeConverter converter1 = TypeDescriptor.GetConverter(typeof(Keys));
                    string text1 = converter1.ConvertToString(null, CultureInfo.CurrentCulture, args1.Key);
                    char ch1 = '\0';
                    if (text1.Length == 1)
                    {
                        ch1 = text1[0];
                    }
                    else if ((text1.Length == 2) && (text1[0] == 'D'))
                    {
                        ch1 = text1[1];
                    }
                    if (char.IsLetterOrDigit(ch1))
                    {
                        flag2 = base.View.SelectNextNode(ch1);
                    }
                }
                if (!flag2)
                {
                    base.DoKeyDown();
                }
            }
        }

        public override void DoMouseDown()
        {
            foreach (IDiagramTool tool1 in base.View.MouseDownTools)
            {
                if ((tool1 != null) && tool1.CanStart())
                {
                    base.View.Tool = tool1;
                    return;
                }
            }
            this.Started = true;
        }

        public override void DoMouseHover()
        {
            base.View.DoHover(base.LastInput);
        }

        public override void DoMouseMove()
        {
            if (this.Started)
            {
                foreach (IDiagramTool tool1 in base.View.MouseMoveTools)
                {
                    if ((tool1 != null) && tool1.CanStart())
                    {
                        base.View.Tool = tool1;
                        return;
                    }
                }
            }
            base.View.DoMouseOver(base.LastInput);
        }

        public override void DoMouseUp()
        {
            if (this.Started)
            {
                foreach (IDiagramTool tool1 in base.View.MouseUpTools)
                {
                    if ((tool1 != null) && tool1.CanStart())
                    {
                        base.View.Tool = tool1;
                        return;
                    }
                }
            }
        }

        public override void DoMouseWheel()
        {
            base.View.DoWheel(base.LastInput);
        }

        public override void Stop()
        {
            this.Started = false;
        }


        public bool Started
        {
            get
            {
                return this.myStarted;
            }
            set
            {
                this.myStarted = value;
            }
        }


        [NonSerialized]
        private bool myStarted;
    }
}
