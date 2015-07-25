using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Text;
using System.ComponentModel;
using System.Globalization;
using System.Security;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class DiagramText : DiagramShape
    {
        static DiagramText()
        {
            char[] chArray1 = new char[2] { '\r', '\n' };
            DiagramText.myNewlineArray = chArray1;
            DiagramText.myDefaultFontName = "Microsoft Sans Serif";
            DiagramText.myDefaultFontSize = 10f;
            DiagramText.myLastFont = null;
            DiagramText.myEmptyChoices = ArrayList.FixedSize(new ArrayList());
            DiagramText.myEmptyBitmap = new Bitmap(10, 10);
        }

        public DiagramText()
        {
            this.myString = "";
            this.myFamilyName = DiagramText.myDefaultFontName;
            this.myFontSize = DiagramText.myDefaultFontSize;
            this.myAlignment = 2;
            this.myTextColor = Color.Black;
            this.myBackgroundColor = Color.White;
            this.myInternalTextFlags = 0x20010101;
            this.myWrappingWidth = 150f;
            this.myMinimum = 0;
            this.myMaximum = 100;
            this.myChoices = DiagramText.myEmptyChoices;
            this.myStringFormat = null;
            this.myFont = null;
            this.myEditor = null;
            this.myNumLines = 1;
            base.InternalFlags &= -273;
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            sel.RemoveHandles(this);
            if (this.BackgroundOpaqueWhenSelected)
            {
                bool flag1 = base.SkipsUndoManager;
                base.SkipsUndoManager = true;
                this.TransparentBackground = false;
                base.SkipsUndoManager = flag1;
            }
            else
            {
                base.AddSelectionHandles(sel, selectedObj);
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 0x5dd:
                    {
                        this.Text = (string)e.GetValue(undo);
                        return;
                    }
                case 0x5de:
                    {
                        this.FamilyName = (string)e.GetValue(undo);
                        return;
                    }
                case 0x5df:
                    {
                        this.FontSize = e.GetFloat(undo);
                        return;
                    }
                case 0x5e0:
                    {
                        this.Alignment = e.GetInt(undo);
                        return;
                    }
                case 0x5e1:
                    {
                        this.TextColor = (Color)e.GetValue(undo);
                        return;
                    }
                case 0x5e2:
                    {
                        this.BackgroundColor = (Color)e.GetValue(undo);
                        return;
                    }
                case 0x5e3:
                    {
                        this.TransparentBackground = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5e4:
                    {
                        this.Bold = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5e5:
                    {
                        this.Italic = (bool)e.GetValue(undo);
                        return;
                    }
                case 1510:
                    {
                        this.Underline = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5e7:
                    {
                        this.StrikeThrough = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5e8:
                    {
                        this.Multiline = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5eb:
                    {
                        this.BackgroundOpaqueWhenSelected = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5ec:
                    {
                        this.Clipping = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5ee:
                    {
                        this.AutoResizes = (bool)e.GetValue(undo);
                        return;
                    }
                case 1520:
                    {
                        this.Wrapping = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5f1:
                    {
                        this.WrappingWidth = e.GetFloat(undo);
                        return;
                    }
                case 0x5f2:
                    {
                        this.GdiCharSet = e.GetInt(undo);
                        return;
                    }
                case 0x5f3:
                    {
                        this.EditorStyle = (TextEditorStyle)e.GetInt(undo);
                        return;
                    }
                case 0x5f4:
                    {
                        this.Minimum = e.GetInt(undo);
                        return;
                    }
                case 0x5f5:
                    {
                        this.Maximum = e.GetInt(undo);
                        return;
                    }
                case 0x5f6:
                    {
                        this.DropDownList = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5f7:
                    {
                        this.Choices = (ArrayList)e.GetValue(undo);
                        return;
                    }
                case 0x5f8:
                    {
                        this.RightToLeft = (bool)e.GetValue(undo);
                        return;
                    }
                case 1530:
                    {
                        this.Bordered = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x5fb:
                    {
                        this.StringTrimming = (System.Drawing.StringTrimming)e.GetInt(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        public virtual string ComputeEdit(string oldtext, string newtext)
        {
            return newtext;
        }

        private float computeHeight(Graphics g, System.Drawing.Font font, float maxw)
        {
            string text1 = this.Text;
            float single1 = this.getLineHeight(font);
            if (text1.Length == 0)
            {
                this.myNumLines = 1;
                return single1;
            }
            if (!this.Multiline)
            {
                int num1 = this.FindFirstLineBreak(text1, 0);
                if (num1 >= 0)
                {
                    text1 = text1.Substring(0, num1);
                }
            }
            StringFormat format1 = this.getStringFormat(null);
            float single2 = 0f;
            this.myNumLines = 0;
            int num2 = 0;
            int num3 = -1;
            int num4 = 0;
            bool flag1 = false;
            while (!flag1)
            {
                num3 = this.FindFirstLineBreak(text1, num2, ref num4);
                if (num3 == -1)
                {
                    num3 = text1.Length;
                    flag1 = true;
                }
                if (num2 <= num3)
                {
                    string text2 = text1.Substring(num2, num3 - num2);
                    if (text2.Length > 0)
                    {
                        if (this.Wrapping)
                        {
                            SizeF ef1 = new SizeF(maxw, 1E+09f);
                            int num5 = 0;
                            SizeF ef2 = this.measureString(text2, g, font, format1, ef1, out num5);
                            single2 += ef2.Height;
                            this.myNumLines += num5;
                        }
                        else
                        {
                            single2 += single1;
                            this.myNumLines++;
                        }
                    }
                    else
                    {
                        single2 += single1;
                        this.myNumLines++;
                    }
                }
                num2 = num4;
            }
            return single2;
        }

        private float computeWidth(Graphics g, System.Drawing.Font font)
        {
            string text1 = this.Text;
            if (text1.Length == 0)
            {
                return 0f;
            }
            StringFormat format1 = StringFormat.GenericTypographic;
            if (this.Multiline)
            {
                float single1 = 0f;
                int num1 = 0;
                bool flag1 = false;
                int num2 = 0;
                while (!flag1)
                {
                    int num3 = this.FindFirstLineBreak(text1, num1, ref num2);
                    if (num3 == -1)
                    {
                        num3 = text1.Length;
                        flag1 = true;
                    }
                    string text2 = text1.Substring(num1, num3 - num1);
                    float single2 = this.getStringWidth(text2, g, font, format1);
                    if (this.Wrapping && (single2 > this.WrappingWidth))
                    {
                        return this.WrappingWidth;
                    }
                    if (single2 > single1)
                    {
                        single1 = single2;
                    }
                    num1 = num2;
                }
                return single1;
            }
            int num4 = this.FindFirstLineBreak(text1, 0);
            if (num4 >= 0)
            {
                text1 = text1.Substring(0, num4);
            }
            float single3 = this.getStringWidth(text1, g, font, format1);
            if (this.Wrapping && (single3 > this.WrappingWidth))
            {
                return this.WrappingWidth;
            }
            return single3;
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            DiagramText text1 = (DiagramText)base.CopyObject(env);
            if (text1 != null)
            {
                text1.myEditor = null;
            }
            return text1;
        }

        public override DiagramControl CreateEditor(DiagramView view)
        {
            int num1;
            DiagramControl control1 = new DiagramControl();
            float single1 = 1f;
            float single2 = 1f;
            if (view != null)
            {
                single1 = view.WorldScale.Width;
                single2 = view.WorldScale.Height;
            }
            if (this.EditorStyle == TextEditorStyle.NumericUpDown)
            {
                control1.ControlType = typeof(NumericUpDownControl);
                RectangleF ef1 = this.Bounds;
                ef1.X -= (2f / single1);
                ef1.Y -= (2f / single2);
                ef1.Width += (36f / single1);
                ef1.Height += (8f / single2);
                control1.Bounds = ef1;
                return control1;
            }
            if (this.EditorStyle == TextEditorStyle.ComboBox)
            {
                control1.ControlType = typeof(ComboBoxControl);
                RectangleF ef2 = this.Bounds;
                ef2.X -= (2f / single1);
                ef2.Y -= (2f / single2);
                ef2.Width += (4f / single1);
                ef2.Height += (4f / single2);
                if (view != null)
                {
                    StringFormat format1 = this.getStringFormat(view);
                    float single3 = ef2.Width * view.DocScale;
                    Graphics graphics1 = view.CreateGraphics();
                    System.Drawing.Font font1 = this.Font;
                    float single4 = font1.Size;
                    if (view != null)
                    {
                        single4 *= (view.DocScale / single2);
                    }
                    System.Drawing.Font font2 = this.makeFont(font1.Name, single4, font1.Style);
                    if (graphics1 != null)
                    {
                        foreach (string text1 in this.Choices)
                        {
                            single3 = System.Math.Max(single3, this.getStringWidth(text1, graphics1, font2, format1));
                        }
                        graphics1.Dispose();
                    }
                    font2.Dispose();
                    single3 += (30f / single1);
                    ef2.Width = single3 / view.DocScale;
                }
                control1.Bounds = ef2;
                return control1;
            }
            control1.ControlType = typeof(TextBoxControl);
            RectangleF ef3 = this.Bounds;
            ef3.X -= (2f / single1);
            ef3.Y -= (2f / single2);
            ef3.Width += (4f / single1);
            ef3.Height += (4f / single2);
            if (this.Multiline || this.Wrapping)
            {
                ef3.Height += (this.getLineHeight(this.Font) * 2f);
            }
            if (!this.Wrapping)
            {
                num1 = this.Alignment;
                if (num1 <= 0x10)
                {
                    switch (num1)
                    {
                        case 1:
                            {
                                goto Label_0456;
                            }
                        case 2:
                        case 3:
                        case 0x10:
                            {
                                goto Label_0436;
                            }
                        case 4:
                        case 8:
                            {
                                goto Label_046D;
                            }
                    }
                }
                else if (num1 <= 0x40)
                {
                    if (num1 == 0x20)
                    {
                        goto Label_0456;
                    }
                    if (num1 == 0x40)
                    {
                        goto Label_046D;
                    }
                }
                else if (num1 == 0x80)
                {
                    goto Label_0456;
                }
                goto Label_0436;
            }
            num1 = this.Alignment;
            if (num1 <= 0x10)
            {
                switch (num1)
                {
                    case 1:
                        {
                            goto Label_0355;
                        }
                    case 2:
                    case 3:
                    case 0x10:
                        {
                            goto Label_0322;
                        }
                    case 4:
                    case 8:
                        {
                            goto Label_0388;
                        }
                }
            }
            else if (num1 <= 0x40)
            {
                if (num1 == 0x20)
                {
                    goto Label_0355;
                }
                if (num1 == 0x40)
                {
                    goto Label_0388;
                }
            }
            else if (num1 == 0x80)
            {
                goto Label_0355;
            }
        Label_0322:
            if (this.isRightToLeft(view))
            {
                ef3.X = ((ef3.X + ef3.Width) - this.WrappingWidth) - (2f / single1);
            }
            goto Label_03B6;
        Label_0355:
            ef3.X = ((ef3.X + (ef3.Width / 2f)) - (this.WrappingWidth / 2f)) - (2f / single1);
            goto Label_03B6;
        Label_0388:
            if (!this.isRightToLeft(view))
            {
                ef3.X = ((ef3.X + ef3.Width) - this.WrappingWidth) - (2f / single1);
            }
        Label_03B6:
            ef3.Width = System.Math.Max((float)(this.WrappingWidth + (4f / single1)), ef3.Width);
            goto Label_04A0;
        Label_0436:
            if (this.isRightToLeft(view))
            {
                ef3.X -= (30f / single1);
            }
            goto Label_048B;
        Label_0456:
            ef3.X -= (15f / single1);
            goto Label_048B;
        Label_046D:
            if (!this.isRightToLeft(view))
            {
                ef3.X -= (30f / single1);
            }
        Label_048B:
            ef3.Width += (30f / single1);
        Label_04A0:
            control1.Bounds = ef3;
            return control1;
        }

        public override void DoBeginEdit(DiagramView view)
        {
            if ((view != null) && (this.Editor == null))
            {
                try
                {
                    view.StartTransaction();
                    this.RemoveSelectionHandles(view.Selection);
                    this.myEditor = this.CreateEditor(view);
                    this.Editor.EditedObject = this;
                    view.EditControl = this.Editor;
                    Control control1 = this.Editor.GetControl(view);
                    if (control1 != null)
                    {
                        control1.Focus();
                    }
                }
                catch (SecurityException exception1)
                {
                    DiagramShape.Trace("GoText DoBeginEdit: " + exception1.ToString());
                    view.EditControl = null;
                    this.myEditor = null;
                    view.AbortTransaction();
                }
            }
        }

        public virtual void DoEdit(DiagramView view, string oldtext, string newtext)
        {
            string text1 = this.ComputeEdit(oldtext, newtext);
            this.Text = text1;
        }

        public override void DoEndEdit(DiagramView view)
        {
            if (this.Editor != null)
            {
                this.Editor.EditedObject = null;
                if (view != null)
                {
                    view.EditControl = null;
                }
                this.myEditor = null;
                if (view != null)
                {
                    view.RaiseShapeEdited(this);
                    view.FinishTransaction("Text Edit");
                }
            }
        }

        private void drawString(string str, Graphics g, DiagramView view, System.Drawing.Font font, Brush br, RectangleF rect, StringFormat fmt)
        {
            g.DrawString(str, font, br, rect, fmt);
        }

        public override RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            if (this.Shadowed)
            {
                SizeF ef1 = this.GetShadowOffset(view);
                if (ef1.Width < 0f)
                {
                    rect.X += ef1.Width;
                    rect.Width -= ef1.Width;
                }
                else
                {
                    rect.Width += ef1.Width;
                }
                if (ef1.Height < 0f)
                {
                    rect.Y += ef1.Height;
                    rect.Height -= ef1.Height;
                }
                else
                {
                    rect.Height += ef1.Height;
                }
            }
            DiagramShape.InflateRect(ref rect, System.Math.Max((float)(rect.Height / 3f), (float)2f), 1f);
            return rect;
        }

        internal int FindFirstLineBreak(string str, int start)
        {
            int num1 = 0;
            return this.FindFirstLineBreak(str, start, ref num1);
        }

        private int FindFirstLineBreak(string str, int start, ref int nextline)
        {
            int num1 = str.IndexOfAny(DiagramText.myNewlineArray, start);
            if (num1 >= 0)
            {
                if (((str[num1] == '\r') && ((num1 + 1) < str.Length)) && (str[num1 + 1] == '\n'))
                {
                    nextline = num1 + 2;
                    return num1;
                }
                nextline = num1 + 1;
            }
            return num1;
        }

        private System.Drawing.Font findLargestFont(Graphics g, RectangleF rect)
        {
            System.Drawing.Font font1;
            string text1 = this.Font.Name;
            FontStyle style1 = this.Font.Style;
            float single1 = 10f;
            while (this.fitsInBox(g, font1 = this.makeFont(text1, single1, style1), rect))
            {
                font1.Dispose();
                single1 += 1f;
            }
            font1.Dispose();
            for (single1 -= 0.1f; !this.fitsInBox(g, font1 = this.makeFont(text1, single1, style1), rect) && (single1 > 1f); single1 -= 0.1f)
            {
                font1.Dispose();
            }
            return font1;
        }

        private bool fitsInBox(Graphics g, System.Drawing.Font font, RectangleF rect)
        {
            float single1 = this.computeWidth(g, font);
            if (rect.Width < single1)
            {
                return false;
            }
            float single2 = this.computeHeight(g, font, rect.Width);
            if (rect.Height < single2)
            {
                return false;
            }
            return true;
        }

        private float getLineHeight(System.Drawing.Font font)
        {
            return font.GetHeight();
        }

        private StringFormat getStringFormat(DiagramView view)
        {
            if (this.myStringFormat == null)
            {
                this.myStringFormat = new StringFormat(StringFormat.GenericTypographic);
                this.myStringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            }
            this.myStringFormat.Trimming = this.StringTrimming;
            if (this.StringTrimming == System.Drawing.StringTrimming.None)
            {
                this.myStringFormat.FormatFlags &= ((StringFormatFlags)(-8193));
            }
            else
            {
                this.myStringFormat.FormatFlags |= StringFormatFlags.LineLimit;
            }
            int num1 = this.Alignment;
            if (num1 <= 0x10)
            {
                switch (num1)
                {
                    case 1:
                        {
                            goto Label_00D6;
                        }
                    case 2:
                    case 3:
                    case 0x10:
                        {
                            goto Label_00C8;
                        }
                    case 4:
                    case 8:
                        {
                            goto Label_00E4;
                        }
                }
            }
            else if (num1 <= 0x40)
            {
                if (num1 == 0x20)
                {
                    goto Label_00D6;
                }
                if (num1 == 0x40)
                {
                    goto Label_00E4;
                }
            }
            else if (num1 == 0x80)
            {
                goto Label_00D6;
            }
        Label_00C8:
            this.myStringFormat.Alignment = StringAlignment.Near;
            goto Label_00F0;
        Label_00D6:
            this.myStringFormat.Alignment = StringAlignment.Center;
            goto Label_00F0;
        Label_00E4:
            this.myStringFormat.Alignment = StringAlignment.Far;
        Label_00F0:
            if (this.isRightToLeft(view))
            {
                this.myStringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }
            else
            {
                this.myStringFormat.FormatFlags &= ((StringFormatFlags)(-2));
            }
            if (this.Wrapping)
            {
                this.myStringFormat.FormatFlags &= ((StringFormatFlags)(-4097));
            }
            else
            {
                this.myStringFormat.FormatFlags |= StringFormatFlags.NoWrap;
            }
            return this.myStringFormat;
        }

        private float getStringWidth(string str, Graphics g, System.Drawing.Font font, StringFormat fmt)
        {
            SizeF ef1 = g.MeasureString(str, font, new PointF(), fmt);
            return ef1.Width;
        }

        internal bool isRightToLeft(DiagramView view)
        {
            if (this.RightToLeftFromView && (view != null))
            {
                return (view.RightToLeft == System.Windows.Forms.RightToLeft.Yes);
            }
            return this.RightToLeft;
        }

        private System.Drawing.Font makeFont(string name, float size, FontStyle style)
        {
            byte num1 = (byte)this.GdiCharSet;
            return new System.Drawing.Font(name, size, style, GraphicsUnit.Point, num1);
        }

        private SizeF measureString(string str, Graphics g, System.Drawing.Font font, StringFormat fmt, SizeF area, out int lines)
        {
            int num1 = 0;
            return g.MeasureString(str, font, area, fmt, out num1, out lines);
        }

        protected override void OnBoundsChanged(RectangleF old)
        {
            base.OnBoundsChanged(old);
            SizeF ef1 = base.Size;
            if ((old.Width != ef1.Width) || (old.Height != ef1.Height))
            {
                this.UpdateScale();
            }
        }

        protected override void OnLayerChanged(DiagramLayer oldlayer, DiagramLayer newlayer, DiagramShape mainObj)
        {
            base.OnLayerChanged(oldlayer, newlayer, mainObj);
            if (this.Editor != null)
            {
                DiagramView view1 = this.Editor.View;
                if (view1 != null)
                {
                    this.DoEndEdit(view1);
                }
            }
        }

        public override bool OnSingleClick(InputEventArgs evt, DiagramView view)
        {
            if (!this.CanEdit())
            {
                return false;
            }
            if (!view.CanEditObjects())
            {
                return false;
            }
            if (evt.Shift || evt.Control)
            {
                return false;
            }
            this.DoBeginEdit(view);
            return true;
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            if (!this.PaintGreek(g, view))
            {
                RectangleF ef1 = this.Bounds;
                if (!this.TransparentBackground)
                {
                    if (this.Shadowed)
                    {
                        SizeF ef2 = this.GetShadowOffset(view);
                        Brush brush1 = this.GetShadowBrush(view);
                        DiagramGraph.DrawRectangle(g, view, null, brush1, ef1.X + ef2.Width, ef1.Y + ef2.Height, ef1.Width, ef1.Height);
                    }
                    Color color1 = this.BackgroundColor;
                    Brush brush2 = (color1 == Color.White) ? DiagramGraph.Brushes_White : new SolidBrush(this.BackgroundColor);
                    DiagramGraph.DrawRectangle(g, view, null, brush2, ef1.X, ef1.Y, ef1.Width, ef1.Height);
                    if (color1 != Color.White)
                    {
                        brush2.Dispose();
                    }
                }
                string text1 = this.Text;
                float single1 = 1f;
                if (view != null)
                {
                    single1 /= view.WorldScale.Width;
                }
                if (this.Shadowed && this.TransparentBackground)
                {
                    RectangleF ef3 = ef1;
                    SizeF ef4 = this.GetShadowOffset(view);
                    ef3.X += ef4.Width;
                    ef3.Y += ef4.Height;
                    if (this.Bordered)
                    {
                        Pen pen1 = this.GetShadowPen(view, single1);
                        DiagramGraph.DrawRectangle(g, view, pen1, null, ef3.X - single1, ef3.Y, ef3.Width + (2f * single1), ef3.Height);
                    }
                    if (text1.Length > 0)
                    {
                        Brush brush3 = this.GetShadowBrush(view);
                        this.paintText(text1, g, view, ef3, brush3);
                    }
                }
                Color color2 = this.TextColor;
                if (this.Bordered)
                {
                    Pen pen2 = new Pen(color2, single1);
                    DiagramGraph.DrawRectangle(g, view, pen2, null, ef1.X - single1, ef1.Y, ef1.Width + (2f * single1), ef1.Height);
                    pen2.Dispose();
                }
                if (text1.Length > 0)
                {
                    Brush brush4 = (color2 == Color.Black) ? DiagramGraph.Brushes_Black : new SolidBrush(this.TextColor);
                    this.paintText(text1, g, view, ef1, brush4);
                    if (color2 != Color.Black)
                    {
                        brush4.Dispose();
                    }
                }
            }
        }

        public virtual bool PaintGreek(Graphics g, DiagramView view)
        {
            float single1 = view.DocScale;
            float single2 = view.PaintNothingScale;
            float single3 = view.PaintGreekScale;
            if (view.IsPrinting)
            {
                single2 /= 4f;
                single3 /= 4f;
            }
            float single4 = this.FontSize / 10f;
            single4 *= view.WorldScale.Height;
            single2 /= single4;
            single3 /= single4;
            if (single1 > single2)
            {
                if (single1 > single3)
                {
                    return false;
                }
                RectangleF ef1 = this.Bounds;
                Pen pen1 = new Pen(this.TextColor, 1f / view.WorldScale.Height);
                int num1 = this.LineCount;
                float single5 = ef1.Y;
                float single6 = ef1.Height / ((float)(num1 + 1));
                for (int num2 = 0; num2 < num1; num2++)
                {
                    single5 += single6;
                    DiagramGraph.DrawLine(g, view, pen1, ef1.X, single5, ef1.X + ef1.Width, single5);
                }
                pen1.Dispose();
            }
            return true;
        }

        private void paintText(string str, Graphics g, DiagramView view, RectangleF rect, Brush textbrush)
        {
            if (str.Length != 0)
            {
                System.Drawing.Font font1 = this.Font;
                if (font1 != null)
                {
                    System.Drawing.Font font2 = null;
                    float single1 = this.getLineHeight(font1);
                    bool flag1 = this.Clipping;
                    Region region1 = null;
                    Region region2 = null;
                    if (flag1)
                    {
                        region1 = g.Clip;
                        region2 = new Region(rect);
                        g.Clip = region2;
                    }
                    if (!this.Multiline)
                    {
                        int num1 = this.FindFirstLineBreak(str, 0);
                        if (num1 >= 0)
                        {
                            str = str.Substring(0, num1);
                        }
                    }
                    StringFormat format1 = this.getStringFormat(view);
                    if (view.IsPrinting)
                    {
                        font2 = this.findLargestFont(g, this.Bounds);
                        font1 = font2;
                    }
                    float single2 = 0f;
                    int num2 = 0;
                    int num3 = -1;
                    int num4 = -1;
                    bool flag2 = false;
                    while (!flag2)
                    {
                        num3 = this.FindFirstLineBreak(str, num2, ref num4);
                        if (num3 == -1)
                        {
                            num3 = str.Length;
                            flag2 = true;
                        }
                        if (num2 <= num3)
                        {
                            string text1 = str.Substring(num2, num3 - num2);
                            if (text1.Length > 0)
                            {
                                RectangleF ef1 = new RectangleF(rect.X, rect.Y + single2, rect.Width, (rect.Height - single2) + 0.01f);
                                this.drawString(text1, g, view, font1, textbrush, ef1, format1);
                                if (this.Wrapping)
                                {
                                    int num5 = 0;
                                    SizeF ef2 = this.measureString(text1, g, font1, format1, new SizeF(ef1.Width, ef1.Height), out num5);
                                    single2 += ef2.Height;
                                }
                                else
                                {
                                    single2 += single1;
                                }
                            }
                            else
                            {
                                single2 += single1;
                            }
                        }
                        num2 = num4;
                    }
                    if (font2 != null)
                    {
                        font2.Dispose();
                    }
                    if (flag1 && (region1 != null))
                    {
                        g.Clip = region1;
                    }
                    if (region2 != null)
                    {
                        region2.Dispose();
                    }
                }
            }
        }

        private void recalcBoundingRect()
        {
            lock (DiagramText.myEmptyBitmap)
            {
                Graphics graphics1 = Graphics.FromImage(DiagramText.myEmptyBitmap);
                graphics1.PageUnit = GraphicsUnit.Pixel;
                graphics1.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                float single1 = this.computeWidth(graphics1, this.Font);
                float single2 = 10f;
                DiagramDocument document1 = base.Document;
                if (document1 != null)
                {
                    single2 /= document1.WorldScale.Width;
                }
                if (single1 < single2)
                {
                    single1 = single2;
                }
                float single3 = this.computeHeight(graphics1, this.Font, single1);
                if ((single1 != base.Width) || (single3 != base.Height))
                {
                    this.SetSizeKeepingLocation(new SizeF(single1, single3));
                }
                graphics1.Dispose();
            }
        }

        public override void RemoveSelectionHandles(DiagramSelection sel)
        {
            if (this.BackgroundOpaqueWhenSelected)
            {
                bool flag1 = base.SkipsUndoManager;
                base.SkipsUndoManager = true;
                this.TransparentBackground = true;
                base.SkipsUndoManager = flag1;
            }
            base.RemoveSelectionHandles(sel);
        }

        private void rescaleFont()
        {
            lock (DiagramText.myEmptyBitmap)
            {
                Graphics graphics1 = Graphics.FromImage(DiagramText.myEmptyBitmap);
                graphics1.PageUnit = GraphicsUnit.Pixel;
                graphics1.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                System.Drawing.Font font1 = this.findLargestFont(graphics1, this.Bounds);
                this.FontSize = font1.Size;
                font1.Dispose();
                graphics1.Dispose();
            }
        }

        private void ResetFont()
        {
            if (this.myFont != null)
            {
                this.myFont = null;
            }
        }

        public override void SetSizeKeepingLocation(SizeF s)
        {
            RectangleF ef1 = this.Bounds;
            ef1.Width = s.Width;
            ef1.Height = s.Height;
            PointF tf1 = this.Location;
            RectangleF ef2 = this.SetRectangleSpotLocation(ef1, this.Alignment, tf1);
            this.Bounds = ef2;
        }

        private System.Drawing.Font shareFont(string name, float size, FontStyle style)
        {
            System.Drawing.Font font1;
            lock (typeof(DiagramText))
            {
                if (((DiagramText.myLastFont != null) && (DiagramText.myLastFont.Name == name)) && ((DiagramText.myLastFont.Size == size) && (DiagramText.myLastFont.Style == style)))
                {
                    return DiagramText.myLastFont;
                }
                DiagramText.myLastFont = this.makeFont(name, size, style);
                font1 = DiagramText.myLastFont;
            }
            return font1;
        }

        internal void UpdateScale()
        {
            if (!base.Initializing && (((this.InternalTextFlags & 0x40000000) == 0) && this.AutoRescales))
            {
                this.InternalTextFlags |= 0x40000000;
                this.rescaleFont();
                this.InternalTextFlags &= -1073741825;
            }
        }

        internal void UpdateSize()
        {
            if (!base.Initializing && (((this.InternalTextFlags & 0x40000000) == 0) && this.AutoResizes))
            {
                this.InternalTextFlags |= 0x40000000;
                this.recalcBoundingRect();
                this.InternalTextFlags &= -1073741825;
            }
        }

        private void UpdateSizeOrScale()
        {
            if (this.AutoResizes)
            {
                this.UpdateSize();
            }
            else
            {
                this.UpdateScale();
            }
        }


        [Description("The text alignment."), DefaultValue(2), Category("Appearance")]
        public virtual int Alignment
        {
            get
            {
                return this.myAlignment;
            }
            set
            {
                int num1 = this.myAlignment;
                if (num1 != value)
                {
                    this.myAlignment = value;
                    this.Changed(0x5e0, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether the bounds are recalculated when the text changes."), Category("Behavior"), DefaultValue(true)]
        public virtual bool AutoResizes
        {
            get
            {
                return ((this.myInternalTextFlags & 0x100) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 0x100) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 0x100;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -257;
                    }
                    this.Changed(0x5ee, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), Description("The background color for this text object.")]
        public virtual Color BackgroundColor
        {
            get
            {
                return this.myBackgroundColor;
            }
            set
            {
                Color color1 = this.myBackgroundColor;
                if (color1 != value)
                {
                    this.myBackgroundColor = value;
                    this.Changed(0x5e2, 0, color1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Whether the text background is displayed when selected, and transparent when not selected")]
        public virtual bool BackgroundOpaqueWhenSelected
        {
            get
            {
                return ((this.myInternalTextFlags & 0x200) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 0x200) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 0x200;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -513;
                    }
                    this.Changed(0x5eb, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether the font is bold."), Category("Appearance"), DefaultValue(false)]
        public virtual bool Bold
        {
            get
            {
                return ((this.myInternalTextFlags & 2) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 2) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 2;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -3;
                    }
                    this.ResetFont();
                    this.Changed(0x5e4, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.UpdateSizeOrScale();
                }
            }
        }

        [DefaultValue(false), Description("Whether a simple border using the TextColor is drawn around the text."), Category("Appearance")]
        public virtual bool Bordered
        {
            get
            {
                return ((base.InternalFlags & 0x100000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x100000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x100000;
                    }
                    else
                    {
                        base.InternalFlags &= -1048577;
                    }
                    this.Changed(1530, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The list of items presented in a drop-down list when editing"), Category("Behavior")]
        public ArrayList Choices
        {
            get
            {
                if (this.myChoices == null)
                {
                    return DiagramText.myEmptyChoices;
                }
                return this.myChoices;
            }
            set
            {
                ArrayList list1 = (this.myChoices != null) ? this.myChoices : DiagramText.myEmptyChoices;
                ArrayList list2 = value;
                if (list2 == null)
                {
                    list2 = DiagramText.myEmptyChoices;
                }
                if (list1 != list2)
                {
                    this.myChoices = list2;
                    this.Changed(0x5f7, 0, list1, DiagramShape.NullRect, 0, list2, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether the text drawing is clipped by the bounds."), DefaultValue(false), Category("Appearance")]
        public virtual bool Clipping
        {
            get
            {
                return ((this.myInternalTextFlags & 0x80) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 0x80) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 0x80;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -129;
                    }
                    this.Changed(0x5ec, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The initial font face name for newly constructed GoText objects.")]
        public static string DefaultFontFamilyName
        {
            get
            {
                return DiagramText.myDefaultFontName;
            }
            set
            {
                if (value != null)
                {
                    DiagramText.myDefaultFontName = value;
                }
            }
        }

        [Description("The initial font size for newly constructed GoText objects.")]
        public static float DefaultFontSize
        {
            get
            {
                return DiagramText.myDefaultFontSize;
            }
            set
            {
                if (value > 0f)
                {
                    DiagramText.myDefaultFontSize = value;
                }
            }
        }

        [DefaultValue(false), Description("Whether the user is limited to values that are in the predefined list of Items."), Category("Behavior")]
        public bool DropDownList
        {
            get
            {
                return ((this.myInternalTextFlags & 0x800) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 0x800) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 0x800;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -2049;
                    }
                    this.Changed(0x5f6, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public override DiagramControl Editor
        {
            get
            {
                return this.myEditor;
            }
        }

        [Description("The kind of Control used when editing"), Category("Behavior"), DefaultValue(0)]
        public TextEditorStyle EditorStyle
        {
            get
            {
                return (TextEditorStyle)((this.myInternalTextFlags & 61440) >> 12);
            }
            set
            {
                TextEditorStyle style1 = (TextEditorStyle)((this.myInternalTextFlags & 61440) >> 12);
                if (style1 != value)
                {
                    this.myInternalTextFlags = (this.myInternalTextFlags & -61441) | (((int)value) << 12);
                    this.Changed(0x5f3, (int)style1, null, DiagramShape.NullRect, (int)value, null, DiagramShape.NullRect);
                }
            }
        }

        [Description("The font family face name."), Category("Appearance")]
        public virtual string FamilyName
        {
            get
            {
                return this.myFamilyName;
            }
            set
            {
                string text1 = value;
                if (text1 == null)
                {
                    text1 = DiagramText.DefaultFontFamilyName;
                }
                string text2 = this.myFamilyName;
                if (text2 != text1)
                {
                    this.myFamilyName = text1;
                    this.ResetFont();
                    this.Changed(0x5de, 0, text2, DiagramShape.NullRect, 0, text1, DiagramShape.NullRect);
                    this.UpdateSizeOrScale();
                }
            }
        }

        [Browsable(false)]
        public System.Drawing.Font Font
        {
            get
            {
                if (this.myFont == null)
                {
                    FontStyle style1 = FontStyle.Regular;
                    if (this.Bold)
                    {
                        style1 |= FontStyle.Bold;
                    }
                    if (this.Italic)
                    {
                        style1 |= FontStyle.Italic;
                    }
                    if (this.Underline)
                    {
                        style1 |= FontStyle.Underline;
                    }
                    if (this.StrikeThrough)
                    {
                        style1 |= FontStyle.Strikeout;
                    }
                    this.myFont = this.shareFont(this.FamilyName, this.FontSize, style1);
                }
                return this.myFont;
            }
            set
            {
                if (value != null)
                {
                    base.Initializing = true;
                    this.FamilyName = value.Name;
                    this.FontSize = value.Size;
                    this.Bold = (value.Style & FontStyle.Bold) != FontStyle.Regular;
                    this.Italic = (value.Style & FontStyle.Italic) != FontStyle.Regular;
                    this.Underline = (value.Style & FontStyle.Underline) != FontStyle.Regular;
                    this.StrikeThrough = (value.Style & FontStyle.Strikeout) != FontStyle.Regular;
                    this.GdiCharSet = value.GdiCharSet;
                    this.myFont = value;
                    base.Initializing = false;
                    this.UpdateSizeOrScale();
                }
            }
        }

        [Category("Appearance"), Description("The text font size, in points")]
        public virtual float FontSize
        {
            get
            {
                return this.myFontSize;
            }
            set
            {
                float single1 = this.myFontSize;
                if ((value > 0f) && (single1 != value))
                {
                    this.myFontSize = value;
                    this.ResetFont();
                    this.Changed(0x5df, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                    this.UpdateSize();
                }
            }
        }

        [Category("Appearance"), DefaultValue((byte)1), Description("The GDI character set.")]
        public virtual int GdiCharSet
        {
            get
            {
                return ((this.myInternalTextFlags & 16711680) >> 0x10);
            }
            set
            {
                int num1 = (this.myInternalTextFlags & 16711680) >> 0x10;
                int num2 = value & 0xff;
                if (num1 != num2)
                {
                    this.myInternalTextFlags = (this.myInternalTextFlags & -16711681) | (num2 << 0x10);
                    this.ResetFont();
                    this.Changed(0x5f2, num1, null, DiagramShape.NullRect, num2, null, DiagramShape.NullRect);
                    this.UpdateSizeOrScale();
                }
            }
        }

        private int InternalTextFlags
        {
            get
            {
                return this.myInternalTextFlags;
            }
            set
            {
                this.myInternalTextFlags = value;
            }
        }

        [Description("Whether the font is italic."), Category("Appearance"), DefaultValue(false)]
        public virtual bool Italic
        {
            get
            {
                return ((this.myInternalTextFlags & 4) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 4) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 4;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -5;
                    }
                    this.ResetFont();
                    this.Changed(0x5e5, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.UpdateSizeOrScale();
                }
            }
        }

        [Category("Appearance"), Description("How many lines of text are being displayed")]
        public virtual int LineCount
        {
            get
            {
                return this.myNumLines;
            }
        }

        public override PointF Location
        {
            get
            {
                return this.GetSpotLocation(this.Alignment);
            }
            set
            {
                this.SetSpotLocation(this.Alignment, value);
            }
        }

        [DefaultValue(100), Description("The maximum value that the user can choose"), Category("Behavior")]
        public int Maximum
        {
            get
            {
                return this.myMaximum;
            }
            set
            {
                int num1 = this.myMaximum;
                if (num1 != value)
                {
                    this.myMaximum = value;
                    this.Changed(0x5f5, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("The minimum value that the user can choose"), DefaultValue(0)]
        public int Minimum
        {
            get
            {
                return this.myMinimum;
            }
            set
            {
                int num1 = this.myMinimum;
                if (num1 != value)
                {
                    this.myMinimum = value;
                    this.Changed(0x5f4, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether the text will be displayed as multiple lines of text."), Category("Appearance"), DefaultValue(false)]
        public virtual bool Multiline
        {
            get
            {
                return ((this.myInternalTextFlags & 0x20) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 0x20) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 0x20;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -33;
                    }
                    this.Changed(0x5e8, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.UpdateSizeOrScale();
                }
            }
        }

        [Description("Whether to draw text from right to left, when RightToLeftFromView is false"), DefaultValue(false), Category("Appearance")]
        public virtual bool RightToLeft
        {
            get
            {
                return ((this.myInternalTextFlags & 0x10000000) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 0x10000000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 0x10000000;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -268435457;
                    }
                    this.Changed(0x5f8, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(true), Description("Whether the view's RightToLeft property takes precedence over this text object's RightToLeft property"), Category("Appearance")]
        public virtual bool RightToLeftFromView
        {
            get
            {
                return ((this.myInternalTextFlags & 0x20000000) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 0x20000000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 0x20000000;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -536870913;
                    }
                    this.Changed(0x5f9, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(false), Description("Whether the font style includes a strike-through."), Category("Appearance")]
        public virtual bool StrikeThrough
        {
            get
            {
                return ((this.myInternalTextFlags & 0x10) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 0x10) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 0x10;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -17;
                    }
                    this.ResetFont();
                    this.Changed(0x5e7, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.UpdateSizeOrScale();
                }
            }
        }

        [Category("Appearance"), DefaultValue(0), Description("How to trim text that does not fit.")]
        public virtual System.Drawing.StringTrimming StringTrimming
        {
            get
            {
                return (System.Drawing.StringTrimming)((this.myInternalTextFlags & 251658240) >> 0x18);
            }
            set
            {
                int num1 = (this.myInternalTextFlags & 251658240) >> 0x18;
                int num2 = ((int)value) & 15;
                if (num1 != num2)
                {
                    this.myInternalTextFlags = (this.myInternalTextFlags & -251658241) | (num2 << 0x18);
                    this.ResetFont();
                    this.Changed(0x5fb, num1, null, DiagramShape.NullRect, num2, null, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), DefaultValue(""), Description("The string that this text object displays.")]
        public virtual string Text
        {
            get
            {
                return this.myString;
            }
            set
            {
                string text1 = value;
                if (text1 == null)
                {
                    text1 = "";
                }
                string text2 = this.myString;
                if (text2 != text1)
                {
                    this.myString = text1;
                    this.Changed(0x5dd, 0, text2, DiagramShape.NullRect, 0, text1, DiagramShape.NullRect);
                    this.UpdateSizeOrScale();
                }
            }
        }

        [Category("Appearance"), Description("The color of the text.")]
        public virtual Color TextColor
        {
            get
            {
                return this.myTextColor;
            }
            set
            {
                Color color1 = this.myTextColor;
                if (color1 != value)
                {
                    this.myTextColor = value;
                    this.Changed(0x5e1, 0, color1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether the text is painted alone, or if the background is painted first."), Category("Appearance"), DefaultValue(true)]
        public virtual bool TransparentBackground
        {
            get
            {
                return ((this.myInternalTextFlags & 1) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 1) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 1;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -2;
                    }
                    this.Changed(0x5e3, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether the font style includes an underline."), Category("Appearance"), DefaultValue(false)]
        public virtual bool Underline
        {
            get
            {
                return ((this.myInternalTextFlags & 8) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 8) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 8;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -9;
                    }
                    this.ResetFont();
                    this.Changed(1510, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.UpdateSizeOrScale();
                }
            }
        }

        [Description("Whether the text is wrapped."), Category("Appearance"), DefaultValue(false)]
        public virtual bool Wrapping
        {
            get
            {
                return ((this.myInternalTextFlags & 0x40) != 0);
            }
            set
            {
                bool flag1 = (this.myInternalTextFlags & 0x40) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.myInternalTextFlags |= 0x40;
                    }
                    else
                    {
                        this.myInternalTextFlags &= -65;
                    }
                    this.Changed(1520, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    this.UpdateSizeOrScale();
                }
            }
        }

        [DefaultValue(150), Description("The width at which wrapping occurs, if Wrapping is true."), Category("Appearance")]
        public virtual float WrappingWidth
        {
            get
            {
                return this.myWrappingWidth;
            }
            set
            {
                float single1 = this.myWrappingWidth;
                if ((value > 0f) && (single1 != value))
                {
                    this.myWrappingWidth = value;
                    this.Changed(0x5f1, 0, null, DiagramShape.MakeRect(single1), 0, null, DiagramShape.MakeRect(value));
                    this.UpdateSizeOrScale();
                }
            }
        }


        public const int ChangedAlignment = 0x5e0;
        public const int ChangedAutoResizes = 0x5ee;
        public const int ChangedBackgroundColor = 0x5e2;
        public const int ChangedBackgroundOpaqueWhenSelected = 0x5eb;
        public const int ChangedBold = 0x5e4;
        public const int ChangedBordered = 1530;
        public const int ChangedChoices = 0x5f7;
        public const int ChangedClipping = 0x5ec;
        public const int ChangedDropDownList = 0x5f6;
        public const int ChangedEditorStyle = 0x5f3;
        public const int ChangedFamilyName = 0x5de;
        public const int ChangedFontSize = 0x5df;
        public const int ChangedGdiCharSet = 0x5f2;
        public const int ChangedItalic = 0x5e5;
        public const int ChangedMaximum = 0x5f5;
        public const int ChangedMinimum = 0x5f4;
        public const int ChangedMultiline = 0x5e8;
        public const int ChangedRightToLeft = 0x5f8;
        public const int ChangedRightToLeftFromView = 0x5f9;
        public const int ChangedStrikeThrough = 0x5e7;
        public const int ChangedStringTrimming = 0x5fb;
        public const int ChangedText = 0x5dd;
        public const int ChangedTextColor = 0x5e1;
        public const int ChangedTransparentBackground = 0x5e3;
        public const int ChangedUnderline = 1510;
        public const int ChangedWrapping = 1520;
        public const int ChangedWrappingWidth = 0x5f1;
        private const byte DEFAULT_CHARSET = 1;
        private const int flagAutoResizes = 0x100;
        private const int flagBackgroundOpaqueWhenSelected = 0x200;
        private const int flagBold = 2;
        private const int flagBordered = 0x100000;
        private const int flagClipping = 0x80;
        private const int flagDropDownList = 0x800;
        private const int flagItalic = 4;
        private const int flagMultiline = 0x20;
        private const int flagRightToLeft = 0x10000000;
        private const int flagRightToLeftFromView = 0x20000000;
        private const int flagStrikeThrough = 0x10;
        private const int flagTransparentBackground = 1;
        private const int flagUnderline = 8;
        private const int flagUpdating = 0x40000000;
        private const int flagWrapping = 0x40;
        private const int maskEditorStyle = 61440;
        private const int maskGdiCharSet = 16711680;
        private const int maskStringTrimming = 251658240;
        private int myAlignment;
        private Color myBackgroundColor;
        private ArrayList myChoices;
        private static string myDefaultFontName;
        private static float myDefaultFontSize;
        [NonSerialized]
        private DiagramControl myEditor;
        private static Bitmap myEmptyBitmap;
        private static readonly ArrayList myEmptyChoices;
        private string myFamilyName;
        [NonSerialized]
        private System.Drawing.Font myFont;
        private float myFontSize;
        private int myInternalTextFlags;
        private static System.Drawing.Font myLastFont;
        private int myMaximum;
        private int myMinimum;
        private static readonly char[] myNewlineArray;
        [NonSerialized]
        private int myNumLines;
        private string myString;
        [NonSerialized]
        private StringFormat myStringFormat;
        private Color myTextColor;
        private float myWrappingWidth;

        internal sealed class ComboBoxControl : ComboBox, IDiagramControl
        {
            public ComboBoxControl()
            {
                this.myGoControl = null;
                this.myGoView = null;
            }

            private void AcceptText()
            {
                DiagramControl control1 = this.DiagramControl;
                if (control1 != null)
                {
                    DiagramText text1 = control1.EditedObject as DiagramText;
                    if (text1 != null)
                    {
                        text1.DoEdit(this.DiagramView, text1.Text, this.Text);
                    }
                    control1.DoEndEdit(this.DiagramView);
                }
            }

            private bool HandleKey(Keys key)
            {
                if (key == Keys.Escape)
                {
                    DiagramControl control1 = this.DiagramControl;
                    if (control1 != null)
                    {
                        control1.DoEndEdit(this.DiagramView);
                    }
                    this.DiagramView.InitFocus();
                    return true;
                }
                if ((key != Keys.Return) && (key != Keys.Tab))
                {
                    return false;
                }
                this.AcceptText();
                this.DiagramView.InitFocus();
                return true;
            }

            protected override void OnLeave(EventArgs evt)
            {
                this.AcceptText();
                base.OnLeave(evt);
            }

            protected override bool ProcessDialogKey(Keys key)
            {
                if (this.HandleKey(key))
                {
                    return true;
                }
                return base.ProcessDialogKey(key);
            }


            public DiagramControl DiagramControl
            {
                get
                {
                    return this.myGoControl;
                }
                set
                {
                    if (this.myGoControl != value)
                    {
                        this.myGoControl = value;
                        if (value != null)
                        {
                            DiagramText text1 = value.EditedObject as DiagramText;
                            if (text1 != null)
                            {
                                this.RightToLeft = text1.isRightToLeft(this.DiagramView) ? RightToLeft.Yes : RightToLeft.No;
                                Font font1 = text1.Font;
                                float single1 = font1.Size;
                                if (this.DiagramView != null)
                                {
                                    single1 *= (this.DiagramView.DocScale / this.DiagramView.WorldScale.Height);
                                }
                                this.Font = text1.makeFont(font1.Name, single1, font1.Style);
                                foreach (object obj1 in text1.Choices)
                                {
                                    base.Items.Add(obj1);
                                }
                                if (!text1.Multiline)
                                {
                                    int num1 = text1.Text.IndexOf("\r\n");
                                    if (num1 >= 0)
                                    {
                                        this.Text = text1.Text.Substring(0, num1);
                                    }
                                    else
                                    {
                                        this.Text = text1.Text;
                                    }
                                }
                                else
                                {
                                    this.Text = text1.Text;
                                }
                                if (text1.DropDownList)
                                {
                                    base.DropDownStyle = ComboBoxStyle.DropDownList;
                                }
                                else
                                {
                                    base.DropDownStyle = ComboBoxStyle.DropDown;
                                }
                            }
                        }
                    }
                }
            }

            public Dot.Utility.Media.Diagram.DiagramView DiagramView
            {
                get
                {
                    return this.myGoView;
                }
                set
                {
                    this.myGoView = value;
                }
            }


            private DiagramControl myGoControl;
            private Dot.Utility.Media.Diagram.DiagramView myGoView;
        }

        internal sealed class NumericUpDownControl : NumericUpDown, IDiagramControl
        {
            public NumericUpDownControl()
            {
                this.myGoControl = null;
                this.myGoView = null;
            }

            private void AcceptText()
            {
                DiagramControl control1 = this.DiagramControl;
                if (control1 != null)
                {
                    DiagramText text1 = control1.EditedObject as DiagramText;
                    if (text1 != null)
                    {
                        text1.DoEdit(this.DiagramView, text1.Text, base.Value.ToString(CultureInfo.CurrentCulture));
                    }
                    control1.DoEndEdit(this.DiagramView);
                }
            }

            private bool HandleKey(Keys key)
            {
                if (key == Keys.Escape)
                {
                    DiagramControl control1 = this.DiagramControl;
                    if (control1 != null)
                    {
                        control1.DoEndEdit(this.DiagramView);
                    }
                    this.DiagramView.InitFocus();
                    return true;
                }
                if ((key != Keys.Return) && (key != Keys.Tab))
                {
                    return false;
                }
                DiagramControl control2 = this.DiagramControl;
                if (control2 != null)
                {
                    DiagramText text1 = control2.EditedObject as DiagramText;
                    if (text1 != null)
                    {
                        text1.DoEdit(this.DiagramView, text1.Text, base.Value.ToString(CultureInfo.CurrentCulture));
                    }
                    control2.DoEndEdit(this.DiagramView);
                }
                this.DiagramView.InitFocus();
                return true;
            }

            protected override void OnLeave(EventArgs evt)
            {
                this.AcceptText();
                base.OnLeave(evt);
            }

            protected override bool ProcessDialogKey(Keys key)
            {
                if (this.HandleKey(key))
                {
                    return true;
                }
                return base.ProcessDialogKey(key);
            }


            public DiagramControl DiagramControl
            {
                get
                {
                    return this.myGoControl;
                }
                set
                {
                    if (this.myGoControl != value)
                    {
                        this.myGoControl = value;
                        if (value != null)
                        {
                            DiagramText text1 = value.EditedObject as DiagramText;
                            if (text1 != null)
                            {
                                this.RightToLeft = text1.isRightToLeft(this.DiagramView) ? RightToLeft.Yes : RightToLeft.No;
                                Font font1 = text1.Font;
                                float single1 = font1.Size;
                                if (this.DiagramView != null)
                                {
                                    single1 *= (this.DiagramView.DocScale / this.DiagramView.WorldScale.Height);
                                }
                                this.Font = text1.makeFont(font1.Name, single1, font1.Style);
                                base.Minimum = (decimal)text1.Minimum;
                                base.Maximum = (decimal)text1.Maximum;
                                try
                                {
                                    base.Value = decimal.Parse(text1.Text, CultureInfo.CurrentCulture);
                                }
                                catch (FormatException)
                                {
                                    base.Value = base.Minimum;
                                }
                                catch (OverflowException)
                                {
                                    base.Value = base.Minimum;
                                }
                            }
                        }
                    }
                }
            }

            public Dot.Utility.Media.Diagram.DiagramView DiagramView
            {
                get
                {
                    return this.myGoView;
                }
                set
                {
                    this.myGoView = value;
                }
            }


            private DiagramControl myGoControl;
            private Dot.Utility.Media.Diagram.DiagramView myGoView;
        }

        internal sealed class TextBoxControl : TextBox, IDiagramControl
        {
            public TextBoxControl()
            {
                this.myGoControl = null;
                this.myGoView = null;
            }

            private void AcceptText()
            {
                DiagramControl control1 = this.DiagramControl;
                if (control1 != null)
                {
                    DiagramText text1 = control1.EditedObject as DiagramText;
                    if (text1 != null)
                    {
                        text1.DoEdit(this.DiagramView, text1.Text, this.Text);
                    }
                    control1.DoEndEdit(this.DiagramView);
                }
            }

            private bool HandleKey(Keys key)
            {
                if (key == Keys.Escape)
                {
                    DiagramControl control1 = this.DiagramControl;
                    if (control1 != null)
                    {
                        control1.DoEndEdit(this.DiagramView);
                    }
                    this.DiagramView.InitFocus();
                    return true;
                }
                if ((key != Keys.Return) && (key != Keys.Tab))
                {
                    return false;
                }
                if ((key == Keys.Return) && base.AcceptsReturn)
                {
                    return false;
                }
                DiagramControl control2 = this.DiagramControl;
                if (control2 != null)
                {
                    DiagramText text1 = control2.EditedObject as DiagramText;
                    if (text1 != null)
                    {
                        text1.DoEdit(this.DiagramView, text1.Text, this.Text);
                    }
                    control2.DoEndEdit(this.DiagramView);
                    this.DiagramView.InitFocus();
                }
                return true;
            }

            protected override void OnLeave(EventArgs evt)
            {
                this.AcceptText();
                base.OnLeave(evt);
            }

            protected override bool ProcessDialogKey(Keys key)
            {
                if (this.HandleKey(key))
                {
                    return true;
                }
                return base.ProcessDialogKey(key);
            }


            public DiagramControl DiagramControl
            {
                get
                {
                    return this.myGoControl;
                }
                set
                {
                    if (this.myGoControl == value)
                    {
                        return;
                    }
                    this.myGoControl = value;
                    if (value == null)
                    {
                        return;
                    }
                    DiagramText text1 = value.EditedObject as DiagramText;
                    if (text1 == null)
                    {
                        return;
                    }
                    if (!text1.Multiline)
                    {
                        int num1 = text1.FindFirstLineBreak(text1.Text, 0);
                        if (num1 >= 0)
                        {
                            this.Text = text1.Text.Substring(0, num1);
                        }
                        else
                        {
                            this.Text = text1.Text;
                        }
                    }
                    else
                    {
                        this.Text = text1.Text;
                    }
                    int num2 = text1.Alignment;
                    if (num2 <= 0x10)
                    {
                        switch (num2)
                        {
                            case 1:
                                {
                                    goto Label_00D9;
                                }
                            case 2:
                            case 3:
                            case 0x10:
                                {
                                    goto Label_00D0;
                                }
                            case 4:
                            case 8:
                                {
                                    goto Label_00E2;
                                }
                        }
                    }
                    else if (num2 <= 0x40)
                    {
                        if (num2 == 0x20)
                        {
                            goto Label_00D9;
                        }
                        if (num2 == 0x40)
                        {
                            goto Label_00E2;
                        }
                    }
                    else if (num2 == 0x80)
                    {
                        goto Label_00D9;
                    }
                Label_00D0:
                    base.TextAlign = HorizontalAlignment.Left;
                    goto Label_00E9;
                Label_00D9:
                    base.TextAlign = HorizontalAlignment.Center;
                    goto Label_00E9;
                Label_00E2:
                    base.TextAlign = HorizontalAlignment.Right;
                Label_00E9:
                    this.Multiline = text1.Multiline || text1.Wrapping;
                    base.AcceptsReturn = text1.Multiline;
                    base.WordWrap = text1.Wrapping;
                    this.RightToLeft = text1.isRightToLeft(this.DiagramView) ? RightToLeft.Yes : RightToLeft.No;
                    Font font1 = text1.Font;
                    float single1 = font1.Size;
                    if (this.DiagramView != null)
                    {
                        single1 *= (this.DiagramView.DocScale / this.DiagramView.WorldScale.Height);
                    }
                    this.Font = text1.makeFont(font1.Name, single1, font1.Style);
                }
            }

            public Dot.Utility.Media.Diagram.DiagramView DiagramView
            {
                get
                {
                    return this.myGoView;
                }
                set
                {
                    this.myGoView = value;
                }
            }


            private DiagramControl myGoControl;
            private Dot.Utility.Media.Diagram.DiagramView myGoView;
        }
    }
}
