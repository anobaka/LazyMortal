using System;

namespace Bootstrap.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetFirstDayOfMonth(this DateTime d) => new DateTime(d.Year, d.Month, 1);

        public static int ToTimestamp(this DateTime d) => (int) d.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public static int ToMillisecondTimestamp(this DateTime d) =>
            (int) d.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
}