namespace Recipes.Application.Services.Interfaces;

public interface IClock
{
    DateTime UtcNow { get; }
}
