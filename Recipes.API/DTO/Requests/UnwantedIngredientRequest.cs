namespace Recipes.API.DTO.Requests;

public class UnwantedIngredientRequest
{
    public List<Guid> IngredientIds { get; set; } = null!;
}