using Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApi.Authorization;

/// <summary>
/// Authorize user has access to / is owner of requested document
/// As an alternative to db look-up we can add property ids owned by person as seperate claim in token (owned properties should rarely change)
/// </summary>
public sealed class DocumentOwnerAuthorizationHandler(
    IHttpContextAccessor httpContextAccessor,
    IDocumentRepository documentRepository)
    : AuthorizationHandler<DocumentOwnerRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DocumentOwnerRequirement requirement)
    {
        var httpContext = httpContextAccessor.HttpContext!;
        var userHasRequirement = await UserHasRequirment(httpContext, context.User);

        if (userHasRequirement)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }

    private async Task<bool> UserHasRequirment(HttpContext httpContext, ClaimsPrincipal user)
    {
        if (!httpContext.Request.RouteValues.TryGetValue("documentId", out var documentIdRouteValue)
            || !Guid.TryParse(documentIdRouteValue?.ToString(), out var documentId))
        {
            return false;
        }

        if (!user.TryFindPersonIdClaim(out var personId))
        {
            return false;
        }

        var documentOwnerId = await documentRepository.GetOwnerByIdAsync(documentId);
        if (documentOwnerId != personId)
        {
            return false;
        }

        return true;
    }
}