using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Bootstrap.Components.Tasks;

public class PauseTokenSource(CancellationToken ct)
{
    private volatile TaskCompletionSource<bool>? _pauseTask;

    [MemberNotNullWhen(true, nameof(_pauseTask))]
    public bool IsPauseRequested => _pauseTask != null;

    public PauseToken Token => new PauseToken(this);

    public event Func<Task>? OnWaitPauseStart;
    public event Func<Task>? OnWaitPauseEnd;

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

    internal async Task WaitWhilePausedAsync()
    {
        if (IsPauseRequested)
        {
            if (OnWaitPauseStart != null)
            {
                await OnWaitPauseStart();
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var cancelTask = Task.Delay(Timeout.Infinite, cts.Token);
            var completedTask = await Task.WhenAny(cancelTask, _pauseTask.Task);
            await cts.CancelAsync();
            if (completedTask == cancelTask)
            {
                throw new OperationCanceledException(ct);
            }

            if (OnWaitPauseEnd != null)
            {
                await OnWaitPauseEnd();
            }
        }
    }
}