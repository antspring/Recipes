using Recipes.Application.Common;

namespace Recipes.API.Helpers;

public static class RecipeIncludesRequestParser
{
    private static readonly IReadOnlyDictionary<string, RecipeIncludes> IncludeByName =
        new Dictionary<string, RecipeIncludes>(StringComparer.OrdinalIgnoreCase)
        {
            ["creator"] = RecipeIncludes.Creator,
            ["ingredients"] = RecipeIncludes.Ingredients,
            ["images"] = RecipeIncludes.Images,
            ["steps"] = RecipeIncludes.Steps,
            ["full"] = RecipeIncludes.Full
        };

    public static RecipeIncludes? Parse(string? include)
    {
        if (include == null)
            return null;

        if (string.IsNullOrWhiteSpace(include))
            return RecipeIncludes.None;

        var includes = RecipeIncludes.None;
        var names = include.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var name in names)
        {
            if (!IncludeByName.TryGetValue(name, out var value))
                throw new ArgumentException($"Unknown recipe include: {name}");

            includes |= value;
        }

        return includes;
    }
}
