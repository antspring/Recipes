using Recipes.Domain.Models.UserRelations;

namespace Recipes.API.Endpoints;

public static class AllergenEndpoints
{
    public static void MapAllergenEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUserIngredientRelationEndpoints<Allergens>("/api/allergens", "Allergens");
    }
}
