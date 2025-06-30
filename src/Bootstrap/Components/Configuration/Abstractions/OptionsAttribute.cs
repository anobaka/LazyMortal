using System;

namespace Bootstrap.Components.Configuration.Abstractions
{
    /// <summary>
    /// One root configuration has its own configuration entity(file, cache key, etc).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OptionsAttribute : Attribute
    {
        public string? FileKey { get; set; }
        public string? OptionsKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsKey">
        /// Key of options. If it's not set, it will be the type name trimming end with 'Options'.
        /// </param>
        /// /// <param name="fileKey">
        /// The configuration file will be saved as {key}.{ext}. If it's not set, it will be word-separated, lowercased and '-'-joined <see cref="OptionsKey"/>
        /// </param>
        public OptionsAttribute(string? optionsKey = null, string? fileKey = null)
        {
            OptionsKey = optionsKey;
            FileKey = fileKey;
        }
    }
}
