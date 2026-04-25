namespace Recipes.API.Helpers;

public static class EndpointErrorHelper
{
    public static IResult AuthBadRequest(ArgumentException exception)
    {
        return Results.BadRequest(new { error = exception.Message });
    }

    public static IResult AuthUniqueViolation(string message)
    {
        return Results.BadRequest(new { error = message });
    }

    public static IResult BadRequest(Exception exception)
    {
        return Results.BadRequest(exception.Message);
    }

    public static IResult NotFoundOrBadRequest(Exception exception)
    {
        return exception is ArgumentException argumentException
            ? Results.NotFound(argumentException.Message)
            : Results.BadRequest(exception.Message);
    }

    public static IResult ForbiddenNotFoundOrBadRequest(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => Results.Forbid(),
            ArgumentException argumentException => Results.NotFound(argumentException.Message),
            _ => Results.BadRequest(exception.Message)
        };
    }
}
