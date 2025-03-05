using Microsoft.Extensions.Options;

namespace Bootstrap.Components.Configuration.Abstractions
{
    public interface IBOptions<out TOptions>: IOptions<TOptions> where TOptions : class
    {

    }
}
