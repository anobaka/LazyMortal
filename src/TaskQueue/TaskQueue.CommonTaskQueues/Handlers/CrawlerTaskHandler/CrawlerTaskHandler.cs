using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProxyProvider.HttpClientProvider;

namespace TaskQueue.CommonTaskQueues.Handlers.CrawlerTaskHandler
{
    public abstract class
        CrawlerTaskHandler<TTaskData> : CrawlerTaskHandler<CrawlerTaskHandlerOptions, TTaskData>
        where TTaskData : TaskData

    {
        protected CrawlerTaskHandler(IOptions<CrawlerTaskHandlerOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider, ILoggerFactory loggerFactory) : base(options, taskDistributor,
            httpClientProvider, loggerFactory)
        {
        }
    }

    /// <summary>
    /// Provide proxy client for each request.
    /// </summary>
    /// <typeparam name="TTaskData"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    public abstract class
        CrawlerTaskHandler<TOptions, TTaskData> : TaskHandler<TOptions, TTaskData>
        where TTaskData : TaskData
        where TOptions : CrawlerTaskHandlerOptions, new()
    {
        protected IHttpClientProvider HttpClientProvider { get; }

        protected CrawlerTaskHandler(IOptions<TOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider, ILoggerFactory loggerFactory) : base(options,
            taskDistributor, loggerFactory)
        {
            HttpClientProvider = httpClientProvider;
        }

        protected virtual async Task<HttpClient> GetHttpClient() =>
            await HttpClientProvider.GetClient(Options.Value.HttpClientPurpose);
    }
}