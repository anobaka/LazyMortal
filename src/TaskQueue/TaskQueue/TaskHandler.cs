using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskQueue.Constants;

namespace TaskQueue
{
    public abstract class
        TaskHandler<TOptions, TTaskData> : ITaskHandler
        where TOptions : TaskHandlerOptions, new() where TTaskData : TaskData
    {
        protected IOptions<TOptions> Options { get; }
        public ITaskDistributor TaskDistributor { get; }
        protected ILogger Logger { get; }

        protected TaskHandler(IOptions<TOptions> options, ITaskDistributor taskDistributor,
            ILoggerFactory loggerFactory)
        {
            Options = options;
            TaskDistributor = taskDistributor;
            Logger = loggerFactory.CreateLogger(GetType().FullName);
        }


        protected abstract Task<TaskDataExecutionResult> HandleInternal(TTaskData taskData, CancellationToken ct);

        public virtual async Task<TaskDataExecutionResult> Handle(TaskData taskData)
        {
            taskData.TryTimes++;
            CancellationToken ct;
            if (Options.Value.Timeout > 0)
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(Options.Value.Timeout);
                ct = cts.Token;
            }
            else
            {
                ct = CancellationToken.None;
            }

            TaskDataExecutionResult result;

            try
            {
                result = await HandleInternal((TTaskData) taskData, ct);
            }
            catch (Exception e)
            {
                result = TaskDataExecutionResult.Failed;
                Logger.LogError(e, $"An error occured during handling task data.");
                await OnException(taskData, e);
            }

            if (result == TaskDataExecutionResult.Failed &&
                (Options.Value.MaxTryTimes == 0 || taskData.TryTimes < Options.Value.MaxTryTimes))
            {
                await TaskDistributor.Distribute(new List<TaskData> {taskData});
                result = TaskDataExecutionResult.Pending;
            }

            return result;
        }

        protected virtual Task OnException(TaskData data, Exception e) => Task.CompletedTask;

        public virtual bool CanHandle(TaskData data)
        {
            return data is TTaskData;
        }
    }
}