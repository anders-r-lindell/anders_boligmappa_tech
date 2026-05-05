using Application.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.DAOs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class DocumentRepository(BoligmappaDbContext dbContext, IDateTimeProvider dateTimeProvider) : IDocumentRepository
{
    public async Task<IReadOnlyList<Document>> GetExpiringByPropertyIdAsync(
        Guid propertyId, int withinDays, CancellationToken cancellationToken = default)
    {
        var now = dateTimeProvider.DateOnlyUtcNow();
        var end = now.AddDays(withinDays);

        var documentDaos = await dbContext.Documents
            .AsNoTracking()
            .Include(d => d.ReminderSnooze)
            .Where(d => d.PropertyId == propertyId)
            .Where(d => (d.ReminderSnooze == null || d.ReminderSnooze.SnoozedUntil < now) && (d.ExpiryDate >= now && d.ExpiryDate <= end))
            .ToListAsync(cancellationToken);

        return documentDaos.Select(d => d.ToDomain()).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Document>> GetAllExpiringAsync(
        int withinDays, Guid? cursorId = null, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var now = dateTimeProvider.DateOnlyUtcNow();
        var end = now.AddDays(withinDays);

        IQueryable<DocumentDao> query = dbContext.Documents
            .AsNoTracking()
            .Include(d => d.ReminderSnooze)
            .Where(d => (d.ReminderSnooze == null || d.ReminderSnooze.SnoozedUntil < now) && (d.ExpiryDate >= now && d.ExpiryDate <= end))
            .OrderBy(d => d.Id);

        if (cursorId.HasValue)
        {
            query = query.Where(d => d.Id > cursorId.Value);
        }

        var documentDaos = await query
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return documentDaos.Select(d => d.ToDomain()).ToList().AsReadOnly();
    }
}
