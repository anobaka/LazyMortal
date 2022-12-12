using System;

namespace Bootstrap.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetFirstDayOfMonth(this DateTime d) => new DateTime(d.Year, d.Month, 1);

        public static long ToTimestamp(this DateTime d) => (long) d.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public static long ToMillisecondTimestamp(this DateTime d) =>
            (long) d.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
}