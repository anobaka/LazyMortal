namespace TaskQueue
{
    public class TaskQueuePoolOptions
    {
        public int MaxThreads { get; set; } = int.MaxValue;
        public int MinInterval { get; set; }
        // /// <summary>
        // /// Default value is 10000.
        // /// </summary>
        // public int MaxExecutedTaskDataStorageCapacity { get; set; } = 10000;
    }
}