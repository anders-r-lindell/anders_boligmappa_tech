using Application.Abstractions;
using Application.Cqrs;
using Domain.Entities;

namespace Application.Features.Documents.Queries;

public sealed record GetExpiringDocumentsByPropertyIdQuery(Guid PropertyId) : IQuery<IReadOnlyList<Document>>;

public sealed class GetExpiringDocumentsQueryHandler(IDocumentRepository repository)
    : IQueryHandler<GetExpiringDocumentsByPropertyIdQuery, IReadOnlyList<Document>>
{
    private const int ExpiringWithinDays = 90; // Consider to add to configuration

    public Task<IReadOnlyList<Document>> HandleAsync(GetExpiringDocumentsByPropertyIdQuery query, CancellationToken cancellationToken = default)
        => repository.GetExpiringByPropertyIdAsync(query.PropertyId, ExpiringWithinDays, cancellationToken);
}
