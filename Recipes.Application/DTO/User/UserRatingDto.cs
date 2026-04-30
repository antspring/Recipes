namespace Recipes.Application.DTO.User;

public class UserRatingDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public string? AvatarUrl { get; init; }
    public int RecipesCount { get; init; }
    public int FollowersCount { get; init; }
    public int Rating { get; init; }
    public bool IsSubscribedByCurrentUser { get; set; }
}
