using System.Threading.Tasks;
using Bootstrap.Components.Configuration.Abstractions;

namespace Bootstrap.Components.Configuration;

public class MemoryOptionsManager<TOptions> : AbstractOptionsManager<TOptions> where TOptions : class, new()
{
    private TOptions _options;
    public override TOptions Value => _options;

    public override void Save(TOptions options)
    {
        _options = options;
    }

    public override async Task SaveAsync(TOptions options)
    {
        _options = options;
    }
}