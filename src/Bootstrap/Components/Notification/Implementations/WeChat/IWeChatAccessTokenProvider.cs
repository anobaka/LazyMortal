using System.Threading.Tasks;

namespace Bootstrap.Components.Notification.Implementations.WeChat
{
    public interface IWeChatAccessTokenProvider
    {
        Task<string> GetAccessToken();
    }
}
