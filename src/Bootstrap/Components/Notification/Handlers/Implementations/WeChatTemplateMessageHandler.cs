using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Components.Notification.RequestModels;
using Bootstrap.Components.Notification.Senders;
using Bootstrap.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Notification.Handlers.Implementations
{
    public abstract class WeChatTemplateMessageHandler : MessageHandler<WeChatTemplateMessage>, IWeChatTemplateMessageHandler
    {
        protected WeChatTemplateMessageHandler(NotificationDbContext db, IWeChatTemplateMessageSender sender) : base(db, sender)
        {
        }

        public virtual async Task<SearchResponse<WeChatTemplateMessage>> Search(WeChatTemplateMessageSearchRequestModel model)
        {
            var query = Db.WeChatTemplateMessages.Where(t => t.OpenId.Equals(model.OpenId)).OrderByDescending(a => a.Id);
            return new SearchResponse<WeChatTemplateMessage>
            {
                Data = await query.Skip(model.PageIndex * model.PageSize).Take(model.PageSize).ToListAsync(),
                TotalCount = await query.CountAsync(),
                PageSize = model.PageSize,
                PageIndex = model.PageIndex
            };
        }
    }
}
