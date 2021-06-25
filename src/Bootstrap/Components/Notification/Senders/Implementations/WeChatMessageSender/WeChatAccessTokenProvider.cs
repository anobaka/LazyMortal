using System.Threading.Tasks;

namespace Bootstrap.Components.Notification.Senders.Implementations.WeChatMessageSender
{
    public class WeChatAccessTokenProvider : IWeChatAccessTokenProvider
    {
        public async Task<string> GetAccessToken()
        {
            return await Task.FromResult((string) null);
        }
    }
}
