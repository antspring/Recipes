using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface IImageRepository
{
    Task<Image?> GetByIdAsync(Guid id);
    Task<List<Image>> GetAllAsync();
    Task AddAsync(Image image);
    Task UpdateAsync(Image image);
    Task DeleteAsync(Image image);
    Task<bool> ExistsAsync(Guid id);
}