using System;
using System.Collections.Generic;
using System.Text;

namespace TaskQueue.Constants
{
    public enum TaskQueueStatus
    {
        Stopped = 1,
        Starting = 2,
        Running = 3,
        Stopping = 4
    }
}