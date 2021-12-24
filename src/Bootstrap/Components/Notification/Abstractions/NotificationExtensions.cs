using System;
using Bootstrap.Components.Orm.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bootstrap.Components.Notification.Abstractions
{
    public static class NotificationExtensions
    {
        public static IServiceCollection AddBootstrapNotificationService<TDbContextImplementation>(
            this IServiceCollection services, Action<DbContextOptionsBuilder> configure = null)
            where TDbContextImplementation : NotificationDbContext
        {
            services.TryAddScoped<MessageService>();
            return services.AddBootstrapNotificationService<MessageService, TDbContextImplementation>(configure);
        }

        public static IServiceCollection AddBootstrapNotificationService<TRegisteredServiceImplementation,
            TDbContextImplementation>(
            this IServiceCollection services, Action<DbContextOptionsBuilder> configure = null)
            where TRegisteredServiceImplementation : MessageService
            where TDbContextImplementation : NotificationDbContext
        {
            services.TryAddSingleton<MessageService, TRegisteredServiceImplementation>();
            services.AddBootstrapServices<NotificationDbContext, TDbContextImplementation>(configure);
            return services;
        }
    }
}