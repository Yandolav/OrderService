using Task1.Domain;

namespace Task1.Refit;

public class RefitConfigClient : IConfigClient
{
    private readonly IConfigurationsApi _api;

    public RefitConfigClient(IConfigurationsApi api)
    {
        _api = api;
    }

    public async Task<IReadOnlyList<ConfigurationItem>> GetAllAsync(int pageSize, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        var all = new List<ConfigurationItem>();
        string? token = null;

        do
        {
            QueryConfigurationsResponse page = await _api.GetAsync(pageSize, token, ct);
            if (page.Items.Count > 0)
            {
                all.AddRange(page.Items);
            }

            token = page.PageToken;
        }
        while (!string.IsNullOrEmpty(token));

        return all;
    }
}