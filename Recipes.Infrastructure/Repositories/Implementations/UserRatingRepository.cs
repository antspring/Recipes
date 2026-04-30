using Microsoft.EntityFrameworkCore;
using Recipes.Application.DTO.User;
using Recipes.Application.Repositories.Interfaces;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class UserRatingRepository(BaseDbContext context) : IUserRatingRepository
{
    public Task<List<UserRatingDto>> GetTopAsync(int count)
    {
        return context.Users
            .Select(user => new UserRatingDto
            {
                Id = user.Id,
                UserName = user.UserName,
                AvatarUrl = user.AvatarUrl,
                RecipesCount = user.Recipes.Count,
                FollowersCount = user.Followers.Count,
                Rating = user.Recipes
                    .SelectMany(recipe => recipe.Ratings)
                    .Sum(rating => (int?)rating.Value) ?? 0
            })
            .OrderByDescending(user => user.Rating)
            .ThenByDescending(user => user.FollowersCount)
            .ThenBy(user => user.UserName)
            .Take(count)
            .ToListAsync();
    }
}
