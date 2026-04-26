using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class ImageRepository(BaseDbContext context) : IImageRepository
{
    public async Task AddAsync(Image image)
    {
        await context.Images.AddAsync(image);
    }

    public Task DeleteAsync(Image image)
    {
        context.Images.Remove(image);
        return Task.CompletedTask;
    }
}