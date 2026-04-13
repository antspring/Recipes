using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Domain.Models;

public class User
{
    public Guid Id { get; init; }

    [StringLength(50, MinimumLength = 3)]
    [Column(TypeName = "varchar(50)")]
    public string UserName { get; set; } = null!;

    [StringLength(254)]
    [Column(TypeName = "varchar(254)")]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string Name { get; set; } = null!;

    [StringLength(1000)]
    [Column(TypeName = "varchar(1000)")]
    public string? Description { get; set; }

    public string? AvatarUrl { get; set; }

    [StringLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string Password { get; set; } = null!;

    public DateTime CreatedAt { get; init; } = DateTime.Now.ToUniversalTime();
    public DateTime UpdatedAt { get; private set; } = DateTime.Now.ToUniversalTime();

    public List<Recipe>? Recipes { get; init; }
    public List<Like>? Likes { get; init; }
    public List<Favorite>? Favorites { get; init; }
    public List<Comment>? Comments { get; init; }
    public List<UnwantedIngredients>? UnwantedIngredients { get; init; }
}