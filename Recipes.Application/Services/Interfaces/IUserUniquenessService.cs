namespace Recipes.Application.Services.Interfaces;

public interface IUserUniquenessService
{
    Task EnsureUserNameOrEmailAvailableAsync(string? userName, string? email, Guid? currentUserId = null);
}
