namespace Recipes.Domain.Models.Reports;

public class Report
{
    public Guid Id { get; init; }

    public Guid ReporterId { get; init; }
    public User Reporter { get; init; } = null!;

    public ReportTargetType TargetType { get; init; }
    public Guid TargetId { get; init; }

    public ReportReason Reason { get; init; }
    public string? CustomReason { get; init; }

    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    public Guid? ModeratorId { get; set; }
    public User? Moderator { get; set; }

    public string? ResolutionComment { get; set; }

    public DateTime CreatedAt { get; init; }
    public DateTime? ResolvedAt { get; set; }
}
