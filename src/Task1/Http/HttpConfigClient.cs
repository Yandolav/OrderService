using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Task1.Domain;

namespace Task1.Http;

public class HttpConfigClient : IConfigClient
{
    private readonly IHttpClientFactory _factory;

    private readonly IOptionsMonitor<ConfigClientOptions> _options;

    public HttpConfigClient(IHttpClientFactory factory, IOptionsMonitor<ConfigClientOptions> options)
    {
        _factory = factory;
        _options = options;
    }

    public async IAsyncEnumerable<ConfigurationItem> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        HttpClient client = _factory.CreateClient();
        string? token = null;

        do
        {
            string url = $"configurations?pageSize={_options.CurrentValue.PageSize}" + (token is null ? string.Empty : $"&pageToken={Uri.EscapeDataString(token)}");

            string json = await client.GetStringAsync(url, cancellationToken);
            QueryConfigurationsResponse? page = JsonSerializer.Deserialize<QueryConfigurationsResponse>(json);

            if (page?.Items is { Count: > 0 })
            {
                foreach (ConfigurationItemDto itemDto in page.Items)
                    yield return new ConfigurationItem(itemDto.Key, itemDto.Value);
            }

            token = page?.PageToken;
        }
        while (!string.IsNullOrEmpty(token));
    }
}