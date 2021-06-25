namespace Bootstrap.Components.Tasks.Progressor.Abstractions.Models
{
    public class ProgressorProgress
    {
        public virtual double Percentage { get; set; }
        public virtual long ElapsedMilliseconds { get; set; }
    }
}
