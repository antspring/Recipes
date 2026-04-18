namespace Recipes.API.DTO.Requests.UserIngredient;

public class UnwantedIngredientRequest
{
    public List<Guid> IngredientIds { get; set; } = null!;
}