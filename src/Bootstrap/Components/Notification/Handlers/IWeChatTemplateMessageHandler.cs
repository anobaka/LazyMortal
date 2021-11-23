using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Components.Notification.RequestModels;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Handlers
{
    public interface IWeChatTemplateMessageHandler : IMessageHandler<WeChatTemplateMessageContent>
    {
        Task<SearchResponse<WeChatTemplateMessageContent>> Search(WeChatTemplateMessageSearchRequestModel model);
    }
}
