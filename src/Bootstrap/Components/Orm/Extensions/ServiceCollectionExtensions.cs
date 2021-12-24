using System;
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
            services.TryAddSingleton<BaseService<TDbContext>>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<TDbContext, TDbContextImplementation>(configure);
            return services;
        }
    }
}