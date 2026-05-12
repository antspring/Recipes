using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.DTO.Requests.Report;
using Recipes.API.Helpers;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var reportEndpoints = app.MapGroup("/api/reports").WithTags("Reports");

        reportEndpoints.MapPost("/recipes/{recipeId:guid}", async (
                Guid recipeId,
                [FromBody] CreateReportRequest request,
                ClaimsPrincipal user,
                IReportService reportService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var reason = ReportRequestMapper.ToReportReason(request);
                    await reportService.CreateRecipeReportAsync(recipeId, userId, reason, request.CustomReason);
                    return Results.NoContent();
                }
                catch (InvalidOperationException ex)
                {
                    return EndpointErrorHelper.BadRequest(ex);
                }
                catch (ArgumentException ex)
                {
                    return EndpointErrorHelper.NotFoundOrBadRequest(ex);
                }
            })
            .RequireAuthorization();

        reportEndpoints.MapPost("/users/{userId:guid}", async (
                Guid userId,
                [FromBody] CreateReportRequest request,
                ClaimsPrincipal user,
                IReportService reportService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var reporterId))
                    {
                        return Results.Unauthorized();
                    }

                    var reason = ReportRequestMapper.ToReportReason(request);
                    await reportService.CreateUserProfileReportAsync(userId, reporterId, reason, request.CustomReason);
                    return Results.NoContent();
                }
                catch (InvalidOperationException ex)
                {
                    return EndpointErrorHelper.BadRequest(ex);
                }
                catch (ArgumentException ex)
                {
                    return EndpointErrorHelper.NotFoundOrBadRequest(ex);
                }
            })
            .RequireAuthorization();

        reportEndpoints.MapPost("/comments/{commentId:guid}", async (
                Guid commentId,
                [FromBody] CreateReportRequest request,
                ClaimsPrincipal user,
                IReportService reportService) =>
            {
                try
                {
                    if (!EndpointUserHelper.TryGetUserId(user, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var reason = ReportRequestMapper.ToReportReason(request);
                    await reportService.CreateCommentReportAsync(commentId, userId, reason, request.CustomReason);
                    return Results.NoContent();
                }
                catch (InvalidOperationException ex)
                {
                    return EndpointErrorHelper.BadRequest(ex);
                }
                catch (ArgumentException ex)
                {
                    return EndpointErrorHelper.NotFoundOrBadRequest(ex);
                }
            })
            .RequireAuthorization();
    }
}
