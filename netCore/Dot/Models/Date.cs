using System;
using System.Collections.Generic;
using System.Text;

namespace Dot.Models
{
    public class Date
    {
        public Date(int year, int month, int day)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
        }
        public int Year
        {
            get;
            set;
        }
        public int Month
        {
            get;
            set;
        }
        public int Day
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}", Year, Month, Day);
        }
    }
}
