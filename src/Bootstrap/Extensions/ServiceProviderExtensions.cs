using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static T GetFirstRequiredService<T>(this IServiceProvider sp, Func<T, bool> where)
        {
            return sp.GetRequiredService<IEnumerable<T>>().FirstOrDefault(where);
        }
    }
}