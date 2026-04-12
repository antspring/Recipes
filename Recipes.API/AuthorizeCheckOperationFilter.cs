using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Recipes.API;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("#/components/securitySchemes/Bearer"),
                    new List<string>()
                }
            }
        };
    }
}