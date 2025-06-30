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
    }
}