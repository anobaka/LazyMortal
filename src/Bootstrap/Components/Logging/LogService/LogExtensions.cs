using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Configuration.SystemProperty;
using Bootstrap.Components.Orm.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bootstrap.Components.Logging.LogService
{
    public static class LogExtensions
    {
        public static IServiceCollection AddBootstrapLogService<TRegisteredServiceImplementation>(
            this IServiceCollection services, Action<DbContextOptionsBuilder> configure = null)
            where TRegisteredServiceImplementation : LogService =>
            services.AddBootstrapLogService<TRegisteredServiceImplementation, LogDbContext>();

        public static IServiceCollection AddBootstrapLogService<TRegisteredServiceImplementation,
            TDbContextImplementation>(
            this IServiceCollection services, Action<DbContextOptionsBuilder> configure = null)
            where TRegisteredServiceImplementation : LogService where TDbContextImplementation : LogDbContext
        {
            services.TryAddSingleton<LogService>(sp => sp.GetRequiredService<TRegisteredServiceImplementation>());
            services.AddBootstrapServices<LogDbContext, TDbContextImplementation>(configure);
            return services;
        }
    }
}