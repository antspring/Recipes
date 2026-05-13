using Recipes.Application.Common;
using Recipes.Application.DTO.Comment;
using Recipes.Application.DTO.Moderation;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.Reports;

namespace Recipes.Application.Services.Implementations;

public class ModerationReportService(
    IReportRepository reportRepository,
    IRecipeRepository recipeRepository,
    IRecipeDtoFactory recipeDtoFactory,
    ICommentRepository commentRepository,
    IUserPublicProfileService userPublicProfileService,
    IRecipeCrudService recipeCrudService,
    ICommentService commentService,
    IUserProfileService userProfileService,
    IImageUrlMapper imageUrlMapper,
    IUnitOfWork unitOfWork,
    IClock clock) : IModerationReportService
{
    public async Task<PagedResult<ModerationReportDto>> GetReportsAsync(ReportStatus? status, int page, int pageSize)
    {
        EnsureValidPagination(page, pageSize);

        var reports = await reportRepository.GetPagedAsync(status, page, pageSize);

        return new PagedResult<ModerationReportDto>
        {
            Items = reports.Items.Select(ToDto).ToList(),
            TotalCount = reports.TotalCount,
            Page = reports.Page,
            PageSize = reports.PageSize
        };
    }

    public async Task<ModerationReportDetailsDto> GetReportAsync(Guid reportId, Guid moderatorId)
    {
        var report = await GetRequiredReportAsync(reportId);
        var target = await GetTargetAsync(report, moderatorId);

        return ToDetailsDto(report, target);
    }

    public async Task DismissAsync(Guid reportId, Guid moderatorId, string? resolutionComment)
    {
        var report = await GetRequiredReportAsync(reportId);
        EnsurePending(report);

        report.Status = ReportStatus.Dismissed;
        report.ModeratorId = moderatorId;
        report.ResolutionComment = NormalizeResolutionComment(resolutionComment);
        report.ResolvedAt = clock.UtcNow;

        await reportRepository.UpdateAsync(report);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task TakeActionAsync(Guid reportId, Guid moderatorId, string? resolutionComment)
    {
        var report = await GetRequiredReportAsync(reportId);
        EnsurePending(report);

        await ApplyActionAsync(report, moderatorId);

        report.Status = ReportStatus.ActionTaken;
        report.ModeratorId = moderatorId;
        report.ResolutionComment = NormalizeResolutionComment(resolutionComment);
        report.ResolvedAt = clock.UtcNow;

        await reportRepository.UpdateAsync(report);
        await unitOfWork.SaveChangesAsync();
    }

    private async Task<Report> GetRequiredReportAsync(Guid reportId)
    {
        var report = await reportRepository.GetByIdAsync(reportId);
        if (report == null)
            throw new ArgumentException("Report not found");

        return report;
    }

    private async Task<object?> GetTargetAsync(Report report, Guid moderatorId)
    {
        return report.TargetType switch
        {
            ReportTargetType.Recipe => await GetRecipeTargetAsync(report.TargetId, moderatorId),
            ReportTargetType.Comment => await GetCommentTargetAsync(report.TargetId),
            ReportTargetType.UserProfile => await GetUserProfileTargetAsync(report.TargetId, moderatorId),
            _ => null
        };
    }

    private async Task ApplyActionAsync(Report report, Guid moderatorId)
    {
        switch (report.TargetType)
        {
            case ReportTargetType.Recipe:
                await recipeCrudService.DeleteRecipeByModeratorAsync(report.TargetId);
                break;
            case ReportTargetType.Comment:
                await commentService.DeleteCommentByModeratorAsync(report.TargetId);
                break;
            case ReportTargetType.UserProfile:
                await userProfileService.BlockByModeratorAsync(report.TargetId, moderatorId);
                break;
            default:
                throw new InvalidOperationException("Unsupported report target type");
        }
    }

    private async Task<object?> GetRecipeTargetAsync(Guid recipeId, Guid moderatorId)
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId, RecipeIncludes.Full);
        return recipe == null
            ? null
            : await recipeDtoFactory.CreateAsync(recipe, moderatorId);
    }

    private async Task<object?> GetCommentTargetAsync(Guid commentId)
    {
        var comment = await commentRepository.GetByIdAsync(commentId);
        return comment == null
            ? null
            : ToCommentDto(comment);
    }

    private async Task<object?> GetUserProfileTargetAsync(Guid userId, Guid moderatorId)
    {
        try
        {
            return await userPublicProfileService.GetProfileAsync(userId, moderatorId);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private CommentDto ToCommentDto(Comment comment)
    {
        var dto = CommentDto.FromComment(comment);
        dto.CommentatorAvatarUrl = imageUrlMapper.ToImageUrl(dto.CommentatorAvatarUrl);
        imageUrlMapper.ApplyUrls(dto.Images, image => image.FileName, (image, url) => image.Url = url);
        return dto;
    }

    private static ModerationReportDto ToDto(Report report)
    {
        return new ModerationReportDto
        {
            Id = report.Id,
            ReporterId = report.ReporterId,
            ReporterUserName = report.Reporter.UserName,
            TargetType = report.TargetType.ToString(),
            TargetId = report.TargetId,
            Reason = report.Reason.ToString(),
            CustomReason = report.CustomReason,
            Status = report.Status.ToString(),
            ModeratorId = report.ModeratorId,
            ModeratorUserName = report.Moderator?.UserName,
            ResolutionComment = report.ResolutionComment,
            CreatedAt = report.CreatedAt,
            ResolvedAt = report.ResolvedAt
        };
    }

    private static ModerationReportDetailsDto ToDetailsDto(Report report, object? target)
    {
        return new ModerationReportDetailsDto
        {
            Id = report.Id,
            ReporterId = report.ReporterId,
            ReporterUserName = report.Reporter.UserName,
            TargetType = report.TargetType.ToString(),
            TargetId = report.TargetId,
            Reason = report.Reason.ToString(),
            CustomReason = report.CustomReason,
            Status = report.Status.ToString(),
            ModeratorId = report.ModeratorId,
            ModeratorUserName = report.Moderator?.UserName,
            ResolutionComment = report.ResolutionComment,
            CreatedAt = report.CreatedAt,
            ResolvedAt = report.ResolvedAt,
            Target = target
        };
    }

    private static void EnsurePending(Report report)
    {
        if (report.Status != ReportStatus.Pending)
            throw new InvalidOperationException("Report is already resolved");
    }

    private static void EnsureValidPagination(int page, int pageSize)
    {
        if (page < 1)
            throw new InvalidOperationException("Page must be greater than 0");

        if (pageSize is < 1 or > 100)
            throw new InvalidOperationException("Page size must be between 1 and 100");
    }

    private static string? NormalizeResolutionComment(string? resolutionComment)
    {
        return string.IsNullOrWhiteSpace(resolutionComment)
            ? null
            : resolutionComment.Trim();
    }
}
