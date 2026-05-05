using Application.Cqrs;
using Application.Features.Documents.Queries;
using Asp.Versioning;
using Domain.Abstractions;

namespace WebApi.Features.Documents;

public static class DocumentEndpoints
{
    public static IEndpointRouteBuilder MapDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .Build();

        app.MapGet("/api/v{version:apiVersion}/properties/{propertyId:guid}/documents/expiring", GetExpiringByPropertyIdAsync)
            .WithApiVersionSet(versionSet)
            .WithTags("Documents");

        return app;
    }

    private static async Task<IResult> GetExpiringByPropertyIdAsync(
        Guid propertyId, IQueryDispatcher dispatcher, IDateTimeProvider dateTimeProvider, CancellationToken ct)
    {
        var documents = await dispatcher.DispatchAsync(
            new GetExpiringDocumentsByPropertyIdQuery(propertyId), ct);

        return TypedResults.Ok(documents.ToListResponse(dateTimeProvider));
    }
}
