using Domain.Abstractions;

namespace IntegrationTests;

public sealed class FakeDateTimeProvider : IDateTimeProvider
{
    public DateOnly MockedValue { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public DateOnly DateOnlyUtcNow() => MockedValue;
    public DateTime DateTimeUtcNow() => MockedValue.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
}
