using FluentAssertions;
using IntegrationTests.ApiClients;
using Xunit;

namespace IntegrationTests.WebApi;

/// <summary>
/// This is some basic tests - it should be completed with more tests to cover more scenarios
/// </summary>
public class GetAllExpiringDocumentsTests : IAsyncLifetime
{
    private readonly WebApplicationFactory _factory = new();
    private IDocumentsApi _documentsApi = null!;

    public async Task InitializeAsync()
    {
        await _factory.InitialiseDatabaseAsync();
        _documentsApi = _factory.CreateDocumentsApi();
    }

    public Task DisposeAsync() => _factory.DisposeAsync().AsTask();

    [Fact]
    public async Task GetAllExpiring_WhenExpiringDocumentsExists_ReturnsExpectedDocuments()
    {
        var seedData = await _factory.SeedAsync();

        var documentsApi = _factory.CreateDocumentsApi();

        var response = await documentsApi.GetAllExpiringAsync(null, 20);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.Content!.Documents.Count.Should().Be(2);
        response.Content!.Documents.All(x => x.ReminderSnoozedUntil == null).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllExpiring_WhenDocumentListHasNextCursor_ReturnsExpectedCursor()
    {
        var seedData = await _factory.SeedAsync(4);

        var propertyId = seedData.Properties[0].Id;
        var clientPersonId = seedData.Properties[0].OwnerId;

        var documentsApi = _factory.CreateDocumentsApi(clientPersonId);

        var response1 = await documentsApi.GetAllExpiringAsync(null, 2);

        response1.IsSuccessStatusCode.Should().BeTrue();
        response1.Content!.Documents.Count.Should().Be(2);
        response1.Content.NextCursor.Should().NotBeNull();

        var response2 = await documentsApi.GetAllExpiringAsync(response1.Content.NextCursor, 2);

        response2.IsSuccessStatusCode.Should().BeTrue();
        response2.Content!.Documents.Count.Should().Be(1);
        response2.Content.NextCursor.Should().BeNull();

        var commonIds = response1.Content.Documents.Select(x => x.Id).Intersect(response2.Content.Documents.Select(x => x.Id));
        commonIds.Should().BeEmpty();
    }
}
