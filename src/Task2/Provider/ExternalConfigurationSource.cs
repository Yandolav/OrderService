using Microsoft.Extensions.Configuration;

namespace Task2.Provider;

public sealed class ExternalConfigurationSource : IConfigurationSource
{
    private readonly ExternalConfigurationProvider _provider;

    public ExternalConfigurationSource(ExternalConfigurationProvider provider)
    {
        _provider = provider;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder) => _provider;
}