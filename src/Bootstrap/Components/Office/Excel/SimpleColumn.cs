using NPOI.SS.UserModel;

namespace Bootstrap.Components.Office.Excel
{
    public class SimpleColumn
    {
        public int Span { get; set; }

        public string Content { get; set; }

        public bool Bold { get; set; }

        public HorizontalAlignment? HorizontalAlignment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="span">Default value is 1.</param>
        public SimpleColumn(string content, int span = 1)
        {
            Content = content;
            Span = span;
        }
    }
}
