using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Recipes.API;

public class MultipartFormDataOperationFilter : IOperationFilter
{
    private const string MultipartFormData = "multipart/form-data";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formModelType = GetFormModelType(context);

        if (formModelType == null)
            return;

        var properties = formModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        if (properties.Length == 0)
            return;

        var requestBody = operation.RequestBody ??= new OpenApiRequestBody();
        requestBody.Content![MultipartFormData] = new OpenApiMediaType
        {
            Schema = CreateSchema(properties)
        };
    }

    private static Type? GetFormModelType(OperationFilterContext context)
    {
        var methodParameter = context.MethodInfo
            .GetParameters()
            .FirstOrDefault(parameter => parameter.GetCustomAttribute<FromFormAttribute>() != null);

        if (methodParameter != null)
            return methodParameter.ParameterType;

        return context.ApiDescription.ParameterDescriptions
            .FirstOrDefault(parameter => IsFormModel(parameter.Source) && IsComplexFormModel(parameter.Type))
            ?.Type;
    }

    private static bool IsFormModel(BindingSource? source)
    {
        return source == BindingSource.Form || source == BindingSource.FormFile;
    }

    private static bool IsComplexFormModel(Type? type)
    {
        if (type == null)
            return false;

        return type != typeof(string)
               && !type.IsPrimitive
               && !typeof(IFormFile).IsAssignableFrom(type)
               && !typeof(IFormFileCollection).IsAssignableFrom(type);
    }

    private static OpenApiSchema CreateSchema(IEnumerable<PropertyInfo> properties)
    {
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>(),
            Required = new HashSet<string>()
        };

        foreach (var property in properties)
        {
            var propertySchema = CreatePropertySchema(property);
            schema.Properties.Add(property.Name, propertySchema);

            if (property.GetCustomAttribute<RequiredAttribute>() != null)
                schema.Required.Add(property.Name);
        }

        return schema;
    }

    private static OpenApiSchema CreatePropertySchema(PropertyInfo property)
    {
        if (typeof(IFormFile).IsAssignableFrom(property.PropertyType))
            return CreateFileSchema();

        if (typeof(IFormFileCollection).IsAssignableFrom(property.PropertyType))
        {
            return new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Items = CreateFileSchema()
            };
        }

        if (IsJsonStringProperty(property))
        {
            return new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Description = "JSON string.",
                Example = CreateJsonStringExample(property.Name)
            };
        }

        return CreatePrimitiveSchema(property.PropertyType);
    }

    private static OpenApiSchema CreateFileSchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.String,
            Format = "binary"
        };
    }

    private static bool IsJsonStringProperty(PropertyInfo property)
    {
        return property.PropertyType == typeof(string)
               && (property.Name.EndsWith("Json", StringComparison.OrdinalIgnoreCase)
                   || property.Name.Equals("ImageIdsToDelete", StringComparison.OrdinalIgnoreCase));
    }

    private static JsonNode? CreateJsonStringExample(string propertyName)
    {
        if (propertyName.Equals("IngredientsJson", StringComparison.OrdinalIgnoreCase))
        {
            return JsonValue.Create(
                "[{\"ingredientId\":\"00000000-0000-0000-0000-000000000000\",\"weight\":100,\"alternativeWeight\":\"1 cup\"}]");
        }

        if (propertyName.Equals("ImageIdsToDelete", StringComparison.OrdinalIgnoreCase))
            return JsonValue.Create("[\"00000000-0000-0000-0000-000000000000\"]");

        return null;
    }

    private static OpenApiSchema CreatePrimitiveSchema(Type propertyType)
    {
        var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (type == typeof(bool))
            return new OpenApiSchema { Type = JsonSchemaType.Boolean };

        if (type == typeof(int) || type == typeof(long))
            return new OpenApiSchema
                { Type = JsonSchemaType.Integer, Format = type == typeof(long) ? "int64" : "int32" };

        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            return new OpenApiSchema
                { Type = JsonSchemaType.Number, Format = type == typeof(float) ? "float" : "double" };

        if (type == typeof(Guid))
            return new OpenApiSchema { Type = JsonSchemaType.String, Format = "uuid" };

        if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
            return new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time" };

        if (type == typeof(TimeSpan))
            return new OpenApiSchema { Type = JsonSchemaType.String, Example = JsonValue.Create("00:30:00") };

        return new OpenApiSchema { Type = JsonSchemaType.String };
    }
}