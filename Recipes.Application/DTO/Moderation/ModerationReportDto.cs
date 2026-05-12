namespace Recipes.Application.DTO.Moderation;

public class ModerationReportDto
{
    public Guid Id { get; init; }
    public Guid ReporterId { get; init; }
    public string ReporterUserName { get; init; } = null!;
    public string TargetType { get; init; } = null!;
    public Guid TargetId { get; init; }
    public string Reason { get; init; } = null!;
    public string? CustomReason { get; init; }
    public string Status { get; init; } = null!;
    public Guid? ModeratorId { get; init; }
    public string? ModeratorUserName { get; init; }
    public string? ResolutionComment { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ResolvedAt { get; init; }
}
