namespace Task1.Domain;

public record QueryConfigurationsResponse(IList<ConfigurationItem> Items, string? PageToken);