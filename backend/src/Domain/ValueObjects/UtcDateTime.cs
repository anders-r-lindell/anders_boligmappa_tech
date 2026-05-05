namespace Domain.ValueObjects;

public sealed record UtcDateTime
{
    public DateTime Value { get; }

    private UtcDateTime(DateTime value) => Value = value;

    public static UtcDateTime Of(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("DateTime must have Kind = Utc.", nameof(value));
        }
        return new UtcDateTime(value);
    }
}
