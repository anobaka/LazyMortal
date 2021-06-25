using Bootstrap.Components.Notification.Messages;

namespace Bootstrap.Components.Notification.Senders
{
    public interface IEmailSender : IMessageSender<EmailMessage>
    {
    }
}
