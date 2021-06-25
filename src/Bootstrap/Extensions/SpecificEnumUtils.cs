using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Bootstrap.Extensions
{
    public static class SpecificEnumUtils<TEnum> where TEnum : Enum
    {
        public static readonly List<TEnum>
            Values = Enum.GetValues(SpecificTypeUtils<TEnum>.Type).Cast<TEnum>().ToList();

        private static Dictionary<TEnum, string> _displayNames;

        public static Dictionary<TEnum, string> DisplayNames =>
            _displayNames ??=
                Values.ToDictionary(t => t, t => t.GetAttribute<DisplayAttribute>()?.Name ?? t.ToString());

        public static bool ParseByDisplayName(string displayName, out TEnum e)
        {
            if (DisplayNames.ContainsValue(displayName))
            {
                e = DisplayNames.FirstOrDefault(t => t.Value == displayName).Key;
                return true;
            }

            e = default;
            return false;
        }
    }
}