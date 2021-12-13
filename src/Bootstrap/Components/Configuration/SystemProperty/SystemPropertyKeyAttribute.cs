using System;

namespace Bootstrap.Components.Configuration.SystemProperty
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SystemPropertyKeyAttribute : Attribute
    {
        public virtual string Key { get; set; }
        public string Description { get; set; }
        public bool RestartRequired { get; set; }
        public string Example { get; set; }
        public bool Required { get; set; }
    }
}