using System.Threading.Tasks;

namespace Bootstrap.Components.Notification.Senders.Implementations.WeChatMessageSender
{
    public interface IWeChatAccessTokenProvider
    {
        Task<string> GetAccessToken();
    }
}
