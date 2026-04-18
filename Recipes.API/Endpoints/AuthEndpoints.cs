using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipes.API.DTO.Requests;
using Recipes.API.DTO.Responses;
using Recipes.Application.Services.Interfaces;
using MiniValidation;
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
                if (!MiniValidator.TryValidate(request, out var errors))
                {
                    return Results.ValidationProblem(errors);
                }

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
            })
            .AllowAnonymous();

        authEndpoints.MapPost("/refresh", async Task<IResult> (
                IAuthService authService,
                HttpContext httpContext) =>
            {
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
            })
            .AllowAnonymous();

        authEndpoints.MapPatch("/profile", async Task<IResult> (
                [FromForm] UpdateUserRequest request,
                IAuthService authService,
                HttpContext httpContext) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                {
                    return Results.Unauthorized();
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
            .DisableAntiforgery()
            .RequireAuthorization();

        authEndpoints.MapDelete("/avatar", async Task<IResult> (
                IAuthService authService,
                HttpContext httpContext) =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                {
                    return Results.Unauthorized();
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
            })
            .RequireAuthorization();
    }
}