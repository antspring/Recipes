using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class ImageRepository(BaseDbContext context) : IImageRepository
{
    public Task AddAsync(Image image)
    {
        return context.Images.AddAsync(image).AsTask();
    }

    public Task DeleteAsync(Image image)
    {
        context.Images.Remove(image);
        return Task.CompletedTask;
    }
}