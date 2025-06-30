using System;
using System.Linq;
using JetBrains.Annotations;

namespace Bootstrap.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsDefined<T>(this T e) where T : Enum
        {
            return SpecificEnumUtils<T>.Values.Contains(e);
        }

        public static TAttribute? GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            return value.GetAttributes<TAttribute>().FirstOrDefault();
        }

        public static TAttribute[] GetAttributes<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value)!;
            return enumType.GetField(name)!.GetCustomAttributes(false).OfType<TAttribute>().ToArray();
        }

        public static string GetDisplayName<T>(this T @enum) where T : Enum
        {
            return SpecificEnumUtils<T>.DisplayNames[@enum];
        }
    }
}