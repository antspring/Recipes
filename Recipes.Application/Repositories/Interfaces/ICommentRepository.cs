using Recipes.Application.Common;
using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(Guid id);

    Task<PagedResult<Comment>> GetByRecipeIdPagedAsync(Guid recipeId, int page, int pageSize, DateTime? from,
        DateTime? to);

    Task<Dictionary<Guid, int>> GetCountsByRecipeIdsAsync(IReadOnlyCollection<Guid> recipeIds);

    Task AddAsync(Comment comment);
    Task UpdateAsync(Comment comment);
    Task DeleteAsync(Comment comment);
}
