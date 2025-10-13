using System.Text.Json;
using Task1.Domain;

namespace Task1.Http;

public class HttpConfigClient : IConfigClient
{
    private readonly IHttpClientFactory _factory;

    public HttpConfigClient(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<ConfigurationItem>> GetAllAsync(int pageSize, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        HttpClient client = _factory.CreateClient(ConfigClientDefaults.HttpClientName);

        var all = new List<ConfigurationItem>();
        string? token = null;

        do
        {
            string url = $"configurations?pageSize={pageSize}" + (token is null ? string.Empty : $"&pageToken={Uri.EscapeDataString(token)}");

            string json = await client.GetStringAsync(url, ct);
            QueryConfigurationsResponse? page = JsonSerializer.Deserialize<QueryConfigurationsResponse>(json);

            if (page?.Items is { Count: > 0 })
            {
                all.AddRange(page.Items);
            }

            token = page?.PageToken;
        }
        while (!string.IsNullOrEmpty(token));

        return all;
    }
}