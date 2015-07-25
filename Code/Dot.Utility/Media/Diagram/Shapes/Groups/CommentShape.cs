using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class CommentShape : GroupShape, ITextNode, IIdentifiablePart
    {
        public CommentShape()
        {
            this.myLabel = null;
            this.myBack = null;
            this.myTopLeftMargin = new SizeF(4f, 2f);
            this.myBottomRightMargin = new SizeF(4f, 2f);
            this.myPartID = -1;
            base.InternalFlags &= -17;
            this.myBack = this.CreateBackground();
            this.Add(this.myBack);
            this.myLabel = this.CreateLabel();
            this.Add(this.myLabel);
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x8fd:
                    {
                        base.Initializing = true;
                        this.TopLeftMargin = e.GetSize(undo);
                        base.Initializing = false;
                        return;
                    }
                case 0x8fe:
                    {
                        base.Initializing = true;
                        this.BottomRightMargin = e.GetSize(undo);
                        base.Initializing = false;
                        return;
                    }
                case 0x8ff:
                    {
                        this.PartID = e.GetInt(undo);
                        return;
                    }
                case 0x900:
                    {
                        this.Background = (DiagramShape)e.GetValue(undo);
                        return;
                    }
                case 0x901:
                    {
                        this.Label = (DiagramText)e.GetValue(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        protected override void CopyChildren(GroupShape newgroup, CopyDictionary env)
        {
            base.CopyChildren(newgroup, env);
            CommentShape comment1 = (CommentShape)newgroup;
            comment1.myPartID = -1;
            comment1.myBack = (DiagramShape)env[this.myBack];
            comment1.myLabel = (DiagramText)env[this.myLabel];
        }

        protected virtual DiagramShape CreateBackground()
        {
            RectangleGraph rectangle1 = new RectangleGraph();
            rectangle1.Shadowed = true;
            rectangle1.Selectable = false;
            rectangle1.Pen = DiagramGraph.Pens_LightGray;
            rectangle1.Brush = DiagramGraph.Brushes_LemonChiffon;
            return rectangle1;
        }

        protected virtual DiagramText CreateLabel()
        {
            DiagramText text1 = new DiagramText();
            text1.Selectable = false;
            text1.Multiline = true;
            text1.Editable = true;
            this.Editable = true;
            return text1;
        }

        public override void DoBeginEdit(DiagramView view)
        {
            if (this.Label != null)
            {
                this.Label.DoBeginEdit(view);
            }
        }

        public override void LayoutChildren(DiagramShape childchanged)
        {
            if (!base.Initializing)
            {
                DiagramText text1 = this.Label;
                if (text1 != null)
                {
                    DiagramShape obj1 = this.Background;
                    if (obj1 != null)
                    {
                        SizeF ef1 = this.TopLeftMargin;
                        SizeF ef2 = this.BottomRightMargin;
                        obj1.Bounds = new RectangleF(text1.Left - ef1.Width, text1.Top - ef1.Height, (text1.Width + ef1.Width) + ef2.Width, (text1.Height + ef1.Height) + ef2.Height);
                    }
                }
            }
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
        }


        public DiagramShape Background
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
                        if (value != null)
                        {
                            value.Selectable = obj1.Selectable;
                            value.Shadowed = obj1.Shadowed;
                        }
                        this.Remove(obj1);
                    }
                    this.myBack = value;
                    if (value != null)
                    {
                        this.InsertBefore(null, value);
                    }
                    this.Changed(0x900, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [TypeConverter(typeof(SizeFConverter)), Description("The margin around the text inside the background at the right side and the bottom"), Category("Appearance")]
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
                    this.Changed(0x8fe, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
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
                        this.Add(value);
                    }
                    this.Changed(0x901, 0, text1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Ownership"), Description("The unique ID of this part in its document.")]
        public int PartID
        {
            get
            {
                return this.myPartID;
            }
            set
            {
                int num1 = this.myPartID;
                if (num1 != value)
                {
                    this.myPartID = value;
                    this.Changed(0x8ff, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        public override bool Shadowed
        {
            get
            {
                if (this.Background != null)
                {
                    return this.Background.Shadowed;
                }
                return base.Shadowed;
            }
            set
            {
                if (this.Background != null)
                {
                    this.Background.Shadowed = value;
                }
                else
                {
                    base.Shadowed = value;
                }
            }
        }

        public virtual string Text
        {
            get
            {
                return this.Label.Text;
            }
            set
            {
                this.Label.Text = value;
            }
        }

        [TypeConverter(typeof(SizeFConverter)), Description("The margin around the text inside the background at the left side and the top"), Category("Appearance")]
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
                    this.Changed(0x8fd, 0, null, DiagramShape.MakeRect(ef1), 0, null, DiagramShape.MakeRect(value));
                    this.LayoutChildren(null);
                }
            }
        }


        public const int ChangedBackground = 0x900;
        public const int ChangedBottomRightMargin = 0x8fe;
        public const int ChangedLabel = 0x901;
        public const int ChangedPartID = 0x8ff;
        public const int ChangedTopLeftMargin = 0x8fd;
        private DiagramShape myBack;
        private SizeF myBottomRightMargin;
        private DiagramText myLabel;
        private int myPartID;
        private SizeF myTopLeftMargin;
    }
}
