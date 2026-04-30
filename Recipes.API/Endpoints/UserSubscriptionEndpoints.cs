using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Recipes.API.DTO.Requests.User;
using Recipes.API.DTO.Responses.User;
using Recipes.API.Helpers;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.Endpoints;

public static class UserSubscriptionEndpoints
{
    public static void MapUserSubscriptionEndpoints(this IEndpointRouteBuilder app)
    {
        var userEndpoints = app.MapGroup("/api/users").WithTags("Users");

        userEndpoints.MapGet("/rating/top", async (
            ClaimsPrincipal user,
            IUserRatingService userRatingService,
            IImageUrlProvider imageUrlProvider) =>
        {
            var currentUserId = EndpointUserHelper.TryGetUserId(user, out var actorUserId)
                ? actorUserId
                : (Guid?)null;
            var topUsers = await userRatingService.GetTopAsync(currentUserId);

            return Results.Ok(topUsers.Select(topUser => new UserRatingResponse(topUser, imageUrlProvider)));
        });

        userEndpoints.MapGet("/{userId:guid}", async (
            Guid userId,
            ClaimsPrincipal user,
            IUserPublicProfileService userPublicProfileService,
            IImageUrlProvider imageUrlProvider) =>
        {
            try
            {
                var currentUserId = EndpointUserHelper.TryGetUserId(user, out var actorUserId)
                    ? actorUserId
                    : (Guid?)null;
                var profile = await userPublicProfileService.GetProfileAsync(userId, currentUserId);

                return Results.Ok(new PublicUserProfileResponse(profile, imageUrlProvider));
            }
            catch (ArgumentException ex)
            {
                return EndpointErrorHelper.NotFoundOrBadRequest(ex);
            }
        });

        userEndpoints.MapPut("/{userId:guid}/subscription", async (
                Guid userId,
                [FromBody] ToggleUserSubscriptionRequest request,
                ClaimsPrincipal user,
                IUserSubscriptionService userSubscriptionService) =>
            {
                if (!EndpointUserHelper.TryGetUserId(user, out var currentUserId))
                {
                    return Results.Unauthorized();
                }

                try
                {
                    await userSubscriptionService.SetSubscriptionAsync(
                        currentUserId,
                        userId,
                        request.IsSubscribed);

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

        userEndpoints.MapGet("/{userId:guid}/followers", async (
            Guid userId,
            ClaimsPrincipal user,
            IUserSubscriptionService userSubscriptionService,
            IImageUrlProvider imageUrlProvider) =>
        {
            try
            {
                var currentUserId = EndpointUserHelper.TryGetUserId(user, out var actorUserId)
                    ? actorUserId
                    : (Guid?)null;
                var followers = await userSubscriptionService.GetFollowersAsync(userId, currentUserId);

                return Results.Ok(followers.Select(f => new PublicUserResponse(f, imageUrlProvider)));
            }
            catch (ArgumentException ex)
            {
                return EndpointErrorHelper.NotFoundOrBadRequest(ex);
            }
        });

        userEndpoints.MapGet("/{userId:guid}/following", async (
            Guid userId,
            ClaimsPrincipal user,
            IUserSubscriptionService userSubscriptionService,
            IImageUrlProvider imageUrlProvider) =>
        {
            try
            {
                var currentUserId = EndpointUserHelper.TryGetUserId(user, out var actorUserId)
                    ? actorUserId
                    : (Guid?)null;
                var following = await userSubscriptionService.GetFollowingAsync(userId, currentUserId);

                return Results.Ok(following.Select(f => new PublicUserResponse(f, imageUrlProvider)));
            }
            catch (ArgumentException ex)
            {
                return EndpointErrorHelper.NotFoundOrBadRequest(ex);
            }
        });

        userEndpoints.MapGet("/{userId:guid}/subscription-stats", async (
            Guid userId,
            ClaimsPrincipal user,
            IUserSubscriptionService userSubscriptionService) =>
        {
            try
            {
                var currentUserId = EndpointUserHelper.TryGetUserId(user, out var actorUserId)
                    ? actorUserId
                    : (Guid?)null;

                return Results.Ok(await userSubscriptionService.GetStatsAsync(userId, currentUserId));
            }
            catch (ArgumentException ex)
            {
                return EndpointErrorHelper.NotFoundOrBadRequest(ex);
            }
        });
    }
}
