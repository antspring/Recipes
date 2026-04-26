using System.Text.Json;
using AutoMapper;
using Recipes.API.DTO.Requests.Recipe;
using Recipes.Application.DTO.Recipe;

namespace Recipes.API.Helpers;

public static class RecipeRequestMapper
{
    public static async Task<CreateRecipeDto> ToCreateRecipeDtoAsync(
        CreateRecipeWithFilesRequest request,
        Guid creatorId,
        IMapper mapper)
    {
        var ingredients = DeserializeRequired<List<CreateRecipeIngredientRequest>>(
            request.IngredientsJson,
            "Invalid ingredients JSON");

        var dto = mapper.Map<CreateRecipeDto>(request);
        dto.CreatorId = creatorId;
        dto.Ingredients = mapper.Map<List<RecipeIngredientInputDto>>(ingredients);

        if (request.Images != null)
            dto.ImageUploads.AddRange(await ImageUploadFactory.CreateManyAsync(request.Images));

        return dto;
    }

    public static async Task<UpdateRecipeDto> ToUpdateRecipeDtoAsync(
        UpdateRecipeWithFilesRequest request,
        Guid recipeId,
        Guid actorUserId,
        IMapper mapper)
    {
        var ingredients = DeserializeRequired<List<UpdateRecipeIngredientRequest>>(
            request.IngredientsJson,
            "Invalid ingredients JSON");

        var dto = mapper.Map<UpdateRecipeDto>(request);
        dto.Id = recipeId;
        dto.ActorUserId = actorUserId;
        dto.Ingredients = mapper.Map<List<RecipeIngredientInputDto>>(ingredients);

        if (!string.IsNullOrEmpty(request.ImageIdsToDelete))
            dto.ImageIdsToDelete = JsonSerializer.Deserialize<List<Guid>>(request.ImageIdsToDelete) ?? new List<Guid>();

        if (request.Images != null)
            dto.ImageUploads.AddRange(await ImageUploadFactory.CreateManyAsync(request.Images));

        return dto;
    }

    private static T DeserializeRequired<T>(string json, string errorMessage) where T : class
    {
        return JsonSerializer.Deserialize<T>(json)
               ?? throw new InvalidOperationException(errorMessage);
    }
}
