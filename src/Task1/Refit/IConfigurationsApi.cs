using Refit;
using Task1.Domain;

namespace Task1.Refit;

public interface IConfigurationsApi
{
    [Get("/configurations")]
    Task<QueryConfigurationsResponse> GetAsync([Query] int pageSize, [Query] string? pageToken = null, CancellationToken ct = default);
}