using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dot.IExcel
{
    public interface IExcelHandler
    {
        /// <summary>
        /// 导入excel
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        DataSet Import(string path);

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheetName">需要导出的sheet</param>
        /// <param name="headerRowIndex">列头所在行号，-1表示没有列头</param>
        DataTable Import(string strFileName, string sheetName, int headerRowIndex);

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheetIndex">需要导出的sheet序号</param>
        /// <param name="headerRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        DataTable Import(string strFileName, int sheetIndex, int headerRowIndex);

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="obj">数据集</param>
        /// <param name="configFilePath">配置文件路径</param>
        /// <param name="templateFilePath">模板物理路径</param>
        /// <param name="exportType">导出类型，对应配置属性ExportType</param>
        /// <param name="fileLength">导出文件大小</param>
        /// <returns></returns>
        string Export(object obj, string configFilePath, string templateFilePath, string exportDirectory, string exportType, out long fileLength);

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="objs">数据集</param>
        /// <param name="configFilePath">配置文件路径</param>
        /// <param name="templateFilePath">模板物理路径</param>
        /// <param name="exportDirectory">导出类型，对应配置属性ExportType</param>
        /// <param name="fileLength">导出文件大小</param>
        /// <returns></returns>
        string Export<T>(IList<T> objs, string configFilePath, string templateFilePath, string exportDirectory , out long fileLength);


        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="exportDirectory">导出文件夹路径</param>
        /// <returns>excel路径</returns>
        string Export(System.Data.DataSet ds, string exportDirectory);
    }
}
