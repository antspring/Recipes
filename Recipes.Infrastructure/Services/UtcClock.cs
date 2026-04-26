using Recipes.Application.Services.Interfaces;

namespace Recipes.Infrastructure.Services;

public class UtcClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
