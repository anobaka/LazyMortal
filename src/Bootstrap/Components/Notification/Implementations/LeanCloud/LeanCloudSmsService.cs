using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Components.Notification.Implementations.LeanCloud.ResponseModels;
using Bootstrap.Models.Constants;
using Bootstrap.Models.ResponseModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bootstrap.Components.Notification.Implementations.LeanCloud
{
    public class LeanCloudSmsService
    {
        private readonly IOptions<LeanCloudOptions> _options;
        private readonly HttpClient _client;

        public LeanCloudSmsService(IOptions<LeanCloudOptions> options)
        {
            _options = options;
            _client = new HttpClient()
            {
                DefaultRequestHeaders =
                {
                    {"X-LC-Id", _options.Value.LcId},
                    {"X-LC-Key", _options.Value.LcKey},
                }
            };
        }

        protected virtual async Task<TRsp> PostJson<TRsp>(string uri, object data)
        {
            var rspBody = await (await _client.PostAsync($"{_options.Value.Endpoint}{uri}",
                    new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"))).Content
                .ReadAsStringAsync();
            var rsp = JsonConvert.DeserializeObject<TRsp>(rspBody);
            return rsp;
        }

        public async Task<BaseResponse> Send(string mobile, string templateName, Dictionary<string, object> @params)
        {
            var rsp = await PostJson<LeanCloudResponseModel>("/1.1/requestSmsCode", new Dictionary<string, object>
            {
                {"mobilePhoneNumber", mobile},
                {"template", templateName}
            }.Concat(@params).ToDictionary(t => t.Key, t => t.Value));
            return rsp.Code == 0
                ? BaseResponseBuilder.Ok
                : BaseResponseBuilder.Build(ResponseCode.SystemError, rsp.Error);
        }

        public async Task<BaseResponse> SendSmsCaptcha(string mobile)
        {
            var rsp = await PostJson<LeanCloudResponseModel>("/1.1/requestSmsCode", new {MobilePhoneNumber = mobile});
            return rsp.Code == 0
                ? BaseResponseBuilder.Ok
                : BaseResponseBuilder.Build(ResponseCode.SystemError, rsp.Error);
        }

        public async Task<bool> ValidateSmsCaptcha(string mobile, string code)
        {
            var rsp = await PostJson<LeanCloudResponseModel>($"/1.1/verifySmsCode/{code}",
                new {MobilePhoneNumber = mobile});
            return rsp.Code == 0;
        }
    }
}