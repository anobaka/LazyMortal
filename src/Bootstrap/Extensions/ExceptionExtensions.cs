using System;
using System.Collections.Generic;

namespace Bootstrap.Extensions
{
    public static class ExceptionExtensions
    {
        private static IEnumerable<string> _getExceptionInformationRecursively(Exception e, int level = 0)
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

            return default;
        }

        public static string BuildFullInformationText(this Exception e)
        {
            return string.Join(Environment.NewLine, _getExceptionInformationRecursively(e));
        }
    }
}