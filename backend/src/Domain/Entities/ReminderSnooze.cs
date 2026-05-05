using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class ReminderSnooze
{
    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public DateOnly SnoozedUntil { get; private set; }
    public UtcDateTime SnoozedAt { get; private set; }

    private ReminderSnooze() { }

    private const int SnoozeDurationDays = 30;

    internal static ReminderSnooze Create(Guid documentId, IDateTimeProvider dateTimeProvider) => new()
    {
        Id = Guid.CreateVersion7(),
        DocumentId = documentId,
        SnoozedUntil = dateTimeProvider.DateOnlyUtcNow().AddDays(SnoozeDurationDays),
        SnoozedAt = UtcDateTime.Of(dateTimeProvider.DateTimeUtcNow())
    };

    internal void Extend(IDateTimeProvider dateTimeProvider)
    {
        SnoozedUntil = dateTimeProvider.DateOnlyUtcNow().AddDays(SnoozeDurationDays);
        SnoozedAt = UtcDateTime.Of(dateTimeProvider.DateTimeUtcNow());
    }

    public static ReminderSnooze Reconstitute(
        Guid id, Guid documentId, DateOnly snoozedUntil, UtcDateTime snoozedAt) => new()
        {
            Id = id,
            DocumentId = documentId,
            SnoozedUntil = snoozedUntil,
            SnoozedAt = snoozedAt
        };
}
