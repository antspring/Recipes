using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class ImageRepository(BaseDbContext context) : IImageRepository
{
    public async Task<Image?> GetByIdAsync(Guid id)
    {
        return await context.Images.FindAsync(id);
    }

    public async Task<List<Image>> GetAllAsync()
    {
        return await context.Images.ToListAsync();
    }

    public async Task AddAsync(Image image)
    {
        await context.Images.AddAsync(image);
    }

    public async Task UpdateAsync(Image image)
    {
        context.Images.Update(image);
    }

    public async Task DeleteAsync(Image image)
    {
        context.Images.Remove(image);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Images.AnyAsync(i => i.Id == id);
    }
}