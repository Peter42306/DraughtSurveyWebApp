using ClosedXML.Excel;

namespace DraughtSurveyWebApp.Services
{
    public static class ExcelTemplateFiller
    {
        public static void FillExcelByName(XLWorkbook workbook, IDictionary<string, object?> data)
        {
            foreach (var (name, value) in data)
            {                
                
                var dn = workbook.DefinedName(name);

                if (dn == null || dn.Ranges.Count == 0)
                {
                    continue;
                }

                var cell = dn.Ranges.First().FirstCell();

                SetCellValue(cell, value);
            }
        }


        private static void SetCellValue(IXLCell cell, object? value)
        {
            if (value is null)
            {
                cell.Clear(XLClearOptions.Contents);
                return;                    
            }

            switch (value)
            {
                case string s:  cell.SetValue(s); break;
                case DateTime dt: cell.SetValue(dt); break;
                case bool b: cell.SetValue(b); break;
                case int i: cell.SetValue(i); break;
                case long l: cell.SetValue(l); break;
                case float f: cell.SetValue(f); break;
                case double d: cell.SetValue(d); break;
                case decimal c: cell.SetValue(c); break;
                default: cell.SetValue(value.ToString() ?? string.Empty); break;
            }
        }
    }
}
