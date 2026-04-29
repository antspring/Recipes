using Microsoft.AspNetCore.Mvc;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.Endpoints;

public static class IngredientEndpoints
{
    public static void MapIngredientEndpoints(this IEndpointRouteBuilder app)
    {
        var ingredientEndpoints = app.MapGroup("/api/ingredients").WithTags("Ingredients");

        ingredientEndpoints.MapGet(string.Empty, async (IIngredientService ingredientService) =>
        {
            var ingredients = await ingredientService.GetAllAsync();
            return Results.Ok(ingredients);
        });

        ingredientEndpoints.MapGet("/search", async (
            [FromQuery] string? title,
            IIngredientService ingredientService) =>
        {
            var ingredients = await ingredientService.SearchByTitleAsync(title);
            return Results.Ok(ingredients);
        });
    }
}
