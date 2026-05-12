using Recipes.Domain.Models.Reports;

namespace Recipes.API.Helpers;

public static class ModerationRequestMapper
{
    public static ReportStatus? ToReportStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return null;

        if (!Enum.TryParse<ReportStatus>(status, true, out var parsedStatus))
            throw new InvalidOperationException("Invalid report status");

        return parsedStatus;
    }
}
