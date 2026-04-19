using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.DTO.Requests.Recipe;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;
using Recipes.Infrastructure.Helpers;

namespace Recipes.API.Endpoints;

public static class RecipeEndpoints
{
    public static void MapRecipeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/recipes", async (
                [FromForm] CreateRecipeWithFilesRequest request,
                ClaimsPrincipal user,
                IRecipeService recipeService,
                IMapper mapper) =>
            {
                try
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var ingredients =
                        JsonSerializer.Deserialize<List<CreateRecipeIngredientRequest>>(request.IngredientsJson)
                        ?? throw new InvalidOperationException("Invalid ingredients JSON");

                    var createRecipeDto = mapper.Map<CreateRecipeDto>(request);
                    createRecipeDto.CreatorId = userId;
                    createRecipeDto.Ingredients = mapper.Map<List<RecipeIngredientInputDto>>(ingredients);

                    if (request.Images != null)
                    {
                        var uploadedFiles = request.Images.Select(f => new FormFileWrapper(f));
                        var imageUploads = await recipeService.ProcessUploadedFilesAsync(uploadedFiles);
                        createRecipeDto.ImageUploads.AddRange(imageUploads);
                    }

                    var recipe = await recipeService.CreateRecipeAsync(createRecipeDto);

                    return Results.Created($"/api/recipes/{recipe.Id}", recipe);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        app.MapGet("/api/recipes/{id:guid}", async (
            Guid id,
            IRecipeService recipeService) =>
        {
            var recipe = await recipeService.GetRecipeByIdAsync(id);
            if (recipe == null) return Results.NotFound();

            return Results.Ok(recipe);
        });

        app.MapGet("/api/recipes", async (IRecipeService recipeService) =>
        {
            var recipes = await recipeService.GetAllRecipesAsync();
            return Results.Ok(recipes);
        });

        app.MapGet("/api/recipes/creator/{creatorId:guid}", async (
            Guid creatorId,
            IRecipeService recipeService) =>
        {
            var recipes = await recipeService.GetRecipesByCreatorIdAsync(creatorId);
            return Results.Ok(recipes);
        });

        app.MapPut("/api/recipes/{id:guid}", async (
                Guid id,
                [FromForm] UpdateRecipeWithFilesRequest request,
                ClaimsPrincipal user,
                IRecipeService recipeService,
                IMapper mapper) =>
            {
                try
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var existingRecipe = await recipeService.GetRecipeByIdAsync(id);
                    if (existingRecipe == null)
                        return Results.NotFound();

                    if (existingRecipe.CreatorId != userId)
                        return Results.Forbid();

                    var ingredients =
                        JsonSerializer.Deserialize<List<UpdateRecipeIngredientRequest>>(request.IngredientsJson)
                        ?? throw new InvalidOperationException("Invalid ingredients JSON");

                    var updateRecipeDto = mapper.Map<UpdateRecipeDto>(request);
                    updateRecipeDto.Id = id;
                    updateRecipeDto.Ingredients = mapper.Map<List<RecipeIngredientInputDto>>(ingredients);

                    if (request.Images != null)
                    {
                        var uploadedFiles = request.Images.Select(f => new FormFileWrapper(f));
                        var imageUploads = await recipeService.ProcessUploadedFilesAsync(uploadedFiles);
                        updateRecipeDto.ImageUploads.AddRange(imageUploads);
                    }

                    var recipe = await recipeService.UpdateRecipeAsync(updateRecipeDto);

                    return Results.Ok(recipe);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        app.MapDelete("/api/recipes/{id:guid}", async (
                Guid id,
                ClaimsPrincipal user,
                IRecipeService recipeService) =>
            {
                try
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var existingRecipe = await recipeService.GetRecipeByIdAsync(id);
                    if (existingRecipe == null)
                        return Results.NotFound();

                    if (existingRecipe.CreatorId != userId)
                        return Results.Forbid();

                    await recipeService.DeleteRecipeAsync(id);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .RequireAuthorization();

        app.MapPut("/api/recipes/{recipeId:guid}/like", async (
            Guid recipeId,
            [FromBody] ToggleLikeRequest request,
            ClaimsPrincipal user,
            IRecipeService recipeService) =>
        {
            try
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                await recipeService.ToggleLikeAsync(recipeId, userId, request.IsLiked);
                return Results.NoContent();
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
        
        app.MapPut("/api/recipes/{recipeId:guid}/favorite", async (
                Guid recipeId,
                [FromBody] ToggleFavoriteRequest request,
                ClaimsPrincipal user,
                IRecipeService recipeService) =>
            {
                try
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    await recipeService.ToggleFavoriteAsync(recipeId, userId, request.IsFavorite);
                    return Results.NoContent();
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
    }
}