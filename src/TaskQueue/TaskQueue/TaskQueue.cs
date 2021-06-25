using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskQueue.Constants;

namespace TaskQueue
{
    public class TaskQueue<TOptions, TTaskHandler> : ConcurrentQueue<TaskData>, ITaskQueue
        where TOptions : TaskQueueOptions, new() where TTaskHandler : ITaskHandler
    {
        protected readonly IOptions<TOptions> Options;
        protected readonly TTaskHandler Handler;
        public event Func<TaskData, Task> OnExecuted;
        public event Func<TaskData, Task> OnExecuting;
        public event Func<TaskData, Task> OnTaskDataEnqueue;
        public TaskQueueStatus Status { get; protected set; } = TaskQueueStatus.Stopped;
        public int MaxThreadCount => Options.Value.MaxThreads;
        public int ActiveThreadCount => Options.Value.MaxThreads - _sm.CurrentCount;
        public bool Active => ActiveThreadCount > 0 || this.Any();

        private readonly object _lock = new object();
        private readonly SemaphoreSlim _sm;
        public virtual string Id => Options.Value.Id;

        private readonly SemaphoreSlim _intervalSm;
        private readonly Queue<DateTime> _executionStartDtList;

        public TaskQueue(IOptions<TOptions> options, TTaskHandler handler)
        {
            Options = options;
            Handler = handler;
            _sm = new SemaphoreSlim(Options.Value.MaxThreads, Options.Value.MaxThreads);

            if (options.Value.MaxExecutionOptions.HasValue)
            {
                _executionStartDtList = new Queue<DateTime>();
                _intervalSm = new SemaphoreSlim(options.Value.MaxExecutionOptions.Value.Number,
                    options.Value.MaxExecutionOptions.Value.Number);
            }
        }

        public virtual bool CanHandle(TaskData data)
        {
            return Handler.CanHandle(data);
        }

        public virtual async Task<bool> TryEnqueueTaskData(TaskData data)
        {
            if (Handler.CanHandle(data))
            {
                if (OnTaskDataEnqueue != null)
                {
                    await OnTaskDataEnqueue(data);
                }

                Enqueue(data);
                return true;
            }

            return false;
        }

        private async Task _executeTask(TaskData taskData)
        {
            if (!taskData.ExecuteImmediately)
            {
                await Task.Delay(Options.Value.Interval);
            }

            if (OnExecuting != null)
            {
                await OnExecuting(taskData);
            }

            var result = await Handler.Handle(taskData);
            taskData.ExecutionResult = result;

            if (OnExecuted != null)
            {
                await OnExecuted(taskData);
            }

            _sm.Release();
            _intervalSm?.Release();
        }

        private void _startInternalTimer()
        {
            Task.Run(async () =>
            {
                if (Options.Value.MaxExecutionOptions.HasValue)
                {
                    while (Status == TaskQueueStatus.Running)
                    {
                        if (_executionStartDtList.TryPeek(out var r))
                        {
                            if (DateTime.Now - r > Options.Value.MaxExecutionOptions.Value.Interval)
                            {
                                _executionStartDtList.Dequeue();
                                _intervalSm.Release();
                            }
                        }

                        await Task.Delay(1);
                    }
                }
            });
        }

        private void _start()
        {
            Task.Run(async () =>
            {
                while (Status == TaskQueueStatus.Running)
                {
                    if (this.Any())
                    {
                        await _sm.WaitAsync();

                        if (_intervalSm != null)
                        {
                            await _intervalSm.WaitAsync();
                        }

                        //todo: the sort of queue data is not stable for now.
                        if (TryDequeue(out var data) && data != null)
                        {
                            // Execute asynchronously.
                            _executeTask(data);
                            continue;
                        }

                        _intervalSm?.Release();
                        _sm.Release();
                    }
                    // Avoid unnecessary cpu usage.
                    await Task.Delay(1);
                }
            });
        }

        public virtual async Task Start()
        {
            var canStart = false;
            lock (_lock)
            {
                if (Status == TaskQueueStatus.Stopped)
                {
                    Status = TaskQueueStatus.Running;
                    canStart = true;
                }
            }

            if (canStart)
            {
                if (Options.Value.MaxExecutionOptions.HasValue)
                {
                    _startInternalTimer();
                }

                _start();
            }
        }

        public virtual async Task Shutdown()
        {
            Status = TaskQueueStatus.Stopping;
            while (Count > 0)
            {
                TryDequeue(out _);
            }

            while (_sm.CurrentCount != Options.Value.MaxThreads)
            {
                await Task.Delay(100);
            }

            while (_intervalSm?.CurrentCount != Options.Value.MaxExecutionOptions?.Number)
            {
                await Task.Delay(100);
            }

            Status = TaskQueueStatus.Stopped;
        }
    }
}