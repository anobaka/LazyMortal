namespace Bootstrap.Models
{
    public abstract class ActiveMultilevelResource<TResource> : MultilevelResource<TResource> where TResource : ActiveMultilevelResource<TResource>
    {
        public virtual bool Active { get; set; }
    }
}