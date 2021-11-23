using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Senders.Implementations.WeChatMessageSender
{
    public class WeChatMessageSender : IMessageSender, IWeChatMessageSender
    {
        public async Task<BaseResponse> Send(WeChatTemplateMessageContent message)
        {
            return await Task.FromResult(new BaseResponse());
        }

        public Task<BaseResponse> Send(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}
