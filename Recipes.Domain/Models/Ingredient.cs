using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipes.Domain.Models;

public class Ingredient
{
    public Guid Id { get; init; }

    [StringLength(100, MinimumLength = 3)]
    [Column(TypeName = "varchar(100)")]
    public string Title { get; set; } = null!;
}