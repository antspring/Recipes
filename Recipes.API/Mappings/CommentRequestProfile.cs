using System.Text.Json;
using AutoMapper;
using Recipes.API.DTO.Requests.Comment;
using Recipes.Application.DTO.Comment;

namespace Recipes.API.Mappings;

public class CommentRequestProfile : Profile
{
    public CommentRequestProfile()
    {
        CreateMap<CreateCommentRequest, CreateCommentDto>()
            .ForMember(dest => dest.Images, opt => opt.Ignore());
        CreateMap<UpdateCommentRequest, UpdateCommentDto>()
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.ImageIdsToDelete, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.ImageIdsToDelete)
                    ? new List<Guid>()
                    : JsonSerializer.Deserialize<List<Guid>>(src.ImageIdsToDelete) ?? new List<Guid>()));
    }
}