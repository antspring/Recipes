using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class UtcClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
