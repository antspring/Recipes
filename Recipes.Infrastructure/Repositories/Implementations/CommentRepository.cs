using Microsoft.EntityFrameworkCore;
using Recipes.Application.DTO.Comment;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class CommentRepository(BaseDbContext context) : ICommentRepository
{
    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await context.Comments
            .Include(c => c.Commentator)
            .Include(c => c.Recipe)
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Comment>> GetByRecipeIdAsync(Guid recipeId)
    {
        return await context.Comments
            .Include(c => c.Commentator)
            .Include(c => c.Images)
            .Where(c => c.RecipeId == recipeId)
            .OrderByDescending(c => c.CreatedAt)
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

    public async Task AddAsync(Comment comment)
    {
        await context.Comments.AddAsync(comment);
    }

    public async Task UpdateAsync(Comment comment)
    {
        comment.UpdatedAt = DateTime.Now.ToUniversalTime();
        context.Comments.Update(comment);
    }

    public async Task DeleteAsync(Comment comment)
    {
        context.Comments.Remove(comment);
    }

    public async Task<int> CountByRecipeIdAsync(Guid recipeId)
    {
        return await context.Comments.CountAsync(c => c.RecipeId == recipeId);
    }
}