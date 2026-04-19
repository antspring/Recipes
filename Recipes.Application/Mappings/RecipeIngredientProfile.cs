using AutoMapper;
using Recipes.Application.DTO.Recipe;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Mappings;

public class RecipeIngredientProfile : Profile
{
    public RecipeIngredientProfile()
    {
        CreateMap<RecipeIngredientInputDto, RecipeIngredient>()
            .ConstructUsing((src, context) => new RecipeIngredient
            {
                RecipeId = (Guid)context.Items["RecipeId"],
                IngredientId = src.IngredientId,
                Weight = src.Weight,
                AlternativeWeight = src.AlternativeWeight
            })
            .ForMember(d => d.Ingredient, opt => opt.Ignore())
            .ForMember(d => d.Recipe, opt => opt.Ignore());
    }
}