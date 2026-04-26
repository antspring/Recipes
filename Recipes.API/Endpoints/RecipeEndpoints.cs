using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.Helpers;
using Recipes.API.DTO.Requests.Recipe;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.Endpoints;

public static class RecipeEndpoints
{
    public static void MapRecipeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/recipes", async (
                [FromForm] CreateRecipeWithFilesRequest request,
                ClaimsPrincipal user,
                IRecipeCrudService recipeCrudService,
                IMapper mapper) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var createRecipeDto = await RecipeRequestMapper.ToCreateRecipeDtoAsync(request, userId, mapper);

                    var recipe = await recipeCrudService.CreateRecipeAsync(createRecipeDto);

                    return Results.Created($"/api/recipes/{recipe.Id}", recipe);
                }
                catch (InvalidOperationException ex)
                {
                    return EndpointErrorHelper.BadRequest(ex);
                }
                catch (ArgumentException ex)
                {
                    return EndpointErrorHelper.BadRequest(ex);
                }
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        app.MapGet("/api/recipes/{id:guid}", async (
            Guid id,
            IRecipeCrudService recipeCrudService) =>
        {
            var recipe = await recipeCrudService.GetRecipeByIdAsync(id);
            if (recipe == null) return Results.NotFound();

            return Results.Ok(recipe);
        });

        app.MapGet("/api/recipes", async (IRecipeCrudService recipeCrudService) =>
        {
            var recipes = await recipeCrudService.GetAllRecipesAsync();
            return Results.Ok(recipes);
        });

        app.MapGet("/api/recipes/creator/{creatorId:guid}", async (
            Guid creatorId,
            IRecipeCrudService recipeCrudService) =>
        {
            var recipes = await recipeCrudService.GetRecipesByCreatorIdAsync(creatorId);
            return Results.Ok(recipes);
        });

        app.MapPut("/api/recipes/{id:guid}", async (
                Guid id,
                [FromForm] UpdateRecipeWithFilesRequest request,
                ClaimsPrincipal user,
                IRecipeCrudService recipeCrudService,
                IMapper mapper) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var updateRecipeDto = await RecipeRequestMapper.ToUpdateRecipeDtoAsync(request, id, userId, mapper);

                    var recipe = await recipeCrudService.UpdateRecipeAsync(updateRecipeDto);

                    return Results.Ok(recipe);
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

        app.MapDelete("/api/recipes/{id:guid}", async (
                Guid id,
                ClaimsPrincipal user,
                IRecipeCrudService recipeCrudService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    await recipeCrudService.DeleteRecipeAsync(id, userId);
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

        app.MapPut("/api/recipes/{recipeId:guid}/like", async (
                Guid recipeId,
                [FromBody] ToggleLikeRequest request,
                ClaimsPrincipal user,
                IRecipeInteractionService recipeInteractionService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    await recipeInteractionService.ToggleLikeAsync(recipeId, userId, request.IsLiked);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return EndpointErrorHelper.NotFoundOrBadRequest(ex);
                }
            })
            .RequireAuthorization();

        app.MapPut("/api/recipes/{recipeId:guid}/favorite", async (
                Guid recipeId,
                [FromBody] ToggleFavoriteRequest request,
                ClaimsPrincipal user,
                IRecipeInteractionService recipeInteractionService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    await recipeInteractionService.ToggleFavoriteAsync(recipeId, userId, request.IsFavorite);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return EndpointErrorHelper.NotFoundOrBadRequest(ex);
                }
            })
            .RequireAuthorization();
    }
}
