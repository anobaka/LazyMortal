using Bootstrap.Components.Notification.Messages;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Notification
{
    public abstract class NotificationDbContext : DbContext
    {
        public DbSet<EmailMessage> EmailMessages { get; set; }
        public DbSet<SmsMessage> SmsMessages { get; set; }
        public DbSet<WeChatTemplateMessage> WeChatTemplateMessages { get; set; }
    }
}