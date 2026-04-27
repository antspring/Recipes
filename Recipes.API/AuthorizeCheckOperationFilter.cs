using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Recipes.API;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .OfType<Microsoft.AspNetCore.Authorization.IAuthorizeData>()
            .Any();

        if (hasAuthorize)
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", context.Document),
                    new List<string>()
                }
            });
        }
    }
}