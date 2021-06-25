using Newtonsoft.Json;

namespace Bootstrap.Components.IM.DingTalk.Models.ResponseModels
{
    public class DingTalkAccessTokenGetResponseModel : DingTalkResponseModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
