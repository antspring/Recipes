using MiniValidation;
using Recipes.API.DTO.Responses.User;
using Recipes.Application.DTO.User;

namespace Recipes.API.Helpers;

public static class AuthEndpointHelper
{
    private const string RefreshTokenCookieName = "refreshToken";

    public static bool TryValidate<T>(T request, out IResult? validationResult)
    {
        if (MiniValidator.TryValidate(request, out var errors))
        {
            validationResult = null;
            return true;
        }

        validationResult = Results.ValidationProblem(errors);
        return false;
    }

    public static string? GetRefreshToken(HttpContext httpContext)
    {
        return httpContext.Request.Cookies[RefreshTokenCookieName];
    }

    public static string? GetUserAgent(HttpContext httpContext)
    {
        return httpContext.Request.Headers.UserAgent.ToString();
    }

    public static IResult OkWithRefreshToken(HttpContext httpContext, UserAuthDto userAuthDto)
    {
        var userResponse = new UserResponse(userAuthDto);

        httpContext.Response.Cookies.Append(RefreshTokenCookieName, userResponse.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true
        });

        return Results.Ok(userResponse);
    }
}
