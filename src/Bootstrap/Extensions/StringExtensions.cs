using System;
using System.IO;

namespace Bootstrap.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);
        public static bool IsNotEmpty(this string s) => !s.IsNullOrEmpty();
        public static string ToNullIfEmpty(this string s) => s.IsNullOrEmpty() ? null : s;

        public static string TrimEnd(this string s, params string[] trimStrings)
        {
            if (trimStrings == null)
            {
                throw new ArgumentNullException(nameof(trimStrings));
            }

            while (true)
            {
                var @continue = false;
                foreach (var t in trimStrings)
                {
                    var l = s.Length;
                    if (l >= t.Length && s.Substring(l - t.Length) == t)
                    {
                        s = s.Substring(0, l - t.Length);
                        @continue = true;
                    }
                }

                if (!@continue)
                {
                    break;
                }
            }

            return s;
        }

        public static bool IsSimilarTo(this string a, string b, decimal threshold)
        {
            var dis = a.GetLevenshteinDistance(b);
            var maxLength = Math.Max(a.Length, b.Length);
            return (decimal)(maxLength - dis) / maxLength >= threshold;
        }

        public static string Format(this string s, params object[] args) => string.Format(s, args);

        public static string RemoveInvalidFileNameChars(this string fullname, string invalidCharReplacement = "_") =>
            fullname.IsNullOrEmpty()
                ? null
                : string.Join(invalidCharReplacement, fullname.Split(Path.GetInvalidFileNameChars()));
    }
}