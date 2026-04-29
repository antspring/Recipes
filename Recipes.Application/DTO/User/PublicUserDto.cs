namespace Recipes.Application.DTO.User;

public class PublicUserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string? AvatarUrl { get; init; }
    public int FollowersCount { get; init; }
    public int FollowingCount { get; init; }
    public bool IsSubscribedByCurrentUser { get; init; }
}
