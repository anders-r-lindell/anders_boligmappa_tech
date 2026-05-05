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

    public async Task UpdateReminderSnoozeAsync(Document document, CancellationToken cancellationToken = default)
    {
        var documentDao = await dbContext.Documents
            .Include(d => d.ReminderSnooze)
            .FirstOrDefaultAsync(d => d.Id == document.Id, cancellationToken);

        if (documentDao is null)
        {
            return;
        }

        if (document.ReminderSnooze is not null)
        {
            if (documentDao.ReminderSnooze is null)
            {
                documentDao.ReminderSnooze = new ReminderSnoozeDao
                {
                    Id = document.ReminderSnooze.Id,
                    DocumentId = document.Id,
                    SnoozedUntil = document.ReminderSnooze.SnoozedUntil,
                    SnoozedAt = document.ReminderSnooze.SnoozedAt.Value
                };

                dbContext.Entry(documentDao.ReminderSnooze).State = EntityState.Added; // Current not sure why this is required, need to investigate a bit more
            }
            else
            {
                documentDao.ReminderSnooze.SnoozedUntil = document.ReminderSnooze.SnoozedUntil;
                documentDao.ReminderSnooze.SnoozedAt = document.ReminderSnooze.SnoozedAt.Value;

                dbContext.Entry(documentDao.ReminderSnooze).State = EntityState.Modified; // Current not sure why this is required, need to investigate a bit more
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid?> GetOwnerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Documents
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => d.Property.OwnerId)
            .Cast<Guid?>()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var documentDao = await dbContext.Documents
            .AsNoTracking()
            .Include(d => d.ReminderSnooze)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        return documentDao?.ToDomain();
    }
}
