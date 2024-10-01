using Bootstrap.Components.Notification.Abstractions.Models.Constants;
using Bootstrap.Models.RequestModels;

namespace Bootstrap.Components.Notification.Abstractions.Models.RequestModels
{
    public record MessageSearchRequestModel : SearchRequestModel
    {
        public string Receiver { get; set; }
        public NotificationType[] Types { get; set; }
    }
}