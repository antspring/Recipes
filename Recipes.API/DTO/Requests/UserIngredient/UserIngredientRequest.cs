namespace Recipes.API.DTO.Requests.UserIngredient;

public class UserIngredientRequest
{
    public List<Guid> IngredientIds { get; set; } = null!;
}
