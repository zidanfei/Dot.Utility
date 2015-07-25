using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class HandleGraph : DiagramGraph, IShapeHandle
    {
        public HandleGraph()
        {
            this.myHandleID = 0;
            this.mySelectedObject = null;
            this._Style = ShapeStyle.Rectangle;
            this.myCursor = null;
            base.Size = new SizeF(0f, 0f);
        }

        public override void AddSelectionHandles(DiagramSelection sel, Dot.Utility.Media.Diagram.Shapes.DiagramShape selectedObj)
        {
        }

        private void ComputeTrianglePoints(PointF[] v)
        {
            RectangleF ef1 = this.Bounds;
            switch (this.Style)
            {
                case ShapeStyle.TriangleTopLeft:
                    {
                        v[0].X = ef1.X + (ef1.Width / 2f);
                        v[0].Y = ef1.Y + ef1.Height;
                        v[1].X = ef1.X;
                        v[1].Y = ef1.Y;
                        v[2].X = ef1.X + ef1.Width;
                        v[2].Y = ef1.Y + (ef1.Height / 2f);
                        return;
                    }
                case ShapeStyle.TriangleTopRight:
                    {
                        v[0].X = ef1.X;
                        v[0].Y = ef1.Y + (ef1.Height / 2f);
                        v[1].X = ef1.X + ef1.Width;
                        v[1].Y = ef1.Y;
                        v[2].X = ef1.X + (ef1.Width / 2f);
                        v[2].Y = ef1.Y + ef1.Height;
                        return;
                    }
                case ShapeStyle.TriangleBottomRight:
                    {
                        v[0].X = ef1.X + (ef1.Width / 2f);
                        v[0].Y = ef1.Y;
                        v[1].X = ef1.X + ef1.Width;
                        v[1].Y = ef1.Y + ef1.Height;
                        v[2].X = ef1.X;
                        v[2].Y = ef1.Y + (ef1.Height / 2f);
                        return;
                    }
                case ShapeStyle.TriangleBottomLeft:
                    {
                        v[0].X = ef1.X + ef1.Width;
                        v[0].Y = ef1.Y + (ef1.Height / 2f);
                        v[1].X = ef1.X;
                        v[1].Y = ef1.Y + ef1.Height;
                        v[2].X = ef1.X + (ef1.Width / 2f);
                        v[2].Y = ef1.Y;
                        return;
                    }
                case ShapeStyle.TriangleMiddleTop:
                    {
                        v[0].X = ef1.X;
                        v[0].Y = ef1.Y + ef1.Height;
                        v[1].X = ef1.X + (ef1.Width / 2f);
                        v[1].Y = ef1.Y;
                        v[2].X = ef1.X + ef1.Width;
                        v[2].Y = ef1.Y + ef1.Height;
                        return;
                    }
                case ShapeStyle.TriangleMiddleRight:
                    {
                        v[0].X = ef1.X;
                        v[0].Y = ef1.Y;
                        v[1].X = ef1.X + ef1.Width;
                        v[1].Y = ef1.Y + (ef1.Height / 2f);
                        v[2].X = ef1.X;
                        v[2].Y = ef1.Y + ef1.Height;
                        return;
                    }
                case ShapeStyle.TriangleMiddleBottom:
                    {
                        v[0].X = ef1.X + ef1.Width;
                        v[0].Y = ef1.Y;
                        v[1].X = ef1.X + (ef1.Width / 2f);
                        v[1].Y = ef1.Y + ef1.Height;
                        v[2].X = ef1.X;
                        v[2].Y = ef1.Y;
                        return;
                    }
                case ShapeStyle.TriangleMiddleLeft:
                    {
                        v[0].X = ef1.X + ef1.Width;
                        v[0].Y = ef1.Y + ef1.Height;
                        v[1].X = ef1.X;
                        v[1].Y = ef1.Y + (ef1.Height / 2f);
                        v[2].X = ef1.X + ef1.Width;
                        v[2].Y = ef1.Y;
                        return;
                    }
            }
        }

        public override bool ContainsPoint(PointF p)
        {
            RectangleF ef1 = this.Bounds;
            float single1 = base.InternalPenWidth;
            DiagramShape.InflateRect(ref ef1, single1 / 2f, single1 / 2f);
            if (!DiagramShape.ContainsRect(ef1, p))
            {
                return false;
            }
            if (this.HandleID == 0)
            {
                DiagramShape.InflateRect(ref ef1, -single1, -single1);
                return !DiagramShape.ContainsRect(ef1, p);
            }
            return true;
        }

        public override Dot.Utility.Media.Diagram.Shapes.DiagramShape CopyObject(CopyDictionary env)
        {
            return null;
        }

        public virtual System.Windows.Forms.Cursor GetCursorForHandle(int id)
        {
            int num1 = id;
            if (num1 <= 0x20)
            {
                switch (num1)
                {
                    case 0:
                        {
                            return null;
                        }
                    case 1:
                        {
                            return Cursors.SizeAll;
                        }
                    case 2:
                        {
                            return Cursors.SizeNWSE;
                        }
                    case 3:
                    case 5:
                    case 6:
                    case 7:
                        {
                            goto Label_00B6;
                        }
                    case 4:
                        {
                            return Cursors.SizeNESW;
                        }
                    case 8:
                        {
                            return Cursors.SizeNWSE;
                        }
                    case 0x10:
                        {
                            return Cursors.SizeNESW;
                        }
                    case 0x20:
                        {
                            return Cursors.SizeNS;
                        }
                }
            }
            else if (num1 <= 0x80)
            {
                if (num1 == 0x40)
                {
                    return Cursors.SizeWE;
                }
                if (num1 == 0x80)
                {
                    return Cursors.SizeNS;
                }
            }
            else
            {
                if (num1 == 0x100)
                {
                    return Cursors.SizeWE;
                }
                switch (num1)
                {
                    case 0x400:
                        {
                            return Cursors.Hand;
                        }
                    case 0x401:
                        {
                            return Cursors.Hand;
                        }
                }
            }
        Label_00B6:
            return Cursors.SizeAll;
        }

        public override GraphicsPath MakePath()
        {
            GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
            RectangleF ef1 = this.Bounds;
            switch (this.Style)
            {
                case ShapeStyle.None:
                    {
                        path1.AddLine(ef1.X, ef1.Y, ef1.X, ef1.Y);
                        return path1;
                    }
                case ShapeStyle.Ellipse:
                    {
                        path1.AddEllipse(ef1.X, ef1.Y, ef1.Width, ef1.Height);
                        return path1;
                    }
                case ShapeStyle.Diamond:
                    {
                        PointF[] tfArray1 = new PointF[4];
                        tfArray1[0].X = ef1.X + (ef1.Width / 2f);
                        tfArray1[0].Y = ef1.Y;
                        tfArray1[1].X = ef1.X + ef1.Width;
                        tfArray1[1].Y = ef1.Y + (ef1.Height / 2f);
                        tfArray1[2].X = tfArray1[0].X;
                        tfArray1[2].Y = ef1.Y + ef1.Height;
                        tfArray1[3].X = ef1.X;
                        tfArray1[3].Y = tfArray1[1].Y;
                        path1.AddPolygon(tfArray1);
                        return path1;
                    }
                case ShapeStyle.TriangleTopLeft:
                case ShapeStyle.TriangleTopRight:
                case ShapeStyle.TriangleBottomRight:
                case ShapeStyle.TriangleBottomLeft:
                case ShapeStyle.TriangleMiddleTop:
                case ShapeStyle.TriangleMiddleRight:
                case ShapeStyle.TriangleMiddleBottom:
                case ShapeStyle.TriangleMiddleLeft:
                    {
                        PointF[] tfArray2 = new PointF[3];
                        this.ComputeTrianglePoints(tfArray2);
                        path1.AddPolygon(tfArray2);
                        return path1;
                    }
            }
            path1.AddRectangle(ef1);
            return path1;
        }

        public override bool OnMouseOver(InputEventArgs evt, DiagramView view)
        {
            DiagramShape obj1 = this.HandledObject;
            if (((obj1 == null) || !view.CanResizeObjects()) || (!obj1.CanResize() && !obj1.CanReshape()))
            {
                return false;
            }
            System.Windows.Forms.Cursor cursor1 = this.Cursor;
            if (cursor1 == null)
            {
                cursor1 = this.GetCursorForHandle(this.HandleID);
            }
            if (cursor1 == null)
            {
                return false;
            }
            if (view.Cursor != cursor1)
            {
                view.Cursor = cursor1;
            }
            return true;
        }

        public override void Paint(Graphics g, DiagramView view)
        {
            RectangleF ef1 = this.Bounds;
            switch (this.Style)
            {
                case ShapeStyle.None:
                    {
                        return;
                    }
                case ShapeStyle.Ellipse:
                    {
                        DiagramGraph.DrawEllipse(g, view, this.Pen, this.Brush, ef1.X, ef1.Y, ef1.Width, ef1.Height);
                        return;
                    }
                case ShapeStyle.Diamond:
                    {
                        PointF[] tfArray1 = view.AllocTempPointArray(4);
                        tfArray1[0].X = ef1.X + (ef1.Width / 2f);
                        tfArray1[0].Y = ef1.Y;
                        tfArray1[1].X = ef1.X + ef1.Width;
                        tfArray1[1].Y = ef1.Y + (ef1.Height / 2f);
                        tfArray1[2].X = tfArray1[0].X;
                        tfArray1[2].Y = ef1.Y + ef1.Height;
                        tfArray1[3].X = ef1.X;
                        tfArray1[3].Y = tfArray1[1].Y;
                        DiagramGraph.DrawPolygon(g, view, this.Pen, this.Brush, tfArray1);
                        view.FreeTempPointArray(tfArray1);
                        return;
                    }
                case ShapeStyle.TriangleTopLeft:
                case ShapeStyle.TriangleTopRight:
                case ShapeStyle.TriangleBottomRight:
                case ShapeStyle.TriangleBottomLeft:
                case ShapeStyle.TriangleMiddleTop:
                case ShapeStyle.TriangleMiddleRight:
                case ShapeStyle.TriangleMiddleBottom:
                case ShapeStyle.TriangleMiddleLeft:
                    {
                        PointF[] tfArray2 = view.AllocTempPointArray(3);
                        this.ComputeTrianglePoints(tfArray2);
                        DiagramGraph.DrawPolygon(g, view, this.Pen, this.Brush, tfArray2);
                        view.FreeTempPointArray(tfArray2);
                        return;
                    }
            }
            DiagramGraph.DrawRectangle(g, view, this.Pen, this.Brush, ef1.X, ef1.Y, ef1.Width, ef1.Height);
        }


        [Description("The Cursor to be shown when the mouse is over this handle.")]
        public System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return this.myCursor;
            }
            set
            {
                this.myCursor = value;
            }
        }

        [Description("Just returns the HandleShape itself.")]
        public Dot.Utility.Media.Diagram.Shapes.DiagramShape DiagramShape
        {
            get
            {
                return this;
            }
        }

        [Description("The object that actually gets the handles.")]
        public Dot.Utility.Media.Diagram.Shapes.DiagramShape HandledObject
        {
            get
            {
                return this.SelectedObject.SelectionObject;
            }
        }

        [Description("An identifier for this handle, often a DiagramShape spot value.")]
        public int HandleID
        {
            get
            {
                return this.myHandleID;
            }
            set
            {
                this.myHandleID = value;
            }
        }

        [Description("The selected object that this handle is marking.")]
        public Dot.Utility.Media.Diagram.Shapes.DiagramShape SelectedObject
        {
            get
            {
                return this.mySelectedObject;
            }
            set
            {
                this.mySelectedObject = value;
            }
        }

        public override Dot.Utility.Media.Diagram.Shapes.DiagramShape SelectionObject
        {
            get
            {
                return null;
            }
        }

        [Description("The appearance style.")]
        public ShapeStyle Style
        {
            get
            {
                return this._Style;
            }
            set
            {
                this._Style = value;
            }
        }


        [NonSerialized]
        private System.Windows.Forms.Cursor myCursor;
        private int myHandleID;
        private Dot.Utility.Media.Diagram.Shapes.DiagramShape mySelectedObject;
        private ShapeStyle _Style;
    }
}
