using System;
using System.Collections.Concurrent;
using Bootstrap.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Components.DependencyInjection;

public abstract class ScopedService(IServiceProvider serviceProvider)
{
    private readonly ConcurrentDictionary<Type, object?> _serviceCache = new();

    protected T GetRequiredService<T>() where T : class =>
        (_serviceCache.GetOrAdd(SpecificTypeUtils<T>.Type, _ => serviceProvider.GetRequiredService<T>()) as T)!;
}