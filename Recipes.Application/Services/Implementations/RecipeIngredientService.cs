using AutoMapper;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Implementations;

public class RecipeIngredientService(
    IIngredientRepository ingredientRepository,
    IMapper mapper) : IRecipeIngredientService
{
    public async Task<List<RecipeIngredient>> SaveRecipeIngredientsAsync(List<RecipeIngredientInputDto> ingredientsDto,
        Guid recipeId)
    {
        ValidateUniqueIngredients(ingredientsDto);
        await ValidateIngredientsExistAsync(ingredientsDto);

        return ingredientsDto
            .Select(ingredientDto =>
                mapper.Map<RecipeIngredient>(ingredientDto, opt => opt.Items.Add("RecipeId", recipeId)))
            .ToList();
    }

    private static void ValidateUniqueIngredients(IEnumerable<RecipeIngredientInputDto> ingredientsDto)
    {
        var duplicateIds = ingredientsDto
            .GroupBy(ingredient => ingredient.IngredientId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateIds.Count > 0)
            throw new ArgumentException($"Duplicate ingredients: {string.Join(", ", duplicateIds)}");
    }

    private async Task ValidateIngredientsExistAsync(IEnumerable<RecipeIngredientInputDto> ingredientsDto)
    {
        var ingredientIds = ingredientsDto.Select(ingredient => ingredient.IngredientId).Distinct().ToList();
        var existingIds = await ingredientRepository.GetExistingIdsAsync(ingredientIds);
        var missingIds = ingredientIds.Except(existingIds).ToList();

        if (missingIds.Count > 0)
            throw new ArgumentException($"Ingredients not found: {string.Join(", ", missingIds)}");
    }
}