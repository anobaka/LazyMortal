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
        internal class TaskTimerStopwatch : IDisposable
        {
            public readonly Stopwatch Sw;
            private readonly string _id;

            public TaskTimerStopwatch(string id)
            {
                _id = id;
                Sw = Stopwatch.StartNew();
                PendingStopwatches.GetOrAdd(id, _ => new ConcurrentDictionary<Stopwatch, bool>())[Sw] = true;
            }

            public void Dispose()
            {
                Sw.Stop();
                if (PendingStopwatches.TryGetValue(_id, out var list))
                {
                    list.TryRemove(Sw, out _);
                }

                TimeCosts[_id] = TimeCosts.GetValueOrDefault(_id).Add(Sw.Elapsed);
            }
        }

        private static readonly ConcurrentDictionary<string, TimeSpan> TimeCosts = new();

        /// <summary>
        /// id - sw - nothing
        /// </summary>
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<Stopwatch, bool>> PendingStopwatches =
            new();

        public static async Task StartTiming(string id, Func<Task> getTask)
        {
            using var sw = new TaskTimerStopwatch(id);
            var task = getTask();
            await task;
        }

        public static async Task<T> StartTiming<T>(string id, Func<Task<T>> getTask)
        {
            using var sw = new TaskTimerStopwatch(id);
            var result = await getTask();
            return result;
        }

        public static async Task Delay(string id, TimeSpan delay, CancellationToken cancellationToken) =>
            await StartTiming(id, () => Task.Delay(delay, cancellationToken));

        public static void Clear()
        {
            TimeCosts.Clear();
        }

        public static void Clear(string id)
        {
            TimeCosts.TryRemove(id, out _);
        }

        public static TimeSpan Get(string id)
        {
            var t = TimeCosts.GetOrAdd(id, TimeSpan.Zero);
            if (PendingStopwatches.TryGetValue(id, out var list))
            {
                t += list.Keys.Aggregate(TimeSpan.Zero, (s, a) => a.Elapsed.Add(s));
            }

            return t;
        }

        public static Dictionary<string, TimeSpan> GetAll() => TimeCosts.Keys.ToDictionary(a => a, Get);
    }
}