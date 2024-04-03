using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bootstrap.Components.Doc.Swagger
{
    public class SwaggerCustomModelDocumentFilter : IDocumentFilter
    {
        protected virtual Assembly[] Assemblies { get; } = [];

        public void Apply(OpenApiDocument openapiDoc, DocumentFilterContext context)
        {
            var types = Assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<SwaggerCustomModelAttribute>() != null);

            foreach (var type in types)
            {
                context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
            }
        }
    }
}