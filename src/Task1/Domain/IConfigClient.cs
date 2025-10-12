namespace Task1.Domain;

public interface IConfigClient
{
    Task<IReadOnlyList<ConfigurationItem>> GetAllAsync(int pageSize, CancellationToken ct = default);
}