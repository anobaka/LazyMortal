using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Bootstrap.Extensions
{
    public static class ObjectExtensions
    {
        internal static void SetKeyPropertyValue(this object instance, object keyValue) =>
            instance.GetType().GetKeyProperty().SetValue(instance, keyValue);

        internal static TValue GetKeyPropertyValue<TValue>(this object instance) =>
            (TValue) instance.GetType().GetKeyProperty().GetValue(instance)!;

        internal static object GetKeyPropertyValue(this object instance) => GetKeyPropertyValue<object>(instance);

        public static T? JsonCopy<T>(this T t) =>
            t == null ? default : JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(t));

        public static bool IsNull(this object? t) => t == null;

        public static string ToJson(this object? obj)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj);
        }

        #region Fast Clone for Records

        private static readonly ConcurrentDictionary<Type, Delegate?> CloneFuncCache = new();

        /// <summary>
        /// Fast clone for record types using their copy constructor.
        /// Falls back to JsonCopy for non-record types.
        /// </summary>
        public static T? FastClone<T>(this T? t) where T : class
        {
            if (t == null) return null;

            var cloneFunc = CloneFuncCache.GetOrAdd(typeof(T), type =>
            {
                // Try to find record copy constructor (takes same type as parameter)
                var copyConstructor = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    [type],
                    null);

                if (copyConstructor != null)
                {
                    // Build expression: (T source) => new T(source)
                    var param = Expression.Parameter(type, "source");
                    var newExpr = Expression.New(copyConstructor, param);
                    var lambda = Expression.Lambda<Func<T, T>>(newExpr, param);
                    return lambda.Compile();
                }

                return null;
            });

            if (cloneFunc is Func<T, T> typedFunc)
            {
                return typedFunc(t);
            }

            // Fallback to JsonCopy
            return t.JsonCopy();
        }

        /// <summary>
        /// Fast clone for a list of records.
        /// </summary>
        public static List<T>? FastClone<T>(this List<T>? list) where T : class
        {
            if (list == null) return null;
            if (list.Count == 0) return new List<T>();

            var result = new List<T>(list.Count);
            foreach (var item in list)
            {
                result.Add(item.FastClone()!);
            }
            return result;
        }

        /// <summary>
        /// Fast clone for an array of records.
        /// </summary>
        public static T[]? FastClone<T>(this T[]? array) where T : class
        {
            if (array == null) return null;
            if (array.Length == 0) return Array.Empty<T>();

            var result = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].FastClone()!;
            }
            return result;
        }

        #endregion
    }
}