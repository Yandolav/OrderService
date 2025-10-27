using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using Task1.Domain;

namespace Task1.Refit;

public class RefitConfigClient : IConfigClient
{
    private readonly IConfigurationsApi _api;

    private readonly IOptionsMonitor<ConfigClientOptions> _options;

    public RefitConfigClient(IConfigurationsApi api, IOptionsMonitor<ConfigClientOptions> options)
    {
        _api = api;
        _options = options;
    }

    public async IAsyncEnumerable<ConfigurationItem> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string? token = null;

        do
        {
            QueryConfigurationsResponse page = await _api.GetAsync(_options.CurrentValue.PageSize, token, cancellationToken);
            if (page.Items.Count > 0)
            {
                foreach (ConfigurationItemDto itemDto in page.Items)
                    yield return new ConfigurationItem(itemDto.Key, itemDto.Value);
            }

            token = page.PageToken;
        }
        while (!string.IsNullOrEmpty(token));
    }
}