using AutoMapper;
using Recipes.API.DTO.Requests.Comment;
using Recipes.Application.DTO.Comment;

namespace Recipes.API.Helpers;

public static class CommentRequestMapper
{
    public static async Task<CreateCommentDto> ToCreateCommentDtoAsync(
        CreateCommentRequest request,
        Guid recipeId,
        Guid commentatorId,
        IMapper mapper)
    {
        var dto = mapper.Map<CreateCommentDto>(request);
        dto.RecipeId = recipeId;
        dto.CommentatorId = commentatorId;

        if (request.Images != null)
            dto.Images.AddRange(await ImageUploadFactory.CreateManyAsync(request.Images));

        return dto;
    }

    public static async Task<UpdateCommentDto> ToUpdateCommentDtoAsync(
        UpdateCommentRequest request,
        Guid commentId,
        Guid commentatorId,
        IMapper mapper)
    {
        var dto = mapper.Map<UpdateCommentDto>(request);
        dto.Id = commentId;
        dto.CommentatorId = commentatorId;

        if (request.Images != null)
            dto.Images.AddRange(await ImageUploadFactory.CreateManyAsync(request.Images));

        return dto;
    }
}
