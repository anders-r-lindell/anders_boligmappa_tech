using Domain.Entities;
using Domain.ValueObjects;

namespace Infrastructure.Persistence.DAOs;

internal static class DocumentMappingExtensions
{
    internal static Document ToDomain(this DocumentDao dao) =>
        Document.Reconstitute(
            dao.Id,
            dao.PropertyId,
            dao.Name,
            Enum.Parse<DocumentType>(dao.DocumentType),
            dao.ExpiryDate,
            UtcDateTime.Of(DateTime.SpecifyKind(dao.CreatedAt, DateTimeKind.Utc)),
            dao.ReminderSnooze?.ToDomain());
}
