using System;
using System.ComponentModel.DataAnnotations;

namespace Bootstrap.Components.Configuration.SystemProperty.Models.Entities
{
    [Obsolete]
    public class SystemProperty
    {
        [Key] public string Key { get; set; }
        public string Value { get; set; }
    }
}