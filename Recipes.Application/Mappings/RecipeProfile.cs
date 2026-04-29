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
            .ForMember(d => d.Creator, opt => opt.Ignore());

        CreateMap<UpdateRecipeDto, Recipe>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatorId, opt => opt.Ignore())
            .ForMember(d => d.Creator, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CaloricValue, opt =>
            {
                opt.PreCondition(s => s.CaloricValue.HasValue);
                opt.MapFrom(s => s.CaloricValue!.Value);
            })
            .ForMember(d => d.Proteins, opt =>
            {
                opt.PreCondition(s => s.Proteins.HasValue);
                opt.MapFrom(s => s.Proteins!.Value);
            })
            .ForMember(d => d.Fats, opt =>
            {
                opt.PreCondition(s => s.Fats.HasValue);
                opt.MapFrom(s => s.Fats!.Value);
            })
            .ForMember(d => d.Carbohydrates, opt =>
            {
                opt.PreCondition(s => s.Carbohydrates.HasValue);
                opt.MapFrom(s => s.Carbohydrates!.Value);
            })
            .ForAllMembers(opt => opt.Condition((_, _, sourceMember) => sourceMember != null));
    }
}
