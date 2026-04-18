using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.DTO.Requests;
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
                [FromBody] CreateCommentRequest request,
                ClaimsPrincipal user,
                ICommentService commentService) =>
            {
                try
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var commentDto = await commentService.CreateCommentAsync(recipeId, userId, request.Value);
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
            .RequireAuthorization();
        
        app.MapPut("/api/recipes/comments/{commentId:guid}", async (
                Guid commentId,
                [FromBody] UpdateCommentRequest request,
                ClaimsPrincipal user,
                ICommentService commentService) =>
            {
                try
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var commentDto = await commentService.UpdateCommentAsync(commentId, userId, request.Value);
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
            .RequireAuthorization();
        
        app.MapDelete("/api/recipes/comments/{commentId:guid}", async (
                Guid commentId,
                ClaimsPrincipal user,
                ICommentService commentService) =>
            {
                try
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
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