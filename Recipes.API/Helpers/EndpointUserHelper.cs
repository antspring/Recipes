using System.Security.Claims;

namespace Recipes.API.Helpers;

public static class EndpointUserHelper
{
    public static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdValue, out userId);
    }
}
