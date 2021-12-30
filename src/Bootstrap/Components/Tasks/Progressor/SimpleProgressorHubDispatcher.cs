using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models.Constants;
using Microsoft.AspNetCore.SignalR;

namespace Bootstrap.Components.Tasks.Progressor
{
    public class SimpleProgressorHubDispatcher : IProgressDispatcher
    {
        private readonly IHubContext<SimpleProgressorHub> _hub;

        public SimpleProgressorHubDispatcher(IHubContext<SimpleProgressorHub> hub)
        {
            _hub = hub;
        }

        public Task Dispatch(string progressorKey, ProgressorEvent topic, object data)
        {
            return _hub.Clients.All.SendAsync(topic.ToString(), progressorKey, data);
        }
    }
}