﻿using System;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bootstrap.Components.Doc.Swagger;
class RequiredNotNullableSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
        {
            return;
        }

        FixNullableProperties(schema, context);

        var notNullableProperties = schema
            .Properties
            .Where(x => !x.Value.Nullable && !schema.Required.Contains(x.Key))
            .ToList();

        foreach (var property in notNullableProperties)
        {
            schema.Required.Add(property.Key);
        }

        // Use undefined instead of nullable.
        var nullableProperties = schema.Properties
            .Where(x => x.Value.Nullable)
            .Select(x => x.Key)
            .ToList();

        foreach (var nullableProp in nullableProperties)
        {
            schema.Properties[nullableProp].Nullable = false;
        }
    }

    /// <summary>
    /// Option "SupportNonNullableReferenceTypes" not working with complex types ({ "type": "object" }), 
    /// so they always have "Nullable = false",
    /// see method "SchemaGenerator.GenerateSchemaForMember"
    /// </summary>
    private static void FixNullableProperties(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var property in schema.Properties)
        {
            if (property.Value.Reference != null)
            {
                var field = context.Type
                    .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x =>
                        string.Equals(x.Name, property.Key, StringComparison.InvariantCultureIgnoreCase));

                if (field != null)
                {
                    var fieldType = field switch
                    {
                        FieldInfo fieldInfo => fieldInfo.FieldType,
                        PropertyInfo propertyInfo => propertyInfo.PropertyType,
                        _ => throw new NotSupportedException(),
                    };

                    property.Value.Nullable = fieldType.IsValueType
                        ? Nullable.GetUnderlyingType(fieldType) != null
                        : !field.IsNonNullableReferenceType();
                }
            }
        }
    }
}