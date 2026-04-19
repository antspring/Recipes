using AutoMapper;
using Recipes.API.DTO.Requests.Recipe;
using Recipes.Application.DTO.Recipe;

namespace Recipes.API.Mappings;

public class RecipeIngredientRequestProfile : Profile
{
    public RecipeIngredientRequestProfile()
    {
        CreateMap<CreateRecipeIngredientRequest, RecipeIngredientInputDto>();
        CreateMap<UpdateRecipeIngredientRequest, RecipeIngredientInputDto>();
    }
}