using System;
using System.Threading.Tasks;

namespace Bootstrap.Components.Notification.Implementations.WeChat
{
    [Obsolete]
    public interface IWeChatAccessTokenProvider
    {
        Task<string> GetAccessToken();
    }
}
