using Bootstrap.Components.Notification.Abstractions.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Notification.Abstractions
{
    public abstract class NotificationDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
    }
}