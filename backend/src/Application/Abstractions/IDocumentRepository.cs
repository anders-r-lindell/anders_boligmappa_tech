using Domain.Entities;

namespace Application.Abstractions;

public interface IDocumentRepository
{
    Task<IReadOnlyList<Document>> GetExpiringByPropertyIdAsync(Guid propertyId, int withinDays, CancellationToken cancellationToken = default);
}
