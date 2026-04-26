namespace Recipes.API.Helpers;

public static class AuthEndpointErrorHelper
{
    public static IResult BadRequest(ArgumentException exception)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
}
