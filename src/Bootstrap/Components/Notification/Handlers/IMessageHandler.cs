using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Handlers
{
    public interface IMessageHandler<in TMessage> where TMessage : Message
    {
        Task<BaseResponse> Send(TMessage message);
        Task<BaseResponse> Save(TMessage message);
    }
}