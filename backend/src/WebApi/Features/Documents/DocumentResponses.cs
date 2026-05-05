namespace WebApi.Features.Documents;

public sealed record DocumentResponse(
    Guid Id,
    Guid PropertyId,
    string Name,
    string DocumentType,
    DateOnly? ExpiryDate,
    DateOnly? ReminderSnoozedUntil,
    DateTime CreatedAt);

public sealed record DocumentListResponse(IReadOnlyList<DocumentResponse> Documents);
