using Microsoft.Extensions.Configuration;

namespace Task2.Provider;

public class ExternalConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ExternalConfigurationProvider();
    }
}