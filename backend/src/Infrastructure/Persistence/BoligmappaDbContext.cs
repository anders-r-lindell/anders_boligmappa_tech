using Infrastructure.Persistence.DAOs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class BoligmappaDbContext(DbContextOptions<BoligmappaDbContext> options) : DbContext(options)
{
    internal DbSet<PropertyDao> Properties => Set<PropertyDao>();
    internal DbSet<DocumentDao> Documents => Set<DocumentDao>();
    internal DbSet<ReminderSnoozeDao> ReminderSnoozes => Set<ReminderSnoozeDao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PropertyDao>(entity =>
        {
            entity.ToTable("Properties");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Address).IsRequired().HasMaxLength(256);
            entity.Property(p => p.OwnerId).IsRequired();
        });

        modelBuilder.Entity<DocumentDao>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(256);
            entity.Property(d => d.DocumentType).IsRequired().HasMaxLength(50);
            entity.Property(d => d.ExpiryDate);
            entity.Property(d => d.CreatedAt).IsRequired();
            entity.HasIndex(d => d.ExpiryDate);
            entity.HasIndex(d => d.PropertyId);
            entity.HasOne(d => d.Property)
                  .WithMany()
                  .HasForeignKey(d => d.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReminderSnoozeDao>(entity =>
        {
            entity.ToTable("ReminderSnoozes");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.SnoozedUntil).IsRequired();
            entity.HasIndex(r => r.SnoozedUntil);
            entity.Property(r => r.SnoozedAt).IsRequired();
            entity.HasOne(r => r.Document)
                  .WithOne(d => d.ReminderSnooze)
                  .HasForeignKey<ReminderSnoozeDao>(r => r.DocumentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
