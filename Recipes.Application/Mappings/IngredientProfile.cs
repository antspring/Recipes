using AutoMapper;
using Recipes.Application.DTO.Ingredient;
using Recipes.Domain.Models;

namespace Recipes.Application.Mappings;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDto>();
    }
}