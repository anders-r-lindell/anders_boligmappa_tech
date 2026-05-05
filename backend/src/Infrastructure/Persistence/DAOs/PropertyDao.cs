namespace Infrastructure.Persistence.DAOs;

internal sealed class PropertyDao
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
}
