using System.Collections.Generic;
using System.Linq;

namespace Bootstrap.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e) => e == null || !e.Any();
        public static bool IsNotEmpty<T>(this IEnumerable<T> e) => e?.Any() == true;
    }
}
