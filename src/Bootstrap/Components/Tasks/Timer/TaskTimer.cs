using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bootstrap.Components.Tasks.Timer
{
    public class TaskTimer
    {
        internal class DisposableSw : IDisposable
        {
            public readonly Stopwatch Sw;

            public DisposableSw()
            {
                Sw = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                Sw.Stop();
            }
        }

        private DisposableSw StartNewStopwatch(string id)
        {
            var ts = new DisposableSw();
            _timeCosts.GetOrAdd(id, _ => new ConcurrentDictionary<Stopwatch, bool>())[ts.Sw] = true;
            return ts;
        }

        /// <summary>
        /// id - fake concurrent hashset
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Stopwatch, bool>> _timeCosts = new();

        public async Task StartTiming(string id, Func<Task> getTask)
        {
            using var sw = StartNewStopwatch(id);
            var task = getTask();
            await task;
        }

        public async Task<T> StartTiming<T>(string id, Func<Task<T>> getTask)
        {
            using var sw = StartNewStopwatch(id);
            var result = await getTask();
            return result;
        }

        public async Task Delay(string id, TimeSpan delay, CancellationToken cancellationToken) =>
            await StartTiming(id, () => Task.Delay(delay, cancellationToken));

        public void Clear()
        {
            _timeCosts.Clear();
        }

        public void Clear(string id)
        {
            _timeCosts.TryRemove(id, out _);
        }

        public TimeSpan Get(string id)
        {
            return _timeCosts.TryGetValue(id, out var list)
                ? new TimeSpan(list.Keys.Sum(a => a.ElapsedTicks))
                : TimeSpan.Zero;
        }

        public Dictionary<string, TimeSpan> GetAll() =>
            _timeCosts.ToDictionary(a => a.Key, a => new TimeSpan(a.Value.Keys.Sum(b => b.ElapsedTicks)));
    }
}