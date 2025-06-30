using System;
using System.Threading.Tasks;

namespace Bootstrap.Components.Notification.Implementations.WeChat
{
    [Obsolete]
    public class WeChatAccessTokenProvider : IWeChatAccessTokenProvider
    {
        public async Task<string> GetAccessToken()
        {
            return await Task.FromResult((string) null);
        }
    }
}
