using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.Helpers;
using Recipes.API.DTO.Requests;
using Recipes.API.DTO.Requests.Comment;
using Recipes.Application.DTO.Comment;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.Endpoints;

public static class CommentEndpoints
{
    public static void MapCommentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/recipes/{recipeId:guid}/comments", async (
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
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        app.MapPost("/api/recipes/{recipeId:guid}/comments", async (
                Guid recipeId,
                [FromForm] CreateCommentRequest request,
                ClaimsPrincipal user,
                ICommentService commentService,
                IMapper mapper) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var createCommentDto = mapper.Map<CreateCommentDto>(request);
                    createCommentDto.RecipeId = recipeId;
                    createCommentDto.CommentatorId = userId;

                    if (request.Images != null)
                    {
                        var imageUploads = await ImageUploadFactory.CreateManyAsync(request.Images);
                        createCommentDto.Images.AddRange(imageUploads);
                    }

                    var commentDto = await commentService.CreateCommentAsync(createCommentDto);
                    return Results.Created($"/api/recipes/{recipeId}/comments/{commentDto.Id}", commentDto);
                }
                catch (ArgumentException ex)
                {
                    return Results.NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        app.MapPut("/api/recipes/comments/{commentId:guid}", async (
                Guid commentId,
                [FromForm] UpdateCommentRequest request,
                ClaimsPrincipal user,
                ICommentService commentService,
                IMapper mapper) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var updateCommentDto = mapper.Map<UpdateCommentDto>(request);
                    updateCommentDto.CommentatorId = userId;
                    updateCommentDto.Id = commentId;

                    if (request.Images != null)
                    {
                        var imageUploads = await ImageUploadFactory.CreateManyAsync(request.Images);
                        updateCommentDto.Images.AddRange(imageUploads);
                    }

                    var commentDto = await commentService.UpdateCommentAsync(updateCommentDto);
                    return Results.Ok(commentDto);
                }
                catch (ArgumentException ex)
                {
                    return Results.NotFound(ex.Message);
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Forbid();
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        app.MapDelete("/api/recipes/comments/{commentId:guid}", async (
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
                catch (ArgumentException ex)
                {
                    return Results.NotFound(ex.Message);
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Forbid();
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .RequireAuthorization();
    }
}
