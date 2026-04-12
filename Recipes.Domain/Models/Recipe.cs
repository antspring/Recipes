using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Domain.Models;

public class Recipe
{
    public Guid Id { get; init; }

    [StringLength(150, MinimumLength = 3)]
    [Column(TypeName = "varchar(150)")]
    public string Title { get; set; } = null!;

    [StringLength(1000, MinimumLength = 3)]
    [Column(TypeName = "varchar(1000)")]
    public string Description { get; set; } = null!;

    public int CaloricValue { get; set; }
    public double Proteins { get; set; }
    public double Fats { get; set; }
    public double Carbohydrates { get; set; }

    public Guid CreatorId { get; init; }
    public User Creator { get; init; } = null!;

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public List<Like>? Likes { get; init; }
    public List<Comment>? Comments { get; init; }
    public List<Image>? Images { get; init; }
    public List<RecipeIngredient>? RecipeIngredients { get; set; }
    public List<RecipeImage>? RecipeImages { get; set; }
}