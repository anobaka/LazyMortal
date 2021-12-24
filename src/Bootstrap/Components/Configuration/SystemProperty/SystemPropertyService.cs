﻿using System;
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
    public abstract class SystemPropertyService : ResourceService<SystemPropertyDbContext, SystemProperty, string>
    {
        protected readonly Dictionary<string, SystemPropertyKeyAttribute> PropertiesCache = new();
        protected readonly HashSet<string> PendingRestartKeys = new();

        protected SystemPropertyService(IServiceProvider serviceProvider) : base(
            serviceProvider)
        {
        }

        public async Task<List<SystemPropertyDto>> GetAll(Expression<Func<SystemProperty, bool>> selector = null)
        {
            var ps = await base.GetAll(selector);
            var results = PropertiesCache.Select(t => new SystemPropertyDto
            {
                Key = t.Value.Key,
                Value = ps.FirstOrDefault(a => a.Key == t.Value.Key)?.Value,
                Properties = t.Value,
                RestartRequired = PendingRestartKeys.Contains(t.Key)
            }).ToList();
            results.AddRange(ps.Where(t => results.All(a => a.Key != t.Key)).Select(t => t.ToDto()));

            return results;
        }

        private SystemPropertyDto _toDto(SystemProperty sp)
        {
            if (sp == null)
            {
                return null;
            }

            return sp.ToDto(PropertiesCache.TryGetValue(sp.Key, out var v) ? v : null,
                PendingRestartKeys.Contains(sp.Key));
        }

        public async Task<SystemPropertyDto> GetByKey(string key, bool throwIfNotSet)
        {
            var p = await base.GetByKey(PropertiesCache[key].Key);
            return p?.Value == null && throwIfNotSet
                ? throw new NotInitializedException(nameof(SystemProperty),
                    $"{key} not {(p == null ? "found" : "set")}")
                : _toDto(p);
        }

        private void _tryAddToPendingChanges(string key)
        {
            if (PropertiesCache.TryGetValue(key, out var a) && a.RestartRequired)
            {
                PendingRestartKeys.Add(key);
            }
        }

        public async Task<BaseResponse> AddOrUpdate(string key, string value)
        {
            var validateResult = await ValidateValue(key, value);
            if (validateResult.Code != 0)
            {
                return validateResult;
            }

            value = validateResult.Data ?? value;

            var p = await GetByKey(key);
            if (p == null)
            {
                var np = new SystemProperty {Key = key, Value = value};
                await Add(np);
                _tryAddToPendingChanges(key);
            }
            else
            {
                if (p.Value != value)
                {
                    _tryAddToPendingChanges(key);
                    p.Value = value;
                    await Update(p);
                }
            }

            return BaseResponseBuilder.Ok;
        }

        protected abstract Task<SingletonResponse<string>> ValidateValue(string key, string value);
    }
}