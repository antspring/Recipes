using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.DTO.Requests;
using Recipes.API.DTO.Requests.Recipe;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.Endpoints;

public static class RecipeEndpoints
{
    public static void MapRecipeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/recipes", async (
                [FromForm] CreateRecipeWithFilesRequest request,
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

                    var ingredients =
                        JsonSerializer.Deserialize<List<CreateRecipeIngredientRequest>>(request.IngredientsJson)
                        ?? throw new InvalidOperationException("Invalid ingredients JSON");

                    var createRecipeDto = new CreateRecipeDto
                    {
                        Title = request.Title,
                        Description = request.Description,
                        CaloricValue = request.CaloricValue,
                        Proteins = request.Proteins,
                        Fats = request.Fats,
                        Carbohydrates = request.Carbohydrates,
                        CreatorId = userId,
                        Ingredients = ingredients.Select(i => new CreateRecipeIngredientDto
                        {
                            IngredientId = i.IngredientId,
                            Weight = i.Weight,
                            AlternativeWeight = i.AlternativeWeight
                        }).ToList()
                    };

                    if (request.Images != null)
                    {
                        foreach (var image in request.Images)
                        {
                            await using var stream = image.OpenReadStream();
                            var memoryStream = new MemoryStream();
                            await stream.CopyToAsync(memoryStream);
                            memoryStream.Position = 0;

                            createRecipeDto.ImageUploads.Add(new ImageUpload
                            {
                                Stream = memoryStream,
                                FileName = image.FileName,
                                ContentType = image.ContentType
                            });
                        }
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

                    var ingredients =
                        JsonSerializer.Deserialize<List<UpdateRecipeIngredientRequest>>(request.IngredientsJson)
                        ?? throw new InvalidOperationException("Invalid ingredients JSON");

                    var updateRecipeDto = new UpdateRecipeDto
                    {
                        Id = id,
                        Title = request.Title,
                        Description = request.Description,
                        CaloricValue = request.CaloricValue,
                        Proteins = request.Proteins,
                        Fats = request.Fats,
                        Carbohydrates = request.Carbohydrates,
                        Ingredients = ingredients.Select(i => new UpdateRecipeIngredientDto
                        {
                            IngredientId = i.IngredientId,
                            Weight = i.Weight,
                            AlternativeWeight = i.AlternativeWeight
                        }).ToList()
                    };

                    if (request.Images != null)
                    {
                        foreach (var image in request.Images)
                        {
                            await using var stream = image.OpenReadStream();
                            var memoryStream = new MemoryStream();
                            await stream.CopyToAsync(memoryStream);
                            memoryStream.Position = 0;

                            updateRecipeDto.ImageUploads.Add(new ImageUpload
                            {
                                Stream = memoryStream,
                                FileName = image.FileName,
                                ContentType = image.ContentType
                            });
                        }
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
    }
}