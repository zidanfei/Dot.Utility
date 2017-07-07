
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Npoi.Core.HSSF.UserModel;
using Dot.Models;
using Dot.Excel.Model;
using System.Xml;
using System.Linq;
using Dot.Excel.Enum;

namespace Dot.Excel
{
    public partial class ExcelHelper
    {
        #region 从datatable中将数据导出到excel

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="obj">数据集</param>
        /// <param name="configFilePath">配置文件路径</param>
        /// <param name="templateFilePath">模板物理路径</param>
        /// <param name="exportType">导出类型，对应配置属性ExportType</param>
        /// <param name="fileLength">导出文件大小</param>
        /// <returns></returns>
        public static string ExportToExcel(object obj, string configFilePath, string templateFilePath, string exportDirectory, string exportType, out long fileLength)
        {
            List<ExcelCell> list = new List<ExcelCell>();

            XmlDocument doc = new XmlDocument();
            doc.Load(configFilePath);

            #region 读取配置

            //使用xpath表达式选择文档中所有的student子节点         
            XmlNodeList modelList = doc.SelectNodes("/Models/Class");
            if (null == modelList || null == obj)
                return Export(list, templateFilePath, exportDirectory, out fileLength);
            var props = obj.GetType().GetProperties().ToDictionary(p => p.Name);

            foreach (XmlNode model in modelList)
            {
                if (null == model.Attributes["ExportType"] || string.Format(",{0},", model.Attributes["ExportType"].Value).IndexOf(string.Format(",{0},", exportType)) < 0)
                    continue;
                //处理列表对象
                if (props.ContainsKey(model.Attributes["DataName"].Value))
                {
                    XmlNode sheetNameNode = model.SelectSingleNode("SheetName");
                    if (null == sheetNameNode)
                        throw new Exception("SheetName不能为空");

                    XmlNode columnsNode = model.SelectSingleNode("Columns");
                    if (null != columnsNode)
                    {
                        var prop = props[model.Attributes["DataName"].Value].GetValue(obj, null);
                        System.Collections.IList propList = null;
                        if (null != columnsNode.Attributes["DataName"] && !string.IsNullOrEmpty(columnsNode.Attributes["DataName"].Value))
                        {
                            propList = prop.GetType().GetProperty(columnsNode.Attributes["DataName"].Value).GetValue(prop, null) as System.Collections.IList;
                        }
                        else
                        {
                            propList = prop as System.Collections.IList;
                        }

                        #region 清除空对象

                        if (propList is System.Array)
                        {
                            System.Collections.IList templist = new System.Collections.ArrayList();
                            foreach (var item in propList)
                            {
                                if (item != null)
                                    templist.Add(item);
                            }
                            propList = templist;
                        }
                        else
                        {
                            for (int i = 0; i < propList.Count;)
                            {
                                if (propList.Contains(null))
                                {
                                    propList.Remove(null);
                                }
                                else
                                {
                                    i++;
                                }
                            }
                        }

                        #endregion

                        Log.LogFactory.WebBusinessLog.Debug(model.Attributes["DataName"].Value + " 导出数据总条数为" + propList.Count);
                        if (propList.Count > 0)
                        {
                            AddColumnNode(list, columnsNode, propList, sheetNameNode.InnerText);
                        }
                    }

                    #region 一般属性 单元格处理

                    XmlNode cellsNode = model.SelectSingleNode("Cells");
                    if (null != cellsNode)
                    {
                        XmlNodeList cellList = cellsNode.SelectNodes("Cell");
                        var prop = props[model.Attributes["DataName"].Value];
                        var propobj = prop.GetValue(obj, null);
                        if (null == propobj)
                            continue;
                        foreach (XmlNode c in cellList)
                        {
                            ExcelCell cell = new ExcelCell();
                            cell.SheetName = sheetNameNode.InnerText;
                            cell.ColumnIndex = Convert.ToInt32(c.Attributes["ColumnIndex"].Value);
                            cell.RowIndex = Convert.ToInt32(c.Attributes["RowIndex"].Value);
                            var value = propobj.GetType().GetProperty(c.Attributes["Name"].Value).GetValue(propobj, null);
                            if (value != null)
                            {
                                if (value.GetType() == typeof(decimal))
                                {
                                    string format = "0.00";
                                    if (null != c.Attributes["Format"] && !string.IsNullOrEmpty(c.Attributes["Format"].Value))
                                    {
                                        format = c.Attributes["Format"].Value;
                                    }
                                    cell.CellType = CellType.Numeric;
                                    cell.Value = Convert.ToDecimal(value).ToString(format);
                                }
                                else if (value.GetType() == typeof(int))
                                {
                                    cell.CellType = CellType.Numeric;
                                    cell.Value = value.ToString();
                                }
                                else if (value.GetType() == typeof(DateTime))
                                {
                                    string format = "yyyy/MM/dd";
                                    if (null != c.Attributes["Format"] && !string.IsNullOrEmpty(c.Attributes["Format"].Value))
                                    {
                                        format = c.Attributes["Format"].Value;
                                    }
                                    cell.CellType = CellType.String;
                                    cell.Value = Convert.ToDateTime(value).ToString(format);
                                }
                                else
                                {
                                    cell.Value = value.ToString();
                                }
                            }

                            list.Add(cell);
                        }
                    }
                    #endregion
                }
            }
            #endregion

            return Export(list, templateFilePath, exportDirectory, out fileLength);
        }


        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="objs">数据集</param>
        /// <param name="configFilePath">配置文件路径</param>
        /// <param name="templateFilePath">模板物理路径</param>
        /// <param name="exportType">导出类型，对应配置属性ExportType</param>
        /// <param name="fileLength">导出文件大小</param>
        /// <returns></returns>
        public static string ExportToExcel<T>(IList<T> objs, string configFilePath, string templateFilePath, string exportDirectory, out long fileLength)
        {
            List<ExcelCell> list = new List<ExcelCell>();

            XmlDocument doc = new XmlDocument();
            doc.Load(configFilePath);

            #region 读取配置
            //使用xpath表达式选择文档中所有的student子节点         
            XmlNode classNode = doc.SelectSingleNode("/Models/Class[@ClassName='" + typeof(T).Name + "']");
            if (null == classNode || null == objs)
                return Export(list, templateFilePath, exportDirectory, out fileLength);

            XmlNode sheetNameNode = classNode.SelectSingleNode("SheetName");
            if (null == sheetNameNode)
                throw new Exception("SheetName不能为空");

            XmlNode columnsNode = classNode.SelectSingleNode("Columns");
            if (null != columnsNode)
            {
                AddColumnNode(list, columnsNode, objs.ToArray(), sheetNameNode.InnerText);
            }
            #endregion

            return Export(list, templateFilePath, exportDirectory, out fileLength);
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="cellList">数据集</param>
        /// <param name="templateFilePath">模板物理路径</param>
        /// <param name="exportDirectory">导出文件夹路径</param>
        /// <param name="fileLength">导出文件大小</param>
        /// <returns>导出文件路径</returns>
        static string Export(List<ExcelCell> cellList, string templateFilePath, string exportDirectory, out long fileLength)
        {
            FileStream file = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read);
            HSSFWorkbook modelWorkBook = new HSSFWorkbook(file);

            foreach (var cell in cellList)
            {
                HSSFSheet sheet = (HSSFSheet)modelWorkBook.GetSheet(cell.SheetName);
                sheet.ForceFormulaRecalculation = true;
                if (null == sheet)
                    throw new Exception("不存在此工作簿");
                HSSFRow row = (HSSFRow)sheet.GetRow(cell.RowIndex);
                if (null == row)
                    row = (HSSFRow)sheet.CreateRow(cell.RowIndex);
                var hssfcell = row.GetCell(cell.ColumnIndex);
                if (null == hssfcell)
                    hssfcell = (HSSFCell)row.CreateCell(cell.ColumnIndex);
                if (null == hssfcell)
                    row.CreateCell(cell.ColumnIndex);

                if (cell.CellType == CellType.Numeric)
                {
                    hssfcell.SetCellValue(double.Parse(cell.Value));
                    //hssfcell.CellStyle = numericStyle;
                }
                else
                {
                    hssfcell.SetCellValue(cell.Value);
                }
            }
            if (!Directory.Exists(exportDirectory))
                Directory.CreateDirectory(exportDirectory);
            string exportFilePath = Path.Combine(exportDirectory, DateTime.Now.Ticks.ToString() + ".xls");
            FileStream exportStream = new FileStream(exportFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            modelWorkBook.Write(exportStream);
            file.Close();
            fileLength = exportStream.Length;
            exportStream.Close();
            return exportFilePath;
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="cellList">数据集</param>
        /// <param name="excelModelFilePath">模板物理路径</param>
        /// <param name="exportExcelDirectory">导出文件夹路径</param>
        /// <param name="fileLength">导出文件大小</param>
        /// <returns>导出文件路径</returns>
        static string Export(ExcelWorkbook book, out long fileLength)
        {

            FileStream file = new FileStream(book.ModelFilePath, FileMode.Open, FileAccess.Read);
            HSSFWorkbook modelWorkBook = new HSSFWorkbook(file);

            foreach (var sheetitem in book.SheetList)
            {
                HSSFSheet sheet = (HSSFSheet)modelWorkBook.GetSheet(sheetitem.SheetName);
                if (null == sheet && sheetitem.Create)
                    sheet = (HSSFSheet)modelWorkBook.CreateSheet(sheetitem.SheetName);
                if (null == sheet)
                    throw new Exception("不存在此工作簿");
                sheet.ForceFormulaRecalculation = true;
                foreach (var regionitem in sheetitem.RegionList)
                {
                    Npoi.Core.SS.Util.Region region = new Npoi.Core.SS.Util.Region(regionitem.RowFrom, regionitem.ColumnFrom, regionitem.RowTo, regionitem.ColumnTo);
                    //合并单元格
                    sheet.AddMergedRegion(region);
                }

                foreach (var cell in sheetitem.CellList)
                {
                    HSSFRow row = (HSSFRow)sheet.GetRow(cell.RowIndex);
                    if (null == row)
                        row = (HSSFRow)sheet.CreateRow(cell.RowIndex);

                    var hssfcell = row.GetCell(cell.ColumnIndex);
                    if (null == hssfcell)
                        hssfcell = (HSSFCell)row.CreateCell(cell.RowIndex);

                    if (cell.CellType == CellType.Numeric)
                    {
                        hssfcell.SetCellValue(double.Parse(cell.Value));
                    }
                    else
                    {
                        hssfcell.SetCellValue(cell.Value);
                    }
                }
            }

            //创建导出文件夹
            if (!Directory.Exists(book.ExportDirectory))
                Directory.CreateDirectory(book.ExportDirectory);
            string exportFilePath = Path.Combine(book.ExportDirectory, DateTime.Now.Ticks.ToString() + ".xls");
            FileStream exportStream = new FileStream(exportFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            modelWorkBook.Write(exportStream);
            file.Close();
            fileLength = exportStream.Length;
            exportStream.Close();
            return exportFilePath;
        }

        /// <summary>
        /// 添加列表数据
        /// </summary>
        /// <param name="list">结果数据集</param>
        /// <param name="columnsNode">Columns节点</param>
        /// <param name="propList">来源数据集</param>
        /// <param name="sheetName"></param>
        static void AddColumnNode(List<ExcelCell> list, XmlNode columnsNode, System.Collections.IList propList, string sheetName)
        {
            if (null == propList || 0 == propList.Count)
                return;
            XmlNodeList columnList = columnsNode.SelectNodes("Column");
            int startRow = Convert.ToInt32(columnsNode.Attributes["StartRow"].Value);

            int endRow = 0;
            if (null != columnsNode.Attributes["EndRow"])
            {
                endRow = Convert.ToInt32(columnsNode.Attributes["EndRow"].Value);
            }
            else
            {
                endRow = propList.Count + startRow;
            }
            int rowIndex = startRow;
            #region 添加年份信息

            XmlNode yearsNode = columnsNode.SelectSingleNode("Years");
            int startYear = 0;
            int endYear = 0;
            int startColumn = 0;
            string yearDataName = string.Empty;
            if (null != yearsNode)
            {
                startYear = Convert.ToInt32(yearsNode.Attributes["StartYear"].Value);
                endYear = Convert.ToInt32(yearsNode.Attributes["EndYear"].Value);
                startColumn = Convert.ToInt32(yearsNode.Attributes["StartColumn"].Value);
                yearDataName = yearsNode.Attributes["DataName"].Value;
            }

            #endregion

            #region 添加月份信息

            XmlNode monthsNode = columnsNode.SelectSingleNode("Months");
            string monthDataName = string.Empty;
            if (null != monthsNode)
            {
                startYear = Convert.ToInt32(monthsNode.Attributes["StartYear"].Value);
                endYear = Convert.ToInt32(monthsNode.Attributes["EndYear"].Value);
                startColumn = Convert.ToInt32(monthsNode.Attributes["StartColumn"].Value);
                monthDataName = monthsNode.Attributes["DataName"].Value;
            }
            #endregion


            foreach (var p in propList)
            {
                #region 普通列表属性

                foreach (XmlNode column in columnList)
                {
                    ExcelCell cell = new ExcelCell();
                    cell.SheetName = sheetName;
                    cell.ColumnIndex = Convert.ToInt32(column.InnerText);
                    cell.RowIndex = rowIndex;
                    var att = p.GetType().GetProperty(column.Attributes["Name"].Value);
                    if (null == p.GetType().GetProperty(column.Attributes["Name"].Value))
                    {
                        continue;
                    }
                    var value = att.GetValue(p, null);

                    if (null != value)
                    {
                        if (value.GetType() == typeof(decimal))
                        {
                            string format = "0.00";
                            if (null != column.Attributes["Format"] && !string.IsNullOrEmpty(column.Attributes["Format"].Value))
                            {
                                format = column.Attributes["Format"].Value;
                            }
                            cell.Value = Convert.ToDecimal(value).ToString(format);
                            cell.CellType = CellType.Numeric;
                        }
                        else if (value.GetType() == typeof(int))
                        {
                            cell.CellType = CellType.Numeric;
                            cell.Value = value.ToString();
                        }
                        else if (value.GetType() == typeof(DateTime))
                        {
                            string format = "yyyy/MM/dd";
                            if (null != column.Attributes["Format"] && !string.IsNullOrEmpty(column.Attributes["Format"].Value))
                            {
                                format = column.Attributes["Format"].Value;
                            }
                            cell.CellType = CellType.String;
                            cell.Value = Convert.ToDateTime(value).ToString(format);
                        }
                        else
                        {
                            cell.Value = value.ToString();
                        }
                    }
                    list.Add(cell);
                }

                #endregion

                #region 添加年份

                ///添加年份
                if (null != yearsNode)
                {
                    var yearList = p.GetType().GetProperty(yearDataName).GetValue(p, null) as System.Array;
                    Dictionary<int, string> yearRate = new Dictionary<int, string>();

                    Type type = null;
                    foreach (var rate in yearList)
                    {
                        yearRate.Add(Convert.ToInt32(rate.GetType().GetProperty(yearsNode.Attributes["Key"].Value).GetValue(rate, null)),
                             rate.GetType().GetProperty(yearsNode.Attributes["Value"].Value).GetValue(rate, null).ToString());
                        if (type == null && rate.GetType().GetProperty(yearsNode.Attributes["Value"].Value).GetValue(rate, null).GetType() == typeof(int))
                        {
                            type = typeof(int);
                        }
                        else if (type == null && rate.GetType().GetProperty(yearsNode.Attributes["Value"].Value).GetValue(rate, null).GetType() == typeof(decimal))
                        {
                            type = typeof(decimal);
                        }
                    }
                    for (int y = startYear; y <= endYear; y++)
                    {
                        ExcelCell cell = new ExcelCell();
                        cell.SheetName = sheetName;
                        cell.ColumnIndex = y - startYear + startColumn;
                        cell.RowIndex = rowIndex;
                        if (type == typeof(decimal) && yearRate.ContainsKey(y))
                        {
                            cell.Value = Convert.ToDecimal(yearRate[y]).ToString("###,##0.00");
                            cell.CellType = CellType.Numeric;
                        }
                        else if (yearRate.ContainsKey(y))
                        {

                            cell.Value = yearRate[y];
                        }

                        list.Add(cell);
                    }
                }

                #endregion

                #region 添加月份信息

                if (null != monthsNode)
                {
                    var monthList = p.GetType().GetProperty(monthDataName).GetValue(p, null) as System.Array;
                    Dictionary<string, string> monthRate = new Dictionary<string, string>();

                    Type type = null;
                    if (null != monthList)
                    {
                        foreach (var rate in monthList)
                        {
                            if (null == rate)
                                continue;
                            monthRate.Add(new Date(Convert.ToInt32(rate.GetType().GetProperty(monthsNode.Attributes["Year"].Value).GetValue(rate, null)),
                                Convert.ToInt32(rate.GetType().GetProperty(monthsNode.Attributes["Month"].Value).GetValue(rate, null)),
                                0).ToString(),
                                 rate.GetType().GetProperty(monthsNode.Attributes["Value"].Value).GetValue(rate, null).ToString());
                            if (type == null && rate.GetType().GetProperty(monthsNode.Attributes["Value"].Value).GetValue(rate, null).GetType() == typeof(int))
                            {
                                type = typeof(int);
                            }
                            else if (type == null && rate.GetType().GetProperty(monthsNode.Attributes["Value"].Value).GetValue(rate, null).GetType() == typeof(decimal))
                            {
                                type = typeof(decimal);
                            }
                        }
                        for (int y = startYear; y <= endYear; y++)
                        {
                            for (int m = 1; m < 13; m++)
                            {
                                if (monthRate.ContainsKey(new Date(y, m, 0).ToString()))
                                {
                                    ExcelCell cell = new ExcelCell();
                                    cell.SheetName = sheetName;
                                    cell.ColumnIndex = (y - startYear) * 12 + m - 1 + startColumn;
                                    cell.RowIndex = rowIndex;
                                    if (type == typeof(decimal))
                                    {
                                        cell.Value = Convert.ToDecimal(monthRate[new Date(y, m, 0).ToString()]).ToString("###,##0.00");
                                        cell.CellType = CellType.Numeric;
                                    }
                                    else
                                    {

                                        cell.Value = monthRate[new Date(y, m, 0).ToString()];
                                    }

                                    list.Add(cell);
                                }
                            }
                        }
                    }
                }

                #endregion
                rowIndex++;
            }
        }

        #endregion


    }
}