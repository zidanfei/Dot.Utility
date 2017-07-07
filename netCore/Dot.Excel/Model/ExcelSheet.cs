using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Excel.Model
{
    public class ExcelSheet
    {
        public ExcelSheet()
        {
            RegionList = new List<Region>();
            CellList = new List<ExcelCell>();
        }
        /// <summary>
        /// 工作簿名称
        /// </summary>
        public string SheetName { get; set; }
        /// <summary>
        /// 单元格列表
        /// </summary>
        public List<ExcelCell> CellList { get; set; }
        /// <summary>
        /// 不存在则创建
        /// </summary>
        public bool Create { get; set; }
        /// <summary>
        /// 合并单元格
        /// </summary>
        public List<Region> RegionList { get; set; }


    }

}
