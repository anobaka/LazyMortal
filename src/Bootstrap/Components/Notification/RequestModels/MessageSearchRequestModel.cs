using Bootstrap.Models.RequestModels;

namespace Bootstrap.Components.Notification.RequestModels
{
    public class MessageSearchRequestModel : SearchRequestModel
    {
        public string Receiver { get; set; }
    }
}