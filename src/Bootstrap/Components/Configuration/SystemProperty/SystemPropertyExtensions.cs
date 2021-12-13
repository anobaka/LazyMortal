namespace Bootstrap.Components.Configuration.SystemProperty
{
    public static class SystemPropertyExtensions
    {
        public static SystemPropertyDto ToDto(this SystemProperty sp, SystemPropertyKeyAttribute properties = null,
            bool restartRequired = false)
        {
            if (sp == null)
            {
                return null;
            }

            return new SystemPropertyDto
            {
                Key = sp.Key,
                Value = sp.Value,
                Properties = properties,
                RestartRequired = restartRequired
            };
        }
    }
}
