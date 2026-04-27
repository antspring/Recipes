using Recipes.Application.DTO.Recipe;

namespace Recipes.Application.Services.Interfaces;

public interface IRecipeStepService
{
    Task<List<RecipeStepDto>> GetStepsByRecipeIdAsync(Guid recipeId);
    Task<RecipeStepDto> CreateStepAsync(CreateRecipeStepDto createStepDto);
    Task<RecipeStepDto> UpdateStepAsync(UpdateRecipeStepDto updateStepDto);
    Task DeleteStepAsync(Guid recipeId, Guid stepId, Guid actorUserId);
    Task<List<RecipeStepDto>> ReorderStepsAsync(ReorderRecipeStepsDto reorderStepsDto);
}
