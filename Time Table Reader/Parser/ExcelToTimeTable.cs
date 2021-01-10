﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace Time_Table_Generator
{
    public abstract class ExcelToTimeTable : IDisposable
    {
        public string FileLoc { get; set; }
        Workbook Workbook { get; set; }
        Worksheet Worksheet { get; set; }

        public void Dispose()
        {
            Marshal.ReleaseComObject(Worksheet);
            Workbook.Close();
            Marshal.ReleaseComObject(Workbook);
        }

        public void Load()
        {
            var app = new Application();
            Workbook = app.Workbooks.Open(FileLoc);

            foreach (Worksheet xlWorkSheet in Workbook.Sheets)
            {
                Worksheet = xlWorkSheet;
                break;
            }
        }

        public TimeTable GenerateTimeTable()
        {
            var mainList = new List<string[]>();
            foreach (Worksheet xlWorkSheet in Workbook.Sheets)
            {
                var list = ExtractRows(xlWorkSheet.UsedRange);
                if (list.Count > 0)
                    list.RemoveAt(0);
                if (list.Count > 0)
                    list.RemoveAt(0);
                mainList.AddRange(list);
            }
            var contents = ExtractRows(Worksheet.UsedRange);
            return DoGenerateTimeTable(mainList);
        }

        protected abstract TimeTable DoGenerateTimeTable(List<string[]> contents);

        private List<string[]> ExtractRows(Range range)
        {
            int TotalRows = range.Rows.Count;
            int TotalColumns = range.Columns.Count;
            object[,] array = range.Cells.Value;

            List<string[]> Content = new List<string[]>();

            for (int i = 1; i <= TotalRows; ++i)
            {
                var row = new List<string>();

                for (int j = 1; j <= TotalColumns; ++j)
                    row.Add(array[i, j]?.ToString() ?? "");

                Content.Add(row.ToArray());
            }
            return Content;
        }
    }
}