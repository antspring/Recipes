using Recipes.Domain.Models;
using Recipes.Application.DTO.Comment;

namespace Recipes.Application.Repositories.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(Guid id);
    Task<List<Comment>> GetByRecipeIdAsync(Guid recipeId);
    Task<PagedResult<Comment>> GetByRecipeIdPagedAsync(Guid recipeId, int page, int pageSize, DateTime? from, DateTime? to);
    Task AddAsync(Comment comment);
    Task UpdateAsync(Comment comment);
    Task DeleteAsync(Comment comment);
    Task<int> CountByRecipeIdAsync(Guid recipeId);
}