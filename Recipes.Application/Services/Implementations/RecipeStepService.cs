using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class RecipeStepService(
    IRecipeRepository recipeRepository,
    IRecipeStepRepository recipeStepRepository,
    IImageRepository imageRepository,
    IImageStorageService imageStorageService,
    IImageUrlProvider imageUrlProvider,
    IUnitOfWork unitOfWork,
    IClock clock) : IRecipeStepService
{
    public async Task<List<RecipeStepDto>> GetStepsByRecipeIdAsync(Guid recipeId)
    {
        var recipe = await GetRequiredRecipeAsync(recipeId);
        return ToRecipeStepDtos(recipe.Steps);
    }

    public async Task<RecipeStepDto> CreateStepAsync(CreateRecipeStepDto createStepDto)
    {
        var recipe = await GetRequiredRecipeAsync(createStepDto.RecipeId);
        EnsureRecipeAuthor(recipe, createStepDto.ActorUserId, "create steps for");

        var steps = GetOrderedSteps(recipe);
        var insertIndex = ClampOrder(createStepDto.Order, steps.Count);
        InsertOrderSlot(steps, insertIndex);

        var step = new RecipeStep
        {
            Id = Guid.NewGuid(),
            RecipeId = createStepDto.RecipeId,
            Description = createStepDto.Description,
            Order = insertIndex
        };

        step.Image = await UploadImageAsync(createStepDto.ImageUpload);

        await recipeStepRepository.AddAsync(step);
        await unitOfWork.SaveChangesAsync();

        return await GetRequiredStepDtoAsync(step.Id);
    }

    public async Task<RecipeStepDto> UpdateStepAsync(UpdateRecipeStepDto updateStepDto)
    {
        var recipe = await GetRequiredRecipeAsync(updateStepDto.RecipeId);
        EnsureRecipeAuthor(recipe, updateStepDto.ActorUserId, "update steps for");

        var step = GetRequiredRecipeStep(recipe, updateStepDto.StepId);
        step.Description = updateStepDto.Description;

        if (updateStepDto.Order.HasValue)
            MoveStep(recipe, step, updateStepDto.Order.Value);

        if (updateStepDto.DeleteImage || updateStepDto.ImageUpload != null)
            await DeleteImageAsync(step);

        if (updateStepDto.ImageUpload != null)
            step.Image = await UploadImageAsync(updateStepDto.ImageUpload);

        await recipeStepRepository.UpdateAsync(step);
        await unitOfWork.SaveChangesAsync();

        return await GetRequiredStepDtoAsync(step.Id);
    }

    public async Task DeleteStepAsync(Guid recipeId, Guid stepId, Guid actorUserId)
    {
        var recipe = await GetRequiredRecipeAsync(recipeId);
        EnsureRecipeAuthor(recipe, actorUserId, "delete steps from");

        var step = GetRequiredRecipeStep(recipe, stepId);
        await DeleteImageAsync(step);

        recipe.Steps.Remove(step);
        NormalizeOrders(recipe.Steps);

        await recipeStepRepository.DeleteAsync(step);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<List<RecipeStepDto>> ReorderStepsAsync(ReorderRecipeStepsDto reorderStepsDto)
    {
        var recipe = await GetRequiredRecipeAsync(reorderStepsDto.RecipeId);
        EnsureRecipeAuthor(recipe, reorderStepsDto.ActorUserId, "reorder steps for");

        var orderedSteps = BuildReorderedSteps(recipe.Steps, reorderStepsDto.StepIds);
        NormalizeOrders(orderedSteps);

        foreach (var step in orderedSteps)
        {
            await recipeStepRepository.UpdateAsync(step);
        }

        await unitOfWork.SaveChangesAsync();

        return await GetStepsByRecipeIdAsync(recipe.Id);
    }

    private async Task<Recipe> GetRequiredRecipeAsync(Guid recipeId)
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
            throw new ArgumentException($"Recipe with id {recipeId} not found");

        return recipe;
    }

    private static void EnsureRecipeAuthor(Recipe recipe, Guid actorUserId, string action)
    {
        if (recipe.CreatorId != actorUserId)
            throw new UnauthorizedAccessException($"Only the author can {action} this recipe");
    }

    private static RecipeStep GetRequiredRecipeStep(Recipe recipe, Guid stepId)
    {
        var step = recipe.Steps.FirstOrDefault(rs => rs.Id == stepId);
        if (step == null)
            throw new ArgumentException($"Recipe step with id {stepId} not found");

        return step;
    }

    private static List<RecipeStep> GetOrderedSteps(Recipe recipe)
    {
        return recipe.Steps.OrderBy(rs => rs.Order).ToList();
    }

    private static int ClampOrder(int? order, int maxOrder)
    {
        if (!order.HasValue)
            return maxOrder;

        return Math.Clamp(order.Value, 0, maxOrder);
    }

    private static void InsertOrderSlot(IEnumerable<RecipeStep> steps, int insertIndex)
    {
        foreach (var step in steps.Where(rs => rs.Order >= insertIndex))
        {
            step.Order++;
        }
    }

    private static void MoveStep(Recipe recipe, RecipeStep step, int newOrder)
    {
        var steps = GetOrderedSteps(recipe);
        steps.Remove(step);
        var insertIndex = Math.Clamp(newOrder, 0, steps.Count);
        steps.Insert(insertIndex, step);
        NormalizeOrders(steps);
    }

    private static void NormalizeOrders(IEnumerable<RecipeStep> steps)
    {
        var order = 0;
        foreach (var step in steps.OrderBy(rs => rs.Order))
        {
            step.Order = order++;
        }
    }

    private static List<RecipeStep> BuildReorderedSteps(IReadOnlyCollection<RecipeStep> steps, IReadOnlyCollection<Guid> stepIds)
    {
        if (steps.Count != stepIds.Count)
            throw new ArgumentException("Step ids must contain all recipe steps");

        if (stepIds.Distinct().Count() != stepIds.Count)
            throw new ArgumentException("Step ids must be unique");

        var stepsById = steps.ToDictionary(rs => rs.Id);
        var invalidStepIds = stepIds.Where(id => !stepsById.ContainsKey(id)).ToList();
        if (invalidStepIds.Count > 0)
            throw new ArgumentException($"Recipe steps not found: {string.Join(", ", invalidStepIds)}");

        return stepIds.Select(id => stepsById[id]).ToList();
    }

    private async Task<Image?> UploadImageAsync(ImageUpload? imageUpload)
    {
        if (imageUpload == null)
            return null;

        var fileName = await imageStorageService.UploadImageAsync(
            imageUpload.Stream,
            imageUpload.FileName,
            imageUpload.ContentType);

        var image = new Image
        {
            FileName = fileName,
            CreatedAt = clock.UtcNow
        };

        await imageRepository.AddAsync(image);
        return image;
    }

    private async Task DeleteImageAsync(RecipeStep step)
    {
        if (step.Image == null)
            return;

        await imageStorageService.DeleteImageAsync(step.Image.FileName);
        await imageRepository.DeleteAsync(step.Image);
        step.Image = null;
        step.ImageId = null;
    }

    private async Task<RecipeStepDto> GetRequiredStepDtoAsync(Guid stepId)
    {
        var step = await recipeStepRepository.GetByIdAsync(stepId);
        if (step == null)
            throw new InvalidOperationException("Recipe step not found after save");

        return ToRecipeStepDto(step);
    }

    private List<RecipeStepDto> ToRecipeStepDtos(IEnumerable<RecipeStep> steps)
    {
        return steps.OrderBy(rs => rs.Order).Select(ToRecipeStepDto).ToList();
    }

    private RecipeStepDto ToRecipeStepDto(RecipeStep step)
    {
        var dto = RecipeStepDto.FromRecipeStep(step);
        ApplyImageUrl(dto);
        return dto;
    }

    private void ApplyImageUrl(RecipeStepDto step)
    {
        if (step.Image != null)
            step.Image.Url = imageUrlProvider.GetImageUrl(step.Image.FileName);
    }
}
