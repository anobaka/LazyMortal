using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskQueue.Constants;

namespace TaskQueue
{
    /// <summary>
    /// Queue for execution.
    /// </summary>
    public interface ITaskQueue
    {
        string Id { get; }
        bool CanHandle(TaskData data);

        /// <summary>
        /// Try to push <see cref="TaskData"/> to the end of queue.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Whether <see cref="TaskData"/> has been enqueued.</returns>
        Task<bool> TryEnqueueTaskData(TaskData data);

        /// <summary>
        /// Start this queue.
        /// </summary>
        /// <returns></returns>
        Task Start();

        /// <summary>
        /// Shutdown this queue.
        /// </summary>
        /// <returns></returns>
        Task Shutdown();

        TaskQueueStatus Status { get; }

        int ActiveThreadCount { get; }
        int MaxThreadCount { get; }

        /// <summary>
        /// Whether there is any data or running task in queue.
        /// </summary>
        bool Active { get; }

        event Func<TaskData, Task> OnExecuted;
        event Func<TaskData, Task> OnExecuting;
        event Func<TaskData, Task> OnTaskDataEnqueue;
    }
}