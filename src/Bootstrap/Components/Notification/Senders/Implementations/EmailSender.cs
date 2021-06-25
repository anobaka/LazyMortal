using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Senders.Implementations
{
    public class EmailSender : IEmailSender
    {
        public async Task<BaseResponse> Send(EmailMessage message)
        {
            return await Task.FromResult(new BaseResponse());
        }
    }
}
