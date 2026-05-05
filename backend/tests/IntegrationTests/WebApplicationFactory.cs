using Domain.Abstractions;
using Domain.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Persistence.DAOs;
using IntegrationTests.ApiClients;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Refit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntegrationTests;

public class WebApplicationFactory : WebApplicationFactory<Program>
{
    // Kept open for the lifetime of the factory — in-memory SQLite drops the DB when the connection closes
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public FakeDateTimeProvider DateTimeProvider { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(
                d => d.ServiceType.Namespace != null &&
                     d.ServiceType.Namespace.Contains("Microsoft.EntityFrameworkCore") &&
                     d.ServiceType.GenericTypeArguments.Any(t => t == typeof(BoligmappaDbContext)))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<BoligmappaDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            services.AddSingleton<IDateTimeProvider>(DateTimeProvider);

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BoligmappaDbContext>();
                db.Database.Migrate();
            }
        });

        builder.UseEnvironment("Testing");
    }

    private static readonly RefitSettings _refitSettings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        })
    };

    public IDocumentsApi CreateDocumentsApi(Guid? personId = null)
    {
        var client = CreateClient();
        return RestService.For<IDocumentsApi>(client, _refitSettings);
    }

    public async Task InitialiseDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoligmappaDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task<TestSeedData> SeedAsync(int? documentsExpiringWithinDays = null)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoligmappaDbContext>();

        var owner1Id = Guid.CreateVersion7();
        var owner2Id = Guid.CreateVersion7();
        var property1Id = Guid.CreateVersion7();
        var property2Id = Guid.CreateVersion7();
        Guid[] property1DocumentIds = [Guid.CreateVersion7(), Guid.CreateVersion7()];
        Guid[] property2DocumentIds = [Guid.CreateVersion7()];

        db.Properties.AddRange(
            new PropertyDao { Id = property1Id, Address = "Street #1", OwnerId = owner1Id },
            new PropertyDao { Id = property2Id, Address = "Street #2", OwnerId = owner2Id }
        );

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        db.Documents.AddRange(
            new DocumentDao
            {
                Id = property1DocumentIds[0],
                PropertyId = property1Id,
                Name = "Document #1",
                DocumentType = DocumentType.Type1.ToString(),
                ExpiryDate = today.AddDays(documentsExpiringWithinDays ?? 5),
                CreatedAt = DateTime.UtcNow
            },
            new DocumentDao
            {
                Id = property1DocumentIds[1],
                PropertyId = property1Id,
                Name = "Document #2",
                DocumentType = DocumentType.Type2.ToString(),
                ExpiryDate = today.AddDays(documentsExpiringWithinDays ?? 365),
                CreatedAt = DateTime.UtcNow
            },
            new DocumentDao
            {
                Id = property2DocumentIds[0],
                PropertyId = property2Id,
                Name = "Document #3",
                DocumentType = DocumentType.Type3.ToString(),
                ExpiryDate = today.AddDays(documentsExpiringWithinDays ?? 7),
                CreatedAt = DateTime.UtcNow
            }
        );

        await db.SaveChangesAsync();

        return new([new(property1Id, owner1Id, property1DocumentIds), new(property2Id, owner2Id, property2DocumentIds)]);
    }

    public override async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        await base.DisposeAsync();
    }
}
