using System.IO;
using System.Linq;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Bootstrap.Components.Office.Excel
{
    public class ExcelUtils
    {
        public static Stream CreateExcel(ExcelData data)
        {
            var wb = new XSSFWorkbook();
            var sheet = wb.CreateSheet();

            var boldFont = wb.CreateFont();
            boldFont.FontHeightInPoints = 11;
            boldFont.FontName = "Calibri";
            boldFont.IsBold = true;

            foreach (var cell in data.Cells.OrderBy(t => t.Top).ThenBy(t => t.Left))
            {
                for (var i = cell.Top; i <= cell.Bottom; i++)
                {
                    var tr = sheet.GetRow(cell.Top) ?? sheet.CreateRow(cell.Top);
                    for (var j = cell.Left; j <= cell.Right; j++)
                    {
                        var tc = tr.GetCell(j) ?? tr.CreateCell(j);
                    }
                }

                var r = sheet.GetRow(cell.Top);
                var c = r.GetCell(cell.Left);
                sheet.AddMergedRegion(new CellRangeAddress(cell.Top, cell.Bottom, cell.Left, cell.Right));

                c.CellStyle = wb.CreateCellStyle();

                if (cell.Bold)
                {
                    c.CellStyle.SetFont(boldFont);
                }

                if (cell.HorizontalAlignment.HasValue)
                {
                    c.CellStyle.Alignment = cell.HorizontalAlignment.Value;
                }

                c.SetCellValue(cell.Content);
            }

            wb.SetForceFormulaRecalculation(true);

            var ms = new MemoryStream();
            wb.Write(ms, true);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}