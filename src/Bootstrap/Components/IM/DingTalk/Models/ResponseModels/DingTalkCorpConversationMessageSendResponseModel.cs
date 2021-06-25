using Newtonsoft.Json;

namespace Bootstrap.Components.IM.DingTalk.Models.ResponseModels
{
    public class DingTalkCorpConversationMessageSendResponseModel : DingTalkResponseModel
    {
        [JsonProperty("task_id")]
        public long TaskId { get; set; }
    }
}
