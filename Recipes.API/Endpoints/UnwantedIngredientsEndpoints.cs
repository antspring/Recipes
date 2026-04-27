using Recipes.Domain.Models.UserRelations;

namespace Recipes.API.Endpoints;

public static class UnwantedIngredientsEndpoints
{
    public static void MapUnwantedIngredientsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUserIngredientRelationEndpoints<UnwantedIngredients>(
            "/api/unwanted-ingredients",
            "Unwanted ingredients");
    }
}
