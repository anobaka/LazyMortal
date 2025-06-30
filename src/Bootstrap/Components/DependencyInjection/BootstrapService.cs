using System;
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
        public T? GetService<T>() => _serviceProvider.GetService<T>();
    }
}