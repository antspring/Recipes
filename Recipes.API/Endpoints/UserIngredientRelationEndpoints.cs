using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.DTO.Requests.UserIngredient;
using Recipes.API.Helpers;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.API.Endpoints;

public static class UserIngredientRelationEndpoints
{
    public static void MapUserIngredientRelationEndpoints<TRelation>(
        this IEndpointRouteBuilder app,
        string route)
        where TRelation : class, IUserIngredientRelation, new()
    {
        var endpoints = app.MapGroup(route).RequireAuthorization();

        endpoints.MapGet("/", async Task<IResult> (
            IUserIngredientRelationService<TRelation> service,
            ClaimsPrincipal user) =>
        {
            if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                return Results.Unauthorized();

            return Results.Ok(await service.GetUserIngredientRelationAsync(userId));
        });

        endpoints.MapPost("/", (
            [FromBody] UserIngredientRequest request,
            IUserIngredientRelationService<TRelation> service,
            ClaimsPrincipal user) => HandleMutationAsync(
            user,
            request.IngredientIds,
            service.SetUserIngredientRelationAsync));

        endpoints.MapPut("/", (
            [FromBody] UserIngredientRequest request,
            IUserIngredientRelationService<TRelation> service,
            ClaimsPrincipal user) => HandleMutationAsync(
            user,
            request.IngredientIds,
            service.AddUserIngredientRelationAsync));

        endpoints.MapDelete("/", (
            [FromBody] UserIngredientRequest request,
            IUserIngredientRelationService<TRelation> service,
            ClaimsPrincipal user) => HandleMutationAsync(
            user,
            request.IngredientIds,
            service.RemoveUserIngredientRelationAsync));
    }

    private static async Task<IResult> HandleMutationAsync(
        ClaimsPrincipal user,
        IReadOnlyCollection<Guid> ingredientIds,
        Func<Guid, IReadOnlyCollection<Guid>, Task> action)
    {
        if (!EndpointUserHelper.TryGetUserId(user, out var userId))
            return Results.Unauthorized();

        try
        {
            await action(userId, ingredientIds);
        }
        catch (KeyNotFoundException exception)
        {
            return Results.NotFound(new { Error = exception.Message });
        }

        return Results.Ok();
    }
}
