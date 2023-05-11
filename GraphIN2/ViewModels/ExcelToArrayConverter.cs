using System;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;

namespace GraphIN2.ViewModels
{
    public class ExcelToArrayConverter
    {
        public static string[] ConvertColumnToArray(string filePath, int columnNumber)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkbook = null;
            Excel.Worksheet excelWorksheet = null;

            try
            {
                excelWorkbook = excelApp.Workbooks.Open(filePath);
                excelWorksheet = excelWorkbook.Sheets[1];

                Excel.Range usedRange = excelWorksheet.UsedRange;
                Excel.Range column = usedRange.Columns[columnNumber];

                if (column == null)
                {
                    return null;
                }

                object[,] values = column.Value;

                string[] array = new string[values.GetLength(0)];
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    array[i] = Convert.ToString(values[i + 1, 1]);
                }

                return array;
            }
            finally
            {
                if (excelWorksheet != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorksheet);
                }
                if (excelWorkbook != null)
                {
                    excelWorkbook.Close();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
                }
                if (excelApp != null)
                {
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                }
            }
        }
    }
}