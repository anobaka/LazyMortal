using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.XWPF.UserModel;

namespace Bootstrap.Components.Office.Word
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Do not use this. It's NOT fully tested.")]
    public class WordUtils
    {
        public static void ReplaceAll(XWPFDocument doc, Dictionary<string, string> replacements)
        {
            var allRuns = new List<XWPFRun>();

            foreach (var p in doc.Paragraphs)
            {
                allRuns.AddRange(p.Runs);
            }

            foreach (var t in doc.Tables)
            {
                foreach (var p in t.Rows.SelectMany(r => r.GetTableCells().SelectMany(c => c.Paragraphs)))
                {
                    allRuns.AddRange(p.Runs);
                }
            }

            foreach (var r in allRuns)
            {
                var text = r.GetText(0);
                text = replacements.Aggregate(text, (current, rp) => current.Replace(rp.Key, rp.Value));
                r.SetText(text);
            }
        }

        public static void AppendRowWithLastRowStyles(XWPFTable table, List<string> contents, bool overrideLastRow = true)
        {
            var lastRow = table.Rows.LastOrDefault();
            if (lastRow != null)
            {
                foreach (var content in contents)
                {
                    var nr = new XWPFTableRow(lastRow.GetCTRow(), table);
                    var nc = nr.GetCell(0);
                    var np = nc.Paragraphs.FirstOrDefault();
                    var nRun = np?.Runs.FirstOrDefault();
                    nRun?.SetText(content);
                }
            }
        }
    }
}