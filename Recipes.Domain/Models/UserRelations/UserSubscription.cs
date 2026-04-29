using Recipes.Domain.Models;

namespace Recipes.Domain.Models.UserRelations;

public class UserSubscription
{
    public Guid SubscriberId { get; init; }
    public User Subscriber { get; init; } = null!;

    public Guid SubscribedToId { get; init; }
    public User SubscribedTo { get; init; } = null!;

    public DateTime CreatedAt { get; init; }
}
