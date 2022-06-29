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
    public class GlobalCacheVault : ConcurrentDictionary<string, object>
    {
        private static readonly SemaphoreSlim Lock = new(1, 1);
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();

        public async Task<SemaphoreSlim> RequestLock(string resourceKey)
        {
            await Lock.WaitAsync();
            if (!Locks.TryGetValue(resourceKey, out var @lock))
            {
                @lock = Locks[resourceKey] = new SemaphoreSlim(1, 1);
            }

            Lock.Release();

            await @lock.WaitAsync();
            return @lock;
        }
    }
}