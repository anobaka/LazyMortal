using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Senders.Implementations.WeChatMessageSender
{
    internal interface IWeChatMessageSender
    {
        Task<BaseResponse> Send(WeChatTemplateMessageContent message);
    }
}
