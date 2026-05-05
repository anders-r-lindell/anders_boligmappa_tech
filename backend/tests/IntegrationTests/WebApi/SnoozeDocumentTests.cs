using FluentAssertions;
using IntegrationTests.ApiClients;
using System.Net;
using Xunit;

namespace IntegrationTests.WebApi;

/// <summary>
/// This is some basic tests - it should be completed with more tests to cover more scenarios
/// </summary>
public class SnoozeDocumentTests : IAsyncLifetime
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
    public async Task SnoozeDocument_WhenAnonymousClient_Unauthorized()
    {
        var seedData = await _factory.SeedAsync();

        var documentId = seedData.Properties[0].DocumentIds[0];

        var documentsApi = _factory.CreateDocumentsApi();

        var response = await documentsApi.SnoozeAsync(documentId);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SnoozeDocument_WhenClientIsNotDocumentOwner_Forbidden()
    {
        var seedData = await _factory.SeedAsync();

        var propertyId = seedData.Properties[0].Id;
        var clientPersonId = seedData.Properties[0].OwnerId;
        var documentId = seedData.Properties[1].DocumentIds[0];

        var documentsApi = _factory.CreateDocumentsApi(clientPersonId);

        var response = await documentsApi.SnoozeAsync(documentId);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SnoozeDocument_WhenClientIsDocumentOwner_OkWithExpectedResponse()
    {
        var mockedUtcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        _factory.DateTimeProvider.MockedValue = mockedUtcNow;

        var seedData = await _factory.SeedAsync();

        var clientPersonId = seedData.Properties[0].OwnerId;
        var documentId = seedData.Properties[0].DocumentIds[0];

        var documentsApi = _factory.CreateDocumentsApi(clientPersonId);

        var response = await documentsApi.SnoozeAsync(documentId);

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content!.ReminderSnoozedUntil.Should().Be(mockedUtcNow.AddDays(30));
    }
}
