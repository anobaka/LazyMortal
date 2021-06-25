using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bootstrap.Components.Communication.HttpClient
{
    public class BootstrapEnsureSuccessCodeHttpMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var rsp = await base.SendAsync(request, cancellationToken);
            rsp.EnsureSuccessStatusCode();
            return rsp;
        }
    }
}