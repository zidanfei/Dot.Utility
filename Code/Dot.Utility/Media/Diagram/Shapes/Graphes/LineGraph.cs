using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Dot.Utility.Media.Diagram.Shapes
{
    [Serializable]
    public class LineGraph : StrokeGraph, IDiagramLine, IGraphPart, IIdentifiablePart
    {
        public LineGraph()
        {
            this.myFromPort = null;
            this.myToPort = null;
            this.myAbstractLink = null;
            this.myUserFlags = 0;
            this.myUserObject = null;
            this.myPartID = -1;
            this.myAdjustingStyle = Shapes.LineAdjustingStyle.Calculate;
            this.myAbstractLink = this;
            base.InternalFlags &= -5;
            base.InternalFlags |= 0x8000000;
        }

        protected virtual void AddOrthoPoints(PointF startFrom, float fromDir, PointF endTo, float toDir)
        {
            RectangleF ef1;
            RectangleF ef2;
            PointF tf5;
            PointF tf6;
            if ((-45f <= fromDir) && (fromDir < 45f))
            {
                fromDir = 0f;
            }
            else if ((45f <= fromDir) && (fromDir < 135f))
            {
                fromDir = 90f;
            }
            else if ((135f <= fromDir) && (fromDir < 225f))
            {
                fromDir = 180f;
            }
            else
            {
                fromDir = 270f;
            }
            if ((-45f <= toDir) && (toDir < 45f))
            {
                toDir = 0f;
            }
            else if ((45f <= toDir) && (toDir < 135f))
            {
                toDir = 90f;
            }
            else if ((135f <= toDir) && (toDir < 225f))
            {
                toDir = 180f;
            }
            else
            {
                toDir = 270f;
            }
            PointF tf1 = startFrom;
            PointF tf2 = endTo;
            float single1 = base.InternalPenWidth + 1f;
            DiagramShape obj1 = this.FromPort.DiagramShape;
            IDiagramNode node1 = this.FromPort.Node;
            if ((node1 != null) && (node1.DiagramShape != null))
            {
                ef1 = node1.DiagramShape.Bounds;
            }
            else if (obj1.Parent != null)
            {
                ef1 = obj1.Parent.Bounds;
            }
            else
            {
                ef1 = obj1.Bounds;
            }
            DiagramShape.InflateRect(ref ef1, single1, single1);
            DiagramShape obj2 = this.ToPort.DiagramShape;
            IDiagramNode node2 = this.ToPort.Node;
            if ((node2 != null) && (node2.DiagramShape != null))
            {
                ef2 = node2.DiagramShape.Bounds;
            }
            else if (obj2.Parent != null)
            {
                ef2 = obj2.Parent.Bounds;
            }
            else
            {
                ef2 = obj2.Bounds;
            }
            DiagramShape.InflateRect(ref ef2, single1, single1);
            if (this.AvoidsNodes && (base.Document != null))
            {
                PositionArray array1 = base.Document.GetPositions();
                RectangleF ef3 = DiagramShape.UnionRect(ef1, ef2);
                DiagramShape.InflateRect(ref ef3, array1.CellSize.Width * 2f, array1.CellSize.Height * 2f);
                array1.Propagate(startFrom, fromDir, endTo, toDir, ef3);
                int num1 = array1.GetDist(endTo.X, endTo.Y);
                if (num1 >= 0x7fffffff)
                {
                    array1.SetAllUnoccupied(0x7fffffff);
                    array1.Propagate(startFrom, fromDir, endTo, toDir, array1.Bounds);
                    num1 = array1.GetDist(endTo.X, endTo.Y);
                }
                if ((num1 < 0x7fffffff) && !array1.IsOccupied(endTo.X, endTo.Y))
                {
                    this.TraversePositions(array1, endTo.X, endTo.Y, toDir, true);
                    PointF tf3 = this.GetPoint(2);
                    if (this.PointsCount < 4)
                    {
                        if ((fromDir == 0f) || (fromDir == 180f))
                        {
                            tf3.X = startFrom.X;
                            tf3.Y = endTo.Y;
                        }
                        else
                        {
                            tf3.X = endTo.X;
                            tf3.Y = startFrom.Y;
                        }
                        this.SetPoint(2, tf3);
                        this.InsertPoint(3, tf3);
                        return;
                    }
                    PointF tf4 = this.GetPoint(3);
                    if ((fromDir == 0f) || (fromDir == 180f))
                    {
                        if (base.IsApprox(tf3.X, tf4.X))
                        {
                            float single2 = (fromDir == 0f) ? System.Math.Max(tf3.X, startFrom.X) : System.Math.Min(tf3.X, startFrom.X);
                            this.SetPoint(2, new PointF(single2, startFrom.Y));
                            this.SetPoint(3, new PointF(single2, tf4.Y));
                            return;
                        }
                        if (base.IsApprox(tf3.Y, tf4.Y))
                        {
                            float single3 = startFrom.Y;
                            this.SetPoint(2, new PointF(tf3.X, single3));
                            this.SetPoint(3, new PointF(tf4.X, single3));
                            return;
                        }
                        this.SetPoint(2, new PointF(startFrom.X, tf3.Y));
                        return;
                    }
                    if ((fromDir == 90f) || (fromDir == 270f))
                    {
                        if (base.IsApprox(tf3.Y, tf4.Y))
                        {
                            float single4 = (fromDir == 90f) ? System.Math.Max(tf3.Y, startFrom.Y) : System.Math.Min(tf3.Y, startFrom.Y);
                            this.SetPoint(2, new PointF(startFrom.X, single4));
                            this.SetPoint(3, new PointF(tf4.X, single4));
                            return;
                        }
                        if (base.IsApprox(tf3.X, tf4.X))
                        {
                            float single5 = startFrom.X;
                            this.SetPoint(2, new PointF(single5, tf3.Y));
                            this.SetPoint(3, new PointF(single5, tf4.Y));
                            return;
                        }
                        this.SetPoint(2, new PointF(tf3.X, startFrom.Y));
                    }
                    return;
                }
            }
            if (fromDir == 0f)
            {
                if (((tf2.X > tf1.X) || (((toDir == 270f) && (tf2.Y < tf1.Y)) && (ef2.Right > tf1.X))) || (((toDir == 90f) && (tf2.Y > tf1.Y)) && (ef2.Right > tf1.X)))
                {
                    tf5 = new PointF(tf2.X, tf1.Y);
                    tf6 = new PointF(tf2.X, (tf1.Y + tf2.Y) / 2f);
                    if (toDir == 180f)
                    {
                        tf5.X = this.GetMidOrthoPosition(tf1.X, tf2.X, false);
                        tf6.X = tf5.X;
                        tf6.Y = tf2.Y;
                    }
                    else if (((toDir == 270f) && (tf2.Y < tf1.Y)) || ((toDir == 90f) && (tf2.Y >= tf1.Y)))
                    {
                        if (tf1.X < ef2.Left)
                        {
                            tf5.X = this.GetMidOrthoPosition(tf1.X, ef2.Left, false);
                        }
                        else if ((tf1.X < tf2.X) && (tf1.Y < ef2.Bottom))
                        {
                            tf5.X = this.GetMidOrthoPosition(tf1.X, tf2.X, false);
                        }
                        else
                        {
                            tf5.X = ef2.Right;
                        }
                        tf6.X = tf5.X;
                        tf6.Y = tf2.Y;
                    }
                    else if (((toDir == 0f) && (tf5.Y > ef2.Top)) && (tf5.Y < ef2.Bottom))
                    {
                        tf5.X = tf1.X;
                        if (tf1.Y < tf2.Y)
                        {
                            tf5.Y = System.Math.Min(tf2.Y, ef2.Top);
                        }
                        else
                        {
                            tf5.Y = System.Math.Max(tf2.Y, ef2.Bottom);
                        }
                        tf6.Y = tf5.Y;
                    }
                }
                else
                {
                    tf5 = new PointF(tf1.X, tf2.Y);
                    tf6 = new PointF((tf1.X + tf2.X) / 2f, tf2.Y);
                    if (((toDir == 180f) || ((toDir == 90f) && (tf2.Y < ef1.Top))) || ((toDir == 270f) && (tf2.Y > ef1.Bottom)))
                    {
                        if ((tf2.Y < tf1.Y) && ((toDir == 180f) || (toDir == 90f)))
                        {
                            tf5.Y = this.GetMidOrthoPosition(ef1.Top, System.Math.Max(tf2.Y, ef2.Bottom), true);
                        }
                        else if ((tf2.Y >= tf1.Y) && ((toDir == 180f) || (toDir == 270f)))
                        {
                            tf5.Y = this.GetMidOrthoPosition(ef1.Bottom, System.Math.Min(tf2.Y, ef2.Top), true);
                        }
                        tf6.X = tf2.X;
                        tf6.Y = tf5.Y;
                    }
                    if ((tf5.Y > ef1.Top) && (tf5.Y < ef1.Bottom))
                    {
                        if (((tf2.X >= ef1.Left) && (tf2.X <= tf1.X)) || ((tf1.X <= ef2.Right) && (tf1.X >= tf2.X)))
                        {
                            if ((toDir == 0f) || (toDir == 180f))
                            {
                                tf5 = new PointF(tf1.X, (tf1.Y + tf2.Y) / 2f);
                                tf6 = new PointF(tf2.X, tf5.Y);
                            }
                            else
                            {
                                tf5 = new PointF(System.Math.Max((float)((tf1.X + tf2.X) / 2f), tf1.X), tf1.Y);
                                tf6 = new PointF(tf5.X, tf2.Y);
                            }
                        }
                        else
                        {
                            tf6.X = tf2.X;
                            if ((toDir == 270f) || (((toDir == 0f) || (toDir == 180f)) && (tf2.Y < tf1.Y)))
                            {
                                tf5.Y = System.Math.Min(tf2.Y, System.Math.Min(ef1.Top, ef2.Top));
                            }
                            else
                            {
                                tf5.Y = System.Math.Max(tf2.Y, System.Math.Max(ef1.Bottom, ef2.Bottom));
                            }
                            tf6.Y = tf5.Y;
                        }
                    }
                }
            }
            else if (fromDir == 180f)
            {
                if (((tf2.X <= tf1.X) || (((toDir == 270f) && (tf2.Y < tf1.Y)) && (ef2.Left < tf1.X))) || (((toDir == 90f) && (tf2.Y > tf1.Y)) && (ef2.Left < tf1.X)))
                {
                    tf5 = new PointF(tf2.X, tf1.Y);
                    tf6 = new PointF(tf2.X, (tf1.Y + tf2.Y) / 2f);
                    if (toDir == 0f)
                    {
                        tf5.X = this.GetMidOrthoPosition(tf1.X, tf2.X, false);
                        tf6.X = tf5.X;
                        tf6.Y = tf2.Y;
                    }
                    else if (((toDir == 270f) && (tf2.Y < tf1.Y)) || ((toDir == 90f) && (tf2.Y >= tf1.Y)))
                    {
                        if (tf1.X > ef2.Right)
                        {
                            tf5.X = this.GetMidOrthoPosition(tf1.X, ef2.Right, false);
                        }
                        else if ((tf1.X > tf2.X) && (tf1.Y < ef2.Bottom))
                        {
                            tf5.X = this.GetMidOrthoPosition(tf1.X, tf2.X, false);
                        }
                        else
                        {
                            tf5.X = ef2.Left;
                        }
                        tf6.X = tf5.X;
                        tf6.Y = tf2.Y;
                    }
                    else if (((toDir == 180f) && (tf5.Y > ef2.Top)) && (tf5.Y < ef2.Bottom))
                    {
                        tf5.X = tf1.X;
                        if (tf1.Y < tf2.Y)
                        {
                            tf5.Y = System.Math.Min(tf2.Y, ef2.Top);
                        }
                        else
                        {
                            tf5.Y = System.Math.Max(tf2.Y, ef2.Bottom);
                        }
                        tf6.Y = tf5.Y;
                    }
                }
                else
                {
                    tf5 = new PointF(tf1.X, tf2.Y);
                    tf6 = new PointF((tf1.X + tf2.X) / 2f, tf2.Y);
                    if (((toDir == 0f) || ((toDir == 90f) && (tf2.Y < ef1.Top))) || ((toDir == 270f) && (tf2.Y > ef1.Bottom)))
                    {
                        if ((tf2.Y < tf1.Y) && ((toDir == 0f) || (toDir == 90f)))
                        {
                            tf5.Y = this.GetMidOrthoPosition(ef1.Top, System.Math.Max(tf2.Y, ef2.Bottom), true);
                        }
                        else if ((tf2.Y >= tf1.Y) && ((toDir == 0f) || (toDir == 270f)))
                        {
                            tf5.Y = this.GetMidOrthoPosition(ef1.Bottom, System.Math.Min(tf2.Y, ef2.Top), true);
                        }
                        tf6.X = tf2.X;
                        tf6.Y = tf5.Y;
                    }
                    if ((tf5.Y > ef1.Top) && (tf5.Y < ef1.Bottom))
                    {
                        if (((tf2.X >= ef1.Left) && (tf2.X <= tf1.X)) || ((tf1.X <= ef2.Right) && (tf1.X >= tf2.X)))
                        {
                            if ((toDir == 0f) || (toDir == 180f))
                            {
                                tf5 = new PointF(tf1.X, (tf1.Y + tf2.Y) / 2f);
                                tf6 = new PointF(tf2.X, tf5.Y);
                            }
                            else
                            {
                                tf5 = new PointF(System.Math.Min((float)((tf1.X + tf2.X) / 2f), tf1.X), tf1.Y);
                                tf6 = new PointF(tf5.X, tf2.Y);
                            }
                        }
                        else
                        {
                            tf6.X = tf2.X;
                            if ((toDir == 270f) || (((toDir == 0f) || (toDir == 180f)) && (tf2.Y < tf1.Y)))
                            {
                                tf5.Y = System.Math.Min(tf2.Y, System.Math.Min(ef1.Top, ef2.Top));
                            }
                            else
                            {
                                tf5.Y = System.Math.Max(tf2.Y, System.Math.Max(ef1.Bottom, ef2.Bottom));
                            }
                            tf6.Y = tf5.Y;
                        }
                    }
                }
            }
            else if (fromDir == 90f)
            {
                if (((tf2.Y > tf1.Y) || (((toDir == 180f) && (tf2.X < tf1.X)) && (ef2.Bottom > tf1.Y))) || (((toDir == 0f) && (tf2.X > tf1.X)) && (ef2.Bottom > tf1.Y)))
                {
                    tf5 = new PointF(tf1.X, tf2.Y);
                    tf6 = new PointF((tf1.X + tf2.X) / 2f, tf2.Y);
                    if (toDir == 270f)
                    {
                        tf5.Y = this.GetMidOrthoPosition(tf1.Y, tf2.Y, true);
                        tf6.X = tf2.X;
                        tf6.Y = tf5.Y;
                    }
                    else if (((toDir == 180f) && (tf2.X < tf1.X)) || ((toDir == 0f) && (tf2.X >= tf1.X)))
                    {
                        if (tf1.Y < ef2.Top)
                        {
                            tf5.Y = this.GetMidOrthoPosition(tf1.Y, ef2.Top, true);
                        }
                        else if ((tf1.Y < tf2.Y) && (tf1.X < ef2.Right))
                        {
                            tf5.Y = this.GetMidOrthoPosition(tf1.Y, tf2.Y, true);
                        }
                        else
                        {
                            tf5.Y = ef2.Bottom;
                        }
                        tf6.X = tf2.X;
                        tf6.Y = tf5.Y;
                    }
                    else if (((toDir == 90f) && (tf5.X > ef2.Left)) && (tf5.X < ef2.Right))
                    {
                        if (tf1.X < tf2.X)
                        {
                            tf5.X = System.Math.Min(tf2.X, ef2.Left);
                        }
                        else
                        {
                            tf5.X = System.Math.Max(tf2.X, ef2.Right);
                        }
                        tf5.Y = tf1.Y;
                        tf6.X = tf5.X;
                    }
                }
                else
                {
                    tf5 = new PointF(tf2.X, tf1.Y);
                    tf6 = new PointF(tf2.X, (tf1.Y + tf2.Y) / 2f);
                    if (((toDir == 270f) || ((toDir == 0f) && (tf2.X < ef1.Left))) || ((toDir == 180f) && (tf2.X > ef1.Right)))
                    {
                        if ((tf2.X < tf1.X) && ((toDir == 270f) || (toDir == 0f)))
                        {
                            tf5.X = this.GetMidOrthoPosition(ef1.Left, System.Math.Max(tf2.X, ef2.Right), false);
                        }
                        else if ((tf2.X >= tf1.X) && ((toDir == 270f) || (toDir == 180f)))
                        {
                            tf5.X = this.GetMidOrthoPosition(ef1.Right, System.Math.Min(tf2.X, ef2.Left), false);
                        }
                        tf6.X = tf5.X;
                        tf6.Y = tf2.Y;
                    }
                    if ((tf5.X > ef1.Left) && (tf5.X < ef1.Right))
                    {
                        if (((tf2.Y >= ef1.Top) && (tf2.Y <= tf1.Y)) || ((tf1.Y <= ef2.Bottom) && (tf1.Y >= tf2.Y)))
                        {
                            if ((toDir == 0f) || (toDir == 180f))
                            {
                                tf5 = new PointF(tf1.X, System.Math.Max((float)((tf1.Y + tf2.Y) / 2f), tf1.Y));
                                tf6 = new PointF(tf2.X, tf5.Y);
                            }
                            else
                            {
                                tf5 = new PointF((tf1.X + tf2.X) / 2f, tf1.Y);
                                tf6 = new PointF(tf5.X, tf2.Y);
                            }
                        }
                        else
                        {
                            if ((toDir == 180f) || (((toDir == 90f) || (toDir == 270f)) && (tf2.X < tf1.X)))
                            {
                                tf5.X = System.Math.Min(tf2.X, System.Math.Min(ef1.Left, ef2.Left));
                            }
                            else
                            {
                                tf5.X = System.Math.Max(tf2.X, System.Math.Max(ef1.Right, ef2.Right));
                            }
                            tf6.X = tf5.X;
                            tf6.Y = tf2.Y;
                        }
                    }
                }
            }
            else if (((tf2.Y <= tf1.Y) || (((toDir == 180f) && (tf2.X < tf1.X)) && (ef2.Top < tf1.Y))) || (((toDir == 0f) && (tf2.X > tf1.X)) && (ef2.Top < tf1.Y)))
            {
                tf5 = new PointF(tf1.X, tf2.Y);
                tf6 = new PointF((tf1.X + tf2.X) / 2f, tf2.Y);
                if (toDir == 90f)
                {
                    tf5.Y = this.GetMidOrthoPosition(tf1.Y, tf2.Y, true);
                    tf6.X = tf2.X;
                    tf6.Y = tf5.Y;
                }
                else if (((toDir == 180f) && (tf2.X < tf1.X)) || ((toDir == 0f) && (tf2.X >= tf1.X)))
                {
                    if (tf1.Y > ef2.Bottom)
                    {
                        tf5.Y = this.GetMidOrthoPosition(tf1.Y, ef2.Bottom, true);
                    }
                    else if ((tf1.Y > tf2.Y) && (tf1.X < ef2.Right))
                    {
                        tf5.Y = this.GetMidOrthoPosition(tf1.Y, tf2.Y, true);
                    }
                    else
                    {
                        tf5.Y = ef2.Top;
                    }
                    tf6.X = tf2.X;
                    tf6.Y = tf5.Y;
                }
                else if (((toDir == 270f) && (tf5.X > ef2.Left)) && (tf5.X < ef2.Right))
                {
                    if (tf1.X < tf2.X)
                    {
                        tf5.X = System.Math.Min(tf2.X, ef2.Left);
                    }
                    else
                    {
                        tf5.X = System.Math.Max(tf2.X, ef2.Right);
                    }
                    tf5.Y = tf1.Y;
                    tf6.X = tf5.X;
                }
            }
            else
            {
                tf5 = new PointF(tf2.X, tf1.Y);
                tf6 = new PointF(tf2.X, (tf1.Y + tf2.Y) / 2f);
                if (((toDir == 90f) || ((toDir == 0f) && (tf2.X < ef1.Left))) || ((toDir == 180f) && (tf2.X > ef1.Right)))
                {
                    if ((tf2.X < tf1.X) && ((toDir == 90f) || (toDir == 0f)))
                    {
                        tf5.X = this.GetMidOrthoPosition(ef1.Left, System.Math.Max(tf2.X, ef2.Right), false);
                    }
                    else if ((tf2.X >= tf1.X) && ((toDir == 90f) || (toDir == 180f)))
                    {
                        tf5.X = this.GetMidOrthoPosition(ef1.Right, System.Math.Min(tf2.X, ef2.Left), false);
                    }
                    tf6.X = tf5.X;
                    tf6.Y = tf2.Y;
                }
                if ((tf5.X > ef1.Left) && (tf5.X < ef1.Right))
                {
                    if (((tf2.Y >= ef1.Top) && (tf2.Y <= tf1.Y)) || ((tf1.Y <= ef2.Bottom) && (tf1.Y >= tf2.Y)))
                    {
                        if ((toDir == 0f) || (toDir == 180f))
                        {
                            tf5 = new PointF(tf1.X, System.Math.Min((float)((tf1.Y + tf2.Y) / 2f), tf1.Y));
                            tf6 = new PointF(tf2.X, tf5.Y);
                        }
                        else
                        {
                            tf5 = new PointF((tf1.X + tf2.X) / 2f, tf1.Y);
                            tf6 = new PointF(tf5.X, tf2.Y);
                        }
                    }
                    else
                    {
                        if ((toDir == 180f) || (((toDir == 90f) || (toDir == 270f)) && (tf2.X < tf1.X)))
                        {
                            tf5.X = System.Math.Min(tf2.X, System.Math.Min(ef1.Left, ef2.Left));
                        }
                        else
                        {
                            tf5.X = System.Math.Max(tf2.X, System.Math.Max(ef1.Right, ef2.Right));
                        }
                        tf6.X = tf5.X;
                        tf6.Y = tf2.Y;
                    }
                }
            }
            this.AddPoint(tf5);
            this.AddPoint(tf6);
        }

        public override void AddSelectionHandles(DiagramSelection sel, DiagramShape selectedObj)
        {
            if (this.HighlightWhenSelected || !this.CanResize())
            {
                base.AddSelectionHandles(sel, selectedObj);
            }
            else
            {
                sel.RemoveHandles(this);
                if (this.PointsCount != 0)
                {
                    int num3;
                    int num1 = this.FirstPickIndex;
                    int num2 = this.LastPickIndex;
                    bool flag1 = this.CanReshape();
                    bool flag2 = this.Relinkable;
                    PointF tf1 = this.GetPoint(num1);
                    if (flag2)
                    {
                        num3 = 0x400;
                    }
                    else
                    {
                        num3 = 0;
                    }
                    IShapeHandle handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, num3, num3 != 0);
                    if (num3 == 0x400)
                    {
                        base.MakeDiamondResizeHandle(handle1, 0);
                    }
                    tf1 = this.GetPoint(num2);
                    if (flag2)
                    {
                        num3 = 0x401;
                    }
                    else
                    {
                        num3 = 0;
                    }
                    handle1 = sel.CreateResizeHandle(this, selectedObj, tf1, num3, num3 != 0);
                    if (num3 == 0x401)
                    {
                        base.MakeDiamondResizeHandle(handle1, 0);
                    }
                    for (int num4 = num1 + 1; num4 <= (num2 - 1); num4++)
                    {
                        tf1 = this.GetPoint(num4);
                        num3 = 0x2000 + num4;
                        if (!flag1)
                        {
                            num3 = 0;
                        }
                        else if (this.Orthogonal)
                        {
                            if (this.PointsCount < 6)
                            {
                                num3 = 0;
                            }
                            else if ((num4 == (num1 + 1)) && (this.FromPort != null))
                            {
                                PointF tf2 = this.GetPoint(num1);
                                if (base.IsApprox(tf2.Y, tf1.Y) && !base.IsApprox(tf2.X, tf1.X))
                                {
                                    num3 = 0x100;
                                }
                                else if (base.IsApprox(tf2.X, tf1.X) && !base.IsApprox(tf2.Y, tf1.Y))
                                {
                                    num3 = 0x20;
                                }
                                else if ((base.IsApprox(tf2.X, tf1.X) && base.IsApprox(tf2.Y, tf1.Y)) && ((num1 + 2) <= num2))
                                {
                                    PointF tf3 = this.GetPoint(num1 + 2);
                                    if (base.IsApprox(tf3.Y, tf1.Y) && !base.IsApprox(tf3.X, tf1.X))
                                    {
                                        num3 = 0x20;
                                    }
                                    else if (base.IsApprox(tf3.X, tf1.X) && !base.IsApprox(tf3.Y, tf1.Y))
                                    {
                                        num3 = 0x100;
                                    }
                                }
                            }
                            else if ((num4 == (num2 - 1)) && (this.ToPort != null))
                            {
                                PointF tf4 = this.GetPoint(num2);
                                if (base.IsApprox(tf1.Y, tf4.Y) && !base.IsApprox(tf1.X, tf4.X))
                                {
                                    num3 = 0x40;
                                }
                                else if (base.IsApprox(tf1.X, tf4.X) && !base.IsApprox(tf1.Y, tf4.Y))
                                {
                                    num3 = 0x80;
                                }
                                else if ((base.IsApprox(tf4.X, tf1.X) && base.IsApprox(tf4.Y, tf1.Y)) && ((num2 - 2) >= num1))
                                {
                                    PointF tf5 = this.GetPoint(num2 - 2);
                                    if (base.IsApprox(tf5.Y, tf1.Y) && !base.IsApprox(tf5.X, tf1.X))
                                    {
                                        num3 = 0x80;
                                    }
                                    else if (base.IsApprox(tf5.X, tf1.X) && !base.IsApprox(tf5.Y, tf1.Y))
                                    {
                                        num3 = 0x40;
                                    }
                                }
                            }
                        }
                        sel.CreateResizeHandle(this, selectedObj, tf1, num3, num3 != 0);
                    }
                }
            }
        }

        protected virtual bool AdjustPoints(int startIndex, PointF newFromPoint, int endIndex, PointF newToPoint)
        {
            LineAdjustingStyle style1 = this.AdjustingStyle;
            if (this.Orthogonal)
            {
                if (style1 == LineAdjustingStyle.Scale)
                {
                    return false;
                }
                if (style1 == LineAdjustingStyle.Stretch)
                {
                    style1 = LineAdjustingStyle.End;
                }
            }
            switch (style1)
            {
                case LineAdjustingStyle.Scale:
                    {
                        return this.RescalePoints(startIndex, newFromPoint, endIndex, newToPoint);
                    }
                case LineAdjustingStyle.Stretch:
                    {
                        return this.StretchPoints(startIndex, newFromPoint, endIndex, newToPoint);
                    }
                case LineAdjustingStyle.End:
                    {
                        return this.ModifyEndPoints(startIndex, newFromPoint, endIndex, newToPoint);
                    }
            }
            return false;
        }

        private void CalculateBezierNoSpot(DiagramShape fromObj, DiagramPort from, DiagramShape toObj, DiagramPort to)
        {
            this.ClearPoints();
            PointF tf1 = fromObj.Center;
            PointF tf2 = toObj.Center;
            if (from == null)
            {
                if (!fromObj.GetNearestIntersectionPoint(tf2, tf1, out tf1))
                {
                    tf1 = fromObj.Center;
                }
            }
            else
            {
                tf1 = from.GetFromLinkPoint(this.AbstractLink);
            }
            if (to == null)
            {
                if (!toObj.GetNearestIntersectionPoint(tf1, tf2, out tf2))
                {
                    tf2 = toObj.Center;
                }
            }
            else
            {
                tf2 = to.GetToLinkPoint(this.AbstractLink);
            }
            float single1 = tf2.X - tf1.X;
            float single2 = tf2.Y - tf1.Y;
            System.Math.Sqrt((double)((single1 * single1) + (single2 * single2)));
            float single3 = this.Curviness;
            float single4 = System.Math.Abs(single3);
            if (single3 < 0f)
            {
                single4 = -single4;
            }
            float single5 = 0f;
            float single6 = 0f;
            float single7 = tf1.X + (single1 / 3f);
            float single8 = tf1.Y + (single2 / 3f);
            float single9 = single7;
            float single10 = single8;
            if (base.IsApprox(single2, (float)0f))
            {
                if (single1 > 0f)
                {
                    single10 -= single4;
                }
                else
                {
                    single10 += single4;
                }
            }
            else
            {
                single5 = -single1 / single2;
                single6 = (float)System.Math.Sqrt((double)((single4 * single4) / ((single5 * single5) + 1f)));
                if (single3 < 0f)
                {
                    single6 = -single6;
                }
                single9 = (((single2 < 0f) ? ((float)(-1)) : ((float)1)) * single6) + single7;
                single10 = (single5 * (single9 - single7)) + single8;
            }
            single7 = tf1.X + ((2f * single1) / 3f);
            single8 = tf1.Y + ((2f * single2) / 3f);
            float single11 = single7;
            float single12 = single8;
            if (base.IsApprox(single2, (float)0f))
            {
                if (single1 > 0f)
                {
                    single12 -= single4;
                }
                else
                {
                    single12 += single4;
                }
            }
            else
            {
                single11 = (((single2 < 0f) ? ((float)(-1)) : ((float)1)) * single6) + single7;
                single12 = (single5 * (single11 - single7)) + single8;
            }
            this.AddPoint(tf1);
            base.AddPoint(single9, single10);
            base.AddPoint(single11, single12);
            this.AddPoint(tf2);
            this.SetPoint(0, from.GetFromLinkPoint(this.AbstractLink));
            this.SetPoint(3, to.GetToLinkPoint(this.AbstractLink));
        }

        private void CalculateLineNoSpot(DiagramShape fromObj, DiagramPort from, DiagramShape toObj, DiagramPort to)
        {
            this.ClearPoints();
            PointF tf1 = fromObj.Center;
            PointF tf2 = toObj.Center;
            if (from == null)
            {
                if (!fromObj.GetNearestIntersectionPoint(tf2, tf1, out tf1))
                {
                    tf1 = fromObj.Center;
                }
            }
            else
            {
                tf1 = from.GetFromLinkPoint(this.AbstractLink);
            }
            if (to == null)
            {
                if (!toObj.GetNearestIntersectionPoint(tf1, tf2, out tf2))
                {
                    tf2 = toObj.Center;
                }
            }
            else
            {
                tf2 = to.GetToLinkPoint(this.AbstractLink);
            }
            this.AddPoint(tf1);
            this.AddPoint(tf2);
        }

        public virtual void CalculateStroke()
        {
            IDiagramPort port1 = this.FromPort;
            IDiagramPort port2 = this.ToPort;
            if ((port1 != null) && (port2 != null))
            {
                DiagramShape obj1 = port1.DiagramShape;
                DiagramShape obj2 = port2.DiagramShape;
                if ((obj1 != null) && (obj2 != null))
                {
                    DiagramPort port3 = obj1 as DiagramPort;
                    DiagramPort port4 = obj2 as DiagramPort;
                    int num1 = this.PointsCount;
                    int num2 = (port3 != null) ? port3.FromSpot : 0;
                    int num3 = (port4 != null) ? port4.ToSpot : 0;
                    bool flag1 = this.IsSelfLoop;
                    bool flag2 = this.Orthogonal;
                    bool flag3 = this.Style == StrokeGraphStyle.Bezier;
                    bool flag4 = this.AdjustingStyle == Shapes.LineAdjustingStyle.Calculate;
                    float single1 = this.Curviness;
                    bool flag5 = base.SuspendsUpdates;
                    if (!flag5)
                    {
                        this.Changing(0x4b4);
                    }
                    base.SuspendsUpdates = true;
                    if (((port3 == null) || (port4 == null)) || ((!flag2 && (num2 == 0)) && ((num3 == 0) && !flag1)))
                    {
                        bool flag6 = false;
                        if (!flag4 && (num1 >= 3))
                        {
                            PointF tf1 = obj1.Center;
                            PointF tf2 = obj2.Center;
                            if (port3 == null)
                            {
                                if (!obj1.GetNearestIntersectionPoint(tf2, tf1, out tf1))
                                {
                                    tf1 = obj1.Center;
                                }
                            }
                            else
                            {
                                tf1 = port3.GetFromLinkPoint(this.AbstractLink);
                            }
                            if (port4 == null)
                            {
                                if (!obj2.GetNearestIntersectionPoint(tf1, tf2, out tf2))
                                {
                                    tf2 = obj2.Center;
                                }
                            }
                            else
                            {
                                tf2 = port4.GetToLinkPoint(this.AbstractLink);
                            }
                            flag6 = this.AdjustPoints(0, tf1, num1 - 1, tf2);
                        }
                        if (!flag6)
                        {
                            if (flag3)
                            {
                                this.CalculateBezierNoSpot(obj1, port3, obj2, port4);
                            }
                            else
                            {
                                this.CalculateLineNoSpot(obj1, port3, obj2, port4);
                            }
                        }
                    }
                    else
                    {
                        PointF tf3 = port3.GetFromLinkPoint(this.AbstractLink);
                        float single2 = 0f;
                        float single3 = 0f;
                        float single4 = 0f;
                        if ((flag2 || (num2 != 0)) || flag1)
                        {
                            float single5 = port3.EndSegmentLength;
                            single4 = port3.GetFromLinkDir(this.AbstractLink);
                            if (flag1)
                            {
                                single4 -= (flag2 ? ((float)90) : ((float)30));
                                if (single1 < 0f)
                                {
                                    single4 -= 180f;
                                }
                                if (single4 < 0f)
                                {
                                    single4 += 360f;
                                }
                            }
                            if (flag3 && (num1 >= 4))
                            {
                                single5 += 15f;
                                if (flag1)
                                {
                                    single5 += System.Math.Abs(single1);
                                }
                            }
                            if (single4 == 0f)
                            {
                                single2 = single5;
                            }
                            else if (single4 == 90f)
                            {
                                single3 = single5;
                            }
                            else if (single4 == 180f)
                            {
                                single2 = -single5;
                            }
                            else if (single4 == 270f)
                            {
                                single3 = -single5;
                            }
                            else
                            {
                                single2 = single5 * ((float)System.Math.Cos((single4 * 3.1415926535897931) / 180));
                                single3 = single5 * ((float)System.Math.Sin((single4 * 3.1415926535897931) / 180));
                            }
                            if ((num2 == 0) && flag1)
                            {
                                tf3 = port3.GetLinkPointFromPoint(new PointF(tf3.X + (single2 * 1000f), tf3.Y + (single3 * 1000f)));
                            }
                        }
                        PointF tf4 = port4.GetToLinkPoint(this.AbstractLink);
                        float single6 = 0f;
                        float single7 = 0f;
                        float single8 = 0f;
                        if ((flag2 || (num3 != 0)) || flag1)
                        {
                            float single9 = port4.EndSegmentLength;
                            single8 = port4.GetToLinkDir(this.AbstractLink);
                            if (flag1)
                            {
                                single8 += (flag2 ? ((float)0) : ((float)30));
                                if (single1 < 0f)
                                {
                                    single8 += 180f;
                                }
                                if (single8 > 360f)
                                {
                                    single8 -= 360f;
                                }
                            }
                            if (flag3 && (num1 >= 4))
                            {
                                single9 += 15f;
                                if (flag1)
                                {
                                    single9 += System.Math.Abs(single1);
                                }
                            }
                            if (single8 == 0f)
                            {
                                single6 = single9;
                            }
                            else if (single8 == 90f)
                            {
                                single7 = single9;
                            }
                            else if (single8 == 180f)
                            {
                                single6 = -single9;
                            }
                            else if (single8 == 270f)
                            {
                                single7 = -single9;
                            }
                            else
                            {
                                single6 = single9 * ((float)System.Math.Cos((single8 * 3.1415926535897931) / 180));
                                single7 = single9 * ((float)System.Math.Sin((single8 * 3.1415926535897931) / 180));
                            }
                            if ((num3 == 0) && flag1)
                            {
                                tf4 = port4.GetLinkPointFromPoint(new PointF(tf4.X + (single6 * 1000f), tf4.Y + (single7 * 1000f)));
                            }
                        }
                        PointF tf5 = tf3;
                        if ((flag2 || (num2 != 0)) || flag1)
                        {
                            tf5 = new PointF(tf3.X + single2, tf3.Y + single3);
                        }
                        PointF tf6 = tf4;
                        if ((flag2 || (num3 != 0)) || flag1)
                        {
                            tf6 = new PointF(tf4.X + single6, tf4.Y + single7);
                        }
                        if (((!flag4 && !flag2) && ((num2 == 0) && (num1 > 3))) && this.AdjustPoints(0, tf3, num1 - 2, tf6))
                        {
                            this.SetPoint(num1 - 1, tf4);
                        }
                        else if (((!flag4 && !flag2) && ((num3 == 0) && (num1 > 3))) && this.AdjustPoints(1, tf5, num1 - 1, tf4))
                        {
                            this.SetPoint(0, tf3);
                        }
                        else if ((!flag4 && !flag2) && ((num1 > 4) && this.AdjustPoints(1, tf5, num1 - 2, tf6)))
                        {
                            this.SetPoint(0, tf3);
                            this.SetPoint(num1 - 1, tf4);
                        }
                        else if (((!flag4 && flag2) && ((num1 >= 6) && !this.AvoidsNodes)) && this.AdjustPoints(1, tf5, num1 - 2, tf6))
                        {
                            this.SetPoint(0, tf3);
                            this.SetPoint(num1 - 1, tf4);
                        }
                        else
                        {
                            this.ClearPoints();
                            this.AddPoint(tf3);
                            if ((flag2 || (num2 != 0)) || flag1)
                            {
                                this.AddPoint(tf5);
                            }
                            if (flag2)
                            {
                                this.AddOrthoPoints(tf5, single4, tf6, single8);
                            }
                            if ((flag2 || (num3 != 0)) || flag1)
                            {
                                this.AddPoint(tf6);
                            }
                            this.AddPoint(tf4);
                        }
                    }
                    base.InvalidBounds = true;
                    base.SuspendsUpdates = flag5;
                    if (!flag5)
                    {
                        RectangleF ef1 = this.Bounds;
                        this.Changed(0x4b4, 0, null, ef1, 0, null, ef1);
                    }
                }
            }
        }

        public override void Changed(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            if (!base.SuspendsUpdates)
            {
                base.Changed(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
                if ((((subhint == 0x4b3) || (subhint == 0x4b1)) || ((subhint == 0x4b2) || (subhint == 0x4b4))) || ((subhint == 0x4b6) || (subhint == 0x4b5)))
                {
                    this.AbstractLink.OnPortChanged(null, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
                    this.PortsOnLinkChanged(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
                }
            }
        }

        public override void ChangeValue(ChangedEventArgs e, bool undo)
        {
            switch (e.SubHint)
            {
                case 1300:
                    {
                        this.UserFlags = e.GetInt(undo);
                        return;
                    }
                case 0x515:
                    {
                        this.UserObject = e.GetValue(undo);
                        return;
                    }
                case 0x516:
                    {
                        this.FromPort = (IDiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0x517:
                    {
                        this.ToPort = (IDiagramPort)e.GetValue(undo);
                        return;
                    }
                case 0x518:
                    {
                        this.setOrthogonal((bool)e.GetValue(undo), true);
                        return;
                    }
                case 0x519:
                    {
                        this.Relinkable = (bool)e.GetValue(undo);
                        return;
                    }
                case 0x51a:
                    {
                        this.AbstractLink = (IDiagramLine)e.GetValue(undo);
                        return;
                    }
                case 0x51b:
                    {
                        this.setAvoidsNodes((bool)e.GetValue(undo), true);
                        return;
                    }
                case 0x51d:
                    {
                        this.PartID = e.GetInt(undo);
                        return;
                    }
                case 1310:
                    {
                        this.AdjustingStyle = (LineAdjustingStyle)e.GetInt(undo);
                        return;
                    }
            }
            base.ChangeValue(e, undo);
        }

        public override DiagramShape CopyObject(CopyDictionary env)
        {
            LineGraph link1 = (LineGraph)base.CopyObject(env);
            if (link1 != null)
            {
                env.Delayeds.Add(this);
                link1.myAbstractLink = (IDiagramLine)env.Copy(this.myAbstractLink.DiagramShape);
                link1.myFromPort = null;
                link1.myToPort = null;
                link1.myPartID = -1;
            }
            return link1;
        }

        public override void CopyObjectDelayed(CopyDictionary env, DiagramShape newobj)
        {
            base.CopyObjectDelayed(env, newobj);
            LineGraph link1 = newobj as LineGraph;
            if (link1 != null)
            {
                IDiagramPort port1 = this.FromPort;
                IDiagramPort port2 = this.ToPort;
                IDiagramPort port3 = env[port1] as IDiagramPort;
                IDiagramPort port4 = env[port2] as IDiagramPort;
                IDiagramLine link2 = link1.AbstractLink;
                if (link1.Movable || (((port1 == null) || (port3 != null)) && ((port2 == null) || (port4 != null))))
                {
                    link1.myFromPort = port3;
                    link1.myToPort = port4;
                    if (port3 != null)
                    {
                        port3.AddDestinationLink(link2);
                    }
                    if (port4 != null)
                    {
                        port4.AddSourceLink(link2);
                    }
                }
                else
                {
                    link2.DiagramShape.Remove();
                }
            }
        }

        public override void DoResize(DiagramView view, RectangleF origRect, PointF newPoint, int whichHandle, InputState evttype, SizeF min, SizeF max)
        {
            if ((this.ResizesRealtime || (evttype == InputState.Finish)) || (evttype == InputState.Cancel))
            {
                PointF tf4;
                int num1 = this.FirstPickIndex + 1;
                int num2 = this.LastPickIndex - 1;
                int num5 = whichHandle;
                if (num5 <= 0x40)
                {
                    if (num5 == 0x20)
                    {
                        tf4 = this.GetPoint(num1 - 1);
                        this.SetPoint(num1, new PointF(tf4.X, newPoint.Y));
                        tf4 = this.GetPoint(num1 + 2);
                        this.SetPoint(num1 + 1, new PointF(tf4.X, newPoint.Y));
                        return;
                    }
                    if (num5 == 0x40)
                    {
                        tf4 = this.GetPoint(num2 - 2);
                        this.SetPoint(num2 - 1, new PointF(newPoint.X, tf4.Y));
                        tf4 = this.GetPoint(num2 + 1);
                        this.SetPoint(num2, new PointF(newPoint.X, tf4.Y));
                        return;
                    }
                }
                else
                {
                    if (num5 == 0x80)
                    {
                        tf4 = this.GetPoint(num2 - 2);
                        this.SetPoint(num2 - 1, new PointF(tf4.X, newPoint.Y));
                        tf4 = this.GetPoint(num2 + 1);
                        this.SetPoint(num2, new PointF(tf4.X, newPoint.Y));
                        return;
                    }
                    if (num5 == 0x100)
                    {
                        tf4 = this.GetPoint(num1 - 1);
                        this.SetPoint(num1, new PointF(newPoint.X, tf4.Y));
                        tf4 = this.GetPoint(num1 + 2);
                        this.SetPoint(num1 + 1, new PointF(newPoint.X, tf4.Y));
                        return;
                    }
                }
                int num3 = this.PointsCount;
                if ((num3 >= 2) && (whichHandle >= 0x2000))
                {
                    int num4 = whichHandle - 0x2000;
                    PointF tf1 = this.GetPoint(num4);
                    if (this.Orthogonal)
                    {
                        PointF tf2 = this.GetPoint(num4 - 1);
                        PointF tf3 = this.GetPoint(num4 + 1);
                        if (base.IsApprox(tf2.X, tf1.X) && base.IsApprox(tf1.Y, tf3.Y))
                        {
                            this.SetPoint(num4 - 1, new PointF(newPoint.X, tf2.Y));
                            this.SetPoint(num4 + 1, new PointF(tf3.X, newPoint.Y));
                        }
                        else if (base.IsApprox(tf2.Y, tf1.Y) && base.IsApprox(tf1.X, tf3.X))
                        {
                            this.SetPoint(num4 - 1, new PointF(tf2.X, newPoint.Y));
                            this.SetPoint(num4 + 1, new PointF(newPoint.X, tf3.Y));
                        }
                        else if (base.IsApprox(tf2.X, tf1.X) && base.IsApprox(tf1.X, tf3.X))
                        {
                            this.SetPoint(num4 - 1, new PointF(newPoint.X, tf2.Y));
                            this.SetPoint(num4 + 1, new PointF(newPoint.X, tf3.Y));
                        }
                        else if (base.IsApprox(tf2.Y, tf1.Y) && base.IsApprox(tf1.Y, tf3.Y))
                        {
                            this.SetPoint(num4 - 1, new PointF(tf2.X, newPoint.Y));
                            this.SetPoint(num4 + 1, new PointF(tf3.X, newPoint.Y));
                        }
                    }
                    this.SetPoint(num4, newPoint);
                    if (num3 >= 3)
                    {
                        if ((num4 == 1) && (this.FromPort != null))
                        {
                            DiagramPort port1 = this.FromPort.DiagramShape as DiagramPort;
                            if (port1 != null)
                            {
                                this.SetPoint(0, port1.GetFromLinkPoint(this.AbstractLink));
                            }
                        }
                        if ((num4 == (num3 - 2)) && (this.ToPort != null))
                        {
                            DiagramPort port2 = this.ToPort.DiagramShape as DiagramPort;
                            if (port2 != null)
                            {
                                this.SetPoint(num3 - 1, port2.GetToLinkPoint(this.AbstractLink));
                            }
                        }
                    }
                }
            }
        }

        protected virtual float GetMidOrthoPosition(float fromPosition, float toPosition, bool vertical)
        {
            return ((fromPosition + toPosition) / 2f);
        }

        public IDiagramNode GetOtherNode(IDiagramNode n)
        {
            return LineGraph.GetOtherNode(this, n);
        }

        public static IDiagramNode GetOtherNode(IDiagramLine l, IDiagramNode n)
        {
            if (l.FromPort.Node == n)
            {
                return l.ToPort.Node;
            }
            if (l.ToPort.Node == n)
            {
                return l.FromPort.Node;
            }
            return null;
        }

        public IDiagramPort GetOtherPort(IDiagramPort p)
        {
            return LineGraph.GetOtherPort(this, p);
        }

        public static IDiagramPort GetOtherPort(IDiagramLine l, IDiagramPort p)
        {
            if (l.FromPort == p)
            {
                return l.ToPort;
            }
            if (l.ToPort == p)
            {
                return l.FromPort;
            }
            return null;
        }

        protected virtual bool ModifyEndPoints(int startIndex, PointF newFromPoint, int endIndex, PointF newToPoint)
        {
            if (this.Orthogonal)
            {
                PointF tf1 = this.GetPoint(startIndex + 1);
                PointF tf2 = this.GetPoint(startIndex + 2);
                if (base.IsApprox(tf1.X, tf2.X) && !base.IsApprox(tf1.Y, tf2.Y))
                {
                    this.SetPoint(startIndex + 1, new PointF(tf1.X, newFromPoint.Y));
                }
                else if (base.IsApprox(tf1.Y, tf2.Y))
                {
                    this.SetPoint(startIndex + 1, new PointF(newFromPoint.X, tf1.Y));
                }
                tf1 = this.GetPoint(endIndex - 1);
                tf2 = this.GetPoint(endIndex - 2);
                if (base.IsApprox(tf1.X, tf2.X) && !base.IsApprox(tf1.Y, tf2.Y))
                {
                    this.SetPoint(endIndex - 1, new PointF(tf1.X, newToPoint.Y));
                }
                else if (base.IsApprox(tf1.Y, tf2.Y))
                {
                    this.SetPoint(endIndex - 1, new PointF(newToPoint.X, tf1.Y));
                }
            }
            this.SetPoint(startIndex, newFromPoint);
            this.SetPoint(endIndex, newToPoint);
            return true;
        }

        protected override void OnLayerChanged(DiagramLayer oldlayer, DiagramLayer newlayer, DiagramShape mainObj)
        {
            base.OnLayerChanged(oldlayer, newlayer, mainObj);
            if (((newlayer == null) && !this.NoClearPorts) && ((mainObj is IDiagramLine) || !base.IsChildOf(mainObj)))
            {
                IDiagramLine link1 = this.AbstractLink;
                IDiagramPort port1 = this.FromPort;
                if (port1 != null)
                {
                    port1.RemoveLink(link1);
                }
                IDiagramPort port2 = this.ToPort;
                if (port2 != null)
                {
                    port2.RemoveLink(link1);
                }
            }
            else if (newlayer != null)
            {
                IDiagramLine link2 = this.AbstractLink;
                IDiagramPort port3 = this.FromPort;
                if (port3 != null)
                {
                    port3.AddDestinationLink(link2);
                }
                IDiagramPort port4 = this.ToPort;
                if (port4 != null)
                {
                    port4.AddSourceLink(link2);
                }
            }
        }

        public virtual void OnPortChanged(IDiagramPort port, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            if (port != null)
            {
                if ((subhint == 0x516) || (subhint == 0x517))
                {
                    if (((oldVal != newVal) || (this.AdjustingStyle == LineAdjustingStyle.Calculate)) || ((this.AdjustingStyle == LineAdjustingStyle.Scale) && this.Orthogonal))
                    {
                        this.CalculateStroke();
                    }
                    this.PortsOnLinkChanged(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
                }
                else if ((subhint != 0x6a6) && (subhint != 0x6a7))
                {
                    DiagramPort port1 = port.DiagramShape as DiagramPort;
                    if (((port1 != null) && (port1 == this.FromPort)) && (this.PointsCount > 0))
                    {
                        PointF tf1 = port1.GetFromLinkPoint(this.AbstractLink);
                        PointF tf2 = this.GetPoint(0);
                        if (!base.IsApprox(tf1.X, tf2.X) || !base.IsApprox(tf1.Y, tf2.Y))
                        {
                            this.CalculateStroke();
                        }
                    }
                    else if (((port1 != null) && (port1 == this.ToPort)) && (this.PointsCount >= 2))
                    {
                        PointF tf3 = port1.GetToLinkPoint(this.AbstractLink);
                        PointF tf4 = this.GetPoint(this.PointsCount - 1);
                        if (!base.IsApprox(tf3.X, tf4.X) || !base.IsApprox(tf3.Y, tf4.Y))
                        {
                            this.CalculateStroke();
                        }
                    }
                    else
                    {
                        this.CalculateStroke();
                    }
                }
            }
        }

        public virtual void PortsOnLinkChanged(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
        {
            if (this.FromPort != null)
            {
                this.FromPort.OnLinkChanged(this, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
            }
            if (this.ToPort != null)
            {
                this.ToPort.OnLinkChanged(this, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
            }
        }

        protected virtual bool RescalePoints(int startIndex, PointF newFromPoint, int endIndex, PointF newToPoint)
        {
            PointF tf1 = this.GetPoint(startIndex);
            PointF tf2 = this.GetPoint(endIndex);
            if ((tf1 != newFromPoint) || (tf2 != newToPoint))
            {
                double num8;
                double num16;
                double num1 = tf1.X;
                double num2 = tf1.Y;
                double num3 = tf2.X;
                double num4 = tf2.Y;
                double num5 = num3 - num1;
                double num6 = num4 - num2;
                double num7 = System.Math.Sqrt((num5 * num5) + (num6 * num6));
                if (base.IsApprox(num7, (double)0))
                {
                    return true;
                }
                if (base.IsApprox(num5, (double)0))
                {
                    if (num6 < 0)
                    {
                        num8 = -1.5707963267948966;
                    }
                    else
                    {
                        num8 = 1.5707963267948966;
                    }
                }
                else
                {
                    num8 = System.Math.Atan(num6 / System.Math.Abs(num5));
                    if (num5 < 0)
                    {
                        num8 = 3.1415926535897931 - num8;
                    }
                }
                double num9 = newFromPoint.X;
                double num10 = newFromPoint.Y;
                double num11 = newToPoint.X;
                double num12 = newToPoint.Y;
                double num13 = num11 - num9;
                double num14 = num12 - num10;
                double num15 = System.Math.Sqrt((num13 * num13) + (num14 * num14));
                if (base.IsApprox(num13, (double)0))
                {
                    if (num14 < 0)
                    {
                        num16 = -1.5707963267948966;
                    }
                    else
                    {
                        num16 = 1.5707963267948966;
                    }
                }
                else
                {
                    num16 = System.Math.Atan(num14 / System.Math.Abs(num13));
                    if (num13 < 0)
                    {
                        num16 = 3.1415926535897931 - num16;
                    }
                }
                double num17 = num15 / num7;
                double num18 = num16 - num8;
                this.SetPoint(startIndex, newFromPoint);
                for (int num19 = startIndex + 1; num19 < endIndex; num19++)
                {
                    PointF tf3 = this.GetPoint(num19);
                    num5 = tf3.X - num1;
                    num6 = tf3.Y - num2;
                    double num20 = System.Math.Sqrt((num5 * num5) + (num6 * num6));
                    if (!base.IsApprox(num20, (double)0))
                    {
                        double num21;
                        if (base.IsApprox(num5, (double)0))
                        {
                            if (num6 < 0)
                            {
                                num21 = -1.5707963267948966;
                            }
                            else
                            {
                                num21 = 1.5707963267948966;
                            }
                        }
                        else
                        {
                            num21 = System.Math.Atan(num6 / System.Math.Abs(num5));
                            if (num5 < 0)
                            {
                                num21 = 3.1415926535897931 - num21;
                            }
                        }
                        double num22 = num21 + num18;
                        double num23 = num20 * num17;
                        double num24 = num9 + (num23 * System.Math.Cos(num22));
                        double num25 = num10 + (num23 * System.Math.Sin(num22));
                        this.SetPoint(num19, new PointF((float)num24, (float)num25));
                    }
                }
                this.SetPoint(endIndex, newToPoint);
            }
            return true;
        }

        private void setAvoidsNodes(bool avoid, bool undoing)
        {
            bool flag1 = (base.InternalFlags & 0x2000000) != 0;
            if (flag1 != avoid)
            {
                if (avoid)
                {
                    base.InternalFlags |= 0x2000000;
                }
                else
                {
                    base.InternalFlags &= -33554433;
                }
                this.Changed(0x51b, 0, flag1, DiagramShape.NullRect, 0, avoid, DiagramShape.NullRect);
                this.PortsOnLinkChanged(0x51b, 0, flag1, DiagramShape.NullRect, 0, avoid, DiagramShape.NullRect);
                if (!undoing && avoid)
                {
                    this.ClearPoints();
                    this.CalculateStroke();
                }
            }
        }

        private void setOrthogonal(bool ortho, bool undoing)
        {
            bool flag1 = (base.InternalFlags & 0x4000000) != 0;
            if (flag1 != ortho)
            {
                if (ortho)
                {
                    base.InternalFlags |= 0x4000000;
                }
                else
                {
                    base.InternalFlags &= -67108865;
                }
                this.Changed(0x518, 0, flag1, DiagramShape.NullRect, 0, ortho, DiagramShape.NullRect);
                this.PortsOnLinkChanged(0x518, 0, flag1, DiagramShape.NullRect, 0, ortho, DiagramShape.NullRect);
                if (!undoing && ortho)
                {
                    this.ClearPoints();
                    this.CalculateStroke();
                }
            }
        }

        protected virtual bool StretchPoints(int startIndex, PointF newFromPoint, int endIndex, PointF newToPoint)
        {
            PointF tf1 = this.GetPoint(startIndex);
            PointF tf2 = this.GetPoint(endIndex);
            if ((tf1 != newFromPoint) || (tf2 != newToPoint))
            {
                float single1 = tf1.X;
                float single2 = tf1.Y;
                float single3 = tf2.X;
                float single4 = tf2.Y;
                float single5 = ((single3 - single1) * (single3 - single1)) + ((single4 - single2) * (single4 - single2));
                float single6 = newFromPoint.X;
                float single7 = newFromPoint.Y;
                float single8 = newToPoint.X;
                float single9 = newToPoint.Y;
                float single10 = 0f;
                float single11 = 1f;
                if ((single8 - single6) != 0f)
                {
                    single10 = (single9 - single7) / (single8 - single6);
                }
                if (single10 != 0f)
                {
                    single11 = (float)System.Math.Sqrt((double)(1f + (1f / (single10 * single10))));
                }
                this.SetPoint(startIndex, newFromPoint);
                for (int num1 = startIndex + 1; num1 < endIndex; num1++)
                {
                    PointF tf3 = this.GetPoint(num1);
                    float single12 = tf3.X;
                    float single13 = tf3.Y;
                    float single14 = 0.5f;
                    if (single5 != 0f)
                    {
                        single14 = (((single1 - single12) * (single1 - single3)) + ((single2 - single13) * (single2 - single4))) / single5;
                    }
                    float single15 = single1 + (single14 * (single3 - single1));
                    float single16 = single2 + (single14 * (single4 - single2));
                    float single17 = (float)System.Math.Sqrt((double)(((single12 - single15) * (single12 - single15)) + ((single13 - single16) * (single13 - single16))));
                    if (single13 < ((single10 * (single12 - single15)) + single16))
                    {
                        single17 = -single17;
                    }
                    if (single10 > 0f)
                    {
                        single17 = -single17;
                    }
                    float single18 = single6 + (single14 * (single8 - single6));
                    float single19 = single7 + (single14 * (single9 - single7));
                    if (single10 != 0f)
                    {
                        float single20 = single18 + (single17 / single11);
                        float single21 = single19 - ((single20 - single18) / single10);
                        this.SetPoint(num1, new PointF(single20, single21));
                    }
                    else
                    {
                        this.SetPoint(num1, new PointF(single18, single19 + single17));
                    }
                }
                this.SetPoint(endIndex, newToPoint);
            }
            return true;
        }

        private void TraversePositions(PositionArray positions, float px, float py, float dir, bool first)
        {
            SizeF ef1 = positions.CellSize;
            int num1 = positions.GetDist(px, py);
            float single1 = px;
            float single2 = py;
            float single3 = single1;
            float single4 = single2;
            if (dir == 0f)
            {
                single3 += ef1.Width;
            }
            else if (dir == 90f)
            {
                single4 += ef1.Height;
            }
            else if (dir == 180f)
            {
                single3 -= ef1.Width;
            }
            else
            {
                single4 -= ef1.Height;
            }
            while ((num1 > 1) && (positions.GetDist(single3, single4) == (num1 - 1)))
            {
                single1 = single3;
                single2 = single4;
                if (dir == 0f)
                {
                    single3 += ef1.Width;
                }
                else if (dir == 90f)
                {
                    single4 += ef1.Height;
                }
                else if (dir == 180f)
                {
                    single3 -= ef1.Width;
                }
                else
                {
                    single4 -= ef1.Height;
                }
                num1--;
            }
            if (first)
            {
                if (num1 > 1)
                {
                    if ((dir == 180f) || (dir == 0f))
                    {
                        single1 = (((float)System.Math.Floor((double)(single1 / ef1.Width))) * ef1.Width) + (ef1.Width / 2f);
                    }
                    else
                    {
                        single2 = (((float)System.Math.Floor((double)(single2 / ef1.Height))) * ef1.Height) + (ef1.Height / 2f);
                    }
                }
            }
            else
            {
                single1 = (((float)System.Math.Floor((double)(single1 / ef1.Width))) * ef1.Width) + (ef1.Width / 2f);
                single2 = (((float)System.Math.Floor((double)(single2 / ef1.Height))) * ef1.Height) + (ef1.Height / 2f);
            }
            if (num1 > 1)
            {
                float single5 = dir;
                float single6 = single1;
                float single7 = single2;
                if (dir == 0f)
                {
                    single5 = 90f;
                    single7 += ef1.Height;
                }
                else if (dir == 90f)
                {
                    single5 = 180f;
                    single6 -= ef1.Width;
                }
                else if (dir == 180f)
                {
                    single5 = 270f;
                    single7 -= ef1.Height;
                }
                else if (dir == 270f)
                {
                    single5 = 0f;
                    single6 += ef1.Width;
                }
                if (positions.GetDist(single6, single7) == (num1 - 1))
                {
                    this.TraversePositions(positions, single6, single7, single5, false);
                }
                else
                {
                    float single8 = single1;
                    float single9 = single2;
                    if (dir == 0f)
                    {
                        single5 = 270f;
                        single9 -= ef1.Height;
                    }
                    else if (dir == 90f)
                    {
                        single5 = 0f;
                        single8 += ef1.Width;
                    }
                    else if (dir == 180f)
                    {
                        single5 = 90f;
                        single9 += ef1.Height;
                    }
                    else if (dir == 270f)
                    {
                        single5 = 180f;
                        single8 -= ef1.Width;
                    }
                    if (positions.GetDist(single8, single9) == (num1 - 1))
                    {
                        this.TraversePositions(positions, single8, single9, single5, false);
                    }
                }
            }
            base.AddPoint(single1, single2);
        }

        public virtual void Unlink()
        {
            this.AbstractLink.DiagramShape.Remove();
        }


        [Description("The object acting as the IGoLink.")]
        public virtual IDiagramLine AbstractLink
        {
            get
            {
                return this.myAbstractLink;
            }
            set
            {
                IDiagramLine link1 = this.myAbstractLink;
                if ((link1 != value) && (value != null))
                {
                    IDiagramPort port1 = this.FromPort;
                    if (port1 != null)
                    {
                        port1.RemoveLink(link1);
                    }
                    IDiagramPort port2 = this.ToPort;
                    if (port2 != null)
                    {
                        port2.RemoveLink(link1);
                    }
                    this.myAbstractLink = value;
                    if (port1 != null)
                    {
                        port1.AddDestinationLink(value);
                    }
                    if (port2 != null)
                    {
                        port2.AddSourceLink(value);
                    }
                    this.Changed(0x51a, 0, link1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("How CalculateStroke behaves."), DefaultValue(0)]
        public virtual LineAdjustingStyle AdjustingStyle
        {
            get
            {
                return this.myAdjustingStyle;
            }
            set
            {
                LineAdjustingStyle style1 = this.myAdjustingStyle;
                if (style1 != value)
                {
                    this.myAdjustingStyle = value;
                    this.Changed(1310, (int)style1, null, DiagramShape.NullRect, (int)value, null, DiagramShape.NullRect);
                }
            }
        }

        [Description("Whether an Orthogonal link tries to avoid crossing over any nodes."), Category("Appearance"), DefaultValue(false)]
        public virtual bool AvoidsNodes
        {
            get
            {
                return ((base.InternalFlags & 0x2000000) != 0);
            }
            set
            {
                this.setAvoidsNodes(value, false);
            }
        }

        public override int FirstPickIndex
        {
            get
            {
                DiagramPort port1 = this.FromPort as DiagramPort;
                if (port1 == null)
                {
                    return 0;
                }
                if (this.PointsCount <= 2)
                {
                    return 0;
                }
                if ((port1.FromSpot == 0) && !this.Orthogonal)
                {
                    return 0;
                }
                return 1;
            }
        }

        [Description("The node that the link is coming from.")]
        public virtual IDiagramNode FromNode
        {
            get
            {
                IDiagramPort port1 = this.FromPort;
                if (port1 != null)
                {
                    return port1.Node;
                }
                return null;
            }
        }

        [DefaultValue((string)null), Description("The port that the link is coming from.")]
        public virtual IDiagramPort FromPort
        {
            get
            {
                return this.myFromPort;
            }
            set
            {
                IDiagramPort port1 = this.myFromPort;
                if (port1 != value)
                {
                    IDiagramLine link1 = this.AbstractLink;
                    if ((port1 != null) && (link1.ToPort != port1))
                    {
                        port1.RemoveLink(link1);
                    }
                    this.myFromPort = value;
                    if (value != null)
                    {
                        value.AddDestinationLink(link1);
                    }
                    this.Changed(0x516, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    link1.OnPortChanged(value, 0x516, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("Returns itself as a DiagramShape.")]
        public DiagramShape DiagramShape
        {
            get
            {
                return this;
            }
            set
            {
            }
        }

        public virtual bool IsSelfLoop
        {
            get
            {
                if (this.FromPort == this.ToPort)
                {
                    return (this.FromPort != null);
                }
                return false;
            }
        }

        public override int LastPickIndex
        {
            get
            {
                int num1 = this.PointsCount;
                if (num1 == 0)
                {
                    return 0;
                }
                DiagramPort port1 = this.ToPort as DiagramPort;
                if (port1 == null)
                {
                    return (num1 - 1);
                }
                if (num1 <= 2)
                {
                    return (num1 - 1);
                }
                if ((port1.ToSpot == 0) && !this.Orthogonal)
                {
                    return (num1 - 1);
                }
                return (num1 - 2);
            }
        }

        internal bool NoClearPorts
        {
            get
            {
                return ((base.InternalFlags & 0x10000000) != 0);
            }
            set
            {
                if (value)
                {
                    base.InternalFlags |= 0x10000000;
                }
                else
                {
                    base.InternalFlags &= -268435457;
                }
            }
        }

        [Category("Appearance"), Description("Whether the segments of the link are always horizontal and vertical."), DefaultValue(false)]
        public virtual bool Orthogonal
        {
            get
            {
                return ((base.InternalFlags & 0x4000000) != 0);
            }
            set
            {
                this.setOrthogonal(value, false);
            }
        }

        [Description("The unique ID of this part in its document."), Category("Ownership")]
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
                    this.Changed(0x51d, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [Category("Behavior"), Description("Whether the user may reconnect this link to another port."), DefaultValue(true)]
        public virtual bool Relinkable
        {
            get
            {
                return ((base.InternalFlags & 0x8000000) != 0);
            }
            set
            {
                bool flag1 = (base.InternalFlags & 0x8000000) != 0;
                if (flag1 != value)
                {
                    if (value)
                    {
                        base.InternalFlags |= 0x8000000;
                    }
                    else
                    {
                        base.InternalFlags &= -134217729;
                    }
                    this.Changed(0x519, 0, flag1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        public override StrokeGraphStyle Style
        {
            get
            {
                if (this.IsSelfLoop && !this.Orthogonal)
                {
                    return StrokeGraphStyle.Bezier;
                }
                return base.Style;
            }
            set
            {
                base.Style = value;
            }
        }

        [Description("The node that the link is going to.")]
        public virtual IDiagramNode ToNode
        {
            get
            {
                IDiagramPort port1 = this.ToPort;
                if (port1 != null)
                {
                    return port1.Node;
                }
                return null;
            }
        }

        [DefaultValue((string)null), Description("The port that the link is going to.")]
        public virtual IDiagramPort ToPort
        {
            get
            {
                return this.myToPort;
            }
            set
            {
                IDiagramPort port1 = this.myToPort;
                if (port1 != value)
                {
                    IDiagramLine link1 = this.AbstractLink;
                    if ((port1 != null) && (link1.FromPort != port1))
                    {
                        port1.RemoveLink(link1);
                    }
                    this.myToPort = value;
                    if (value != null)
                    {
                        value.AddSourceLink(link1);
                    }
                    this.Changed(0x517, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                    link1.OnPortChanged(value, 0x517, 0, port1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }

        [Description("An integer value associated with this link."), DefaultValue(0)]
        public virtual int UserFlags
        {
            get
            {
                return this.myUserFlags;
            }
            set
            {
                int num1 = this.myUserFlags;
                if (num1 != value)
                {
                    this.myUserFlags = value;
                    this.Changed(1300, num1, null, DiagramShape.NullRect, value, null, DiagramShape.NullRect);
                }
            }
        }

        [Description("An object associated with this link."), DefaultValue((string)null)]
        public virtual object UserObject
        {
            get
            {
                return this.myUserObject;
            }
            set
            {
                object obj1 = this.myUserObject;
                if (obj1 != value)
                {
                    this.myUserObject = value;
                    this.Changed(0x515, 0, obj1, DiagramShape.NullRect, 0, value, DiagramShape.NullRect);
                }
            }
        }


        public const int ChangedAbstractLink = 0x51a;
        public const int ChangedAdjustingStyle = 1310;
        public const int ChangedAvoidsNodes = 0x51b;
        public const int ChangedFromPort = 0x516;
        public const int ChangedLinkUserFlags = 1300;
        public const int ChangedLinkUserObject = 0x515;
        public const int ChangedOrthogonal = 0x518;
        public const int ChangedPartID = 0x51d;
        public const int ChangedRelinkable = 0x519;
        public const int ChangedToPort = 0x517;
        private const float DOWN = 90f;
        private const int flagLinkAvoidsNodes = 0x2000000;
        private const int flagLinkOrtho = 0x4000000;
        private const int flagNoClearPorts = 0x10000000;
        private const int flagRelinkable = 0x8000000;
        private const float LEFT = 180f;
        private IDiagramLine myAbstractLink;
        private LineAdjustingStyle myAdjustingStyle;
        private IDiagramPort myFromPort;
        private int myPartID;
        private IDiagramPort myToPort;
        private int myUserFlags;
        private object myUserObject;
        public const int RelinkableFromHandle = 0x400;
        public const int RelinkableToHandle = 0x401;
        private const float RIGHT = 0f;
        private const float UP = 270f;
    }
}
