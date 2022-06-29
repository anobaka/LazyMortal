using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Components.Orm.Abstractions
{
    /// <summary>
    /// todo: 
    /// </summary>
    internal interface IResourceService<out TDbContext, TResource, in TKey>
        where TDbContext : DbContext where TResource : class
    {
        IServiceProvider ServiceProvider { get; }
        TDbContext DbContext => ServiceProvider.GetRequiredService<TDbContext>();

        Task<TResource> GetByKey(TKey key)
        {
            var exp = ExpressionExtensions.BuildKeyEqualsExpression<TResource>(key);
            return DbContext.Set<TResource>().FirstOrDefaultAsync(exp);
        }

        Task<TResource[]> GetByKeys(IEnumerable<TKey> keys)
        {
            var exp = ExpressionExtensions.BuildKeyContainsExpression<TResource>(keys.Distinct().Cast<object>()
                .ToArray());
            return GetAll(exp);
        }

        Task<TResource[]> GetAll(Expression<Func<TResource, bool>> selector,
            bool asNoTracking = false)
        {
            IQueryable<TResource> query = DbContext.Set<TResource>();
            if (selector != null)
            {
                query = query.Where(selector);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.ToArrayAsync();
        }
    }
}