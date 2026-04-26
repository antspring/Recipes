using AutoMapper;
using Recipes.Application.DTO.User;
using Recipes.Domain.Models;

namespace Recipes.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>()
            .ForMember(d => d.AvatarUrl, opt => opt.Ignore())
            .ForMember(d => d.Password, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((_, _, sourceMember) => sourceMember != null));
    }
}