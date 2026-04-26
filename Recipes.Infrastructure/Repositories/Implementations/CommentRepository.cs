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
            .FirstOrDefaultAsync(c => c.Id == id);
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

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Comment>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
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
