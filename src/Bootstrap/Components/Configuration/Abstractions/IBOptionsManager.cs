using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bootstrap.Components.Configuration.Abstractions
{
    public interface IBOptionsManager<TOptions> : IBOptionsManagerInternal, IBOptions<TOptions> where TOptions : class
    {
        void Save(TOptions options);
        Task SaveAsync(TOptions options);
        Task SaveAsync(Action<TOptions> modify);
    }
}