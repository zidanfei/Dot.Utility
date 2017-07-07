using Dot.Excel.Enum;
using Dot.Excel.Model;
using Dot.IExcel;
using Dot.IOC;
using Dot.Log;
using Dot.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Dot.Excel.Excel2007
{
    /// <summary>
    /// http://www.cnblogs.com/yukaizhao/archive/2011/07/19/csharp_xmldocument_access_xml.html
    /// http://www.cnblogs.com/tonyqus/archive/2009/04/12/1434209.html
    /// </summary>
    [Export(typeof(IExcelHandler), Key = "NPOIHelper2007")]
    public class Excel2007Handler : IExcelHandler
    {
        /// <summary>
        /// 导入excel
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public DataSet Import(string path)
        {
            return ExcelHelper.ImportExceltoDS(path);
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheetName">需要导出的sheet</param>
        /// <param name="headerRowIndex">列头所在行号，-1表示没有列头</param>
        public DataTable Import(string strFileName, string sheetName, int headerRowIndex)
        {
            return ExcelHelper.ImportExceltoDt(strFileName, sheetName, headerRowIndex, headerRowIndex > -1 ? true : false);

        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheetIndex">需要导出的sheet序号</param>
        /// <param name="headerRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public DataTable Import(string strFileName, int sheetIndex, int headerRowIndex)
        {
            return ExcelHelper.ImportExceltoDt(strFileName, sheetIndex, headerRowIndex, headerRowIndex > -1 ? true : false);
        }

        static readonly ILog _businessDebugLog = LogFactory.BusinessLog;

        #region 导出excel数据

        #endregion

        public string Export(System.Data.DataSet ds, string exportDirectory)
        {
            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }
            string fileName = DateTime.Now.Ticks.ToString();
            string filePath = System.IO.Path.Combine(exportDirectory, fileName + ".xls");
            if (ExcelHelper.ExportDStoExcel(ds, filePath))
                return filePath;
            else
                return string.Empty;
        }

        public string Export(object obj, string configFilePath, string templateFilePath, string exportDirectory, string exportType, out long fileLength)
        {
            return ExcelHelper.ExportToExcel(obj,  configFilePath,  templateFilePath,  exportDirectory,  exportType, out  fileLength);
        }

        public string Export<T>(IList<T> objs, string configFilePath, string templateFilePath, string exportDirectory, out long fileLength)
        {
            return ExcelHelper.ExportToExcel<T>(objs, configFilePath, templateFilePath, exportDirectory, out fileLength);

        }
    }

}
