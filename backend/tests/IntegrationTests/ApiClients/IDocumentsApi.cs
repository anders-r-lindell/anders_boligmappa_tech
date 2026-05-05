using WebApi.Features.Documents;

namespace IntegrationTests.ApiClients;

public interface IDocumentsApi
{
    [Refit.Get("/api/v1/properties/{propertyId}/documents/expiring")]
    Task<Refit.IApiResponse<DocumentListResponse>> GetExpiringByPropertyIdAsync(Guid propertyId, CancellationToken ct = default);
}
