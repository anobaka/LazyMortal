using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bootstrap.Components.Configuration.Abstractions
{
    public abstract class AbstractOptionsManager<TOptions> : IBOptionsManager<TOptions> where TOptions : class
    {
        public abstract TOptions Value { get; }

        public abstract void Save(TOptions options);
        public abstract Task SaveAsync(TOptions options);
        public async Task SaveAsync(Action<TOptions> modify)
        {
            var options = JsonConvert.DeserializeObject<TOptions>(JsonConvert.SerializeObject(Value));
            modify(options);
            await SaveAsync(options);
        }
    }
}