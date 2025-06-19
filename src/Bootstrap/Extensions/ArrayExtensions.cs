using System.Collections.Generic;
using System.Linq;

namespace Bootstrap.Extensions;

public static class ArrayExtensions
{
    public static IEnumerable<IEnumerable<T>> GetAllCombinations<T>(this T[] array)
    {
        var count = 1 << array.Length; // 2^n combinations
        for (var i = 0; i < count; i++)
        {
            var combination = array.Where((t, j) => (i & (1 << j)) != 0).ToList();
            yield return combination;
        }
    }
}