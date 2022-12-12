using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Components.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection RegisterAllRegisteredTypeAs<TServiceType>(this IServiceCollection services)
            where TServiceType : class
        {
            var availableServices = services.Where(a => a.ServiceType.IsAssignableTo(typeof(TServiceType))).ToArray();
            foreach (var service in availableServices)
            {
                if (service.ServiceType != typeof(TServiceType))
                {
                    services.Add(new ServiceDescriptor(SpecificTypeUtils<TServiceType>.Type,
                        t => t.GetRequiredService(service.ImplementationType ?? service.ServiceType) as TServiceType,
                        service.Lifetime));
                }
            }

            return services;
        }
    }
}