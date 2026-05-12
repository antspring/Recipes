namespace Recipes.API.DTO.Requests.Report;

public class CreateReportRequest
{
    public string Reason { get; set; } = null!;
    public string? CustomReason { get; set; }
}
