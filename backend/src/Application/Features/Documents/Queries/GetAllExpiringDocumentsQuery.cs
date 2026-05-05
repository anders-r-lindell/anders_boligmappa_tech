using Application.Abstractions;
using Application.Cqrs;
using Domain.Entities;

namespace Application.Features.Documents.Queries;

public sealed record GetAllExpiringDocumentsQuery(Guid? CursorId = null, int PageSize = 20) : IQuery<IReadOnlyList<Document>>;

public sealed class GetAllExpiringDocumentsQueryHandler(IDocumentRepository repository)
    : IQueryHandler<GetAllExpiringDocumentsQuery, IReadOnlyList<Document>>
{
    private const int ExpiringWithinDays = 7; // Consider to add to configuration

    public Task<IReadOnlyList<Document>> HandleAsync(GetAllExpiringDocumentsQuery query, CancellationToken cancellationToken = default)
        => repository.GetAllExpiringAsync(ExpiringWithinDays, query.CursorId, query.PageSize, cancellationToken);
}