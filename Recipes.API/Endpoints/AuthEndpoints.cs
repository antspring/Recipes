using Microsoft.AspNetCore.Mvc;
using Recipes.API.Helpers;
using Recipes.Application.Services.Interfaces;
using Recipes.API.DTO.Requests.User;
using Recipes.API.DTO.Responses.User;

namespace Recipes.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var authEndpoints = app.MapGroup("/auth").WithTags("Auth");

        authEndpoints.MapPost("/register/email-code", async Task<IResult> (
                [FromBody] SendRegistrationEmailCodeRequest request,
                IEmailVerificationService emailVerificationService) =>
            {
                if (!AuthEndpointHelper.TryValidate(request, out var validationResult))
                    return validationResult!;

                try
                {
                    await emailVerificationService.SendRegistrationCodeAsync(request.Email);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
            })
            .AllowAnonymous();

        authEndpoints.MapPost("/register", async Task<IResult> (
                [FromForm] RegisterUserRequest request,
                IAuthService authService,
                IImageUrlMapper imageUrlMapper,
                HttpContext httpContext) =>
            {
                if (!AuthEndpointHelper.TryValidate(request, out var validationResult))
                    return validationResult!;

                try
                {
                    var userAgent = AuthEndpointHelper.GetUserAgent(httpContext);
                    var createUserDto = await AuthRequestMapper.ToCreateUserDtoAsync(request);
                    var userAuthDto = await authService.Register(createUserDto, userAgent);
                    return AuthEndpointHelper.OkWithRefreshToken(httpContext, userAuthDto, imageUrlMapper);
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
            })
            .DisableAntiforgery()
            .AllowAnonymous();

        authEndpoints.MapPost("/login", async Task<IResult> (
                [FromBody] LoginUserRequest request,
                IAuthService authService,
                IImageUrlMapper imageUrlMapper,
                HttpContext httpContext) =>
            {
                if (!AuthEndpointHelper.TryValidate(request, out var validationResult))
                    return validationResult!;

                try
                {
                    var userAgent = AuthEndpointHelper.GetUserAgent(httpContext);
                    var loginUserDto = AuthRequestMapper.ToLoginUserDto(request, userAgent);
                    var userAuthDto = await authService.Login(loginUserDto);
                    return AuthEndpointHelper.OkWithRefreshToken(httpContext, userAuthDto, imageUrlMapper);
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
            })
            .AllowAnonymous();

        authEndpoints.MapPost("/refresh", async Task<IResult> (
                IAuthService authService,
                IImageUrlMapper imageUrlMapper,
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
                    return AuthEndpointHelper.OkWithRefreshToken(httpContext, userAuthDto, imageUrlMapper);
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
            })
            .AllowAnonymous();

        authEndpoints.MapPost("/logout", async Task<IResult> (
                IAuthService authService,
                HttpContext httpContext) =>
            {
                var refreshToken = AuthEndpointHelper.GetRefreshToken(httpContext);
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    await authService.LogoutAsync(refreshToken);
                }

                AuthEndpointHelper.DeleteRefreshToken(httpContext);
                return Results.NoContent();
            })
            .AllowAnonymous();

        authEndpoints.MapPatch("/profile", async Task<IResult> (
                [FromForm] UpdateUserRequest request,
                IAuthService authService,
                IImageUrlMapper imageUrlMapper,
                HttpContext httpContext) =>
            {
                if (!EndpointUserHelper.TryGetUserId(httpContext.User, out var userId))
                {
                    return Results.Unauthorized();
                }

                try
                {
                    var userAgent = AuthEndpointHelper.GetUserAgent(httpContext);
                    var updateUserDto = await AuthRequestMapper.ToUpdateUserDtoAsync(request);
                    var userAuthDto = await authService.UpdateUserAsync(userId, updateUserDto, userAgent);
                    return AuthEndpointHelper.OkWithRefreshToken(httpContext, userAuthDto, imageUrlMapper);
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
                IImageUrlMapper imageUrlMapper,
                HttpContext httpContext) =>
            {
                if (!EndpointUserHelper.TryGetUserId(httpContext.User, out var userId))
                {
                    return Results.Unauthorized();
                }

                try
                {
                    var user = await authService.DeleteAvatarAsync(userId);
                    return Results.Ok(new UserResponse(user, imageUrlMapper: imageUrlMapper));
                }
                catch (ArgumentException ex)
                {
                    return AuthEndpointErrorHelper.BadRequest(ex);
                }
            })
            .RequireAuthorization();
    }
}
