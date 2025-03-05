using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bootstrap.Components.Configuration.Abstractions
{
    public interface IBOptionsManager<TOptions> : IBOptions<TOptions> where TOptions : class
    {
        void Save(TOptions options);
        Task SaveAsync(TOptions options);
        public async Task SaveAsync(Action<TOptions> modify)
        {
            var options = JsonConvert.DeserializeObject<TOptions>(JsonConvert.SerializeObject(Value));
            modify(options);
            await SaveAsync(options);
        }
    }
}