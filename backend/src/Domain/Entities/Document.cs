using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Document
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DocumentType DocumentType { get; private set; }
    public DateOnly? ExpiryDate { get; private set; }
    public UtcDateTime CreatedAt { get; private set; }
    public ReminderSnooze? ReminderSnooze { get; private set; }

    public void Snooze(IDateTimeProvider dateTimeProvider)
    {
        if (ReminderSnooze is null)
        {
            ReminderSnooze = ReminderSnooze.Create(Id, dateTimeProvider);
        }
        else
        {
            ReminderSnooze.Extend(dateTimeProvider);
        }
    }

    public static Document Create(Guid propertyId, string name, DocumentType documentType, DateOnly? expiryDate, IDateTimeProvider dateTimeProvider)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(propertyId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > 256)
        {
            throw new ArgumentException("Name must not exceed 256 characters.", nameof(name));
        }

        return new Document
        {
            Id = Guid.CreateVersion7(),
            PropertyId = propertyId,
            Name = name,
            DocumentType = documentType,
            ExpiryDate = expiryDate,
            CreatedAt = UtcDateTime.Of(dateTimeProvider.DateTimeUtcNow())
        };
    }

    public static Document Reconstitute(
        Guid id, Guid propertyId, string name, DocumentType documentType,
        DateOnly? expiryDate, UtcDateTime createdAt, ReminderSnooze? reminderSnooze = null) => new()
        {
            Id = id,
            PropertyId = propertyId,
            Name = name,
            DocumentType = documentType,
            ExpiryDate = expiryDate,
            CreatedAt = createdAt,
            ReminderSnooze = reminderSnooze
        };
}