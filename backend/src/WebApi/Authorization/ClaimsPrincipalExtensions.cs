using System.Security.Claims;

namespace WebApi.Authorization;

internal static class ClaimsPrincipalExtensions
{
    internal static bool TryFindPersonIdClaim(this ClaimsPrincipal user, out Guid? personId)
    {
        personId = null;
        var personIdClaim = user.FindFirst("person-id");
        if (personIdClaim is null || !Guid.TryParse(personIdClaim.Value, out var guidValue))
        {
            return false;
        }

        personId = guidValue;
        return true;
    }
}