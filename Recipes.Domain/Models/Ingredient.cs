using Recipes.Domain.Models.UserRelations;

namespace Recipes.Domain.Models;

public class Ingredient
{
    public Guid Id { get; init; }

    public string Title { get; set; } = null!;

    public List<UnwantedIngredients>? UsersUnwantedIngredients { get; init; }
    public List<Allergens>? UsersAllergens { get; init; }
}