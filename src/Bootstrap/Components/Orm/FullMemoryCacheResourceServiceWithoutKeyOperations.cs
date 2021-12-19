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
    public abstract class
        FullMemoryCacheResourceServiceWithoutKeyOperations<TDbContext, TResource> : ServiceBase<TDbContext>
        where TDbContext : DbContext where TResource : class
    {
        protected ConcurrentBag<TResource> CacheVault;
        protected ResourceServiceWithoutKeyOperations<TDbContext, TResource> ResourceService;
        protected abstract TResource AttachCacheEntry(TResource resource);


        protected FullMemoryCacheResourceServiceWithoutKeyOperations(IServiceProvider serviceProvider) : base(
            serviceProvider)
        {
            ResourceService = new ResourceServiceWithoutKeyOperations<TDbContext, TResource>(serviceProvider);
            var data = ResourceService.GetAll().ConfigureAwait(false).GetAwaiter().GetResult();
            CacheVault = new ConcurrentBag<TResource>(data);
        }

        public virtual Task<TResource> GetFirst(Expression<Func<TResource, bool>> selector,
            Expression<Func<TResource, object>> orderBy = null, bool asc = false)
        {
            var data = CacheVault.Where(selector.Compile());
            if (orderBy != null)
            {
                var ob = orderBy.Compile();
                data = asc ? data.OrderBy(ob) : data.OrderByDescending(ob);
            }

            return Task.FromResult(data.FirstOrDefault().JsonCopy());
        }

        public virtual Task<List<TResource>> GetAll(Expression<Func<TResource, bool>> selector = null) =>
            Task.FromResult((selector == null ? CacheVault : CacheVault.Where(selector.Compile())).ToList().JsonCopy());

        public virtual Task<int> Count(Func<TResource, bool> selector = null) =>
            Task.FromResult(selector == null ? CacheVault.Count : CacheVault.Count(selector));

        public virtual Task<SearchResponse<TResource>> Search(Func<TResource, bool> selector,
            int pageIndex, int pageSize, Func<TResource, object> orderBy = null, bool asc = false)
        {
            var resources = CacheVault.ToList();
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
            return Task.FromResult(result.JsonCopy());
        }

        public virtual async Task<BaseResponse> Remove(TResource resource)
        {
            var rsp = await ResourceService.Remove(resource);
            if (rsp.Code == (int) ResponseCode.Success)
            {
                CacheVault = CacheVault.Remove(AttachCacheEntry(resource));
            }

            return rsp;
        }

        public virtual async Task<BaseResponse> RemoveRange(IEnumerable<TResource> resources)
        {
            var rs = resources.ToList();
            var rsp = await ResourceService.RemoveRange(rs);
            if (rsp.Code == (int) ResponseCode.Success)
            {
                var associateCache = rs.Select(AttachCacheEntry).ToArray();
                CacheVault = CacheVault.RemoveRange(associateCache);
            }

            return rsp;
        }

        public virtual async Task<BaseResponse> RemoveAll(Expression<Func<TResource, bool>> selector)
        {
            var rsp = await ResourceService.RemoveAll(selector);
            if (rsp.Code == (int) ResponseCode.Success)
            {
                var func = selector.Compile();
                var data = CacheVault.Where(t => func(t));
                CacheVault = CacheVault.RemoveRange(data.Select(AttachCacheEntry));
            }

            return rsp;
        }

        public virtual async Task<SingletonResponse<TResource>> Add(TResource resource)
        {
            var rsp = await ResourceService.Add(resource);
            if (rsp.Data != null)
            {
                CacheVault.Add(rsp.Data);
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }

        public virtual async Task<ListResponse<TResource>> AddRange(List<TResource> resources)
        {
            var rsp = await ResourceService.AddRange(resources);
            if (rsp.Data != null)
            {
                CacheVault.AddRange(resources);
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }

        public virtual async Task<BaseResponse> Update(TResource resource)
        {
            var rsp = await ResourceService.Update(resource);
            if (rsp.Code == (int) ResponseCode.Success)
            {
                CacheVault = CacheVault.Replace(AttachCacheEntry(resource), resource.JsonCopy());
            }

            return rsp;
        }

        public virtual async Task<BaseResponse> UpdateRange(IReadOnlyCollection<TResource> resources)
        {
            var rsp = await ResourceService.UpdateRange(resources);
            if (rsp.Code == (int) ResponseCode.Success)
            {
                CacheVault = CacheVault.RemoveRange(resources.Select(AttachCacheEntry));
                CacheVault.AddRange(resources.JsonCopy());
            }

            return rsp;
        }


        public virtual async Task<SingletonResponse<TResource>> UpdateFirst(Expression<Func<TResource, bool>> selector,
            Action<TResource> modify)
        {
            var rsp = await ResourceService.UpdateFirst(selector, modify);
            if (rsp.Data != null)
            {
                CacheVault = CacheVault.Replace(AttachCacheEntry(rsp.Data), rsp.Data);
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
                CacheVault = CacheVault.RemoveRange(rsp.Data.Select(AttachCacheEntry));
                CacheVault.AddRange(rsp.Data);
            }

            rsp.Data = rsp.Data.JsonCopy();
            return rsp;
        }
    }
}