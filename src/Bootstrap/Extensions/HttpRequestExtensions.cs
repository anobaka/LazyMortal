using System;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Bootstrap.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string XForwardedForHeader = "x-forwarded-for";
        private const string AjaxHeader = "X-Requested-With";
        private const string AjaxHeaderValue = "XMLHttpRequest";

        public static string GetClientIp(this HttpRequest request)
        {
            return request.Headers.TryGetValue(XForwardedForHeader, out var values)
                ? values.FirstOrDefault().Split(',')[0].Trim()
                : request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return request.Headers.TryGetValue(AjaxHeader, out var value) &&
                   AjaxHeaderValue.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool AcceptJson(this HttpRequest request)
        {
            return request.Headers[HeaderNames.Accept].ToString().Contains("application/json");
        }

        public static string GetAbsoluteUri(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }

        public static string GetSchemeAndHostParts(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .ToString();
        }

        /// <summary>
        /// Todo: not fully support urlencoding yet.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="relativeUri"></param>
        /// <returns></returns>
        public static string BuildUrl(this HttpRequest request, string relativeUri)
        {
            var prefix = request.GetSchemeAndHostParts().TrimEnd('/');
            var suffix = relativeUri.TrimStart('/');
            if (HttpUtility.UrlDecode(relativeUri).Equals(relativeUri, StringComparison.OrdinalIgnoreCase))
            {
                suffix = string.Join('/',
                    suffix.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(HttpUtility.UrlEncode));
            }

            return $"{prefix}/{suffix}";
        }
    }
}