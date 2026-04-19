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
                IRecipeCrudService recipeCrudService,
                IFileProcessingService fileProcessingService,
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
                        var imageUploads = await fileProcessingService.ProcessUploadedFilesAsync(uploadedFiles);
                        createRecipeDto.ImageUploads.AddRange(imageUploads);
                    }

                    var recipe = await recipeCrudService.CreateRecipeAsync(createRecipeDto);

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
                IFileProcessingService fileProcessingService,
                IMapper mapper) =>
            {
                try
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var existingRecipe = await recipeCrudService.GetRecipeByIdAsync(id);
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
                        var imageUploads = await fileProcessingService.ProcessUploadedFilesAsync(uploadedFiles);
                        updateRecipeDto.ImageUploads.AddRange(imageUploads);
                    }

                    var recipe = await recipeCrudService.UpdateRecipeAsync(updateRecipeDto);

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
                IRecipeCrudService recipeCrudService) =>
            {
                try
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var existingRecipe = await recipeCrudService.GetRecipeByIdAsync(id);
                    if (existingRecipe == null)
                        return Results.NotFound();

                    if (existingRecipe.CreatorId != userId)
                        return Results.Forbid();

                    await recipeCrudService.DeleteRecipeAsync(id);
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
            IRecipeInteractionService recipeInteractionService) =>
        {
            try
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                await recipeInteractionService.ToggleLikeAsync(recipeId, userId, request.IsLiked);
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
                IRecipeInteractionService recipeInteractionService) =>
            {
                try
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    await recipeInteractionService.ToggleFavoriteAsync(recipeId, userId, request.IsFavorite);
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