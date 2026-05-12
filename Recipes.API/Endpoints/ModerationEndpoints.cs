using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.DTO.Requests.Moderation;
using Recipes.API.Helpers;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.Endpoints;

public static class ModerationEndpoints
{
    public static void MapModerationEndpoints(this IEndpointRouteBuilder app)
    {
        var moderationEndpoints = app.MapGroup("/api/moderation").WithTags("Moderation")
            .RequireAuthorization("Moderator");

        moderationEndpoints.MapGet("/reports", async (
            IModerationReportService moderationReportService,
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
        {
            try
            {
                var reportStatus = ModerationRequestMapper.ToReportStatus(status);
                var reports = await moderationReportService.GetReportsAsync(reportStatus, page, pageSize);
                return Results.Ok(reports);
            }
            catch (InvalidOperationException ex)
            {
                return EndpointErrorHelper.BadRequest(ex);
            }
        });

        moderationEndpoints.MapGet("/reports/{reportId:guid}", async (
            Guid reportId,
            ClaimsPrincipal user,
            IModerationReportService moderationReportService) =>
        {
            try
            {
                if (!EndpointUserHelper.TryGetUserId(user, out var moderatorId))
                {
                    return Results.Unauthorized();
                }

                var report = await moderationReportService.GetReportAsync(reportId, moderatorId);
                return Results.Ok(report);
            }
            catch (ArgumentException ex)
            {
                return EndpointErrorHelper.NotFoundOrBadRequest(ex);
            }
        });

        moderationEndpoints.MapPost("/reports/{reportId:guid}/dismiss", async (
            Guid reportId,
            [FromBody] DismissReportRequest request,
            ClaimsPrincipal user,
            IModerationReportService moderationReportService) =>
        {
            try
            {
                if (!EndpointUserHelper.TryGetUserId(user, out var moderatorId))
                {
                    return Results.Unauthorized();
                }

                await moderationReportService.DismissAsync(reportId, moderatorId, request.ResolutionComment);
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
        });
    }
}
