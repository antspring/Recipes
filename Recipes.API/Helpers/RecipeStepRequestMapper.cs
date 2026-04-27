using Recipes.API.DTO.Requests.Recipe;
using Recipes.Application.DTO.Recipe;

namespace Recipes.API.Helpers;

public static class RecipeStepRequestMapper
{
    public static async Task<CreateRecipeStepDto> ToCreateRecipeStepDtoAsync(
        CreateRecipeStepRequest request,
        Guid recipeId,
        Guid actorUserId)
    {
        return new CreateRecipeStepDto
        {
            RecipeId = recipeId,
            ActorUserId = actorUserId,
            Description = request.Description,
            Order = request.Order,
            ImageUpload = request.Image == null
                ? null
                : await ImageUploadFactory.CreateAsync(request.Image)
        };
    }

    public static async Task<UpdateRecipeStepDto> ToUpdateRecipeStepDtoAsync(
        UpdateRecipeStepRequest request,
        Guid recipeId,
        Guid stepId,
        Guid actorUserId)
    {
        return new UpdateRecipeStepDto
        {
            RecipeId = recipeId,
            StepId = stepId,
            ActorUserId = actorUserId,
            Description = request.Description,
            Order = request.Order,
            ImageUpload = request.Image == null
                ? null
                : await ImageUploadFactory.CreateAsync(request.Image),
            DeleteImage = request.DeleteImage
        };
    }

    public static ReorderRecipeStepsDto ToReorderRecipeStepsDto(
        ReorderRecipeStepsRequest request,
        Guid recipeId,
        Guid actorUserId)
    {
        return new ReorderRecipeStepsDto
        {
            RecipeId = recipeId,
            ActorUserId = actorUserId,
            StepIds = request.StepIds
        };
    }
}
