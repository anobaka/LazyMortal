using Newtonsoft.Json;

namespace Bootstrap.Components.IM.DingTalk.Models.ResponseModels
{
    public class DingTalkResponseModel
    {
        [JsonProperty("errcode")] public int ErrCode { get; set; }
        [JsonProperty("errmsg")] public string ErrMsg { get; set; }
    }
}
