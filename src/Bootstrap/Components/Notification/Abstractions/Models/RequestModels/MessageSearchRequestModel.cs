using Bootstrap.Components.Notification.Abstractions.Models.Constants;
using Bootstrap.Models.RequestModels;
using System;

namespace Bootstrap.Components.Notification.Abstractions.Models.RequestModels
{
    [Obsolete]
    public record MessageSearchRequestModel : SearchRequestModel
    {
        public string Receiver { get; set; }
        public NotificationType[] Types { get; set; }
    }
}