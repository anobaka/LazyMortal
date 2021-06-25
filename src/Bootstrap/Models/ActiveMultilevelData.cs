using System;

namespace Bootstrap.Models
{
    [Obsolete("Use ActiveMultilevelResource instead.")]
    public abstract class ActiveMultilevelData<TData> : MultilevelData<TData> where TData : ActiveMultilevelData<TData>
    {
        public bool Active { get; set; }
    }
}