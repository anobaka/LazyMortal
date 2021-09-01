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
        private static IServiceProvider _rootServiceProvider;

        public static IServiceProvider RootServiceProvider
        {
            get
            {
                if (_rootServiceProvider == null)
                {
                    throw new Exception(
                        "Root service provider is not configured, use IApplicationBuilder.ConfigureBootstrapService to configure it or set BootstrapService.RootServiceProvider manually.");
                }

                return _rootServiceProvider;
            }
            set => _rootServiceProvider = value;
        }

        public T GetRequiredService<T>() => RootServiceProvider.GetRequiredService<T>();
        public T GetService<T>() => RootServiceProvider.GetService<T>();
    }

    public static class BootstrapServiceExtensions
    {
        public static IApplicationBuilder ConfigureBootstrapService(this IApplicationBuilder app)
        {
            BootstrapService.RootServiceProvider = app.ApplicationServices;
            return app;
        }
    }
}