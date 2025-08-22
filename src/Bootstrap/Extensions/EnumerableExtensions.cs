using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Bootstrap.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? e) => e == null || !e.Any();
        public static bool IsNotEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? e) => e?.Any() == true;
    }
}
