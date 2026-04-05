using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Recipes.Domain.Models.RecipesRelations;

[PrimaryKey(nameof(RecipeId), nameof(ImageId))]
public class RecipeImage
{
    public Guid RecipeId { get; init; }
    public Guid ImageId { get; init; }

    public Recipe Recipe { get; init; } = null!;
    public Image Image { get; init; } = null!;

    public int Order { get; set; }
}