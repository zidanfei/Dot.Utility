using Dot.Excel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Excel.Model
{
    public class ExcelCell
    {
        public ExcelCell()
        {
            CellType = CellType.String;
        }
        public ExcelCell(object value, int rowIndex, int columnIndex, CellType cellType, string sheetName)
            : this(value, rowIndex, columnIndex, cellType)
        {
            this.SheetName = sheetName;
            //this.ColumnIndex = columnIndex;
            //this.RowIndex = rowIndex;
            //this.Value = value;
            //this.CellType = cellType;
        }
        public ExcelCell(object value, int rowIndex, int columnIndex, CellType cellType)
            : this(value, rowIndex, columnIndex)
        {
            //this.ColumnIndex = columnIndex;
            //this.RowIndex = rowIndex;
            //this.Value = value;
            this.CellType = cellType;
        }
        public ExcelCell(object value, int rowIndex, int columnIndex)
        {
            this.ColumnIndex = columnIndex;
            this.RowIndex = rowIndex;
            if (value != null)
            {
                this.Value = value.ToString();
            }
            this.CellType = CellType.String;
        }
        /// <summary>
        /// 工作簿名称
        /// </summary>
        public string SheetName
        {
            get;
            set;
        }

        /// <summary>
        /// 第几行
        /// </summary>
        public int RowIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 第几列
        /// </summary>
        public int ColumnIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// 类型
        /// </summary>
        public CellType CellType
        {
            get;
            set;
        }
    }
}
