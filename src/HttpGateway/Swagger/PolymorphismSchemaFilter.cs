using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json.Serialization;

namespace HttpGateway.Swagger;

public sealed class PolymorphismSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        JsonPolymorphicAttribute? poly = context.Type.GetCustomAttribute<JsonPolymorphicAttribute>();
        if (poly is null) return;

        JsonDerivedTypeAttribute[] derived = context.Type.GetCustomAttributes<JsonDerivedTypeAttribute>().ToArray();
        if (derived.Length == 0) return;

        string discriminatorName = poly.TypeDiscriminatorPropertyName ?? "type";
        schema.Discriminator ??= new OpenApiDiscriminator { PropertyName = discriminatorName };

        if (!schema.Properties.ContainsKey(discriminatorName)) schema.Properties[discriminatorName] = new OpenApiSchema { Type = "string" };

        schema.Required.Add(discriminatorName);
        schema.OneOf ??= new List<OpenApiSchema>();
        foreach (JsonDerivedTypeAttribute d in derived)
        {
            Type dt = d.DerivedType;
            OpenApiSchema derivedSchema = context.SchemaGenerator.GenerateSchema(dt, context.SchemaRepository);
            string? schemaId = derivedSchema.Reference?.Id ?? context.SchemaRepository.Schemas.FirstOrDefault(kv => ReferenceEquals(kv.Value, derivedSchema)).Key;

            if (!string.IsNullOrEmpty(schemaId))
            {
                schema.OneOf.Add(new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = schemaId } });
                string disc = d.TypeDiscriminator?.ToString() ?? dt.Name;
                schema.Discriminator.Mapping[disc] = $"#/components/schemas/{schemaId}";
            }
        }
    }
}