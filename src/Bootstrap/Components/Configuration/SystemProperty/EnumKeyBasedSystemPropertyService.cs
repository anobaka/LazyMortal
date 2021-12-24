using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Components.Orm.Infrastructures;
using Bootstrap.Extensions;
using Bootstrap.Models.Exceptions;
using Bootstrap.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace Bootstrap.Components.Configuration.SystemProperty
{
    public abstract class EnumKeyBasedSystemPropertyService<TKey> : SystemPropertyService where TKey : Enum
    {
        protected readonly Dictionary<TKey, string> KeysMap = new();
        protected readonly Dictionary<string, TKey> ReversedKeysMap = new();

        protected EnumKeyBasedSystemPropertyService(IServiceProvider serviceProvider) : base(
            serviceProvider)
        {
            foreach (var v in SpecificEnumUtils<TKey>.Values)
            {
                var attr = v.GetAttributeOfType<SystemPropertyKeyAttribute>();
                PropertiesCache[attr.Key] = attr;
                KeysMap[v] = attr.Key;
                ReversedKeysMap[attr.Key] = v;
            }
        }

        public async Task<SystemPropertyDto> GetByKey(TKey key, bool throwIfNotSet) =>
            await GetByKey(KeysMap[key], throwIfNotSet);

        protected override Task<SingletonResponse<string>> ValidateValue(string key, string value)
        {
            var enumKey = ReversedKeysMap[key];
            return ValidateValue(enumKey, value);
        }

        protected abstract Task<SingletonResponse<string>> ValidateValue(TKey key, string value);
    }
}