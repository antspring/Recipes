using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.DTO.Requests;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.API.Endpoints;

public static class UnwantedIngredientsEndpoints
{
    public static void MapUnwantedIngredientsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/api/unwanted-ingredients").RequireAuthorization();

        endpoints.MapGet("/", async Task<IResult> (
            IUserIngredientRelationService<UnwantedIngredients> service,
            ClaimsPrincipal user
        ) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            return
                Results.Ok(await service.GetUserIngredientRelationAsync(userId));
        });

        endpoints.MapPost("/", async Task<IResult> (
            [FromBody] UnwantedIngredientRequest request,
            IUserIngredientRelationService<UnwantedIngredients> service,
            ClaimsPrincipal user
        ) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                await service.SetUserIngredientRelationAsync(userId, request.IngredientIds);
            }
            catch (KeyNotFoundException e)
            {
                return Results.NotFound(new { Error = e.Message });
            }

            return Results.Ok();
        });

        endpoints.MapPut("/", async Task<IResult> (
            [FromBody] UnwantedIngredientRequest request,
            IUserIngredientRelationService<UnwantedIngredients> service,
            ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                await service.AddUserIngredientRelationAsync(userId, request.IngredientIds);
            }
            catch (KeyNotFoundException e)
            {
                return Results.NotFound(new { Error = e.Message });
            }

            return Results.Ok();
        });

        endpoints.MapDelete("/", async Task<IResult> (
            [FromBody] UnwantedIngredientRequest request,
            IUserIngredientRelationService<UnwantedIngredients> service,
            ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                await service.RemoveUserIngredientRelationAsync(userId, request.IngredientIds);
            }
            catch (KeyNotFoundException e)
            {
                return Results.NotFound(new { Error = e.Message });
            }

            return Results.Ok();
        });
    }
}