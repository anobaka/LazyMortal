using Bootstrap.Components.Communication.HttpClient;

namespace Bootstrap.Components.IM.DingTalk
{
    public class DingTalkClientOptions : ServiceHttpClientOptions
    {
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string AgentId { get; set; }
    }
}
