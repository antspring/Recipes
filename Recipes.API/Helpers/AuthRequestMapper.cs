using Recipes.API.DTO.Requests.User;
using Recipes.Application.DTO.User;

namespace Recipes.API.Helpers;

public static class AuthRequestMapper
{
    public static async Task<CreateUserDto> ToCreateUserDtoAsync(RegisterUserRequest request)
    {
        return new CreateUserDto
        {
            UserName = request.UserName,
            Email = request.Email,
            Name = request.Name,
            Description = request.Description,
            Avatar = request.Avatar != null ? await ImageUploadFactory.CreateAsync(request.Avatar) : null,
            Password = request.Password,
            EmailVerificationCode = request.EmailVerificationCode
        };
    }

    public static LoginUserDto ToLoginUserDto(LoginUserRequest request, string? userAgent)
    {
        return new LoginUserDto
        {
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password,
            UserAgent = userAgent
        };
    }

    public static async Task<UpdateUserDto> ToUpdateUserDtoAsync(UpdateUserRequest request)
    {
        return new UpdateUserDto
        {
            UserName = NormalizeOptionalText(request.UserName),
            Email = NormalizeOptionalText(request.Email),
            Name = NormalizeOptionalText(request.Name),
            Description = NormalizeOptionalText(request.Description),
            Avatar = request.Avatar != null ? await ImageUploadFactory.CreateAsync(request.Avatar) : null
        };
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
