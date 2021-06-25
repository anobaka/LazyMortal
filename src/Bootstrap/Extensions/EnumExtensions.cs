using System;
using System.Linq;

namespace Bootstrap.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsDefined<T>(this T e) where T : Enum
        {
            return SpecificEnumUtils<T>.Values.Contains(e);
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }

        public static string GetDisplayName<T>(this T @enum) where T : Enum
        {
            return SpecificEnumUtils<T>.DisplayNames[@enum];
        }
    }
}