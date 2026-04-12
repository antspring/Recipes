using System.Security.Claims;
using Recipes.Domain.Models;

namespace Recipes.Application.Providers;

public class ClaimsProvider
{
    public List<Claim> GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.UserData, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        return claims;
    }
}