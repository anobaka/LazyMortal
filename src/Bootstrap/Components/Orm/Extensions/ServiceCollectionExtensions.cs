using System;
using System.Linq;
using Bootstrap.Components.Configuration.SystemProperty;
using Bootstrap.Components.Configuration.SystemProperty.Services;
using Bootstrap.Components.Orm.Infrastructures;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bootstrap.Components.Orm.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBootstrapServices<TDbContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> configure = null) where TDbContext : DbContext =>
            AddBootstrapServices<TDbContext, TDbContext>(services, configure);

        public static IServiceCollection AddBootstrapServices<TDbContext, TDbContextImplementation>(
            this IServiceCollection services, Action<DbContextOptionsBuilder> configure = null)
            where TDbContext : DbContext where TDbContextImplementation : TDbContext
        {
            services.TryAddScoped<BaseService<TDbContext>>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<GlobalCacheVault>();
            services.AddDbContext<TDbContext, TDbContextImplementation>(configure);
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceType"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceBootstrapServices<TDbContext>(
            this IServiceCollection services, Type serviceType, Action<DbContextOptionsBuilder> configure = null)
            where TDbContext : DbContext
        {
            return services.AddServiceBootstrapServices<TDbContext, TDbContext>(serviceType, configure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TDbContextImplementation"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceType"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceBootstrapServices<TDbContext, TDbContextImplementation>(
            this IServiceCollection services, Type serviceType, Action<DbContextOptionsBuilder> configure = null)
            where TDbContextImplementation : TDbContext where TDbContext : DbContext
        {
            services.TryAddSingleton(serviceType);
            services.AddBootstrapServices<TDbContext, TDbContextImplementation>(configure);
            return services;
        }
    }
}