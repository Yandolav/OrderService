using System.Text.Json.Serialization;

namespace Task1.Domain;

public record QueryConfigurationsResponse([property: JsonPropertyName("items")] IList<ConfigurationItemDto> Items, [property: JsonPropertyName("pageToken")] string? PageToken);