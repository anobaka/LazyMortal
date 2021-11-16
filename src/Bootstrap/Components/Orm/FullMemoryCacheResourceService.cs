﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bootstrap.Components.Orm.Infrastructures;
using Bootstrap.Extensions;
using Bootstrap.Models.Constants;
using Bootstrap.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Orm
{
    public class FullMemoryCacheResourceService<TDbContext, TResource, TKey> : ServiceBase<TDbContext>
        where TDbContext : DbContext where TResource : class
    {
        protected ResourceService<TDbContext, TResource, TKey> ResourceService;
        protected ConcurrentDictionary<TKey, TResource> CacheVault;
        public FullMemoryCacheResourceService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ResourceService = new ResourceService<TDbContext, TResource, TKey>(serviceProvider);
            var data = ResourceService.GetAll(null, false, true).ConfigureAwait(false).GetAwaiter().GetResult()
                .ToDictionary(FuncExtensions.BuildKeySelector<TResource, TKey>(), t => t);
            CacheVault = new ConcurrentDictionary<TKey, TResource>(data);
        }

        public virtual TResource GetByKey(TKey key) => (CacheVault.TryGetValue(key, out var v) ? v : null).JsonCopy();

        public virtual List<TResource> GetByKeys(IEnumerable<TKey> keys) =>
            keys.Select(GetByKey).Where(v => v != null).ToList();

        public virtual TResource GetFirst(Expression<Func<TResource, bool>> selector,
            Expression<Func<TResource, object>> orderBy = null, bool asc = false)
        {
            var data = CacheVault.Values.Where(selector.Compile());
            if (orderBy != null)
            {
                var ob = orderBy.Compile();
                data = asc ? data.OrderBy(ob) : data.OrderByDescending(ob);
            }

            return data.FirstOrDefault().JsonCopy();
        }

        public virtual List<TResource> GetAll(Expression<Func<TResource, bool>> selector = null) =>
            (selector == null ? CacheVault.Values : CacheVault.Values.Where(selector.Compile())).ToList().JsonCopy();

        public virtual int Count(Func<TResource, bool> selector = null) =>
            selector == null ? CacheVault.Values.Count : CacheVault.Values.Count(selector);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orders">Key Selector - Asc</param>
        /// <returns></returns>
        public virtual SearchResponse<TResource> Search(Func<TResource, bool> selector,
            int pageIndex, int pageSize, List<(Func<TResource, object> SelectKey, bool Asc)> orders)
        {
            var resources = CacheVault.Values.ToList();
            if (selector != null)
            {
                resources = resources.Where(selector).ToList();
            }

            if (orders?.Any() == true)
            {
                var (fk, fa) = orders[0];
                var temp = fa ? resources.OrderBy(fk) : resources.OrderByDescending(fk);
                foreach (var (k, asc) in orders.Skip(1))
                {
                    temp = asc ? temp.ThenBy(k) : temp.ThenByDescending(k);
                }

                resources = temp.ToList();
            }

            var count = resources.Count;
            var data = resources.Skip(Math.Max(pageIndex - 1, 0) * pageSize).Take(pageSize).ToList();
            var result = new SearchResponse<TResource>(data, count, pageIndex, pageSize);
            return result.JsonCopy();
        }

        public virtual SearchResponse<TResource> Search(Func<TResource, bool> selector,
            int pageIndex, int pageSize, Func<TResource, object> orderBy = null, bool asc = false)
        {
            var resources = CacheVault.Values.ToList();
            if (selector != null)
            {
                resources = resources.Where(selector).ToList();
            }

            if (orderBy != null)
            {
                resources = (asc ? resources.OrderBy(orderBy) : resources.OrderByDescending(orderBy)).ToList();
            }

            var count = resources.Count;
            var data = resources.Skip(Math.Max(pageIndex - 1, 0) * pageSize).Take(pageSize).ToList();
            var result = new SearchResponse<TResource>(data, count, pageIndex, pageSize);
            return result.JsonCopy();
        }

        public virtual async Task<BaseResponse> Remove(TResource resource)
        {
            var rsp = await ResourceService.Remove(resource);
            if (rsp.Code == (int) ResponseCode.Success)
            {
                CacheVault.Remove(resource.GetKeyPropertyValue<TKey>(), out _);
            }

            return rsp;
        }

        public virtual async Task<BaseResponse> RemoveRange(IEnumerable<TResource> resources)
        {
            var rs = resources.ToList();
            var rsp = await ResourceService.RemoveRange(rs);
            if (rsp.Code == (int) ResponseCode.Success)
            {
                foreach (var r in rs)
                {
                    CacheVault.Remove(r.GetKeyPropertyValue<TKey>(), out _);
                }
            }

            return rsp;
        }


        public virtual Task<BaseResponse> RemoveAll(Expression<Func<TResource, bool>> selector)
        {
            var func = selector.Compile();
            var keys = CacheVault.Where(t => func(t.Value)).Select(a => a.Key);
            foreach (var k in keys)
            {
                CacheVault.Remove(k, out _);
            }

            return ResourceService.RemoveAll(selector);
        }

        public virtual Task<BaseResponse> RemoveByKey(TKey key)
        {
            CacheVault.Remove(key, out _);
            return ResourceService.RemoveByKey(key);
        }

        public virtual Task<BaseResponse> RemoveByKeys(IEnumerable<TKey> keys)
        {
            var ks = keys.ToList();
            ks.ForEach(k => RemoveByKey(k));
            return ResourceService.RemoveByKeys(ks);
        }

        public virtual async Task<SingletonResponse<TResource>> Add(TResource resource)
        {
            var rsp = await ResourceService.Add(resource);
            if (rsp.Data != null)
            {
                var d = rsp.Data;
                CacheVault[d.GetKeyPropertyValue<TKey>()] = d;
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }

        public virtual async Task<ListResponse<TResource>> AddRange(List<TResource> resources)
        {
            var rsp = await ResourceService.AddRange(resources);
            if (rsp.Data != null)
            {
                foreach (var d in rsp.Data)
                {
                    CacheVault[d.GetKeyPropertyValue<TKey>()] = d;
                }
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }

        public virtual async Task<SingletonResponse<TResource>> UpdateByKey(TKey key, Action<TResource> modify)
        {
            var rsp = await ResourceService.UpdateByKey(key, modify);
            if (rsp.Data != null)
            {
                CacheVault[rsp.Data.GetKeyPropertyValue<TKey>()] = rsp.Data;
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }

        public virtual async Task<BaseResponse> Update(TResource resource)
        {
            var rsp = await ResourceService.Update(resource);
            if (rsp.Code == (int)ResponseCode.Success)
            {
                CacheVault[resource.GetKeyPropertyValue<TKey>()] = resource;
            }

            return rsp;
        }

        public virtual async Task<BaseResponse> UpdateRange(IReadOnlyCollection<TResource> resources)
        {
            var rsp = await ResourceService.UpdateRange(resources);
            if (rsp.Code == (int)ResponseCode.Success)
            {
                foreach (var resource in resources)
                {
                    CacheVault[resource.GetKeyPropertyValue<TKey>()] = resource;
                }
            }

            return rsp;
        }

        public virtual async Task<ListResponse<TResource>> UpdateByKeys(IReadOnlyCollection<TKey> keys,
            Action<TResource> modify)
        {
            var rsp = await ResourceService.UpdateByKeys(keys, modify);
            if (rsp.Data != null)
            {
                foreach (var d in rsp.Data)
                {
                    CacheVault[d.GetKeyPropertyValue<TKey>()] = d;
                }
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }

        public virtual async Task<SingletonResponse<TResource>> UpdateFirst(Expression<Func<TResource, bool>> selector,
            Action<TResource> modify)
        {
            var rsp = await ResourceService.UpdateFirst(selector, modify);
            if (rsp.Data != null)
            {
                CacheVault[rsp.Data.GetKeyPropertyValue<TKey>()] = rsp.Data;
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }

        public virtual async Task<ListResponse<TResource>> UpdateAll(Expression<Func<TResource, bool>> selector,
            Action<TResource> modify)
        {
            var rsp = await ResourceService.UpdateAll(selector, modify);
            if (rsp.Data != null)
            {
                foreach (var d in rsp.Data)
                {
                    CacheVault[d.GetKeyPropertyValue<TKey>()] = d;
                }
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }
    }
}