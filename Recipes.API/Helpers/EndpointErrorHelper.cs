namespace Recipes.API.Helpers;

public static class EndpointErrorHelper
{
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
