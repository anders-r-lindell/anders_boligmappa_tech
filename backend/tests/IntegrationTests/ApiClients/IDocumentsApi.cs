using Refit;
using WebApi.Features.Documents;

namespace IntegrationTests.ApiClients;

public interface IDocumentsApi
{
    [Get("/api/v1/properties/{propertyId}/documents/expiring")]
    Task<Refit.IApiResponse<DocumentListResponse>> GetExpiringByPropertyIdAsync(Guid propertyId, CancellationToken ct = default);

    [Get("/api/v1/documents/expiring")]
    Task<IApiResponse<DocumentListWithCursorResponse>> GetAllExpiringAsync([Query] Guid? cursorId, [Query] int pageSize = 20, CancellationToken ct = default);

    [Post("/api/v1/documents/{documentId}/snooze")]
    Task<IApiResponse<DocumentResponse>> SnoozeAsync(Guid documentId, CancellationToken ct = default);
}
