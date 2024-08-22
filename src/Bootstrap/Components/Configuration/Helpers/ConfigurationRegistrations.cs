using Humanizer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SignalR;
using Bootstrap.Components.Configuration.Abstractions;
using Microsoft.Extensions.Logging;

namespace Bootstrap.Components.Configuration.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigurationRegistrations
    {
        private readonly List<Assembly> _additionalAssemblies = new();

        public void AddApplicationPart(Assembly assembly)
        {
            _additionalAssemblies.Add(assembly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPathToSaveOptions"></param>
        /// <returns></returns>
        public OptionsDescriber[] DiscoverAllOptionsDescribers(string rootPathToSaveOptions)
        {
            var sw = new Stopwatch();
            sw.Start();

            var refsAssemblies = Assembly.GetEntryAssembly()!.GetReferencedAssemblies().Select(Assembly.Load).ToArray();
            var allAssemblies =
                refsAssemblies.Concat(
                    _additionalAssemblies.Where(a => refsAssemblies.All(b => b.FullName != a.FullName)));

            var allOptionTypes = allAssemblies.SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch
                {
                    return [];
                }
            }).Where(a =>
                a.IsPublic && !a.IsAbstract && a.IsClass &&
                a.GetCustomAttribute<OptionsAttribute>() != null);
            var describers = allOptionTypes
                .Select(t => ConfigurationUtils.GetOptionsDescriber(t, rootPathToSaveOptions)).ToArray();

            sw.Stop();

            return describers;
        }

        public IConfigurationBuilder AddRegisteredFile(IConfigurationBuilder builder, OptionsDescriber optionsDescriber)
        {
            builder.AddJsonFile(optionsDescriber.FilePath, optional: true, reloadOnChange: true);

            return builder;
        }

        public IServiceCollection Configure(IServiceCollection services, IConfiguration configuration,
            OptionsDescriber optionsDescriber)
        {

            var reflectedMethods =
                typeof(OptionsConfigurationServiceCollectionExtensions).GetMethods(BindingFlags.Static |
                    BindingFlags.Public);
            var targetMethod = reflectedMethods.FirstOrDefault(a =>
            {
                if (a.Name == nameof(OptionsConfigurationServiceCollectionExtensions.Configure))
                {
                    var parameters = a.GetParameters();
                    if (parameters.Length == 2)
                    {
                        var x = parameters[0];
                        if (x.ParameterType == SpecificTypeUtils<IServiceCollection>.Type)
                        {
                            var y = parameters[1];
                            if (y.ParameterType == SpecificTypeUtils<IConfiguration>.Type)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            })!;

            targetMethod.MakeGenericMethod(optionsDescriber.OptionsType).Invoke(
                typeof(OptionsConfigurationServiceCollectionExtensions),
                new object[] {services, configuration.GetSection(optionsDescriber.OptionsKey)});

            // inject IOptionsManager
            var monitorType = typeof(IOptionsMonitor<>).MakeGenericType(optionsDescriber.OptionsType);
            var loggerType =
                typeof(ILogger<>).MakeGenericType(
                    typeof(AspNetCoreOptionsManager<>).MakeGenericType(optionsDescriber.OptionsType));
            var optionsManagerType = typeof(AspNetCoreOptionsManager<>).MakeGenericType(optionsDescriber.OptionsType);
            services.AddSingleton(optionsManagerType, sp =>
            {
                try
                {
                    var monitor = sp.GetRequiredService(monitorType);
                    var logger = sp.GetRequiredService(loggerType);
                    var instance = Activator.CreateInstance(optionsManagerType,
                        optionsDescriber.FilePath,
                        optionsDescriber.OptionsKey,
                        monitor,
                        logger
                    );
                    return instance;
                }
                catch
                {
                    throw;
                }

                return null;
            });

            var optionsManagerServiceType = typeof(IBOptionsManager<>).MakeGenericType(optionsDescriber.OptionsType);
            var optionsServiceType = typeof(IBOptions<>).MakeGenericType(optionsDescriber.OptionsType);
            services.AddSingleton(optionsManagerServiceType, sp => sp.GetRequiredService(optionsManagerType));
            services.AddSingleton(optionsServiceType, sp => sp.GetRequiredService(optionsManagerType));

            return services;
        }
    }
}