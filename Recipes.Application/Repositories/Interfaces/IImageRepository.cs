using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface IImageRepository
{
    Task AddAsync(Image image);
    Task DeleteAsync(Image image);
}