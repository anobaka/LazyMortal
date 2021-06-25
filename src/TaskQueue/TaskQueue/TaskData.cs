using TaskQueue.Constants;

namespace TaskQueue
{
    public abstract class TaskData
    {
        public int TryTimes { get; set; }
        public bool ExecuteImmediately { get; set; }
        public TaskDataExecutionResult ExecutionResult { get; set; }
    }
}