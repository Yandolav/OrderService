using Microsoft.Extensions.Configuration;
using Task1.Domain;

namespace Task2.Provider;

public sealed class ExternalConfigurationProvider : ConfigurationProvider
{
    public bool TryApplyItems(IEnumerable<ConfigurationItem> items)
    {
        var next = items
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => (string?)g.Last().Value, StringComparer.OrdinalIgnoreCase);

        return TryApplySnapshot(next);
    }

    private static bool IsSame(IDictionary<string, string?> current, IDictionary<string, string?> incoming)
    {
        if (current.Count != incoming.Count) return false;

        foreach (KeyValuePair<string, string?> kv in current)
        {
            if (!incoming.TryGetValue(kv.Key, out string? other)) return false;
            if (!string.Equals(kv.Value, other, StringComparison.Ordinal)) return false;
        }

        return true;
    }

    private bool TryApplySnapshot(IDictionary<string, string?> snapshot)
    {
        if (IsSame(Data, snapshot)) return false;

        Data = new Dictionary<string, string?>(snapshot, StringComparer.OrdinalIgnoreCase);
        OnReload();
        return true;
    }
}
