using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading;
using System;
using NPOI.OpenXmlFormats.Shared;

namespace Bootstrap.Components.Tasks;

public class PauseTokenSource
{
    private volatile TaskCompletionSource<bool>? _pauseTask;

    [MemberNotNullWhen(true, nameof(_pauseTask))]
    public bool IsPauseRequested => _pauseTask != null;

    public PauseToken Token => new PauseToken(this);

    public event Func<CancellationToken, Task>? OnPause;
    public event Func<CancellationToken, Task>? OnResume;

    public void Pause()
    {
        if (IsPauseRequested) return;

        _pauseTask = new TaskCompletionSource<bool>();
    }

    public void Resume()
    {
        if (!IsPauseRequested) return;

        var pt = _pauseTask;
        _pauseTask = null;
        pt?.TrySetResult(true);
    }

    private int _pauseTaskId = -1;

    internal async Task WaitWhilePausedAsync(CancellationToken ct)
    {
        var task = _pauseTask?.Task;
        if (task != null)
        {
            if (OnPause != null)
            {
                await OnPause(ct);
            }

            var cancelTask = Task.Delay(Timeout.Infinite, ct);
            await Task.WhenAny(cancelTask, task);
            ct.ThrowIfCancellationRequested();
            
            var currentId = task.Id;
            var originalId = Interlocked.CompareExchange(ref _pauseTaskId, currentId, _pauseTaskId);

            if (originalId != currentId && OnResume != null)
            {
                await OnResume(ct);
            }
        }
    }
}