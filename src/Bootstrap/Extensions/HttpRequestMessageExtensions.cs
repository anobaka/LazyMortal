using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Bootstrap.Extensions
{
    public class HttpRequestMessageBuilder
    {
        private readonly string _uri;
        private readonly HttpMethod _method;
        private readonly Dictionary<string, List<object>> _queryParameters;
        private readonly Dictionary<string, IEnumerable<object>> _headers;
        private HttpContent _content;

        public HttpRequestMessageBuilder(string uri, HttpMethod method = null,
            Dictionary<string, List<object>> queryParameters = null,
            Dictionary<string, IEnumerable<object>> headers = null)
        {
            _uri = uri;
            _method = method ?? HttpMethod.Get;
            _queryParameters = queryParameters;
            _headers = headers;
        }

        public HttpRequestMessageBuilder AddContent(HttpContent content)
        {
            _content = content;
            return this;
        }

        public HttpRequestMessageBuilder AddJsonContent(object content)
        {
            _content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return this;
        }

        public HttpRequestMessage Build()
        {
            var url = Regex.Replace($"{_uri}", "/{2,}", "/").Replace(":/", "://");
            if (_queryParameters?.Any() == true)
            {
                if (!url.Contains("?")) url += "?";

                var qs = new List<string>();

                qs.AddRange(_queryParameters.Where(t => t.Value?.Any() == true).SelectMany(t =>
                    t.Value.Select(p => $"{WebUtility.UrlEncode(t.Key)}={WebUtility.UrlEncode(p.ToString())}")));

                qs.AddRange(_queryParameters.Where(t => t.Value?.Any() != true)
                    .Select(t => $"{WebUtility.UrlEncode(t.Key)}="));

                url += string.Join("&", qs);
            }

            var m = new HttpRequestMessage(_method, url)
            {
                Content = _content
            };
            if (_headers?.Any() == true)
            {
                foreach (var (key, value) in _headers)
                {
                    m.Headers.Add(key, value.ToString());
                }
            }

            return m;
        }
    }
}