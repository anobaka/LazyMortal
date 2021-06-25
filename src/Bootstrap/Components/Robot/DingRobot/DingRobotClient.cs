using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.IM.DingTalk.Models;
using Bootstrap.Components.IM.DingTalk.Models.ResponseModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bootstrap.Components.Robot.DingRobot
{
    public class DingRobotClient
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly IOptions<DingRobotClientOptions> _options;

        public DingRobotClient(IOptions<DingRobotClientOptions> options)
        {
            _options = options;
        }

        public async Task<DingTalkResponseModel> SendText(string content)
        {
            return await Send(new DingTalkMessage
            {
                MsgType = "text",
                Text = new DingTalkMessage.MessageText
                {
                    Content = _options.Value.ContentPrefix + content
                }
            });
        }

        public async Task<DingTalkResponseModel> Send(DingTalkMessage message)
        {
            var rsp = await _client.PostAsync(_options.Value.WebhookAddress,
                new StringContent(
                    JsonConvert.SerializeObject(message,
                        new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()}),
                    Encoding.UTF8, "application/json"));
            var body = await rsp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DingTalkResponseModel>(body);
        }
    }
}