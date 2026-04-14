using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.DTO.Requests;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.Endpoints;

public static class UnwantedIngredientsEndpoints
{
    public static void MapUnwantedIngredientsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/api/unwanted-ingredients").RequireAuthorization();

        endpoints.MapGet("/", async Task<IResult> (
            IUnwantedIngredientsService unwantedIngredientsService,
            ClaimsPrincipal user
        ) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);
            return
                Results.Ok(await unwantedIngredientsService.GetUnwantedIngredientsAsync(userId));
        });

        endpoints.MapPost("/", async Task<IResult> (
            [FromBody] UnwantedIngredientRequest request,
            IUnwantedIngredientsService unwantedIngredientsService,
            ClaimsPrincipal user
        ) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                await unwantedIngredientsService.SetUnwantedIngredientsAsync(userId, request.IngredientIds);
            }
            catch (KeyNotFoundException e)
            {
                return Results.BadRequest(e.Message);
            }

            return Results.Ok();
        });

        endpoints.MapPut("/", async Task<IResult> (
            [FromBody] UnwantedIngredientRequest request,
            IUnwantedIngredientsService unwantedIngredientsService,
            ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                await unwantedIngredientsService.AddUnwantedIngredientsAsync(userId, request.IngredientIds);
            }
            catch (KeyNotFoundException e)
            {
                return Results.NotFound(e.Message);
            }

            return Results.Ok();
        });

        endpoints.MapDelete("/", async Task<IResult> (
            [FromBody] UnwantedIngredientRequest request,
            IUnwantedIngredientsService unwantedIngredientsService,
            ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            try
            {
                await unwantedIngredientsService.RemoveUnwantedIngredientsAsync(userId, request.IngredientIds);
            }
            catch (KeyNotFoundException e)
            {
                return Results.NotFound(e.Message);
            }

            return Results.Ok();
        });
    }
}