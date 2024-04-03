using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bootstrap.Components.Doc.Swagger.ApiVisibility;
using Bootstrap.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bootstrap.Components.Doc.Swagger
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Common configuration.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="docs"></param>
        private static void Configure<TCustomModelDocumentFilter>(this SwaggerGenOptions t,
            IEnumerable<KeyValuePair<string, OpenApiInfo>> docs)
            where TCustomModelDocumentFilter : SwaggerCustomModelDocumentFilter
        {
            if (docs != null)
            {
                foreach (var kv in docs)
                {
                    t.SwaggerDoc(kv.Key, kv.Value);
                }
            }

            t.DescribeAllParametersInCamelCase();

            t.DocumentFilter<EnumSchemaFilter>();
            t.SchemaFilter<EnumSchemaFilter>();
            t.OperationFilter<EnumSchemaFilter>();

            t.EnableAnnotations();

            t.CustomSchemaIds(type => type.ToString());
            t.DocumentFilter<TCustomModelDocumentFilter>();
        }

        public static IServiceCollection
            AddBootstrapSwaggerGen<TCustomModelDocumentFilter>(this IServiceCollection services, string docName,
                string docTitle) where TCustomModelDocumentFilter : SwaggerCustomModelDocumentFilter =>
            services.AddBootstrapSwaggerGen<TCustomModelDocumentFilter>(new Dictionary<string, OpenApiInfo>
                {{docName, new OpenApiInfo {Version = "v1", Title = docTitle}}});

        public static IServiceCollection AddBootstrapSwaggerGen<TCustomModelDocumentFilter>(
            this IServiceCollection services,
            IEnumerable<KeyValuePair<string, OpenApiInfo>> docs)
            where TCustomModelDocumentFilter : SwaggerCustomModelDocumentFilter
        {
            return services.AddSwaggerGen(t => { Configure<TCustomModelDocumentFilter>(t, docs); });
        }

        public static IServiceCollection AddBootstrapSwaggerGen<TApiVisibleAttribute, TApiVisibleRealm,
            TCustomModelDocumentFilter>(
            this IServiceCollection services,
            IEnumerable<KeyValuePair<string, OpenApiInfo>> docs) where TApiVisibleRealm : Enum
            where TApiVisibleAttribute : Attribute, IApiVisibilityAttribute<TApiVisibleRealm>
            where TCustomModelDocumentFilter : SwaggerCustomModelDocumentFilter
        {
            return services.AddSwaggerGen(t =>
            {
                Configure<TCustomModelDocumentFilter>(t, docs);

                t.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;

                    var apiVisibility = methodInfo.GetCustomAttribute<TApiVisibleAttribute>(true);

                    if (apiVisibility == null || apiVisibility.Realms?.Any() != true)
                    {
                        return true;
                    }

                    if (apiVisibility.Realms?.Any(r => Convert.ToInt32(r) == 0) == true)
                    {
                        return false;
                    }

                    var docNames = new List<string>();
                    var enumDocs = SpecificEnumUtils<TApiVisibleRealm>.Values
                        .ToDictionary(r => r,
                            r => r.GetAttributeOfType<SwaggerDocAttribute>()?.DocName)
                        .Where(r => !string.IsNullOrEmpty(r.Value)).ToDictionary(r => r.Key, r => r.Value);
                    foreach (var r in apiVisibility.Realms)
                    {
                        foreach (var sr in SpecificEnumUtils<TApiVisibleRealm>.Values)
                        {
                            if (r.HasFlag(sr))
                            {
                                if (enumDocs.TryGetValue(sr, out var d))
                                {
                                    docNames.Add(d);
                                }
                            }
                        }

                        ;
                    }

                    return docNames.Contains(docName);
                });
            });
        }
    }
}