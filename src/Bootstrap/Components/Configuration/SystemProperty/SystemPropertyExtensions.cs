using System;
using Bootstrap.Components.Orm.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bootstrap.Components.Configuration.SystemProperty
{
    public static class SystemPropertyExtensions
    {
        public static SystemPropertyDto ToDto(this SystemProperty sp, SystemPropertyKeyAttribute properties = null,
            bool restartRequired = false)
        {
            if (sp == null)
            {
                return null;
            }

            return new SystemPropertyDto
            {
                Key = sp.Key,
                Value = sp.Value,
                Properties = properties,
                RestartRequired = restartRequired
            };
        }

        public static IServiceCollection AddBootstrapSystemPropertyService<TRegisteredServiceImplementation>(
            this IServiceCollection services, Action<DbContextOptionsBuilder> configure = null)
            where TRegisteredServiceImplementation : SystemPropertyService => services
            .AddBootstrapSystemPropertyService<TRegisteredServiceImplementation, SystemPropertyDbContext>(configure);

        public static IServiceCollection
            AddBootstrapSystemPropertyService<TRegisteredServiceImplementation, TDbContextImplementation>(
                this IServiceCollection services, Action<DbContextOptionsBuilder> configure = null)
            where TRegisteredServiceImplementation : SystemPropertyService
            where TDbContextImplementation : SystemPropertyDbContext
        {
            services.AddSingleton<SystemPropertyService, TRegisteredServiceImplementation>(sp =>
                sp.GetRequiredService<TRegisteredServiceImplementation>());
            services.AddBootstrapServices<SystemPropertyDbContext, TDbContextImplementation>(configure);
            return services;
        }
    }
}