using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public abstract class DiagramShape
    {
        static DiagramShape()
        {
            DiagramShape.NullRect = new RectangleF();
        }

        protected DiagramShape()
        {
            this.myLayer = null;
            this.myParent = null;
            this.myBounds = new RectangleF(0f, 0f, 10f, 10f);
            this.myInternalFlags = 0x8017f;
            this.myObservers = null;
        }

        public virtual void AddObserver(DiagramShape obj)
        {
            if (obj != null)
            {
                if (this.myObservers == null)
                {
                    this.myObservers = new DiagramShapeCollection();
                }
                if (!this.myObservers.Contains(obj))
                {
                    this.myObservers.Add(obj);
                    this.Changed(0x3f6, 0, null, DiagramShape.NullRect, 0, obj, DiagramShape.NullRect);
                }
            }
        }

        public virtual void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            this.RemoveSelectionHandles(sel);
            if (!this.CanResize())
            {
                sel.CreateBoundingHandle(this, selectedObj);
            }
            else
            {
                RectangleF ef1 = this.Bounds;
                float single1 = ef1.X;
                float single2 = ef1.X + (ef1.Width / 2f);
                float single3 = ef1.X + ef1.Width;
                float single4 = ef1.Y;
                float single5 = ef1.Y + (ef1.Height / 2f);
                float single6 = ef1.Y + ef1.Height;
                sel.CreateResizeHandle(this, selectedObj, new PointF(single1, single4), 2, true);
                sel.CreateResizeHandle(this, selectedObj, new PointF(single3, single4), 4, true);
                sel.CreateResizeHandle(this, selectedObj, new PointF(single3, single6), 8, true);
                sel.CreateResizeHandle(this, selectedObj, new PointF(single1, single6), 0x10, true);
                if (this.CanReshape())
                {
                    sel.CreateResizeHandle(this, selectedObj, new PointF(single2, single4), 0x20, true);
                    sel.CreateResizeHandle(this, selectedObj, new PointF(single3, single5), 0x40, true);
                    sel.CreateResizeHandle(this, selectedObj, new PointF(single2, single6), 0x80, true);
                    sel.CreateResizeHandle(this, selectedObj, new PointF(single1, single5), 0x100, true);
                }
            }
        }

        public virtual bool CanCopy()
        {
            if (!this.Copyable)
            {
                return false;
            }
            if (this.Layer != null)
            {
                return this.Layer.CanCopyObjects();
            }
            return true;
        }

        public virtual bool CanDelete()
        {
            if (!this.Deletable)
            {
                return false;
            }
            if (this.Layer != null)
            {
                return this.Layer.CanDeleteObjects();
            }
            return true;
        }

        public virtual bool CanEdit()
        {
            if (!this.Editable)
            {
                return false;
            }
            if (this.Layer != null)
            {
                return this.Layer.CanEditObjects();
            }
            return true;
        }

        public virtual bool CanMove()
        {
            if (!this.Movable)
            {
                return false;
            }
            if (this.Layer != null)
            {
                return this.Layer.CanMoveObjects();
            }
            return true;
        }

        public virtual bool CanPrint()
        {
            if (!this.Printable)
            {
                return false;
            }
            if (this.Parent != null)
            {
                return this.Parent.CanPrint();
            }
            if (this.Layer != null)
            {
                return this.Layer.CanPrintObjects();
            }
            return true;
        }

        public virtual bool CanReshape()
        {
            if (!this.Reshapable)
            {
                return false;
            }
            if (this.Layer != null)
            {
                return this.Layer.CanReshapeObjects();
            }
            return true;
        }

        public virtual bool CanResize()
        {
            if (!this.Resizable)
            {
                return false;
            }
            if (this.Layer != null)
            {
                return this.Layer.CanResizeObjects();
            }
            return true;
        }

        public virtual bool CanSelect()
        {
            if (!this.Selectable)
            {
                return false;
            }
            if (this.Layer != null)
            {
                return this.Layer.CanSelectObjects();
            }
            return true;
        }

        public virtual bool CanView()
        {
            if (!this.Visible)
            {
                return false;
            }
            if (this.Parent != null)
            {
                return this.Parent.CanView();
            }
            if (this.Layer != null)
            {
                return this.Layer.CanViewObjects();
            }
            return true;
        }

        public virtual void Changed(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            if (!this.SuspendsUpdates)
            {
                if (this.InvalidBounds)
                {
                    RectangleF ef1 = this.Bounds;
                }
                DiagramLayer layer1 = this.Layer;
                if ((layer1 != null) && (layer1.LayerCollectionContainer != null))
                {
                    layer1.LayerCollectionContainer.RaiseChanged(0x385, subhint, this, oldI, oldVal, oldRect, newI, newVal, newRect);
                }
                if (this.myObservers != null)
                {
                    CollectionEnumerator enumerator1 = this.myObservers.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        enumerator1.Current.OnObservedChanged(this, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
                    }
                }
            }
        }

        public virtual void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 1000:
                    {
                        return;
                    }
                case 0x3e9:
                    {
                        this.myBounds = e.GetRect(undo);
                        this.InvalidateViews();
                        return;
                    }
                case 0x3eb:
                    {
                        this.Visible = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3ec:
                    {
                        this.Selectable = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3ed:
                    {
                        this.Movable = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3ee:
                    {
                        this.Copyable = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3ef:
                    {
                        this.Resizable = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3f0:
                    {
                        this.Reshapable = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3f1:
                    {
                        this.Deletable = (bool)e.GetValue(undo);
                        return;
                    }
                case 1010:
                    {
                        this.Editable = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3f3:
                    {
                        this.AutoRescales = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3f4:
                    {
                        this.ResizesRealtime = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3f5:
                    {
                        this.Shadowed = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3f6:
                    {
                        DiagramShape obj1 = e.NewValue as DiagramShape;
                        if (!undo)
                        {
                            this.AddObserver(obj1);
                            return;
                        }
                        this.RemoveObserver(obj1);
                        return;
                    }
                case 0x3f7:
                    {
                        DiagramShape obj2 = e.OldValue as DiagramShape;
                        if (!undo)
                        {
                            this.RemoveObserver(obj2);
                            return;
                        }
                        this.AddObserver(obj2);
                        return;
                    }
                case 0x3f8:
                    {
                        this.DragsNode = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x3f9:
                    {
                        this.Printable = (bool)e.GetValue(undo);
                        return;
                    }
            }
            throw new ArgumentException("Unknown GoChangedEventArgs subhint--override DiagramShape.ChangeValue to handle the case: " + e.SubHint.ToString(NumberFormatInfo.InvariantInfo));
        }

        public virtual void Changing(int subhint)
        {
            if (!this.SuspendsUpdates)
            {
                DiagramDocument document1 = this.Document;
                if (document1 != null)
                {
                    document1.RaiseChanging(0x385, subhint, this);
                }
            }
        }

        protected virtual RectangleF ComputeBounds()
        {
            return this.Bounds;
        }

        public virtual PointF ComputeMove(PointF origLoc, PointF newLoc)
        {
            return newLoc;
        }

        public virtual RectangleF ComputeResize(RectangleF origRect, PointF newPoint, int handle, SizeF min, SizeF max, bool reshape)
        {
            float single1 = origRect.X;
            float single2 = origRect.Y;
            float single3 = origRect.X + origRect.Width;
            float single4 = origRect.Y + origRect.Height;
            float single5 = 1f;
            if (!reshape)
            {
                float single6 = origRect.Width;
                float single7 = origRect.Height;
                if (single6 <= 0f)
                {
                    single6 = 1f;
                }
                if (single7 <= 0f)
                {
                    single7 = 1f;
                }
                single5 = single7 / single6;
            }
            RectangleF ef1 = origRect;
            int num1 = handle;
            if (num1 <= 0x10)
            {
                switch (num1)
                {
                    case 2:
                        {
                            ef1.X = System.Math.Max(newPoint.X, (float)(single3 - max.Width));
                            ef1.X = System.Math.Min(ef1.X, (float)(single3 - min.Width));
                            ef1.Width = single3 - ef1.X;
                            if (ef1.Width <= 0f)
                            {
                                ef1.Width = 1f;
                            }
                            ef1.Y = System.Math.Max(newPoint.Y, (float)(single4 - max.Height));
                            ef1.Y = System.Math.Min(ef1.Y, (float)(single4 - min.Height));
                            ef1.Height = single4 - ef1.Y;
                            if (ef1.Height <= 0f)
                            {
                                ef1.Height = 1f;
                            }
                            if (!reshape)
                            {
                                float single8 = ef1.Height / ef1.Width;
                                if (single5 < single8)
                                {
                                    ef1.Height = single5 * ef1.Width;
                                    ef1.Y = single4 - ef1.Height;
                                    return ef1;
                                }
                                ef1.Width = ef1.Height / single5;
                                ef1.X = single3 - ef1.Width;
                            }
                            return ef1;
                        }
                    case 3:
                        {
                            return ef1;
                        }
                    case 4:
                        {
                            ef1.Width = System.Math.Min((float)(newPoint.X - single1), max.Width);
                            ef1.Width = System.Math.Max(ef1.Width, min.Width);
                            ef1.Y = System.Math.Max(newPoint.Y, (float)(single4 - max.Height));
                            ef1.Y = System.Math.Min(ef1.Y, (float)(single4 - min.Height));
                            ef1.Height = single4 - ef1.Y;
                            if (ef1.Height <= 0f)
                            {
                                ef1.Height = 1f;
                            }
                            if (!reshape)
                            {
                                float single9 = ef1.Height / ef1.Width;
                                if (single5 < single9)
                                {
                                    ef1.Height = single5 * ef1.Width;
                                    ef1.Y = single4 - ef1.Height;
                                    return ef1;
                                }
                                ef1.Width = ef1.Height / single5;
                            }
                            return ef1;
                        }
                    case 8:
                        {
                            ef1.Width = System.Math.Min((float)(newPoint.X - single1), max.Width);
                            ef1.Width = System.Math.Max(ef1.Width, min.Width);
                            ef1.Height = System.Math.Min((float)(newPoint.Y - single2), max.Height);
                            ef1.Height = System.Math.Max(ef1.Height, min.Height);
                            if (!reshape)
                            {
                                float single11 = ef1.Height / ef1.Width;
                                if (single5 < single11)
                                {
                                    ef1.Height = single5 * ef1.Width;
                                    return ef1;
                                }
                                ef1.Width = ef1.Height / single5;
                            }
                            return ef1;
                        }
                    case 0x10:
                        {
                            ef1.X = System.Math.Max(newPoint.X, (float)(single3 - max.Width));
                            ef1.X = System.Math.Min(ef1.X, (float)(single3 - min.Width));
                            ef1.Width = single3 - ef1.X;
                            if (ef1.Width <= 0f)
                            {
                                ef1.Width = 1f;
                            }
                            ef1.Height = System.Math.Min((float)(newPoint.Y - single2), max.Height);
                            ef1.Height = System.Math.Max(ef1.Height, min.Height);
                            if (!reshape)
                            {
                                float single10 = ef1.Height / ef1.Width;
                                if (single5 < single10)
                                {
                                    ef1.Height = single5 * ef1.Width;
                                    return ef1;
                                }
                                ef1.Width = ef1.Height / single5;
                                ef1.X = single3 - ef1.Width;
                            }
                            return ef1;
                        }
                }
                return ef1;
            }
            if (num1 <= 0x40)
            {
                if (num1 == 0x20)
                {
                    ef1.Y = System.Math.Max(newPoint.Y, (float)(single4 - max.Height));
                    ef1.Y = System.Math.Min(ef1.Y, (float)(single4 - min.Height));
                    ef1.Height = single4 - ef1.Y;
                    if (ef1.Height <= 0f)
                    {
                        ef1.Height = 1f;
                    }
                    return ef1;
                }
                if (num1 == 0x40)
                {
                    ef1.Width = System.Math.Min((float)(newPoint.X - single1), max.Width);
                    ef1.Width = System.Math.Max(ef1.Width, min.Width);
                }
                return ef1;
            }
            if (num1 == 0x80)
            {
                ef1.Height = System.Math.Min((float)(newPoint.Y - single2), max.Height);
                ef1.Height = System.Math.Max(ef1.Height, min.Height);
                return ef1;
            }
            if (num1 == 0x100)
            {
                ef1.X = System.Math.Max(newPoint.X, (float)(single3 - max.Width));
                ef1.X = System.Math.Min(ef1.X, (float)(single3 - min.Width));
                ef1.Width = single3 - ef1.X;
                if (ef1.Width <= 0f)
                {
                    ef1.Width = 1f;
                }
            }
            return ef1;
        }

        public virtual bool ContainedByRectangle(RectangleF r)
        {
            return DiagramShape.ContainsRect(r, this.Bounds);
        }

        public virtual bool ContainsPoint(PointF p)
        {
            return DiagramShape.ContainsRect(this.Bounds, p);
        }

        internal static bool ContainsRect(RectangleF a, PointF b)
        {
            if (((a.X <= b.X) && (b.X <= (a.X + a.Width))) && (a.Y <= b.Y))
            {
                return (b.Y <= (a.Y + a.Height));
            }
            return false;
        }

        internal static bool ContainsRect(RectangleF a, RectangleF b)
        {
            if ((((a.X <= b.X) && ((b.X + b.Width) <= (a.X + a.Width))) && ((a.Y <= b.Y) && ((b.Y + b.Height) <= (a.Y + a.Height)))) && (a.Width >= 0f))
            {
                return (a.Height >= 0f);
            }
            return false;
        }

        public virtual void CopyNewValueForRedo(ChangedEventArgs e)
        {
        }

        public virtual DiagramShape CopyObject(CopyDictionary env)
        {
            DiagramShape obj1 = (DiagramShape)env[this];
            if (obj1 != null)
            {
                return null;
            }
            obj1 = (DiagramShape)base.MemberwiseClone();
            env[this] = obj1;
            obj1.myLayer = null;
            obj1.myParent = null;
            if ((this.myObservers != null) && (this.myObservers.Count > 0))
            {
                env.Delayeds.Add(this);
            }
            obj1.myObservers = null;
            return obj1;
        }

        public virtual void CopyObjectDelayed(CopyDictionary env, DiagramShape newobj)
        {
            CollectionEnumerator enumerator1 = this.Observers.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                DiagramShape obj1 = enumerator1.Current;
                DiagramShape obj2 = env[obj1] as DiagramShape;
                newobj.AddObserver(obj2);
            }
        }

        public virtual void CopyOldValueForUndo(ChangedEventArgs e)
        {
        }

        public virtual IShapeHandle CreateBoundingHandle()
        {
            HandleGraph handle1 = new HandleGraph();
            RectangleF ef1 = this.Bounds;
            SizeF ef2 = new SizeF(1f, 1f);
            DiagramDocument document1 = this.Document;
            if (document1 != null)
            {
                SizeF ef3 = document1.WorldScale;
                ef2.Width /= ef3.Width;
                ef2.Height /= ef3.Height;
            }
            DiagramShape.InflateRect(ref ef1, ef2.Width, ef2.Height);
            handle1.Bounds = ef1;
            return handle1;
        }

        public virtual DiagramControl CreateEditor(DiagramView view)
        {
            return null;
        }

        public virtual IShapeHandle CreateResizeHandle(int handleid)
        {
            return new HandleGraph();
        }

        public virtual void DoBeginEdit(DiagramView view)
        {
        }

        public virtual void DoEndEdit(DiagramView view)
        {
        }

        public virtual void DoMove(DiagramView view, PointF origLoc, PointF newLoc)
        {
            PointF tf1 = this.ComputeMove(origLoc, newLoc);
            this.Location = tf1;
        }

        public virtual void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            if (evttype == InputState.Cancel)
            {
                this.Bounds = origRect;
            }
            else
            {
                RectangleF ef1 = this.ComputeResize(origRect, newPoint, whichHandle, min, max, this.CanReshape() && !view.LastInput.Shift);
                if (this.ResizesRealtime)
                {
                    this.Bounds = ef1;
                }
                else
                {
                    Rectangle rectangle1 = view.ConvertDocToView(ef1);
                    view.DrawXorBox(rectangle1, evttype != InputState.Finish);
                    if (evttype == InputState.Finish)
                    {
                        this.Bounds = ef1;
                    }
                }
            }
        }

        public virtual RectangleF ExpandPaintBounds(RectangleF rect, DiagramView view)
        {
            return rect;
        }

        public static DiagramShape FindCommonParent(DiagramShape a, DiagramShape b)
        {
            if (a == b)
            {
                return a;
            }
            if (b != null)
            {
                for (DiagramShape obj1 = a; obj1 != null; obj1 = obj1.Parent)
                {
                    for (DiagramShape obj2 = b; obj2 != null; obj2 = obj2.Parent)
                    {
                        if (obj2 == obj1)
                        {
                            return obj2;
                        }
                    }
                }
            }
            return null;
        }

        public virtual bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
        {
            return DiagramShape.GetNearestIntersectionPoint(this.Bounds, p1, p2, out result);
        }

        public static bool GetNearestIntersectionPoint(RectangleF rect, PointF p1, PointF p2, out PointF result)
        {
            PointF tf5;
            PointF tf1 = new PointF(rect.X, rect.Y);
            PointF tf2 = new PointF(rect.X + rect.Width, rect.Y);
            PointF tf3 = new PointF(rect.X, rect.Y + rect.Height);
            PointF tf4 = new PointF(rect.X + rect.Width, rect.Y + rect.Height);
            float single1 = p1.X;
            float single2 = p1.Y;
            float single3 = 1E+21f;
            PointF tf6 = new PointF();
            if (StrokeGraph.NearestIntersectionOnLine(tf1, tf2, p1, p2, out tf5))
            {
                float single4 = ((tf5.X - single1) * (tf5.X - single1)) + ((tf5.Y - single2) * (tf5.Y - single2));
                if (single4 < single3)
                {
                    single3 = single4;
                    tf6 = tf5;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf2, tf4, p1, p2, out tf5))
            {
                float single5 = ((tf5.X - single1) * (tf5.X - single1)) + ((tf5.Y - single2) * (tf5.Y - single2));
                if (single5 < single3)
                {
                    single3 = single5;
                    tf6 = tf5;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf4, tf3, p1, p2, out tf5))
            {
                float single6 = ((tf5.X - single1) * (tf5.X - single1)) + ((tf5.Y - single2) * (tf5.Y - single2));
                if (single6 < single3)
                {
                    single3 = single6;
                    tf6 = tf5;
                }
            }
            if (StrokeGraph.NearestIntersectionOnLine(tf3, tf1, p1, p2, out tf5))
            {
                float single7 = ((tf5.X - single1) * (tf5.X - single1)) + ((tf5.Y - single2) * (tf5.Y - single2));
                if (single7 < single3)
                {
                    single3 = single7;
                    tf6 = tf5;
                }
            }
            result = tf6;
            return (single3 < 1E+21f);
        }

        public virtual PointF GetRectangleSpotLocation(RectangleF r, int spot)
        {
            PointF tf1 = new PointF(r.X, r.Y);
            switch (spot)
            {
                case 1:
                    tf1.X += (r.Width / 2f);
                    tf1.Y += (r.Height / 2f);
                    return tf1;
                case 2:
                    return tf1;
                case 4:
                    tf1.X += r.Width;
                    return tf1;
                case 8:
                    tf1.X += r.Width;
                    tf1.Y += r.Height;
                    return tf1;
                case 0x10:
                    tf1.Y += r.Height;
                    return tf1;
                case 0x20:
                    tf1.X += (r.Width / 2f);
                    return tf1;
                case 0x40:
                    tf1.X += r.Width;
                    tf1.Y += (r.Height / 2f);
                    return tf1;
                case 0x80:
                    tf1.X += (r.Width / 2f);
                    tf1.Y += r.Height;
                    return tf1;
                case 0x100:
                    tf1.Y += (r.Height / 2f);
                    return tf1;
                default:

                    return tf1;
            }
        }

        public virtual Brush GetShadowBrush(DiagramView view)
        {
            if (view != null)
            {
                return view.GetShadowBrush();
            }
            return null;
        }

        public virtual SizeF GetShadowOffset(DiagramView view)
        {
            if (view != null)
            {
                return view.ShadowOffset;
            }
            return new SizeF();
        }

        public virtual Pen GetShadowPen(DiagramView view, float width)
        {
            if (view != null)
            {
                return view.GetShadowPen(width);
            }
            return null;
        }

        public virtual PointF GetSpotLocation(int spot)
        {
            RectangleF ef1 = this.Bounds;
            return this.GetRectangleSpotLocation(ef1, spot);
        }

        public virtual string GetToolTip(DiagramView view)
        {
            return null;
        }

        internal static void InflateRect(ref RectangleF a, float w, float h)
        {
            a.X -= w;
            a.Width += (w * 2f);
            a.Y -= h;
            a.Height += (h * 2f);
        }

        internal static bool IntersectsRect(RectangleF a, RectangleF b)
        {
            float single1 = a.Width;
            float single2 = a.Height;
            float single3 = b.Width;
            float single4 = b.Height;
            if (((single3 >= 0f) && (single4 >= 0f)) && ((single1 >= 0f) && (single2 >= 0f)))
            {
                float single5 = a.X;
                float single6 = a.Y;
                float single7 = b.X;
                float single8 = b.Y;
                single3 += single7;
                single4 += single8;
                single1 += single5;
                single2 += single6;
                if ((((single3 <= single7) || (single3 >= single5)) && ((single4 <= single8) || (single4 >= single6))) && ((single1 <= single5) || (single1 >= single7)))
                {
                    if (single2 > single6)
                    {
                        return (single2 >= single8);
                    }
                    return true;
                }
            }
            return false;
        }

        public void InvalidateViews()
        {
            if (this.Parent != null)
            {
                this.Parent.InvalidatePaintBounds();
            }
            this.Changed(1000, 0, null, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
        }

        internal bool IsApprox(double x, double y)
        {
            double num1 = 0.5;
            DiagramDocument document1 = this.Document;
            if (document1 != null)
            {
                num1 = document1.WorldEpsilon;
            }
            return (System.Math.Abs((double)(x - y)) < num1);
        }

        internal bool IsApprox(float x, float y)
        {
            float single1 = 0.5f;
            DiagramDocument document1 = this.Document;
            if (document1 != null)
            {
                single1 = document1.WorldEpsilon;
            }
            return (System.Math.Abs((float)(x - y)) < single1);
        }

        public bool IsChildOf(DiagramShape obj)
        {
            if (obj is GroupShape)
            {
                for (GroupShape group1 = this.Parent; group1 != null; group1 = group1.Parent)
                {
                    if (group1 == obj)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal void MakeDiamondResizeHandle(IShapeHandle handle, int spot)
        {
            HandleGraph handle1 = handle.DiagramShape as HandleGraph;
            if (handle1 != null)
            {
                handle1.Style = ShapeStyle.Diamond;
                if (!(handle1.SelectedObject is IDiagramLine))
                {
                    handle1.Brush = DiagramGraph.Brushes_Yellow;
                }
                RectangleF ef1 = handle1.Bounds;
                DiagramShape.InflateRect(ref ef1, ef1.Width / 6f, ef1.Height / 6f);
                handle1.Bounds = ef1;
                if (spot == 0x40)
                {
                    handle1.Cursor = Cursors.SizeWE;
                }
                else if (spot == 0x80)
                {
                    handle1.Cursor = Cursors.SizeNS;
                }
                else if (spot == 8)
                {
                    handle1.Cursor = Cursors.SizeNWSE;
                }
                else if (spot == 4)
                {
                    handle1.Cursor = Cursors.SizeNESW;
                }
                else if (spot != 0)
                {
                    handle1.Cursor = Cursors.SizeAll;
                }
            }
        }

        public static RectangleF MakeRect(PointF p)
        {
            return new RectangleF(p.X, p.Y, 0f, 0f);
        }

        public static RectangleF MakeRect(SizeF s)
        {
            return new RectangleF(0f, 0f, s.Width, s.Height);
        }

        public static RectangleF MakeRect(float x)
        {
            return new RectangleF(x, 0f, 0f, 0f);
        }

        protected virtual void OnBoundsChanged(RectangleF old)
        {
        }

        public virtual bool OnContextClick(InputEventArgs evt, DiagramView view)
        {
            return false;
        }

        public virtual bool OnDoubleClick(InputEventArgs evt, DiagramView view)
        {
            return false;
        }

        public virtual void OnGotSelection(DiagramSelection sel)
        {
            if (this.IsInDocument && this.CanView())
            {
                DiagramShape obj1 = this.SelectionObject;
                if (obj1 != null)
                {
                    obj1.AddSelectionHandles(sel, this);
                }
            }
        }

        public virtual bool OnHover(InputEventArgs evt, DiagramView view)
        {
            return false;
        }

        protected virtual void OnLayerChanged(DiagramLayer oldlayer, DiagramLayer newlayer, DiagramShape mainObj)
        {
        }

        public virtual void OnLostSelection(DiagramSelection sel)
        {
            DiagramShape obj1 = this.SelectionObject;
            if (obj1 != null)
            {
                obj1.RemoveSelectionHandles(sel);
            }
        }

        public virtual bool OnMouseOver(InputEventArgs evt, DiagramView view)
        {
            return false;
        }

        protected virtual void OnObservedChanged(DiagramShape observed, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
        }

        protected virtual void OnParentChanged(GroupShape oldgroup, GroupShape newgroup)
        {
        }

        public virtual bool OnSingleClick(InputEventArgs evt, DiagramView view)
        {
            return false;
        }

        public virtual void Paint(Graphics g, DiagramView view)
        {
        }

        public virtual DiagramShape Pick(PointF p, bool selectableOnly)
        {
            if (this.CanView())
            {
                if (!this.ContainsPoint(p))
                {
                    return null;
                }
                if (!selectableOnly)
                {
                    return this;
                }
                if (this.CanSelect())
                {
                    return this;
                }
                DiagramShape obj1 = this;
                while (obj1.Parent != null)
                {
                    obj1 = obj1.Parent;
                    if (obj1 == null)
                    {
                        return null;
                    }
                    if (obj1.CanSelect())
                    {
                        return obj1;
                    }
                }
            }
            return null;
        }

        public void Remove()
        {
            DiagramLayer layer1 = this.Layer;
            if (layer1 != null)
            {
                layer1.Remove(this);
            }
            else
            {
                GroupShape group1 = this.Parent;
                if (group1 != null)
                {
                    group1.Remove(this);
                }
            }
        }

        public virtual void RemoveObserver(DiagramShape obj)
        {
            if ((obj != null) && ((this.myObservers != null) && this.myObservers.Contains(obj)))
            {
                this.myObservers.Remove(obj);
                this.Changed(0x3f7, 0, obj, DiagramShape.NullRect, 0, null, DiagramShape.NullRect);
            }
        }

        public virtual void RemoveSelectionHandles(DiagramSelection sel)
        {
            sel.RemoveHandles(this);
        }

        internal void SetBeingRemoved(bool value)
        {
            if (value)
            {
                this.InternalFlags |= 0x10000;
            }
            else
            {
                this.InternalFlags &= -65537;
            }
        }

        internal void SetLayer(DiagramLayer value, DiagramShape mainObj, bool undoing)
        {
            GroupShape group1 = this as GroupShape;
            if (group1 != null)
            {
                DiagramShape[] objArray1 = group1.CopyArray();
                for (int num1 = 0; num1 < objArray1.Length; num1++)
                {
                    DiagramShape obj1 = objArray1[num1];
                    obj1.SetLayer(value, mainObj, undoing);
                }
            }
            DiagramLayer layer1 = this.myLayer;
            DiagramLayer layer2 = value;
            if (layer1 != layer2)
            {
                if (layer2 == null)
                {
                    if (!undoing)
                    {
                        this.OnLayerChanged(layer1, null, mainObj);
                    }
                    this.myLayer = null;
                }
                else
                {
                    this.myLayer = layer2;
                    if (!undoing)
                    {
                        this.OnLayerChanged(layer1, layer2, mainObj);
                    }
                }
            }
        }

        internal void SetParent(GroupShape value, bool undoing)
        {
            GroupShape group1 = this.myParent;
            GroupShape group2 = value;
            if (group1 != group2)
            {
                if (group2 == null)
                {
                    if (!undoing)
                    {
                        this.OnParentChanged(group1, null);
                    }
                    this.SetLayer(null, this, undoing);
                    this.myParent = null;
                }
                else
                {
                    this.myParent = group2;
                    this.SetLayer(group2.Layer, this, undoing);
                    if (!undoing)
                    {
                        this.OnParentChanged(group1, group2);
                    }
                }
            }
        }

        public virtual RectangleF SetRectangleSpotLocation(RectangleF Rect, int Spot, PointF Point)
        {
            switch (Spot)
            {
                case 1:
                    Rect.X = Point.X - (Rect.Width / 2f);
                    Rect.Y = Point.Y - (Rect.Height / 2f);
                    return Rect;
                case 2:
                    Rect.X = Point.X;
                    Rect.Y = Point.Y;
                    return Rect;
                case 4:
                    Rect.X = Point.X - Rect.Width;
                    Rect.Y = Point.Y;
                    return Rect;
                case 8:
                    Rect.X = Point.X - Rect.Width;
                    Rect.Y = Point.Y - Rect.Height;
                    return Rect;
                case 0x10:
                    Rect.X = Point.X;
                    Rect.Y = Point.Y - Rect.Height;
                    return Rect;
                case 0x20:
                    Rect.X = Point.X - (Rect.Width / 2f);
                    Rect.Y = Point.Y;
                    return Rect;
                case 0x40:
                    Rect.X = Point.X - Rect.Width;
                    Rect.Y = Point.Y - (Rect.Height / 2f);
                    return Rect;
                case 0x80:
                    Rect.X = Point.X - (Rect.Width / 2f);
                    Rect.Y = Point.Y - Rect.Height;
                    return Rect;
                case 0x100:
                    Rect.X = Point.X;
                    Rect.Y = Point.Y - (Rect.Height / 2f);
                    return Rect;
                default:
                    Rect.X = Point.X;
                    Rect.Y = Point.Y;
                    return Rect;
            }
        }

        public virtual void SetSizeKeepingLocation(SizeF s)
        {
            this.Size = s;
        }

        public virtual void SetSpotLocation(int spot, PointF newp)
        {
            RectangleF ef1 = this.Bounds;
            this.Bounds = this.SetRectangleSpotLocation(ef1, spot, newp);
        }

        public void SetSpotLocation(int spot, DiagramShape obj, int otherSpot)
        {
            PointF tf1 = obj.GetSpotLocation(otherSpot);
            this.SetSpotLocation(spot, tf1);
        }

        public virtual int SpotOpposite(int spot)
        {
            int num1 = spot;
            if (num1 <= 0x10)
            {
                switch (num1)
                {
                    case 1:
                        {
                            return 1;
                        }
                    case 2:
                        {
                            return 8;
                        }
                    case 3:
                        {
                            return spot;
                        }
                    case 4:
                        {
                            return 0x10;
                        }
                    case 8:
                        {
                            return 2;
                        }
                    case 0x10:
                        {
                            return 4;
                        }
                }
                return spot;
            }
            if (num1 <= 0x40)
            {
                if (num1 == 0x20)
                {
                    return 0x80;
                }
                if (num1 == 0x40)
                {
                    return 0x100;
                }
                return spot;
            }
            if (num1 == 0x80)
            {
                return 0x20;
            }
            if (num1 == 0x100)
            {
                return 0x40;
            }
            return spot;
        }

        internal static void Trace(string msg)
        {
            System.Diagnostics.Trace.WriteLine(msg);
        }

        internal static RectangleF UnionRect(RectangleF r, PointF p)
        {
            if (p.X < r.X)
            {
                r.Width = (r.X + r.Width) - p.X;
                r.X = p.X;
            }
            else if (p.X > (r.X + r.Width))
            {
                r.Width = p.X - r.X;
            }
            if (p.Y < r.Y)
            {
                r.Height = (r.Y + r.Height) - p.Y;
                r.Y = p.Y;
                return r;
            }
            if (p.Y > (r.Y + r.Height))
            {
                r.Height = p.Y - r.Y;
            }
            return r;
        }

        internal static RectangleF UnionRect(RectangleF a, RectangleF b)
        {
            float single1 = System.Math.Min(a.X, b.X);
            float single2 = System.Math.Min(a.Y, b.Y);
            float single3 = System.Math.Max((float)(a.X + a.Width), (float)(b.X + b.Width));
            float single4 = System.Math.Max((float)(a.Y + a.Height), (float)(b.Y + b.Height));
            return new RectangleF(single1, single2, single3 - single1, single4 - single2);
        }


        [DefaultValue(true), Category("Behavior"), Description("Whether this object automatically rescales its appearance when its size changes.")]
        public virtual bool AutoRescales
        {
            get
            {
                return ((this.InternalFlags & 0x100) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 0x100) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 0x100;
                    }
                    else
                    {
                        this.InternalFlags &= -257;
                    }
                    this.Changed(0x3f3, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Browsable(false)]
        public bool BeingRemoved
        {
            get
            {
                return ((this.InternalFlags & 0x10000) != 0);
            }
        }

        [Description("The coordinate of the bottom side of the Bounds."), Category("Bounds")]
        public float Bottom
        {
            get
            {
                RectangleF ef1 = this.Bounds;
                return (ef1.Y + ef1.Height);
            }
            set
            {
                RectangleF ef1 = this.Bounds;
                ef1.Y += (value - (ef1.Y + ef1.Height));
                this.Bounds = ef1;
            }
        }

        [Category("Bounds"), Browsable(false)]
        public virtual RectangleF Bounds
        {
            get
            {
                if (this.InvalidBounds && !this.SkipsBoundsChanged)
                {
                    this.InvalidBounds = false;
                    this.SkipsBoundsChanged = true;
                    this.Bounds = this.ComputeBounds();
                    this.SkipsBoundsChanged = false;
                }
                return this.myBounds;
            }
            set
            {
                RectangleF ef1 = this.myBounds;
                if (((value.Width >= 0f) && (value.Height >= 0f)) && (ef1 != value))
                {
                    this.myBounds = value;
                    this.Changed(0x3e9, 0, null, ef1, 0, null, value);
                    if (!this.SkipsBoundsChanged)
                    {
                        this.SkipsBoundsChanged = true;
                        this.OnBoundsChanged(ef1);
                        if (this.InvalidBounds)
                        {
                            this.InvalidBounds = false;
                            this.Bounds = this.ComputeBounds();
                        }
                    }
                    this.SkipsBoundsChanged = false;
                    GroupShape group1 = this.Parent;
                    if (group1 != null)
                    {
                        group1.InvalidatePaintBounds();
                        if (!group1.SkipsBoundsChanged)
                        {
                            group1.SkipsBoundsChanged = true;
                            group1.OnChildBoundsChanged(this, ef1);
                            if (group1.InvalidBounds)
                            {
                                group1.InvalidBounds = false;
                                group1.Bounds = group1.ComputeBounds();
                            }
                            group1.SkipsBoundsChanged = false;
                        }
                    }
                }
            }
        }

        [Browsable(false), TypeConverter(typeof(PointFConverter)), Category("Bounds")]
        public PointF Center
        {
            get
            {
                RectangleF ef1 = this.Bounds;
                return new PointF(ef1.X + (ef1.Width / 2f), ef1.Y + (ef1.Height / 2f));
            }
            set
            {
                RectangleF ef1 = this.Bounds;
                ef1.X = value.X - (ef1.Width / 2f);
                ef1.Y = value.Y - (ef1.Height / 2f);
                this.Bounds = ef1;
            }
        }

        [Description("Whether users can copy this object."), Category("Behavior"), DefaultValue(true)]
        public virtual bool Copyable
        {
            get
            {
                return ((this.InternalFlags & 8) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 8) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 8;
                    }
                    else
                    {
                        this.InternalFlags &= -9;
                    }
                    this.Changed(0x3ee, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether users can delete this object.")]
        public virtual bool Deletable
        {
            get
            {
                return ((this.InternalFlags & 0x40) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 0x40) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 0x40;
                    }
                    else
                    {
                        this.InternalFlags &= -65;
                    }
                    this.Changed(0x3f1, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The DiagramDocument to which this object belongs."), Category("Ownership")]
        public DiagramDocument Document
        {
            get
            {
                DiagramLayer layer1 = this.Layer;
                if (layer1 != null)
                {
                    return layer1.Document;
                }
                return null;
            }
        }

        [Category("Behavior"), Description("The object that will get dragged when this selected object is dragged.")]
        public virtual DiagramShape DraggingObject
        {
            get
            {
                if (this.DragsNode)
                {
                    for (DiagramShape obj1 = this.Parent; obj1 != null; obj1 = obj1.Parent)
                    {
                        if (obj1 is IDiagramNode)
                        {
                            return obj1;
                        }
                        if (obj1.Parent == null)
                        {
                            return obj1;
                        }
                    }
                }
                return this;
            }
        }

        [Description("Whether this selected child object, when dragged, drags the node instead."), DefaultValue(false), Category("Behavior")]
        public virtual bool DragsNode
        {
            get
            {
                return ((this.InternalFlags & 0x800) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 0x800) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 0x800;
                    }
                    else
                    {
                        this.InternalFlags &= -2049;
                    }
                    this.Changed(0x3f8, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Whether users can edit this object.")]
        public virtual bool Editable
        {
            get
            {
                return ((this.InternalFlags & 0x80) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 0x80) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 0x80;
                    }
                    else
                    {
                        this.InternalFlags &= -129;
                    }
                    this.Changed(1010, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Browsable(false)]
        public virtual DiagramControl Editor
        {
            get
            {
                return null;
            }
        }

        [Category("Bounds"), Description("The height of the Bounds.")]
        public float Height
        {
            get
            {
                return this.Bounds.Height;
            }
            set
            {
                RectangleF ef1 = this.Bounds;
                ef1.Height = value;
                this.Bounds = ef1;
            }
        }

        [Browsable(false)]
        public bool Initializing
        {
            get
            {
                return ((this.InternalFlags & 0x20000) != 0);
            }
            set
            {
                if (value)
                {
                    this.InternalFlags |= 0x20000;
                }
                else
                {
                    this.InternalFlags &= -131073;
                }
            }
        }

        internal int InternalFlags
        {
            get
            {
                return this.myInternalFlags;
            }
            set
            {
                this.myInternalFlags = value;
            }
        }

        [Browsable(false)]
        protected bool InvalidBounds
        {
            get
            {
                return ((this.InternalFlags & 0x8000) != 0);
            }
            set
            {
                if (value)
                {
                    this.InternalFlags |= 0x8000;
                }
                else
                {
                    this.InternalFlags &= -32769;
                }
            }
        }

        [Browsable(false)]
        public bool IsInDocument
        {
            get
            {
                DiagramLayer layer1 = this.Layer;
                if (layer1 != null)
                {
                    return layer1.IsInDocument;
                }
                return false;
            }
        }

        [Browsable(false)]
        public bool IsInView
        {
            get
            {
                DiagramLayer layer1 = this.Layer;
                if (layer1 != null)
                {
                    return layer1.IsInView;
                }
                return false;
            }
        }

        [Browsable(false)]
        public bool IsTopLevel
        {
            get
            {
                return (this.myParent == null);
            }
        }

        [Description("The GoLayer to which this object belongs."), Category("Ownership")]
        public DiagramLayer Layer
        {
            get
            {
                return this.myLayer;
            }
        }

        [Category("Bounds"), Description("The coordinate of the left side of the Bounds.")]
        public float Left
        {
            get
            {
                return this.Bounds.X;
            }
            set
            {
                RectangleF ef1 = this.Bounds;
                ef1.X = value;
                this.Bounds = ef1;
            }
        }

        [Category("Bounds"), Description("The natural location for this object, perhaps different from Position."), TypeConverter(typeof(PointFConverter))]
        public virtual PointF Location
        {
            get
            {
                return this.Position;
            }
            set
            {
                this.Position = value;
            }
        }

        [DefaultValue(true), Description("Whether users can move this object."), Category("Behavior")]
        public virtual bool Movable
        {
            get
            {
                return ((this.InternalFlags & 4) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 4) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 4;
                    }
                    else
                    {
                        this.InternalFlags &= -5;
                    }
                    this.Changed(0x3ed, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public CollectionEnumerator Observers
        {
            get
            {
                if (this.myObservers != null)
                {
                    return this.myObservers.GetEnumerator();
                }
                return CollectionEnumerator.Empty;
            }
        }

        [Description("The parent GoGroup for this object, or null if top-level."), Category("Ownership")]
        public GroupShape Parent
        {
            get
            {
                return this.myParent;
            }
        }

        [Browsable(false)]
        public DiagramShape ParentNode
        {
            get
            {
                DiagramShape obj1 = this;
                while ((obj1.Parent != null) && !(obj1.Parent is SubGraphNode))
                {
                    obj1 = obj1.Parent;
                }
                return obj1;
            }
        }

        [TypeConverter(typeof(PointFConverter)), Browsable(false), Category("Bounds")]
        public PointF Position
        {
            get
            {
                RectangleF ef1 = this.Bounds;
                return new PointF(ef1.X, ef1.Y);
            }
            set
            {
                RectangleF ef1 = this.Bounds;
                ef1.X = value.X;
                ef1.Y = value.Y;
                this.Bounds = ef1;
            }
        }

        [Description("Whether a view can print this object."), Category("Behavior"), DefaultValue(true)]
        public virtual bool Printable
        {
            get
            {
                return ((this.InternalFlags & 0x80000) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 0x80000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 0x80000;
                    }
                    else
                    {
                        this.InternalFlags &= -524289;
                    }
                    this.Changed(0x3f9, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether users can reshape this resizable object."), Category("Behavior"), DefaultValue(true)]
        public virtual bool Reshapable
        {
            get
            {
                return ((this.InternalFlags & 0x20) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 0x20) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 0x20;
                    }
                    else
                    {
                        this.InternalFlags &= -33;
                    }
                    this.Changed(0x3f0, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("Whether users can resize this object."), DefaultValue(true)]
        public virtual bool Resizable
        {
            get
            {
                return ((this.InternalFlags & 0x10) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 0x10) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 0x10;
                    }
                    else
                    {
                        this.InternalFlags &= -17;
                    }
                    this.Changed(0x3ef, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [DefaultValue(false), Category("Behavior"), Description("Whether this object's size continuously changes during a user resizing operation.")]
        public virtual bool ResizesRealtime
        {
            get
            {
                return ((this.InternalFlags & 0x200) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 0x200) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 0x200;
                    }
                    else
                    {
                        this.InternalFlags &= -513;
                    }
                    this.Changed(0x3f4, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Bounds"), Description("The coordinate of the right side of the Bounds.")]
        public float Right
        {
            get
            {
                RectangleF ef1 = this.Bounds;
                return (ef1.X + ef1.Width);
            }
            set
            {
                RectangleF ef1 = this.Bounds;
                ef1.X += (value - (ef1.X + ef1.Width));
                this.Bounds = ef1;
            }
        }

        [Description("Whether users can select this object."), Category("Behavior"), DefaultValue(true)]
        public virtual bool Selectable
        {
            get
            {
                return ((this.InternalFlags & 2) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 2) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 2;
                    }
                    else
                    {
                        this.InternalFlags &= -3;
                    }
                    this.Changed(0x3ec, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Appearance"), Description("The object that will get the selection handles when this object is selected.")]
        public virtual DiagramShape SelectionObject
        {
            get
            {
                return this;
            }
        }

        [Category("Appearance"), DefaultValue(false), Description("Whether this object is painted with a drop shadow.")]
        public virtual bool Shadowed
        {
            get
            {
                return ((this.InternalFlags & 0x400) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 0x400) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 0x400;
                    }
                    else
                    {
                        this.InternalFlags &= -1025;
                    }
                    this.Changed(0x3f5, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [TypeConverter(typeof(SizeFConverter)), Browsable(false), Category("Bounds")]
        public SizeF Size
        {
            get
            {
                RectangleF ef1 = this.Bounds;
                return new SizeF(ef1.Width, ef1.Height);
            }
            set
            {
                RectangleF ef1 = this.Bounds;
                ef1.Width = value.Width;
                ef1.Height = value.Height;
                this.Bounds = ef1;
            }
        }

        private bool SkipsBoundsChanged
        {
            get
            {
                return ((this.InternalFlags & 0x4000) != 0);
            }
            set
            {
                if (value)
                {
                    this.InternalFlags |= 0x4000;
                }
                else
                {
                    this.InternalFlags &= -16385;
                }
            }
        }

        [Browsable(false)]
        public bool SkipsUndoManager
        {
            get
            {
                return ((this.InternalFlags & 0x2000) != 0);
            }
            set
            {
                if (value)
                {
                    this.InternalFlags |= 0x2000;
                }
                else
                {
                    this.InternalFlags &= -8193;
                }
            }
        }

        [Browsable(false)]
        public bool SuspendsUpdates
        {
            get
            {
                return ((this.InternalFlags & 0x1000) != 0);
            }
            set
            {
                if (value)
                {
                    this.InternalFlags |= 0x1000;
                }
                else
                {
                    this.InternalFlags &= -4097;
                }
            }
        }

        [Description("The coordinate of the top side of the Bounds."), Category("Bounds")]
        public float Top
        {
            get
            {
                return this.Bounds.Y;
            }
            set
            {
                RectangleF ef1 = this.Bounds;
                ef1.Y = value;
                this.Bounds = ef1;
            }
        }

        [Browsable(false)]
        public DiagramShape TopLevelObject
        {
            get
            {
                DiagramShape obj1 = this;
                while (obj1.Parent != null)
                {
                    obj1 = obj1.Parent;
                }
                return obj1;
            }
        }

        [Category("Ownership"), Description("The DiagramView to which this object belongs.")]
        public DiagramView View
        {
            get
            {
                DiagramLayer layer1 = this.Layer;
                if (layer1 != null)
                {
                    return layer1.View;
                }
                return null;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether users can see this object.")]
        public virtual bool Visible
        {
            get
            {
                return ((this.InternalFlags & 1) != 0);
            }
            set
            {
                bool flag1 = (this.InternalFlags & 1) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        this.InternalFlags |= 1;
                    }
                    else
                    {
                        this.InternalFlags &= -2;
                    }
                    this.Changed(0x3eb, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("The width of the Bounds."), Category("Bounds")]
        public float Width
        {
            get
            {
                return this.Bounds.Width;
            }
            set
            {
                RectangleF ef1 = this.Bounds;
                ef1.Width = value;
                this.Bounds = ef1;
            }
        }


        public const int BottomCenter = 0x80;
        public const int BottomLeft = 0x10;
        public const int BottomRight = 8;
        public const int ChangedAddedObserver = 0x3f6;
        public const int ChangedAutoRescales = 0x3f3;
        public const int ChangedBounds = 0x3e9;
        public const int ChangedCopyable = 0x3ee;
        public const int ChangedDeletable = 0x3f1;
        public const int ChangedDragsNode = 0x3f8;
        public const int ChangedEditable = 1010;
        public const int ChangedMovable = 0x3ed;
        public const int ChangedPrintable = 0x3f9;
        public const int ChangedRemovedObserver = 0x3f7;
        public const int ChangedReshapable = 0x3f0;
        public const int ChangedResizable = 0x3ef;
        public const int ChangedResizeRealtime = 0x3f4;
        public const int ChangedSelectable = 0x3ec;
        public const int ChangedShadowed = 0x3f5;
        public const int ChangedVisible = 0x3eb;
        internal const int flagAutoRescales = 0x100;
        internal const int flagBeingRemoved = 0x10000;
        internal const int flagCopyable = 8;
        internal const int flagDeletable = 0x40;
        internal const int flagDragsNode = 0x800;
        internal const int flagEditable = 0x80;
        internal const int flagInitializing = 0x20000;
        internal const int flagInvalidBounds = 0x8000;
        internal const int flagMovable = 4;
        internal const int flagObject1 = 0x100000;
        internal const int flagObject10 = 0x20000000;
        internal const int flagObject11 = 0x40000000;
        internal const int flagObject2 = 0x200000;
        internal const int flagObject3 = 0x400000;
        internal const int flagObject4 = 0x800000;
        internal const int flagObject5 = 0x1000000;
        internal const int flagObject6 = 0x2000000;
        internal const int flagObject7 = 0x4000000;
        internal const int flagObject8 = 0x8000000;
        internal const int flagObject9 = 0x10000000;
        internal const int flagPrintable = 0x80000;
        internal const int flagReserved1 = 0x40000;
        internal const int flagReshapable = 0x20;
        internal const int flagResizable = 0x10;
        internal const int flagResizesRealtime = 0x200;
        internal const int flagSelectable = 2;
        internal const int flagShadowed = 0x400;
        internal const int flagSkipsBoundsChanged = 0x4000;
        internal const int flagSkipsUndoManager = 0x2000;
        internal const int flagSuspendsUpdates = 0x1000;
        internal const int flagVisible = 1;
        public const int LastChangedHint = 10000;
        public const int LastHandle = 0x2000;
        public const int LastSpot = 0x2000;
        public const int Middle = 1;
        public const int MiddleBottom = 0x80;
        public const int MiddleCenter = 1;
        public const int MiddleLeft = 0x100;
        public const int MiddleRight = 0x40;
        public const int MiddleTop = 0x20;
        private RectangleF myBounds;
        private int myInternalFlags;
        private DiagramLayer myLayer;
        private DiagramShapeCollection myObservers;
        private GroupShape myParent;
        public const int NoHandle = 0;
        public const int NoSpot = 0;
        public static readonly RectangleF NullRect;
        public const int RepaintAll = 1000;
        public const int TopCenter = 0x20;
        public const int TopLeft = 2;
        public const int TopRight = 4;
    }
}
