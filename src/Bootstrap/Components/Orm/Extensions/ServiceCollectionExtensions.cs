using Bootstrap.Components.Orm.Infrastructures;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bootstrap.Components.Orm.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBootstrapServices<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext => AddBootstrapServices<TDbContext, TDbContext>(services);

        public static IServiceCollection AddBootstrapServices<TDbContext, TDbContextImplementation>(
            this IServiceCollection services) where TDbContext : DbContext
        {
            services.TryAddSingleton<BaseService<TDbContext>>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
    }
}