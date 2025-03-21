using System.Threading;
using System.Threading.Tasks;

namespace Bootstrap.Components.Tasks;

public readonly struct PauseToken
{
    public static PauseToken None => new PauseToken();
    private readonly PauseTokenSource _source;

    internal PauseToken(PauseTokenSource source)
    {
        _source = source;
    }

    public Task WaitWhilePausedAsync()
    {
        return _source?.WaitWhilePausedAsync() ?? Task.CompletedTask;
    }
}