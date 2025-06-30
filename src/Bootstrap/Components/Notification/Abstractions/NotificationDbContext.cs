using Bootstrap.Components.Notification.Abstractions.Models.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;

namespace Bootstrap.Components.Notification.Abstractions
{
    [Obsolete]
    public class NotificationDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public NotificationDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }
    }
}