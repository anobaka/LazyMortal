using System;
using System.Threading.Tasks;
using Bootstrap.Components.Notification.Implementations.WeChat.MessageContents;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Implementations.WeChat
{
    [Obsolete]
    internal interface IWeChatMessageSender
    {
        Task<BaseResponse> Send(WeChatTemplateMessageContent message);
    }
}
