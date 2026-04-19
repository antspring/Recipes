using Recipes.Application.Services.Interfaces;

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
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }

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
            CreatorName = recipe.Creator.Name,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt,
            CookingTime = recipe.CookingTime,
            DishType = recipe.DishType,
            MealType = recipe.MealType,
            Ingredients = recipe.RecipeIngredients?.Select(ri => new RecipeIngredientDto
            {
                IngredientId = ri.IngredientId,
                IngredientTitle = ri.Ingredient.Title,
                Weight = ri.Weight,
                AlternativeWeight = ri.AlternativeWeight
            }).ToList() ?? new(),
            Images = recipe.RecipeImages?.OrderBy(ri => ri.Order).Select(ri => new ImageDto
            {
                Id = ri.ImageId,
                FileName = ri.Image.FileName,
                CreatedAt = ri.Image.CreatedAt
            }).ToList() ?? new(),
            LikesCount = recipe.Likes?.Count ?? 0,
            CommentsCount = recipe.Comments?.Count ?? 0
        };
    }

    public void ApplyImageUrls(IImageStorageService imageStorageService)
    {
        if (Images != null)
        {
            foreach (var image in Images)
            {
                image.Url = imageStorageService.GetImageUrl(image.FileName);
            }
        }
    }
}