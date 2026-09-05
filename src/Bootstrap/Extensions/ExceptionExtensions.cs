using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Extensions
{
    public static class ExceptionExtensions
    {
        [Obsolete("Use Exception.ToString() instead.")]
        public static string BuildFullInformationText(this Exception e) => e.ToString();

        [Obsolete("Use Exception.ToString() instead.")]
        public static string BuildAllMessages(this Exception e) => e.ToString();
    }
}