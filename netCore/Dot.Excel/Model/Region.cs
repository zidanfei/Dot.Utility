using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Excel.Model
{
    public class Region
    {
        public Region()
        {
        }
        public Region(int rowFrom, int colFrom, int rowTo, int colTo)
        {
            RowFrom = rowFrom;
            RowTo = rowTo;
            ColumnFrom = colFrom;
            ColumnTo = colTo;
        }

        public int ColumnFrom
        {
            get;
            set;
        }
        public int ColumnTo
        {
            get;
            set;
        }
        public int RowFrom
        {
            get;
            set;
        }
        public int RowTo
        {
            get;
            set;
        }
    }
}
