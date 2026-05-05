using Domain.Abstractions;

namespace Infrastructure;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateOnly DateOnlyUtcNow() => DateOnly.FromDateTime(DateTime.UtcNow);
    public DateTime DateTimeUtcNow() => DateTime.UtcNow;
}
