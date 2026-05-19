namespace Recipes.Application.Common;

public static class PaginationValidator
{
    public const int MaxPageSize = 100;

    public static void EnsureValid(int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than or equal to 1");

        if (pageSize < 1 || pageSize > MaxPageSize)
            throw new ArgumentException($"Page size must be between 1 and {MaxPageSize}");
    }
}
