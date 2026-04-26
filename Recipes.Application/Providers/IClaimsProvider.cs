using System.Security.Claims;
using Recipes.Domain.Models;

namespace Recipes.Application.Providers;

public interface IClaimsProvider
{
    IReadOnlyList<Claim> GetClaims(User user);
}
