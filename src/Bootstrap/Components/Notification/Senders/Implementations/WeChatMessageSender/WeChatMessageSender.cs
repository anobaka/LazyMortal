using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Senders.Implementations.WeChatMessageSender
{
    public class WeChatMessageSender : IWeChatTemplateMessageSender
    {
        public async Task<BaseResponse> Send(WeChatTemplateMessage message)
        {
            return await Task.FromResult(new BaseResponse());
        }
    }
}
