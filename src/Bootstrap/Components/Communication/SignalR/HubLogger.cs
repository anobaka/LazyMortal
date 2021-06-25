using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bootstrap.Components.Communication.SignalR
{
    public class HubLogger: IHubFilter
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

        public HubLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var logger = GetLogger(invocationContext.Hub.GetType().FullName);
            var requestId = Guid.NewGuid().ToString().Substring(0, 6);
            var keyLog =
                $"[{invocationContext.Context.ConnectionId}:{invocationContext.Context.UserIdentifier}:{requestId}]";
            logger.LogInformation(
                $"{keyLog}Calling hub method: '{invocationContext.HubMethodName}', data: {JsonConvert.SerializeObject(invocationContext.HubMethodArguments)}");
            try
            {
                return await next(invocationContext);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"{keyLog}Error occurred during calling hub method: {e.Message}");
                throw;
            }
        }

        public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            var logger = GetLogger(context.Hub.GetType().FullName);
            var keyLog =
                $"[{context.Context.ConnectionId}:{context.Context.UserIdentifier}]";
            logger.LogInformation($"{keyLog}Connected");
            return Task.CompletedTask;
        }

        public Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception,
            Func<HubLifetimeContext, Exception?, Task> next)
        {
            var logger = GetLogger(context.Hub.GetType().FullName);
            var keyLog =
                $"[{context.Context.ConnectionId}:{context.Context.UserIdentifier}]";
            logger.LogInformation($"{keyLog}Disconnected");
            return Task.CompletedTask;
        }

        private ILogger GetLogger(string hubName)
        {
            return _loggers.GetOrAdd(hubName, t => _loggerFactory.CreateLogger(t));
        }
    }
}
