using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bootstrap.Extensions
{
    public static class CsvUtils
    {
        /// <summary>
        /// Turn a string into a CSV cell output
        /// </summary>
        /// <param name="rawCellValue">String to output</param>
        /// <returns>The CSV cell formatted string</returns>
        public static string Escape(string rawCellValue)
        {
            bool mustQuote = (rawCellValue.Contains(",") || rawCellValue.Contains("\"") ||
                              rawCellValue.Contains("\r") || rawCellValue.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in rawCellValue)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }

                sb.Append("\"");
                return sb.ToString();
            }

            return rawCellValue;
        }

        public static string[][] ReadAllLines(Stream csv)
        {
            using var csvParser = new TextFieldParser(csv);
            csvParser.CommentTokens = new[] { "#" };
            csvParser.SetDelimiters(",");
            csvParser.HasFieldsEnclosedInQuotes = true;

            var lines = new List<string[]>();
            while (!csvParser.EndOfData)
            {
                // Read current line fields, pointer moves to the next line.
                var fields = csvParser.ReadFields()!;
                lines.Add(fields);
            }

            return lines.ToArray();
        }
    }
}