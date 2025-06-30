﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bootstrap.Components.Miscellaneous
{
    //todo: add options
    public class SimpleRequestLoggingMiddleware
    {
        private readonly EventId _eventId = new EventId(0, nameof(SimpleRequestLoggingMiddleware));
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly IOptions<SimpleRequestLoggingOptions> _options;

        public SimpleRequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory,
            IOptions<SimpleRequestLoggingOptions> options)
        {
            _next = next;
            _options = options;
            _logger = loggerFactory.CreateLogger<InternalAuthenticationMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            var startDt = DateTime.Now;
            var sw = new Stopwatch();
            sw.Start();
            using (var reqBody = new MemoryStream())
            {
                var originalReqBody = context.Request.Body;
                if (_options.Value.LogRequestBody(context.Request))
                {
                    await originalReqBody.CopyToAsync(reqBody);
                    reqBody.Seek(0, SeekOrigin.Begin);
                    context.Request.Body = reqBody;
                }

                using (var resBody = new MemoryStream())
                {
                    var originalResBody = context.Response.Body;
                    context.Response.Body = resBody;

                    await _next(context);

                    // logging
                    string reqString;
                    if (_options.Value.LogRequestBody(context.Request))
                    {
                        reqBody.Seek(0, SeekOrigin.Begin);

                        using (var reqSteamReader = new StreamReader(reqBody))
                        {
                            reqString = await reqSteamReader.ReadToEndAsync();
                            context.Request.Body = originalReqBody;
                        }
                    }
                    else
                    {
                        reqString = $"{context.Request.ContentLength} bytes are ignored";
                    }

                    string resString;
                    resBody.Seek(0, SeekOrigin.Begin);
                    using (var resStreamReader = new StreamReader(resBody))
                    {
                        if (_options.Value.LogResponseBody(context.Response))
                        {
                            resString = await resStreamReader.ReadToEndAsync();
                        }
                        else
                        {
                            resString = $"{resBody.Length} bytes are ignored";
                        }

                        resBody.Seek(0, SeekOrigin.Begin);
                        await resBody.CopyToAsync(originalResBody);
                        context.Response.Body = originalResBody;
                    }

                    var authentication = await context.AuthenticateAsync();
                    // may be null
                    var principal = authentication.Principal;
                    var isAuthenticated = principal?.Identity?.IsAuthenticated == true;
                    sw.Stop();
                    var lines = new List<string>
                    {
                        $"trace id: {context.TraceIdentifier}",
                        BuildDelimiter("request"),
                        $"received at: {startDt:yyyy-MM-dd HH:mm:ss.fff}",
                        $"url: {context.Request.GetDisplayUrl()}",
                        $"remote address: {context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}",
                        $"method: {context.Request.Method}",
                        $"headers: {(!context.Request.Headers.Any() ? null : $"{Environment.NewLine}{string.Join(Environment.NewLine, context.Request.Headers.Select(t => $"\t{t.Key}: {t.Value}"))}")}",
                        $"body: {reqString}",
                        BuildDelimiter("session"),
                        $"authenticated: {isAuthenticated}",
                        $"name: {principal?.Identity?.Name}",
                        $"claims: {(principal != null ? $"{{{string.Join(", ", principal.Claims.Select(t => $"{t.Type}: {t.Value}"))}}}" : null)}",
                        BuildDelimiter("response"),
                        $"status: {context.Response.StatusCode}",
                        $"headers: {(!context.Response.Headers.Any() ? null : $"{Environment.NewLine}{string.Join(Environment.NewLine, context.Response.Headers.Select(t => $"\t{t.Key}: {t.Value}"))}")}",
                        $"body: {resString}",
                        $"elapsed: {sw.ElapsedMilliseconds}ms"
                    };
                    _logger.LogInformation(_eventId, $"{string.Join(Environment.NewLine, lines)}");
                }
            }
        }

        private static string BuildDelimiter(string text)
        {
            var leftLength = (50 - text.Length) / 2;
            return $"{text.PadLeft(leftLength + text.Length, '-').PadRight(50, '-')}";
        }
    }

    public class SimpleRequestLoggingOptions
    {
        public Func<HttpRequest, bool> LogRequestBody { get; set; } =
            req =>
            {
                var ct = req.ContentType?.ToLower();
                return string.IsNullOrEmpty(ct) || ct.Contains("json") || ct.Contains("xml");
            };

        public Func<HttpResponse, bool> LogResponseBody { get; set; } =
            res =>
            {
                var ct = res.ContentType?.ToLower();
                return string.IsNullOrEmpty(ct) || ct.Contains("json") || ct.Contains("xml");
            };
    }
}