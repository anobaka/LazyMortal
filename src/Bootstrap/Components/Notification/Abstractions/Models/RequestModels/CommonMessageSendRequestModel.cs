using System;
using Bootstrap.Components.Notification.Abstractions.Models.Constants;

namespace Bootstrap.Components.Notification.Abstractions.Models.RequestModels
{
    public class CommonMessageSendRequestModel
    {
        public string Receiver { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public DateTime? ScheduleDt { get; set; }
        public NotificationType Types { get; set; }
    }
}