using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipes.API.DTO.Requests;
using Recipes.API.DTO.Responses;
using Recipes.Application.Services.Interfaces;
using MiniValidation;

namespace Recipes.API.Endpoints;

public static class AuthEndpoints
{
    private static IResult? CheckAnonymousAccess(HttpContext httpContext)
    {
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            return Results.BadRequest(new { error = "User is already authenticated" });
        }
        return null;
    }

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

            var anonymousCheck = CheckAnonymousAccess(httpContext);
            if (anonymousCheck != null) return anonymousCheck;

            var userAgent = httpContext.Request.Headers["User-Agent"];

            try
            {
                var userAuthDto = await authService.Register(request.ToCreateUserDto(), userAgent);
                var userResponse = new UserResponse(userAuthDto);

                httpContext.Response.Cookies.Append("refreshToken", userResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });

                return Results.Ok(userResponse);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique") ?? false)
            {
                return Results.BadRequest(
                    new
                    {
                        error = "User with this email or username already exists"
                    });
            }
        })
        .DisableAntiforgery();

        authEndpoints.MapPost("/login", async Task<IResult> (
            [FromBody] LoginUserRequest request,
            IAuthService authService,
            HttpContext httpContext) =>
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            var anonymousCheck = CheckAnonymousAccess(httpContext);
            if (anonymousCheck != null) return anonymousCheck;

            try
            {
                var userAgent = httpContext.Request.Headers["User-Agent"];
                
                var userAuthDto = await authService.Login(request.ToLoginUserDto(userAgent));
                var userResponse = new UserResponse(userAuthDto);

                httpContext.Response.Cookies.Append("refreshToken", userResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });

                return Results.Ok(userResponse);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique") ?? false)
            {
                return Results.BadRequest(new { error = "Invalid credentials" });
            }
        });

        authEndpoints.MapPost("/refresh", async Task<IResult> (
            IAuthService authService,
            HttpContext httpContext) =>
        {
            var anonymousCheck = CheckAnonymousAccess(httpContext);
            if (anonymousCheck != null) return anonymousCheck;

            var refreshToken = httpContext.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Results.BadRequest(new { error = "Refresh token not provided" });
            }

            var userAgent = httpContext.Request.Headers["User-Agent"];

            try
            {
                var userAuthDto = await authService.UpdateToken(refreshToken, userAgent);
                var userResponse = new UserResponse(userAuthDto);

                httpContext.Response.Cookies.Append("refreshToken", userResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });

                return Results.Ok(userResponse);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        authEndpoints.MapPatch("/profile", async Task<IResult> (
                [FromForm] UpdateUserRequest request,
                IAuthService authService,
                HttpContext httpContext) =>
            {
                var anonymousCheck = CheckAnonymousAccess(httpContext);
                if (anonymousCheck != null) return anonymousCheck;

                var userId = httpContext.User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                {
                    return Results.BadRequest(new { error = "User ID not found" });
                }

                var userAgent = httpContext.Request.Headers["User-Agent"];

                try
                {
                    var updateUserDto = request.ToUpdateUserDto();
                    var userAuthDto = await authService.UpdateUserAsync(userIdGuid, updateUserDto, userAgent);
                    var userResponse = new UserResponse(userAuthDto);

                    httpContext.Response.Cookies.Append("refreshToken", userResponse.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        Secure = true
                    });

                    return Results.Ok(userResponse);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .DisableAntiforgery();

        authEndpoints.MapDelete("/avatar", async Task<IResult> (
            IAuthService authService,
            HttpContext httpContext) =>
        {
            var anonymousCheck = CheckAnonymousAccess(httpContext);
            if (anonymousCheck != null) return anonymousCheck;

            var userId = httpContext.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
            {
                return Results.BadRequest(new { error = "User ID not found" });
            }

            try
            {
                var userAuthDto = await authService.DeleteAvatarAsync(userIdGuid);
                var userResponse = new UserResponse(userAuthDto);

                return Results.Ok(userResponse);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }
}