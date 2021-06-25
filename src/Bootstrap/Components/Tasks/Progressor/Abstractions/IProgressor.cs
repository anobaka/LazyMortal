using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models;

namespace Bootstrap.Components.Tasks.Progressor.Abstractions
{
    public interface IProgressor
    {
        string Key { get; }

        /// <summary>
        /// For public usage
        /// </summary>
        ProgressorState State { get; }
        object Progress { get; }
        Task Start(string jsonParams, CancellationToken ct);
        Task Stop();
    }
}