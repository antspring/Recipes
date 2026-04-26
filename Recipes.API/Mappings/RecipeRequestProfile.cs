using AutoMapper;
using Recipes.API.DTO.Requests.Recipe;
using Recipes.Application.DTO.Recipe;

namespace Recipes.API.Mappings;

public class RecipeRequestProfile : Profile
{
    public RecipeRequestProfile()
    {
        CreateMap<CreateRecipeWithFilesRequest, CreateRecipeDto>()
            .ForMember(d => d.CreatorId, opt => opt.Ignore())
            .ForMember(d => d.Ingredients, opt => opt.Ignore())
            .ForMember(d => d.ImageUploads, opt => opt.Ignore());

        CreateMap<UpdateRecipeWithFilesRequest, UpdateRecipeDto>()
            .ForMember(d => d.Ingredients, opt => opt.Ignore())
            .ForMember(d => d.ImageUploads, opt => opt.Ignore())
            .ForMember(d => d.ImageIdsToDelete, opt => opt.Ignore());
    }
}