namespace Recipes.API.Helpers;

public static class AuthEndpointErrorHelper
{
    public static IResult BadRequest(ArgumentException exception)
    {
        return Results.BadRequest(new { error = exception.Message });
    }

    public static IResult UniqueViolation(string message)
    {
        return Results.BadRequest(new { error = message });
    }
}
