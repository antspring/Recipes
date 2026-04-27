using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.Helpers;
using Recipes.API.DTO.Requests.Comment;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.Endpoints;

public static class CommentEndpoints
{
    public static void MapCommentEndpoints(this IEndpointRouteBuilder app)
    {
        var commentEndpoints = app.MapGroup("/api/recipes").WithTags("Comments");

        commentEndpoints.MapGet("/{recipeId:guid}/comments", async (
            Guid recipeId,
            ICommentService commentService,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null) =>
        {
            try
            {
                var result = await commentService.GetCommentsByRecipeIdAsync(recipeId, page, pageSize, from, to);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return EndpointErrorHelper.BadRequest(ex);
            }
        });

        commentEndpoints.MapPost("/{recipeId:guid}/comments", async (
                Guid recipeId,
                [FromForm] CreateCommentRequest request,
                ClaimsPrincipal user,
                ICommentService commentService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var createCommentDto =
                        await CommentRequestMapper.ToCreateCommentDtoAsync(request, recipeId, userId);

                    var commentDto = await commentService.CreateCommentAsync(createCommentDto);
                    return Results.Created($"/api/recipes/{recipeId}/comments/{commentDto.Id}", commentDto);
                }
                catch (InvalidOperationException ex)
                {
                    return EndpointErrorHelper.NotFoundOrBadRequest(ex);
                }
                catch (ArgumentException ex)
                {
                    return EndpointErrorHelper.NotFoundOrBadRequest(ex);
                }
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        commentEndpoints.MapPut("/comments/{commentId:guid}", async (
                Guid commentId,
                [FromForm] UpdateCommentRequest request,
                ClaimsPrincipal user,
                ICommentService commentService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var updateCommentDto =
                        await CommentRequestMapper.ToUpdateCommentDtoAsync(request, commentId, userId);

                    var commentDto = await commentService.UpdateCommentAsync(updateCommentDto);
                    return Results.Ok(commentDto);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return EndpointErrorHelper.ForbiddenNotFoundOrBadRequest(ex);
                }
                catch (InvalidOperationException ex)
                {
                    return EndpointErrorHelper.ForbiddenNotFoundOrBadRequest(ex);
                }
                catch (ArgumentException ex)
                {
                    return EndpointErrorHelper.ForbiddenNotFoundOrBadRequest(ex);
                }
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        commentEndpoints.MapDelete("/comments/{commentId:guid}", async (
                Guid commentId,
                ClaimsPrincipal user,
                ICommentService commentService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    await commentService.DeleteCommentAsync(commentId, userId);
                    return Results.NoContent();
                }
                catch (UnauthorizedAccessException ex)
                {
                    return EndpointErrorHelper.ForbiddenNotFoundOrBadRequest(ex);
                }
                catch (ArgumentException ex)
                {
                    return EndpointErrorHelper.ForbiddenNotFoundOrBadRequest(ex);
                }
            })
            .RequireAuthorization();
    }
}
