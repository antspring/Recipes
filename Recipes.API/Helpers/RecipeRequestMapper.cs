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
        dto.Ingredients = ingredients.Select(ToRecipeIngredientInputDto).ToList();

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
        var ingredients = DeserializeOptional<List<UpdateRecipeIngredientRequest>>(
            request.IngredientsJson,
            "Invalid ingredients JSON");

        var dto = mapper.Map<UpdateRecipeDto>(request);
        dto.Id = recipeId;
        dto.ActorUserId = actorUserId;
        dto.CookingTime = ParseOptionalCookingTime(request.CookingTime);
        dto.Ingredients = ingredients?
            .Select(ToRecipeIngredientInputDto)
            .ToList();

        dto.ImageIdsToDelete = DeserializeOptional<List<Guid>>(
            request.ImageIdsToDelete,
            "Invalid image ids JSON");

        if (request.Images != null)
            dto.ImageUploads.AddRange(await ImageUploadFactory.CreateManyAsync(request.Images));

        return dto;
    }

    public static RecipeSearchFilterDto ToRecipeSearchFilterDto(RecipeSearchRequest request)
    {
        return new RecipeSearchFilterDto
        {
            Title = NormalizeSearchText(request.Title),
            MealType = NormalizeSearchText(request.MealType),
            DishType = NormalizeSearchText(request.DishType),
            MaxCookingTime = request.MaxCookingTime,
            MinCalories = request.MinCalories,
            MaxCalories = request.MaxCalories,
            ExcludeUserAllergens = request.ExcludeUserAllergens ?? false,
            ContainsIngredientIds = NormalizeIngredientIds(request.ContainsIngredientIds),
            ExcludedIngredientIds = NormalizeIngredientIds(request.ExcludedIngredientIds)
        };
    }

    private static T DeserializeRequired<T>(string json, string errorMessage) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json)
                   ?? throw new InvalidOperationException(errorMessage);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(errorMessage, ex);
        }
    }

    private static RecipeIngredientInputDto ToRecipeIngredientInputDto(CreateRecipeIngredientRequest request)
    {
        return new RecipeIngredientInputDto
        {
            IngredientId = request.IngredientId,
            Weight = request.Weight,
            AlternativeWeight = request.AlternativeWeight
        };
    }

    private static RecipeIngredientInputDto ToRecipeIngredientInputDto(UpdateRecipeIngredientRequest request)
    {
        return new RecipeIngredientInputDto
        {
            IngredientId = request.IngredientId,
            Weight = request.Weight,
            AlternativeWeight = request.AlternativeWeight
        };
    }

    private static T? DeserializeOptional<T>(string? json, string errorMessage) where T : class
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<T>(json)
                   ?? throw new InvalidOperationException(errorMessage);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(errorMessage, ex);
        }
    }

    private static TimeSpan? ParseOptionalCookingTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!TimeSpan.TryParse(value, out var cookingTime))
            throw new InvalidOperationException("Invalid cooking time");

        if (cookingTime < TimeSpan.Zero || cookingTime >= TimeSpan.FromDays(1))
            throw new InvalidOperationException("CookingTime must be between 00:00:00 and 23:59:59");

        return cookingTime;
    }

    private static string? NormalizeSearchText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static IReadOnlyCollection<Guid> NormalizeIngredientIds(Guid[]? values)
    {
        if (values == null || values.Length == 0)
            return [];

        return values
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();
    }
}
