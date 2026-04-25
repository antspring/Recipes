using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Domain.Models;

public class Recipe
{
    public Guid Id { get; init; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int CaloricValue { get; set; }
    public double Proteins { get; set; }
    public double Fats { get; set; }
    public double Carbohydrates { get; set; }

    public Guid CreatorId { get; init; }
    public User Creator { get; init; } = null!;

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public TimeSpan? CookingTime { get; set; }
    public string? DishType { get; set; }
    public string? MealType { get; set; }

    public List<Like> Likes { get; init; } = new();
    public List<Comment> Comments { get; init; } = new();
    public List<Image> Images { get; init; } = new();
    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
    public List<RecipeImage> RecipeImages { get; set; } = new();
}