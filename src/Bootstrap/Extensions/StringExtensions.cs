using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using NPOI.HPSF;

namespace Bootstrap.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty([CanBeNull] this string s) => string.IsNullOrEmpty(s);
        public static bool IsNotEmpty([CanBeNull] this string s) => !s.IsNullOrEmpty();
        public static string ToNullIfEmpty([CanBeNull] this string s) => s.IsNullOrEmpty() ? null : s;

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
            return (decimal) (maxLength - dis) / maxLength >= threshold;
        }

        public static string Format(this string s, params object[] args) => string.Format(s, args);

        public static string RemoveInvalidFileNameChars(this string filename, string invalidCharReplacement = "_")
        {
            if (filename.IsNullOrEmpty())
            {
                return null;
            }

            // invalidChars
            filename = string.Join(invalidCharReplacement, filename.Split(Path.GetInvalidFileNameChars()));

            return filename;
        }

        public static string RemoveInvalidFilePathChars(this string fullname, string invalidCharReplacement = "_")
        {
            if (fullname.IsNullOrEmpty())
            {
                return null;
            }

            fullname = string.Join(Path.DirectorySeparatorChar, fullname
                .Split(Path.PathSeparator, Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .Where(a => a.IsNotEmpty()).Select(
                    a =>
                    {
                        // Remove leading or trailing dots
                        while (a.Length > 0 && a[0] == '.')
                        {
                            a = a[1..];
                        }

                        while (a.Length > 0 && a[^1] == '.')
                        {
                            a = a[..^1];
                        }

                        // Remove other bad chars
                        return a.RemoveInvalidFileNameChars(invalidCharReplacement);
                    }));


            return fullname;
        }


        /// <summary>
        /// Returns true if <paramref name="path"/> starts with the path <paramref name="baseDirPath"/>.
        /// The comparison is case-insensitive, handles / and \ slashes as folder separators and
        /// only matches if the base dir folder name is matched exactly ("c:\foobar\file.txt" is not a sub path of "c:\foo").
        /// </summary>
        public static bool IsSubPathOf(this string path, string baseDirPath)
        {
            var normalizedPath = Path.GetFullPath(path.Replace('/', '\\').MakeEndWith("\\"));
            var normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', '\\').MakeEndWith("\\"));

            return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns <paramref name="str"/> with the minimal concatenation of <paramref name="ending"/> (starting from end) that
        /// results in satisfying .EndsWith(ending).
        /// </summary>
        /// <example>"hel".WithEnding("llo") returns "hello", which is the result of "hel" + "lo".</example>
        public static string MakeEndWith([CanBeNull] this string str, string ending)
        {
            if (str == null)
                return ending;

            // SubstringFromEnd() is 1-indexed, so include these cases
            // * Append no characters
            // * Append up to N characters, where N is ending length
            for (var i = 0; i <= ending.Length; i++)
            {
                var tmp = str + ending.SubstringFromEnd(i);
                if (tmp.EndsWith(ending))
                    return tmp;
            }

            return str;
        }

        /// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
        /// <param name="value">The string to retrieve the substring from.</param>
        /// <param name="length">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        public static string SubstringFromEnd([NotNull] this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, "Length is less than zero");
            }

            return length < value.Length ? value[^length..] : value;
        }

        public static string[] FindTopLevelPaths([NotNull] this string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            var uniquePaths = paths.Distinct().ToArray();
            var childrenPaths = new List<string>();
            foreach (var p in uniquePaths)
            {
                childrenPaths.AddRange(from p1 in uniquePaths where p != p1 && p.IsSubPathOf(p1) select p);
            }

            return uniquePaths.Except(childrenPaths).ToArray();
        }

        public static string StripInvalidXmlCharacters(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (var current in str.Where(current =>
                         (current == 0x9) || (current == 0xA) || (current == 0xD) ||
                         ((current >= 0x20) && (current <= 0xD7FF)) || ((current >= 0xE000) && (current <= 0xFFFD)) ||
                         ((current >= 0x10000) && (current <= 0x10FFFF))))
            {
                sb.Append(current);
            }

            return sb.ToString();
        }
    }
}