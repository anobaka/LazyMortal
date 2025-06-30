using System;
using System.Threading.Tasks;
using Bootstrap.Components.Notification.Abstractions.Infrastructures;
using Bootstrap.Components.Notification.Abstractions.Models.Constants;
using Bootstrap.Components.Notification.Abstractions.Models.Entities;
using Bootstrap.Components.Notification.Implementations.WeChat.MessageContents;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Implementations.WeChat
{
    [Obsolete]
    public class WeChatMessageSender : INotifier, IWeChatMessageSender
    {
        public async Task<BaseResponse> Send(WeChatTemplateMessageContent message)
        {
            return await Task.FromResult(new BaseResponse());
        }

        public Task<BaseResponse> Send(Message message)
        {
            throw new System.NotImplementedException();
        }

        public NotificationType Type => NotificationType.WeChat;
    }
}
