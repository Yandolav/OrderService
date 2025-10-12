using System.Text.Json.Serialization;

namespace Task1.Domain;

public record ConfigurationItem([property: JsonPropertyName("key")] string Key, [property: JsonPropertyName("value")] string Value);