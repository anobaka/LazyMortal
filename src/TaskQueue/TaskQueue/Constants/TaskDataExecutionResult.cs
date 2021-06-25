using System;
using System.Collections.Generic;
using System.Text;

namespace TaskQueue.Constants
{
    public enum TaskDataExecutionResult
    {
        Pending = 0,
        Complete = 1,
        Failed = 2
    }
}