using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Models.ResponseModels;

namespace Bootstrap.Components.Notification.Senders.Implementations
{
    public class SmsSender : ISmsSender
    {
        public async Task<BaseResponse> Send(SmsMessage message)
        {
            return await Task.FromResult(new BaseResponse());
        }
    }
}
