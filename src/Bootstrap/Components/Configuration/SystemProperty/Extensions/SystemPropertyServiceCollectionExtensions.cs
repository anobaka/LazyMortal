using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Configuration.SystemProperty.Services;
using Bootstrap.Components.Orm.Extensions;
using Bootstrap.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Components.Configuration.SystemProperty.Extensions
{
    public static class SystemPropertyServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDbContextImplementation"></typeparam>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddBootstrapSystemPropertyService<TDbContextImplementation>(
            this IServiceCollection services, Action<DbContextOptionsBuilder> configure = null)
            where TDbContextImplementation : SystemPropertyDbContext
        {
            return services.AddSingleServiceBootstrapServices<SystemPropertyDbContext, TDbContextImplementation>(
                SpecificTypeUtils<SystemPropertyService>.Type, configure);
        }
    }
}