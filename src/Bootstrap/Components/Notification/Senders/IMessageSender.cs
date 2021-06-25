using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Senders
{
    public interface IMessageSender<in TMessage> where TMessage : Message
    {
        Task<BaseResponse> Send(TMessage message);
    }
}