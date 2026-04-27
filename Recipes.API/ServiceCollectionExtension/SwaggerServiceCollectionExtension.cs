using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Recipes.API.ServiceCollectionExtension;

public static class SwaggerServiceCollectionExtension
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token"
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
            options.OperationFilter<MultipartFormDataOperationFilter>();
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerWithAuth(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.OAuthClientId("swagger-ui-client");
            options.OAuthClientSecret("");
            options.OAuthRealm("");
            options.OAuthAppName("Swagger UI");
            options.OAuthScopes(string.Empty);
            options.OAuthUsePkce();
        });

        return app;
    }
}
