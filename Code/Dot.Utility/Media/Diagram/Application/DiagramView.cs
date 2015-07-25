using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Dot.Utility.Media.Diagram.Shapes;
using System.Drawing.Printing;
using System.Collections;
using Dot.Utility.Media.Diagram.Tools;
using System.Threading;
using System.Security;
using System.Globalization;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Security.Permissions;

namespace Dot.Utility.Media.Diagram
{
    [ToolboxBitmap(typeof(DiagramView), "Dot.Utility.Media.Diagram.DiagramView.bmp")]
    public class DiagramView : Control, ILayerCollectionContainer, ILayerAbilities
    {
        public event InputEventHandler BackgroundContextClicked;
        public event InputEventHandler BackgroundDoubleClicked;
        public event InputEventHandler BackgroundHover;
        public event InputEventHandler BackgroundSingleClicked;
        public event EventHandler ClipboardPasted;
        public event ChangedEventHandler DocumentChanged;
        public event InputEventHandler ExternalObjectsDropped;
        public event SelectionEventHandler LinkCreated;
        public event SelectionEventHandler LinkRelinked;
        public event DiagramShapeEventHandler ShapeContextClicked;
        public event DiagramShapeEventHandler ShapeDoubleClicked;
        public event SelectionEventHandler ShapeEdited;
        public event SelectionEventHandler ShapeSelected;
        public event DiagramShapeEventHandler ShapeHover;
        public event SelectionEventHandler ShapeLostSelection;
        public event SelectionEventHandler ShapeResized;
        public event DiagramShapeEventHandler ShapeSingleClicked;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SelectionCopied;
        public event EventHandler SelectionDeleted;
        public event CancelEventHandler SelectionDeleting;
        public event EventHandler SelectionMoved;
        public event SelectionEventHandler ShapeAdded;
        public event SelectionEventHandler ShapeRemoved;

        static DiagramView()
        {
            DiagramView.myVersionName = "";
            DiagramView.myVersionAssembly = null;
        }

        public DiagramView()
        {
            this.myVertScroll = null;
            this.myHorizScroll = null;
            this.myScrollBarWidth = SystemInformation.VerticalScrollBarWidth;
            this.myScrollBarHeight = SystemInformation.HorizontalScrollBarHeight;
            this.myVertScrollHandler = null;
            this.myHorizScrollHandler = null;
            this.myVertScrollVisibility = DiagramViewScrollBarVisibility.Auto;
            this.myHorizScrollVisibility = DiagramViewScrollBarVisibility.Auto;
            this.myCorner = null;
            this.mySafeOnDocumentChangedDelegate = null;
            this.myQueuedEvents = null;
            this.myAllowDragOut = true;
            this.myExternalDragImage = null;
            this.myPretendInternalDrag = false;
            this.myExternalDragDropsOnEnter = false;
            this.myGraphics = null;
            this.myPaintEventArgs = null;
            this.mySuppressPaint = 0;
            this.myUpdatingScrollBars = true;
            this.myAutoScrollRegion = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
            this.myAutoScrollTime = 100;
            this.myAutoScrollDelay = 1000;
            this.myAutoScrollTimer = null;
            this.myAutoScrollTimerEnabled = false;
            this.myAutoScrollPoint = new Point();
            this.myPanning = false;
            this.myPanningOrigin = new Point();
            this.myToolTip = null;
            this.myDefaultCursor = null;
            this.myHoverTimer = null;
            this.myHoverTimerEnabled = false;
            this.myHoverDelay = 1000;
            this.myHoverPoint = new Point(0, 0);
            this.myPrintInfo = null;
            this.myPrintScale = 0.8f;
            this.myPrevXorRect = new Rectangle();
            this.myPrevXorRectValid = false;
            this.myEditControl = null;
            this.myGoControls = null;
            this.myModalControl = null;
            this.myCancelMouseDown = false;
            this.myImageList = null;
            this.myBorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.myBorder3DStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.myBorderSize = SystemInformation.Border3DSize;
            this.myDocument = null;
            this.myDocChangedEventHandler = null;
            this.mySelection = null;
            this.myMaximumSelectionCount = 1000000;
            this.myPrimarySelectionColor = Color.Chartreuse;
            this.mySecondarySelectionColor = Color.Cyan;
            this.myNoFocusSelectionColor = Color.LightGray;
            this.myResizeHandleSize = new SizeF(6f, 6f);
            this.myResizeHandlePenWidth = 1f;
            this.myBoundingHandlePenWidth = 2f;
            this.myHidesSelection = false;
            this.mySelectsByFirstChar = true;
            this.myLayers = null;
            this.myScrollSmallChange = new Size(0x10, 0x10);
            this.myAutoPanRegion = new Size(0x10, 0x10);
            this.myShowsNegativeCoordinates = true;
            this.myOrigin = new PointF();
            this.myHorizScale = 1f;
            this.myVertScale = 1f;
            this.myHorizWorld = 1f;
            this.myVertWorld = 1f;
            this.mySmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.myTextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.myInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.myAllowSelect = true;
            this.myAllowMove = true;
            this.myAllowCopy = true;
            this.myAllowResize = true;
            this.myAllowReshape = true;
            this.myAllowDelete = true;
            this.myAllowInsert = true;
            this.myAllowLink = true;
            this.myAllowEdit = true;
            this.myAllowMouse = true;
            this.myAllowKey = true;
            this.myCurrentResult = null;
            this.myBuffer = null;
            this.myTempArrays = null;
            this.myFirstInput = new InputEventArgs();
            this.myLastInput = new InputEventArgs();
            this.myTool = null;
            this.myDefaultTool = null;
            this.myMouseDownTools = null;
            this.myMouseMoveTools = null;
            this.myMouseUpTools = null;
            this.myDragsRealtime = false;
            this.myPortGravity = 100f;
            this._LineType = typeof(Shapes.LineGraph);
            this.myBackgroundBrush = null;
            this.myGridStyle = DiagramViewGridStyle.None;
            this.myGridOrigin = new PointF();
            this.myGridCellSize = new SizeF(50f, 50f);
            this.myGridColor = Color.LightGray;
            this.myGridPen = null;
            this.myGridPenWidth = 1f;
            this.myGridPenDashStyle = DashStyle.Solid;
            this.mySnapDrag = DiagramViewSnapStyle.None;
            this.mySnapResize = DiagramViewSnapStyle.None;
            this.myShadowOffset = new SizeF(5f, 5f);
            this.myShadowColor = Color.FromArgb(0x7f, Color.Gray);
            this.myShadowBrush = null;
            this.myShadowPen = null;
            this.myPaintNothingScale = 0.13f;
            this.myPaintGreekScale = 0.24f;
            this.init(null);
        }

        public DiagramView(DiagramDocument doc)
        {
            this.myVertScroll = null;
            this.myHorizScroll = null;
            this.myScrollBarWidth = SystemInformation.VerticalScrollBarWidth;
            this.myScrollBarHeight = SystemInformation.HorizontalScrollBarHeight;
            this.myVertScrollHandler = null;
            this.myHorizScrollHandler = null;
            this.myVertScrollVisibility = DiagramViewScrollBarVisibility.Auto;
            this.myHorizScrollVisibility = DiagramViewScrollBarVisibility.Auto;
            this.myCorner = null;
            this.mySafeOnDocumentChangedDelegate = null;
            this.myQueuedEvents = null;
            this.myAllowDragOut = true;
            this.myExternalDragImage = null;
            this.myPretendInternalDrag = false;
            this.myExternalDragDropsOnEnter = false;
            this.myGraphics = null;
            this.myPaintEventArgs = null;
            this.mySuppressPaint = 0;
            this.myUpdatingScrollBars = true;
            this.myAutoScrollRegion = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
            this.myAutoScrollTime = 100;
            this.myAutoScrollDelay = 1000;
            this.myAutoScrollTimer = null;
            this.myAutoScrollTimerEnabled = false;
            this.myAutoScrollPoint = new Point();
            this.myPanning = false;
            this.myPanningOrigin = new Point();
            this.myToolTip = null;
            this.myDefaultCursor = null;
            this.myHoverTimer = null;
            this.myHoverTimerEnabled = false;
            this.myHoverDelay = 1000;
            this.myHoverPoint = new Point(0, 0);
            this.myPrintInfo = null;
            this.myPrintScale = 0.8f;
            this.myPrevXorRect = new Rectangle();
            this.myPrevXorRectValid = false;
            this.myEditControl = null;
            this.myGoControls = null;
            this.myModalControl = null;
            this.myCancelMouseDown = false;
            this.myImageList = null;
            this.myBorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.myBorder3DStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.myBorderSize = SystemInformation.Border3DSize;
            this.myDocument = null;
            this.myDocChangedEventHandler = null;
            this.mySelection = null;
            this.myMaximumSelectionCount = 1000000;
            this.myPrimarySelectionColor = Color.Chartreuse;
            this.mySecondarySelectionColor = Color.Cyan;
            this.myNoFocusSelectionColor = Color.LightGray;
            this.myResizeHandleSize = new SizeF(6f, 6f);
            this.myResizeHandlePenWidth = 1f;
            this.myBoundingHandlePenWidth = 2f;
            this.myHidesSelection = false;
            this.mySelectsByFirstChar = true;
            this.myLayers = null;
            this.myScrollSmallChange = new Size(0x10, 0x10);
            this.myAutoPanRegion = new Size(0x10, 0x10);
            this.myShowsNegativeCoordinates = true;
            this.myOrigin = new PointF();
            this.myHorizScale = 1f;
            this.myVertScale = 1f;
            this.myHorizWorld = 1f;
            this.myVertWorld = 1f;
            this.mySmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.myTextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.myInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.myAllowSelect = true;
            this.myAllowMove = true;
            this.myAllowCopy = true;
            this.myAllowResize = true;
            this.myAllowReshape = true;
            this.myAllowDelete = true;
            this.myAllowInsert = true;
            this.myAllowLink = true;
            this.myAllowEdit = true;
            this.myAllowMouse = true;
            this.myAllowKey = true;
            this.myCurrentResult = null;
            this.myBuffer = null;
            this.myTempArrays = null;
            this.myFirstInput = new InputEventArgs();
            this.myLastInput = new InputEventArgs();
            this.myTool = null;
            this.myDefaultTool = null;
            this.myMouseDownTools = null;
            this.myMouseMoveTools = null;
            this.myMouseUpTools = null;
            this.myDragsRealtime = false;
            this.myPortGravity = 100f;
            this._LineType = typeof(Shapes.LineGraph);
            this.myBackgroundBrush = null;
            this.myGridStyle = DiagramViewGridStyle.None;
            this.myGridOrigin = new PointF();
            this.myGridCellSize = new SizeF(50f, 50f);
            this.myGridColor = Color.LightGray;
            this.myGridPen = null;
            this.myGridPenWidth = 1f;
            this.myGridPenDashStyle = DashStyle.Solid;
            this.mySnapDrag = DiagramViewSnapStyle.None;
            this.mySnapResize = DiagramViewSnapStyle.None;
            this.myShadowOffset = new SizeF(5f, 5f);
            this.myShadowColor = Color.FromArgb(0x7f, Color.Gray);
            this.myShadowBrush = null;
            this.myShadowPen = null;
            this.myPaintNothingScale = 0.13f;
            this.myPaintGreekScale = 0.24f;
            this.init(doc);
        }

        void DefaultLayer_ShapeRemoved(object sender, SelectionEventArgs e)
        {
            if (this.ShapeRemoved != null)
            {
                this.ShapeRemoved(sender, e);
            }
        }

        void DefaultLayer_ShapeAdded(object sender, SelectionEventArgs e)
        {
            if (this.ShapeAdded != null)
            {
                this.ShapeAdded(sender, e);
            }
        }

        public virtual bool AbortTransaction()
        {
            return this.Document.AbortTransaction();
        }

        internal void AddGoControl(Shapes.DiagramControl g, Control c)
        {
            if (this.myGoControls == null)
            {
                this.myGoControls = new ArrayList();
            }
            this.myGoControls.Add(g);
            base.Controls.Add(c);
        }

        internal PointF[] AllocTempPointArray(int len)
        {
            if ((this.myTempArrays == null) || (len >= this.myTempArrays.Length))
            {
                this.myTempArrays = new PointF[System.Math.Max((int)(len + 1), 10)][];
            }
            PointF[] tfArray1 = this.myTempArrays[len];
            if (tfArray1 == null)
            {
                return new PointF[len];
            }
            this.myTempArrays[len] = null;
            return tfArray1;
        }

        private void autoScrollCallback(object obj)
        {
            if (base.IsHandleCreated)
            {
                base.Invoke((EventHandler)obj);
            }
        }

        private void autoScrollTick(object sender, EventArgs evt)
        {
            if (this.myAutoScrollTimerEnabled)
            {
                PointF tf1 = this.myPanning ? this.ComputeAutoPanDocPosition(this.myPanningOrigin, this.myAutoScrollPoint) : this.ComputeAutoScrollDocPosition(this.myAutoScrollPoint);
                if (tf1 == this.DocPosition)
                {
                    this.myAutoScrollTimer.Change(this.AutoScrollDelay, -1);
                }
                else
                {
                    PointF tf2 = this.DocPosition;
                    this.DocPosition = tf1;
                    if (tf2 != tf1)
                    {
                        this.DrawXorBox(this.myPrevXorRect, false);
                    }
                    this.myAutoScrollTimer.Change(this.AutoScrollTime, -1);
                }
            }
        }

        public virtual void BeginUpdate()
        {
            this.mySuppressPaint++;
        }

        public virtual bool CanCopyObjects()
        {
            if (this.AllowCopy)
            {
                return this.Document.CanCopyObjects();
            }
            return false;
        }

        public virtual bool CanDeleteObjects()
        {
            if (this.AllowDelete)
            {
                return this.Document.CanDeleteObjects();
            }
            return false;
        }

        public virtual bool CanEditCopy()
        {
            if (!this.CanCopyObjects())
            {
                return false;
            }
            if (this.Selection.IsEmpty)
            {
                return false;
            }
            if (!this.Selection.Primary.CanCopy())
            {
                return false;
            }
            return true;
        }

        public virtual bool CanEditCut()
        {
            if (!this.CanCopyObjects())
            {
                return false;
            }
            if (!this.CanDeleteObjects())
            {
                return false;
            }
            if (this.Selection.IsEmpty)
            {
                return false;
            }
            Shapes.DiagramShape obj1 = this.Selection.Primary;
            if (!obj1.CanCopy())
            {
                return false;
            }
            if (!obj1.CanDelete())
            {
                return false;
            }
            return true;
        }

        public virtual bool CanEditDelete()
        {
            if (!this.CanDeleteObjects())
            {
                return false;
            }
            if (this.Selection.IsEmpty)
            {
                return false;
            }
            if (!this.Selection.Primary.CanDelete())
            {
                return false;
            }
            return true;
        }

        public virtual bool CanEditEdit()
        {
            if (!this.CanEditObjects())
            {
                return false;
            }
            if (this.Selection.IsEmpty)
            {
                return false;
            }
            if (!this.Selection.Primary.CanEdit())
            {
                return false;
            }
            return true;
        }

        public virtual bool CanEditObjects()
        {
            if (this.AllowEdit)
            {
                return this.Document.CanEditObjects();
            }
            return false;
        }

        [PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\r\n               version=\"1\">\r\n   <IPermission class=\"System.Security.Permissions.UIPermission, mscorlib, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                version=\"1\"\r\n                Clipboard=\"AllClipboard\"/>\r\n</PermissionSet>\r\n")]
        public virtual bool CanEditPaste()
        {
            if (!this.CanInsertObjects())
            {
                return false;
            }
            DiagramDocument document1 = this.Document;
            if (document1 == null)
            {
                return false;
            }
            IDataObject obj1 = Clipboard.GetDataObject();
            if (obj1 == null)
            {
                return false;
            }
            return obj1.GetDataPresent(document1.DataFormat);
        }

        public virtual bool CanInsertObjects()
        {
            if (this.AllowInsert)
            {
                return this.Document.CanInsertObjects();
            }
            return false;
        }

        public virtual bool CanLinkObjects()
        {
            if (this.AllowLink)
            {
                return this.Document.CanLinkObjects();
            }
            return false;
        }

        public virtual bool CanMoveObjects()
        {
            if (this.AllowMove)
            {
                return this.Document.CanMoveObjects();
            }
            return false;
        }

        public virtual bool CanRedo()
        {
            return this.Document.CanRedo();
        }

        public virtual bool CanReshapeObjects()
        {
            if (this.AllowReshape)
            {
                return this.Document.CanReshapeObjects();
            }
            return false;
        }

        public virtual bool CanResizeObjects()
        {
            if (this.AllowResize)
            {
                return this.Document.CanResizeObjects();
            }
            return false;
        }

        public virtual bool CanScroll(bool down, bool vertical)
        {
            PointF tf1 = this.DocPosition;
            SizeF ef1 = this.DocExtentSize;
            PointF tf2 = this.DocumentTopLeft;
            SizeF ef2 = this.DocumentSize;
            if (vertical)
            {
                if (down)
                {
                    tf1.Y += 1f;
                    tf1.Y = System.Math.Min(tf1.Y, System.Math.Max(tf2.Y, (float)((tf2.Y + ef2.Height) - ef1.Height)));
                }
                else
                {
                    tf1.Y -= 1f;
                    tf1.Y = System.Math.Max(tf1.Y, tf2.Y);
                }
            }
            else if (down)
            {
                tf1.X += 1f;
                tf1.X = System.Math.Min(tf1.X, System.Math.Max(tf2.X, (float)((tf2.X + ef2.Width) - ef1.Width)));
            }
            else
            {
                tf1.X -= 1f;
                tf1.X = System.Math.Max(tf1.X, tf2.X);
            }
            return (tf1 != this.DocPosition);
        }

        public virtual bool CanSelectObjects()
        {
            if (this.AllowSelect)
            {
                return this.Document.CanSelectObjects();
            }
            return false;
        }

        public virtual bool CanUndo()
        {
            return this.Document.CanUndo();
        }

        private void CleanUpModalControl()
        {
            if ((this.myEditControl != null) && (this.myModalControl != null))
            {
                Shapes.DiagramControl control1 = this.myEditControl;
                Control control2 = this.myModalControl;
                this.myEditControl = null;
                this.myModalControl = null;
                control1.DisposeControl(control2, this);
            }
        }

        public virtual PointF ComputeAutoPanDocPosition(Point originPnt, Point viewPnt)
        {
            PointF tf1 = this.DocPosition;
            Point point1 = this.ConvertDocToView(tf1);
            Size size1 = this.AutoPanRegion;
            int num1 = this.ScrollSmallChange.Width;
            int num2 = this.ScrollSmallChange.Height;
            Rectangle rectangle1 = this.DisplayRectangle;
            int num3 = viewPnt.X - originPnt.X;
            int num4 = viewPnt.Y - originPnt.Y;
            int num5 = size1.Width;
            int num6 = size1.Height;
            int num7 = 2 * num5;
            int num8 = 2 * num6;
            if (num3 < -num7)
            {
                int num9 = ((num3 + num5) * (num3 + num5)) / 100;
                point1.X -= System.Math.Min(rectangle1.Width, (int)(num1 * num9));
            }
            else if (num3 < -num5)
            {
                point1.X -= num1;
            }
            else if (num3 > num7)
            {
                int num10 = ((num3 - num5) * (num3 - num5)) / 100;
                point1.X += System.Math.Min(rectangle1.Width, (int)(num1 * num10));
            }
            else if (num3 > num5)
            {
                point1.X += num1;
            }
            if (num4 < -num8)
            {
                int num11 = ((num4 + num6) * (num4 + num6)) / 100;
                point1.Y -= System.Math.Min(rectangle1.Height, (int)(num2 * num11));
            }
            else if (num4 < -num6)
            {
                point1.Y -= num2;
            }
            else if (num4 > num8)
            {
                int num12 = ((num4 - num6) * (num4 - num6)) / 100;
                point1.Y += System.Math.Min(rectangle1.Height, (int)(num2 * num12));
            }
            else if (num4 > num6)
            {
                point1.Y += num2;
            }
            return this.ConvertViewToDoc(point1);
        }

        public virtual PointF ComputeAutoScrollDocPosition(Point viewPnt)
        {
            PointF tf1 = this.DocPosition;
            Point point1 = this.ConvertDocToView(tf1);
            Size size1 = this.AutoScrollRegion;
            int num1 = this.ScrollSmallChange.Width;
            int num2 = this.ScrollSmallChange.Height;
            Rectangle rectangle1 = this.DisplayRectangle;
            if ((viewPnt.X >= rectangle1.X) && (viewPnt.X < (rectangle1.X + size1.Width)))
            {
                point1.X -= num1;
                if (viewPnt.X < (rectangle1.X + (size1.Width / 2)))
                {
                    point1.X -= num1;
                }
                if (viewPnt.X < (rectangle1.X + (size1.Width / 4)))
                {
                    point1.X -= (2 * num1);
                }
            }
            else if ((viewPnt.X <= (rectangle1.X + rectangle1.Width)) && (viewPnt.X > ((rectangle1.X + rectangle1.Width) - size1.Width)))
            {
                point1.X += num1;
                if (viewPnt.X > ((rectangle1.X + rectangle1.Width) - (size1.Width / 2)))
                {
                    point1.X += num1;
                }
                if (viewPnt.X > ((rectangle1.X + rectangle1.Width) - (size1.Width / 4)))
                {
                    point1.X += (2 * num1);
                }
            }
            if ((viewPnt.Y >= rectangle1.Y) && (viewPnt.Y < (rectangle1.Y + size1.Height)))
            {
                point1.Y -= num2;
                if (viewPnt.Y < (rectangle1.Y + (size1.Height / 2)))
                {
                    point1.Y -= num2;
                }
                if (viewPnt.Y < (rectangle1.Y + (size1.Height / 4)))
                {
                    point1.Y -= (2 * num2);
                }
            }
            else if ((viewPnt.Y <= (rectangle1.Y + rectangle1.Height)) && (viewPnt.Y > ((rectangle1.Y + rectangle1.Height) - size1.Height)))
            {
                point1.Y += num2;
                if (viewPnt.Y > ((rectangle1.Y + rectangle1.Height) - (size1.Height / 2)))
                {
                    point1.Y += num2;
                }
                if (viewPnt.Y > ((rectangle1.Y + rectangle1.Height) - (size1.Height / 4)))
                {
                    point1.Y += (2 * num2);
                }
            }
            return this.ConvertViewToDoc(point1);
        }

        public virtual RectangleF ComputeDocumentBounds()
        {
            return DiagramDocument.ComputeBounds(this.Document, this);
        }

        public virtual Point ConvertDocToView(PointF p)
        {
            PointF tf1 = this.DocPosition;
            return new Point(((int)System.Math.Floor((double)(((p.X - tf1.X) * this.myHorizScale) * this.myHorizWorld))) + this.myBorderSize.Width, ((int)System.Math.Floor((double)(((p.Y - tf1.Y) * this.myVertScale) * this.myVertWorld))) + this.myBorderSize.Height);
        }

        public virtual Rectangle ConvertDocToView(RectangleF r)
        {
            PointF tf1 = this.DocPosition;
            return new Rectangle(((int)System.Math.Floor((double)(((r.X - tf1.X) * this.myHorizScale) * this.myHorizWorld))) + this.myBorderSize.Width, ((int)System.Math.Floor((double)(((r.Y - tf1.Y) * this.myVertScale) * this.myVertWorld))) + this.myBorderSize.Height, (int)System.Math.Ceiling((double)((r.Width * this.myHorizScale) * this.myHorizWorld)), (int)System.Math.Ceiling((double)((r.Height * this.myVertScale) * this.myVertWorld)));
        }

        public virtual Size ConvertDocToView(SizeF s)
        {
            return new Size((int)System.Math.Ceiling((double)((s.Width * this.myHorizScale) * this.myHorizWorld)), (int)System.Math.Ceiling((double)((s.Height * this.myVertScale) * this.myVertWorld)));
        }

        public virtual PointF ConvertViewToDoc(Point p)
        {
            PointF tf1 = this.DocPosition;
            return new PointF(((((float)(p.X - this.myBorderSize.Width)) / this.myHorizWorld) / this.myHorizScale) + tf1.X, ((((float)(p.Y - this.myBorderSize.Height)) / this.myVertWorld) / this.myVertScale) + tf1.Y);
        }

        public virtual RectangleF ConvertViewToDoc(Rectangle r)
        {
            PointF tf1 = this.DocPosition;
            return new RectangleF(((((float)(r.X - this.myBorderSize.Width)) / this.myHorizWorld) / this.myHorizScale) + tf1.X, ((((float)(r.Y - this.myBorderSize.Height)) / this.myVertWorld) / this.myVertScale) + tf1.Y, (((float)r.Width) / this.myHorizWorld) / this.myHorizScale, (((float)r.Height) / this.myVertWorld) / this.myVertScale);
        }

        public virtual SizeF ConvertViewToDoc(Size s)
        {
            return new SizeF((((float)s.Width) / this.myHorizWorld) / this.myHorizScale, (((float)s.Height) / this.myVertWorld) / this.myVertScale);
        }

        public virtual void CopySelection(DiagramSelection sel, SizeF offset, bool grid)
        {
            if (sel == null)
            {
                sel = this.Selection;
            }
            if (((sel != this.Selection) || this.CanCopyObjects()) && !sel.IsEmpty)
            {
                DiagramDocument document1 = this.Document;
                string text1 = null;
                try
                {
                    this.StartTransaction();
                    CopyDictionary dictionary1 = document1.CreateCopyDictionary();
                    document1.CopyFromCollection(sel, true, true, offset, dictionary1);
                    this.Selection.Clear();
                    IDictionaryEnumerator enumerator1 = dictionary1.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        Shapes.DiagramShape obj1 = enumerator1.Value as Shapes.DiagramShape;
                        if (((obj1 != null) && obj1.IsTopLevel) && (obj1.Document == document1))
                        {
                            this.Selection.Add(obj1);
                        }
                    }
                    if (grid)
                    {
                        Shapes.DiagramShape obj2 = null;
                        CollectionEnumerator enumerator2 = this.Selection.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Shapes.DiagramShape obj3 = enumerator2.Current;
                            if (!(obj3 is Shapes.IDiagramLine))
                            {
                                obj2 = obj3;
                                break;
                            }
                        }
                        SizeF ef1 = offset;
                        if (obj2 != null)
                        {
                            PointF tf1 = obj2.Location;
                            PointF tf2 = this.FindNearestGridPoint(tf1);
                            ef1.Width = tf2.X - tf1.X;
                            ef1.Height = tf2.Y - tf1.Y;
                            enumerator2 = this.Selection.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                Shapes.DiagramShape obj4 = enumerator2.Current;
                                if (obj4 is Shapes.IDiagramLine)
                                {
                                    obj4.Position = new PointF(obj4.Left + ef1.Width, obj4.Top + ef1.Height);
                                }
                            }
                        }
                        enumerator2 = this.Selection.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Shapes.DiagramShape obj5 = enumerator2.Current;
                            if (!(obj5 is Shapes.IDiagramLine))
                            {
                                PointF tf3 = obj5.Location;
                                PointF tf4 = this.FindNearestGridPoint(tf3);
                                obj5.DoMove(this, tf3, tf4);
                            }
                        }
                    }
                    text1 = "Copy Selection";
                }
                finally
                {
                    this.FinishTransaction(text1);
                }
            }
        }

        [PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\r\n               version=\"1\">\r\n   <IPermission class=\"System.Security.Permissions.UIPermission, mscorlib, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                version=\"1\"\r\n                Clipboard=\"AllClipboard\"/>\r\n</PermissionSet>\r\n")]
        public virtual void CopyToClipboard(Shapes.IDiagramShapeCollection coll)
        {
            if ((coll == null) || coll.IsEmpty)
            {
                Clipboard.SetDataObject(new DataObject());
            }
            else
            {
                DiagramDocument document1 = this.Document;
                if (document1 != null)
                {
                    Type type1 = document1.GetType();
                    DiagramDocument document2 = (DiagramDocument)Activator.CreateInstance(type1);
                    document2.UndoManager = null;
                    document2.MergeLayersFrom(document1);
                    document2.CopyFromCollection(coll, true, true, new SizeF(), null);
                    DataObject obj1 = this.CreateDataObject(coll, document2);
                    Clipboard.SetDataObject(obj1);
                }
            }
        }

        protected virtual DataObject CreateDataObject(Shapes.IDiagramShapeCollection coll, DiagramDocument clipdoc)
        {
            DataObject obj1 = new DataObject();
            obj1.SetData(clipdoc.DataFormat, clipdoc);
            Bitmap bitmap1 = this.GetBitmapFromCollection(clipdoc);
            obj1.SetData(DataFormats.Bitmap, true, bitmap1);
            string text1 = null;
            LayerCollectionObjectEnumerator enumerator1 = clipdoc.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                Shapes.DiagramShape obj2 = enumerator1.Current;
                Shapes.ITextNode node1 = obj2 as Shapes.ITextNode;
                if (node1 != null)
                {
                    if (text1 == null)
                    {
                        text1 = node1.Text;
                        continue;
                    }
                    text1 = text1 + Environment.NewLine + node1.Text;
                    continue;
                }
                Shapes.DiagramText text2 = obj2 as Shapes.DiagramText;
                if (text2 != null)
                {
                    if (text1 == null)
                    {
                        text1 = text2.Text;
                        continue;
                    }
                    text1 = text1 + Environment.NewLine + text2.Text;
                }
            }
            if (text1 != null)
            {
                obj1.SetData(DataFormats.UnicodeText, true, text1);
            }
            return obj1;
        }

        public virtual IDiagramTool CreateDefaultTool()
        {
            return new ToolManager(this);
        }

        public virtual DiagramDocument CreateDocument()
        {
            return new DiagramDocument();
        }

        public virtual Shapes.IDiagramLine CreateLink(Shapes.IDiagramPort fromPort, Shapes.IDiagramPort toPort)
        {
            if (((fromPort != null) && (toPort != null)) && ((fromPort.DiagramShape != null) && (toPort.DiagramShape != null)))
            {
                Shapes.IDiagramLine link1 = (Shapes.IDiagramLine)Activator.CreateInstance(this.LineType);
                if ((link1 != null) && (link1.DiagramShape != null))
                {
                    link1.FromPort = fromPort;
                    link1.ToPort = toPort;
                    Shapes.SubGraphNode.ReparentToCommonSubGraph(link1.DiagramShape, fromPort.DiagramShape, toPort.DiagramShape, true, this.Document.LinksLayer);
                    return link1;
                }
            }
            return null;
        }

        public virtual DiagramSelection CreateSelection()
        {
            return new DiagramSelection(this);
        }

        public virtual void DeleteSelection(DiagramSelection sel)
        {
            if (sel == null)
            {
                sel = this.Selection;
            }
            if (((sel != this.Selection) || this.CanDeleteObjects()) && !sel.IsEmpty)
            {
                string text1 = null;
                try
                {
                    this.StartTransaction();
                    CancelEventArgs args1 = new CancelEventArgs();
                    this.RaiseSelectionDeleting(args1);
                    if (args1.Cancel)
                    {
                        return;
                    }
                    Shapes.DiagramShape[] objArray1 = sel.CopyArray();
                    int num1 = objArray1.Length;
                    for (int num2 = num1 - 1; num2 >= 0; num2--)
                    {
                        Shapes.DiagramShape obj1 = objArray1[num2];
                        if ((obj1 != null) && obj1.CanDelete())
                        {
                            obj1.Remove();
                            sel.Remove(obj1);
                        }
                    }
                    text1 = "Delete Selection";
                    this.RaiseSelectionDeleted();
                }
                finally
                {
                    this.FinishTransaction(text1);
                }
            }
        }

        public virtual void DetectHover(Point viewPnt)
        {
            if (this.myHoverTimer == null)
            {
                this.myHoverTimer = new System.Threading.Timer(new TimerCallback(this.hoverCallback), new EventHandler(this.hoverTick), -1, -1);
                this.myHoverTimerEnabled = false;
            }
            if (this.myHoverPoint != viewPnt)
            {
                this.StopHoverTimer();
            }
            if (!this.myHoverTimerEnabled)
            {
                this.myHoverTimer.Change(this.HoverDelay, -1);
                this.myHoverTimerEnabled = true;
            }
            this.myHoverPoint = viewPnt;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this.myAutoScrollTimer != null)
            {
                this.myAutoScrollTimer.Dispose();
                this.myAutoScrollTimer = null;
            }
            if (this.myHoverTimer != null)
            {
                this.myHoverTimer.Dispose();
                this.myHoverTimer = null;
            }
            if (this.myCurrentResult != null)
            {
                this.myCurrentResult.Dispose();
                this.myCurrentResult = null;
            }
            if (this.myModalControl != null)
            {
                this.myModalControl.Dispose();
                this.myModalControl = null;
            }
            this.myDocument.Changed -= this.myDocChangedEventHandler;
            if (this.myBuffer != null)
            {
                this.myBuffer.Dispose();
                this.myBuffer = null;
            }
            if (this.myBackgroundBrush != null)
            {
                this.myBackgroundBrush.Dispose();
                this.myBackgroundBrush = null;
            }
            if (this.myGridPen != null)
            {
                this.myGridPen.Dispose();
                this.myGridPen = null;
            }
            if (this.myShadowBrush != null)
            {
                this.myShadowBrush.Dispose();
                this.myShadowBrush = null;
            }
            if (this.myShadowPen != null)
            {
                this.myShadowPen.Dispose();
                this.myShadowPen = null;
            }
        }

        public virtual void DoAutoPan(Point originPnt, Point viewPnt)
        {
            this.myPanning = true;
            this.myPanningOrigin = originPnt;
            this.myAutoScrollPoint = viewPnt;
            this.DoInternalAutoScroll();
        }

        public virtual void DoAutoScroll(Point viewPnt)
        {
            this.myPanning = false;
            this.myAutoScrollPoint = viewPnt;
            this.DoInternalAutoScroll();
        }

        public virtual void DoBackgroundMouseOver(InputEventArgs evt)
        {
            System.Windows.Forms.Cursor cursor1 = this.DefaultCursor;
            if (this.Cursor != cursor1)
            {
                this.Cursor = cursor1;
            }
        }

        public virtual void DoCancelMouse()
        {
            this.myCancelMouseDown = true;
            this.Tool.DoCancelMouse();
        }

        public virtual bool DoContextClick(InputEventArgs evt)
        {
            Shapes.DiagramShape obj1 = this.PickObject(true, false, evt.DocPoint, false);
            if (obj1 != null)
            {
                this.RaiseShapeContextClicked(obj1, evt);
                while (obj1 != null)
                {
                    if (obj1.OnContextClick(evt, this))
                    {
                        return true;
                    }
                    obj1 = obj1.Parent;
                }
            }
            else
            {
                this.RaiseBackgroundContextClicked(evt);
            }
            return false;
        }

        public virtual bool DoDoubleClick(InputEventArgs evt)
        {
            Shapes.DiagramShape obj1 = this.PickObject(true, false, evt.DocPoint, false);
            if (obj1 != null)
            {
                this.RaiseShapeDoubleClicked(obj1, evt);
                while (obj1 != null)
                {
                    if (obj1.OnDoubleClick(evt, this))
                    {
                        return true;
                    }
                    obj1 = obj1.Parent;
                }
            }
            else
            {
                this.RaiseBackgroundDoubleClicked(evt);
            }
            return false;
        }

        public virtual void DoEndEdit()
        {
            Shapes.DiagramControl control1 = this.EditControl;
            if (control1 != null)
            {
                control1.DoEndEdit(this);
            }
        }

        protected virtual void DoExternalDrag(DragEventArgs evt)
        {
            this.FollowExternalDragImage(this.LastInput.DocPoint);
            if (this.CanInsertObjects())
            {
                evt.Effect = DragDropEffects.Move | (DragDropEffects.Copy | DragDropEffects.Scroll);
                this.DoAutoScroll(this.LastInput.ViewPoint);
            }
            else
            {
                evt.Effect = DragDropEffects.None;
            }
        }

        protected virtual Shapes.IDiagramShapeCollection DoExternalDrop(DragEventArgs evt)
        {
            DiagramSelection selection1 = evt.Data.GetData(typeof(DiagramSelection)) as DiagramSelection;
            if (selection1 != null)
            {
                DiagramDocument document1 = this.Document;
                if (document1 != null)
                {
                    PointF tf1 = this.LastInput.DocPoint;
                    Shapes.DiagramShape obj2 = selection1.Primary;
                    if (obj2 != null)
                    {
                        string text1 = null;
                        Shapes.DiagramShapeCollection collection1 = new Shapes.DiagramShapeCollection();
                        try
                        {
                            this.StartTransaction();
                            SizeF ef1 = DiagramTool.SubtractPoints(tf1, new PointF(obj2.Left + selection1.HotSpot.Width, obj2.Top + selection1.HotSpot.Height));
                            CopyDictionary dictionary1 = document1.CopyFromCollection(selection1, false, true, ef1, null);
                            foreach (Shapes.DiagramShape obj4 in dictionary1.Values)
                            {
                                if (((obj4 == null) || !obj4.IsTopLevel) || (obj4.Document != document1))
                                {
                                    continue;
                                }
                                collection1.Add(obj4);
                                if (this.GridSnapDrag != DiagramViewSnapStyle.None)
                                {
                                    PointF tf2 = obj4.Location;
                                    PointF tf3 = this.FindNearestGridPoint(tf2);
                                    obj4.DoMove(this, tf2, tf3);
                                }
                            }
                            DiagramSelection selection2 = this.Selection;
                            selection2.Clear();
                            Shapes.DiagramShape obj5 = (Shapes.DiagramShape)dictionary1[obj2];
                            if ((obj5 != null) && collection1.Contains(obj5))
                            {
                                selection2.Add(obj5);
                            }
                            CollectionEnumerator enumerator2 = collection1.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                Shapes.DiagramShape obj6 = enumerator2.Current;
                                selection2.Add(obj6);
                            }
                            selection2.HotSpot = selection1.HotSpot;
                            text1 = "Drop";
                            this.RaiseExternalObjectsDropped(this.LastInput);
                        }
                        finally
                        {
                            this.FinishTransaction(text1);
                        }
                        return collection1;
                    }
                }
            }
            return null;
        }

        public virtual bool DoHover(InputEventArgs evt)
        {
            Shapes.DiagramShape obj1 = this.PickObject(true, false, evt.DocPoint, false);
            if (obj1 != null)
            {
                this.RaiseShapeHover(obj1, evt);
                while (obj1 != null)
                {
                    if (obj1.OnHover(evt, this))
                    {
                        return true;
                    }
                    obj1 = obj1.Parent;
                }
            }
            else
            {
                this.RaiseBackgroundHover(evt);
            }
            return false;
        }

        private void DoInternalAutoScroll()
        {
            if (this.myAutoScrollTimer == null)
            {
                this.myAutoScrollTimer = new System.Threading.Timer(new TimerCallback(this.autoScrollCallback), new EventHandler(this.autoScrollTick), -1, -1);
                this.myAutoScrollTimerEnabled = false;
            }
            PointF tf1 = this.myPanning ? this.ComputeAutoPanDocPosition(this.myPanningOrigin, this.myAutoScrollPoint) : this.ComputeAutoScrollDocPosition(this.myAutoScrollPoint);
            if (tf1 != this.DocPosition)
            {
                if (!this.myAutoScrollTimerEnabled)
                {
                    if (!this.Focused)
                    {
                        this.myAutoScrollTimer.Change(this.AutoScrollDelay, -1);
                    }
                    else
                    {
                        this.myAutoScrollTimer.Change(this.AutoScrollTime, -1);
                    }
                    this.myAutoScrollTimerEnabled = true;
                }
            }
            else if (!this.myPanning)
            {
                this.StopAutoScroll();
            }
        }

        protected virtual void DoInternalDrag(DragEventArgs evt)
        {
            this.DoMouseMove();
        }

        protected virtual void DoInternalDrop(DragEventArgs evt)
        {
            this.DoMouseUp();
        }

        public virtual void DoKeyDown()
        {
            this.Tool.DoKeyDown();
        }

        public virtual void DoMouseDown()
        {
            bool flag1 = this.Focused;
            this.InitFocus();
            if (flag1 || !this.myCancelMouseDown)
            {
                this.Tool.DoMouseDown();
            }
            this.myCancelMouseDown = false;
        }

        public virtual void DoMouseHover()
        {
            this.Tool.DoMouseHover();
        }

        public virtual void DoMouseMove()
        {
            this.Tool.DoMouseMove();
        }

        public virtual bool DoMouseOver(InputEventArgs evt)
        {
            Shapes.DiagramShape obj1 = this.PickObject(true, true, evt.DocPoint, false);
            this.DoToolTipObject(obj1);
            bool flag1 = false;
            while (obj1 != null)
            {
                if (obj1.OnMouseOver(evt, this))
                {
                    flag1 = true;
                    break;
                }
                obj1 = obj1.Parent;
            }
            if (!flag1)
            {
                this.DoBackgroundMouseOver(evt);
            }
            this.DetectHover(evt.ViewPoint);
            return flag1;
        }

        public virtual void DoMouseUp()
        {
            this.Tool.DoMouseUp();
        }

        public virtual void DoMouseWheel()
        {
            this.Tool.DoMouseWheel();
        }

        public virtual bool DoSingleClick(InputEventArgs evt)
        {
            Shapes.DiagramShape obj1 = this.PickObject(true, false, evt.DocPoint, false);
            if (obj1 != null)
            {
                this.RaiseShapeSingleClicked(obj1, evt);
                while (obj1 != null)
                {
                    if (obj1.OnSingleClick(evt, this))
                    {
                        return true;
                    }
                    obj1 = obj1.Parent;
                }
            }
            else
            {
                this.RaiseBackgroundSingleClicked(evt);
            }
            return false;
        }

        public virtual void DoToolTipObject(Shapes.DiagramShape obj)
        {
            if (this.ToolTip != null)
            {
                string text1 = this.ToolTip.GetToolTip(this);
                string text2 = null;
                while (obj != null)
                {
                    text2 = obj.GetToolTip(this);
                    if (text2 != null)
                    {
                        break;
                    }
                    obj = obj.Parent;
                }
                if (text2 == null)
                {
                    text2 = "";
                }
                if (text2 != text1)
                {
                    this.ToolTip.SetToolTip(this, text2);
                }
            }
        }

        public virtual void DoWheel(InputEventArgs evt)
        {
            if (evt.Delta != 0)
            {
                if (evt.Control)
                {
                    PointF tf1 = this.DocPosition;
                    this.DocScale *= (1f + (((float)evt.Delta) / 2400f));
                    PointF tf2 = this.ConvertViewToDoc(evt.ViewPoint);
                    this.DocPosition = new PointF((tf1.X + evt.DocPoint.X) - tf2.X, (tf1.Y + evt.DocPoint.Y) - tf2.Y);
                }
                else
                {
                    int num1 = -evt.Delta / 120;
                    if (evt.Shift)
                    {
                        this.ScrollLine((float)num1, 0f);
                    }
                    else
                    {
                        this.ScrollLine(0f, (float)num1);
                    }
                }
            }
        }

        protected virtual void DrawGridCrosses(Graphics g, SizeF cross, RectangleF clipRect)
        {
            float single1 = this.GridCellSize.Width;
            float single2 = this.GridCellSize.Height;
            Color color1 = this.GridColor;
            if (color1 == Color.Empty)
            {
                color1 = this.ForeColor;
            }
            float single3 = this.GridPenWidth;
            if (((this.myGridPen == null) || (this.myGridPen.Color != color1)) || ((this.myGridPen.Width != single3) || (this.myGridPen.DashStyle != DashStyle.Solid)))
            {
                if (this.myGridPen != null)
                {
                    this.myGridPen.Dispose();
                }
                this.myGridPen = new Pen(color1, single3);
            }
            float single4 = clipRect.X - single1;
            float single5 = clipRect.Y - single2;
            float single6 = (clipRect.X + clipRect.Width) + single1;
            float single7 = (clipRect.Y + clipRect.Height) + single2;
            PointF tf1 = this.FindNearestGridPoint(new PointF(single4, single5));
            PointF tf2 = this.FindNearestGridPoint(new PointF(single6, single7));
            if ((cross.Height < 2f) && (cross.Width < 2f))
            {
                float single8 = 1f;
                for (float single9 = tf1.X; single9 < tf2.X; single9 += single1)
                {
                    for (float single10 = tf1.Y; single10 < tf2.Y; single10 += single2)
                    {
                        Shapes.DiagramGraph.DrawEllipse(g, this, this.myGridPen, null, single9, single10, single8, single8);
                    }
                }
            }
            else
            {
                for (float single11 = tf1.X; single11 < tf2.X; single11 += single1)
                {
                    for (float single12 = tf1.Y; single12 < tf2.Y; single12 += single2)
                    {
                        Shapes.DiagramGraph.DrawLine(g, this, this.myGridPen, single11, single12 - (cross.Height / 2f), single11, single12 + (cross.Height / 2f));
                        Shapes.DiagramGraph.DrawLine(g, this, this.myGridPen, single11 - (cross.Width / 2f), single12, single11 + (cross.Width / 2f), single12);
                    }
                }
            }
        }

        protected virtual void DrawGridLines(Graphics g, RectangleF clipRect)
        {
            float single1 = this.GridCellSize.Width;
            float single2 = this.GridCellSize.Height;
            Color color1 = this.GridColor;
            float single3 = this.GridPenWidth;
            DashStyle style1 = this.GridPenDashStyle;
            if (color1 == Color.Empty)
            {
                color1 = this.ForeColor;
            }
            if (((this.myGridPen == null) || (this.myGridPen.Color != color1)) || ((this.myGridPen.Width != single3) || (this.myGridPen.DashStyle != style1)))
            {
                if (this.myGridPen != null)
                {
                    this.myGridPen.Dispose();
                }
                this.myGridPen = new Pen(color1, single3);
                this.myGridPen.DashStyle = style1;
            }
            float single4 = clipRect.X - single1;
            float single5 = clipRect.Y - single2;
            float single6 = (clipRect.X + clipRect.Width) + single1;
            float single7 = (clipRect.Y + clipRect.Height) + single2;
            PointF tf1 = this.FindNearestGridPoint(new PointF(single4, single5));
            PointF tf2 = this.FindNearestGridPoint(new PointF(single6, single7));
            for (float single8 = tf1.X; single8 < tf2.X; single8 += single1)
            {
                Shapes.DiagramGraph.DrawLine(g, this, this.myGridPen, single8, clipRect.Y, single8, clipRect.Y + clipRect.Height);
            }
            for (float single9 = tf1.Y; single9 < tf2.Y; single9 += single2)
            {
                Shapes.DiagramGraph.DrawLine(g, this, this.myGridPen, clipRect.X, single9, clipRect.X + clipRect.Width, single9);
            }
        }

        public virtual void DrawXorBox(Rectangle rect, bool drawnew)
        {
            if (this.myPrevXorRectValid)
            {
                try
                {
                    this.DrawXorRectangle(this.myPrevXorRect);
                }
                catch (SecurityException)
                {
                    this.Refresh();
                }
                this.myPrevXorRectValid = false;
            }
            if (drawnew)
            {
                try
                {
                    this.DrawXorRectangle(rect);
                }
                catch (SecurityException)
                {
                    Graphics graphics1 = base.CreateGraphics();
                    graphics1.DrawRectangle(Shapes.DiagramGraph.Pens_Gray, rect.X, rect.Y, rect.Width, rect.Height);
                    graphics1.Dispose();
                }
                this.myPrevXorRect = rect;
                this.myPrevXorRectValid = true;
            }
        }

        public void DrawXorLine(int ax, int ay, int bx, int by)
        {
            Point point1 = new Point(ax, ay);
            Point point2 = new Point(bx, by);
            Point point3 = base.PointToScreen(point1);
            Point point4 = base.PointToScreen(point2);
            Color color1 = this.Document.PaperColor;
            if (color1 == Color.Empty)
            {
                color1 = this.BackColor;
            }
            ControlPaint.DrawReversibleLine(point3, point4, color1);
        }

        public void DrawXorRectangle(Rectangle rect)
        {
            Rectangle rectangle1 = base.RectangleToScreen(rect);
            Color color1 = this.Document.PaperColor;
            if (color1 == Color.Empty)
            {
                color1 = this.BackColor;
            }
            ControlPaint.DrawReversibleFrame(rectangle1, color1, FrameStyle.Thick);
        }

        public virtual void EditCopy()
        {
            if (this.CanCopyObjects())
            {
                string text1 = null;
                try
                {
                    this.StartTransaction();
                    this.CopyToClipboard(this.Selection);
                    text1 = "Copy";
                }
                catch (Exception exception1)
                {
                    Shapes.DiagramShape.Trace("EditCopy: " + exception1.ToString());
                    throw exception1;
                }
                finally
                {
                    this.FinishTransaction(text1);
                }
            }
        }

        public virtual void EditCut()
        {
            if (this.CanCopyObjects() && this.CanDeleteObjects())
            {
                string text1 = null;
                try
                {
                    this.StartTransaction();
                    this.CopyToClipboard(this.Selection);
                    this.DeleteSelection(this.Selection);
                    text1 = "Cut";
                }
                catch (Exception exception1)
                {
                    Shapes.DiagramShape.Trace("EditCut: " + exception1.ToString());
                    throw exception1;
                }
                finally
                {
                    this.FinishTransaction(text1);
                }
            }
        }

        public virtual void EditDelete()
        {
            this.DeleteSelection(this.Selection);
        }

        public virtual void EditEdit()
        {
            this.EditObject(this.Selection.Primary);
        }

        public virtual void EditObject(Shapes.DiagramShape obj)
        {
            if (((obj != null) && this.CanEditObjects()) && obj.CanEdit())
            {
                obj.DoBeginEdit(this);
            }
        }

        public virtual void EditPaste()
        {
            if (this.CanInsertObjects())
            {
                DiagramDocument document1 = this.Document;
                string text1 = null;
                try
                {
                    this.StartTransaction();
                    CopyDictionary dictionary1 = this.PasteFromClipboard();
                    if (dictionary1 != null)
                    {
                        bool flag1 = false;
                        IDictionaryEnumerator enumerator1 = dictionary1.GetEnumerator();
                        while (enumerator1.MoveNext())
                        {
                            Shapes.DiagramShape obj1 = enumerator1.Key as Shapes.DiagramShape;
                            if ((obj1 != null) && obj1.IsTopLevel)
                            {
                                Shapes.DiagramShape obj2 = enumerator1.Value as Shapes.DiagramShape;
                                if (((obj2 != null) && obj2.IsTopLevel) && (obj2.Document == document1))
                                {
                                    if (!flag1)
                                    {
                                        flag1 = true;
                                        this.Selection.Clear();
                                    }
                                    this.Selection.Add(obj2);
                                }
                            }
                        }
                    }
                    text1 = "Paste";
                    this.RaiseClipboardPasted();
                }
                catch (Exception exception1)
                {
                    Shapes.DiagramShape.Trace("EditPaste: " + exception1.ToString());
                    throw exception1;
                }
                finally
                {
                    this.FinishTransaction(text1);
                }
            }
        }

        public virtual void EndUpdate()
        {
            if (this.mySuppressPaint > 0)
            {
                this.mySuppressPaint--;
                base.Update();
            }
        }

        public virtual IDiagramTool FindMouseTool(Type tooltype)
        {
            return this.FindMouseTool(tooltype, false);
        }

        internal IDiagramTool FindMouseTool(Type tooltype, bool subclass)
        {
            IList list1 = this.MouseDownTools;
            int num1 = 0;
            while (num1 < list1.Count)
            {
                if ((list1[num1].GetType() == tooltype) || (subclass && list1[num1].GetType().IsSubclassOf(tooltype)))
                {
                    return (IDiagramTool)list1[num1];
                }
                num1++;
            }
            list1 = this.MouseMoveTools;
            num1 = 0;
            while (num1 < list1.Count)
            {
                if ((list1[num1].GetType() == tooltype) || (subclass && list1[num1].GetType().IsSubclassOf(tooltype)))
                {
                    return (IDiagramTool)list1[num1];
                }
                num1++;
            }
            list1 = this.MouseUpTools;
            for (num1 = 0; num1 < list1.Count; num1++)
            {
                if ((list1[num1].GetType() == tooltype) || (subclass && list1[num1].GetType().IsSubclassOf(tooltype)))
                {
                    return (IDiagramTool)list1[num1];
                }
            }
            return null;
        }

        public virtual PointF FindNearestGridPoint(PointF p)
        {
            float single1 = p.X;
            float single2 = p.Y;
            float single3 = this.GridOrigin.X;
            float single4 = this.GridOrigin.Y;
            float single5 = this.GridCellSize.Width;
            float single6 = this.GridCellSize.Height;
            float single7 = single1 - single3;
            single7 = (((float)System.Math.Floor((double)(single7 / single5))) * single5) + single3;
            float single8 = single2 - single4;
            single8 = (((float)System.Math.Floor((double)(single8 / single6))) * single6) + single4;
            float single9 = ((single1 - single7) * (single1 - single7)) + ((single2 - single8) * (single2 - single8));
            float single10 = single7;
            float single11 = single8;
            float single12 = single7 + single5;
            float single13 = single8;
            float single14 = ((single1 - single12) * (single1 - single12)) + ((single2 - single13) * (single2 - single13));
            if (single14 < single9)
            {
                single9 = single14;
                single10 = single12;
                single11 = single13;
            }
            float single15 = single7;
            float single16 = single8 + single6;
            float single17 = ((single1 - single15) * (single1 - single15)) + ((single2 - single16) * (single2 - single16));
            if (single17 < single9)
            {
                single9 = single17;
                single10 = single15;
                single11 = single16;
            }
            float single18 = single12;
            float single19 = single16;
            float single20 = ((single1 - single18) * (single1 - single18)) + ((single2 - single19) * (single2 - single19));
            if (single20 < single9)
            {
                single10 = single18;
                single11 = single19;
            }
            return new PointF(single10, single11);
        }

        public virtual bool FinishTransaction(string tname)
        {
            return this.Document.FinishTransaction(tname);
        }

        private void FollowExternalDragImage(PointF pt)
        {
            if (this.myExternalDragImage != null)
            {
                this.myExternalDragImage.Location = pt;
            }
        }

        internal void FreeTempPointArray(PointF[] a)
        {
            int num1 = a.Length;
            if ((this.myTempArrays != null) && (num1 < this.myTempArrays.Length))
            {
                this.myTempArrays[num1] = a;
            }
        }

        public Bitmap GetBitmapFromCollection(Shapes.IDiagramShapeCollection coll)
        {
            RectangleF ef1 = DiagramDocument.ComputeBounds(coll, this);
            return this.GetBitmapFromCollection(coll, ef1, true);
        }

        public Bitmap GetBitmapFromCollection(Shapes.IDiagramShapeCollection coll, RectangleF bounds, bool paper)
        {
            float single1 = 1f;
            float single2 = 2000f / this.WorldScale.Width;
            float single3 = 2000f / this.WorldScale.Height;
            if ((bounds.Width > single2) || (bounds.Height > single3))
            {
                single1 = System.Math.Min((float)(single2 / bounds.Width), (float)(single3 / bounds.Height));
            }
            return this.GetBitmapFromCollection(coll, bounds, single1, paper);
        }

        public virtual Bitmap GetBitmapFromCollection(Shapes.IDiagramShapeCollection coll, RectangleF bounds, float scale, bool paper)
        {
            if (scale < 9E-09f)
            {
                scale = 9E-09f;
            }
            int num1 = (int)System.Math.Ceiling((double)(bounds.Width * scale));
            int num2 = (int)System.Math.Ceiling((double)(bounds.Height * scale));
            if (num1 < 1)
            {
                num1 = 1;
            }
            if (num2 < 1)
            {
                num2 = 1;
            }
            Bitmap bitmap1 = new Bitmap(num1, num2);
            Graphics graphics1 = Graphics.FromImage(bitmap1);
            graphics1.PageUnit = GraphicsUnit.Pixel;
            graphics1.SmoothingMode = this.SmoothingMode;
            graphics1.TextRenderingHint = this.TextRenderingHint;
            graphics1.InterpolationMode = this.InterpolationMode;
            graphics1.ScaleTransform(scale, scale);
            graphics1.TranslateTransform(-bounds.X, -bounds.Y);
            PointF tf1 = this.myOrigin;
            float single1 = this.myHorizScale;
            float single2 = this.myVertScale;
            Size size1 = this.myBorderSize;
            this.myOrigin = new PointF(bounds.X, bounds.Y);
            this.myHorizScale = scale;
            this.myVertScale = scale;
            this.myBorderSize = new Size(0, 0);
            try
            {
                if (paper)
                {
                    RectangleF ef1 = bounds;
                    Shapes.DiagramShape.InflateRect(ref ef1, 1f, 1f);
                    this.PaintPaperColor(graphics1, ef1);
                }
                foreach (Shapes.DiagramShape obj1 in coll)
                {
                    if (obj1.CanView())
                    {
                        obj1.Paint(graphics1, this);
                    }
                }
            }
            finally
            {
                this.myOrigin = tf1;
                this.myHorizScale = single1;
                this.myVertScale = single2;
                this.myBorderSize = size1;
            }
            graphics1.Dispose();
            return bitmap1;
        }

        protected virtual Shapes.DiagramShape GetExternalDragImage(DragEventArgs evt)
        {
            DiagramSelection selection1 = evt.Data.GetData(typeof(DiagramSelection)) as DiagramSelection;
            if (selection1 == null)
            {
                return null;
            }
            Shapes.DiagramShape obj1 = selection1.Primary;
            DiagramSelection selection2 = selection1;
            ToolDragging dragging1 = this.FindMouseTool(typeof(ToolDragging), true) as ToolDragging;
            if (dragging1 != null)
            {
                selection2 = dragging1.ComputeEffectiveSelection(selection1, false);
            }
            RectangleF ef1 = DiagramDocument.ComputeBounds(selection2, null);
            Image image1 = this.GetBitmapFromCollection(selection2, ef1, false);
            ExternalDragImage image2 = new ExternalDragImage();
            image2.Image = image1;
            SizeF ef2 = DiagramTool.SubtractPoints(obj1.Position, ef1.Location);
            image2.Offset = new SizeF(ef2.Width + selection1.HotSpot.Width, ef2.Height + selection1.HotSpot.Height);
            return image2;
        }

        public virtual SolidBrush GetShadowBrush()
        {
            if ((this.myShadowBrush == null) || (this.myShadowBrush.Color != this.ShadowColor))
            {
                if (this.myShadowBrush != null)
                {
                    this.myShadowBrush.Dispose();
                }
                this.myShadowBrush = new SolidBrush(this.ShadowColor);
            }
            return this.myShadowBrush;
        }

        public virtual Pen GetShadowPen(float width)
        {
            if (((this.myShadowPen == null) || (this.myShadowPen.Color != this.ShadowColor)) || (Shapes.DiagramGraph.GetPenWidth(this.myShadowPen) != width))
            {
                if (this.myShadowPen != null)
                {
                    this.myShadowPen.Dispose();
                }
                this.myShadowPen = new Pen(this.ShadowColor, width);
            }
            return this.myShadowPen;
        }

        public virtual void HandleScroll(object sender, ScrollEventArgs e)
        {
            if (e.Type != ScrollEventType.EndScroll)
            {
                int num1 = e.NewValue;
                this.InitFocus();
                PointF tf1 = this.DocPosition;
                if (sender == this.VerticalScrollBar)
                {
                    tf1.Y = (((float)num1) / this.myVertWorld) / this.myVertScale;
                    this.DocPosition = tf1;
                }
                else if (sender == this.HorizontalScrollBar)
                {
                    tf1.X = (((float)num1) / this.myHorizWorld) / this.myHorizScale;
                    this.DocPosition = tf1;
                }
            }
        }

        private void HideExternalDragImage()
        {
            if (this.myExternalDragImage != null)
            {
                this.myExternalDragImage.Remove();
                this.myExternalDragImage = null;
            }
        }

        private void hoverCallback(object obj)
        {
            if (base.IsHandleCreated)
            {
                base.Invoke((EventHandler)obj);
            }
        }

        private void hoverTick(object sender, EventArgs e)
        {
            if (this.myHoverTimerEnabled)
            {
                InputEventArgs args1 = this.LastInput;
                args1.ViewPoint = this.myHoverPoint;
                args1.DocPoint = this.ConvertViewToDoc(args1.ViewPoint);
                args1.Buttons = Control.MouseButtons;
                args1.Modifiers = Control.ModifierKeys;
                args1.Delta = 0;
                args1.Key = Keys.None;
                this.DoMouseHover();
            }
        }

        private void init(DiagramDocument doc)
        {
            //modified by little
            //this.myCurrentResult = (GoViewLicenseProvider.GoViewLicense) LicenseManager.Validate(typeof(DiagramView), this);
            this.myDocChangedEventHandler = new ChangedEventHandler(this.SafeOnDocumentChanged);
            this.myDocument = doc;
            this.myLayers = new LayerCollection();
            this.myLayers.init(this);
            if (this.myDocument == null)
            {
                this.myDocument = this.CreateDocument();
            }
            this.myDocument.Changed += this.myDocChangedEventHandler;
            this.InitializeLayersFromDocument();
            this.mySelection = this.CreateSelection();
            this.myDefaultTool = this.CreateDefaultTool();
            this.myTool = this.DefaultTool;
            this.myTool.Start();
            base.SetStyle(ControlStyles.AllPaintingInWmPaint | (ControlStyles.ResizeRedraw | (ControlStyles.Opaque | ControlStyles.UserPaint)), true);
            this.myVertScroll = new VScrollBar();
            this.myHorizScroll = new HScrollBar();
            this.myCorner = new Control();
            this.myCorner.BackColor = SystemColors.Control;
            base.Controls.Add(this.myVertScroll);
            base.Controls.Add(this.myHorizScroll);
            base.Controls.Add(this.myCorner);
            this.myVertScroll.SmallChange = this.ScrollSmallChange.Height;
            this.myHorizScroll.SmallChange = this.ScrollSmallChange.Width;
            this.myToolTip = new System.Windows.Forms.ToolTip();
            this.myVertScrollHandler = new ScrollEventHandler(this.HandleScroll);
            this.myVertScroll.Scroll += this.myVertScrollHandler;
            this.myHorizScrollHandler = new ScrollEventHandler(this.HandleScroll);
            this.myHorizScroll.Scroll += this.myHorizScrollHandler;
            this.myVertScroll.RightToLeft = RightToLeft.No;
            this.myHorizScroll.RightToLeft = RightToLeft.No;
            this.InitAllowDrop(true);
            this.BackColor = Color.White;

            this.Document.ShapeAdded += new SelectionEventHandler(DefaultLayer_ShapeAdded);
            this.Document.ShapeRemoved += new SelectionEventHandler(DefaultLayer_ShapeRemoved);
        }

        internal bool InitAllowDrop(bool dnd)
        {
            try
            {
                this.InitAllowDrop2(dnd);
            }
            catch (SecurityException exception1)
            {
                this.AllowDragOut = false;
                Shapes.DiagramShape.Trace("DiagramView.init: " + exception1.ToString());
                return false;
            }
            return true;
        }

        private void InitAllowDrop2(bool dnd)
        {
            this.AllowDrop = dnd;
        }

        internal void InitFocus()
        {
            try
            {
                this.InitFocus2();
            }
            catch (SecurityException exception1)
            {
                Shapes.DiagramShape.Trace("Focus: " + exception1.ToString());
                this.OnGotFocus(EventArgs.Empty);
            }
        }

        private void InitFocus2()
        {
            base.Focus();
        }

        public virtual void InitializeLayersFromDocument()
        {
            if (this.Layers != null)
            {
                this.BeginUpdate();
                DiagramLayer layer1 = this.Layers.Default;
                DiagramLayer[] layerArray1 = this.Layers.CopyArray();
                for (int num1 = 0; num1 < layerArray1.Length; num1++)
                {
                    DiagramLayer layer2 = layerArray1[num1];
                    if (layer2.IsInView)
                    {
                        layer2.Clear();
                    }
                    else
                    {
                        this.Layers.Remove(layer2);
                    }
                }
                this.DocPosition = new PointF();
                LayerCollectionEnumerator enumerator1 = this.Document.Layers.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramLayer layer3 = enumerator1.Current;
                    this.Layers.InsertDocumentLayerAfter(null, layer3);
                }
                this.Layers.MoveAfter(null, layer1);
                this.EndUpdate();
            }
        }

        private void InternalOnDocumentChanged(object sender, EventArgs e)
        {
            if (this.myQueuedEvents != null)
            {
                ChangedEventArgs args1 = null;
                lock (this.myQueuedEvents)
                {
                    args1 = this.myQueuedEvents.Dequeue() as ChangedEventArgs;
                }
                if (args1 != null)
                {
                    this.OnDocumentChanged(args1.Document, args1);
                }
            }
        }

        public virtual bool IsInternalDragDrop(DragEventArgs evt)
        {
            if (this.Tool is ToolDragging)
            {
                return !this.myPretendInternalDrag;
            }
            return false;
        }

        public virtual void LayoutScrollBars(bool update)
        {
            if (!this.myUpdatingScrollBars)
            {
                Rectangle rectangle1 = base.ClientRectangle;
                int num1 = rectangle1.Width - this.myBorderSize.Width;
                VScrollBar bar1 = this.VerticalScrollBar;
                if ((bar1 != null) && bar1.Visible)
                {
                    num1 -= this.myScrollBarWidth;
                }
                int num2 = rectangle1.Height - this.myBorderSize.Height;
                HScrollBar bar2 = this.HorizontalScrollBar;
                if ((bar2 != null) && bar2.Visible)
                {
                    num2 -= this.myScrollBarHeight;
                }
                Control control1 = this.CornerControl;
                if (control1 != null)
                {
                    if (((bar1 != null) && bar1.Visible) && ((bar2 != null) && bar2.Visible))
                    {
                        control1.Bounds = new Rectangle(num1, num2, this.myScrollBarWidth, this.myScrollBarHeight);
                        control1.Visible = true;
                    }
                    else
                    {
                        control1.Visible = false;
                    }
                }
                if ((bar1 != null) && bar1.Visible)
                {
                    bar1.Bounds = new Rectangle(num1, this.myBorderSize.Height, this.myScrollBarWidth, num2 - this.myBorderSize.Height);
                    bar1.LargeChange = System.Math.Max(this.ScrollSmallChange.Height, (int)(bar1.Height - this.ScrollSmallChange.Height));
                }
                if ((bar2 != null) && bar2.Visible)
                {
                    bar2.Bounds = new Rectangle(this.myBorderSize.Width, num2, num1 - this.myBorderSize.Width, this.myScrollBarHeight);
                    bar2.LargeChange = System.Math.Max(this.ScrollSmallChange.Width, (int)(bar2.Width - this.ScrollSmallChange.Width));
                }
                if (update)
                {
                    this.UpdateScrollBars();
                }
            }
        }

        public virtual PointF LimitDocPosition(PointF p)
        {
            PointF tf1 = this.DocumentTopLeft;
            SizeF ef1 = this.DocumentSize;
            SizeF ef2 = this.DocExtentSize;
            float single1 = (tf1.X + ef1.Width) - ef2.Width;
            if (single1 < tf1.X)
            {
                p.X = tf1.X;
            }
            else if ((p.X > single1) && (single1 > tf1.X))
            {
                p.X = single1;
            }
            else if (p.X < tf1.X)
            {
                p.X = tf1.X;
            }
            float single2 = (tf1.Y + ef1.Height) - ef2.Height;
            if (single2 < tf1.Y)
            {
                p.Y = tf1.Y;
                return p;
            }
            if ((p.Y > single2) && (single2 > tf1.Y))
            {
                p.Y = single2;
                return p;
            }
            if (p.Y < tf1.Y)
            {
                p.Y = tf1.Y;
            }
            return p;
        }

        public virtual float LimitDocScale(float s)
        {
            float single1 = this.WorldScale.Width;
            float single2 = 0.01f / single1;
            float single3 = 10f / single1;
            if (s < single2)
            {
                s = single2;
            }
            if (s > single3)
            {
                s = single3;
            }
            return s;
        }

        public virtual bool MatchesNodeLabel(Shapes.ITextNode node, char c)
        {
            if (node == null)
            {
                return false;
            }
            string text1 = node.Text;
            if (text1 == null)
            {
                return false;
            }
            if (text1.Length == 0)
            {
                return false;
            }
            CultureInfo info1 = CultureInfo.CurrentCulture;
            return (char.ToUpper(text1[0], info1) == char.ToUpper(c, info1));
        }

        public virtual void MoveSelection(DiagramSelection sel, SizeF offset, bool grid)
        {
            if (sel == null)
            {
                sel = this.Selection;
            }
            if (((sel != this.Selection) || this.CanMoveObjects()) && !sel.IsEmpty)
            {
                string text1 = null;
                try
                {
                    this.StartTransaction();
                    Shapes.DiagramShape obj1 = null;
                    CollectionEnumerator enumerator1 = sel.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        Shapes.DiagramShape obj2 = enumerator1.Current;
                        if (!(obj2 is Shapes.IDiagramLine))
                        {
                            obj1 = obj2;
                            break;
                        }
                    }
                    SizeF ef1 = offset;
                    if (obj1 != null)
                    {
                        PointF tf1 = obj1.Location;
                        PointF tf2 = new PointF(tf1.X + offset.Width, tf1.Y + offset.Height);
                        if (grid)
                        {
                            tf2 = this.FindNearestGridPoint(tf2);
                        }
                        ef1.Width = tf2.X - tf1.X;
                        ef1.Height = tf2.Y - tf1.Y;
                    }
                    enumerator1 = sel.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        Shapes.DiagramShape obj3 = enumerator1.Current;
                        if (obj3 is Shapes.IDiagramLine)
                        {
                            obj3.DoMove(this, obj3.Position, new PointF(obj3.Left + ef1.Width, obj3.Top + ef1.Height));
                        }
                    }
                    enumerator1 = sel.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        Shapes.DiagramShape obj4 = enumerator1.Current;
                        if (!(obj4 is Shapes.IDiagramLine))
                        {
                            PointF tf3 = obj4.Location;
                            PointF tf4 = new PointF(tf3.X + offset.Width, tf3.Y + offset.Height);
                            if (grid)
                            {
                                tf4 = this.FindNearestGridPoint(tf4);
                            }
                            obj4.DoMove(this, tf3, tf4);
                        }
                    }
                    text1 = "Move Selection";
                }
                finally
                {
                    this.FinishTransaction(text1);
                }
            }
        }

        protected override void OnBackColorChanged(EventArgs evt)
        {
            base.OnBackColorChanged(evt);
            this.UpdateView();
        }

        protected virtual void OnBackgroundContextClicked(InputEventArgs evt)
        {
            if (this.BackgroundContextClicked != null)
            {
                this.BackgroundContextClicked(this, evt);
            }
        }

        protected virtual void OnBackgroundDoubleClicked(InputEventArgs evt)
        {
            if (this.BackgroundDoubleClicked != null)
            {
                this.BackgroundDoubleClicked(this, evt);
            }
        }

        protected virtual void OnBackgroundHover(InputEventArgs evt)
        {
            if (this.BackgroundHover != null)
            {
                this.BackgroundHover(this, evt);
            }
        }

        protected override void OnBackgroundImageChanged(EventArgs evt)
        {
            base.OnBackgroundImageChanged(evt);
            this.UpdateView();
        }

        protected virtual void OnBackgroundSingleClicked(InputEventArgs evt)
        {
            if (this.BackgroundSingleClicked != null)
            {
                this.BackgroundSingleClicked(this, evt);
            }
        }

        protected virtual void OnClipboardPasted(EventArgs evt)
        {
            if (this.ClipboardPasted != null)
            {
                this.ClipboardPasted(this, evt);
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.myUpdatingScrollBars = false;
            this.LayoutScrollBars(true);
        }

        protected virtual void OnDocumentChanged(object sender, ChangedEventArgs e)
        {
            Rectangle rectangle2;
            Shapes.DiagramShape obj1 = e.DiagramShape;
            if (e.IsBeforeChanging)
            {
                if ((e.Hint == 0x385) && (obj1 != null))
                {
                    RectangleF ef1 = obj1.Bounds;
                    ef1 = obj1.ExpandPaintBounds(ef1, this);
                    Rectangle rectangle1 = this.ConvertDocToView(ef1);
                    rectangle1.Inflate(2, 2);
                    base.Invalidate(rectangle1);
                }
                goto Label_03B1;
            }
            int num1 = e.Hint;
            if (num1 <= 0xe2)
            {
                if (num1 <= 0xcd)
                {
                    switch (num1)
                    {
                        case 100:
                        case 0xcd:
                            {
                                goto Label_03A0;
                            }
                        case 0x65:
                            {
                                this.BeginUpdate();
                                goto Label_03B1;
                            }
                        case 0x66:
                            {
                                this.EndUpdate();
                                goto Label_03B1;
                            }
                        case 0x67:
                            {
                                base.Update();
                                goto Label_03B1;
                            }
                        case 0xca:
                            {
                                this.UpdateScrollBars();
                                goto Label_03B1;
                            }
                        case 0xcb:
                            {
                                this.UpdateScrollBars();
                                goto Label_03B1;
                            }
                        case 0xcc:
                            {
                                goto Label_03B1;
                            }
                    }
                    goto Label_03B1;
                }
                if (num1 == 220)
                {
                    goto Label_03A0;
                }
                if (num1 == 0xe2)
                {
                    DiagramDocument document1 = sender as DiagramDocument;
                    if (document1 != null)
                    {
                        SizeF ef5 = document1.WorldScale;
                        this.myHorizWorld = ef5.Width;
                        this.myVertWorld = ef5.Height;
                        this.UpdateView();
                    }
                }
                goto Label_03B1;
            }
            if (num1 <= 0x388)
            {
                switch (num1)
                {
                    case 0x321:
                        {
                            DiagramLayer layer1 = (DiagramLayer)e.Object;
                            DiagramLayer layer2 = (DiagramLayer)e.OldValue;
                            if (e.SubHint != 1)
                            {
                                this.Layers.InsertDocumentLayerBefore(layer2, layer1);
                                goto Label_0302;
                            }
                            this.Layers.InsertDocumentLayerAfter(layer2, layer1);
                            goto Label_0302;
                        }
                    case 0x322:
                        {
                            DiagramLayer layer3 = (DiagramLayer)e.Object;
                            this.Layers.Remove(layer3);
                            this.Selection.AddAllSelectionHandles();
                            this.UpdateView();
                            goto Label_03B1;
                        }
                    case 0x323:
                        {
                            DiagramLayer layer4 = (DiagramLayer)e.Object;
                            DiagramLayer layer5 = (DiagramLayer)e.OldValue;
                            try
                            {
                                if (e.SubHint == 1)
                                {
                                    this.Layers.MoveAfter(layer5, layer4);
                                }
                                else
                                {
                                    this.Layers.MoveBefore(layer5, layer4);
                                }
                            }
                            catch (ArgumentException)
                            {
                            }
                            this.Selection.AddAllSelectionHandles();
                            this.UpdateView();
                            goto Label_03B1;
                        }
                    case 0x385:
                        {
                            RectangleF ef2 = obj1.Bounds;
                            ef2 = obj1.ExpandPaintBounds(ef2, this);
                            rectangle2 = this.ConvertDocToView(ef2);
                            rectangle2.Inflate(2, 2);
                            if (e.SubHint != 0x3e9)
                            {
                                if (e.SubHint == 0x3eb)
                                {
                                    this.updateSelectionHandles(obj1);
                                }
                                else if (e.SubHint == 0x41c)
                                {
                                    this.removeFromSelection(e.OldValue as Shapes.DiagramShape);
                                }
                                goto Label_01F7;
                            }
                            DiagramSelection selection1 = this.Selection;
                            if (selection1.GetHandleCount(obj1) > 0)
                            {
                                if (!obj1.CanView())
                                {
                                    obj1.RemoveSelectionHandles(selection1);
                                }
                                else
                                {
                                    IShapeHandle handle1 = selection1.GetAnExistingHandle(obj1);
                                    obj1.AddSelectionHandles(selection1, handle1.SelectedObject);
                                }
                            }
                            RectangleF ef3 = e.OldRect;
                            ef3 = obj1.ExpandPaintBounds(ef3, this);
                            Rectangle rectangle3 = this.ConvertDocToView(ef3);
                            rectangle3.Inflate(2, 2);
                            base.Invalidate(rectangle3);
                            goto Label_01F7;
                        }
                    case 0x386:
                    case 0x387:
                    case 0x388:
                        {
                            if (e.Hint == 0x387)
                            {
                                this.removeFromSelection(obj1);
                            }
                            RectangleF ef4 = obj1.Bounds;
                            ef4 = obj1.ExpandPaintBounds(ef4, this);
                            Rectangle rectangle4 = this.ConvertDocToView(ef4);
                            rectangle4.Inflate(2, 2);
                            base.Invalidate(rectangle4);
                            goto Label_03B1;
                        }
                }
                goto Label_03B1;
            }
            if (num1 == 910)
            {
                goto Label_03A0;
            }
            if (num1 != 930)
            {
            }
            goto Label_03B1;
        Label_01F7:
            base.Invalidate(rectangle2);
            goto Label_03B1;
        Label_0302:
            this.Selection.AddAllSelectionHandles();
            this.UpdateView();
            goto Label_03B1;
        Label_03A0:
            this.Selection.AddAllSelectionHandles();
            this.UpdateView();
        Label_03B1:
            if (this.DocumentChanged != null)
            {
                this.DocumentChanged(sender, e);
            }
        }

        protected override void OnDoubleClick(EventArgs evt)
        {
            InputEventArgs args1 = this.LastInput;
            if (this.AllowMouse)
            {
                args1.MouseEventArgs = new MouseEventArgs(args1.Buttons, 2, args1.ViewPoint.X, args1.ViewPoint.Y, args1.Delta);
                args1.DoubleClick = true;
                this.DoMouseUp();
            }
            base.OnDoubleClick(evt);
            args1.DoubleClick = false;
            args1.MouseEventArgs = null;
        }

        protected override void OnDragDrop(DragEventArgs evt)
        {
            InputEventArgs args1 = this.LastInput;
            if (this.AllowMouse)
            {
                try
                {
                    Point point1 = new Point(evt.X, evt.Y);
                    args1.ViewPoint = base.PointToClient(point1);
                    args1.DocPoint = this.ConvertViewToDoc(args1.ViewPoint);
                    args1.Buttons = Control.MouseButtons;
                    args1.Modifiers = Control.ModifierKeys;
                    args1.Delta = 0;
                    args1.Key = Keys.None;
                    args1.DragEventArgs = evt;
                    if (this.IsInternalDragDrop(evt))
                    {
                        this.DoInternalDrop(evt);
                    }
                    else
                    {
                        this.HideExternalDragImage();
                        if (this.myPretendInternalDrag)
                        {
                            this.DoMouseUp();
                        }
                        else
                        {
                            this.DoExternalDrop(evt);
                        }
                    }
                    this.myPretendInternalDrag = false;
                }
                catch (Exception exception1)
                {
                    Shapes.DiagramShape.Trace("OnDragDrop: " + exception1.ToString());
                    throw exception1;
                }
            }
            base.OnDragDrop(evt);
            args1.DragEventArgs = null;
        }

        protected override void OnDragEnter(DragEventArgs evt)
        {
            if ((this.ExternalDragDropsOnEnter && !this.IsInternalDragDrop(evt)) && this.CanInsertObjects())
            {
                Shapes.IDiagramShapeCollection collection1 = this.DoExternalDrop(evt);
                if ((collection1 != null) && !collection1.IsEmpty)
                {
                    this.myPretendInternalDrag = true;
                    evt.Effect = DragDropEffects.Move | (DragDropEffects.Copy | DragDropEffects.Scroll);
                    ToolDragging dragging1 = null;
                    IList list1 = this.MouseMoveTools;
                    for (int num1 = 0; num1 < list1.Count; num1++)
                    {
                        if (list1[num1] is ToolDragging)
                        {
                            dragging1 = (ToolDragging)list1[num1];
                            break;
                        }
                    }
                    if (dragging1 == null)
                    {
                        dragging1 = new ToolDragging(this);
                    }
                    InputEventArgs args1 = this.FirstInput;
                    Point point1 = new Point(evt.X, evt.Y);
                    args1.ViewPoint = base.PointToClient(point1);
                    args1.DocPoint = this.ConvertViewToDoc(args1.ViewPoint);
                    args1.Buttons = Control.MouseButtons;
                    args1.Modifiers = Control.ModifierKeys;
                    args1.Delta = 0;
                    args1.Key = Keys.None;
                    args1.DragEventArgs = evt;
                    this.LastInput.ViewPoint = args1.ViewPoint;
                    this.LastInput.DocPoint = args1.DocPoint;
                    this.LastInput.Buttons = args1.Buttons;
                    this.LastInput.Modifiers = args1.Modifiers;
                    this.LastInput.Delta = args1.Delta;
                    this.LastInput.Key = args1.Key;
                    this.LastInput.DragEventArgs = args1.DragEventArgs;
                    dragging1.CurrentObject = this.Selection.Primary;
                    dragging1.MoveOffset = this.Selection.HotSpot;
                    dragging1.mySelectionSet = true;
                    this.Tool = dragging1;
                    base.OnDragEnter(evt);
                    return;
                }
            }
            if (!this.IsInternalDragDrop(evt))
            {
                Shapes.DiagramShape obj1 = this.GetExternalDragImage(evt);
                if (obj1 != null)
                {
                    this.ShowExternalDragImage(obj1);
                    this.FollowExternalDragImage(this.LastInput.DocPoint);
                }
            }
            base.OnDragEnter(evt);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            this.StopHoverTimer();
            this.StopAutoScroll();
            if (this.myPretendInternalDrag)
            {
                this.myPretendInternalDrag = false;
                this.DeleteSelection(this.Selection);
                this.Tool = null;
            }
            else if (this.IsInternalDragDrop(null))
            {
                ToolDragging dragging1 = this.Tool as ToolDragging;
                if (dragging1 != null)
                {
                    dragging1.ClearDragSelection();
                }
            }
            else
            {
                this.HideExternalDragImage();
            }
            base.OnDragLeave(e);
        }

        protected override void OnDragOver(DragEventArgs evt)
        {
            InputEventArgs args1 = this.LastInput;
            if (this.AllowMouse)
            {
                try
                {
                    Point point1 = new Point(evt.X, evt.Y);
                    args1.ViewPoint = base.PointToClient(point1);
                    args1.DocPoint = this.ConvertViewToDoc(args1.ViewPoint);
                    args1.Buttons = Control.MouseButtons;
                    args1.Modifiers = Control.ModifierKeys;
                    args1.Delta = 0;
                    args1.Key = Keys.None;
                    args1.DragEventArgs = evt;
                    if (this.IsInternalDragDrop(evt))
                    {
                        this.DoInternalDrag(evt);
                    }
                    else
                    {
                        if (this.myPretendInternalDrag)
                        {
                            this.DoMouseMove();
                        }
                        this.DoExternalDrag(evt);
                    }
                }
                catch (Exception exception1)
                {
                    Shapes.DiagramShape.Trace("OnDragOver: " + exception1.ToString());
                    throw exception1;
                }
            }
            base.OnDragOver(evt);
            args1.DragEventArgs = null;
        }

        protected virtual void OnExternalObjectsDropped(InputEventArgs evt)
        {
            if (this.ExternalObjectsDropped != null)
            {
                this.ExternalObjectsDropped(this, evt);
            }
        }

        protected override void OnGotFocus(EventArgs evt)
        {
            base.OnGotFocus(evt);
            this.CleanUpModalControl();
            if (this.Selection != null)
            {
                this.Selection.OnGotFocus();
            }
        }

        protected override void OnKeyDown(KeyEventArgs evt)
        {
            InputEventArgs args1 = this.LastInput;
            if (this.AllowKey)
            {
                args1.Buttons = MouseButtons.None;
                args1.Modifiers = evt.Modifiers;
                args1.Delta = 0;
                args1.Key = evt.KeyCode;
                args1.KeyEventArgs = evt;
                this.DoKeyDown();
            }
            base.OnKeyDown(evt);
            args1.KeyEventArgs = null;
        }

        protected virtual void OnLinkCreated(SelectionEventArgs evt)
        {
            if (this.LinkCreated != null)
            {
                this.LinkCreated(this, evt);
            }
        }

        protected virtual void OnLinkRelinked(SelectionEventArgs evt)
        {
            if (this.LinkRelinked != null)
            {
                this.LinkRelinked(this, evt);
            }
        }

        protected override void OnLostFocus(EventArgs evt)
        {
            base.OnLostFocus(evt);
            if (this.Selection != null)
            {
                this.Selection.OnLostFocus();
            }
        }

        protected override void OnMouseDown(MouseEventArgs evt)
        {
            InputEventArgs args1 = this.LastInput;
            if (this.AllowMouse)
            {
                args1.ViewPoint = new Point(evt.X, evt.Y);
                args1.DocPoint = this.ConvertViewToDoc(args1.ViewPoint);
                args1.Buttons = evt.Button;
                args1.Modifiers = Control.ModifierKeys;
                args1.Delta = evt.Delta;
                args1.Key = Keys.None;
                args1.MouseEventArgs = evt;
                this.FirstInput.ViewPoint = args1.ViewPoint;
                this.FirstInput.DocPoint = args1.DocPoint;
                this.FirstInput.Buttons = args1.Buttons;
                this.FirstInput.Modifiers = args1.Modifiers;
                this.FirstInput.Delta = args1.Delta;
                this.FirstInput.Key = args1.Key;
                this.FirstInput.MouseEventArgs = evt;
                this.DoMouseDown();
            }
            base.OnMouseDown(evt);
            args1.MouseEventArgs = null;
            this.FirstInput.MouseEventArgs = null;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.StopHoverTimer();
            this.StopAutoScroll();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs evt)
        {
            InputEventArgs args1 = this.LastInput;
            Point point1 = new Point(evt.X, evt.Y);
            if (this.AllowMouse)
            {
                args1.ViewPoint = point1;
                args1.DocPoint = this.ConvertViewToDoc(args1.ViewPoint);
                args1.Buttons = evt.Button;
                args1.Modifiers = Control.ModifierKeys;
                args1.Delta = evt.Delta;
                args1.Key = Keys.None;
                args1.MouseEventArgs = evt;
                this.DoMouseMove();
            }
            base.OnMouseMove(evt);
            args1.MouseEventArgs = null;
        }

        protected override void OnMouseUp(MouseEventArgs evt)
        {
            InputEventArgs args1 = this.LastInput;
            if (this.AllowMouse)
            {
                args1.ViewPoint = new Point(evt.X, evt.Y);
                args1.DocPoint = this.ConvertViewToDoc(args1.ViewPoint);
                args1.Buttons = evt.Button;
                args1.Modifiers = Control.ModifierKeys;
                args1.Delta = evt.Delta;
                args1.Key = Keys.None;
                args1.MouseEventArgs = evt;
                this.DoMouseUp();
            }
            base.OnMouseUp(evt);
            args1.MouseEventArgs = null;
        }

        protected override void OnMouseWheel(MouseEventArgs evt)
        {
            InputEventArgs args1 = this.LastInput;
            if (this.AllowMouse)
            {
                args1.ViewPoint = new Point(evt.X, evt.Y);
                args1.DocPoint = this.ConvertViewToDoc(args1.ViewPoint);
                args1.Buttons = evt.Button;
                args1.Modifiers = Control.ModifierKeys;
                args1.Delta = evt.Delta;
                args1.Key = Keys.None;
                args1.MouseEventArgs = evt;
                this.DoMouseWheel();
            }
            base.OnMouseWheel(evt);
            args1.MouseEventArgs = null;
        }

        protected virtual void OnShapeContextClicked(DiagramShapeEventArgs evt)
        {
            if (this.ShapeContextClicked != null)
            {
                this.ShapeContextClicked(this, evt);
            }
        }

        protected virtual void OnShapeDoubleClicked(DiagramShapeEventArgs evt)
        {
            if (this.ShapeDoubleClicked != null)
            {
                this.ShapeDoubleClicked(this, evt);
            }
        }

        protected virtual void OnShapeEdited(SelectionEventArgs evt)
        {
            if (this.ShapeEdited != null)
            {
                this.ShapeEdited(this, evt);
            }
        }

        protected virtual void OnShapeSelected(SelectionEventArgs evt)
        {
            if (this.ShapeSelected != null)
            {
                this.ShapeSelected(this, evt);
            }
        }

        protected virtual void OnShapeHover(DiagramShapeEventArgs evt)
        {
            if (this.ShapeHover != null)
            {
                this.ShapeHover(this, evt);
            }
        }

        protected virtual void OnShapeLostSelection(SelectionEventArgs evt)
        {
            if (this.ShapeLostSelection != null)
            {
                this.ShapeLostSelection(this, evt);
            }
        }

        protected virtual void OnShapeResized(SelectionEventArgs evt)
        {
            if (this.ShapeResized != null)
            {
                this.ShapeResized(this, evt);
            }
        }

        protected virtual void OnShapeSingleClicked(DiagramShapeEventArgs evt)
        {
            if (this.ShapeSingleClicked != null)
            {
                this.ShapeSingleClicked(this, evt);
            }
        }

        protected override void OnPaint(PaintEventArgs evt)
        {
            try
            {
                this.onPaintCanvas(evt);
                if ((this.myGoControls != null) && (this.myGoControls.Count > 0))
                {
                    Rectangle rectangle1 = this.DisplayRectangle;
                    foreach (Shapes.DiagramControl control1 in this.myGoControls)
                    {
                        Control control2 = control1.FindControl(this);
                        if (control2 != null)
                        {
                            Rectangle rectangle2 = this.ConvertDocToView(control1.Bounds);
                            if (!rectangle1.IntersectsWith(rectangle2))
                            {
                                control2.Bounds = rectangle2;
                            }
                        }
                    }
                }
            }
            catch (Exception exception1)
            {
                Shapes.DiagramShape.Trace("OnPaint: " + exception1.ToString());
                throw exception1;
            }
            base.OnPaint(evt);
        }

        private void onPaintCanvas(PaintEventArgs evt)
        {
            if (this.mySuppressPaint <= 0)
            {
                this.myPaintEventArgs = evt;
                Graphics graphics1 = evt.Graphics;
                this.myGraphics = graphics1;
                graphics1.PageUnit = GraphicsUnit.Pixel;
                Rectangle rectangle1 = evt.ClipRectangle;
                if ((rectangle1.Width > 0) && (rectangle1.Height > 0))
                {
                    Rectangle rectangle2 = base.ClientRectangle;
                    if (((this.myBuffer == null) || (this.myBuffer.Width < rectangle2.Width)) || (this.myBuffer.Height < rectangle2.Height))
                    {
                        if (this.myBuffer != null)
                        {
                            this.myBuffer.Dispose();
                        }
                        this.myBuffer = new Bitmap(rectangle2.Width, rectangle2.Height, graphics1);
                    }
                    Graphics graphics2 = Graphics.FromImage(this.myBuffer);
                    graphics2.PageUnit = GraphicsUnit.Pixel;
                    this.PaintBorder(graphics2, rectangle2, rectangle1);
                    Rectangle rectangle3 = Rectangle.Intersect(rectangle1, this.DisplayRectangle);
                    graphics2.IntersectClip(rectangle3);
                    RectangleF ef1 = this.ConvertViewToDoc(rectangle3);
                    graphics2.TranslateTransform((float)this.myBorderSize.Width, (float)this.myBorderSize.Height);
                    graphics2.ScaleTransform(this.myHorizScale * this.myHorizWorld, this.myVertScale * this.myVertWorld);
                    PointF tf1 = this.DocPosition;
                    graphics2.TranslateTransform(-tf1.X, -tf1.Y);
                    this.PaintView(graphics2, ef1);
                    //modified by little
                    //this.myCurrentResult.Dispose(this);
                    graphics2.Dispose();

                    //added by little
                    myGraphics.DrawImage(myBuffer, ClientRectangle.X, ClientRectangle.Y);
                }
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs evt)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, evt);
            }
            if (evt.PropertyName != "Tool")
            {
                this.UpdateView();
            }
        }

        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs evt)
        {
            if (this.AllowMouse)
            {
                try
                {
                    if (evt.EscapePressed)
                    {
                        if (this.myPretendInternalDrag)
                        {
                            this.myPretendInternalDrag = false;
                            this.DeleteSelection(this.Selection);
                        }
                        this.DoCancelMouse();
                    }
                }
                catch (Exception exception1)
                {
                    Shapes.DiagramShape.Trace("OnQueryContinueDrag: " + exception1.ToString());
                    throw exception1;
                }
            }
            base.OnQueryContinueDrag(evt);
        }

        protected virtual void OnSelectionCopied(EventArgs evt)
        {
            if (this.SelectionCopied != null)
            {
                this.SelectionCopied(this, evt);
            }
        }

        protected virtual void OnSelectionDeleted(EventArgs evt)
        {
            if (this.SelectionDeleted != null)
            {
                this.SelectionDeleted(this, evt);
            }
        }

        protected virtual void OnSelectionDeleting(CancelEventArgs evt)
        {
            if (this.SelectionDeleting != null)
            {
                this.SelectionDeleting(this, evt);
            }
        }

        protected virtual void OnSelectionMoved(EventArgs evt)
        {
            if (this.SelectionMoved != null)
            {
                this.SelectionMoved(this, evt);
            }
        }

        protected override void OnSizeChanged(EventArgs evt)
        {
            base.OnSizeChanged(evt);
            this.LayoutScrollBars(false);
            this.UpdateView();
        }

        protected override void OnStyleChanged(EventArgs evt)
        {
            base.OnStyleChanged(evt);
            this.UpdateView();
        }

        protected override void OnSystemColorsChanged(EventArgs evt)
        {
            base.OnSystemColorsChanged(evt);
            this.UpdateView();
        }

        protected override void OnVisibleChanged(EventArgs evt)
        {
            base.OnVisibleChanged(evt);
            if (base.Visible)
            {
                this.LayoutScrollBars(false);
                this.UpdateView();
            }
        }

        protected virtual void PaintBackgroundDecoration(Graphics g, RectangleF clipRect)
        {
            Image image1 = this.BackgroundImage;
            if (image1 != null)
            {
                RectangleF ef1 = clipRect;
                ef1.Width = System.Math.Min(ef1.Width, (float)32767f);
                ef1.Height = System.Math.Min(ef1.Height, (float)32767f);
                g.DrawImage(image1, ef1, ef1, GraphicsUnit.Pixel);
            }
            switch (this.GridStyle)
            {
                case DiagramViewGridStyle.None:
                    {
                        return;
                    }
                case DiagramViewGridStyle.Dot:
                    {
                        this.DrawGridCrosses(g, new SizeF(1f, 1f), clipRect);
                        return;
                    }
                case DiagramViewGridStyle.Cross:
                    {
                        this.DrawGridCrosses(g, new SizeF(6f, 6f), clipRect);
                        return;
                    }
                case DiagramViewGridStyle.Line:
                    {
                        this.DrawGridLines(g, clipRect);
                        return;
                    }
            }
        }

        internal void PaintBorder(Graphics g, Rectangle rect, Rectangle clipRect)
        {
            switch (this.BorderStyle)
            {
                case System.Windows.Forms.BorderStyle.None:
                    {
                        return;
                    }
                case System.Windows.Forms.BorderStyle.FixedSingle:
                    {
                        if (((clipRect.X <= (rect.X + this.myBorderSize.Width)) || (clipRect.Y <= (rect.Y + this.myBorderSize.Height))) || (((clipRect.X + clipRect.Width) >= ((rect.X + rect.Width) - this.myBorderSize.Width)) || ((clipRect.Y + clipRect.Height) >= ((rect.Y + rect.Height) - this.myBorderSize.Height))))
                        {
                            g.DrawRectangle(Shapes.DiagramGraph.SystemPens_WindowFrame, rect);
                        }
                        return;
                    }
            }
            if (((clipRect.X <= (rect.X + this.myBorderSize.Width)) || (clipRect.Y <= (rect.Y + this.myBorderSize.Height))) || (((clipRect.X + clipRect.Width) >= ((rect.X + rect.Width) - this.myBorderSize.Width)) || ((clipRect.Y + clipRect.Height) >= ((rect.Y + rect.Height) - this.myBorderSize.Height))))
            {
                ControlPaint.DrawBorder3D(g, rect, this.Border3DStyle);
            }
        }

        protected virtual void PaintObjects(bool doc, bool view, Graphics g, RectangleF clipRect)
        {
            LayerCollectionEnumerator enumerator1 = this.Layers.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramLayer layer1 = enumerator1.Current;
                if ((doc && layer1.IsInDocument) || (view && layer1.IsInView))
                {
                    layer1.Paint(g, this, clipRect);
                }
            }
        }

        protected virtual void PaintPaperColor(Graphics g, RectangleF clipRect)
        {
            Color color1 = this.Document.PaperColor;
            if (color1 == Color.Empty)
            {
                color1 = this.BackColor;
            }
            if ((this.myBackgroundBrush == null) || (this.myBackgroundBrush.Color != color1))
            {
                if (this.myBackgroundBrush != null)
                {
                    this.myBackgroundBrush.Dispose();
                }
                this.myBackgroundBrush = new SolidBrush(color1);
            }
            g.FillRectangle(this.myBackgroundBrush, clipRect);
        }

        protected virtual void PaintView(Graphics g, RectangleF clipRect)
        {
            this.PaintPaperColor(g, clipRect);
            this.PaintBackgroundDecoration(g, clipRect);
            g.SmoothingMode = this.SmoothingMode;
            g.TextRenderingHint = this.TextRenderingHint;
            g.InterpolationMode = this.InterpolationMode;
            this.PaintObjects(true, true, g, clipRect);
        }

        [PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\r\n               version=\"1\">\r\n   <IPermission class=\"System.Security.Permissions.UIPermission, mscorlib, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                version=\"1\"\r\n                Clipboard=\"AllClipboard\"/>\r\n</PermissionSet>\r\n")]
        public virtual CopyDictionary PasteFromClipboard()
        {
            DiagramDocument document1 = this.Document;
            if (document1 != null)
            {
                IDataObject obj1 = Clipboard.GetDataObject();
                if (obj1 == null)
                {
                    return null;
                }
                object obj2 = obj1.GetData(document1.DataFormat);
                if ((obj2 != null) && (obj2 is DiagramDocument))
                {
                    DiagramDocument document2 = (DiagramDocument)obj2;
                    return document1.CopyFromCollection(document2, false, false, new SizeF(1f, 1f), null);
                }
            }
            return null;
        }

        public virtual Shapes.DiagramShape PickObject(bool doc, bool view, PointF p, bool selectableOnly)
        {
            if (!selectableOnly || this.CanSelectObjects())
            {
                LayerCollectionEnumerator enumerator1 = this.Layers.Backwards.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramLayer layer1 = enumerator1.Current;
                    if ((doc && layer1.IsInDocument) || (view && layer1.IsInView))
                    {
                        Shapes.DiagramShape obj1 = layer1.PickObject(p, selectableOnly);
                        if (obj1 != null)
                        {
                            return obj1;
                        }
                    }
                }
            }
            return null;
        }

        public virtual Shapes.IDiagramShapeCollection PickObjects(bool doc, bool view, PointF p, bool selectableOnly, Shapes.IDiagramShapeCollection coll, int max)
        {
            if (selectableOnly && !this.CanSelectObjects())
            {
                return null;
            }
            if (coll == null)
            {
                coll = new Shapes.DiagramShapeCollection();
            }
            LayerCollectionEnumerator enumerator1 = this.Layers.Backwards.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramLayer layer1 = enumerator1.Current;
                if (coll.Count >= max)
                {
                    return coll;
                }
                if ((doc && layer1.IsInDocument) || (view && layer1.IsInView))
                {
                    layer1.PickObjects(p, selectableOnly, coll, max);
                }
            }
            return coll;
        }

        public virtual void Print()
        {
            try
            {
                PrintDocument document1 = new PrintDocument();
                document1.PrintPage += new PrintPageEventHandler(this.PrintDocumentPage);
                document1.DocumentName = this.Document.Name;
                if (this.PrintShowDialog(document1) != DialogResult.Cancel)
                {
                    document1.Print();
                }
            }
            catch (Exception exception1)
            {
                Shapes.DiagramShape.Trace("Print: " + exception1.ToString());
                throw exception1;
            }
            finally
            {
                this.myPrintInfo = null;
            }
        }

        protected virtual void PrintDecoration(Graphics g, PrintPageEventArgs e, int hpnum, int hpmax, int vpnum, int vpmax)
        {
            float single1 = e.MarginBounds.X;
            float single2 = e.MarginBounds.Y;
            float single3 = e.MarginBounds.Width;
            float single4 = e.MarginBounds.Height;
            float single5 = single1 + single3;
            float single6 = single2 + single4;
            g.DrawLine(Shapes.DiagramGraph.Pens_Black, single1, single2, (float)(single1 + 10f), single2);
            g.DrawLine(Shapes.DiagramGraph.Pens_Black, single1, single2, single1, (float)(single2 + 10f));
            g.DrawLine(Shapes.DiagramGraph.Pens_Black, single5, single2, (float)(single5 - 10f), single2);
            g.DrawLine(Shapes.DiagramGraph.Pens_Black, single5, single2, single5, (float)(single2 + 10f));
            g.DrawLine(Shapes.DiagramGraph.Pens_Black, single1, single6, (float)(single1 + 10f), single6);
            g.DrawLine(Shapes.DiagramGraph.Pens_Black, single1, single6, single1, (float)(single6 - 10f));
            g.DrawLine(Shapes.DiagramGraph.Pens_Black, single5, single6, (float)(single5 - 10f), single6);
            g.DrawLine(Shapes.DiagramGraph.Pens_Black, single5, single6, single5, (float)(single6 - 10f));
        }

        protected virtual void PrintDocumentPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics1 = e.Graphics;
            if (this.myPrintInfo == null)
            {
                this.myPrintInfo = new PrintInfo();
                this.myPrintInfo.DocRect = new RectangleF(this.PrintDocumentTopLeft, this.PrintDocumentSize);
                this.myPrintInfo.HorizScale = this.PrintScale;
                this.myPrintInfo.VertScale = this.myPrintInfo.HorizScale;
                Rectangle rectangle1 = e.MarginBounds;
                this.myPrintInfo.PrintSize = new SizeF(((float)rectangle1.Width) / this.myPrintInfo.HorizScale, ((float)rectangle1.Height) / this.myPrintInfo.VertScale);
                if ((this.myPrintInfo.PrintSize.Width > 0f) && (this.myPrintInfo.PrintSize.Height > 0f))
                {
                    this.myPrintInfo.NumPagesAcross = (int)System.Math.Ceiling((double)(this.myPrintInfo.DocRect.Width / this.myPrintInfo.PrintSize.Width));
                    this.myPrintInfo.NumPagesDown = (int)System.Math.Ceiling((double)(this.myPrintInfo.DocRect.Height / this.myPrintInfo.PrintSize.Height));
                    switch (e.PageSettings.PrinterSettings.PrintRange)
                    {
                        case PrintRange.Selection:
                            {
                                this.myPrintInfo.CurPage = 0;
                                goto Label_0195;
                            }
                        case PrintRange.SomePages:
                            {
                                this.myPrintInfo.CurPage = e.PageSettings.PrinterSettings.FromPage;
                                goto Label_0195;
                            }
                    }
                    this.myPrintInfo.CurPage = 0;
                }
            }
        Label_0195:
            if ((this.myPrintInfo.NumPagesAcross <= 0) || (this.myPrintInfo.NumPagesDown <= 0))
            {
                return;
            }
            int num1 = this.myPrintInfo.CurPage % this.myPrintInfo.NumPagesAcross;
            int num2 = this.myPrintInfo.CurPage / this.myPrintInfo.NumPagesAcross;
            PointF tf1 = this.myOrigin;
            float single1 = this.myHorizScale;
            float single2 = this.myVertScale;
            Size size1 = this.myBorderSize;
            this.myOrigin = new PointF(this.myPrintInfo.DocRect.X + (num1 * this.myPrintInfo.PrintSize.Width), this.myPrintInfo.DocRect.Y + (num2 * this.myPrintInfo.PrintSize.Height));
            this.myHorizScale = this.myPrintInfo.HorizScale;
            this.myVertScale = this.myPrintInfo.VertScale;
            this.myBorderSize = new Size(e.MarginBounds.X, e.MarginBounds.Y);
            RectangleF ef1 = new RectangleF(this.myOrigin.X, this.myOrigin.Y, System.Math.Min(this.myPrintInfo.PrintSize.Width, this.myPrintInfo.DocRect.Width), System.Math.Min(this.myPrintInfo.PrintSize.Height, this.myPrintInfo.DocRect.Height));
            try
            {
                this.PrintDecoration(graphics1, e, num1, this.myPrintInfo.NumPagesAcross, num2, this.myPrintInfo.NumPagesDown);
                graphics1.IntersectClip(e.MarginBounds);
                graphics1.TranslateTransform((float)this.myBorderSize.Width, (float)this.myBorderSize.Height);
                graphics1.ScaleTransform(this.myHorizScale * this.myHorizWorld, this.myVertScale * this.myVertWorld);
                graphics1.TranslateTransform(-this.myOrigin.X, -this.myOrigin.Y);
                this.PrintView(graphics1, ef1);
            }
            finally
            {
                this.myOrigin = tf1;
                this.myHorizScale = single1;
                this.myVertScale = single2;
                this.myBorderSize = size1;
            }
            int num3 = 0;
            switch (e.PageSettings.PrinterSettings.PrintRange)
            {
                case PrintRange.Selection:
                    {
                        num3 = (this.myPrintInfo.NumPagesAcross * this.myPrintInfo.NumPagesDown) - 1;
                        break;
                    }
                case PrintRange.SomePages:
                    {
                        num3 = e.PageSettings.PrinterSettings.ToPage;
                        break;
                    }
                default:
                    {
                        num3 = (this.myPrintInfo.NumPagesAcross * this.myPrintInfo.NumPagesDown) - 1;
                        break;
                    }
            }
            e.HasMorePages = this.myPrintInfo.CurPage < num3;
            if (e.HasMorePages)
            {
                this.myPrintInfo.CurPage++;
            }
            else
            {
                this.myPrintInfo = null;
            }
        }

        public virtual void PrintPreview()
        {
            try
            {
                PrintDocument document1 = new PrintDocument();
                document1.PrintPage += new PrintPageEventHandler(this.PrintDocumentPage);
                document1.DocumentName = this.Document.Name;
                this.PrintPreviewShowDialog(document1);
            }
            catch (Exception exception1)
            {
                Shapes.DiagramShape.Trace("PrintPreview: " + exception1.ToString());
                throw exception1;
            }
            finally
            {
                this.myPrintInfo = null;
            }
        }

        protected virtual void PrintPreviewShowDialog(PrintDocument pd)
        {
            PrintPreviewDialog dialog1 = new PrintPreviewDialog();
            dialog1.UseAntiAlias = true;
            dialog1.Document = pd;
            dialog1.ShowDialog();
        }

        protected virtual DialogResult PrintShowDialog(PrintDocument pd)
        {
            PrintDialog dialog1 = new PrintDialog();
            dialog1.AllowSomePages = true;
            dialog1.Document = pd;
            return dialog1.ShowDialog();
        }

        protected virtual void PrintView(Graphics g, RectangleF clipRect)
        {
            this.PaintBackgroundDecoration(g, clipRect);
            g.SmoothingMode = this.SmoothingMode;
            g.TextRenderingHint = this.TextRenderingHint;
            g.InterpolationMode = this.InterpolationMode;
            this.PaintObjects(true, false, g, clipRect);
        }


        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            Shapes.DiagramControl control1 = this.EditControl;
            if (control1 != null)
            {
                Control control2 = control1.GetControl(this);
                if ((control2 != null) && control2.Focused)
                {
                    return false;
                }
            }
            return base.ProcessCmdKey(ref m, keyData);
        }

        public void RaiseBackgroundContextClicked(InputEventArgs evt)
        {
            this.OnBackgroundContextClicked(evt);
        }

        public void RaiseBackgroundDoubleClicked(InputEventArgs evt)
        {
            this.OnBackgroundDoubleClicked(evt);
        }

        public void RaiseBackgroundHover(InputEventArgs evt)
        {
            this.OnBackgroundHover(evt);
        }

        public void RaiseBackgroundSingleClicked(InputEventArgs evt)
        {
            this.OnBackgroundSingleClicked(evt);
        }

        public virtual void RaiseChanged(int hint, int subhint, object x, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            int num1 = hint;
            if (num1 <= 0x388)
            {
                switch (num1)
                {
                    case 0x321:
                    case 0x322:
                    case 0x323:
                        {
                            goto Label_0160;
                        }
                    case 0x385:
                    case 0x388:
                        {
                            Shapes.DiagramShape obj1 = x as Shapes.DiagramShape;
                            if (obj1 != null)
                            {
                                RectangleF ef1 = obj1.Bounds;
                                ef1 = obj1.ExpandPaintBounds(ef1, this);
                                Rectangle rectangle1 = this.ConvertDocToView(ef1);
                                rectangle1.Inflate(2, 2);
                                if ((hint == 0x385) && (subhint == 0x3e9))
                                {
                                    oldRect = obj1.ExpandPaintBounds(oldRect, this);
                                    Rectangle rectangle2 = this.ConvertDocToView(oldRect);
                                    rectangle2.Inflate(2, 2);
                                    if (rectangle1.IntersectsWith(rectangle2))
                                    {
                                        base.Invalidate(Rectangle.Union(rectangle1, rectangle2));
                                        return;
                                    }
                                    base.Invalidate(rectangle1);
                                    base.Invalidate(rectangle2);
                                    return;
                                }
                                base.Invalidate(rectangle1);
                            }
                            return;
                        }
                    case 0x386:
                        {
                            Shapes.DiagramShape obj2 = x as Shapes.DiagramShape;
                            if (obj2 != null)
                            {
                                RectangleF ef2 = obj2.Bounds;
                                ef2 = obj2.ExpandPaintBounds(ef2, this);
                                Rectangle rectangle3 = this.ConvertDocToView(ef2);
                                rectangle3.Inflate(2, 2);
                                base.Invalidate(rectangle3);
                            }
                            return;
                        }
                    case 0x387:
                        {
                            Shapes.DiagramShape obj3 = x as Shapes.DiagramShape;
                            if (obj3 != null)
                            {
                                RectangleF ef3 = obj3.Bounds;
                                ef3 = obj3.ExpandPaintBounds(ef3, this);
                                Rectangle rectangle4 = this.ConvertDocToView(ef3);
                                rectangle4.Inflate(2, 2);
                                base.Invalidate(rectangle4);
                            }
                            return;
                        }
                }
                return;
            }
            if (num1 != 910)
            {
                if (num1 == 930)
                {
                }
                return;
            }
        Label_0160:
            this.UpdateView();
        }

        public void RaiseClipboardPasted()
        {
            this.OnClipboardPasted(EventArgs.Empty);
        }

        public void RaiseExternalObjectsDropped(InputEventArgs evt)
        {
            this.OnExternalObjectsDropped(evt);
        }

        public void RaiseLinkCreated(Shapes.DiagramShape obj)
        {
            this.OnLinkCreated(new SelectionEventArgs(obj));
        }

        public void RaiseLinkRelinked(Shapes.DiagramShape obj)
        {
            this.OnLinkRelinked(new SelectionEventArgs(obj));
        }

        public void RaiseShapeContextClicked(Shapes.DiagramShape obj, InputEventArgs evt)
        {
            this.OnShapeContextClicked(new DiagramShapeEventArgs(obj, evt));
        }

        public void RaiseShapeDoubleClicked(Shapes.DiagramShape obj, InputEventArgs evt)
        {
            this.OnShapeDoubleClicked(new DiagramShapeEventArgs(obj, evt));
        }

        public void RaiseShapeEdited(Shapes.DiagramShape obj)
        {
            this.OnShapeEdited(new SelectionEventArgs(obj));
        }

        public void RaiseShapeSelected(Shapes.DiagramShape obj)
        {
            this.OnShapeSelected(new SelectionEventArgs(obj));
        }

        public void RaiseShapeHover(Shapes.DiagramShape obj, InputEventArgs evt)
        {
            this.OnShapeHover(new DiagramShapeEventArgs(obj, evt));
        }

        public void RaiseShapeLostSelection(Shapes.DiagramShape obj)
        {
            this.OnShapeLostSelection(new SelectionEventArgs(obj));
        }

        public void RaiseShapeResized(Shapes.DiagramShape obj)
        {
            this.OnShapeResized(new SelectionEventArgs(obj));
        }

        public void RaiseShapeSingleClicked(Shapes.DiagramShape obj, InputEventArgs evt)
        {
            this.OnShapeSingleClicked(new DiagramShapeEventArgs(obj, evt));
        }

        public void RaisePropertyChangedEvent(string propname)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propname));
        }

        public void RaiseSelectionCopied()
        {
            this.OnSelectionCopied(EventArgs.Empty);
        }

        public void RaiseSelectionDeleted()
        {
            this.OnSelectionDeleted(EventArgs.Empty);
        }

        public void RaiseSelectionDeleting(CancelEventArgs evt)
        {
            this.OnSelectionDeleting(evt);
        }

        public void RaiseSelectionMoved()
        {
            this.OnSelectionMoved(EventArgs.Empty);
        }

        public virtual void Redo()
        {
            if (this.CanRedo())
            {
                this.Document.Redo();
            }
        }

        private void removeFromSelection(Shapes.DiagramShape obj)
        {
            Shapes.GroupShape group1 = obj as Shapes.GroupShape;
            if (group1 != null)
            {
                Shapes.GroupEnumerator enumerator2 = group1.GetEnumerator();
                Shapes.GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj1 = enumerator1.Current;
                    this.removeFromSelection(obj1);
                }
            }
            this.Selection.Remove(obj);
        }

        internal void RemoveDiagramControl(Shapes.DiagramControl g, Control c)
        {
            if (this.myGoControls != null)
            {
                this.myGoControls.Remove(g);
                base.Controls.Remove(c);
            }
        }

        public virtual IDiagramTool ReplaceMouseTool(Type tooltype, IDiagramTool newtool)
        {
            IDiagramTool tool1 = null;
            IList list1 = this.MouseDownTools;
            int num1 = 0;
            while (num1 < list1.Count)
            {
                if (list1[num1].GetType() == tooltype)
                {
                    tool1 = (IDiagramTool)list1[num1];
                    if (newtool == null)
                    {
                        list1.RemoveAt(num1);
                        return tool1;
                    }
                    list1[num1] = newtool;
                    return tool1;
                }
                num1++;
            }
            list1 = this.MouseMoveTools;
            num1 = 0;
            while (num1 < list1.Count)
            {
                if (list1[num1].GetType() == tooltype)
                {
                    tool1 = (IDiagramTool)list1[num1];
                    if (newtool == null)
                    {
                        list1.RemoveAt(num1);
                        return tool1;
                    }
                    list1[num1] = newtool;
                    return tool1;
                }
                num1++;
            }
            list1 = this.MouseUpTools;
            for (num1 = 0; num1 < list1.Count; num1++)
            {
                if (list1[num1].GetType() == tooltype)
                {
                    tool1 = (IDiagramTool)list1[num1];
                    if (newtool == null)
                    {
                        list1.RemoveAt(num1);
                        return tool1;
                    }
                    list1[num1] = newtool;
                    return tool1;
                }
            }
            return null;
        }

        public virtual void RescaleToFit()
        {
            Size size1 = this.DisplayRectangle.Size;
            RectangleF ef1 = this.ComputeDocumentBounds();
            if (ef1.X > 0f)
            {
                ef1.Width += ef1.X;
                ef1.X = 0f;
            }
            if (ef1.Y > 0f)
            {
                ef1.Height += ef1.Y;
                ef1.Y = 0f;
            }
            this.DocPosition = new PointF(ef1.X, ef1.Y);
            float single1 = 1f;
            if ((ef1.Width > 0f) && (ef1.Height > 0f))
            {
                single1 = System.Math.Min((float)(((float)size1.Width) / ef1.Width), (float)(((float)size1.Height) / ef1.Height));
            }
            if (single1 > 1f)
            {
                single1 = 1f;
            }
            this.DocScale = single1;
        }

        private void ResetAutoPanRegion()
        {
            this.AutoPanRegion = new Size(0x10, 0x10);
        }

        private void ResetAutoScrollRegion()
        {
            this.AutoScrollRegion = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
        }

        private void ResetGridColor()
        {
            this.GridColor = Color.LightGray;
        }

        private void ResetNoFocusSelectionColor()
        {
            this.NoFocusSelectionColor = Color.LightGray;
        }

        private void ResetPrimarySelectionColor()
        {
            this.PrimarySelectionColor = Color.Chartreuse;
        }

        private void ResetScrollSmallChange()
        {
            this.ScrollSmallChange = new Size(0x10, 0x10);
        }

        private void ResetSecondarySelectionColor()
        {
            this.SecondarySelectionColor = Color.Cyan;
        }

        private void ResetShadowColor()
        {
            this.ShadowColor = Color.FromArgb(0x7f, Color.Gray);
        }

        internal void SafeOnDocumentChanged(object sender, ChangedEventArgs e)
        {
            if (base.InvokeRequired)
            {
                if (this.mySafeOnDocumentChangedDelegate == null)
                {
                    this.mySafeOnDocumentChangedDelegate = new EventHandler(this.InternalOnDocumentChanged);
                }
                if (this.myQueuedEvents == null)
                {
                    this.myQueuedEvents = new Queue();
                }
                ChangedEventArgs args1 = new ChangedEventArgs(e);
                lock (this.myQueuedEvents)
                {
                    this.myQueuedEvents.Enqueue(args1);
                }
                base.Invoke(this.mySafeOnDocumentChangedDelegate);
            }
            else
            {
                this.OnDocumentChanged(sender, e);
            }
        }

        public virtual void ScrollLine(float dx, float dy)
        {
            PointF tf1 = this.DocPosition;
            SizeF ef1 = this.DocExtentSize;
            PointF tf2 = this.DocumentTopLeft;
            SizeF ef2 = this.DocumentSize;
            Size size1 = this.ScrollSmallChange;
            float single1 = ((dx * size1.Width) / this.myHorizWorld) / this.myHorizScale;
            tf1.X += single1;
            if (single1 >= 0f)
            {
                tf1.X = System.Math.Min(tf1.X, System.Math.Max(tf2.X, (float)((tf2.X + ef2.Width) - ef1.Width)));
            }
            else
            {
                tf1.X = System.Math.Max(tf1.X, tf2.X);
            }
            float single2 = ((dy * size1.Height) / this.myVertWorld) / this.myVertScale;
            tf1.Y += single2;
            if (single2 >= 0f)
            {
                tf1.Y = System.Math.Min(tf1.Y, System.Math.Max(tf2.Y, (float)((tf2.Y + ef2.Height) - ef1.Height)));
            }
            else
            {
                tf1.Y = System.Math.Max(tf1.Y, tf2.Y);
            }
            this.DocPosition = tf1;
        }

        public virtual void ScrollPage(float dx, float dy)
        {
            PointF tf1 = this.DocPosition;
            SizeF ef1 = this.DocExtentSize;
            PointF tf2 = this.DocumentTopLeft;
            SizeF ef2 = this.DocumentSize;
            Size size1 = this.ScrollSmallChange;
            float single1 = ((dx * System.Math.Max((float)size1.Width, (float)(ef1.Width - size1.Width))) / this.myHorizWorld) / this.myHorizScale;
            tf1.X += single1;
            if (single1 >= 0f)
            {
                tf1.X = System.Math.Min(tf1.X, System.Math.Max(tf2.X, (float)((tf2.X + ef2.Width) - ef1.Width)));
            }
            else
            {
                tf1.X = System.Math.Max(tf1.X, tf2.X);
            }
            float single2 = ((dy * System.Math.Max((float)size1.Height, (float)(ef1.Height - size1.Height))) / this.myVertWorld) / this.myVertScale;
            tf1.Y += single2;
            if (single2 >= 0f)
            {
                tf1.Y = System.Math.Min(tf1.Y, System.Math.Max(tf2.Y, (float)((tf2.Y + ef2.Height) - ef1.Height)));
            }
            else
            {
                tf1.Y = System.Math.Max(tf1.Y, tf2.Y);
            }
            this.DocPosition = tf1;
        }

        public virtual void ScrollRectangleToVisible(RectangleF contentRect)
        {
            RectangleF ef1 = this.DocExtent;
            if (!Shapes.DiagramShape.ContainsRect(ef1, contentRect))
            {
                float single1;
                float single2;
                if (contentRect.Width < ef1.Width)
                {
                    single1 = (contentRect.X + (contentRect.Width / 2f)) - (ef1.Width / 2f);
                }
                else
                {
                    single1 = contentRect.X;
                }
                if (contentRect.Height < ef1.Height)
                {
                    single2 = (contentRect.Y + (contentRect.Height / 2f)) - (ef1.Height / 2f);
                }
                else
                {
                    single2 = contentRect.Y;
                }
                this.DocPosition = new PointF(single1, single2);
            }
        }

        public virtual void SelectAll()
        {
            if (this.CanSelectObjects())
            {
                ArrayList list1 = new ArrayList();
                LayerCollectionEnumerator enumerator1 = this.Layers.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramLayer layer1 = enumerator1.Current;
                    if ((layer1.IsInDocument && layer1.CanViewObjects()) && layer1.CanSelectObjects())
                    {
                        LayerEnumerator enumerator2 = layer1.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Shapes.DiagramShape obj1 = enumerator2.Current;
                            if (obj1.CanView() && obj1.CanSelect())
                            {
                                list1.Add(obj1);
                            }
                        }
                    }
                }
                foreach (Shapes.DiagramShape obj2 in list1)
                {
                    this.Selection.Add(obj2);
                }
            }
        }

        public virtual void SelectInRectangle(RectangleF rect)
        {
            if (this.CanSelectObjects())
            {
                ArrayList list1 = new ArrayList();
                LayerCollectionEnumerator enumerator1 = this.Layers.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    DiagramLayer layer1 = enumerator1.Current;
                    if ((layer1.IsInDocument && layer1.CanViewObjects()) && layer1.CanSelectObjects())
                    {
                        LayerEnumerator enumerator2 = layer1.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Shapes.DiagramShape obj1 = enumerator2.Current;
                            this.selectObjectInRectangle(obj1, rect, false, list1);
                        }
                    }
                }
                foreach (Shapes.DiagramShape obj2 in list1)
                {
                    this.Selection.Add(obj2);
                }
            }
        }

        public virtual bool SelectNextNode(char c)
        {
            if (this.CanSelectObjects())
            {
                Shapes.ITextNode node1 = null;
                Shapes.DiagramShape obj1 = this.Selection.Primary;
                if ((obj1 != null) && (obj1 is Shapes.ITextNode))
                {
                    node1 = (Shapes.ITextNode)obj1;
                }
                LayerCollectionObjectEnumerator enumerator1 = this.Document.GetEnumerator();
                if (node1 != null)
                {
                    while (enumerator1.MoveNext())
                    {
                        if (enumerator1.Current == node1)
                        {
                            break;
                        }
                    }
                }
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj2 = enumerator1.Current;
                    Shapes.ITextNode node2 = obj2 as Shapes.ITextNode;
                    if ((node2 != null) && this.MatchesNodeLabel(node2, c))
                    {
                        this.Selection.Select(obj2);
                        this.ScrollRectangleToVisible(obj2.Bounds);
                        return true;
                    }
                }
                enumerator1 = this.Document.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj3 = enumerator1.Current;
                    Shapes.ITextNode node3 = obj3 as Shapes.ITextNode;
                    if (node3 == node1)
                    {
                        break;
                    }
                    if ((node3 != null) && this.MatchesNodeLabel(node3, c))
                    {
                        this.Selection.Select(obj3);
                        this.ScrollRectangleToVisible(obj3.Bounds);
                        return true;
                    }
                }
            }
            return false;
        }

        private void selectObjectInRectangle(Shapes.DiagramShape obj, RectangleF rect, bool top, ArrayList coll)
        {
            if (obj.CanView())
            {
                if (obj.CanSelect() && ((obj.SelectionObject != null) ? obj.SelectionObject.ContainedByRectangle(rect) : obj.ContainedByRectangle(rect)))
                {
                    coll.Add(obj);
                }
                else if (!top && (obj is Shapes.GroupShape))
                {
                    Shapes.GroupShape group1 = (Shapes.GroupShape)obj;
                    Shapes.GroupEnumerator enumerator2 = group1.GetEnumerator();
                    Shapes.GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        Shapes.DiagramShape obj1 = enumerator1.Current;
                        this.selectObjectInRectangle(obj1, rect, false, coll);
                    }
                }
            }
        }

        public virtual void SetModifiable(bool b)
        {
            this.AllowMove = b;
            this.AllowResize = b;
            this.AllowReshape = b;
            this.AllowDelete = b;
            this.AllowInsert = b;
            this.AllowLink = b;
            this.AllowEdit = b;
        }

        private bool ShouldSerializeAutoPanRegion()
        {
            return (this.AutoPanRegion != new Size(0x10, 0x10));
        }

        private bool ShouldSerializeAutoScrollRegion()
        {
            return (this.AutoScrollRegion != new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight));
        }

        private bool ShouldSerializeGridColor()
        {
            return (this.GridColor != Color.LightGray);
        }

        private bool ShouldSerializeNoFocusSelectionColor()
        {
            return (this.NoFocusSelectionColor != Color.LightGray);
        }

        private bool ShouldSerializePrimarySelectionColor()
        {
            return (this.PrimarySelectionColor != Color.Chartreuse);
        }

        private bool ShouldSerializeScrollSmallChange()
        {
            return (this.ScrollSmallChange != new Size(0x10, 0x10));
        }

        private bool ShouldSerializeSecondarySelectionColor()
        {
            return (this.SecondarySelectionColor != Color.Cyan);
        }

        private bool ShouldSerializeShadowColor()
        {
            return (this.ShadowColor != Color.FromArgb(0x7f, Color.Gray));
        }

        private void ShowExternalDragImage(Shapes.DiagramShape img)
        {
            this.myExternalDragImage = img;
            this.Layers.Default.Add(img);
        }

        public virtual bool StartTransaction()
        {
            return this.Document.StartTransaction();
        }

        public void StopAutoScroll()
        {
            if (this.myAutoScrollTimer != null)
            {
                this.myAutoScrollTimer.Change(-1, -1);
                this.myAutoScrollTimerEnabled = false;
            }
        }

        private void StopHoverTimer()
        {
            if (this.myHoverTimer != null)
            {
                this.myHoverTimer.Change(-1, -1);
                this.myHoverTimerEnabled = false;
            }
        }

        public virtual void Undo()
        {
            if (this.CanUndo())
            {
                this.Document.Undo();
            }
        }

        private void UpdateBorderWidths()
        {
            Size size1 = this.myBorderSize;
            switch (this.BorderStyle)
            {
                case System.Windows.Forms.BorderStyle.None:
                    {
                        size1 = new Size();
                        break;
                    }
                case System.Windows.Forms.BorderStyle.FixedSingle:
                    {
                        size1 = SystemInformation.BorderSize;
                        break;
                    }
                default:
                    {
                        size1 = SystemInformation.Border3DSize;
                        break;
                    }
            }
            if (size1 != this.myBorderSize)
            {
                this.myBorderSize = size1;
                this.LayoutScrollBars(false);
            }
        }

        public virtual void UpdateScrollBars()
        {
            if (!this.myUpdatingScrollBars)
            {
                HScrollBar bar1 = this.HorizontalScrollBar;
                VScrollBar bar2 = this.VerticalScrollBar;
                if ((bar2 != null) || (bar1 != null))
                {
                    PointF tf1 = this.DocumentTopLeft;
                    SizeF ef1 = this.DocumentSize;
                    int num1 = (int)System.Math.Floor((double)((tf1.X * this.myHorizScale) * this.myHorizWorld));
                    int num2 = (int)System.Math.Floor((double)((tf1.Y * this.myVertScale) * this.myVertWorld));
                    int num3 = (int)System.Math.Ceiling((double)(((tf1.X + ef1.Width) * this.myHorizScale) * this.myHorizWorld));
                    int num4 = (int)System.Math.Ceiling((double)(((tf1.Y + ef1.Height) * this.myVertScale) * this.myVertWorld));
                    PointF tf2 = this.DocPosition;
                    int num5 = (int)System.Math.Floor((double)((tf2.X * this.myHorizScale) * this.myHorizWorld));
                    int num6 = (int)System.Math.Floor((double)((tf2.Y * this.myVertScale) * this.myVertWorld));
                    Size size1 = base.Size;
                    size1.Width -= (2 * this.myBorderSize.Width);
                    if (size1.Width < 0)
                    {
                        size1.Width = 0;
                    }
                    size1.Height -= (2 * this.myBorderSize.Height);
                    if (size1.Height < 0)
                    {
                        size1.Height = 0;
                    }
                    bool flag1 = (((num4 - num2) > size1.Height) || (num6 > num2)) || (num6 < (num4 - size1.Height));
                    bool flag2 = (bar2 != null) && ((this.VerticalScrollBarVisibility == DiagramViewScrollBarVisibility.Show) || ((this.VerticalScrollBarVisibility == DiagramViewScrollBarVisibility.Auto) && flag1));
                    bool flag3 = (((num3 - num1) > size1.Width) || (num5 > num1)) || (num5 < (num3 - size1.Width));
                    bool flag4 = (bar1 != null) && ((this.HorizontalScrollBarVisibility == DiagramViewScrollBarVisibility.Show) || ((this.HorizontalScrollBarVisibility == DiagramViewScrollBarVisibility.Auto) && flag3));
                    if (flag2)
                    {
                        size1.Width -= this.myScrollBarWidth;
                        size1.Width = System.Math.Max(0, size1.Width);
                    }
                    if (flag4)
                    {
                        size1.Height -= this.myScrollBarHeight;
                        size1.Height = System.Math.Max(0, size1.Height);
                    }
                    flag1 = (((num4 - num2) > size1.Height) || (num6 > num2)) || (num6 < (num4 - size1.Height));
                    flag2 = (bar2 != null) && ((this.VerticalScrollBarVisibility == DiagramViewScrollBarVisibility.Show) || ((this.VerticalScrollBarVisibility == DiagramViewScrollBarVisibility.Auto) && flag1));
                    flag3 = (((num3 - num1) > size1.Width) || (num5 > num1)) || (num5 < (num3 - size1.Width));
                    flag4 = (bar1 != null) && ((this.HorizontalScrollBarVisibility == DiagramViewScrollBarVisibility.Show) || ((this.HorizontalScrollBarVisibility == DiagramViewScrollBarVisibility.Auto) && flag3));
                    this.myUpdatingScrollBars = true;
                    bool flag5 = false;
                    if (bar2 != null)
                    {
                        int num7 = num4 - size1.Height;
                        if ((num6 > num7) && (num7 > num2))
                        {
                            num6 = num7;
                        }
                        else if (num6 < num2)
                        {
                            num6 = num2;
                        }
                        int num8 = System.Math.Max(num4, (int)(num6 + size1.Height)) - 12;
                        num8 = System.Math.Max(num8, num6);
                        if (bar2.Minimum != num2)
                        {
                            bar2.Minimum = num2;
                        }
                        if (bar2.Maximum != num8)
                        {
                            bar2.Maximum = num8;
                        }
                        if (bar2.Value != num6)
                        {
                            bar2.Value = num6;
                        }
                        if (bar2.Visible != flag2)
                        {
                            flag5 = true;
                        }
                        bar2.Visible = flag2;
                        bar2.Enabled = flag1;
                    }
                    if (bar1 != null)
                    {
                        int num9 = num3 - size1.Width;
                        if ((num5 > num9) && (num9 > num1))
                        {
                            num5 = num9;
                        }
                        else if (num5 < num1)
                        {
                            num5 = num1;
                        }
                        int num10 = System.Math.Max(num3, (int)(num5 + size1.Width)) - 12;
                        num10 = System.Math.Max(num10, num5);
                        if (bar1.Minimum != num1)
                        {
                            bar1.Minimum = num1;
                        }
                        if (bar1.Maximum != num10)
                        {
                            bar1.Maximum = num10;
                        }
                        if (bar1.Value != num5)
                        {
                            bar1.Value = num5;
                        }
                        if (bar1.Visible != flag4)
                        {
                            flag5 = true;
                        }
                        bar1.Visible = flag4;
                        bar1.Enabled = flag3;
                    }
                    this.myUpdatingScrollBars = false;
                    if (flag5)
                    {
                        this.LayoutScrollBars(false);
                    }
                }
            }
        }

        private void updateSelectionHandles(Shapes.DiagramShape obj)
        {
            Shapes.GroupShape group1 = obj as Shapes.GroupShape;
            if (group1 != null)
            {
                Shapes.GroupEnumerator enumerator2 = group1.GetEnumerator();
                Shapes.GroupEnumerator enumerator1 = enumerator2.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    Shapes.DiagramShape obj1 = enumerator1.Current;
                    this.updateSelectionHandles(obj1);
                }
            }
            Shapes.DiagramShape obj2 = obj.SelectionObject;
            if (obj2 != null)
            {
                DiagramSelection selection1 = this.Selection;
                if (selection1.Contains(obj) && obj.CanView())
                {
                    obj2.AddSelectionHandles(selection1, obj);
                }
                else
                {
                    obj2.RemoveSelectionHandles(selection1);
                }
            }
        }

        public virtual void UpdateView()
        {
            this.UpdateBorderWidths();
            this.UpdateScrollBars();
            base.Invalidate();
        }


        [Category("Behavior"), DefaultValue(true), Description("Whether the user can copy selected objects.")]
        public bool AllowCopy
        {
            get
            {
                return this.myAllowCopy;
            }
            set
            {
                if (this.myAllowCopy != value)
                {
                    this.myAllowCopy = value;
                    this.RaisePropertyChangedEvent("AllowCopy");
                }
            }
        }

        [DefaultValue(true), Category("Behavior"), Description("Whether the user can delete selected objects.")]
        public bool AllowDelete
        {
            get
            {
                return this.myAllowDelete;
            }
            set
            {
                if (this.myAllowDelete != value)
                {
                    this.myAllowDelete = value;
                    this.RaisePropertyChangedEvent("AllowDelete");
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the user can drag the selection out of this view to another window.")]
        public bool AllowDragOut
        {
            get
            {
                return this.myAllowDragOut;
            }
            set
            {
                if (this.myAllowDragOut != value)
                {
                    this.myAllowDragOut = value;
                    this.RaisePropertyChangedEvent("AllowDragOut");
                }
            }
        }

        [Description("Whether the user can edit objects."), DefaultValue(true), Category("Behavior")]
        public bool AllowEdit
        {
            get
            {
                return this.myAllowEdit;
            }
            set
            {
                if (this.myAllowEdit != value)
                {
                    this.myAllowEdit = value;
                    this.RaisePropertyChangedEvent("AllowEdit");
                }
            }
        }

        [DefaultValue(true), Description("Whether the user can insert new objects."), Category("Behavior")]
        public bool AllowInsert
        {
            get
            {
                return this.myAllowInsert;
            }
            set
            {
                if (this.myAllowInsert != value)
                {
                    this.myAllowInsert = value;
                    this.RaisePropertyChangedEvent("AllowInsert");
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the user can type keystroke commands in this view.")]
        public bool AllowKey
        {
            get
            {
                return this.myAllowKey;
            }
            set
            {
                if (this.myAllowKey != value)
                {
                    this.myAllowKey = value;
                    this.RaisePropertyChangedEvent("AllowKey");
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the user can link ports.")]
        public bool AllowLink
        {
            get
            {
                return this.myAllowLink;
            }
            set
            {
                if (this.myAllowLink != value)
                {
                    this.myAllowLink = value;
                    this.RaisePropertyChangedEvent("AllowLink");
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the user can use the mouse in this view.")]
        public bool AllowMouse
        {
            get
            {
                return this.myAllowMouse;
            }
            set
            {
                if (this.myAllowMouse != value)
                {
                    this.myAllowMouse = value;
                    this.RaisePropertyChangedEvent("AllowMouse");
                }
            }
        }

        [Description("Whether the user can move selected objects."), Category("Behavior"), DefaultValue(true)]
        public bool AllowMove
        {
            get
            {
                return this.myAllowMove;
            }
            set
            {
                if (this.myAllowMove != value)
                {
                    this.myAllowMove = value;
                    this.RaisePropertyChangedEvent("AllowMove");
                }
            }
        }

        [Category("Behavior"), Description("Whether the user can reshape objects, if resizable."), DefaultValue(true)]
        public bool AllowReshape
        {
            get
            {
                return this.myAllowReshape;
            }
            set
            {
                if (this.myAllowReshape != value)
                {
                    this.myAllowReshape = value;
                    this.RaisePropertyChangedEvent("AllowReshape");
                }
            }
        }

        [Description("Whether the user can resize selected objects."), DefaultValue(true), Category("Behavior")]
        public bool AllowResize
        {
            get
            {
                return this.myAllowResize;
            }
            set
            {
                if (this.myAllowResize != value)
                {
                    this.myAllowResize = value;
                    this.RaisePropertyChangedEvent("AllowResize");
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the user can select objects, if visible.")]
        public bool AllowSelect
        {
            get
            {
                return this.myAllowSelect;
            }
            set
            {
                if (this.myAllowSelect != value)
                {
                    this.myAllowSelect = value;
                    this.RaisePropertyChangedEvent("AllowSelect");
                }
            }
        }

        [Description("The area around the original pan point outside of which the mouse will automatically cause the view to scroll."), Category("Behavior")]
        public virtual Size AutoPanRegion
        {
            get
            {
                return this.myAutoPanRegion;
            }
            set
            {
                if (this.myAutoPanRegion != value)
                {
                    if ((value.Width < 0) || (value.Height < 0))
                    {
                        throw new ArgumentException("New Size value for DiagramView.AutoPanRegion must have non-negative dimensions");
                    }
                    this.myAutoPanRegion = value;
                    this.RaisePropertyChangedEvent("AutoPanRegion");
                }
            }
        }

        [Category("Behavior"), DefaultValue(1000), Description("How long to wait in the autoscroll margin before performing any autoscrolling.")]
        public int AutoScrollDelay
        {
            get
            {
                return this.myAutoScrollDelay;
            }
            set
            {
                if ((this.myAutoScrollDelay != value) && (value >= 0))
                {
                    this.myAutoScrollDelay = value;
                    this.RaisePropertyChangedEvent("AutoScrollDelay");
                }
            }
        }

        [Description("The margin in the view where a mouse drag will automatically cause the view to scroll."), Category("Behavior")]
        public virtual Size AutoScrollRegion
        {
            get
            {
                return this.myAutoScrollRegion;
            }
            set
            {
                if (this.myAutoScrollRegion != value)
                {
                    if ((value.Width < 0) || (value.Height < 0))
                    {
                        throw new ArgumentException("New Size value for DiagramView.AutoScrollRegion must have non-negative dimensions");
                    }
                    this.myAutoScrollRegion = value;
                    this.RaisePropertyChangedEvent("AutoScrollRegion");
                }
            }
        }

        [Description("How long to wait before changing the DocPosition during autoscrolling."), DefaultValue(100), Category("Behavior")]
        public int AutoScrollTime
        {
            get
            {
                return this.myAutoScrollTime;
            }
            set
            {
                if ((this.myAutoScrollTime != value) && (value >= 0))
                {
                    this.myAutoScrollTime = value;
                    this.RaisePropertyChangedEvent("AutoScrollTime");
                }
            }
        }

        [Description("The 3D border style for this view, when BorderStyle is Fixed3D."), Category("Appearance"), DefaultValue(6)]
        public virtual System.Windows.Forms.Border3DStyle Border3DStyle
        {
            get
            {
                return this.myBorder3DStyle;
            }
            set
            {
                if (this.myBorder3DStyle != value)
                {
                    this.myBorder3DStyle = value;
                    this.RaisePropertyChangedEvent("Border3DStyle");
                }
            }
        }

        [Category("Appearance"), Description("The border style for this view."), DefaultValue(2)]
        public virtual System.Windows.Forms.BorderStyle BorderStyle
        {
            get
            {
                return this.myBorderStyle;
            }
            set
            {
                if (this.myBorderStyle != value)
                {
                    this.myBorderStyle = value;
                    this.UpdateBorderWidths();
                    this.RaisePropertyChangedEvent("BorderStyle");
                }
            }
        }

        [Category("Selection"), Description("The width of the pen used to draw the standard bounding handle"), DefaultValue((float)2f)]
        public virtual float BoundingHandlePenWidth
        {
            get
            {
                return this.myBoundingHandlePenWidth;
            }
            set
            {
                if (this.myBoundingHandlePenWidth != value)
                {
                    this.myBoundingHandlePenWidth = value;
                    this.RaisePropertyChangedEvent("BoundingHandlePenWidth");
                }
            }
        }

        [DefaultValue(800), Description("[Only supported in GoDiagram Pocket]"), Category("Behavior")]
        public virtual int ContextClickTime
        {
            get
            {
                return 800;
            }
            set
            {
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Control CornerControl
        {
            get
            {
                return this.myCorner;
            }
            set
            {
                Control control1 = this.myCorner;
                if (control1 != value)
                {
                    if (control1 != null)
                    {
                        base.Controls.Remove(control1);
                    }
                    this.myCorner = value;
                    if (value != null)
                    {
                        base.Controls.Add(value);
                    }
                    this.LayoutScrollBars(true);
                    this.RaisePropertyChangedEvent("CornerControl");
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return base.Cursor;
            }
            set
            {
                if (this.myDefaultCursor == null)
                {
                    this.myDefaultCursor = this.Cursor;
                }
                if (this.Cursor != value)
                {
                    base.Cursor = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual new System.Windows.Forms.Cursor DefaultCursor
        {
            get
            {
                if (this.myDefaultCursor == null)
                {
                    return this.Cursor;
                }
                return this.myDefaultCursor;
            }
            set
            {
                if (this.myDefaultCursor != value)
                {
                    this.myDefaultCursor = value;
                    this.RaisePropertyChangedEvent("DefaultCursor");
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual IDiagramTool DefaultTool
        {
            get
            {
                return this.myDefaultTool;
            }
            set
            {
                if (this.myDefaultTool != value)
                {
                    if (value == null)
                    {
                        throw new ArgumentException("New value for DiagramView.DefaultTool must not be null");
                    }
                    this.myDefaultTool = value;
                    this.RaisePropertyChangedEvent("DefaultTool");
                }
            }
        }

        [Browsable(false)]
        public override Rectangle DisplayRectangle
        {
            get
            {
                Size size1 = base.Size;
                int num1 = size1.Width - (2 * this.myBorderSize.Width);
                if ((this.VerticalScrollBar != null) && this.VerticalScrollBar.Visible)
                {
                    num1 -= this.myScrollBarWidth;
                }
                int num2 = size1.Height - (2 * this.myBorderSize.Height);
                if ((this.HorizontalScrollBar != null) && this.HorizontalScrollBar.Visible)
                {
                    num2 -= this.myScrollBarHeight;
                }
                return new Rectangle(this.myBorderSize.Width, this.myBorderSize.Height, System.Math.Max(1, num1), System.Math.Max(1, num2));
            }
        }

        [Browsable(false)]
        public RectangleF DocExtent
        {
            get
            {
                PointF tf1 = this.DocPosition;
                SizeF ef1 = this.DocExtentSize;
                return new RectangleF(tf1.X, tf1.Y, ef1.Width, ef1.Height);
            }
        }

        [Browsable(false)]
        public virtual SizeF DocExtentSize
        {
            get
            {
                Size size1 = this.DisplayRectangle.Size;
                return new SizeF((((float)size1.Width) / this.myHorizWorld) / this.myHorizScale, (((float)size1.Height) / this.myVertWorld) / this.myVertScale);
            }
        }

        [TypeConverter(typeof(PointFConverter)), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Description("The position in the document that this view is displaying.")]
        public virtual PointF DocPosition
        {
            get
            {
                return this.myOrigin;
            }
            set
            {
                PointF tf1 = this.myOrigin;
                PointF tf2 = this.LimitDocPosition(value);
                if (tf1 != tf2)
                {
                    this.myOrigin = tf2;
                    this.RaisePropertyChangedEvent("DocPosition");
                }
            }
        }

        [Category("Appearance"), DefaultValue((float)1f), Description("The scale at which this view displays its document.")]
        public virtual float DocScale
        {
            get
            {
                return this.myHorizScale;
            }
            set
            {
                float single1 = System.Math.Max((float)9E-09f, this.LimitDocScale(value));
                if ((this.myHorizScale != single1) || (this.myVertScale != single1))
                {
                    this.myHorizScale = single1;
                    this.myVertScale = single1;
                    this.RaisePropertyChangedEvent("DocScale");
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual DiagramDocument Document
        {
            get
            {
                return this.myDocument;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("New value for DiagramView.Document must not be null");
                }
                DiagramDocument document1 = this.Document;
                if (value != document1)
                {
                    if ((document1 != null) && (this.myDocChangedEventHandler != null))
                    {
                        document1.Changed -= this.myDocChangedEventHandler;
                    }
                    if (this.Tool != null)
                    {
                        this.DoCancelMouse();
                    }
                    this.DoEndEdit();
                    if (this.Selection != null)
                    {
                        this.Selection.Clear();
                    }
                    this.myDocument = value;
                    value.Changed += this.myDocChangedEventHandler;
                    this.RaisePropertyChangedEvent("Document");
                    this.InitializeLayersFromDocument();
                }
            }
        }

        [Browsable(false)]
        public virtual SizeF DocumentSize
        {
            get
            {
                DiagramDocument document1 = this.Document;
                if (document1 == null)
                {
                    return new SizeF();
                }
                SizeF ef1 = document1.Size;
                ef1.Width += System.Math.Abs(this.ShadowOffset.Width);
                ef1.Height += System.Math.Abs(this.ShadowOffset.Height);
                if (!this.ShowsNegativeCoordinates)
                {
                    PointF tf1 = document1.TopLeft;
                    if (tf1.X < 0f)
                    {
                        ef1.Width += tf1.X;
                    }
                    if (tf1.Y < 0f)
                    {
                        ef1.Height += tf1.Y;
                    }
                }
                return ef1;
            }
        }

        [Browsable(false)]
        public virtual PointF DocumentTopLeft
        {
            get
            {
                if (!this.ShowsNegativeCoordinates || (this.Document == null))
                {
                    return new PointF();
                }
                PointF tf1 = this.Document.TopLeft;
                SizeF ef1 = this.ShadowOffset;
                if (ef1.Width < 0f)
                {
                    tf1.X += ef1.Width;
                }
                if (ef1.Height < 0f)
                {
                    tf1.Y += ef1.Height;
                }
                return tf1;
            }
        }

        [DefaultValue(false), Description("Whether a user's drag of the selection occurs continuously instead of dragging an outline."), Category("Behavior")]
        public virtual bool DragsRealtime
        {
            get
            {
                return this.myDragsRealtime;
            }
            set
            {
                if (this.myDragsRealtime != value)
                {
                    this.myDragsRealtime = value;
                    this.RaisePropertyChangedEvent("DragsRealtime");
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual Shapes.DiagramControl EditControl
        {
            get
            {
                return this.myEditControl;
            }
            set
            {
                Shapes.DiagramControl control1 = this.myEditControl;
                if (control1 != value)
                {
                    if ((control1 != null) && (control1.View == this))
                    {
                        control1.Remove();
                    }
                    if (value != null)
                    {
                        this.myEditControl = value;
                        this.Layers.Default.Add(value);
                        this.myModalControl = value.GetControl(this);
                    }
                }
            }
        }

        [DefaultValue(false), Description("Whether the user drags newly dropped objects on a drag enter."), Category("Behavior")]
        public bool ExternalDragDropsOnEnter
        {
            get
            {
                return this.myExternalDragDropsOnEnter;
            }
            set
            {
                this.myExternalDragDropsOnEnter = value;
            }
        }

        [Browsable(false)]
        public InputEventArgs FirstInput
        {
            get
            {
                return this.myFirstInput;
            }
        }

        [Description("The size of each cell in the grid."), TypeConverter(typeof(SizeFConverter)), Browsable(false), Category("Grid"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual SizeF GridCellSize
        {
            get
            {
                return this.myGridCellSize;
            }
            set
            {
                if (this.myGridCellSize != value)
                {
                    if ((value.Width <= 0f) || (value.Height <= 0f))
                    {
                        throw new ArgumentException("New SizeF value for DiagramView.GridCellSize must have positive dimensions");
                    }
                    this.myGridCellSize = value;
                    this.RaisePropertyChangedEvent("GridCellSize");
                }
            }
        }

        [Category("Grid"), Description("The color used in drawing the grid lines.")]
        public virtual Color GridColor
        {
            get
            {
                return this.myGridColor;
            }
            set
            {
                if (this.myGridColor != value)
                {
                    this.myGridColor = value;
                    this.RaisePropertyChangedEvent("GridColor");
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Grid"), Description("The origin for the grid."), TypeConverter(typeof(PointFConverter)), Browsable(false)]
        public virtual PointF GridOrigin
        {
            get
            {
                return this.myGridOrigin;
            }
            set
            {
                if (this.myGridOrigin != value)
                {
                    this.myGridOrigin = value;
                    this.RaisePropertyChangedEvent("GridOrigin");
                }
            }
        }

        [DefaultValue(0), Description("The pen dash style used in drawing the grid lines."), Category("Grid")]
        public virtual DashStyle GridPenDashStyle
        {
            get
            {
                return this.myGridPenDashStyle;
            }
            set
            {
                if ((this.myGridPenDashStyle != value) && (value != DashStyle.Custom))
                {
                    this.myGridPenDashStyle = value;
                    this.RaisePropertyChangedEvent("GridPenDashStyle");
                }
            }
        }

        [Category("Grid"), Description("The width of the pen used in drawing the grid lines."), DefaultValue((float)1f)]
        public virtual float GridPenWidth
        {
            get
            {
                return this.myGridPenWidth;
            }
            set
            {
                if (this.myGridPenWidth != value)
                {
                    this.myGridPenWidth = value;
                    this.RaisePropertyChangedEvent("GridPenWidth");
                }
            }
        }

        [Category("Grid"), Description("The interactive dragging behavior for positioning objects."), DefaultValue(0)]
        public virtual DiagramViewSnapStyle GridSnapDrag
        {
            get
            {
                return this.mySnapDrag;
            }
            set
            {
                if (this.mySnapDrag != value)
                {
                    this.mySnapDrag = value;
                    this.RaisePropertyChangedEvent("GridSnapDrag");
                }
            }
        }

        [Description("The interactive resizing behavior for resizing objects."), Category("Grid"), DefaultValue(0)]
        public virtual DiagramViewSnapStyle GridSnapResize
        {
            get
            {
                return this.mySnapResize;
            }
            set
            {
                if (this.mySnapResize != value)
                {
                    this.mySnapResize = value;
                    this.RaisePropertyChangedEvent("GridSnapResize");
                }
            }
        }

        [Description("The appearance style of the grid."), Category("Grid"), DefaultValue(0)]
        public virtual DiagramViewGridStyle GridStyle
        {
            get
            {
                return this.myGridStyle;
            }
            set
            {
                if (this.myGridStyle != value)
                {
                    this.myGridStyle = value;
                    this.RaisePropertyChangedEvent("GridStyle");
                }
            }
        }

        [Category("Selection"), DefaultValue(false), Description("Whether the selection disappears when this view loses focus.")]
        public virtual bool HidesSelection
        {
            get
            {
                return this.myHidesSelection;
            }
            set
            {
                if (this.myHidesSelection != value)
                {
                    this.myHidesSelection = value;
                    this.RaisePropertyChangedEvent("HidesSelection");
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual HScrollBar HorizontalScrollBar
        {
            get
            {
                return this.myHorizScroll;
            }
            set
            {
                HScrollBar bar1 = this.myHorizScroll;
                if (bar1 != value)
                {
                    if (bar1 != null)
                    {
                        bar1.Scroll -= this.myHorizScrollHandler;
                        base.Controls.Remove(bar1);
                    }
                    this.myHorizScroll = value;
                    if (value != null)
                    {
                        value.SmallChange = this.ScrollSmallChange.Width;
                        base.Controls.Add(value);
                        value.Scroll += this.myHorizScrollHandler;
                    }
                    this.LayoutScrollBars(true);
                    this.RaisePropertyChangedEvent("HorizontalScrollBar");
                }
            }
        }

        [Description("How long a mouse should stay at one spot before a hover event occurs."), Category("Behavior"), DefaultValue(1000)]
        public int HoverDelay
        {
            get
            {
                return this.myHoverDelay;
            }
            set
            {
                if (this.myHoverDelay != value)
                {
                    this.myHoverDelay = value;
                    this.RaisePropertyChangedEvent("HoverDelay");
                }
            }
        }

        [Description("The ImageList from which GoImage objects can draw an image."), Category("Appearance"), DefaultValue((string)null)]
        public virtual System.Windows.Forms.ImageList ImageList
        {
            get
            {
                return this.myImageList;
            }
            set
            {
                if (this.myImageList != value)
                {
                    this.myImageList = value;
                    this.RaisePropertyChangedEvent("ImageList");
                }
            }
        }

        [Category("Appearance"), Description("How images are rendered when scaled or stretched"), DefaultValue(2)]
        public System.Drawing.Drawing2D.InterpolationMode InterpolationMode
        {
            get
            {
                return this.myInterpolationMode;
            }
            set
            {
                if (this.myInterpolationMode != value)
                {
                    this.myInterpolationMode = value;
                    this.RaisePropertyChangedEvent("InterpolationMode");
                }
            }
        }

        [Browsable(false)]
        public virtual bool IsEditing
        {
            get
            {
                return (this.EditControl != null);
            }
        }

        [Browsable(false)]
        public virtual bool IsPrinting
        {
            get
            {
                return (this.myPrintInfo != null);
            }
        }

        [Browsable(false)]
        public InputEventArgs LastInput
        {
            get
            {
                return this.myLastInput;
            }
        }

        [Browsable(false)]
        public LayerCollection Layers
        {
            get
            {
                return this.myLayers;
            }
        }

        [DefaultValue(1000000), Category("Selection"), Description("The maximum number of selected objects")]
        public virtual int MaximumSelectionCount
        {
            get
            {
                return this.myMaximumSelectionCount;
            }
            set
            {
                if ((value != this.myMaximumSelectionCount) && (value >= 0))
                {
                    this.myMaximumSelectionCount = value;
                    this.RaisePropertyChangedEvent("MaximumSelectionCount");
                    while (this.Selection.Count > value)
                    {
                        Shapes.DiagramShape obj1 = this.Selection.Last;
                        this.Selection.Remove(obj1);
                    }
                }
            }
        }

        [Browsable(false)]
        public virtual IList MouseDownTools
        {
            get
            {
                if (this.myMouseDownTools == null)
                {
                    this.myMouseDownTools = new ArrayList();
                    this.myMouseDownTools.Add(new ToolAction(this));
                    this.myMouseDownTools.Add(new ToolContext(this));
                    this.myMouseDownTools.Add(new ToolPanning(this));
                    this.myMouseDownTools.Add(new ToolRelinking(this));
                    this.myMouseDownTools.Add(new ToolResizing(this));
                    this.myMouseDownTools.Add(new ToolLinkingNew(this));
                }
                return this.myMouseDownTools;
            }
        }

        [Browsable(false)]
        public virtual IList MouseMoveTools
        {
            get
            {
                if (this.myMouseMoveTools == null)
                {
                    this.myMouseMoveTools = new ArrayList();
                    this.myMouseMoveTools.Add(new ToolDragging(this));
                    this.myMouseMoveTools.Add(new ToolRubberBanding(this));
                }
                return this.myMouseMoveTools;
            }
        }

        [Browsable(false)]
        public virtual IList MouseUpTools
        {
            get
            {
                if (this.myMouseUpTools == null)
                {
                    this.myMouseUpTools = new ArrayList();
                    this.myMouseUpTools.Add(new ToolSelecting(this));
                }
                return this.myMouseUpTools;
            }
        }

        private Type _LineType;
        [DefaultValue(typeof(Shapes.LineGraph)), Description("The Type of the link to be created when linking."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Behavior"), Browsable(false)]
        public virtual Type LineType
        {
            get
            {
                return this._LineType;
            }
            set
            {
                if (this._LineType != value)
                {
                    if (value == null)
                    {
                        throw new ArgumentException("New Type value for DiagramView.NewLinkClass must implement IGoLink");
                    }
                    this._LineType = value;
                    this.RaisePropertyChangedEvent("NewLinkClass");
                }
            }
        }

        [Description("The handle color for objects when the view does not have focus."), Category("Selection")]
        public virtual Color NoFocusSelectionColor
        {
            get
            {
                return this.myNoFocusSelectionColor;
            }
            set
            {
                if (this.myNoFocusSelectionColor != value)
                {
                    this.myNoFocusSelectionColor = value;
                    this.RaisePropertyChangedEvent("NoFocusSelectionColor");
                }
            }
        }

        [DefaultValue((float)0.24f), Category("Appearance"), Description("The scale at which greeked objects paint something simple.")]
        public virtual float PaintGreekScale
        {
            get
            {
                return this.myPaintGreekScale;
            }
            set
            {
                if (this.myPaintGreekScale != value)
                {
                    this.myPaintGreekScale = value;
                    this.RaisePropertyChangedEvent("PaintGreekScale");
                }
            }
        }

        [DefaultValue((float)0.13f), Description("The scale at which greeked objects paint nothing."), Category("Appearance")]
        public virtual float PaintNothingScale
        {
            get
            {
                return this.myPaintNothingScale;
            }
            set
            {
                if (this.myPaintNothingScale != value)
                {
                    this.myPaintNothingScale = value;
                    this.RaisePropertyChangedEvent("PaintNothingScale");
                }
            }
        }

        [Category("Behavior"), Description("The distance at which potential links will snap to valid ports."), DefaultValue((float)100f)]
        public virtual float PortGravity
        {
            get
            {
                return this.myPortGravity;
            }
            set
            {
                if (this.myPortGravity != value)
                {
                    if (value <= 0f)
                    {
                        throw new ArgumentException("New distance value for DiagramView.PortGravity must be positive");
                    }
                    this.myPortGravity = value;
                    this.RaisePropertyChangedEvent("PortGravity");
                }
            }
        }

        [Category("Selection"), Description("The handle color for the primary selection.")]
        public virtual Color PrimarySelectionColor
        {
            get
            {
                return this.myPrimarySelectionColor;
            }
            set
            {
                if (this.myPrimarySelectionColor != value)
                {
                    this.myPrimarySelectionColor = value;
                    this.RaisePropertyChangedEvent("PrimarySelectionColor");
                }
            }
        }

        [Browsable(false)]
        public virtual SizeF PrintDocumentSize
        {
            get
            {
                RectangleF ef1 = this.ComputeDocumentBounds();
                PointF tf1 = new PointF(ef1.X + ef1.Width, ef1.Y + ef1.Height);
                PointF tf2 = this.PrintDocumentTopLeft;
                SizeF ef2 = DiagramTool.SubtractPoints(tf1, tf2);
                ef2.Width += System.Math.Abs(this.ShadowOffset.Width);
                ef2.Height += System.Math.Abs(this.ShadowOffset.Height);
                return ef2;
            }
        }

        [Browsable(false)]
        public virtual PointF PrintDocumentTopLeft
        {
            get
            {
                PointF tf1 = this.Document.TopLeft;
                SizeF ef1 = this.ShadowOffset;
                if (ef1.Width < 0f)
                {
                    tf1.X += ef1.Width;
                }
                if (ef1.Height < 0f)
                {
                    tf1.Y += ef1.Height;
                }
                return tf1;
            }
        }

        [DefaultValue((float)0.8f), Description("The scale at which we should print."), Category("Appearance")]
        public virtual float PrintScale
        {
            get
            {
                return this.myPrintScale;
            }
            set
            {
                if (this.myPrintScale != value)
                {
                    if (value <= 0f)
                    {
                        throw new ArgumentException("New value for DiagramView.PrintScale must be positive");
                    }
                    this.myPrintScale = value;
                    this.RaisePropertyChangedEvent("PrintScale");
                }
            }
        }

        [Category("Selection"), Description("The width of the pen used to draw the standard resize handle"), DefaultValue((float)1f)]
        public virtual float ResizeHandlePenWidth
        {
            get
            {
                return this.myResizeHandlePenWidth;
            }
            set
            {
                if (this.myResizeHandlePenWidth != value)
                {
                    this.myResizeHandlePenWidth = value;
                    this.RaisePropertyChangedEvent("ResizeHandlePenWidth");
                }
            }
        }

        [TypeConverter(typeof(SizeFConverter)), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("The default size for new resize handles."), Category("Selection")]
        public virtual SizeF ResizeHandleSize
        {
            get
            {
                return this.myResizeHandleSize;
            }
            set
            {
                if (this.myResizeHandleSize != value)
                {
                    this.myResizeHandleSize = value;
                    this.RaisePropertyChangedEvent("ResizeHandleSize");
                }
            }
        }

        [Category("Behavior"), Description("The distance to scroll when scrolling a small amount.")]
        public virtual Size ScrollSmallChange
        {
            get
            {
                return this.myScrollSmallChange;
            }
            set
            {
                if (this.myScrollSmallChange != value)
                {
                    if ((value.Width <= 0) || (value.Height <= 0))
                    {
                        throw new ArgumentException("New Size value for DiagramView.ScrollSmallChange must have positive dimensions");
                    }
                    this.myScrollSmallChange = value;
                    HScrollBar bar1 = this.HorizontalScrollBar;
                    if ((bar1 != null) && (bar1.SmallChange != this.myScrollSmallChange.Width))
                    {
                        bar1.SmallChange = this.myScrollSmallChange.Width;
                    }
                    VScrollBar bar2 = this.VerticalScrollBar;
                    if ((bar2 != null) && (bar2.SmallChange != this.myScrollSmallChange.Height))
                    {
                        bar2.SmallChange = this.myScrollSmallChange.Height;
                    }
                    this.RaisePropertyChangedEvent("ScrollSmallChange");
                }
            }
        }

        [Category("Selection"), Description("The handle color for objects other than the primary selection.")]
        public virtual Color SecondarySelectionColor
        {
            get
            {
                return this.mySecondarySelectionColor;
            }
            set
            {
                if (this.mySecondarySelectionColor != value)
                {
                    this.mySecondarySelectionColor = value;
                    this.RaisePropertyChangedEvent("SecondarySelectionColor");
                }
            }
        }

        [Browsable(false)]
        public virtual DiagramSelection Selection
        {
            get
            {
                return this.mySelection;
            }
        }

        [Category("Selection"), DefaultValue(true), Description("Whether the user typing a letter or digit will select the next node starting with that character.")]
        public bool SelectsByFirstChar
        {
            get
            {
                return this.mySelectsByFirstChar;
            }
            set
            {
                if (this.mySelectsByFirstChar != value)
                {
                    this.mySelectsByFirstChar = value;
                    this.RaisePropertyChangedEvent("SelectsByFirstChar");
                }
            }
        }

        [Description("The color used for drawing drop shadows."), Category("Shadows")]
        public virtual Color ShadowColor
        {
            get
            {
                return this.myShadowColor;
            }
            set
            {
                if (this.myShadowColor != value)
                {
                    this.myShadowColor = value;
                    this.RaisePropertyChangedEvent("ShadowColor");
                }
            }
        }

        [TypeConverter(typeof(SizeFConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("The offset distance for drop shadows."), Category("Shadows"), Browsable(false)]
        public virtual SizeF ShadowOffset
        {
            get
            {
                return this.myShadowOffset;
            }
            set
            {
                if (this.myShadowOffset != value)
                {
                    this.myShadowOffset = value;
                    this.RaisePropertyChangedEvent("ShadowOffset");
                }
            }
        }

        [DefaultValue(2), Category("Appearance"), Description("The visibility policy for the horizontal scroll bar.")]
        public virtual DiagramViewScrollBarVisibility HorizontalScrollBarVisibility
        {
            get
            {
                return this.myHorizScrollVisibility;
            }
            set
            {
                if (this.myHorizScrollVisibility != value)
                {
                    this.myHorizScrollVisibility = value;
                    this.LayoutScrollBars(true);
                    this.RaisePropertyChangedEvent("ShowHorizontalScrollBar");
                }
            }
        }

        [DefaultValue(true), Description("Whether any parts of the document at negative coordinates can be seen or scrolled to."), Category("Behavior")]
        public virtual bool ShowsNegativeCoordinates
        {
            get
            {
                return this.myShowsNegativeCoordinates;
            }
            set
            {
                if (this.myShowsNegativeCoordinates != value)
                {
                    this.myShowsNegativeCoordinates = value;
                    this.RaisePropertyChangedEvent("ShowsNegativeCoordinates");
                }
            }
        }

        [Category("Appearance"), DefaultValue(2), Description("The visibility policy for the vertical scroll bar.")]
        public virtual DiagramViewScrollBarVisibility VerticalScrollBarVisibility
        {
            get
            {
                return this.myVertScrollVisibility;
            }
            set
            {
                if (this.myVertScrollVisibility != value)
                {
                    this.myVertScrollVisibility = value;
                    this.LayoutScrollBars(true);
                    this.RaisePropertyChangedEvent("ShowVerticalScrollBar");
                }
            }
        }

        [DefaultValue(2), Category("Appearance"), Description("How nicely lines are drawn")]
        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode
        {
            get
            {
                return this.mySmoothingMode;
            }
            set
            {
                if (this.mySmoothingMode != value)
                {
                    this.mySmoothingMode = value;
                    this.RaisePropertyChangedEvent("SmoothingMode");
                }
            }
        }

        [Description("How nicely text is rendered"), DefaultValue(5), Category("Appearance")]
        public System.Drawing.Text.TextRenderingHint TextRenderingHint
        {
            get
            {
                return this.myTextRenderingHint;
            }
            set
            {
                if (this.myTextRenderingHint != value)
                {
                    this.myTextRenderingHint = value;
                    this.RaisePropertyChangedEvent("TextRenderingHint");
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual IDiagramTool Tool
        {
            get
            {
                return this.myTool;
            }
            set
            {
                if (this.myTool != value)
                {
                    if (this.myTool != null)
                    {
                        this.myTool.Stop();
                    }
                    if (value == null)
                    {
                        this.myTool = this.DefaultTool;
                    }
                    else
                    {
                        this.myTool = value;
                    }
                    if (this.myTool != null)
                    {
                        this.myTool.Start();
                    }
                    this.RaisePropertyChangedEvent("Tool");
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual System.Windows.Forms.ToolTip ToolTip
        {
            get
            {
                return this.myToolTip;
            }
            set
            {
                if (this.myToolTip != value)
                {
                    this.myToolTip = value;
                    this.RaisePropertyChangedEvent("ToolTip");
                }
            }
        }

        public static float Version
        {
            get
            {
                return 2.2f;
            }
        }

        public static string VersionName
        {
            get
            {
                return "2.2.2.0";
            }
            set
            {
                DiagramView.myVersionName = value;
                DiagramView.myVersionAssembly = Assembly.GetCallingAssembly();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual VScrollBar VerticalScrollBar
        {
            get
            {
                return this.myVertScroll;
            }
            set
            {
                VScrollBar bar1 = this.myVertScroll;
                if (bar1 != value)
                {
                    if (bar1 != null)
                    {
                        bar1.Scroll -= this.myVertScrollHandler;
                        base.Controls.Remove(bar1);
                    }
                    this.myVertScroll = value;
                    if (value != null)
                    {
                        value.SmallChange = this.ScrollSmallChange.Height;
                        base.Controls.Add(value);
                        value.Scroll += this.myVertScrollHandler;
                    }
                    this.LayoutScrollBars(true);
                    this.RaisePropertyChangedEvent("VerticalScrollBar");
                }
            }
        }

        internal SizeF WorldScale
        {
            get
            {
                return new SizeF(this.myHorizWorld, this.myVertWorld);
            }
        }


        private bool myAllowCopy;
        private bool myAllowDelete;
        private bool myAllowDragOut;
        private bool myAllowEdit;
        private bool myAllowInsert;
        private bool myAllowKey;
        private bool myAllowLink;
        private bool myAllowMouse;
        private bool myAllowMove;
        private bool myAllowReshape;
        private bool myAllowResize;
        private bool myAllowSelect;
        private Size myAutoPanRegion;
        private int myAutoScrollDelay;
        [NonSerialized]
        private Point myAutoScrollPoint;
        private Size myAutoScrollRegion;
        private int myAutoScrollTime;
        [NonSerialized]
        private System.Threading.Timer myAutoScrollTimer;
        [NonSerialized]
        private bool myAutoScrollTimerEnabled;
        [NonSerialized]
        private SolidBrush myBackgroundBrush;
        private System.Windows.Forms.Border3DStyle myBorder3DStyle;
        private Size myBorderSize;
        private System.Windows.Forms.BorderStyle myBorderStyle;
        private float myBoundingHandlePenWidth;
        [NonSerialized]
        internal Bitmap myBuffer;
        [NonSerialized]
        private bool myCancelMouseDown;
        private Control myCorner;
        internal DiagramViewLicenseProvider.DiagramViewLicense myCurrentResult;
        [NonSerialized]
        private System.Windows.Forms.Cursor myDefaultCursor;
        private IDiagramTool myDefaultTool;
        [NonSerialized]
        private ChangedEventHandler myDocChangedEventHandler;
        private DiagramDocument myDocument;
        private bool myDragsRealtime;
        [NonSerialized]
        private Shapes.DiagramControl myEditControl;
        private bool myExternalDragDropsOnEnter;
        private Shapes.DiagramShape myExternalDragImage;
        private InputEventArgs myFirstInput;
        [NonSerialized]
        private ArrayList myGoControls;
        [NonSerialized]
        internal Graphics myGraphics;
        private SizeF myGridCellSize;
        private Color myGridColor;
        private PointF myGridOrigin;
        [NonSerialized]
        private Pen myGridPen;
        private DashStyle myGridPenDashStyle;
        private float myGridPenWidth;
        private DiagramViewGridStyle myGridStyle;
        private bool myHidesSelection;
        private float myHorizScale;
        private HScrollBar myHorizScroll;
        private ScrollEventHandler myHorizScrollHandler;
        [NonSerialized]
        private float myHorizWorld;
        private int myHoverDelay;
        private Point myHoverPoint;
        [NonSerialized]
        private System.Threading.Timer myHoverTimer;
        [NonSerialized]
        private bool myHoverTimerEnabled;
        private System.Windows.Forms.ImageList myImageList;
        private System.Drawing.Drawing2D.InterpolationMode myInterpolationMode;
        private InputEventArgs myLastInput;
        private LayerCollection myLayers;
        private int myMaximumSelectionCount;
        [NonSerialized]
        private Control myModalControl;
        private ArrayList myMouseDownTools;
        private ArrayList myMouseMoveTools;
        private ArrayList myMouseUpTools;
        private Color myNoFocusSelectionColor;
        private PointF myOrigin;
        [NonSerialized]
        internal PaintEventArgs myPaintEventArgs;
        private float myPaintGreekScale;
        private float myPaintNothingScale;
        [NonSerialized]
        private bool myPanning;
        [NonSerialized]
        private Point myPanningOrigin;
        private float myPortGravity;
        [NonSerialized]
        private bool myPretendInternalDrag;
        private Rectangle myPrevXorRect;
        private bool myPrevXorRectValid;
        private Color myPrimarySelectionColor;
        [NonSerialized]
        private PrintInfo myPrintInfo;
        private float myPrintScale;
        [NonSerialized]
        private Queue myQueuedEvents;
        private float myResizeHandlePenWidth;
        private SizeF myResizeHandleSize;
        [NonSerialized]
        private EventHandler mySafeOnDocumentChangedDelegate;
        private int myScrollBarHeight;
        private int myScrollBarWidth;
        private Size myScrollSmallChange;
        private Color mySecondarySelectionColor;
        private DiagramSelection mySelection;
        private bool mySelectsByFirstChar;
        [NonSerialized]
        private SolidBrush myShadowBrush;
        private Color myShadowColor;
        private SizeF myShadowOffset;
        [NonSerialized]
        private Pen myShadowPen;
        private DiagramViewScrollBarVisibility myHorizScrollVisibility;
        private bool myShowsNegativeCoordinates;
        private DiagramViewScrollBarVisibility myVertScrollVisibility;
        private System.Drawing.Drawing2D.SmoothingMode mySmoothingMode;
        private DiagramViewSnapStyle mySnapDrag;
        private DiagramViewSnapStyle mySnapResize;
        private int mySuppressPaint;
        [NonSerialized]
        private PointF[][] myTempArrays;
        private System.Drawing.Text.TextRenderingHint myTextRenderingHint;
        private IDiagramTool myTool;
        [NonSerialized]
        private System.Windows.Forms.ToolTip myToolTip;
        private bool myUpdatingScrollBars;
        internal static Assembly myVersionAssembly;
        internal static string myVersionName;
        private float myVertScale;
        private VScrollBar myVertScroll;
        private ScrollEventHandler myVertScrollHandler;
        [NonSerialized]
        private float myVertWorld;

        internal class ExternalDragImage : Shapes.DiagramImage
        {
            public ExternalDragImage()
            {
                this.myOffset = new SizeF(0f, 0f);
            }


            public override PointF Location
            {
                get
                {
                    return new PointF(base.Left + this.myOffset.Width, base.Top + this.myOffset.Height);
                }
                set
                {
                    base.Position = new PointF(value.X - this.myOffset.Width, value.Y - this.myOffset.Height);
                }
            }

            public SizeF Offset
            {
                get
                {
                    return this.myOffset;
                }
                set
                {
                    this.myOffset = value;
                }
            }


            private SizeF myOffset;
        }

        [Serializable]
        internal sealed class PrintInfo
        {
            public PrintInfo()
            {
            }


            internal int CurPage;
            internal RectangleF DocRect;
            internal float HorizScale;
            internal int NumPagesAcross;
            internal int NumPagesDown;
            internal SizeF PrintSize;
            internal float VertScale;
        }
    }
}
