using Recipes.Application.Common;
using Recipes.Application.DTO.Moderation;
using Recipes.Domain.Models.Reports;

namespace Recipes.Application.Services.Interfaces;

public interface IModerationReportService
{
    Task<PagedResult<ModerationReportDto>> GetReportsAsync(ReportStatus? status, int page, int pageSize);
    Task<ModerationReportDetailsDto> GetReportAsync(Guid reportId, Guid moderatorId);
    Task DismissAsync(Guid reportId, Guid moderatorId, string? resolutionComment);
}
