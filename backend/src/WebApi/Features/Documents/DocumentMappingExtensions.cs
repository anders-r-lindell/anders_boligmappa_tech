using Domain.Abstractions;
using Domain.Entities;

namespace WebApi.Features.Documents;

internal static class DocumentMappingExtensions
{
    internal static DocumentResponse ToResponse(this Document document, IDateTimeProvider dateTimeProvider)
    {
        var reminderSnooze = document.ReminderSnooze;
        var reminderSnoozedUntil = reminderSnooze != null && reminderSnooze.SnoozedUntil >= dateTimeProvider.DateOnlyUtcNow() ? reminderSnooze.SnoozedUntil : (DateOnly?)null;

        return new(
                document.Id,
                document.PropertyId,
                document.Name,
                document.DocumentType.ToString(),
                document.ExpiryDate,
                reminderSnoozedUntil,
                document.CreatedAt.Value);
    }

    internal static DocumentListResponse ToListResponse(this IReadOnlyList<Document> documents, IDateTimeProvider dateTimeProvider) => new(
        documents.Select(d => d.ToResponse(dateTimeProvider)).ToList().AsReadOnly());
}
