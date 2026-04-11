using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipes.API.DTO.Requests;
using Recipes.API.DTO.Responses;
using Recipes.Application.Services.Interfaces;

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
            [FromBody] RegisterUserRequest request,
            IAuthService authService,
            HttpContext httpContext) =>
        {
            // Проверка: эндпоинт доступен только для анонимных пользователей
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
        });

        authEndpoints.MapPost("/login", async Task<IResult> (
            [FromBody] LoginUserRequest request,
            IAuthService authService,
            HttpContext httpContext) =>
        {
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
    }
}