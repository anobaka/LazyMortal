using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models.Constants;

namespace Bootstrap.Components.Tasks.Progressor.Abstractions
{
    public interface IProgressNotifier
    {
        Task Send(ProcessorClientMethod topic, string key, object data);
    }
}
