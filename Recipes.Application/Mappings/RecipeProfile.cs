using AutoMapper;
using Recipes.Application.DTO.Recipe;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Mappings;

public class RecipeProfile : Profile
{
    public RecipeProfile()
    {
        CreateMap<CreateRecipeDto, Recipe>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => Guid.NewGuid()))
            .ForMember(d => d.Creator, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => DateTime.Now.ToUniversalTime()))
            .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => DateTime.Now.ToUniversalTime()));

        CreateMap<UpdateRecipeDto, Recipe>()
            .ForMember(d => d.Creator, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => DateTime.Now.ToUniversalTime()));
    }
}