using Recipes.Domain.Models.Reports;

namespace Recipes.Application.Services.Interfaces;

public interface IReportService
{
    Task CreateRecipeReportAsync(Guid recipeId, Guid reporterId, ReportReason reason, string? customReason);
    Task CreateUserProfileReportAsync(Guid userId, Guid reporterId, ReportReason reason, string? customReason);
    Task CreateCommentReportAsync(Guid commentId, Guid reporterId, ReportReason reason, string? customReason);
}
