using AutoMapper;
using Recipes.Application.DTO.Comment;
using Recipes.Domain.Models;

namespace Recipes.Application.Mappings;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CreateCommentDto, Comment>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Images, opt => opt.Ignore());
    }
}