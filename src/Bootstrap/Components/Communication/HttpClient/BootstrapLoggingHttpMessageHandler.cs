using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Bootstrap.Components.Communication.HttpClient
{
    public class BootstrapLoggingHttpMessageHandler : DelegatingHandler
    {
        private readonly ILogger<BootstrapLoggingHttpMessageHandler> _logger;
        private readonly EventId _eventId;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BootstrapLoggingHttpMessageHandler(ILogger<BootstrapLoggingHttpMessageHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _eventId = new EventId(0, GetType().Name);
        }

        private static Dictionary<string, bool> _validContentTypes = new Dictionary<string, bool>
        {
            {"application/json", true},
            {"application/xml", true},
            {"text/xml", true}
        };

        private static string BuildLogDelimiter(string text)
        {
            var leftLength = (50 - text.Length) / 2;
            return $"{text.PadLeft(leftLength + text.Length, '-').PadRight(50, '-')}";
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage rsp = null;
            var sw = new Stopwatch();
            sw.Start();
            Exception ex = null;
            try
            {
                rsp = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception e)
            {
                ex = e;
                throw;
            }
            finally
            {
                if (sw.IsRunning)
                {
                    sw.Stop();
                }

                string body = null;
                if (request.Content != null)
                {
                    if (request.Content.Headers.ContentType.IsNull() ||
                        _validContentTypes.ContainsKey(request.Content.Headers.ContentType.MediaType))
                    {
                        body = await request.Content.ReadAsStringAsync();
                    }
                }

                body ??= $"Ignored by {nameof(BootstrapLoggingHttpMessageHandler)}";

                var lines = new List<string>
                {
                    $"trace id: {_httpContextAccessor.HttpContext?.TraceIdentifier}",
                    BuildLogDelimiter("request"),
                    $"url: {request.RequestUri}",
                    $"method: {request.Method.Method}",
                    $"headers: {string.Join(Environment.NewLine, request.Headers.Select(t => $"{t.Key}:{string.Join(Environment.NewLine, t.Value)}"))}",
                    $"body: {body}"
                };

                if (rsp != null)
                {
                    string rspBody = null;
                    if (rsp.Content != null)
                    {
                        if (rsp.Content.Headers.ContentType.IsNull() ||
                            _validContentTypes.ContainsKey(rsp.Content.Headers.ContentType.MediaType))
                        {
                            rspBody = await rsp.Content.ReadAsStringAsync();
                        }
                    }

                    rspBody ??= $"Ignored by {nameof(BootstrapLoggingHttpMessageHandler)}";

                    lines.AddRange(new List<string>
                    {
                        BuildLogDelimiter("response"),
                        $"status code: {rsp.StatusCode}",
                        $"headers: {string.Join(Environment.NewLine, rsp.Headers.Select(t => $"{t.Key}:{string.Join(Environment.NewLine, t.Value)}"))}",
                        $"body: {rspBody}",
                        $"elapsed: {sw.ElapsedMilliseconds}ms"
                    });
                }
                else
                {
                    if (ex != null)
                    {
                        lines.AddRange(new List<string>
                        {
                            BuildLogDelimiter("response"),
                            $"elapsed: {sw.ElapsedMilliseconds}ms",
                            ex.BuildFullInformationText()
                        });
                    }
                }

                _logger.LogInformation(_eventId, $"{string.Join(Environment.NewLine, lines)}");
            }

            sw.Stop();
            return rsp;
        }
    }
}