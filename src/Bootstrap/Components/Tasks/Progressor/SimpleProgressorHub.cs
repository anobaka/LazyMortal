using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models.Constants;
using Bootstrap.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Components.Tasks.Progressor
{
    public class SimpleProgressorHub : Hub
    {
        public static IServiceProvider ServiceProvider;

        protected IEnumerable<IProgressor> Progressors =>
            ServiceProvider.GetRequiredService<IEnumerable<IProgressor>>();

        private Dictionary<string, IProgressor> _progressors;

        private IProgressor GetProgressor(string key)
        {
            _progressors ??= Progressors.ToDictionary(a => a.Key, a => a);

            return key.IsNullOrEmpty() || !_progressors.TryGetValue(key, out var p) ? null : p;
        }

        public async Task Invoke(string key, ProgressorClientAction action, string jsonParams = null)
        {
            var p = GetProgressor(key);
            if (p == null)
            {
                await Clients.Caller.SendAsync(ProcessorClientMethod.NotRegistered.ToString(), key, $"Progressor with key {key} is not registered.");
                return;
            }
            switch (action)
            {
                case ProgressorClientAction.Start:
                    await p.Start(jsonParams, new CancellationToken());
                    break;
                case ProgressorClientAction.Stop:
                    await p.Stop();
                    break;
                case ProgressorClientAction.Initialize:
                    await Clients.Caller.SendAsync(ProcessorClientMethod.State.ToString(), p.Key, p.State);
                    if (p.State.Status == ProgressorStatus.Running)
                    {
                        await Clients.Caller.SendAsync(ProcessorClientMethod.Progress.ToString(), p.Key, p.Progress);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }
}