using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.DTO.Requests.Recipe;
using Recipes.API.Helpers;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.Endpoints;

public static class RecipeStepEndpoints
{
    public static void MapRecipeStepEndpoints(this IEndpointRouteBuilder app)
    {
        var stepEndpoints = app.MapGroup("/api/recipes/{recipeId:guid}/steps").WithTags("Recipe steps");

        stepEndpoints.MapGet(string.Empty, async (
            Guid recipeId,
            IRecipeStepService recipeStepService) =>
        {
            try
            {
                var steps = await recipeStepService.GetStepsByRecipeIdAsync(recipeId);
                return Results.Ok(steps);
            }
            catch (ArgumentException ex)
            {
                return EndpointErrorHelper.NotFoundOrBadRequest(ex);
            }
        });

        stepEndpoints.MapPost(string.Empty, async (
                Guid recipeId,
                [FromForm] CreateRecipeStepRequest request,
                ClaimsPrincipal user,
                IRecipeStepService recipeStepService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var dto = await RecipeStepRequestMapper.ToCreateRecipeStepDtoAsync(request, recipeId, userId);
                    var step = await recipeStepService.CreateStepAsync(dto);

                    return Results.Created($"/api/recipes/{recipeId}/steps/{step.Id}", step);
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

        stepEndpoints.MapPut("/{stepId:guid}", async (
                Guid recipeId,
                Guid stepId,
                [FromForm] UpdateRecipeStepRequest request,
                ClaimsPrincipal user,
                IRecipeStepService recipeStepService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var dto = await RecipeStepRequestMapper.ToUpdateRecipeStepDtoAsync(
                        request,
                        recipeId,
                        stepId,
                        userId);

                    var step = await recipeStepService.UpdateStepAsync(dto);
                    return Results.Ok(step);
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

        stepEndpoints.MapDelete("/{stepId:guid}", async (
                Guid recipeId,
                Guid stepId,
                ClaimsPrincipal user,
                IRecipeStepService recipeStepService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    await recipeStepService.DeleteStepAsync(recipeId, stepId, userId);
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

        stepEndpoints.MapPut("/reorder", async (
                Guid recipeId,
                [FromBody] ReorderRecipeStepsRequest request,
                ClaimsPrincipal user,
                IRecipeStepService recipeStepService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var dto = RecipeStepRequestMapper.ToReorderRecipeStepsDto(request, recipeId, userId);
                    var steps = await recipeStepService.ReorderStepsAsync(dto);

                    return Results.Ok(steps);
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
