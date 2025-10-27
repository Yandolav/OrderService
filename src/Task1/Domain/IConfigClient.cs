namespace Task1.Domain;

public interface IConfigClient
{
    IAsyncEnumerable<ConfigurationItem> GetAllAsync(CancellationToken cancellationToken);
}