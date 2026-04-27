namespace Recipes.Application.Common;

[Flags]
public enum RecipeIncludes
{
    None = 0,
    Creator = 1 << 0,
    Ingredients = 1 << 1,
    Images = 1 << 2,
    Steps = 1 << 3,
    Full = Creator | Ingredients | Images | Steps
}
