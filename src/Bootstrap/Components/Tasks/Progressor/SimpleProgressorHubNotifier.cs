using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models.Constants;
using Microsoft.AspNetCore.SignalR;

namespace Bootstrap.Components.Tasks.Progressor
{
    public class SimpleProgressorHubNotifier : IProgressNotifier
    {
        private readonly IHubContext<SimpleProgressorHub> _hub;

        public SimpleProgressorHubNotifier(IHubContext<SimpleProgressorHub> hub)
        {
            _hub = hub;
        }

        public Task Send(ProcessorClientMethod topic, string key, object data)
        {
            return _hub.Clients.All.SendAsync(topic.ToString(), key, data);
        }
    }
}