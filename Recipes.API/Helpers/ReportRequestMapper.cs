using Recipes.API.DTO.Requests.Report;
using Recipes.Domain.Models.Reports;

namespace Recipes.API.Helpers;

public static class ReportRequestMapper
{
    public static ReportReason ToReportReason(CreateReportRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new InvalidOperationException("Report reason is required");

        if (!Enum.TryParse<ReportReason>(request.Reason, true, out var reason))
            throw new InvalidOperationException("Invalid report reason");

        return reason;
    }
}
