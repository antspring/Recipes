using Microsoft.EntityFrameworkCore;
using Recipes.Application.Common;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models.Reports;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class ReportRepository(BaseDbContext context) : IReportRepository
{
    public Task CreateAsync(Report report)
    {
        return context.Reports.AddAsync(report).AsTask();
    }

    public Task<Report?> GetByIdAsync(Guid id)
    {
        return context.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Moderator)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<PagedResult<Report>> GetPagedAsync(ReportStatus? status, int page, int pageSize)
    {
        var query = context.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Moderator)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Report>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task UpdateAsync(Report report)
    {
        context.Reports.Update(report);
        return Task.CompletedTask;
    }
}
