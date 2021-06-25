using System;
using System.Collections.Generic;
using System.Text;

namespace TaskQueue.CommonTaskQueues.Handlers.DownloadTaskHandler
{
    public enum DownloadTaskDataResult
    {
        Pending = 0,
        Filtered = 1,
        Skipped = 2,
        Downloaded = 3,
        Error = 4
    }
}