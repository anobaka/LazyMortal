using System;
using System.Threading.Tasks;

namespace TaskQueue
{
    public class TaskQueueOptions
    {
        public string Id { get; set; }
        /// <summary>
        /// Max concurrent count of running tasks.
        /// </summary>
        public int MaxThreads { get; set; } = int.MaxValue;

        /// <summary>
        /// For executing Number times in Interval.
        /// </summary>
        public (int Number, TimeSpan Interval)? MaxExecutionOptions { get; set; }

        /// <summary>
        /// Interval(ms) for starting a new task.
        /// </summary>
        public int Interval { get; set; }
    }
}