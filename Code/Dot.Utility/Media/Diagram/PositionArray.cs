using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    internal sealed class PositionArray
    {
        internal PositionArray()
        {
            this.myInvalid = true;
            this.myMinX = 1f;
            this.myMinY = 1f;
            this.myMaxX = -1f;
            this.myMaxY = -1f;
            this.myCellX = 10f;
            this.myCellY = 10f;
            this.myArray = null;
            this.myUpperBoundX = 0;
            this.myUpperBoundY = 0;
        }

        private void BreakIn(int x, int y, int inc, bool vert, int lowx, int hix, int lowy, int hiy)
        {
            int num1 = this.myArray[x, y];
            this.myArray[x, y] = 0x7fffffff;
            while (((num1 == 0) && (x > lowx)) && (((x < hix) && (y > lowy)) && (y < hiy)))
            {
                if (vert)
                {
                    y += inc;
                }
                else
                {
                    x += inc;
                }
                num1 = this.myArray[x, y];
                this.myArray[x, y] = 0x7fffffff;
            }
        }

        private int BreakOut(int x, int y, int inc, bool vert, int lowx, int hix, int lowy, int hiy)
        {
            int num1 = 1;
            int num2 = this.myArray[x, y];
            this.myArray[x, y] = num1;
            while (((num2 == 0) && (x > lowx)) && (((x < hix) && (y > lowy)) && (y < hiy)))
            {
                if (vert)
                {
                    y += inc;
                }
                else
                {
                    x += inc;
                }
                num2 = this.myArray[x, y];
                this.myArray[x, y] = num1++;
            }
            if (vert)
            {
                return y;
            }
            return x;
        }

        internal int GetDist(float x, float y)
        {
            if (!this.InBounds(x, y))
            {
                return 0;
            }
            x -= this.myMinX;
            x /= this.myCellX;
            y -= this.myMinY;
            y /= this.myCellY;
            int num1 = (int)x;
            int num2 = (int)y;
            return this.myArray[num1, num2];
        }

        private bool InBounds(float x, float y)
        {
            if (((this.myMinX <= x) && (x <= this.myMaxX)) && (this.myMinY <= y))
            {
                return (y <= this.myMaxY);
            }
            return false;
        }

        internal void Initialize(RectangleF rect)
        {
            if ((rect.Width > 0f) && (rect.Height > 0f))
            {
                float single1 = rect.X;
                float single2 = rect.Y;
                float single3 = rect.X + rect.Width;
                float single4 = rect.Y + rect.Height;
                this.myMinX = ((float)System.Math.Floor((double)((single1 - this.myCellX) / this.myCellX))) * this.myCellX;
                this.myMinY = ((float)System.Math.Floor((double)((single2 - this.myCellY) / this.myCellY))) * this.myCellY;
                this.myMaxX = ((float)System.Math.Ceiling((double)((single3 + (2f * this.myCellX)) / this.myCellX))) * this.myCellX;
                this.myMaxY = ((float)System.Math.Ceiling((double)((single4 + (2f * this.myCellY)) / this.myCellY))) * this.myCellY;
                int num1 = 1 + ((int)System.Math.Ceiling((double)((this.myMaxX - this.myMinX) / this.myCellX)));
                int num2 = 1 + ((int)System.Math.Ceiling((double)((this.myMaxY - this.myMinY) / this.myCellY)));
                if (((this.myArray == null) || (this.myUpperBoundX < (num1 - 1))) || (this.myUpperBoundY < (num2 - 1)))
                {
                    this.myArray = new int[num1, num2];
                    this.myUpperBoundX = num1 - 1;
                    this.myUpperBoundY = num2 - 1;
                }
                this.SetAll(0x7fffffff);
            }
        }

        internal bool IsOccupied(float x, float y)
        {
            return (this.GetDist(x, y) == 0);
        }

        internal bool IsUnoccupied(float x, float y, float w, float h)
        {
            int num1 = (int)((x - this.myMinX) / this.myCellX);
            int num2 = (int)((y - this.myMinY) / this.myCellY);
            int num3 = ((int)(System.Math.Max((float)0f, w) / this.myCellX)) + 1;
            int num4 = ((int)(System.Math.Max((float)0f, h) / this.myCellY)) + 1;
            int num5 = System.Math.Min((int)(num1 + num3), this.myUpperBoundX);
            int num6 = System.Math.Min((int)(num2 + num4), this.myUpperBoundY);
            for (int num7 = num1; num7 <= num5; num7++)
            {
                for (int num8 = num2; num8 <= num6; num8++)
                {
                    if (this.myArray[num7, num8] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal void Propagate(PointF p1, float fromDir, PointF p2, float toDir, RectangleF bounds)
        {
            if (this.myArray != null)
            {
                float single1 = p1.X;
                float single2 = p1.Y;
                if (this.InBounds(single1, single2))
                {
                    single1 -= this.myMinX;
                    single1 /= this.myCellX;
                    single2 -= this.myMinY;
                    single2 /= this.myCellY;
                    float single3 = p2.X;
                    float single4 = p2.Y;
                    if (this.InBounds(single3, single4))
                    {
                        single3 -= this.myMinX;
                        single3 /= this.myCellX;
                        single4 -= this.myMinY;
                        single4 /= this.myCellY;
                        if ((System.Math.Abs((float)(single1 - single3)) <= 1f) && (System.Math.Abs((float)(single2 - single4)) <= 1f))
                        {
                            this.myArray[(int)single1, (int)single2] = 0;
                        }
                        else
                        {
                            bool flag1 = false;	//此句有待考证
                            float single5 = bounds.X;
                            float single6 = bounds.Y;
                            float single7 = bounds.X + bounds.Width;
                            float single8 = bounds.Y + bounds.Height;
                            single5 -= this.myMinX;
                            single5 /= this.myCellX;
                            single6 -= this.myMinY;
                            single6 /= this.myCellY;
                            single7 -= this.myMinX;
                            single7 /= this.myCellX;
                            single8 -= this.myMinY;
                            single8 /= this.myCellY;
                            int num1 = System.Math.Max(0, System.Math.Min(this.myUpperBoundX, (int)single5));
                            int num2 = System.Math.Min(this.myUpperBoundX, System.Math.Max(0, (int)single7));
                            int num3 = System.Math.Max(0, System.Math.Min(this.myUpperBoundY, (int)single6));
                            int num4 = System.Math.Min(this.myUpperBoundY, System.Math.Max(0, (int)single8));
                            int num5 = (int)single1;
                            int num6 = (int)single2;
                            int num7 = ((fromDir == 0f) || (fromDir == 90f)) ? 1 : -1;
                            if ((fromDir == 90f) || (fromDir == 270f))
                            {
                                num6 = this.BreakOut(num5, num6, num7, flag1, num1, num2, num3, num4);
                            }
                            else
                            {
                                num5 = this.BreakOut(num5, num6, num7, flag1, num1, num2, num3, num4);
                            }
                            this.BreakIn((int)single3, (int)single4, ((toDir == 0f) || (toDir == 90f)) ? 1 : -1, (toDir == 90f) || (toDir == 270f), num1, num2, num3, num4);
                            this.Spread(num5, num6, 1, false, num1, num2, num3, num4);
                            this.Spread(num5, num6, -1, false, num1, num2, num3, num4);
                            this.Spread(num5, num6, 1, true, num1, num2, num3, num4);
                            this.Spread(num5, num6, -1, true, num1, num2, num3, num4);
                        }
                    }
                }
            }
        }

        private int Ray(int x, int y, int inc, bool vert, int lowx, int hix, int lowy, int hiy)
        {
            int num1 = this.myArray[x, y];
            if ((num1 != 0) && (num1 != 0x7fffffff))
            {
                if (vert)
                {
                    y += inc;
                }
                else
                {
                    x += inc;
                }
                while (((lowx <= x) && (x <= hix)) && (((lowy <= y) && (y <= hiy)) && (++num1 < this.myArray[x, y])))
                {
                    this.myArray[x, y] = num1;
                    if (vert)
                    {
                        y += inc;
                        continue;
                    }
                    x += inc;
                }
            }
            if (vert)
            {
                return y;
            }
            return x;
        }

        internal void SetAll(int v)
        {
            if (this.myArray != null)
            {
                for (int num1 = 0; num1 <= this.myUpperBoundX; num1++)
                {
                    for (int num2 = 0; num2 <= this.myUpperBoundY; num2++)
                    {
                        this.myArray[num1, num2] = v;
                    }
                }
            }
        }

        internal void SetAllUnoccupied(int v)
        {
            if (this.myArray != null)
            {
                for (int num1 = 0; num1 <= this.myUpperBoundX; num1++)
                {
                    for (int num2 = 0; num2 <= this.myUpperBoundY; num2++)
                    {
                        if (this.myArray[num1, num2] != 0)
                        {
                            this.myArray[num1, num2] = v;
                        }
                    }
                }
            }
        }

        internal void SetDist(float x, float y, int v)
        {
            if (this.InBounds(x, y))
            {
                x -= this.myMinX;
                x /= this.myCellX;
                y -= this.myMinY;
                y /= this.myCellY;
                int num1 = (int)x;
                int num2 = (int)y;
                this.myArray[num1, num2] = v;
            }
        }

        private void Spread(int x, int y, int inc, bool vert, int lowx, int hix, int lowy, int hiy)
        {
            if (((x >= lowx) && (x <= hix)) && ((y >= lowy) && (y <= hiy)))
            {
                int num1 = this.Ray(x, y, inc, vert, lowx, hix, lowy, hiy);
                if (vert)
                {
                    if (inc > 0)
                    {
                        for (int num2 = y + inc; num2 < num1; num2 += inc)
                        {
                            this.Spread(x, num2, 1, !vert, lowx, hix, lowy, hiy);
                            this.Spread(x, num2, -1, !vert, lowx, hix, lowy, hiy);
                        }
                    }
                    else
                    {
                        for (int num3 = y + inc; num3 > num1; num3 += inc)
                        {
                            this.Spread(x, num3, 1, !vert, lowx, hix, lowy, hiy);
                            this.Spread(x, num3, -1, !vert, lowx, hix, lowy, hiy);
                        }
                    }
                }
                else if (inc > 0)
                {
                    for (int num4 = x + inc; num4 < num1; num4 += inc)
                    {
                        this.Spread(num4, y, 1, !vert, lowx, hix, lowy, hiy);
                        this.Spread(num4, y, -1, !vert, lowx, hix, lowy, hiy);
                    }
                }
                else
                {
                    for (int num5 = x + inc; num5 > num1; num5 += inc)
                    {
                        this.Spread(num5, y, 1, !vert, lowx, hix, lowy, hiy);
                        this.Spread(num5, y, -1, !vert, lowx, hix, lowy, hiy);
                    }
                }
            }
        }


        internal RectangleF Bounds
        {
            get
            {
                return new RectangleF(this.myMinX, this.myMinY, this.myMaxX - this.myMinX, this.myMaxY - this.myMinY);
            }
        }

        internal SizeF CellSize
        {
            get
            {
                return new SizeF(this.myCellX, this.myCellY);
            }
            set
            {
                if (((value.Width > 0f) && (value.Height > 0f)) && ((value.Width != this.myCellX) || (value.Height != this.myCellY)))
                {
                    this.myCellX = value.Width;
                    this.myCellY = value.Height;
                    this.Initialize(new RectangleF(this.myMinX, this.myMinY, this.myMaxX - this.myMinX, this.myMaxY - this.myMinY));
                }
            }
        }

        internal bool Invalid
        {
            get
            {
                return this.myInvalid;
            }
            set
            {
                this.myInvalid = value;
            }
        }


        private const float DOWN = 90f;
        private const float LEFT = 180f;
        internal const int MAXDIST = 0x7fffffff;
        private int[,] myArray;
        private float myCellX;
        private float myCellY;
        private bool myInvalid;
        private float myMaxX;
        private float myMaxY;
        private float myMinX;
        private float myMinY;
        private int myUpperBoundX;
        private int myUpperBoundY;
        internal const int OCCUPIED = 0;
        private const float RIGHT = 0f;
        internal const int START = 1;
        private const float UP = 270f;
    }
}
