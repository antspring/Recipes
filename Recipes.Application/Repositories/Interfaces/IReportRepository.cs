using Recipes.Application.Common;
using Recipes.Domain.Models.Reports;

namespace Recipes.Application.Repositories.Interfaces;

public interface IReportRepository
{
    Task CreateAsync(Report report);
    Task<Report?> GetByIdAsync(Guid id);
    Task<PagedResult<Report>> GetPagedAsync(ReportStatus? status, int page, int pageSize);
    Task UpdateAsync(Report report);
}
