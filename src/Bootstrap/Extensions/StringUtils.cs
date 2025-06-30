﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bootstrap.Extensions
{
    public static class StringUtils
    {
        private static readonly Random Random = new Random();

        public static string GetRandomNumber(int length)
        {
            return Random.Next(Convert.ToInt32(Math.Pow(10, length))).ToString($"D{length}");
        }

        public static string GenerateOrderNo(string? prefix = null)
        {
            return GenerateOrderNoList(1, prefix).First();
        }

        public static string[] GenerateOrderNoList(int count, string? prefix = null)
        {
            prefix = $"{prefix}{DateTime.Now:yyyyMMddHHmmssfff}";
            var suffixInt = GetRandomNumber(6);
            var result = new string[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = prefix + (suffixInt + i % 10E9);
            }

            return result;
        }

        public static bool IsPublicIpV4(string ip)
        {
            return Regex.IsMatch(ip,
                @"^([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(?<!172\.(16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31))(?<!127)(?<!^10)(?<!^0)\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(?<!192\.168)(?<!172\.(16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31))\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(?<!\.255$)$");
        }

        public static bool IsPrivateIpV4(string ip)
        {
            return Regex.IsMatch(ip,
                @"(^127\.)|(^10\.) |(^172\.1[6-9]\.)|(^172\.2[0-9]\.)|(^172\.3[0-1]\.)|(^192\.168\.)|(^::1)");
        }

        /// <summary>
        /// Support http(s) url only for now.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static string AddSchemaSafely(this string url, string schema = "https")
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.StartsWith("https:") && !url.StartsWith("http:"))
                {
                    var prefix = schema + ":";
                    if (!url.StartsWith("//"))
                    {
                        prefix += "//";
                    }

                    return prefix + url;
                }
            }

            return url;
        }

        public static string GetFilename(this string url)
        {
            var filename = url.Substring(url.LastIndexOf('/') + 1);
            if (filename.Contains('?'))
            {
                filename = filename.Substring(0, filename.IndexOf('?'));
            }

            return filename;
        }

        public static int GetLevenshteinDistance(this string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            {
                return a?.Length ?? b?.Length ?? 0;
            }

            a = a.ToLower();
            b = b.ToLower();

            if (a.Equals(b))
            {
                return 0;
            }

            var d = new int[a.Length + 1, b.Length + 1];

            for (var i = 0; i <= d.GetUpperBound(0); i += 1)
            {
                d[i, 0] = i;
            }

            for (var i = 0; i <= d.GetUpperBound(1); i += 1)
            {
                d[0, i] = i;
            }

            for (var i = 1; i <= d.GetUpperBound(0); i += 1)
            {
                for (var j = 1; j <= d.GetUpperBound(1); j += 1)
                {
                    var cost = Convert.ToInt32(a[i - 1] != b[j - 1]);

                    var min1 = d[i - 1, j] + 1;
                    var min2 = d[i, j - 1] + 1;
                    var min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }

        public static List<string> BeautifyTable(this List<List<string>> table, int spaceCountBetween = 2)
        {
            var outputs = new List<string>(new string[table.Count]);
            var spaceBetween = new string(Enumerable.Repeat(' ', spaceCountBetween).ToArray());
            for (var i = 0; i < int.MaxValue; i++)
            {
                if (table.All(t => t.Count <= i))
                {
                    break;
                }

                var maxLength = table.Select(t => t.Count > i ? t[i]?.Length ?? 0 : 0).Max();
                for (var j = 0; j < table.Count; j++)
                {
                    var r = table[j];
                    string? p = null;
                    if (r.Count > i)
                    {
                        p = r[i];
                    }

                    outputs[j] += p?.PadRight(maxLength) + spaceBetween;
                }
            }

            return outputs;
        }
    }
}