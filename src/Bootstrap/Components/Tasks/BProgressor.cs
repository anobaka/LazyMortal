using System;
using System.Threading.Tasks;
using DotNext.Threading;

namespace Bootstrap.Components.Tasks;

public class BProgressor(Func<int, Task>? onChange) : IAsyncDisposable
{
    private Atomic<float> _progress;
    private Atomic<int> _lastReported;

    /// <summary>
    /// Concurrent scoped progressors are not supported.
    /// </summary>
    /// <param name="baseProgress"></param>
    /// <param name="totalProgressOfScope"></param>
    /// <returns></returns>
    public BProgressor CreateNewScope(float baseProgress, float totalProgressOfScope)
    {
        Func<int, Task>? scopedOnChange = null;
        if (onChange != null)
        {
            var unitProgress = totalProgressOfScope / 100;
            scopedOnChange = async p => { await Set(baseProgress + p * unitProgress); };
        }

        return new BProgressor(scopedOnChange);
    }

    public async Task Add(float add)
    {
        _progress.AccumulateAndGet(add, Add, out var np);
        var lr = _lastReported.Value;
        _lastReported.AccumulateAndGet((int) np, SetInt, out var nlr);

        if (nlr != lr && onChange != null)
        {
            await onChange(nlr);
        }
    }

    public async Task Set(float @new)
    {
        _progress.AccumulateAndGet(@new, SetFloat, out var np);
        var lr = _lastReported.Value;
        _lastReported.AccumulateAndGet((int) np, SetInt, out var nlr);

        if (nlr != lr && onChange != null)
        {
            await onChange(nlr);
        }
    }

    private static void Add(ref float current, in float delta)
    {
        current += delta;
    }

    private static void SetInt(ref int current, in int @new)
    {
        current = @new;
    }

    private static void SetFloat(ref float current, in float @new)
    {
        current = @new;
    }

    public async ValueTask DisposeAsync()
    {
        if (_lastReported.Value != 100)
        {
            await Set(100f);
        }
    }
}