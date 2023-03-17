using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.HSSF.Record.Chart;

namespace Bootstrap.Components.Orm
{
    /// <summary>
    /// Full type name - data
    /// </summary>
    public class GlobalCacheVault : ConcurrentDictionary<string, object>
    {
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public async Task<SemaphoreSlim> RequestLock(string resourceKey)
        {
            await _lock.WaitAsync();
            if (!_locks.TryGetValue(resourceKey, out var @lock))
            {
                @lock = _locks[resourceKey] = new SemaphoreSlim(1, 1);
            }

            _lock.Release();

            await @lock.WaitAsync();
            return @lock;
        }
    }
}