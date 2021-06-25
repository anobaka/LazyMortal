using System;
using System.IO;
using System.Threading.Tasks;
using TaskQueue.CommonTaskQueues.Handlers.CrawlerTaskHandler;

namespace TaskQueue.CommonTaskQueues.Handlers.DownloadTaskHandler
{
    public class DownloadTaskHandlerOptions : CrawlerTaskHandlerOptions
    {
        public string DownloadPath { get; set; }
    }
}