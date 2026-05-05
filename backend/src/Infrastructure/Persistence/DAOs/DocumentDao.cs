namespace Infrastructure.Persistence.DAOs;

internal sealed class DocumentDao
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public DateOnly? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public PropertyDao Property { get; set; } = null!;
    public ReminderSnoozeDao? ReminderSnooze { get; set; }
}
