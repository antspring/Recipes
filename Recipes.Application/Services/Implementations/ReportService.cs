using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.Reports;

namespace Recipes.Application.Services.Implementations;

public class ReportService(
    IReportRepository reportRepository,
    IRecipeExistenceRepository recipeExistenceRepository,
    IUserRepository userRepository,
    ICommentRepository commentRepository,
    IUnitOfWork unitOfWork,
    IClock clock) : IReportService
{
    private static readonly IReadOnlySet<ReportReason> RecipeReasons = new HashSet<ReportReason>
    {
        ReportReason.PhotoDoesNotMatchRecipe,
        ReportReason.DescriptionDoesNotMatchRecipe,
        ReportReason.Other
    };

    private static readonly IReadOnlySet<ReportReason> UserProfileReasons = new HashSet<ReportReason>
    {
        ReportReason.CommunityRulesViolation,
        ReportReason.Other
    };

    private static readonly IReadOnlySet<ReportReason> CommentReasons = new HashSet<ReportReason>
    {
        ReportReason.InappropriatePhoto,
        ReportReason.OffensiveContent,
        ReportReason.Other
    };

    public async Task CreateRecipeReportAsync(Guid recipeId, Guid reporterId, ReportReason reason, string? customReason)
    {
        if (!await recipeExistenceRepository.ExistsAsync(recipeId))
            throw new ArgumentException("Recipe not found");

        await CreateReportAsync(reporterId, ReportTargetType.Recipe, recipeId, reason, customReason, RecipeReasons);
    }

    public async Task CreateUserProfileReportAsync(Guid userId, Guid reporterId, ReportReason reason, string? customReason)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found");

        await CreateReportAsync(reporterId, ReportTargetType.UserProfile, userId, reason, customReason,
            UserProfileReasons);
    }

    public async Task CreateCommentReportAsync(Guid commentId, Guid reporterId, ReportReason reason, string? customReason)
    {
        var comment = await commentRepository.GetByIdAsync(commentId);
        if (comment == null)
            throw new ArgumentException("Comment not found");

        await CreateReportAsync(reporterId, ReportTargetType.Comment, commentId, reason, customReason, CommentReasons);
    }

    private async Task CreateReportAsync(
        Guid reporterId,
        ReportTargetType targetType,
        Guid targetId,
        ReportReason reason,
        string? customReason,
        IReadOnlySet<ReportReason> allowedReasons)
    {
        EnsureReasonAllowed(reason, allowedReasons);

        var normalizedCustomReason = NormalizeCustomReason(reason, customReason);

        await reportRepository.CreateAsync(new Report
        {
            Id = Guid.NewGuid(),
            ReporterId = reporterId,
            TargetType = targetType,
            TargetId = targetId,
            Reason = reason,
            CustomReason = normalizedCustomReason,
            Status = ReportStatus.Pending,
            CreatedAt = clock.UtcNow
        });

        await unitOfWork.SaveChangesAsync();
    }

    private static void EnsureReasonAllowed(ReportReason reason, IReadOnlySet<ReportReason> allowedReasons)
    {
        if (!allowedReasons.Contains(reason))
            throw new InvalidOperationException("Report reason is not allowed for this target");
    }

    private static string? NormalizeCustomReason(ReportReason reason, string? customReason)
    {
        if (reason != ReportReason.Other)
            return null;

        if (string.IsNullOrWhiteSpace(customReason))
            throw new InvalidOperationException("Custom reason is required when reason is Other");

        return customReason.Trim();
    }
}
