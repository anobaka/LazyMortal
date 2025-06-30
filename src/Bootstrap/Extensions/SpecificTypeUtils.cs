using System;
using System.Reflection;

namespace Bootstrap.Extensions
{
    public class SpecificTypeUtils<T>
    {
        public static Type Type = typeof(T);

        private static PropertyInfo[]? _propertyInfos;

        public static PropertyInfo[] Properties => _propertyInfos ??= Type.GetProperties();
        public static TypeInfo TypeInfo => Type.GetCachedTypeInfo();

        public static bool IsTypeOfNullable => TypeInfo.IsTypeOfNullable();

        public static PropertyInfo IdProperty => Type.GetKeyProperty();
    }
}