using System.Threading.Tasks;
using Bootstrap.Components.Notification.Abstractions.Models.Constants;
using Bootstrap.Components.Notification.Abstractions.Models.Entities;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Abstractions.Infrastructures
{
    public interface INotifier
    {
        Task<BaseResponse> Send(Message message);
        NotificationType Type { get; }
    }
}