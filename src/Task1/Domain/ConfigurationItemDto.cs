using System.Text.Json.Serialization;

namespace Task1.Domain;

public record ConfigurationItemDto([property: JsonPropertyName("key")] string Key, [property: JsonPropertyName("value")] string Value);
