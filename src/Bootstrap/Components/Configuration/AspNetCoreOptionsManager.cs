using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bootstrap.Components.Configuration
{
    public class AspNetCoreOptionsManager<TOptions> : JsonOptionsManager<TOptions> where TOptions : class, new()
    {
        private TOptions _options;
        private readonly ConcurrentDictionary<Action<TOptions>, bool> _listeners = new();
        private readonly ILogger<AspNetCoreOptionsManager<TOptions>> _logger;

        public AspNetCoreOptionsManager(string filePath, string key, IOptionsMonitor<TOptions> monitor,
            ILogger<AspNetCoreOptionsManager<TOptions>> logger) : base(
            filePath, key)
        {
            _logger = logger;
            _options = monitor.CurrentValue;

            monitor.OnChange(OnChange);
            OnChange(@new => { _options = @new; });
        }

        public override TOptions Value => _options;

        private void OnChange(TOptions newOptions)
        {
            // Console.WriteLine($"{typeof(TOptions).Name} fake changed");
            if (JsonConvert.SerializeObject(_options) != JsonConvert.SerializeObject(newOptions))
            {
                _options = newOptions;
                foreach (var l in _listeners.Keys)
                {
                    try
                    {
                        l(newOptions);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Error when calling listener: {e.Message}");
                    }
                }
            }
        }

        public IDisposable OnChange(Action<TOptions> handler)
        {
            _listeners[handler] = true;
            var listener = new Listener(handler, this);
            return listener;
        }

        private void Dispose(Action<TOptions> handler)
        {
            _listeners.Remove(handler, out _);
        }

        private class Listener : IDisposable
        {
            private readonly Action<TOptions> _handler;
            private readonly AspNetCoreOptionsManager<TOptions> _om;

            public Listener(Action<TOptions> handler, AspNetCoreOptionsManager<TOptions> om)
            {
                _handler = handler;
                _om = om;
            }

            public void Dispose()
            {
                _om.Dispose(_handler);
            }
        }
    }
}