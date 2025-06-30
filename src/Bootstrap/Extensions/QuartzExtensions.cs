using System.Collections.Generic;
using System.Linq;
using Quartz;

namespace Bootstrap.Extensions
{
    public static class QuartzExtensions
    {
        public static T? GetData<T>(this IJobExecutionContext context)
        {
            return context.MergedJobDataMap.Values.FirstOrDefault(t => t is T) is T ? (T?) context.MergedJobDataMap.Values.FirstOrDefault(t => t is T) : default;
        }

        public static IEnumerable<T> GetAllData<T>(this IJobExecutionContext context)
        {
            return context.MergedJobDataMap.Where(t => t.Value is T).Select(t => (T) t.Value);
        }

        public static T GetData<T>(this IJobExecutionContext context, string key)
        {
            return (T) context.MergedJobDataMap[key];
        }
    }
}