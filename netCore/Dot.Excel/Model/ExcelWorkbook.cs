using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Excel.Model
{
    public class ExcelWorkbook
    {
        public ExcelWorkbook()
        {
            SheetList = new List<ExcelSheet>();
        }
        /// <summary>
        /// 模板不存在则创建
        /// </summary>
        public bool Create { get; set; }
        /// <summary>
        /// 模板路径
        /// </summary>
        public string ModelFilePath { get; set; }
        /// <summary>
        /// 导出文件夹
        /// </summary>
        public string ExportDirectory { get; set; }
        /// <summary>
        /// Sheet列表
        /// </summary>
        public List<ExcelSheet> SheetList { get; set; }
    }
}
