using System;
using Bootstrap.Components.Orm.Extensions;
using Bootstrap.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bootstrap.Components.Logging.LogService.Extensions
{
    public static class LogServiceServiceCollectionExtensions
    {
        public static IServiceCollection AddBootstrapLogService<TDbContextImplementation>(
            this IServiceCollection services, Action<DbContextOptionsBuilder> configure = null)
            where TDbContextImplementation : LogDbContext
        {
            return services.AddServiceBootstrapServices<LogDbContext, TDbContextImplementation>(
                SpecificTypeUtils<Services.LogService>.Type, configure);
        }
    }
}