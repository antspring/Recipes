using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipes.API.Helpers;
using Recipes.Application.Services.Interfaces;
using MiniValidation;
using Recipes.API.DTO.Requests.User;
using Recipes.API.DTO.Responses.User;

namespace Recipes.API.Endpoints;

public static class AuthEndpoints
{
    private const string RefreshTokenCookieName = "refreshToken";

    public static void MapAuthEndpoints(this WebApplication app)
    {
        var authEndpoints = app.MapGroup("/auth");

        authEndpoints.MapPost("/register", async Task<IResult> (
                [FromForm] RegisterUserRequest request,
                IAuthService authService,
                HttpContext httpContext) =>
            {
                if (!MiniValidator.TryValidate(request, out var errors))
                {
                    return Results.ValidationProblem(errors);
                }

                var userAgent = GetUserAgent(httpContext);

                try
                {
                    var userAuthDto = await authService.Register(await request.ToCreateUserDtoAsync(), userAgent);
                    var userResponse = new UserResponse(userAuthDto);

                    AppendRefreshTokenCookie(httpContext, userResponse.RefreshToken);

                    return Results.Ok(userResponse);
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique") ?? false)
                {
                    return AuthEndpointErrorHelper.UniqueViolation("User with this email or username already exists");
                }
            })
            .DisableAntiforgery()
            .AllowAnonymous();

        authEndpoints.MapPost("/login", async Task<IResult> (
                [FromBody] LoginUserRequest request,
                IAuthService authService,
                HttpContext httpContext) =>
            {
                if (!MiniValidator.TryValidate(request, out var errors))
                {
                    return Results.ValidationProblem(errors);
                }

                try
                {
                    var userAgent = GetUserAgent(httpContext);

                    var userAuthDto = await authService.Login(request.ToLoginUserDto(userAgent));
                    var userResponse = new UserResponse(userAuthDto);

                    AppendRefreshTokenCookie(httpContext, userResponse.RefreshToken);

                    return Results.Ok(userResponse);
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique") ?? false)
                {
                    return AuthEndpointErrorHelper.UniqueViolation("Invalid credentials");
                }
            })
            .AllowAnonymous();

        authEndpoints.MapPost("/refresh", async Task<IResult> (
                IAuthService authService,
                HttpContext httpContext) =>
            {
                var refreshToken = httpContext.Request.Cookies[RefreshTokenCookieName];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Results.BadRequest(new { error = "Refresh token not provided" });
                }

                var userAgent = GetUserAgent(httpContext);

                try
                {
                    var userAuthDto = await authService.UpdateToken(refreshToken, userAgent);
                    var userResponse = new UserResponse(userAuthDto);

                    AppendRefreshTokenCookie(httpContext, userResponse.RefreshToken);

                    return Results.Ok(userResponse);
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
            })
            .AllowAnonymous();

        authEndpoints.MapPatch("/profile", async Task<IResult> (
                [FromForm] UpdateUserRequest request,
                IAuthService authService,
                HttpContext httpContext) =>
            {
                if (!TryGetUserId(httpContext, out var userId))
                {
                    return Results.Unauthorized();
                }

                var userAgent = GetUserAgent(httpContext);

                try
                {
                    var updateUserDto = await request.ToUpdateUserDtoAsync();
                    var userAuthDto = await authService.UpdateUserAsync(userId, updateUserDto, userAgent);
                    var userResponse = new UserResponse(userAuthDto);

                    AppendRefreshTokenCookie(httpContext, userResponse.RefreshToken);

                    return Results.Ok(userResponse);
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
            })
            .DisableAntiforgery()
            .RequireAuthorization();

        authEndpoints.MapDelete("/avatar", async Task<IResult> (
                IAuthService authService,
                HttpContext httpContext) =>
            {
                if (!TryGetUserId(httpContext, out var userId))
                {
                    return Results.Unauthorized();
                }

                try
                {
                    var userAuthDto = await authService.DeleteAvatarAsync(userId);
                    var userResponse = new UserResponse(userAuthDto);

                    return Results.Ok(userResponse);
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
            })
            .RequireAuthorization();
    }

    private static string? GetUserAgent(HttpContext httpContext)
    {
        return httpContext.Request.Headers.UserAgent.ToString();
    }

    private static bool TryGetUserId(HttpContext httpContext, out Guid userId)
    {
        var userIdValue = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdValue, out userId);
    }

    private static void AppendRefreshTokenCookie(HttpContext httpContext, string refreshToken)
    {
        httpContext.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true
        });
    }
}
