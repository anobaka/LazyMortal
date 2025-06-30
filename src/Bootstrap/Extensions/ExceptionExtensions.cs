using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Extensions
{
    public static class ExceptionExtensions
    {
        private static IEnumerable<string> _getExceptionInformationRecursively(Exception? e, int level = 0)
        {
            if (e != null)
            {
                var info = new List<string>
                {
                    $"{(level == 0 ? "message" : "inner1 message")}: {e.Message}",
                    $"{(level == 0 ? "stacktrace" : "inner1 stacktrace")}: {e.StackTrace}",
                };
                if (e.InnerException != null)
                {
                    info.AddRange(_getExceptionInformationRecursively(e.InnerException, ++level));
                }

                return info;
            }

            return [];
        }

        public static string BuildFullInformationText(this Exception e)
        {
            return string.Join(Environment.NewLine, _getExceptionInformationRecursively(e));
        }

        public static string BuildAllMessages(this Exception e)
        {
            var sb = new StringBuilder();
            sb.AppendLine(e.Message);
            if (e.InnerException != null)
            {
                sb.AppendLine($"->{BuildAllMessages(e.InnerException)}");
            }

            return sb.ToString();
        }
    }
}