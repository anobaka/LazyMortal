using Bootstrap.Components.Tasks.Progressor.Abstractions.Models.Constants;

namespace Bootstrap.Components.Tasks.Progressor.Abstractions
{
    public static class ProgressorExtensions
    {
        public static bool CanStart(this ProgressorStatus status)
        {
            switch (status)
            {
                case ProgressorStatus.Idle:
                case ProgressorStatus.Complete:
                case ProgressorStatus.Suspended:
                    return true;
                case ProgressorStatus.Running:
                default:
                    return false;
            }
        }
    }
}