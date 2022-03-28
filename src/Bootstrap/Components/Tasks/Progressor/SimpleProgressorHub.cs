using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models.Constants;
using Bootstrap.Components.Tasks.Progressor.SignalR;
using Bootstrap.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Bootstrap.Components.Tasks.Progressor
{
    public class SimpleProgressorHub : Hub
    {
        private readonly Dictionary<string, IProgressor> _progressors;
        private readonly IProgressDispatcher _progressDispatcher;

        public SimpleProgressorHub(IEnumerable<IProgressor> progressors, IProgressDispatcher progressDispatcher)
        {
            _progressDispatcher = progressDispatcher;
            _progressors = progressors.ToDictionary(a => a.Key, a => a);
        }

        private IProgressor GetProgressor(string key)
        {
            return key.IsNullOrEmpty() || !_progressors.TryGetValue(key, out var p) ? null : p;
        }

        public async Task Invoke(string key, ProgressorClientAction action, string jsonParam = null)
        {
            try
            {
                var p = GetProgressor(key);
                if (p == null)
                {
                    throw new Exception($"Progressor with key {key} is not registered.");
                }

                switch (action)
                {
                    case ProgressorClientAction.Start:
                        object startParam;
                        if (p is ISignalRProgressor sp)
                        {
                            var param = jsonParam.IsNullOrEmpty()
                                ? null
                                : JsonConvert.DeserializeObject(jsonParam, sp.JsonParamType);
                            startParam = await sp.ConvertToStartModel(param);
                        }
                        else
                        {
                            startParam = jsonParam;
                        }

                        await p.Start(startParam, new CancellationToken());
                        break;
                    case ProgressorClientAction.Stop:
                        await p.Stop();
                        break;
                    case ProgressorClientAction.Initialize:
                        await _progressDispatcher.Dispatch(p.Key, ProgressorEvent.StateChanged, p.State);
                        // if (p.State.Status == ProgressorStatus.Running)
                        {
                            await _progressDispatcher.Dispatch(p.Key, ProgressorEvent.ProgressChanged, p.Progress);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(action), action, null);
                }
            }
            catch (Exception e)
            {
                await _progressDispatcher.Dispatch(key, ProgressorEvent.ErrorOccurred, e.Message);
            }
        }
    }
}