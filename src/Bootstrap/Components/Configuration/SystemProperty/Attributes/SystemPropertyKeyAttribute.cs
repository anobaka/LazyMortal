﻿using System;

namespace Bootstrap.Components.Configuration.SystemProperty.Attributes
{
    [Obsolete]
    [AttributeUsage(AttributeTargets.Field)]
    public class SystemPropertyKeyAttribute : Attribute
    {
        public virtual string Key { get; set; }
        public virtual string Description { get; set; }
        public virtual bool RestartRequired { get; set; }
        public virtual string Example { get; set; }
        public virtual bool Required { get; set; }
    }
}