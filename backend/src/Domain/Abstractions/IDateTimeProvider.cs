namespace Domain.Abstractions;

public interface IDateTimeProvider
{
    DateOnly DateOnlyUtcNow();
    DateTime DateTimeUtcNow();
}
