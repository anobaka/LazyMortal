﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Components.Orm.Extensions;
using Bootstrap.Extensions;
using Bootstrap.Models.Constants;
using Bootstrap.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Orm.Infrastructures
{
    public class BaseService<TDbContext> : ServiceBase<TDbContext> where TDbContext : DbContext
    {
        public BaseService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        #region IdRelatedOperations

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<TResource?> GetByKey<TResource>(object key)
            where TResource : class
        {
            var exp = ExpressionExtensions.BuildKeyEqualsExpression<TResource>(key);
            return await GetFirst(exp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual async Task<List<TResource>> GetByKeys<TResource>(IEnumerable<object> keys)
            where TResource : class
        {
            var exp = ExpressionExtensions.BuildKeyContainsExpression<TResource>(keys.Distinct().ToList());
            var resources =
                (await GetAll(exp)).ToDictionary(FuncExtensions.BuildKeySelector<TResource>(), t => t);
            return resources.Values.ToList();
        }

        // todo: test
        public virtual async Task<BaseResponse> Remove<TResource>(TResource resource)
        {
            var ctx = DbContext;
            ctx.Entry(resource!).State = EntityState.Deleted;
            await ctx.SaveChangesAsync();
            return BaseResponseBuilder.Ok;
        }

        // todo: test
        public virtual async Task<BaseResponse> RemoveRange<TResource>(IEnumerable<TResource> resources)
        {
            var ctx = DbContext;
            foreach (var r in resources)
            {
                ctx.Entry(r!).State = EntityState.Deleted;
            }

            await ctx.SaveChangesAsync();
            return BaseResponseBuilder.Ok;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<BaseResponse> RemoveByKey<TResource>(object key) where TResource : class
        {
            var exp = ExpressionExtensions.BuildKeyEqualsExpression<TResource>(key);
            var rsp = await RemoveAll(exp);
            return rsp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual async Task<BaseResponse> RemoveByKeys<TResource>(IEnumerable<object> keys)
            where TResource : class
        {
            var ks = keys.ToList();
            var exp = ExpressionExtensions.BuildKeyContainsExpression<TResource>(ks);
            var rsp = await RemoveAll(exp);
            return rsp;
        }

        public virtual async Task<SingletonResponse<TResource>> UpdateByKey<TResource>(object key,
            Action<TResource> modify)
            where TResource : class
        {
            var r = await GetByKey<TResource>(key);
            if (r != null)
            {
                var ctx = DbContext;
                if (ctx.Entry(r).State == EntityState.Detached)
                {
                    ctx.Attach(r);
                }

                modify(r);
                await ctx.SaveChangesAsync();

                ctx.Detach(r);

                return new SingletonResponse<TResource>(r);
            }

            return SingletonResponseBuilder<TResource>.NotFound;
        }

        public virtual async Task<ListResponse<TResource>> UpdateByKeys<TResource>(IReadOnlyCollection<object> keys,
            Action<TResource> modify)
            where TResource : class
        {
            var rs = await GetByKeys<TResource>(keys);
            var ctx = DbContext;
            foreach (var r in rs)
            {
                if (ctx.Entry(r).State == EntityState.Detached)
                {
                    ctx.Attach(r);
                }

                modify(r);
            }

            await ctx.SaveChangesAsync();

            ctx.DetachAll(rs);

            return new ListResponse<TResource>(rs);
        }

        #endregion

        #region IdUnrelatedOperations

        /// <summary>
        /// 获取第一条
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="selector"></param>
        /// <param name="orderBy"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public virtual async Task<TResource?> GetFirst<TResource>(Expression<Func<TResource, bool>>? selector,
            Expression<Func<TResource, object>>? orderBy = null, bool asc = false)
            where TResource : class
        {
            IQueryable<TResource> query = DbContext.Set<TResource>();
            if (selector != null)
            {
                query = query.Where(selector);
            }

            if (orderBy != null)
            {
                query = asc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            var result = await query.AsNoTracking().FirstOrDefaultAsync();
            return result;
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="selector">Null for getting all resources.</param>
        /// <returns></returns>
        public virtual async Task<List<TResource>> GetAll<TResource>(Expression<Func<TResource, bool>>? selector)
            where TResource : class
        {
            IQueryable<TResource> query = DbContext.Set<TResource>();
            if (selector != null)
            {
                query = query.Where(selector);
            }

            var result = await query.AsNoTracking().ToListAsync();
            return result;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="selector"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="asc"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public virtual async Task<SearchResponse<TResource>> Search<TResource>(
            Expression<Func<TResource, bool>>? selector, int pageIndex, int pageSize,
            Expression<Func<TResource, object>>? orderBy = null, bool asc = false,
            Expression<Func<TResource, object>>? include = null)
            where TResource : class
        {
            IQueryable<TResource> query = DbContext.Set<TResource>();
            if (selector != null)
            {
                query = include == null ? query.Where(selector) : query.Include(include).Where(selector);
            }

            if (orderBy != null)
            {
                query = asc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            var count = await query.CountAsync();
            var data = await query.Skip(Math.Max(pageIndex - 1, 0) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
            var result = new SearchResponse<TResource>(data, count, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual async Task<BaseResponse> RemoveAll<TResource>(Expression<Func<TResource, bool>> selector)
            where TResource : class
        {
            var ctx = DbContext;
            ctx.RemoveRange(ctx.Set<TResource>().Where(selector));
            return BaseResponseBuilder.Build(await ctx.SaveChangesAsync() > 0
                ? ResponseCode.Success
                : ResponseCode.NotModified);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resource"></param>
        /// <returns></returns>
        public virtual async Task<SingletonResponse<TResource>> Add<TResource>(TResource resource)
            where TResource : class
        {
            var ctx = DbContext;
            ctx.Add(resource);
            await ctx.SaveChangesAsync();
            ctx.Detach(resource);
            return new SingletonResponse<TResource>(resource);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resources"></param>
        /// <returns></returns>
        public virtual async Task<ListResponse<TResource>> AddRange<TResource>(IEnumerable<TResource> resources)
            where TResource : class
        {
            var data = resources.ToList();
            var ctx = DbContext;
            await ctx.AddRangeAsync(data);
            await ctx.SaveChangesAsync();
            ctx.DetachAll(data);
            return new ListResponse<TResource>(data);
        }

        public virtual async Task<int> Count<TResource>(Expression<Func<TResource, bool>>? selector)
            where TResource : class
        {
            return selector == null
                ? await DbContext.Set<TResource>().CountAsync()
                : await DbContext.Set<TResource>().CountAsync(selector);
        }

        public virtual async Task<bool> Any<TResource>(Expression<Func<TResource, bool>> selector)
            where TResource : class
        {
            return await DbContext.Set<TResource>().AnyAsync(selector);
        }

        public virtual async Task<BaseResponse> Update<TResource>(TResource resource)
        {
            var ctx = DbContext;
            ctx.Entry(resource!).State = EntityState.Modified;
            await ctx.SaveChangesAsync();
            ctx.Detach(resource);
            return BaseResponseBuilder.Ok;
        }

        public virtual async Task<BaseResponse> UpdateRange<TResource>(IEnumerable<TResource> resources)
            where TResource : class
        {
            var array = resources as TResource[] ?? resources.ToArray();
            var rs = array.ToList();
            var ks = FuncExtensions.BuildKeySelector<TResource>();
            var keys = rs.Select(r => ks(r)).ToList();
            var ctx = DbContext;
            var locals = ctx.Set<TResource>().Local.Where(t => keys.Contains(ks(t))).ToList();
            locals.ForEach(t => { ctx.Entry(t).State = EntityState.Detached; });
            foreach (var r in rs)
            {
                ctx.Entry(r).State = EntityState.Modified;
            }

            await ctx.SaveChangesAsync();
            ctx.DetachAll(array);
            return BaseResponseBuilder.Ok;
        }

        public virtual async Task<SingletonResponse<TResource>> UpdateFirst<TResource>(
            Expression<Func<TResource, bool>> selector,
            Action<TResource> modify)
            where TResource : class
        {
            var ctx = DbContext;
            var r = (await GetAll(selector)).FirstOrDefault();
            if (r == null)
            {
                return SingletonResponseBuilder<TResource>.NotFound;
            }

            if (ctx.Entry(r).State == EntityState.Detached)
            {
                ctx.Attach(r);
            }

            modify(r);
            await ctx.SaveChangesAsync();
            ctx.Detach(r);
            return new SingletonResponse<TResource>(r);
        }

        public virtual async Task<ListResponse<TResource>> UpdateAll<TResource>(
            Expression<Func<TResource, bool>> selector,
            Action<TResource> modify)
            where TResource : class
        {
            var ctx = DbContext;
            var rs = await GetAll(selector);
            foreach (var r in rs)
            {
                if (ctx.Entry(r).State == EntityState.Detached)
                {
                    ctx.Attach(r);
                }

                modify(r);

            }

            await ctx.SaveChangesAsync();
            ctx.DetachAll(rs);
            return new ListResponse<TResource>(rs);
        }

        #endregion
    }
}