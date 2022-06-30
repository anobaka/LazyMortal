using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bootstrap.Components.Orm.Infrastructures
{
    public class ServiceBase<TDbContext> where TDbContext : DbContext
    {
        protected IServiceProvider ServiceProvider;
        protected IOptions<ServiceOptions> Options;
        protected ILogger Logger;

        protected ServiceBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
            Options = serviceProvider.GetRequiredService<IOptions<ServiceOptions>>();
        }

        #region Services

        public virtual TDbContext DbContext => GetRequiredService<TDbContext>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Resolved service from current request scope. Or from global scope if it doesn't exist.</returns>
        protected virtual T GetRequiredService<T>() => ServiceProvider.GetRequiredService<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Resolved service from current request scope. Or from global scope if it doesn't exist.</returns>
        protected virtual T GetRequiredService<T>(Type implementationType) where T : class
        {
            var instance = ServiceProvider.GetRequiredService(implementationType);
            return instance as T;
        }

        #endregion
    }
}