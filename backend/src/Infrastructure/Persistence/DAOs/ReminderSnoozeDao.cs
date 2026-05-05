namespace Infrastructure.Persistence.DAOs;

internal sealed class ReminderSnoozeDao
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public DateOnly SnoozedUntil { get; set; }
    public DateTime SnoozedAt { get; set; }
    public DocumentDao Document { get; set; } = null!;
}
