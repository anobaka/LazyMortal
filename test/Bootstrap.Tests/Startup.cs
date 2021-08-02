using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android;
using Bootstrap.Components.Mobiles.Android.Infrastructures;
using Bootstrap.Components.Mobiles.Android.Wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Bootstrap.Tests
{
    class Startup
    {
        public void ConfigureHost(IHostBuilder hostBuilder)
        {
            hostBuilder
                .ConfigureHostConfiguration(builder => { builder.AddJsonFile("appsettings.json", true); });
        }

        public void ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            services.AddLogging(t =>
            {
                t.AddConsole();
            });

            services.Configure<AdbOptions>(hostBuilderContext.Configuration.GetSection(nameof(AdbOptions)));
            services.AddSingleton<AdbInvoker>();
            services.AddSingleton<Adb>();
        }
    }
}