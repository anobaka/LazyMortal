using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Senders
{
    public interface IMessageSender
    {
        Task<BaseResponse> Send(Message message);
    }
}