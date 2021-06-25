using System;

namespace Bootstrap.Components.Communication.HttpClient
{
    [Obsolete("Use BootstrapLoggingDelegatingHandler instead")]
    public class ServiceHttpClientOptions
    {
        public TimeSpan? Timeout { get; set; }
        public string Endpoint { get; set; }
        public bool BlockException { get; set; }
    }
}