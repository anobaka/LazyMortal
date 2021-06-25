namespace TaskQueue.CommonTaskQueues.Handlers.DownloadTaskHandler
{
    public class DownloadTaskData : TaskData
    {
        public string Url { get; set; }
        public string RelativeFilename { get; set; }
        public DownloadTaskDataResult DownloadResult { get; set; }
    }
}