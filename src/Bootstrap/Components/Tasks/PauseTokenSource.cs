using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading;
using System;

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

        _pauseTask.TrySetResult(true);
        _pauseTask = null;
    }

    private int _pauseTaskId = -1;

    internal async Task WaitWhilePausedAsync(CancellationToken ct)
    {
        if (IsPauseRequested)
        {
            if (OnPause != null)
            {
                await OnPause(ct);
            }

            var cancelTask = Task.Delay(Timeout.Infinite, ct);
            await Task.WhenAny(cancelTask, _pauseTask.Task);
            ct.ThrowIfCancellationRequested();
            
            var currentId = _pauseTask.Task.Id;
            var originalId = Interlocked.CompareExchange(ref _pauseTaskId, currentId, _pauseTaskId);

            if (originalId != currentId && OnResume != null)
            {
                await OnResume(ct);
            }
        }
    }
}