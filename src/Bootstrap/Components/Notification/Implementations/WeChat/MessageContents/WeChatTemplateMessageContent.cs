using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace Bootstrap.Components.Notification.Implementations.WeChat.MessageContents
{
    public class WeChatTemplateMessageContent
    {
        public string TemplateId { get; set; }
        public string Url { get; set; }

        public string MiniProgramJson => MiniProgram == null ? null : JsonConvert.SerializeObject(MiniProgram);

        [NotMapped]
        public WeChatTemplateMessageMiniProgram MiniProgram { get; set; }

        public string DataJson => Data?.Any() == true ? JsonConvert.SerializeObject(Data) : null;

        [NotMapped]
        public IDictionary<string, WeChatTemplateMessageData> Data { get; set; }

        public class WeChatTemplateMessageMiniProgram
        {
            public string AppId { get; set; }
            public string PagePath { get; set; }
        }

        public class WeChatTemplateMessageData
        {
            public string Value { get; set; }
            public string Color { get; set; }
        }
    }
}