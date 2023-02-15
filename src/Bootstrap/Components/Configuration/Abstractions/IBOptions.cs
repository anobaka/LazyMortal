using Microsoft.Extensions.Options;

namespace Bootstrap.Components.Configuration.Abstractions
{
    public interface IBOptions<TOptions>: IOptions<TOptions> where TOptions : class
    {

    }
}
