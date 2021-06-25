using Newtonsoft.Json;

namespace Bootstrap.Components.IM.DingTalk.Models
{
    public class DingTalkMessage
    {
        [JsonProperty("msgtype")]
        public string MsgType { get; set; }
        public MessageText Text { get; set; }
        public class MessageText
        {
            public string Content { get; set; }
        }
    }
}
