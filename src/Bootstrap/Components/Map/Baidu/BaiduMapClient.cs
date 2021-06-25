using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Communication.HttpClient;
using Bootstrap.Components.Map.Baidu.Models.ResponseModels;
using Bootstrap.Models.RequestModels;
using Microsoft.Extensions.Options;

namespace Bootstrap.Components.Map.Baidu
{
    public class BaiduMapClient : ServiceHttpClient<BaiduMapClientOptions>
    {
        public BaiduMapClient(IOptions<BaiduMapClientOptions> options) : base(options)
        {
        }

        public async Task<BaiduMapResponseModel<BaiduMapParseCoordinateResult>> ParseCoordinate(decimal longitude,
            decimal latitude)
        {
            return await InvokeAsync<BaiduMapResponseModel<BaiduMapParseCoordinateResult>>(
                new ServiceHttpClientRequestModel
                {
                    RelativeUri = "/geocoder/v2/",
                    QueryParameters = new Dictionary<string, List<object>>
                    {
                        {"location", new List<object> {$"{latitude},{longitude}"}}
                    },
                    Method = HttpMethod.Get
                });
        }

        public async Task<BaiduMapResponseModel<BaiduMapParseAddressResult>> ParseAddress(string address)
        {
            return await InvokeAsync<BaiduMapResponseModel<BaiduMapParseAddressResult>>(
                new ServiceHttpClientRequestModel
                {
                    RelativeUri = "/geocoder/v2/",
                    QueryParameters = new Dictionary<string, List<object>>
                    {
                        {"address", new List<object> {address}}
                    },
                    Method = HttpMethod.Get
                });
        }

        protected override Task<T> InvokeAsync<T>(ServiceHttpClientRequestModel request,
            CancellationToken? cancellationToken = null)
        {
            if (request.QueryParameters == null)
            {
                request.QueryParameters = new Dictionary<string, List<object>>();
            }

            request.QueryParameters["ak"] = new List<object> {Options.Value.Ak};
            request.QueryParameters["output"] = new List<object> {"json"};
            return base.InvokeAsync<T>(request, cancellationToken);
        }
    }
}