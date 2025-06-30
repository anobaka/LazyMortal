using Bootstrap.Components.Configuration.SystemProperty.Attributes;
using System;

namespace Bootstrap.Components.Configuration.SystemProperty.Models.Dtos
{
    [Obsolete]
    public class SystemPropertyDto : Entities.SystemProperty
    {
        public bool RestartRequired { get; set; }
        public SystemPropertyKeyAttribute Properties { get; set; }
    }
}