using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Excel.Enum
{
    public enum CellType
    {
        Unknown = -1,
        Numeric = 0,
        String = 1,
        Formula = 2,
        Blank = 3,
        Boolean = 4,
        Error = 5,
    }
}
