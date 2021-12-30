using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models.Constants;

namespace Bootstrap.Components.Tasks.Progressor.Abstractions
{
    public interface IProgressDispatcher
    {
        Task Dispatch(string progressorKey, ProgressorEvent topic, object data);
    }
}
