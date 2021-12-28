using Bootstrap.Components.Tasks.Progressor.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bootstrap.Components.Tasks.Progressor
{
    public static class SimpleProgressorStartupExtensions
    {
        public static IServiceCollection AddSimpleProgressor<TProgressor>(this IServiceCollection services)
            where TProgressor : class, IProgressor
        {
            services.TryAddSingleton<IProgressor, TProgressor>();
            services.TryAddSingleton<IProgressDispatcher, SimpleProgressorHubDispatcher>();
            return services;
        }

        public static IEndpointRouteBuilder MapSimpleProgressorHub(this IEndpointRouteBuilder endpoints, string uri)
        {
            endpoints.MapHub<SimpleProgressorHub>(uri);
            SimpleProgressorHub.ServiceProvider = endpoints.ServiceProvider;
            return endpoints;
        }
    }
}