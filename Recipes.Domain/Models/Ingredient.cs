using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Domain.Models;

public class Ingredient
{
    public Guid Id { get; init; }

    [StringLength(100, MinimumLength = 3)]
    [Column(TypeName = "varchar(100)")]
    public string Title { get; set; } = null!;

    public List<UnwantedIngredients>? UsersUnwantedIngredients { get; init; }
    public List<Allergens>? UsersAllergens { get; init; }
}