using Domain.Entities;

namespace Application.Abstractions;

public interface IDocumentRepository
{
    Task<IReadOnlyList<Document>> GetExpiringByPropertyIdAsync(Guid propertyId, int withinDays, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Document>> GetAllExpiringAsync(int withinDays, Guid? cursorId = null, int pageSize = 20, CancellationToken cancellationToken = default);

    Task UpdateReminderSnoozeAsync(Document document, CancellationToken cancellationToken = default);

    Task<Guid?> GetOwnerByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
