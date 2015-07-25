using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;

namespace Dot.Utility.Media.Diagram
{
    [ToolboxBitmap(typeof(DiagramPalette), "Dot.Utility.Media.Diagram.GoPalette.bmp")]
    public class DiagramPalette : DiagramView
    {
        static DiagramPalette()
        {
            DiagramPalette.AlphabeticNodeTextComparer = new AlphaComparer();
        }

        public DiagramPalette()
        {
            this.myOrientation = System.Windows.Forms.Orientation.Vertical;
            this.mySorting = SortOrder.Ascending;
            this.myComparer = DiagramPalette.AlphabeticNodeTextComparer;
            this.myAlignsSelectionObject = true;
            this.myAutomaticLayout = true;
            this.ShowsNegativeCoordinates = false;
            this.SetModifiable(false);
            this.AutoScrollRegion = new Size();
            if (base.InitAllowDrop(true))
            {
                base.AllowCopy = true;
            }
            else
            {
                base.AllowCopy = false;
            }
            base.GridCellSize = new SizeF(60f, 60f);
            base.GridOrigin = new PointF(20f, 5f);
            (this.FindMouseTool(typeof(ToolDragging)) as ToolDragging).HidesSelectionHandles = false;
        }

        public virtual void LayoutItems()
        {
            if (this.AutomaticLayout)
            {
                bool flag1 = this.Orientation == System.Windows.Forms.Orientation.Vertical;
                if (flag1)
                {
                    this.HorizontalScrollBarVisibility = DiagramViewScrollBarVisibility.Hide;
                    this.VerticalScrollBarVisibility = DiagramViewScrollBarVisibility.Auto;
                }
                else
                {
                    this.HorizontalScrollBarVisibility = DiagramViewScrollBarVisibility.Auto;
                    this.VerticalScrollBarVisibility = DiagramViewScrollBarVisibility.Hide;
                }
                ICollection collection1 = this.Document;
                if ((this.Sorting != SortOrder.None) && (this.Comparer != null))
                {
                    Shapes.DiagramShape[] objArray1 = this.Document.CopyArray();
                    Array.Sort(objArray1, 0, objArray1.Length, this.Comparer);
                    if (this.Sorting == SortOrder.Descending)
                    {
                        Array.Reverse(objArray1, 0, objArray1.Length);
                    }
                    collection1 = objArray1;
                }
                SizeF ef1 = this.DocExtentSize;
                SizeF ef2 = this.GridCellSize;
                PointF tf1 = this.GridOrigin;
                bool flag2 = this.AlignsSelectionObject;
                bool flag3 = true;
                PointF tf2 = tf1;
                float single1 = System.Math.Min(tf1.X, (float)0f);
                float single2 = System.Math.Min(tf1.Y, (float)0f);
                foreach (Shapes.DiagramShape obj1 in collection1)
                {
                    Shapes.DiagramShape obj2 = obj1;
                    if (flag2)
                    {
                        obj2 = obj1.SelectionObject;
                        if (obj2 == null)
                        {
                            obj2 = obj1;
                        }
                    }
                    obj2.Position = tf2;
                    if (flag1)
                    {
                        tf2 = this.ShiftRight(obj1, obj2, single1, tf2, ef2);
                        if (!flag3 && (obj1.Right >= ef1.Width))
                        {
                            single1 = System.Math.Min(tf1.X, (float)0f);
                            tf2.X = tf1.X;
                            tf2.Y = System.Math.Max((float)(tf2.Y + ef2.Height), single2);
                            obj2.Position = tf2;
                            tf2 = this.ShiftRight(obj1, obj2, single1, tf2, ef2);
                        }
                        tf2.X += ef2.Width;
                    }
                    else
                    {
                        tf2 = this.ShiftDown(obj1, obj2, single2, tf2, ef2);
                        if (!flag3 && (obj1.Bottom >= ef1.Height))
                        {
                            single2 = System.Math.Min(tf1.Y, (float)0f);
                            tf2.Y = tf1.Y;
                            tf2.X += System.Math.Max((float)(tf2.X + ef2.Width), single1);
                            obj2.Position = tf2;
                            tf2 = this.ShiftDown(obj1, obj2, single2, tf2, ef2);
                        }
                        tf2.Y += ef2.Height;
                    }
                    single1 = System.Math.Max(single1, obj1.Right);
                    single2 = System.Math.Max(single2, obj1.Bottom);
                    flag3 = false;
                }
                RectangleF ef3 = this.ComputeDocumentBounds();
                this.Document.Size = new SizeF(ef3.Width, ef3.Height);
                this.Document.TopLeft = new PointF(ef3.X, ef3.Y);
            }
        }

        protected override void OnDocumentChanged(object sender, ChangedEventArgs e)
        {
            base.OnDocumentChanged(sender, e);
            if ((e.Hint == 0x386) || (e.Hint == 0x387))
            {
                this.LayoutItems();
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs evt)
        {
            base.OnPropertyChanged(evt);
            if (evt.PropertyName == "DocScale")
            {
                this.LayoutItems();
            }
        }

        protected override void OnSizeChanged(EventArgs evt)
        {
            base.OnSizeChanged(evt);
            this.LayoutItems();
        }

        private PointF ShiftDown(Shapes.DiagramShape obj, Shapes.DiagramShape selobj, float maxrow, PointF pnt, SizeF cellsize)
        {
            while (obj.Top < maxrow)
            {
                pnt.Y += cellsize.Height;
                float single1 = obj.Top;
                selobj.Top = pnt.Y;
                if (obj.Top <= single1)
                {
                    break;
                }
            }
            return pnt;
        }

        private PointF ShiftRight(Shapes.DiagramShape obj, Shapes.DiagramShape selobj, float maxcol, PointF pnt, SizeF cellsize)
        {
            while (obj.Left < maxcol)
            {
                pnt.X += cellsize.Width;
                float single1 = obj.Left;
                selobj.Left = pnt.X;
                if (obj.Left <= single1)
                {
                    break;
                }
            }
            return pnt;
        }


        [Description("Whether to grid-align each whole item or each item's SelectionObject"), Category("Appearance"), DefaultValue(true)]
        public virtual bool AlignsSelectionObject
        {
            get
            {
                return this.myAlignsSelectionObject;
            }
            set
            {
                if (this.myAlignsSelectionObject != value)
                {
                    this.myAlignsSelectionObject = value;
                    this.LayoutItems();
                    base.RaisePropertyChangedEvent("AlignsSelectionObject");
                }
            }
        }

        [DefaultValue(true), Category("Appearance"), Description("Whether to automatically position all of the items in a grid")]
        public virtual bool AutomaticLayout
        {
            get
            {
                return this.myAutomaticLayout;
            }
            set
            {
                if (this.myAutomaticLayout != value)
                {
                    this.myAutomaticLayout = value;
                    this.LayoutItems();
                    base.RaisePropertyChangedEvent("AutomaticLayout");
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual IComparer Comparer
        {
            get
            {
                return this.myComparer;
            }
            set
            {
                IComparer comparer1 = this.myComparer;
                if (value == null)
                {
                    value = DiagramPalette.AlphabeticNodeTextComparer;
                }
                if (comparer1 != value)
                {
                    this.myComparer = value;
                    this.LayoutItems();
                    base.RaisePropertyChangedEvent("Comparer");
                }
            }
        }

        public override SizeF GridCellSize
        {
            get
            {
                return base.GridCellSize;
            }
            set
            {
                base.GridCellSize = value;
                this.LayoutItems();
            }
        }

        public override PointF GridOrigin
        {
            get
            {
                return base.GridOrigin;
            }
            set
            {
                base.GridOrigin = value;
                this.LayoutItems();
            }
        }

        [DefaultValue(1), Category("Appearance"), Description("How to fill the palette by positioning its items.")]
        public virtual System.Windows.Forms.Orientation Orientation
        {
            get
            {
                return this.myOrientation;
            }
            set
            {
                if (this.myOrientation != value)
                {
                    this.myOrientation = value;
                    this.LayoutItems();
                    base.RaisePropertyChangedEvent("Orientation");
                }
            }
        }

        [DefaultValue(1), Description("Whether the items in the palette are sorted before being positioned."), Category("Appearance")]
        public virtual SortOrder Sorting
        {
            get
            {
                return this.mySorting;
            }
            set
            {
                if (this.mySorting != value)
                {
                    this.mySorting = value;
                    this.LayoutItems();
                    base.RaisePropertyChangedEvent("Sorting");
                }
            }
        }


        internal static readonly IComparer AlphabeticNodeTextComparer;
        private bool myAlignsSelectionObject;
        private bool myAutomaticLayout;
        private IComparer myComparer;
        private System.Windows.Forms.Orientation myOrientation;
        private SortOrder mySorting;

        [Serializable]
        internal sealed class AlphaComparer : IComparer
        {
            internal AlphaComparer()
            {
                this.myCultureInfo = CultureInfo.CurrentCulture;
            }

            public int Compare(object x, object y)
            {
                Shapes.ITextNode node1 = x as Shapes.ITextNode;
                Shapes.ITextNode node2 = y as Shapes.ITextNode;
                if (node1 != null)
                {
                    if (node2 != null)
                    {
                        return string.Compare(node1.Text, node2.Text, true, this.myCultureInfo);
                    }
                    return 1;
                }
                if (node2 != null)
                {
                    return -1;
                }
                return 0;
            }


            public CultureInfo Culture
            {
                get
                {
                    return this.myCultureInfo;
                }
                set
                {
                    this.myCultureInfo = value;
                }
            }


            private CultureInfo myCultureInfo;
        }
    }
}
