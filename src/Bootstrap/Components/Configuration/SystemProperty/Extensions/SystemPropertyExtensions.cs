using System;
using Bootstrap.Components.Configuration.SystemProperty.Attributes;
using Bootstrap.Components.Configuration.SystemProperty.Models.Dtos;
using Bootstrap.Components.Configuration.SystemProperty.Services;
using Bootstrap.Components.Orm.Extensions;
using Bootstrap.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrap.Components.Configuration.SystemProperty.Extensions
{
    public static class SystemPropertyExtensions
    {
        public static SystemPropertyDto ToDto(this Models.Entities.SystemProperty sp, SystemPropertyKeyAttribute properties = null,
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