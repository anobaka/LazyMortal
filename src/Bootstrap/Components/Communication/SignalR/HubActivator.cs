using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Components.Communication.SignalR
{
    public class HubActivator<THub> : IHubActivator<THub> where THub : Hub
    {
        private readonly IServiceProvider _serviceProvider;

        public HubActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public THub Create()
        {
            return _serviceProvider.GetRequiredService<THub>();
        }

        public void Release(THub hub)
        {
        }
    }
}