using Domain.Entities;
using Domain.ValueObjects;

namespace Infrastructure.Persistence.DAOs;

internal static class ReminderSnoozeMappingExtensions
{
    internal static ReminderSnooze ToDomain(this ReminderSnoozeDao dao) =>
        ReminderSnooze.Reconstitute(
            dao.Id,
            dao.DocumentId,
            dao.SnoozedUntil,
            UtcDateTime.Of(DateTime.SpecifyKind(dao.SnoozedAt, DateTimeKind.Utc)));
}
