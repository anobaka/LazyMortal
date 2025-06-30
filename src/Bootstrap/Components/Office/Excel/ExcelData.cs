using System.Collections.Generic;

namespace Bootstrap.Components.Office.Excel
{
    public class ExcelData
    {
        public ExcelData()
        {
        }

        /// <summary>
        /// Simple constructor.
        /// </summary>
        /// <param name="data">List of list of (span, content).</param>
        public ExcelData(IReadOnlyList<SimpleColumn[]> data)
        {
            Cells = new List<ExcelCell>();
            for (var i = 0; i < data.Count; i++)
            {
                var row = data[i];
                var colIndex = 0;
                foreach (var col in row)
                {
                    var c = new ExcelCell
                    {
                        Top = i, Bottom = i, Content = col.Content, Left = colIndex, Bold = col.Bold,
                        HorizontalAlignment = col.HorizontalAlignment
                    };
                    colIndex += col.Span;
                    c.Right = colIndex - 1;
                    Cells.Add(c);
                }
            }
        }

        public List<ExcelCell> Cells { get; set; } = [];

    }
}