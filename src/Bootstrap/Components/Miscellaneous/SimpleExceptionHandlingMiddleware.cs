using System;
using System.Threading.Tasks;
using Bootstrap.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Bootstrap.Components.Miscellaneous
{
    /// <summary>
    /// todo: optimization
    /// </summary>
    public class SimpleExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IOptions<SimpleExceptionHandlingOptions> _options;
        private readonly EventId _eventId = new EventId(0, nameof(SimpleExceptionHandlingMiddleware));

        public SimpleExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory,
            IOptions<SimpleExceptionHandlingOptions> options)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<SimpleExceptionHandlingMiddleware>();
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(_eventId, e, context.BuildRequestInformation());
                if (!context.Response.HasStarted)
                {
                    if (_options.Value.NoCache)
                    {
                        context.Response.Headers[HeaderNames.CacheControl] = "no-cache";
                        context.Response.Headers[HeaderNames.Pragma] = "no-cache";
                        context.Response.Headers[HeaderNames.Expires] = "-1";
                        context.Response.Headers.Remove(HeaderNames.ETag);
                    }

                    if (_options.Value.ModifyResponse != null)
                    {
                        await _options.Value.ModifyResponse(context.Response, e);
                    }
                }
            }
        }
    }

    public static class ExceptionHandlingMiddlewareServiceCollectionExtensions
    {
        public static IApplicationBuilder UseSimpleExceptionHandler(this IApplicationBuilder app,
            SimpleExceptionHandlingOptions options)
        {
            return app.UseMiddleware<SimpleExceptionHandlingMiddleware>(Options.Create(options));
        }
    }
}