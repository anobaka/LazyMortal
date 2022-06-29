using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bootstrap.Components.Orm.Infrastructures;
using Bootstrap.Extensions;
using Bootstrap.Models;
using Bootstrap.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Orm
{
    public class
        MultilevelResourceService<TDbContext, TMultilevelResource, TKey> : ResourceService<TDbContext,
            TMultilevelResource,
            TKey>
        where TDbContext : DbContext where TMultilevelResource : MultilevelResource<TMultilevelResource>
    {
        public MultilevelResourceService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public virtual async Task<List<TMultilevelResource>> GetPath(TKey id)
        {
            var resource = await GetByKey(id);
            return resource == null ? null : DbContext.Set<TMultilevelResource>().GetPath(resource);
        }

        public virtual async Task<List<TMultilevelResource>> GetFullTree()
        {
            var data = await base.GetAll();
            var root = data.FindAll(t => !t.ParentId.HasValue);
            _populateTree(root, data);
            return root;
        }

        private void _populateTree(IReadOnlyCollection<TMultilevelResource> parents, List<TMultilevelResource> allData)
        {
            if (parents != null && allData != null)
            {
                foreach (var p in parents)
                {
                    p.Children = allData.FindAll(t => t.ParentId == p.Id);
                    p.Children.ForEach(t => t.Parent = p);
                    _populateTree(p.Children, allData);
                }
            }
        }

        /// <summary>
        /// 异步刷新分级关系
        /// </summary>
        /// <returns></returns>
        public virtual async Task BuildTree()
        {
            var tree = await GetFullTree();
            tree.BuildTree();
            await DbContext.SaveChangesAsync();
        }

        public override async Task<SingletonResponse<TMultilevelResource>> Add(TMultilevelResource resource)
        {
            var rsp = await base.Add(resource);
            await BuildTree();
            return rsp;
        }

        public override async Task<ListResponse<TMultilevelResource>> AddRange(IEnumerable<TMultilevelResource> resources)
        {
            var rsp = await base.AddRange(resources);
            await BuildTree();
            return rsp;
        }

        public override async Task<BaseResponse> RemoveAll(Expression<Func<TMultilevelResource, bool>> selector)
        {
            var rsp = await base.RemoveAll(selector);
            await DbContext.SaveChangesAsync();
            await BuildTree();
            return rsp;
        }

        public override async Task<BaseResponse> RemoveByKey(TKey key)
        {
            var rsp = await base.RemoveByKey(key);
            await DbContext.SaveChangesAsync();
            await BuildTree();
            return rsp;
        }

        public override async Task<BaseResponse> RemoveByKeys(IEnumerable<TKey> keys)
        {
            var rsp = await base.RemoveByKeys(keys);
            await DbContext.SaveChangesAsync();
            await BuildTree();
            return rsp;
        }

        public virtual async Task<List<TMultilevelResource>> GetProgenies(TKey key, bool tile)
        {
            var descent = await GetByKey(key);
            var progenies = await GetAll(t => t.Left > descent.Left && t.Right < descent.Right);
            var result = tile ? progenies : progenies.Connect();
            return result;
        }
    }
}