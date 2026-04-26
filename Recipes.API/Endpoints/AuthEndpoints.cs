using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipes.API.Helpers;
using Recipes.Application.Services.Interfaces;
using Recipes.API.DTO.Requests.User;
using Recipes.API.DTO.Responses.User;

namespace Recipes.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var authEndpoints = app.MapGroup("/auth");

        authEndpoints.MapPost("/register", async Task<IResult> (
                [FromForm] RegisterUserRequest request,
                IAuthService authService,
                HttpContext httpContext) =>
            {
                if (!AuthEndpointHelper.TryValidate(request, out var validationResult))
                    return validationResult!;

                try
                {
                    var userAgent = AuthEndpointHelper.GetUserAgent(httpContext);
                    var userAuthDto = await authService.Register(await request.ToCreateUserDtoAsync(), userAgent);
                    return AuthEndpointHelper.OkWithRefreshToken(httpContext, userAuthDto);
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
                if (!AuthEndpointHelper.TryValidate(request, out var validationResult))
                    return validationResult!;

                try
                {
                    var userAgent = AuthEndpointHelper.GetUserAgent(httpContext);
                    var userAuthDto = await authService.Login(request.ToLoginUserDto(userAgent));
                    return AuthEndpointHelper.OkWithRefreshToken(httpContext, userAuthDto);
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
                var refreshToken = AuthEndpointHelper.GetRefreshToken(httpContext);
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Results.BadRequest(new { error = "Refresh token not provided" });
                }

                try
                {
                    var userAgent = AuthEndpointHelper.GetUserAgent(httpContext);
                    var userAuthDto = await authService.UpdateToken(refreshToken, userAgent);
                    return AuthEndpointHelper.OkWithRefreshToken(httpContext, userAuthDto);
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

                try
                {
                    var userAgent = AuthEndpointHelper.GetUserAgent(httpContext);
                    var updateUserDto = await request.ToUpdateUserDtoAsync();
                    var userAuthDto = await authService.UpdateUserAsync(userId, updateUserDto, userAgent);
                    return AuthEndpointHelper.OkWithRefreshToken(httpContext, userAuthDto);
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
                    return Results.Ok(new UserResponse(userAuthDto));
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
            })
            .RequireAuthorization();
    }

    private static bool TryGetUserId(HttpContext httpContext, out Guid userId)
    {
        var userIdValue = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdValue, out userId);
    }
}
