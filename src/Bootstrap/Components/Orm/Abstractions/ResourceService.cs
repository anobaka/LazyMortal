using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Components.Orm.Abstractions
{
    internal class ResourceService<TDbContext, TResource, TKey> : IResourceService<TDbContext, TResource, TKey>
        where TDbContext : DbContext where TResource : class
    {
        public IServiceProvider ServiceProvider { get; }

        public ResourceService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IResourceService<TDbContext, TResource, TKey> UseNewScope() =>
            new ResourceService<TDbContext, TResource, TKey>(ServiceProvider.CreateScope().ServiceProvider);
    }
}