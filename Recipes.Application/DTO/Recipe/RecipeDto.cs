namespace Recipes.Application.DTO.Recipe;

public class RecipeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int CaloricValue { get; set; }
    public double Proteins { get; set; }
    public double Fats { get; set; }
    public double Carbohydrates { get; set; }
    public Guid CreatorId { get; set; }
    public string CreatorName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public TimeSpan? CookingTime { get; set; }
    public string? DishType { get; set; }
    public string? MealType { get; set; }
    public List<RecipeIngredientDto> Ingredients { get; set; } = new();
    public List<ImageDto> Images { get; set; } = new();
    public List<RecipeStepDto> Steps { get; set; } = new();
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }

    public static RecipeDto FromRecipe(Domain.Models.Recipe recipe)
    {
        return new RecipeDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            CaloricValue = recipe.CaloricValue,
            Proteins = recipe.Proteins,
            Fats = recipe.Fats,
            Carbohydrates = recipe.Carbohydrates,
            CreatorId = recipe.CreatorId,
            CreatorName = recipe.Creator?.Name ?? string.Empty,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt,
            CookingTime = recipe.CookingTime,
            DishType = recipe.DishType,
            MealType = recipe.MealType,
            Ingredients = recipe.RecipeIngredients?
                .Where(ri => ri.Ingredient != null)
                .Select(ri => new RecipeIngredientDto
            {
                IngredientId = ri.IngredientId,
                IngredientTitle = ri.Ingredient.Title,
                Weight = ri.Weight,
                AlternativeWeight = ri.AlternativeWeight
            }).ToList() ?? new(),
            Images = recipe.RecipeImages?
                .Where(ri => ri.Image != null)
                .OrderBy(ri => ri.Order)
                .Select(ri => new ImageDto
            {
                Id = ri.ImageId,
                FileName = ri.Image.FileName,
                CreatedAt = ri.Image.CreatedAt
            }).ToList() ?? new(),
            Steps = recipe.Steps?.OrderBy(rs => rs.Order).Select(RecipeStepDto.FromRecipeStep).ToList() ?? new(),
            LikesCount = recipe.Likes?.Count ?? 0,
            CommentsCount = recipe.Comments?.Count ?? 0,
            AverageRating = recipe.Ratings?.Count > 0 ? recipe.Ratings.Average(r => r.Value) : 0,
            RatingsCount = recipe.Ratings?.Count ?? 0
        };
    }

}
