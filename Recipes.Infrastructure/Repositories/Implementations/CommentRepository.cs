using Microsoft.EntityFrameworkCore;
using Recipes.Application.Common;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class CommentRepository(BaseDbContext context) : ICommentRepository
{
    public Task<Comment?> GetByIdAsync(Guid id)
    {
        return context.Comments
            .Include(c => c.Commentator)
            .Include(c => c.Recipe)
            .Include(c => c.Images)
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<List<Comment>> GetByRecipeIdAsync(Guid recipeId)
    {
        return context.Comments
            .Include(c => c.Images)
            .Where(c => c.RecipeId == recipeId)
            .ToListAsync();
    }

    public async Task<PagedResult<Comment>> GetByRecipeIdPagedAsync(Guid recipeId, int page, int pageSize,
        DateTime? from, DateTime? to)
    {
        var query = context.Comments
            .Include(c => c.Commentator)
            .Include(c => c.Images)
            .Where(c => c.RecipeId == recipeId);

        if (from.HasValue)
            query = query.Where(c => c.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(c => c.CreatedAt <= to.Value);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return new PagedResult<Comment>
        {
            Items = items,
            TotalCount = await query.CountAsync(c => c.ParentCommentId == null),
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<Dictionary<Guid, int>> GetCountsByRecipeIdsAsync(IReadOnlyCollection<Guid> recipeIds)
    {
        return context.Comments
            .Where(c => recipeIds.Contains(c.RecipeId))
            .GroupBy(c => c.RecipeId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public Task AddAsync(Comment comment)
    {
        return context.Comments.AddAsync(comment).AsTask();
    }

    public Task UpdateAsync(Comment comment)
    {
        context.Comments.Update(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Comment comment)
    {
        context.Comments.Remove(comment);
        return Task.CompletedTask;
    }
}
