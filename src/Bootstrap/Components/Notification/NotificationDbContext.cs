using Bootstrap.Components.Notification.Messages;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Notification
{
    public abstract class NotificationDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
    }
}