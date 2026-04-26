using Recipes.Application.UnitOfWork.Interfaces;

namespace Recipes.Infrastructure.UnitOfWork.Implementations;

public class UnitOfWork(BaseDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
