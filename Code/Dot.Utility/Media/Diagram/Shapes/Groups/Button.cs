using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class Button : GroupShape, ITextNode, IActionObject
    {
        [field: NonSerialized]
        public event InputEventHandler ActionEvent;
        public virtual void OnAction(DiagramView view, InputEventArgs e)
        {
            if (this.ActionEvent != null)
            {
                this.ActionEvent(this, e);
            }
        }

        public Button()
        {
            this.myBack = null;
            this.myIcon = null;
            this.myLabel = null;
            this.myTopLeftMargin = new SizeF(3f, 2f);
            this.myBottomRightMargin = new SizeF(2f, 3f);
            this.ActionEvent = null;
            this.myActionActivated = false;
            base.InternalFlags &= -17;
            base.InternalFlags |= 0x1000000;
            this.myBack = this.CreateBackground();
            this.Add(this.myBack);
            this.myIcon = this.CreateIcon();
            this.Add(this.myIcon);
            this.myLabel = this.CreateLabel();
            this.Add(this.myLabel);
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0xb55:
                    {
                        this.Background = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0xb56:
                    {
                        this.Icon = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0xb57:
                    {
                        this.Label = (DiagramText)e.GetValue(undo);
                        return;
                    }
                case 0xb58:
                    {
                        base.Initializing = true;
                        this.TopLeftMargin = e.GetSize(undo);
                        base.Initializing = false;
                        return;
                    }
                case 0xb59:
                    {
                        base.Initializing = true;
                        this.BottomRightMargin = e.GetSize(undo);
                        base.Initializing = false;
                        return;
                    }
                case 0xb5a:
                    {
                        this.ActionEnabled = (bool)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            base.CopyChildren(newgroup, env);
            Button button1 = (Button)newgroup;
            button1.myBack = (DiagramShape)env[this.myBack];
            button1.myIcon = (DiagramShape)env[this.myIcon];
            button1.myLabel = (DiagramText)env[this.myLabel];
            button1.ActionEvent = null;
            button1.myActionActivated = false;
        }

        protected virtual DiagramShape CreateBackground()
        {
            RectangleGraph rectangle1 = new RectangleGraph();
            rectangle1.Selectable = false;
            rectangle1.Pen = null;
            rectangle1.Brush = DiagramGraph.SystemBrushes_Control;
            return rectangle1;
        }

        protected virtual DiagramShape CreateIcon()
        {
            return null;
        }

        protected virtual DiagramText CreateLabel()
        {
            DiagramText text1 = new DiagramText();
            text1.Selectable = false;
            return text1;
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            DiagramShape.InflateRect(ref rect, 2f, 2f);
            return rect;
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing)
            {
                DiagramShape obj1 = this.Background;
                DiagramText text1 = this.Label;
                DiagramShape obj2 = this.Icon;
                if ((obj2 != null) && (text1 != null))
                {
                    obj2.SetSpotLocation(0x40, text1, 0x100);
                }
                if (obj1 != null)
                {
                    RectangleF ef1 = this.Bounds;
                    if (text1 != null)
                    {
                        ef1 = text1.Bounds;
                    }
                    else if (obj2 != null)
                    {
                        ef1 = obj2.Bounds;
                    }
                    if ((obj2 != null) && (text1 != null))
                    {
                        ef1.X -= obj2.Width;
                        ef1.Width += obj2.Width;
                        if (obj2.Height > text1.Height)
                        {
                            ef1.Y -= ((obj2.Height - text1.Height) / 2f);
                            ef1.Height = obj2.Height;
                        }
                    }
                    SizeF ef2 = this.TopLeftMargin;
                    SizeF ef3 = this.BottomRightMargin;
                    ef1.X -= ef2.Width;
                    ef1.Width += (ef2.Width + ef3.Width);
                    ef1.Y -= ef2.Height;
                    ef1.Height += (ef2.Height + ef3.Height);
                    obj1.Bounds = ef1;
                }
            }
        }

        public virtual void OnActionAdjusted(DiagramView view, InputEventArgs e)
        {
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            base.Paint(g, view);
            this.PaintButton(g, view);
        }

        protected virtual void PaintButton(Graphics g, DiagramView view)
        {
            Pen pen1;
            Pen pen2;
            Pen pen3;
            Pen pen4;
            RectangleF ef1 = this.Bounds;
            if (this.ActionActivated)
            {
                pen1 = DiagramGraph.SystemPens_ControlDarkDark;
                pen2 = DiagramGraph.SystemPens_ControlLightLight;
                pen3 = DiagramGraph.SystemPens_ControlDark;
                pen4 = DiagramGraph.SystemPens_Control;
            }
            else
            {
                pen1 = DiagramGraph.SystemPens_ControlLightLight;
                pen2 = DiagramGraph.SystemPens_ControlDarkDark;
                pen3 = DiagramGraph.SystemPens_Control;
                pen4 = DiagramGraph.SystemPens_ControlDark;
            }
            PointF[] tfArray1 = view.AllocTempPointArray(3);
            float single1 = 0.5f;
            float single2 = 0.5f;
            if (base.Document != null)
            {
                single1 /= base.Document.WorldScale.Width;
                single2 /= base.Document.WorldScale.Height;
            }
            PointF tf1 = new PointF(ef1.X + single1, ef1.Y + single2);
            PointF tf2 = new PointF((ef1.X + ef1.Width) - single1, ef1.Y + single2);
            PointF tf3 = new PointF(ef1.X + single1, (ef1.Y + ef1.Height) - single2);
            PointF tf4 = new PointF((ef1.X + ef1.Width) - single1, (ef1.Y + ef1.Height) - single2);
            tfArray1[0] = tf2;
            tfArray1[1] = tf1;
            tfArray1[2] = tf3;
            DiagramGraph.DrawLines(g, view, pen3, tfArray1);
            tfArray1[0].Y -= single2;
            tfArray1[1] = tf4;
            tfArray1[2].X -= single1;
            DiagramGraph.DrawLines(g, view, pen4, tfArray1);
            tf1.X -= (single1 * 2f);
            tf1.Y -= (single2 * 2f);
            tf2.X += (single1 * 2f);
            tf2.Y -= (single2 * 2f);
            tf4.X += (single1 * 2f);
            tf4.Y += (single2 * 2f);
            tf3.X -= (single1 * 2f);
            tf3.Y += (single2 * 2f);
            tfArray1[0] = tf2;
            tfArray1[1] = tf1;
            tfArray1[2] = tf3;
            DiagramGraph.DrawLines(g, view, pen1, tfArray1);
            tfArray1[0].Y -= single2;
            tfArray1[1] = tf4;
            tfArray1[2].X -= single1;
            DiagramGraph.DrawLines(g, view, pen2, tfArray1);
            view.FreeTempPointArray(tfArray1);
        }

        public override void Remove(DiagramShape obj)
        {
            base.Remove(obj);
            if (obj == this.myLabel)
            {
                this.myLabel = null;
            }
            else if (obj == this.myBack)
            {
                this.myBack = null;
            }
            else if (obj == this.myIcon)
            {
                this.myIcon = null;
            }
        }


        [Category("Appearance"), DefaultValue(false), Description("Whether the button appears depressed")]
        public virtual bool ActionActivated
        {
            get
            {
                return this.myActionActivated;
            }
            set
            {
                if (this.myActionActivated != value)
                {
                    this.myActionActivated = value;
                    base.InvalidateViews();
                }
            }
        }

        [Category("Behavior"), Description("Whether the user can click on this button to perform an action"), DefaultValue(true)]
        public virtual bool ActionEnabled
        {
            get
            {
                return ((base.InternalFlags & 0x1000000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x1000000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x1000000;
                    }
                    else
                    {
                        base.InternalFlags &= -16777217;
                    }
                    this.Changed(0xb5a, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public virtual DiagramShape Background
        {
            get
            {
                return this.myBack;
            }
            set
            {
                DiagramShape obj1 = this.myBack;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        this.Remove(obj1);
                    }
                    this.myBack = value;
                    if (value != null)
                    {
                        value.Selectable = false;
                        this.InsertBefore(null, value);
                    }
                    this.Changed(0xb55, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The margin around the icon and label inside the background at the right side and the bottom"), Category("Appearance"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF BottomRightMargin
        {
            get
            {
                return this.myBottomRightMargin;
            }
            set
            {
                SizeF ef1 = this.myBottomRightMargin;
                if (ef1 != value)
                {
                    this.myBottomRightMargin = value;
                    this.Changed(0xb59, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }

        public virtual DiagramShape Icon
        {
            get
            {
                return this.myIcon;
            }
            set
            {
                DiagramShape obj1 = this.myIcon;
                if (obj1 != value)
                {
                    if (obj1 != null)
                    {
                        this.Remove(obj1);
                    }
                    this.myIcon = value;
                    if (value != null)
                    {
                        value.Selectable = false;
                        this.Add(value);
                    }
                    this.Changed(0xb56, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public virtual DiagramText Label
        {
            get
            {
                return this.myLabel;
            }
            set
            {
                DiagramText text1 = this.myLabel;
                if (text1 != value)
                {
                    if (text1 != null)
                    {
                        this.Remove(text1);
                    }
                    this.myLabel = value;
                    if (value != null)
                    {
                        value.Selectable = false;
                        this.Add(value);
                    }
                    this.Changed(0xb57, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), Description("The text string for the button"), DefaultValue("")]
        public virtual string Text
        {
            get
            {
                if (this.Label != null)
                {
                    return this.Label.Text;
                }
                return "";
            }
            set
            {
                if (this.Label != null)
                {
                    this.Label.Text = value;
                }
            }
        }

        [Category("Appearance"), Description("The margin around the icon and label inside the background at the left side and the top"), TypeConverter(typeof(SizeFConverter))]
        public virtual SizeF TopLeftMargin
        {
            get
            {
                return this.myTopLeftMargin;
            }
            set
            {
                SizeF ef1 = this.myTopLeftMargin;
                if (ef1 != value)
                {
                    this.myTopLeftMargin = value;
                    this.Changed(0xb58, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }


        public const int ChangedActionEnabled = 0xb5a;
        public const int ChangedBackground = 0xb55;
        public const int ChangedBottomRightMargin = 0xb59;
        public const int ChangedIcon = 0xb56;
        public const int ChangedLabel = 0xb57;
        public const int ChangedTopLeftMargin = 0xb58;
        private const int flagActionEnabled = 0x1000000;
        [NonSerialized]
        private bool myActionActivated;
        private DiagramShape myBack;
        private SizeF myBottomRightMargin;
        private DiagramShape myIcon;
        private DiagramText myLabel;
        private SizeF myTopLeftMargin;
    }
}
