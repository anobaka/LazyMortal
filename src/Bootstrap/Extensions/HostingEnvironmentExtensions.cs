using Microsoft.Extensions.Hosting;

namespace Bootstrap.Extensions
{
    public static class HostingEnvironmentExtensions
    {
        public static bool IsTesting(this IHostEnvironment env)
        {
            return env.IsEnvironment("Testing");
        }
    }
}
