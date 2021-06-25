using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskQueue.Constants;

namespace TaskQueue
{
    public class TaskQueuePool<TOptions> : ITaskDistributor
        where TOptions : TaskQueuePoolOptions, new()
    {
        protected readonly IOptions<TOptions> Options;
        protected List<ITaskQueue> Queues = new List<ITaskQueue>();
        protected ILogger Logger { get; }
        protected object Lock { get; } = new object();
        protected bool Active => Queues.Any(t => t.Active);

        public ConcurrentQueue<TaskData> TaskDataVault { get; } = new ConcurrentQueue<TaskData>();

        public TaskQueueStatus Status { get; private set; } = TaskQueueStatus.Stopped;

        private readonly SemaphoreSlim _sm;
        public int ThreadCount => Options.Value.MaxThreads - _sm.CurrentCount;

        public TaskQueuePool(IOptions<TOptions> options, ILoggerFactory loggerFactory)
        {
            Options = options;
            _sm = new SemaphoreSlim(Options.Value.MaxThreads, Options.Value.MaxThreads);
            Logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        private async Task _onTaskDataEnqueue(TaskData taskData)
        {
            TaskDataVault.Enqueue(taskData);
            // while (TaskDataVault.Count > Options.Value.MaxExecutedTaskDataStorageCapacity)
            // {
            //     // TaskDataVault.TryDequeue(out _);
            // }
        }

        private Task _onTaskDataExecuted(TaskData taskData)
        {
            return Task.FromResult(_sm.Release());
        }

        public void AddQueue(ITaskQueue q)
        {
            Queues.Add(q);
            q.OnTaskDataEnqueue += _onTaskDataEnqueue;
            q.OnExecuted += _onTaskDataExecuted;
        }

        public virtual async Task Stop()
        {
            var canStop = false;
            lock (Lock)
            {
                if (Status == TaskQueueStatus.Running)
                {
                    canStop = true;
                    Status = TaskQueueStatus.Stopping;
                }
            }

            if (canStop)
            {
                try
                {
                    var shuttingDownTasks = Queues.Select(t => t.Shutdown());
                    await Task.WhenAll(shuttingDownTasks);
                }
                catch (Exception e)
                {
                    // Roll back status.
                    Status = TaskQueueStatus.Running;
                    Logger.LogError(e, "An error occured during stopping task queue pool.");
                    return;
                }

                Status = TaskQueueStatus.Stopped;
            }
        }

        public virtual async Task Start()
        {
            var canStart = false;
            lock (Lock)
            {
                if (Status == TaskQueueStatus.Stopped)
                {
                    canStart = true;
                    Status = TaskQueueStatus.Starting;
                }
            }

            if (canStart)
            {
                try
                {
                    var startingTasks = Queues.Select(t => t.Start());
                    await Task.WhenAll(startingTasks);
                }
                catch (Exception e)
                {
                    // Roll back status.
                    Status = TaskQueueStatus.Stopped;
                    Logger.LogError(e, "An error occured during starting task queue pool.");
                    return;
                }

                Status = TaskQueueStatus.Running;
            }
        }

        public async Task Distribute(IEnumerable<TaskData> data)
        {
            foreach (var d in data)
            {
                foreach (var q in Queues.Where(q => q.CanHandle(d)))
                {
                    await _sm.WaitAsync();

                    if (await q.TryEnqueueTaskData(d))
                    {
                        break;
                    }

                    _sm.Release();
                }
            }
        }

        public Task Distribute(params TaskData[] data)
        {
            return Distribute((IEnumerable<TaskData>) data);
        }
    }
}