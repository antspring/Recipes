using System.Text.Json;
using Recipes.API.DTO.Requests.Comment;
using Recipes.Application.DTO.Comment;

namespace Recipes.API.Helpers;

public static class CommentRequestMapper
{
    public static async Task<CreateCommentDto> ToCreateCommentDtoAsync(
        CreateCommentRequest request,
        Guid recipeId,
        Guid commentatorId)
    {
        var dto = new CreateCommentDto
        {
            RecipeId = recipeId,
            CommentatorId = commentatorId,
            ParentCommentId = request.ParentCommentId,
            Value = request.Value
        };

        if (request.Images != null)
            dto.Images.AddRange(await ImageUploadFactory.CreateManyAsync(request.Images));

        return dto;
    }

    public static async Task<UpdateCommentDto> ToUpdateCommentDtoAsync(
        UpdateCommentRequest request,
        Guid commentId,
        Guid commentatorId)
    {
        var dto = new UpdateCommentDto
        {
            Id = commentId,
            CommentatorId = commentatorId,
            Value = request.Value,
            ImageIdsToDelete = DeserializeImageIdsToDelete(request.ImageIdsToDelete)
        };

        if (request.Images != null)
            dto.Images.AddRange(await ImageUploadFactory.CreateManyAsync(request.Images));

        return dto;
    }

    private static List<Guid> DeserializeImageIdsToDelete(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<Guid>();

        try
        {
            return JsonSerializer.Deserialize<List<Guid>>(json) ?? new List<Guid>();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid image ids JSON", ex);
        }
    }
}
