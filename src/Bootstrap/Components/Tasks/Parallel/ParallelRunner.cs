using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bootstrap.Components.Tasks.Parallel
{
    public class ParallelRunner
    {
        private int _currentThread;
        private readonly object _lock = new();

        public async Task<List<T>> Run<T>(IEnumerable<Task<T>> tasks, int maxThread)
        {
            var queuedTasks = new List<Task<T>>();
            foreach (var t in tasks)
            {
                if (maxThread == 1)
                {
                    if (t.Status == TaskStatus.Created)
                    {
                        t.Start();
                    }
                }
                if (t.Status == TaskStatus.Created)
                {
                    lock (_lock)
                    {
                        while (_currentThread >= maxThread)
                        {
                            Thread.Sleep(1);
                        }

                        Interlocked.Increment(ref _currentThread);
                    }

                    t.Start();
                    var newTask = t.ContinueWith(x =>
                    {
                        Interlocked.Decrement(ref _currentThread);
                        return x.Result;
                    });
                    queuedTasks.Add(newTask);
                }
                else
                {
                    queuedTasks.Add(t);
                }
            }

            await Task.WhenAll(queuedTasks);

            return queuedTasks.Select(a => a.Result).ToList();
        }
    }
}