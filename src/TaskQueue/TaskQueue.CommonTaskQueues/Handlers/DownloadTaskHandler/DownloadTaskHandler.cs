using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProxyProvider.HttpClientProvider;
using TaskQueue.CommonTaskQueues.Handlers.CrawlerTaskHandler;
using TaskQueue.Constants;

namespace TaskQueue.CommonTaskQueues.Handlers.DownloadTaskHandler
{
    public class DownloadTaskHandler : DownloadTaskHandler<DownloadTaskHandlerOptions, DownloadTaskData>
    {
        public DownloadTaskHandler(IOptions<DownloadTaskHandlerOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider, IDownloadTaskFilter fileFilter, ILoggerFactory loggerFactory) :
            base(options, taskDistributor,
                httpClientProvider, fileFilter, loggerFactory)
        {
        }
    }

    public class DownloadTaskHandler<TTaskData> : DownloadTaskHandler<DownloadTaskHandlerOptions, TTaskData>
        where TTaskData : DownloadTaskData

    {
        public DownloadTaskHandler(IOptions<DownloadTaskHandlerOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider, IDownloadTaskFilter fileFilter, ILoggerFactory loggerFactory) :
            base(options, taskDistributor,
                httpClientProvider, fileFilter, loggerFactory)
        {
        }
    }

    public class
        DownloadTaskHandler<TOptions, TTaskData> : CrawlerTaskHandler<TOptions, TTaskData>
        where TTaskData : DownloadTaskData
        where TOptions : DownloadTaskHandlerOptions, new()
    {
        protected IDownloadTaskFilter FileFilter { get; }

        public DownloadTaskHandler(IOptions<TOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider, IDownloadTaskFilter fileFilter, ILoggerFactory loggerFactory) :
            base(options, taskDistributor,
                httpClientProvider, loggerFactory)
        {
            FileFilter = fileFilter ?? new DownloadTaskFilter();
        }

        protected virtual Task AfterDownloading(TTaskData taskData, string fileFullname)
        {
            return Task.CompletedTask;
        }

        protected virtual async Task<DownloadTaskDataResult> HandleDownloading(TTaskData taskData, CancellationToken ct)
        {
            var filename = Path.GetFileName(taskData.RelativeFilename);
            var newFilename = filename.RemoveInvalidFileNameChars();
            taskData.RelativeFilename = taskData.RelativeFilename.Replace(filename, newFilename);
            var fullname = Path.Combine(Options.Value.DownloadPath, taskData.RelativeFilename);
            if (!File.Exists(fullname))
            {
                var client = await GetHttpClient();
                var fileDataRsp = await client.GetAsync(taskData.Url, ct);
                if (fileDataRsp.StatusCode != HttpStatusCode.NotFound)
                {
                    if (await FileFilter.Filter(fileDataRsp, out var stream))
                    {
                        return DownloadTaskDataResult.Filtered;
                    }

                    // Seek stream
                    if (stream.Position != 0)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }

                    var directory = Path.GetDirectoryName(fullname);
                    // Ensure directory exist
                    Directory.CreateDirectory(directory);
#if NETCOREAPP2_1
                    using var fs = File.Create(fullname);
#else
                    await using var fs = File.Create(fullname);
#endif
                    await stream.CopyToAsync(fs, (int) stream.Length, ct);
                    await AfterDownloading(taskData, fullname);
                    return DownloadTaskDataResult.Downloaded;
                }

                return DownloadTaskDataResult.Error;
            }
            else
            {
                return DownloadTaskDataResult.Skipped;
            }
        }

        protected override async Task<TaskDataExecutionResult> HandleInternal(TTaskData taskData, CancellationToken ct)
        {
            taskData.DownloadResult = await HandleDownloading(taskData, ct);
            return taskData.DownloadResult == DownloadTaskDataResult.Error
                ? TaskDataExecutionResult.Failed
                : TaskDataExecutionResult.Complete;
        }
    }
}