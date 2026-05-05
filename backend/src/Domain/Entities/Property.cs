namespace Domain.Entities;

public class Property
{
    public Guid Id { get; private set; }
    public string Address { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }

    private Property() { }

    public static Property Create(string address, Guid ownerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        if (address.Length > 256)
        {
            throw new ArgumentException("Address must not exceed 256 characters.", nameof(address));
        }
        ArgumentOutOfRangeException.ThrowIfEqual(ownerId, Guid.Empty);

        return new Property
        {
            Id = Guid.CreateVersion7(),
            Address = address,
            OwnerId = ownerId
        };
    }

    public static Property Reconstitute(Guid id, string address, Guid ownerId) => new()
    {
        Id = id,
        Address = address,
        OwnerId = ownerId
    };
}
