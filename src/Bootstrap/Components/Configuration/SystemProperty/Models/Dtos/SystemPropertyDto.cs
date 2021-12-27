using Bootstrap.Components.Configuration.SystemProperty.Attributes;

namespace Bootstrap.Components.Configuration.SystemProperty.Models.Dtos
{
    public class SystemPropertyDto : Entities.SystemProperty
    {
        public bool RestartRequired { get; set; }
        public SystemPropertyKeyAttribute Properties { get; set; }
    }
}