using FluentAssertions;
using Xunit;

namespace IntegrationTests.WebApi;

/// <summary>
/// This is some basic tests - it should be completed with more tests to cover more scenarios
/// </summary>
public class GetExpiringDocumentsByPropertyIdTests : IAsyncLifetime
{
    private readonly WebApplicationFactory _factory = new();

    public async Task InitializeAsync()
    {
        await _factory.InitialiseDatabaseAsync();
    }

    public Task DisposeAsync() => _factory.DisposeAsync().AsTask();

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(89, true)]
    [InlineData(90, true)]
    [InlineData(91, false)]
    public async Task GetExpiringByPropertyId_WhenDocumentHasExpirationDate_ReturnsExpectedDocuments(int documentsExpiringWithinDays, bool documentsIsExpiring)
    {
        var seedData = await _factory.SeedAsync(documentsExpiringWithinDays);

        var propertyId = seedData.Properties[0].Id;
        var clientPersonId = seedData.Properties[0].OwnerId;

        var documentsApi = _factory.CreateDocumentsApi(clientPersonId);

        var response = await documentsApi.GetExpiringByPropertyIdAsync(propertyId);

        response.IsSuccessStatusCode.Should().BeTrue();
        if (documentsIsExpiring)
        {
            response.Content!.Documents.Should().Contain(x => x.Id == seedData.Properties[0].DocumentIds[0]);
            response.Content!.Documents.Should().Contain(x => x.Id == seedData.Properties[0].DocumentIds[1]);
            response.Content!.Documents.All(x => x.ReminderSnoozedUntil == null).Should().BeTrue();
        }
        else
        {
            response.Content!.Documents.Should().BeEmpty();
        }
    }
}
