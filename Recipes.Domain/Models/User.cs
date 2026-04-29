using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Domain.Models;

public class User
{
    public Guid Id { get; init; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? AvatarUrl { get; set; }

    public string Password { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Recipe> Recipes { get; init; } = new();
    public List<Like> Likes { get; init; } = new();
    public List<RecipeRating> RecipeRatings { get; init; } = new();
    public List<Favorite> Favorites { get; init; } = new();
    public List<Comment> Comments { get; init; } = new();
    public List<UnwantedIngredients> UnwantedIngredients { get; init; } = new();
    public List<Allergens> Allergens { get; init; } = new();
    public List<UserSubscription> Following { get; init; } = new();
    public List<UserSubscription> Followers { get; init; } = new();
}
