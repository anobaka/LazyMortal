using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bootstrap.Components.Configuration.Abstractions
{
    public abstract class AbstractOptionsManager<TOptions> : IBOptionsManager<TOptions> where TOptions : class
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public abstract TOptions Value { get; }

        public abstract void Save(TOptions options);
        public abstract Task SaveAsync(TOptions options);

        public virtual async Task SaveAsync(Action<TOptions> modify)
        {
            await _lock.WaitAsync();
            try
            {
                // Read latest value INSIDE the lock to prevent lost updates
                var options = JsonConvert.DeserializeObject<TOptions>(JsonConvert.SerializeObject(Value))!;
                modify(options);
                await SaveAsync(options);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}