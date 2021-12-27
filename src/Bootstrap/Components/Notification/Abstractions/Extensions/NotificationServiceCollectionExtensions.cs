using System;
using Bootstrap.Components.Notification.Abstractions.Services;
using Bootstrap.Components.Orm.Extensions;
using Bootstrap.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bootstrap.Components.Notification.Abstractions.Extensions
{
    public static class NotificationServiceCollectionExtensions
    {
        public static IServiceCollection AddBootstrapNotificationService<TDbContextImplementation>(
            this IServiceCollection services, Action<DbContextOptionsBuilder> configure = null)
            where TDbContextImplementation : NotificationDbContext
        {
            return services.AddSingleServiceBootstrapServices<NotificationDbContext, TDbContextImplementation>(
                SpecificTypeUtils<MessageService>.Type, configure);
        }
    }
}