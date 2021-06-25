using System;
using System.Collections.Generic;

namespace Bootstrap.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
            Func<TValue> newValue)
        {
            if (!dict.TryGetValue(key, out var v))
            {
                dict[key] = v = newValue();
            }

            return v;
        }

        public static TValue GetValueSafely<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            var v = default(TValue);
            dict?.TryGetValue(key, out v);
            return v;
        }

        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other,
            bool @override)
        {
            foreach (var (k, v) in other)
            {
                if (@override || !dict.ContainsKey(k))
                {
                    dict[k] = v;
                }
            }
        }
    }
}