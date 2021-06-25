using NPOI.SS.UserModel;

namespace Bootstrap.Components.Office.Excel
{
    public class ExcelCell
    {
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }
        public string Content { get; set; }
        public bool Bold { get; set; }
        public HorizontalAlignment? HorizontalAlignment { get; set; }
    }
}
