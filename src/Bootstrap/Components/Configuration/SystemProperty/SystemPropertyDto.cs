namespace Bootstrap.Components.Configuration.SystemProperty
{
    public class SystemPropertyDto : SystemProperty
    {
        public bool RestartRequired { get; set; }
        public SystemPropertyKeyAttribute Properties { get; set; }
    }
}