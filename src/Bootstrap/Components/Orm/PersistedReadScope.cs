using System;
using System.Threading;

namespace Bootstrap.Components.Orm;

/// <summary>
/// Opt-in reads of committed database values, without consuming or changing the shared cache.
/// Expression predicates still execute in the database. This is not a database transaction;
/// callers requiring a snapshot must provide transaction/revision coordination themselves.
/// </summary>
public sealed class PersistedReadScope : IDisposable
{
    private static readonly AsyncLocal<PersistedReadScope?> Current = new();
    private readonly PersistedReadScope? _previous;
    private bool _disposed;
    public static bool IsActive => Current.Value != null;
    private PersistedReadScope() { _previous = Current.Value; Current.Value = this; }
    public static PersistedReadScope Enter() => new();
    public void Dispose()
    {
        if (_disposed) return;
        Current.Value = _previous;
        _disposed = true;
    }
}
