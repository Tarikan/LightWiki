using System.Linq;
using System.Reflection;
using LightWiki.Infrastructure.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LightWiki.Infrastructure.Web.Swagger;

public sealed class SwaggerExcludeSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null)
        {
            return;
        }

        var excludedProperties = context.Type.GetProperties()
            .Where(t => t.GetCustomAttribute<JsonIgnoreAttribute>() != null);

        foreach (PropertyInfo excludedProperty in excludedProperties)
        {
            var propertyName = excludedProperty.Name.ToCamelCase();

            if (schema.Properties.ContainsKey(propertyName))
            {
                schema.Properties.Remove(propertyName);
            }
        }
    }
}