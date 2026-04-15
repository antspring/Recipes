using AutoMapper;
using Recipes.Application.DTO.User;
using Recipes.Domain.Models;

namespace Recipes.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
    }
}