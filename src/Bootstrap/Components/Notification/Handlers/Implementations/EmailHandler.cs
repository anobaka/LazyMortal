using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Components.Notification.Messages;
using Bootstrap.Components.Notification.RequestModels;
using Bootstrap.Components.Notification.Senders;
using Bootstrap.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Notification.Handlers.Implementations
{
    public abstract class EmailHandler : MessageHandler<EmailMessage>, IEmailHandler
    {
        protected EmailHandler(NotificationDbContext db, IEmailSender sender) : base(db, sender)
        {
        }

        public virtual async Task<SearchResponse<EmailMessage>> Search(MessageSearchRequestModel model)
        {
            var query = Db.EmailMessages.Where(t => t.Email.Equals(model.Email)).OrderByDescending(a => a.Id);
            return new SearchResponse<EmailMessage>
            {
                Data = await query.Skip(model.PageIndex * model.PageSize).Take(model.PageSize).ToListAsync(),
                TotalCount = await query.CountAsync(),
                PageSize = model.PageSize,
                PageIndex = model.PageIndex
            };
        }
    }
}