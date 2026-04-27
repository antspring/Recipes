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
        var recipeEndpoints = app.MapGroup("/api/recipes").WithTags("Recipes");

        recipeEndpoints.MapPost(string.Empty, async (
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

        recipeEndpoints.MapGet("/{id:guid}", async (
            Guid id,
            [FromQuery] string? include,
            IRecipeCrudService recipeCrudService) =>
        {
            try
            {
                var recipeIncludes = RecipeIncludesRequestParser.Parse(include);
                var recipe = await recipeCrudService.GetRecipeByIdAsync(id, recipeIncludes);
                if (recipe == null) return Results.NotFound();

                return Results.Ok(recipe);
            }
            catch (ArgumentException ex)
            {
                return EndpointErrorHelper.BadRequest(ex);
            }
        });

        recipeEndpoints.MapGet(string.Empty, async (
            [FromQuery] string? include,
            IRecipeCrudService recipeCrudService) =>
        {
            try
            {
                var recipeIncludes = RecipeIncludesRequestParser.Parse(include);
                var recipes = await recipeCrudService.GetAllRecipesAsync(recipeIncludes);
                return Results.Ok(recipes);
            }
            catch (ArgumentException ex)
            {
                return EndpointErrorHelper.BadRequest(ex);
            }
        });

        recipeEndpoints.MapGet("/creator/{creatorId:guid}", async (
            Guid creatorId,
            [FromQuery] string? include,
            IRecipeCrudService recipeCrudService) =>
        {
            try
            {
                var recipeIncludes = RecipeIncludesRequestParser.Parse(include);
                var recipes = await recipeCrudService.GetRecipesByCreatorIdAsync(creatorId, recipeIncludes);
                return Results.Ok(recipes);
            }
            catch (ArgumentException ex)
            {
                return EndpointErrorHelper.BadRequest(ex);
            }
        });

        recipeEndpoints.MapPut("/{id:guid}", async (
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

        recipeEndpoints.MapDelete("/{id:guid}", async (
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

        recipeEndpoints.MapPut("/{recipeId:guid}/like", async (
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

        recipeEndpoints.MapPut("/{recipeId:guid}/favorite", async (
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
