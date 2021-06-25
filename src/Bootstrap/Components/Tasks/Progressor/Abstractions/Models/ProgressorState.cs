using Bootstrap.Components.Tasks.Progressor.Abstractions.Models.Constants;

namespace Bootstrap.Components.Tasks.Progressor.Abstractions.Models
{
    public class ProgressorState
    {
        public ProgressorStatus Status { get; set; } = ProgressorStatus.Idle;
        public string Message { get; set; }
    }
}
