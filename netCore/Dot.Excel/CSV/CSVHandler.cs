using Dot.IExcel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Dot.IOC;

namespace Dot.Excel.CSV
{
    [Export(typeof(IExcelHandler), Key = "CSV")]

    public class CSVHandler : IExcelHandler
    {
        public string Export(object obj, string configFilePath, string templateFilePath, string exportDirectory, string exportType, out long fileLength)
        {
            throw new NotImplementedException();
        }

        public string Export<T>(IList<T> objs, string configFilePath, string templateFilePath, string exportDirectory, out long fileLength)
        {
            throw new NotImplementedException();
        }

        public string Export(DataSet ds, string exportDirectory)
        {
            throw new NotImplementedException();
        }

        public DataSet Import(string path)
        {
            throw new NotImplementedException();
        }

        public DataTable Import(string strFileName, string sheetName, int headerRowIndex)
        {
            throw new NotImplementedException();
        }

        public DataTable Import(string strFileName, int sheetIndex, int headerRowIndex)
        {
            throw new NotImplementedException();
        }
    }
}
