using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    public class ToolPanning : DiagramTool
    {
        public ToolPanning(DiagramView v)
            : base(v)
        {
            this.myAutoPan = true;
            this.myModal = false;
            this.myActive = false;
            this.myOriginSet = false;
            this.myOrigin = new Point();
            this.myPaintHandler = null;
        }

        public override bool CanStart()
        {
            InputEventArgs args1 = base.LastInput;
            if ((args1.Alt || args1.Control) || args1.Shift)
            {
                return false;
            }
            if (this.AutoPan)
            {
                return (args1.Buttons == MouseButtons.Middle);
            }
            if (args1.Buttons != MouseButtons.Left)
            {
                return false;
            }
            Shapes.DiagramShape obj1 = base.View.PickObject(true, false, args1.DocPoint, true);
            return (obj1 == null);
        }

        public override void DoKeyDown()
        {
            base.StopTool();
        }

        private void DoManualPan()
        {
            PointF tf1 = base.View.DocPosition;
            Size size1 = new Size(base.LastInput.ViewPoint.X - this.myLastViewPoint.X, base.LastInput.ViewPoint.Y - this.myLastViewPoint.Y);
            SizeF ef1 = base.View.ConvertViewToDoc(size1);
            this.myLastViewPoint = base.LastInput.ViewPoint;
            base.View.DocPosition = new PointF(tf1.X + ef1.Width, tf1.Y + ef1.Height);
        }

        public override void DoMouseDown()
        {
            if (this.AutoPan)
            {
                base.DoMouseDown();
            }
            else
            {
                this.Active = true;
            }
        }

        public override void DoMouseMove()
        {
            if (this.AutoPan)
            {
                if (this.myOriginSet)
                {
                    Size size1 = new Size(0x10, 0x10);
                    int num1 = size1.Width;
                    int num2 = size1.Height;
                    int num3 = base.LastInput.ViewPoint.X - this.Origin.X;
                    int num4 = base.LastInput.ViewPoint.Y - this.Origin.Y;
                    if (num3 < -num1)
                    {
                        if (num4 < -num2)
                        {
                            base.View.Cursor = Cursors.PanNW;
                        }
                        else if (num4 > num2)
                        {
                            base.View.Cursor = Cursors.PanSW;
                        }
                        else
                        {
                            base.View.Cursor = Cursors.PanWest;
                        }
                    }
                    else if (num3 > num1)
                    {
                        if (num4 < -num2)
                        {
                            base.View.Cursor = Cursors.PanNE;
                        }
                        else if (num4 > num2)
                        {
                            base.View.Cursor = Cursors.PanSE;
                        }
                        else
                        {
                            base.View.Cursor = Cursors.PanEast;
                        }
                    }
                    else if (num4 < -num2)
                    {
                        base.View.Cursor = Cursors.PanNorth;
                    }
                    else if (num4 > num2)
                    {
                        base.View.Cursor = Cursors.PanSouth;
                    }
                    else
                    {
                        base.View.Cursor = Cursors.NoMove2D;
                    }
                    base.View.DoAutoPan(this.Origin, base.LastInput.ViewPoint);
                }
            }
            else if (!this.Active)
            {
                if (!this.Modal)
                {
                    this.Active = true;
                }
            }
            else
            {
                this.DoManualPan();
            }
        }

        public override void DoMouseUp()
        {
            if (this.AutoPan)
            {
                if (!this.myOriginSet)
                {
                    this.Origin = base.LastInput.ViewPoint;
                    this.SetPaintingOriginMarker(true);
                }
                else
                {
                    base.StopTool();
                }
            }
            else if (this.Modal)
            {
                this.Active = false;
            }
            else
            {
                base.StopTool();
            }
        }

        public override void DoMouseWheel()
        {
            base.StopTool();
        }

        private void HandlePaint(object sender, PaintEventArgs evt)
        {
            Cursor cursor1 = Cursors.NoMove2D;
            int num1 = cursor1.Size.Width;
            int num2 = cursor1.Size.Height;
            cursor1.Draw(evt.Graphics, this.OriginRect);
        }

        private void SetPaintingOriginMarker(bool b)
        {
            if (b)
            {
                this.myPaintHandler = new PaintEventHandler(this.HandlePaint);
                base.View.Paint += this.myPaintHandler;
                base.View.Invalidate(this.OriginRect);
            }
            else if (this.myPaintHandler != null)
            {
                base.View.Paint -= this.myPaintHandler;
                this.myPaintHandler = null;
                base.View.Invalidate(this.OriginRect);
            }
        }

        public override void Start()
        {
            if (this.AutoPan)
            {
                base.View.Cursor = Cursors.NoMove2D;
                if (!this.myOriginSet)
                {
                    return;
                }
                this.SetPaintingOriginMarker(true);
            }
            else
            {
                base.View.Cursor = Cursors.SizeAll;
            }
        }

        public override void Stop()
        {
            if (this.AutoPan)
            {
                this.myOriginSet = false;
                base.View.StopAutoScroll();
                base.View.Cursor = base.View.DefaultCursor;
                this.SetPaintingOriginMarker(false);
            }
            else
            {
                this.Active = false;
                base.View.Cursor = base.View.DefaultCursor;
            }
        }


        private bool Active
        {
            get
            {
                return this.myActive;
            }
            set
            {
                if (this.myActive != value)
                {
                    this.myActive = value;
                    if (value)
                    {
                        this.myLastViewPoint = base.LastInput.ViewPoint;
                    }
                }
            }
        }

        public virtual bool AutoPan
        {
            get
            {
                return this.myAutoPan;
            }
            set
            {
                this.myAutoPan = value;
            }
        }

        public virtual bool Modal
        {
            get
            {
                return this.myModal;
            }
            set
            {
                this.myModal = value;
            }
        }

        public Point Origin
        {
            get
            {
                return this.myOrigin;
            }
            set
            {
                if (this.myOrigin != value)
                {
                    this.myOrigin = value;
                    this.myOriginSet = true;
                }
            }
        }

        private Rectangle OriginRect
        {
            get
            {
                Cursor cursor1 = Cursors.NoMove2D;
                int num1 = cursor1.Size.Width;
                int num2 = cursor1.Size.Height;
                return new Rectangle(this.Origin.X - (num1 / 2), this.Origin.Y - (num2 / 2), num1, num2);
            }
        }


        [NonSerialized]
        private bool myActive;
        private bool myAutoPan;
        [NonSerialized]
        private Point myLastViewPoint;
        private bool myModal;
        [NonSerialized]
        private Point myOrigin;
        [NonSerialized]
        private bool myOriginSet;
        [NonSerialized]
        private PaintEventHandler myPaintHandler;
    }
}
