using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Recipe;

public class ReorderRecipeStepsRequest
{
    [Required(ErrorMessage = "StepIds are required")]
    public List<Guid> StepIds { get; set; } = new();
}
