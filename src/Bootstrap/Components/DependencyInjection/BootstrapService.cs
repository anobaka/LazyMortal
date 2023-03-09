using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Api.LogService.Domain.MachineGroup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Components.DependencyInjection
{
    public abstract class BootstrapService
    {
        private readonly IServiceProvider _serviceProvider;

        protected BootstrapService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T GetRequiredService<T>() => _serviceProvider.GetRequiredService<T>();
        public object GetRequiredService(Type serviceType) => _serviceProvider.GetRequiredService(serviceType);
        public T GetService<T>() => _serviceProvider.GetService<T>();
    }
}