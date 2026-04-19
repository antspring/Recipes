using AutoMapper;
using AutoMapper.Configuration;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Implementations;

public class RecipeIngredientService(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRecipeIngredientService
{
    public async Task<List<RecipeIngredient>> SaveRecipeIngredientsAsync(List<RecipeIngredientInputDto> ingredientsDto,
        Guid recipeId)
    {
        var recipeIngredients = new List<RecipeIngredient>();
        foreach (var ingredientDto in ingredientsDto)
        {
            var ingredient = await unitOfWork.Ingredients.GetByIdAsync(ingredientDto.IngredientId);
            if (ingredient == null)
                throw new ArgumentException($"Ingredient with id {ingredientDto.IngredientId} not found");

            var recipeIngredient =
                mapper.Map<RecipeIngredient>(ingredientDto, opt => opt.Items.Add("RecipeId", recipeId));
            recipeIngredients.Add(recipeIngredient);
        }

        return recipeIngredients;
    }
}