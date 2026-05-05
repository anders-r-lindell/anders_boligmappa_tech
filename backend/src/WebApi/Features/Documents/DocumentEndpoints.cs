using Application.Cqrs;
using Application.Features.Documents.Commands;
using Application.Features.Documents.Queries;
using Asp.Versioning;
using Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Features.Documents;

public static class DocumentEndpoints
{
    private readonly static string TagName = "Documents";

    public static IEndpointRouteBuilder MapDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .Build();

        app.MapGet("/api/v{version:apiVersion}/properties/{propertyId:guid}/documents/expiring", GetExpiringByPropertyIdAsync)
            .WithApiVersionSet(versionSet)
            .WithTags(TagName);

        app.MapGet("/api/v{version:apiVersion}/documents/expiring", GetAllExpiringAsync)
            .WithApiVersionSet(versionSet)
            .WithTags(TagName)
            .AllowAnonymous();

        app.MapPost("/api/v{version:apiVersion}/documents/{documentId:guid}/snooze", SnoozeAsync)
            .WithApiVersionSet(versionSet)
            .WithTags(TagName)
            .RequireAuthorization("DocumentOwner");

        return app;
    }

    private static async Task<IResult> SnoozeAsync(
    Guid documentId, ICommandDispatcher dispatcher, IDateTimeProvider dateTimeProvider, CancellationToken ct)
    {
        var document = await dispatcher.DispatchAsync(new SnoozeDocumentCommand(documentId), ct);
        return TypedResults.Ok(document.ToResponse(dateTimeProvider));
    }

    private static async Task<IResult> GetExpiringByPropertyIdAsync(
        Guid propertyId, IQueryDispatcher dispatcher, IDateTimeProvider dateTimeProvider, CancellationToken ct)
    {
        var documents = await dispatcher.DispatchAsync(
            new GetExpiringDocumentsByPropertyIdQuery(propertyId), ct);

        return TypedResults.Ok(documents.ToListResponse(dateTimeProvider));
    }

    private static async Task<IResult> GetAllExpiringAsync(
        IQueryDispatcher dispatcher, IDateTimeProvider dateTimeProvider, CancellationToken ct,
        Guid? cursorId = null, int pageSize = 20)
    {
        var documents = await dispatcher.DispatchAsync(new GetAllExpiringDocumentsQuery(cursorId, pageSize), ct);
        return TypedResults.Ok(documents.ToListWithCursorResponse(pageSize, dateTimeProvider));
    }
}
