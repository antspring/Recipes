using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Recipes.Domain.Models.RecipesRelations;

[PrimaryKey(nameof(RecipeId), nameof(IngredientId))]
public class RecipeIngredient
{
    public Guid RecipeId { get; init; }
    public Guid IngredientId { get; init; }

    public Recipe Recipe { get; init; } = null!;
    public Ingredient Ingredient { get; init; } = null!;

    public int? Weight { get; set; }

    [StringLength(50, MinimumLength = 2)]
    [Column(TypeName = "varchar(50)")]
    public string? AlternativeWeight { get; set; }
}