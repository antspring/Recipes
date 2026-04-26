using AutoMapper;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Implementations;

public class RecipeIngredientService(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRecipeIngredientService
{
    public async Task<List<RecipeIngredient>> SaveRecipeIngredientsAsync(List<RecipeIngredientInputDto> ingredientsDto,
        Guid recipeId)
    {
        await ValidateIngredientsExistAsync(ingredientsDto);

        return ingredientsDto
            .Select(ingredientDto =>
                mapper.Map<RecipeIngredient>(ingredientDto, opt => opt.Items.Add("RecipeId", recipeId)))
            .ToList();
    }

    private async Task ValidateIngredientsExistAsync(IEnumerable<RecipeIngredientInputDto> ingredientsDto)
    {
        var ingredientIds = ingredientsDto.Select(ingredient => ingredient.IngredientId).Distinct().ToList();
        var existingIds = await unitOfWork.Ingredients.GetExistingIdsAsync(ingredientIds);
        var missingIds = ingredientIds.Except(existingIds).ToList();

        if (missingIds.Count > 0)
            throw new ArgumentException($"Ingredients not found: {string.Join(", ", missingIds)}");
    }
}