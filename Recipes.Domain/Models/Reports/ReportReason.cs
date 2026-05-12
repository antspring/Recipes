namespace Recipes.Domain.Models.Reports;

public enum ReportReason
{
    PhotoDoesNotMatchRecipe = 0,
    DescriptionDoesNotMatchRecipe = 1,
    CommunityRulesViolation = 2,
    InappropriatePhoto = 3,
    OffensiveContent = 4,
    Other = 5
}
