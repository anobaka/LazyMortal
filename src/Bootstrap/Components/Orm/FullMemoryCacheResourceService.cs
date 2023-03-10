using System;
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
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Components.Orm
{
    public class FullMemoryCacheResourceService<TDbContext, TResource, TKey> : ServiceBase<TDbContext>
        where TDbContext : DbContext where TResource : class
    {
        protected ResourceService<TDbContext, TResource, TKey> ResourceService;
        private readonly GlobalCacheVault _cacheVault;

        protected async Task<ConcurrentDictionary<TKey, TResource>> GetCacheVault()
        {
            var key = SpecificTypeUtils<TResource>.Type.FullName!;
            if (!_cacheVault.TryGetValue(key, out var vault))
            {
                var @lock = await _cacheVault.RequestLock(key);
                // ignore the other concurrent requests
                if (!_cacheVault.TryGetValue(key, out vault))
                {
                    var data = await ResourceService.GetAll(null, true);
                    _cacheVault[key] = vault =
                        new ConcurrentDictionary<TKey, TResource>(
                            data.ToDictionary(FuncExtensions.BuildKeySelector<TResource, TKey>(), t => t));
                }

                @lock.Release();
            }

            return vault as ConcurrentDictionary<TKey, TResource>;
        }

        public FullMemoryCacheResourceService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ResourceService = new ResourceService<TDbContext, TResource, TKey>(serviceProvider);
            _cacheVault = serviceProvider.GetRequiredService<GlobalCacheVault>();
        }

        public virtual async Task<TResource> GetByKey(TKey key, bool returnCopy = true)
        {
            var data = (await GetCacheVault()).TryGetValue(key, out var v) ? v : null;
            if (returnCopy)
            {
                data = data.JsonCopy();
            }

            return data;
        }

        public virtual async Task<TResource[]> GetByKeys(IEnumerable<TKey> keys, bool returnCopy = true)
        {
            var cache = await GetCacheVault();
            var data = keys.Select(k => cache.TryGetValue(k, out var v) ? v : null).Where(v => v != null)
                .ToArray();
            if (returnCopy)
            {
                data = data.JsonCopy();
            }

            return data;
        }

        public virtual async Task<TResource> GetFirst(Expression<Func<TResource, bool>> selector,
            Expression<Func<TResource, object>> orderBy = null, bool asc = false, bool returnCopy = true)
        {
            var list = (await GetCacheVault()).Values.Where(selector.Compile());
            if (orderBy != null)
            {
                var ob = orderBy.Compile();
                list = asc ? list.OrderBy(ob) : list.OrderByDescending(ob);
            }

            var data = list.FirstOrDefault();
            if (returnCopy)
            {
                data = data.JsonCopy();
            }

            return data;
        }

        public virtual async Task<List<TResource>> GetAll(Expression<Func<TResource, bool>> selector = null, bool returnCopy = true)
        {
            var data = (selector == null
                    ? (await GetCacheVault()).Values
                    : (await GetCacheVault()).Values.Where(selector.Compile()))
                .ToList();
            if (returnCopy)
            {
                data = data.JsonCopy();
            }

            return data;
        }

        public virtual async Task<int> Count(Func<TResource, bool> selector = null) => selector == null
            ? (await GetCacheVault()).Values.Count
            : (await GetCacheVault()).Values.Count(selector);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orders">Key Selector - Asc</param>
        /// <param name="returnCopy"></param>
        /// <returns></returns>
        public virtual async Task<SearchResponse<TResource>> Search(Func<TResource, bool> selector,
            int pageIndex, int pageSize, (Func<TResource, object> SelectKey, bool Asc)[] orders,
            bool returnCopy = true)
        {
            var cache = await GetCacheVault();
            var resources = cache.Values.ToList();
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
            if (returnCopy)
            {
                data = data.JsonCopy();
            }

            var result = new SearchResponse<TResource>(data, count, pageIndex, pageSize);
            return result;
        }

        public virtual Task<SearchResponse<TResource>> Search(Func<TResource, bool> selector,
            int pageIndex, int pageSize, Func<TResource, object> orderBy = null, bool asc = false, bool returnCopy = true)
        {
            var orders = orderBy == null ? null : new[] {(orderBy, asc)};
            var r = Search(selector, pageIndex, pageSize, orders, returnCopy);
            return r;
        }

        public virtual async Task<BaseResponse> Remove(TResource resource)
        {
            var rsp = await ResourceService.Remove(resource);
            if (rsp.Code == (int) ResponseCode.Success)
            {
                (await GetCacheVault()).Remove(resource.GetKeyPropertyValue<TKey>(), out _);
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
                    (await GetCacheVault()).Remove(r.GetKeyPropertyValue<TKey>(), out _);
                }
            }

            return rsp;
        }


        public virtual async Task<BaseResponse> RemoveAll(Expression<Func<TResource, bool>> selector)
        {
            var func = selector.Compile();
            var keys = (await GetCacheVault()).Where(t => func(t.Value)).Select(a => a.Key);
            foreach (var k in keys)
            {
                (await GetCacheVault()).Remove(k, out _);
            }

            return await ResourceService.RemoveAll(selector);
        }

        public virtual async Task<BaseResponse> RemoveByKey(TKey key)
        {
            (await GetCacheVault()).Remove(key, out _);
            return await ResourceService.RemoveByKey(key);
        }

        public virtual async Task<BaseResponse> RemoveByKeys(IEnumerable<TKey> keys)
        {
            var ks = keys.ToList();
            var cache = await GetCacheVault();
            ks.ForEach(k => cache.Remove(k, out _));
            return await ResourceService.RemoveByKeys(ks);
        }

        public virtual async Task<SingletonResponse<TResource>> Add(TResource resource)
        {
            var rsp = await ResourceService.Add(resource);
            if (rsp.Data != null)
            {
                var d = rsp.Data;
                (await GetCacheVault())[d.GetKeyPropertyValue<TKey>()] = d;
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
                    (await GetCacheVault())[d.GetKeyPropertyValue<TKey>()] = d;
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
                (await GetCacheVault())[rsp.Data.GetKeyPropertyValue<TKey>()] = rsp.Data;
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }

        public virtual async Task<BaseResponse> Update(TResource resource)
        {
            var rsp = await ResourceService.Update(resource);
            if (rsp.Code == (int) ResponseCode.Success)
            {
                (await GetCacheVault())[resource.GetKeyPropertyValue<TKey>()] = resource;
            }

            return rsp;
        }

        public virtual async Task<BaseResponse> UpdateRange(IReadOnlyCollection<TResource> resources)
        {
            var rsp = await ResourceService.UpdateRange(resources);
            if (rsp.Code == (int) ResponseCode.Success)
            {
                foreach (var resource in resources)
                {
                    (await GetCacheVault())[resource.GetKeyPropertyValue<TKey>()] = resource;
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
                    (await GetCacheVault())[d.GetKeyPropertyValue<TKey>()] = d;
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
                (await GetCacheVault())[rsp.Data.GetKeyPropertyValue<TKey>()] = rsp.Data;
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
                    (await GetCacheVault())[d.GetKeyPropertyValue<TKey>()] = d;
                }
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }
    }
}